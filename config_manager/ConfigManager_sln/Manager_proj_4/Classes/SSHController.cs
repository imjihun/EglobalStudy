using Manager_proj_4.UserControls;
using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Threading;

namespace Manager_proj_4.Classes
{
	public static class SSHController
	{
		//static string[] IGNORE_FILENAME = new string[] {".", ".."};
		public static SshClient ssh;
		public static SftpClient sftp;
		public static ShellStream shell_stream;
		public static StreamReader shell_stream_reader;
		public static StreamWriter shell_stream_writer;
		public static DispatcherTimer shell_stream_read_timer;

		#region linux ssh, sftp connection
		public static string add_path_config_upload = "/var/conf/";
		public static string add_path_run_cofile = "/bin/cofile";
		public static CofileOption selected_type = CofileOption.file;
		private static string[] cofile_option = new string[] {"file", "sam", "tail" };
		public static string MakeCommandRunCofile(string path_run, CofileOption type, bool isEncrypt, string path, string configname)
		{
			string str = path_run;
			str += " " + cofile_option[(int)type];

			if(isEncrypt)
				str += " -e";
			else
				str += " -d";

			switch(type)
			{
				case CofileOption.sam:
					str += " -i " + path;
					if(isEncrypt)
						str += " -o " + path + ".coenc";
					else
						str += " -o " + path + ".codec";
					break;
				case CofileOption.file:
				case CofileOption.tail:
					str += " -f " + path;
					break;
			}

			if(configname != null)
				str += " -c " + configname;

			return str;
		}
		private static string LoadEnvCoHome()
		{
			if(!SSHController.sendCommand(cmd_get_co_home))
				return null;
			string env_co_home = readCoHomeBlocking(cmd_get_co_home);

			if(env_co_home == null || env_co_home == "")
			{
				Log.PrintError("not defined $CO_HOME\r", "load $CO_HOME", WindowMain.current.richTextBox_status);
				return null;
			}
			Log.Print("$CO_HOME = " + env_co_home, "load $CO_HOME");
			return env_co_home;
		}
		//public static string remote_directory_upload = "/home/cofile/bin";

		static bool CheckConnection(BaseClient client, string ip, int port, string id)
		{
			if((client == null || !client.IsConnected)
					|| (client.ConnectionInfo.Host != ip
						|| client.ConnectionInfo.Port != port
						|| client.ConnectionInfo.Username != id))
				return false;

			return true;
		}
		static bool ReConnect(int timeout_ms = -1)
		{
			if(ServerList.selected_serverinfo_textblock == null)
				return false;

			string ip = ServerList.selected_serverinfo_textblock.serverinfo.ip;
			string id = ServerList.selected_serverinfo_textblock.serverinfo.id;
			string password = ServerList.selected_serverinfo_textblock.serverinfo.password;
			int port = 22;

			try
			{
				if(!CheckConnection(sftp, ip, port, id) || !CheckConnection(ssh, ip, port, id))
				{
					TextRange txt = new TextRange(WindowMain.current.richTextBox_status.Document.ContentStart, WindowMain.current.richTextBox_status.Document.ContentEnd);
					txt.Text = "";

					sftp = new SftpClient(ip, port, id, password);
					if(timeout_ms != -1)
						sftp.ConnectionInfo.Timeout = new TimeSpan(0, 0, 0, 0, timeout_ms);
					sftp.Connect();
					ssh = new SshClient(ip, port, id, password);
					if(timeout_ms != -1)
						ssh.ConnectionInfo.Timeout = new TimeSpan(0, 0, 0, 0, timeout_ms);
					ssh.Connect();

					if(ssh.IsConnected)
					{
						shell_stream = ssh.CreateShellStream("customCommand", 80, 24, 800, 600, 4096);
						shell_stream_reader = new StreamReader(shell_stream);
						shell_stream_writer = new StreamWriter(shell_stream);
					}

					if(shell_stream_read_timer == null)
					{
						shell_stream_read_timer = new DispatcherTimer();
						shell_stream_read_timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
						shell_stream_read_timer.Tick += Shell_stream_read_timer_Tick;
					}
					//shell_stream_read_timer.Stop();
					//shell_stream_read_timer.Start();
					Log.Print(ip + " / " + port + " / " + id, "ReConnect"/*, test4.m_wnd.richTextBox_status*/);

					readMessageBlocking(null);
					return true;
				}
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "ReConnect", WindowMain.current.richTextBox_status);
				return false;
			}
			return true;
		}
		private static SftpFile[] _PollListInDirectory(string Path)
		{
			IEnumerable<SftpFile> files = null;
			try
			{
				files = sftp.ListDirectory(Path).OrderBy(x => x.FullName);
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "_PollListInDirectory", WindowMain.current.richTextBox_status);
			}

			Log.Print(Path, "_PollListInDirectory"/*, test4.m_wnd.richTextBox_status*/);
			return files.ToArray();
		}
		public static SftpFile[] PollListInDirectory(string Path)
		{
			if(!SSHController.ReConnect(1000))
				return null;
			if(Path == null)
				Path = LinuxTreeViewItem.root.Path = sftp.WorkingDirectory;

			return _PollListInDirectory(Path);
		}

		static string cmd_get_co_home = "echo $CO_HOME";
		static string current_cofile_name = "";
		static bool UploadFile(string local_path, string remote_directory)
		{
			//LinuxTreeViewItem.ReconnectServer();
			//LinuxTreeViewItem.ReConnect();
			if(!SSHController.ReConnect(1000))
				return false;

			try
			{
				FileInfo fi = new FileInfo(local_path);
				if(fi.Exists)
				{
					FileStream fs = File.Open(local_path, FileMode.Open, FileAccess.Read);
					sftp.UploadFile(fs, remote_directory + fi.Name);
					Log.Print(fi.Name + " => " + remote_directory + fi.Name, "upload file"/*, test4.m_wnd.richTextBox_status*/);
					SSHController.current_cofile_name = fi.Name;
					fs.Close();
				}
				else
				{
					Log.PrintError("check the config file path", "upload file", WindowMain.current.richTextBox_status);
					return false;
				}
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "UploadFile", WindowMain.current.richTextBox_status);
				return false;
			}
			return true;
		}
		private static bool sendCommand(string command)
		{
			//LinuxTreeViewItem.ReconnectServer();
			//LinuxTreeViewItem.ReConnect();
			if(!SSHController.ReConnect(1000))
				return false;

			try
			{
				// send
				if(shell_stream_reader != null)
				{
					shell_stream_writer.Write(command);
					shell_stream_writer.Write("\n");
					shell_stream_writer.Flush();
					Log.Print(command, "send command"/*, test4.m_wnd.richTextBox_status*/);
					return true;
				}
			}
			catch(Exception ex)
			{
				Log.PrintError(ex.Message, "send command", WindowMain.current.richTextBox_status);
			}
			return false;
		}
		//private static void SendCommandOnce(string command)
		//{
		//	SshCommand com = ssh.CreateCommand(command);
		//	string str = com.Execute();
		//	Console.WriteLine("\n\n\n" + str + "\n\n\n");
		//	Console.WriteLine(com.Error);
		//	Console.WriteLine(com.ExitStatus);
		//	Log.ViewMessage(com.Result, "test", WindowMain.current.richTextBox_status);
		//}
		public static void sendCofileCommand(LinuxTreeViewItem[] selected_list, bool isEncrypt)
		{
			string env_co_home = LoadEnvCoHome();
			string remote_directory = env_co_home + add_path_config_upload;

			if(UploadFile(Cofile.current.Selected_config_file_path, remote_directory))
			{
				for(int i = 0; i < selected_list.Length; i++)
				{
					//string str = LinuxTreeViewItem.selected_list[i].Path.Substring(env_co_home.Length);
					if(!selected_list[i].isDirectory)
					{
						string send_cmd = MakeCommandRunCofile(env_co_home + add_path_run_cofile, selected_type, isEncrypt, selected_list[i].Path, remote_directory + current_cofile_name);
						//SendCommandOnce(send_cmd);
						sendCommand(send_cmd);
						//string ret = readMessageBlocking(send_cmd);
						string ret = readCofileMessageBlocking();
						string caption = isEncrypt ? "Encrypt" : "Decrypt";
						Log.ViewMessage(ret + "\n", caption, WindowMain.current.richTextBox_status);
					}
				}
			}
		}

		static string read_line_ssh = "";
		static string[] view_log_start = new string[] {"inform"};
		static string[] view_error_start = new string[] {"error" };
		public static string view_message_caption = "";
		private static string read()
		{
			int size_buffer = 4096;
			char[] buffer = new char[size_buffer];
			try
			{
				int cnt = shell_stream_reader.Read(buffer, 0, size_buffer);

				read_line_ssh += new string(buffer, 0, cnt);/* Encoding.UTF8.GetString(buffer, 0, cnt);*/
				if(read_line_ssh.Length > 0)
				{
					//Log.Print(read_line_ssh, "read"/*, test4.m_wnd.richTextBox_status*/);
					int idx_newline = 0;
					if((idx_newline = read_line_ssh.IndexOf('\n')) >= 0)
					{
						string line = read_line_ssh.Substring(0, idx_newline);
						int i;
						for(i = 0; i < view_log_start.Length; i++)
						{
							if(line.Length > view_log_start[i].Length && line.Substring(0, view_log_start[i].Length).ToLower() == view_log_start[i])
							{
								Log.ViewMessage(line, view_message_caption, WindowMain.current.richTextBox_status);
							}
						}
						if(i == view_log_start.Length)
						{
							for(i = 0; i < view_error_start.Length; i++)
							{
								if(line.Length > view_error_start[i].Length && line.Substring(0, view_error_start[i].Length).ToLower() == view_error_start[i])
									Log.PrintError(line, view_message_caption, WindowMain.current.richTextBox_status);
							}
							if(i == view_error_start.Length)
								Log.ViewUndefine(line, "__undefined][" + view_message_caption, WindowMain.current.richTextBox_status);
						}

						Log.Print(line, "Read");
						read_line_ssh = read_line_ssh.Substring(idx_newline + 1);
					}
				}

			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "read", WindowMain.current.richTextBox_status);
			}
			return read_line_ssh;
		}
		private static string readMessageBlocking(string cmd_send)
		{
			// 수정중..
			shell_stream_read_timer.Stop();

			int size_buffer = 4096;
			char[] buffer = new char[size_buffer];
			string message = "";
			string ret_message = null;
			try
			{
				// '$' 의 값이 나올떄까지 리드
				string str_recv = "";
				while(str_recv.Length < 1 || str_recv.LastIndexOf('$') < 0)
				{
					int cnt = shell_stream_reader.Read(buffer, 0, size_buffer);
					str_recv += new string(buffer, 0, cnt);/* Encoding.UTF8.GetString(buffer, 0, cnt);*/
				}
				int l = str_recv.Length;
				Log.Print(str_recv, "readMessageBlocking][" + l);

				if(cmd_send == null || cmd_send == "")
					return cmd_send;

				// 명령어 전까지 자르기.(dummy 제거)
				if(str_recv.LastIndexOf(cmd_send) >= 0)
					message = str_recv.Substring(str_recv.LastIndexOf(cmd_send));

				// 명령어 라인과 
				// 마지막라인 '[~] $' 제거
				if(message.IndexOf('\n') >= 0 && message.IndexOf('\n') + 1 < message.Length)
				{
					int idx = message.IndexOf('\n');
					int len = message.LastIndexOf('\n') - idx;
					if(len > 0)
						ret_message = message.Substring(idx + 1, len);
				}


			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "readMessageBlocking", WindowMain.current.richTextBox_status);
			}

			shell_stream_read_timer.Start();
			return ret_message;
		}
		private static string readCofileMessageBlocking()
		{
			shell_stream_read_timer.Stop();

			int size_buffer = 4096;
			char[] buffer = new char[size_buffer];
			string message = "";
			string ret_message = null;
			try
			{
				// '$' 의 값이 나올떄까지 리드
				string str_recv = "";
				while(str_recv.Length < 1 || str_recv.LastIndexOf('$') < 0)
				{
					int cnt = shell_stream_reader.Read(buffer, 0, size_buffer);
					str_recv += new string(buffer, 0, cnt);/* Encoding.UTF8.GetString(buffer, 0, cnt);*/
				}
				int l = str_recv.Length;
				Log.Print(str_recv, "readMessageBlocking][" + l);

				// Copyright 부분 다음 라인까지 자르기
				string Copyright = "Copyright";
				if(str_recv.LastIndexOf(Copyright) >= 0)
				{
					str_recv = str_recv.Substring(str_recv.LastIndexOf(Copyright));
					if(str_recv.IndexOf('\n') >= 0 && str_recv.Length > str_recv.IndexOf('\n') + 1)
						str_recv = str_recv.Substring(str_recv.IndexOf('\n') + 1);
					if(str_recv.IndexOf('\n') >= 0 && str_recv.Length > str_recv.IndexOf('\n') + 1)
						message = str_recv.Substring(str_recv.IndexOf('\n') + 1);
				}
				
				// 마지막라인 '[~] $' 제거
				if(message.IndexOf('\n') >= 0 && message.IndexOf('\n') + 1 < message.Length)
				{
					int len = message.LastIndexOf('\n');
					if(len > 0)
						ret_message = message.Substring(0, len + 1);
				}
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "readMessageBlocking", WindowMain.current.richTextBox_status);
			}

			shell_stream_read_timer.Start();
			return ret_message;
		}
		private static string readCoHomeBlocking(string cmd_send)
		{
			cmd_send += "\r\n";
			int cmd_length = cmd_send.Length;
			shell_stream_read_timer.Stop();
			int size_buffer = 4096;
			char[] buffer = new char[size_buffer];
			string prevstr = "";
			string line = "";
			string env_co_home = null;
			try
			{
				while((prevstr.Length < cmd_length || prevstr.IndexOf(cmd_send) < 0 || prevstr.Length <= prevstr.IndexOf(cmd_send) + cmd_length))
				{
					int cnt = shell_stream_reader.Read(buffer, 0, size_buffer);
					prevstr += new string(buffer, 0, cnt);/* Encoding.UTF8.GetString(buffer, 0, cnt);*/
				}
				int startline = 0;
				startline = prevstr.IndexOf(cmd_send);
				line = prevstr.Substring(startline + cmd_length);
				if(prevstr.IndexOf(cmd_send) > 2 && prevstr[startline - 2] != '$')
				{
					prevstr = line;
					while((prevstr.Length < cmd_length || prevstr.IndexOf(cmd_send) < 0 || prevstr.Length <= prevstr.IndexOf(cmd_send) + cmd_length))
					{
						int cnt = shell_stream_reader.Read(buffer, 0, size_buffer);
						prevstr += new string(buffer, 0, cnt);/* Encoding.UTF8.GetString(buffer, 0, cnt);*/
					}
					startline = 0;
					startline = prevstr.IndexOf(cmd_send);
					line = prevstr.Substring(startline + cmd_length);
				}

				//bool bGet_end = false;
				while(line.Length < cmd_length || line.IndexOf('\n') < 0)
				{
					int cnt = shell_stream_reader.Read(buffer, 0, size_buffer);
					line += new string(buffer, 0, cnt);/* Encoding.UTF8.GetString(buffer, 0, cnt);*/
				}
				string str = line.ToString();
				int idx_end = str.IndexOf('\r');

				if(idx_end > 0)
				{
					env_co_home = str.Substring(0, idx_end);
				}
				else
					env_co_home = "";

				Log.Print(str, "readCoHomeBlocking");

			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "readCoHomeBlocking", WindowMain.current.richTextBox_status);
			}

			shell_stream_read_timer.Start();
			return env_co_home;
		}
		private static string readInit(string cmd_send)
		{
			int cmd_length = cmd_send.Length + 2;
			shell_stream_read_timer.Stop();
			int size_buffer = 4096;
			char[] buffer = new char[size_buffer];
			StringBuilder read = new StringBuilder();
			try
			{
				//bool bGet_end = false;
				while(read.Length < cmd_length || read.ToString().IndexOf('\n', cmd_length) < 0)
				{
					int cnt = shell_stream_reader.Read(buffer, 0, size_buffer);
					read.Append(new string(buffer, 0, cnt)/* Encoding.UTF8.GetString(buffer, 0, cnt)*/);
				}
				string str = read.ToString().Substring(cmd_length + 2);

				Log.Print(str, "readInit");

			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "readInit", WindowMain.current.richTextBox_status);
			}

			shell_stream_read_timer.Start();
			return read.ToString();
		}
		private static void Shell_stream_read_timer_Tick(object sender, EventArgs e)
		{
			if(shell_stream_reader != null)
			{
				string str = read();
				if(str.Length > 0)
					;//LinuxTreeViewItem.ViewLog("[read] " + str);
			}
		}
		#endregion

		#region DownLoad
		private static bool downloadFile(string local_path_folder, string local_file_name, string remote_path_file, string filename = null)
		{
			//LinuxTreeViewItem.ReconnectServer();
			//LinuxTreeViewItem.ReConnect();
			if(!SSHController.ReConnect(1000))
				return false;

			try
			{
				if(filename == null)
				{
					string[] split = remote_path_file.Split('/');
					filename = split[split.Length - 1];
				}

				string local;
				if(local_file_name != null)
					local = local_path_folder + local_file_name;
				else
					local = local_path_folder + filename;

				Directory.CreateDirectory(local_path_folder);

				FileStream fs = new FileStream(local, FileMode.Create);
				sftp.DownloadFile(remote_path_file, fs);
				Log.Print(filename + " => " + local, "downloadFile"/*, test4.m_wnd.richTextBox_status*/);
				fs.Close();
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "downloadFile", WindowMain.current.richTextBox_status);
				return false;
			}
			return true;
		}

		static string db_name = "cofile.db";
		static string add_path_database = "/var/data/" + db_name;
		public static string GetDataBase(string local_folder, string local_file)
		{
			string remote_directory = LoadEnvCoHome();
			if(remote_directory == null)
				return null;

			string remote_path_file = remote_directory + add_path_database;
			if(downloadFile(local_folder, local_file, remote_path_file, db_name))
			{
				return local_folder + local_file;
			}
			return null;
		}
		#endregion
	}
}
