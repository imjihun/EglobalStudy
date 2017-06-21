using CofileUI.UserControls;
using CofileUI.Windows;
using Newtonsoft.Json.Linq;
using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;

namespace CofileUI.Classes
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


		public static bool CheckConnection(string ip, int port)
		{
			if(CheckConnection(sftp, ip, port) && CheckConnection(ssh, ip, port))
				return true;

			return false;
		}
		static bool CheckConnection(BaseClient client, string ip, int port, string id)
		{
			if((client == null || !client.IsConnected)
					|| (client.ConnectionInfo.Host != ip
						|| client.ConnectionInfo.Port != port
						|| client.ConnectionInfo.Username != id))
				return false;

			return true;
		}
		static bool CheckConnection(BaseClient client, string ip, int port)
		{
			if((client == null || !client.IsConnected)
					|| (client.ConnectionInfo.Host != ip
						|| client.ConnectionInfo.Port != port))
				return false;

			return true;
		}

		const int NO_TIMEOUT = -1;
		static int timeout_connect_ms = NO_TIMEOUT;
		public static bool ReConnect(int timeout_ms = NO_TIMEOUT)
		{
			if(ServerList.selected_serverinfo_textblock == null)
				return false;

			string ip = ServerList.selected_serverinfo_textblock.serverinfo.ip;
			//string id = ServerList.selected_serverinfo_textblock.serverinfo.id;
			//string password = ServerList.selected_serverinfo_textblock.serverinfo.password;
			int port = ServerList.selected_serverinfo_textblock.serverinfo.port;

			try
			{
				//if(!CheckConnection(sftp, ip, port, id) || !CheckConnection(ssh, ip, port, id))
				if(!CheckConnection(ip, port))
				{
					if(WindowMain.current != null)
						WindowMain.current.Changed_server_name = "";


					TextRange txt = new TextRange(Status.current.richTextBox_status.Document.ContentStart, Status.current.richTextBox_status.Document.ContentEnd);
					txt.Text = "";
					
					Window_LogIn wl = new Window_LogIn();

					Point pt = ServerList.selected_serverinfo_textblock.PointToScreen(new Point(0, 0));
					wl.Left = pt.X;
					wl.Top = pt.Y;

					if(wl.ShowDialog() != true)
						return false;

					string id = wl.Id;
					string password = wl.Password;
					//string id = "cofile";
					//string password = "cofile";
					ServerList.selected_serverinfo_textblock.serverinfo.id = id;

					ssh = new SshClient(ip, port, id, password);
					if(timeout_ms != NO_TIMEOUT)
						ssh.ConnectionInfo.Timeout = new TimeSpan(0, 0, 0, 0, timeout_ms);
					ssh.Connect();
					sftp = new SftpClient(ip, port, id, password);
					if(timeout_ms != NO_TIMEOUT)
						sftp.ConnectionInfo.Timeout = new TimeSpan(0, 0, 0, 0, timeout_ms);
					sftp.Connect();

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
					shell_stream_read_timer.Stop();
					shell_stream_read_timer.Start();
					Log.PrintConsole(ip + " / " + port + " / " + id, "ReConnect"/*, test4.m_wnd.richTextBox_status*/);

					readDummyMessageBlocking(null);
					//for(int i = 0; i < 500; i++)
					//	Log.PrintConsole(read(), "Lost SSH Stream");

					if(WindowMain.current != null)
						WindowMain.current.Changed_server_name = ServerList.selected_serverinfo_textblock.serverinfo.name;
					return true;
				}
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "ReConnect", Status.current.richTextBox_status);
				return false;
			}
			return true;
		}
		public static bool DisConnect()
		{
			if(ssh != null)
				ssh.Disconnect();
			if(sftp != null)
				sftp.Disconnect();
			if(WindowMain.current != null)
				WindowMain.current.Changed_server_name = "";
			return true;
		}
		private static bool CreateDirectory(string remote_dir_path)
		{
			string _path = "/";
			try
			{
				string[] split = remote_dir_path.Split('/');

				for(int i = 0; i < split.Length; i++)
				{
					_path += split[i] + "/";
					if(!sftp.Exists(_path))
						sftp.CreateDirectory(_path);
				}
				return true;
			}
			catch(Exception ex)
			{
				Log.PrintError(ex.Message + " [" + _path + "]", "CreateDirectory_Remote");
				return false;
			}
		}
		public static string UploadFile(string local_path, string remote_directory, string remote_backup_dir = null)
		{
			//LinuxTreeViewItem.ReconnectServer();
			//LinuxTreeViewItem.ReConnect();
			if(!SSHController.ReConnect(timeout_connect_ms))
				return null;
			

			string remote_file_path = null;
			try
			{
				FileInfo fi = new FileInfo(local_path);
				if(fi.Exists)
				{
					FileStream fs = File.Open(local_path, FileMode.Open, FileAccess.Read);
					remote_file_path = remote_directory + fi.Name;
					//if(isOverride)
					//{
					//	sftp.UploadFile(fs, remote_file_path);
					//	Log.PrintConsole(fi.Name + " => " + remote_file_path, "upload file"/*, test4.m_wnd.richTextBox_status*/);
					//}
					if(remote_backup_dir != null && sftp.Exists(remote_file_path))
					{
						if(CreateDirectory(remote_backup_dir))
						{
							DateTime dt;
							//dt = DateTime.Now;

							// 원래는 서버시간으로 생성해야함.
							// 서버마다 시간을 알수있는 함수가 다를수 있으므로 sftp를 사용
							// 위 if 문의 sftp.Exists(remote_file_path) 에서 엑세스한 시간을 가져옴.
							dt = sftp.GetLastAccessTime(remote_file_path);

							// '파일 명'.'연도'.'달'.'날짜'.'시간'.'분'.'초'.backup 형식으로 백업파일 생성
							string remote_backup_file = remote_backup_dir + fi.Name + dt.ToString(".yyyy.MM.dd.hh.mm.ss") + ".backup";
							ssh.RunCommand(@"cp " + remote_file_path + " " + remote_backup_file);
						}
						else
						{
							fs.Close();
							Log.PrintError("Create Directory Error", "upload file");
							return null;
						}

					}
					if(CreateDirectory(remote_directory))
					{
						sftp.UploadFile(fs, remote_file_path, true);
						Log.PrintConsole(fi.Name + " => " + remote_file_path, "upload file"/*, test4.m_wnd.richTextBox_status*/);
					}
					fs.Close();
				}
				else
				{
					Log.PrintError("Not Exist File", "upload file", Status.current.richTextBox_status);
					return null;
				}
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "UploadFile", Status.current.richTextBox_status);
				return null;
			}
			return remote_file_path;
		}
		public static bool downloadFile(string local_path_folder, string remote_path_file, string local_file_name = null, string remote_filename = null)
		{
			//LinuxTreeViewItem.ReconnectServer();
			//LinuxTreeViewItem.ReConnect();
			if(!SSHController.ReConnect(timeout_connect_ms))
				return false;

			try
			{
				string local;
				if(local_file_name != null)
					local = local_path_folder + local_file_name;
				else
				{
					if(remote_filename == null)
					{
						string[] split = remote_path_file.Split('/');
						remote_filename = split[split.Length - 1];
					}
					local = local_path_folder + remote_filename;
				}

				FileContoller.CreateDirectory(local_path_folder);

				FileStream fs = new FileStream(local, FileMode.Create);
				sftp.DownloadFile(remote_path_file, fs);
				Log.PrintConsole(remote_path_file + " => " + local, "downloadFile"/*, test4.m_wnd.richTextBox_status*/);
				fs.Close();
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "downloadFile", Status.current.richTextBox_status);
				return false;
			}
			return true;
		}
		public static bool moveFileToLocal(string local_path_folder, string remote_path_file, string local_file_name = null)
		{
			if(!downloadFile(local_path_folder, remote_path_file, local_file_name))
				return false;
			ssh.RunCommand("rm -rf " + remote_path_file);
			//sendCommand("rm -rf " + remote_path_file);
			readDummyMessageBlocking(null);

			return true;
		}
		private static bool downloadDirectory(string local_folder_path, string remote_directory_path, Regex filter_file = null, Regex filter_except_dir = null)
		{
			//LinuxTreeViewItem.ReconnectServer();
			//LinuxTreeViewItem.ReConnect();
			if(!SSHController.ReConnect(timeout_connect_ms))
				return false;

			try
			{
				FileContoller.CreateDirectory(local_folder_path);

				SftpFile[] files = PullListInDirectory(remote_directory_path);
				for(int i = 0; i < files.Length; i++)
				{
					if(files[i].Name == "." || files[i].Name == "..")
						continue;

					if(files[i].IsDirectory
						&& (filter_except_dir == null || !filter_except_dir.IsMatch(files[i].Name)))
					{
						string re_local_folder_path = local_folder_path + files[i].Name + @"\";
						downloadDirectory(re_local_folder_path, files[i].FullName, filter_file, filter_except_dir);
						continue;
					}

					if(filter_file != null && !filter_file.IsMatch(files[i].Name))
						continue;

					downloadFile(local_folder_path, files[i].FullName, files[i].Name, files[i].Name);
				}
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "downloadFile", Status.current.richTextBox_status);
				return false;
			}
			return true;
		}
		private static bool sendCommand(string command)
		{
			//LinuxTreeViewItem.ReconnectServer();
			//LinuxTreeViewItem.ReConnect();
			if(!SSHController.ReConnect(timeout_connect_ms))
				return false;

			try
			{
				// send
				if(shell_stream_reader != null)
				{
					readDummyMessageBlocking(null);
					//while(!shell_stream_reader.EndOfStream)
					//	Log.PrintConsole(read(), "Lost SSH Stream");

					shell_stream_writer.Write(command);
					shell_stream_writer.Write("\n");
					shell_stream_writer.Flush();
					Log.PrintConsole(command, "send command"/*, test4.m_wnd.richTextBox_status*/);

					bStart = false;

					return true;
				}
			}
			catch(Exception ex)
			{
				Log.PrintError(ex.Message, "send command", Status.current.richTextBox_status);
			}
			return false;
		}
		
		#region Read
		static string read_line_ssh = "";
		static string[] view_log_start = new string[] {"inform"};
		static string[] view_error_start = new string[] {"error" };
		public static string view_message_caption = "";
		static bool bstart = false;
		static bool bStart { get { return bstart; }
			set
			{
				bstart = value;
				//if(!value)
				//	read_line_ssh = "";
			}
		}
		private static string read()
		{
			int size_buffer = 4096;
			char[] buffer = new char[size_buffer];

			string _read = "";
			try
			{
				int cnt = shell_stream_reader.Read(buffer, 0, size_buffer);

				_read = new string(buffer, 0, cnt);
				read_line_ssh += _read;/* Encoding.UTF8.GetString(buffer, 0, cnt);*/
				if(read_line_ssh.Length > 0)
				{
					//Log.Print(read_line_ssh, "read"/*, test4.m_wnd.richTextBox_status*/);
					int idx_newline = 0;
					if((idx_newline = read_line_ssh.IndexOf('\n')) >= 0
						|| (idx_newline = read_line_ssh.IndexOf('\r')) >= 0)
					{
						string line = read_line_ssh.Substring(0, idx_newline);
						//int i;
						//for(i = 0; i < view_log_start.Length; i++)
						//{
						//	if(line.Length > view_log_start[i].Length && line.Substring(0, view_log_start[i].Length).ToLower() == view_log_start[i])
						//	{
						//		Log.ViewMessage(line, view_message_caption, Status.current.richTextBox_status);
						//	}
						//}
						//if(i == view_log_start.Length)
						//{
						//	for(i = 0; i < view_error_start.Length; i++)
						//	{
						//		if(line.Length > view_error_start[i].Length && line.Substring(0, view_error_start[i].Length).ToLower() == view_error_start[i])
						//			Log.PrintError(line, view_message_caption, Status.current.richTextBox_status);
						//	}
						//	if(i == view_error_start.Length)
						//		Log.ViewUndefine(line, "__undefined][" + view_message_caption, Status.current.richTextBox_status);
						//}
						if(line.IndexOf("Copyright (c) 2004-2016, eGlobal Systems, Co., Ltd.") >= 0)
						{
							bStart = true;
						}
						else if(bStart)
						{
							Log.ViewMessage(line, view_message_caption, Status.current.richTextBox_status);
						}
					

						Log.PrintConsole(line, "Read");
						read_line_ssh = read_line_ssh.Substring(idx_newline + 1);
					}
				}

			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "read", Status.current.richTextBox_status);
			}
			return _read;
		}
		private static string readDummyMessageBlocking(string cmd_send)
		{
			// 수정중..

			int size_buffer = 4096;
			char[] buffer = new char[size_buffer];
			for(int i = 0; i < 500; i++)
				shell_stream_reader.Read(buffer, 0, size_buffer);
			return null;

			//shell_stream_read_timer.Stop();

			//int size_buffer = 4096;
			//char[] buffer = new char[size_buffer];
			//string message = "";
			//string ret_message = null;
			//try
			//{
			//	// '$' or '#' 의 값이 나올떄까지 리드
			//	// 대신 output 에 $ 이나 # 이 있으면 안됨.
			//	string str_recv = "";
			//	while(str_recv.Length < 1 || (str_recv.LastIndexOf('$') < 0 && str_recv.LastIndexOf('#') < 0))
			//	{
			//		int cnt = shell_stream_reader.Read(buffer, 0, size_buffer);
			//		str_recv += new string(buffer, 0, cnt);/* Encoding.UTF8.GetString(buffer, 0, cnt);*/
			//	}
			//	int l = str_recv.Length;
			//	Log.PrintConsole(str_recv, "readMessageBlocking][" + l);
			//	//Log.ViewMessage(str_recv, "readMessageBlocking][" + l, Status.current.richTextBox_status);
			//	if(cmd_send == null || cmd_send == "")
			//		return cmd_send;

			//	// 명령어 전까지 자르기.(dummy 제거)
			//	//if(str_recv.LastIndexOf(cmd_send) >= 0)
			//	//	message = str_recv.Substring(str_recv.LastIndexOf(cmd_send));
			//	// 명령어가 이상한 값이 들어가므로.. 명령어 라인까지 자르기 (전제조건 : 명령어 부터 리시브를 받는다)
			//	if(str_recv.IndexOf('\n') >= 0 && str_recv.Length > str_recv.IndexOf('\n') + 1)
			//	{
			//		message = str_recv.Substring(str_recv.IndexOf('\n') + 1);
			//	}

			//	// 명령어 라인과 
			//	// 마지막라인 '[~] $' 제거
			//	if(message.IndexOf('\n') >= 0 && message.IndexOf('\n') + 1 < message.Length)
			//	{
			//		// 명령어는 이미 잘려있기 때문에
			//		//int idx = message.IndexOf('\n');
			//		int idx = 0;

			//		int len = message.LastIndexOf('\n') - idx;
			//		if(len > 0)
			//			ret_message = message.Substring(idx + 1, len);
			//	}


			//}
			//catch(Exception e)
			//{
			//	Log.PrintError(e.Message, "readMessageBlocking", Status.current.richTextBox_status);
			//}

			//shell_stream_read_timer.Start();
			//return ret_message;
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
				Log.PrintConsole(str_recv, "readMessageBlocking][" + l);

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
				Log.PrintError(e.Message, "readMessageBlocking", Status.current.richTextBox_status);
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
				int i=0;
				while(i < 500 || (prevstr.Length < cmd_length || prevstr.IndexOf(cmd_send) < 0 || prevstr.Length <= prevstr.IndexOf(cmd_send) + cmd_length))
				{
					int cnt = shell_stream_reader.Read(buffer, 0, size_buffer);
					prevstr += new string(buffer, 0, cnt);/* Encoding.UTF8.GetString(buffer, 0, cnt);*/
					i++;
				}

				int startline = 0;
				startline = prevstr.IndexOf(cmd_send);
				line = prevstr.Substring(startline + cmd_length);
				//if(startline > 2 && prevstr[startline - 2] != '$')
				//{
				//	prevstr = line;
				//	while((prevstr.Length < cmd_length || prevstr.IndexOf(cmd_send) < 0 || prevstr.Length <= prevstr.IndexOf(cmd_send) + cmd_length))
				//	{
				//		int cnt = shell_stream_reader.Read(buffer, 0, size_buffer);
				//		prevstr += new string(buffer, 0, cnt);/* Encoding.UTF8.GetString(buffer, 0, cnt);*/
				//	}
				//	startline = 0;
				//	startline = prevstr.IndexOf(cmd_send);
				//	line = prevstr.Substring(startline + cmd_length);
				//}

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

				Log.PrintConsole(str, "readCoHomeBlocking");

			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "readCoHomeBlocking", Status.current.richTextBox_status);
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

				Log.PrintConsole(str, "readInit");

			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "readInit", Status.current.richTextBox_status);
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
					Log.PrintConsole("[read] " + str, "Shell_stream_read_timer_Tick");
			}
		}
		#endregion


		#region Poll Linux Directory
		private static SftpFile[] _PullListInDirectory(string Path)
		{
			IEnumerable<SftpFile> files = null;
			try
			{
				files = sftp.ListDirectory(Path).OrderBy(x => x.FullName);
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "_PollListInDirectory", Status.current.richTextBox_status);
			}

			Log.PrintConsole(Path, "_PollListInDirectory"/*, test4.m_wnd.richTextBox_status*/);
			if(files == null)
				return null;

			return files.ToArray();
		}
		public static string WorkingDirectory {
			get
			{
				if(!SSHController.ReConnect(timeout_connect_ms))
					return null;
				return sftp.WorkingDirectory;
			}
		}
		public static SftpFile[] PullListInDirectory(string Path)
		{
			if(!SSHController.ReConnect(timeout_connect_ms))
				return null;
			if(Path == null)
				return null;

			return _PullListInDirectory(Path);
		}
		#endregion

		#region Excute Cofile Command
		static string cmd_get_co_home = "echo $CO_HOME";
		private static string LoadEnvCoHome()
		{
			//LinuxTreeViewItem.ReconnectServer();
			//LinuxTreeViewItem.ReConnect();
			if(!SSHController.ReConnect(timeout_connect_ms))
				return null;

			if(!SSHController.sendCommand(cmd_get_co_home))
				return null;
			string env_co_home = readCoHomeBlocking(cmd_get_co_home);

			if(env_co_home == null || env_co_home == "")
			{
				Log.PrintError("not defined $CO_HOME\r", "load $CO_HOME", Status.current.richTextBox_status);
				return null;
			}
			Log.PrintConsole("$CO_HOME = " + env_co_home, "load $CO_HOME");
			return env_co_home;
		}

		public static string add_path_config_upload = "/var/conf/";
		public static string add_path_bin = "/bin/";
		public static string add_path_run_cofile = add_path_bin + "cofile";
		public static CofileOption selected_type = CofileOption.file;
		public static string MakeCommandRunCofile(string path_run, CofileOption type, bool isEncrypt, string path, string configname, bool isDirectory)
		{
			string str = path_run;
			str += " " + type.ToString().ToLower();

			if(isEncrypt)
				str += " -e";
			else
				str += " -d";

			string[] split = path.Split('/');
			string filename = split[split.Length - 1];

			if(!isDirectory)
				str += " -f " + filename;
			//switch(type)
			//{
			//	case CofileOption.sam:
			//		str += " -i " + path;
			//		if(isEncrypt)
			//			str += " -o " + path + ".coenc";
			//		else
			//			str += " -o " + path + ".codec";
			//		break;
			//	case CofileOption.file:
			//		str += " -f " + filename;
			//		break;
			//	case CofileOption.tail:
			//		str += " -f " + path;
			//		break;
			//}

			if(configname != null)
				str += " -c " + configname;

			//if(type == CofileOption.file)
			//{
			//	str += " -id " + path.Substring(0, path.Length - filename.Length);
			//}
			if(isDirectory)
				str += " -id " + path;
			else
				str += " -id " + path.Substring(0, path.Length - filename.Length);

			return str;
		}

		public static string MakeCommandRunCofilePreview(string path_run, CofileOption type, bool isEncrypt, string path, string configname, bool isDirectory)
		{
			string str = path_run;
			str += " " + type.ToString().ToLower();

			if(isEncrypt)
				str += " -e";
			else
				str += " -d";

			string[] split = path.Split('/');
			string filename = split[split.Length - 1];

			if(!isDirectory)
				str += " -f " + filename;

			if(configname != null)
				str += " -c " + configname;

			if(isDirectory)
				str += " -id " + path;
			else
				str += " -id " + path.Substring(0, path.Length - filename.Length);

			if(isDirectory)
				str += " -od " + path;
			else
				str += " -od " + path.Substring(0, path.Length - filename.Length);

			str += " -oe " + Cofile.PreviewExtension;
			return str;
		}

		public static string GetRemoteConfigFilePath(string local_path_configfile)
		{
			string env_co_home = LoadEnvCoHome();
			string remote_path_configfile_dir = env_co_home + add_path_config_upload + ServerList.selected_serverinfo_textblock.serverinfo.id + "/";
			
			string path_root = ConfigJsonTree.cur_root_path;
			string remote_path_configfile = remote_path_configfile_dir;
			if(local_path_configfile.Length > path_root.Length
				&& local_path_configfile.Substring(0, path_root.Length) == ConfigJsonTree.cur_root_path)
			{
				remote_path_configfile += local_path_configfile.Substring(path_root.Length).Replace('\\', '/');
				if(!sftp.Exists(remote_path_configfile))
					return null;
			}
			else
			{
				string[] split = local_path_configfile.Split('\\');
				remote_path_configfile = remote_path_configfile_dir + split[split.Length - 1];
				if(!sftp.Exists(remote_path_configfile))
					return null;
			}
			return remote_path_configfile;
		}
		public static bool SendNRecvCofileCommand(IEnumerable<Object> selected_list, bool isEncrypt)
		{
			Status.current.Clear();

			string local_path_configfile = Cofile.current.Selected_config_file_path;
			if(local_path_configfile == null)
			{
				Log.PrintError("Check Config File", output_ui: Status.current.richTextBox_status);
				return false;
			}
			string remote_configfile_path = GetRemoteConfigFilePath(local_path_configfile);
			if(remote_configfile_path == null)
			{
				Log.PrintError("Check Config File", output_ui: Status.current.richTextBox_status);
				return false;
			}

			//string remote_file_path = UploadFile(Cofile.current.Selected_config_file_path, remote_directory);
			//if(remote_file_path == null)
			//	return false;

			string env_co_home = LoadEnvCoHome();
			List <LinuxTreeViewItem> parents = new List<LinuxTreeViewItem>();
			var enumerator = selected_list.GetEnumerator();
			for(int i =0; enumerator.MoveNext(); i++)
			{
				//RateProcessed = ((double)(i * 100)) / count;
				//WindowMain.current.progressBar.Value = RateProcessed;
				////WindowMain.current.UpdateLayout();
				//WindowMain.current.InvalidateVisual();

				LinuxTreeViewItem ltvi = enumerator.Current as LinuxTreeViewItem;

				CofileUI.UserControls.Cofile.LinuxListViewItem llvi = enumerator.Current as CofileUI.UserControls.Cofile.LinuxListViewItem;
				if(llvi != null)
					ltvi = llvi.LinuxTVI as LinuxTreeViewItem;

				if(ltvi == null)
					break;

				//string str = LinuxTreeViewItem.selected_list[i].Path.Substring(env_co_home.Length);
				string send_cmd;
				send_cmd = MakeCommandRunCofile(env_co_home + add_path_run_cofile, selected_type, isEncrypt, ltvi.Path, remote_configfile_path, ltvi.IsDirectory);
				//SendCommandOnce(send_cmd);
				sendCommand(send_cmd);
				//string ret = readMessageBlocking(send_cmd);

				//string ret = readCofileMessageBlocking();
				//string caption = isEncrypt ? "Encrypt" : "Decrypt";
				//Log.ViewMessage(ret, caption, Status.current.richTextBox_status);

				LinuxTreeViewItem parent = ltvi.Parent;
				if(parent != null && parents.IndexOf(parent) < 0)
					parents.Add(parent);
			}
			WindowMain.current.progressBar.Value = 0;
			for(int i = 0; i < parents.Count; i++)
				parents[i].RefreshChild();
			Cofile.current.RefreshListView(Cofile.cur_LinuxTreeViewItem);
			return true;
		}
		public static bool SendNRecvCofileCommandPreview(IEnumerable<Object> selected_list, bool isEncrypt)
		{
			Status.current.Clear();

			string local_path_configfile = Cofile.current.Selected_config_file_path;
			if(local_path_configfile == null)
			{
				Log.PrintError("Check Config File", output_ui: Status.current.richTextBox_status);
				return false;
			}
			string remote_configfile_path = GetRemoteConfigFilePath(local_path_configfile);
			if(remote_configfile_path == null)
			{
				Log.PrintError("Check Config File", output_ui: Status.current.richTextBox_status);
				return false;
			}

			string env_co_home = LoadEnvCoHome();
			List <LinuxTreeViewItem> parents = new List<LinuxTreeViewItem>();
			var enumerator = selected_list.GetEnumerator();
			for(int i = 0; enumerator.MoveNext(); i++)
			{
				LinuxTreeViewItem ltvi = enumerator.Current as LinuxTreeViewItem;

				CofileUI.UserControls.Cofile.LinuxListViewItem llvi = enumerator.Current as CofileUI.UserControls.Cofile.LinuxListViewItem;
				if(llvi != null)
					ltvi = llvi.LinuxTVI as LinuxTreeViewItem;

				if(ltvi == null)
					break;

				string send_cmd = MakeCommandRunCofilePreview(env_co_home + add_path_run_cofile, selected_type, isEncrypt, ltvi.Path, remote_configfile_path, ltvi.IsDirectory);

				sendCommand(send_cmd);

				//string ret = readCofileMessageBlocking();
				//string caption = isEncrypt ? "Encrypt" : "Decrypt";
				//Log.ViewMessage(ret, caption, Status.current.richTextBox_status);

				LinuxTreeViewItem parent = ltvi.Parent;
				if(parent != null && parents.IndexOf(parent) < 0)
					parents.Add(parent);
			}
			WindowMain.current.progressBar.Value = 0;
			for(int i = 0; i < parents.Count; i++)
				parents[i].RefreshChild();
			Cofile.current.RefreshListView(Cofile.cur_LinuxTreeViewItem);
			return true;
		}
		public static bool RunCofileCommand(string command)
		{
			string env_co_home = LoadEnvCoHome();
			string path_bin = env_co_home + add_path_bin;
			sendCommand(path_bin + command);
			//ssh.RunCommand(path_bin + command);
			return true;
		}
		public static bool GetFileAfterDecrypt(string remote_path_enc_file, string remote_path_config_file)
		{
			//string env_co_home = LoadEnvCoHome();
			//string path_bin = env_co_home + add_path_bin;

			//string[] split = remote_path_enc_file.Split('/');
			//if(remote_path_enc_file.Length <= split[split.Length - 1].Length)
			//	return false;

			//string remote_path_enc_file_base_dir = remote_path_enc_file.Substring(0, remote_path_enc_file.Length - split[split.Length - 1].Length);
			//string command = path_bin + "cofile file -d -f " + remote_path_enc_file + " -c " + remote_path_config_file + " -id / -od " + remote_path_enc_file_base_dir;
			//ssh.RunCommand(command);

			//string remote_path_dec_file = remote_path_enc_file_base_dir + 

			//moveFileToLocal(root_path, remote_path_dec_file, local_filename)
			return true;
		}
		#endregion


		#region Get DataBase (Cofile.db)
		static string db_name = "cofile.db";
		static string add_path_database = "/var/data/" + db_name;
		public static string GetDataBase(string local_folder, string local_file)
		{
			string remote_directory = LoadEnvCoHome();
			if(remote_directory == null)
				return null;

			string remote_path_file = remote_directory + add_path_database;
			if(downloadFile(local_folder, remote_path_file, local_file, db_name))
			{
				return local_folder + local_file;
			}
			return null;
		}
		#endregion

		#region Get Config File
		static string add_path_config_dir = @"/var/conf/";
		public static string GetConfig(string local_folder)
		{
			string remote_directory = LoadEnvCoHome();
			if(remote_directory == null)
				return null;

			string remote_path_dir = remote_directory + add_path_config_dir;
			remote_path_dir += ServerList.selected_serverinfo_textblock.serverinfo.id + "/";
			CreateDirectory(remote_path_dir);
			//if(downloadDirectory(local_folder, remote_path_dir, new Regex(@"\.json$"), new Regex(@"^\.backup")))
			if(downloadDirectory(local_folder, remote_path_dir, new Regex(@"\.json$")))
			{
				//JsonTreeViewItem.RemotePathRootDir = remote_path_dir;
				return local_folder;
			}
			return null;
		}
		public static string SetConfig(string local_file_path, string cur_local_root_path)
		{
			string remote_directory = LoadEnvCoHome();
			if(remote_directory == null)
				return null;

			string remote_path_dir = remote_directory + add_path_config_dir + ServerList.selected_serverinfo_textblock.serverinfo.id + "/";
			if(local_file_path.Length > cur_local_root_path.Length
				&& local_file_path.Substring(0, cur_local_root_path.Length) == cur_local_root_path)
			{
				string added_path = local_file_path.Substring(cur_local_root_path.Length).Replace('\\', '/');
				string[] split = added_path.Split('/');
				added_path = added_path.Substring(0, added_path.Length - split[split.Length - 1].Length);

				remote_path_dir += added_path;
			}

			string remote_backup_path_dir = remote_directory + add_path_config_dir + @".backup/" + ServerList.selected_serverinfo_textblock.serverinfo.id + "/";
			return UploadFile(local_file_path, remote_path_dir, remote_backup_path_dir);
		}

		public static SftpFileTree GetListConfigFile()
		{
			string remote_directory = LoadEnvCoHome();
			if(remote_directory == null)
				return null;

			string remote_path_dir = remote_directory + add_path_config_dir + ServerList.selected_serverinfo_textblock.serverinfo.id + "/";

			SftpFileTree.MakeTree(SftpFileTree.root, remote_path_dir);
			return SftpFileTree.root;
		}
		#endregion
	}

	public class SftpFileTree
	{
		public static SftpFileTree root = new SftpFileTree();
		private SftpFile file;
		public SftpFile File { get { return file; } set { file = value; } }
		public SftpFileTree parent = null;
		public List<SftpFileTree> children = new List<SftpFileTree>();
		private SftpFileTree() { parent = this; }
		public SftpFileTree(SftpFile _file)
		{
			File = _file;
		}

		public static void MakeTree(SftpFileTree cur, string remote_path_dir)
		{
			cur.children.Clear();

			SftpFile[] files = SSHController.PullListInDirectory(remote_path_dir);
			for(int i = 0; i < files.Length; i++)
			{
				if(files[i].Name == ".")
					continue;

				SftpFileTree child = new SftpFileTree(files[i]);

				cur.children.Add(child);
				child.parent = cur;

				if(files[i].IsDirectory)
				{
					if(files[i].Name == "..")
					{
						if(cur.parent != null)
							child.children = cur.parent.children;
					}
					else
						MakeTree(child, files[i].FullName);
				}
			}
		}
	}
}
