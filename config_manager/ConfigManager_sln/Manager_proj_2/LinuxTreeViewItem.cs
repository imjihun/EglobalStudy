using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Manager_proj_2
{
	class LinuxTreeViewItem : TreeViewItem
	{
		static string[] IGNORE_FILENAME = new string[] {".", ".."};
		public static LinuxTreeViewItem root;
		public static SshClient ssh;
		public static SftpClient sftp;
		public static ShellStream shell_stream;
		public static DispatcherTimer shell_stream_read_timer;

		#region header
		public class Grid_Header : Grid
		{
			const int HEIGHT = 25;
			public Grid_Header(string header)
			{
				//this.Height = HEIGHT;

				TextBlock tb = new TextBlock();
				tb.Text = header;
				tb.VerticalAlignment = VerticalAlignment.Center;
				this.Children.Add(tb);
			}
			public void SetText(string newText)
			{
				TextBlock tb = this.Children[0] as TextBlock;
				if(tb == null)
					return;
				tb.Text = newText;
			}
			public void SetBold()
			{
				TextBlock tb = this.Children[0] as TextBlock;
				if(tb == null)
					return;

				tb.FontWeight = FontWeights.Bold;
			}
		}
		public new Grid_Header Header { get { return base.Header as Grid_Header; } set { base.Header = value; } }
		#endregion

		#region variable
		private string path;
		public string Path { get { return path; } set { path = value; if(this == LinuxTreeViewItem.root) this.Header.SetText(value); } }
		bool isDirectory = false;
		#endregion

		public LinuxTreeViewItem(string _path, string header = null, bool _isDirectory = false)
		{
			if(header == null)
			{
				string[] splited = _path.Split('/');
				header = splited[splited.Length - 1];
			}

			this.Header = new Grid_Header(header);
			this.Cursor = Cursors.Hand;
			this.Path = _path;

			this.ContextMenu = new ContextMenu();
			MenuItem item = new MenuItem();
			item.Header = "암호화";
			item.Click += OnClickEncrypt;
			this.ContextMenu.Items.Add(item);
			item = new MenuItem();
			item.Header = "복호화";
			item.Click += OnClickDecrypt;
			this.ContextMenu.Items.Add(item);

			this.isDirectory = _isDirectory;

			if(this.isDirectory)
			{
				this.Foreground = Brushes.DarkBlue;
				this.Header.SetBold();

				// 임시
				Label dummy = new Label();
				this.Items.Add(dummy);
			}

		}
		private void OnClickEncrypt(object sender, RoutedEventArgs e)
		{
			sendCofileCommand(true);
		}
		private void OnClickDecrypt(object sender, RoutedEventArgs e)
		{
			sendCofileCommand(false);
		}
		private void sendCofileCommand(bool isEncrypt)
		{
			string remote_directory = GetConfigUploadDirectory();

			if(UploadFile(test4.m_wnd.Selected_config_file_path, remote_directory))
			{
				for(int i = 0; i < LinuxTreeViewItem.selected_list.Count; i++)
				{
					//string str = LinuxTreeViewItem.selected_list[i].Path.Substring(env_co_home.Length);
					sendCommand(MakeCommandRunCofile(env_co_home + added_path_run_cofile, "file", isEncrypt, LinuxTreeViewItem.selected_list[i].Path, remote_directory + LinuxTreeViewItem.current_cofile_name));
				}
			}
		}

		#region mouse handle for multi select
		static List<LinuxTreeViewItem> selected_list = new List<LinuxTreeViewItem>();
		bool mySelected = false;
		bool MySelected
		{
			get { return mySelected; }
			set
			{
				mySelected = value;
				if(value)
				{
					selected_list.Add(this);
					this.Background = Brushes.LightBlue;
				}
				else
				{
					selected_list.Remove(this);
					this.Background = Brushes.White;
				}
			}
		}
		private void MyFocuse()
		{
			while(selected_list.Count > 0)
			{
				selected_list[0].MySelected = false;
			}
			MySelected = true;
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if(test4.bCtrl)
				MySelected = !MySelected;
			else
				MyFocuse();

			e.Handled = true;
		}
		protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
		{
			if(selected_list.IndexOf(this) < 0)
				this.MyFocuse();
			base.OnMouseRightButtonDown(e);
			e.Handled = true;
		}
		#endregion

		#region linux directory
		protected override void OnExpanded(RoutedEventArgs e)
		{
			if(isDirectory)
			{
				this.Items.Clear();
				loadDirectory();
			}
		}
		void loadDirectory()
		{
			SftpFile[] files = PollListInDirectory();
			foreach(var file in files)
			{
				int i;
				for(i = 0; i < IGNORE_FILENAME.Length; i++)
				{
					if(file.Name == IGNORE_FILENAME[i])
						break;
				}
				if(i != IGNORE_FILENAME.Length)
					continue;

				if(file.IsDirectory)
					//this.Items.Insert(0, new LinuxTreeViewItem(file.FullName, file.Name, true));
					this.Items.Add(new LinuxTreeViewItem(file.FullName, file.Name, true));
				else
					//this.Items.Insert(0, new LinuxTreeViewItem(file.FullName, file.Name, false));
					this.Items.Add(new LinuxTreeViewItem(file.FullName, file.Name, false));
			}
		}
		#endregion

		#region linux ssh, sftp connection
		public static string env_co_home = "";
		public static string added_path_config_upload = "/var/conf";
		public static string added_path_run_cofile = "/bin/cofile";
		public static string MakeCommandRunCofile(string path_run, string mode, bool isEncrypt, string filename, string configname)
		{
			string str = path_run;
			if(mode != null)
				str += " " + mode;

			if(isEncrypt)
				str += " -e";
			else
				str += " -d";

			if(filename != null)
				str += " -f " + filename;

			if(configname != null)
				str += " -c " + configname;

			return str;
		}
		public static string GetConfigUploadDirectory()
		{
			LinuxTreeViewItem.sendCommand(cmd_get_co_home);
			read2(cmd_get_co_home);

			if(env_co_home == "")
			{
				Log.PrintError("not defined $CO_HOME", "load $CO_HOME");
				return null;
			}

			string remote_directory = env_co_home + added_path_config_upload;

			if(remote_directory[remote_directory.Length - 1] != '/')
				remote_directory += '/';

			Log.Print(remote_directory, "load $CO_HOME");
			return remote_directory;
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
		static bool ReConnection()
		{
			if(ServerList.selected_serverinfo_textblock == null)
				return false;

			string ip = ServerList.selected_serverinfo_textblock.serverinfo.ip;
			string id = ServerList.selected_serverinfo_textblock.serverinfo.id;
			string password = ServerList.selected_serverinfo_textblock.serverinfo.password;
			int port = 22;

			try
			{
				if(!CheckConnection(LinuxTreeViewItem.sftp, ip, port, id) || !CheckConnection(LinuxTreeViewItem.ssh, ip, port, id))
				{
					LinuxTreeViewItem.sftp = new SftpClient(ip, port, id, password);
					LinuxTreeViewItem.sftp.Connect();
					LinuxTreeViewItem.ssh = new SshClient(ip, port, id, password);
					LinuxTreeViewItem.ssh.Connect();
					LinuxTreeViewItem.shell_stream = ssh.CreateShellStream("customCommand", 80, 24, 800, 600, 1024);

					if(shell_stream_read_timer == null)
					{
						LinuxTreeViewItem.shell_stream_read_timer = new DispatcherTimer();
						LinuxTreeViewItem.shell_stream_read_timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
						LinuxTreeViewItem.shell_stream_read_timer.Tick += Shell_stream_read_timer_Tick;
					}
					LinuxTreeViewItem.shell_stream_read_timer.Stop();
					LinuxTreeViewItem.shell_stream_read_timer.Start();
					Log.Print(ip + " / " + port + " / " + id, "reconnection", test4.m_wnd.richTextBox_status);

					LinuxTreeViewItem.root.Path = sftp.WorkingDirectory;
					return true;
				}
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "reconnection", test4.m_wnd.richTextBox_status);
				return false;
			}
			return false;
		}
		SftpFile[] PollListInDirectory()
		{
			ReConnection();

			string local_directory = AppDomain.CurrentDomain.BaseDirectory;
			IEnumerable<SftpFile> files = null;
			try
			{
				files = sftp.ListDirectory(this.Path);
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "load directory", test4.m_wnd.richTextBox_status);
			}

			Log.Print(this.path, "load directory", test4.m_wnd.richTextBox_status);
			return files.ToArray();
		}

		static string cmd_get_co_home = "echo $CO_HOME";
		static string current_cofile_name = "";
		bool UploadFile(string local_path, string remote_directory)
		{
			ReConnection();
			try
			{
				var files = sftp.ListDirectory(remote_directory);

				FileInfo fi = new FileInfo(local_path);
				if(fi.Exists)
				{
					FileStream fs = File.Open(local_path, FileMode.Open, FileAccess.Read);
					sftp.UploadFile(fs, remote_directory + fi.Name);
					Log.Print(fi.Name + " => " + remote_directory + fi.Name, "upload file", test4.m_wnd.richTextBox_status);
					LinuxTreeViewItem.current_cofile_name = fi.Name;
				}
				else
				{
					Log.PrintError("check the config file path", "upload file", test4.m_wnd.richTextBox_status);
					return false;
				}
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "upload file", test4.m_wnd.richTextBox_status);
				return false;
			}
			return true;
		}
		private static void sendCommand(string command)
		{
			ReConnection();

			try
			{
				// send
				if(shell_stream != null)
				{
					shell_stream.Write(command);
					shell_stream.Write("\n");
					shell_stream.Flush();
					Log.Print(command, "send command", test4.m_wnd.richTextBox_status);
				}
			}
			catch(Exception ex)
			{
				Log.PrintError(ex.Message, "send command", test4.m_wnd.richTextBox_status);
			}
		}

		private static string read()
		{
			int size_buffer = 4096;
			byte[] buffer = new byte[size_buffer];
			string read = null;
			try
			{
				int cnt = shell_stream.Read(buffer, 0, size_buffer);

				read = Encoding.UTF8.GetString(buffer, 0, cnt);
				if(read.Length > 0)
					Log.Print(read, "read");

			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "read", test4.m_wnd.richTextBox_status);
			}
			return read;
		}
		private static string read2(string cmd_send)
		{
			int cmd_length = cmd_send.Length + 2;
			shell_stream_read_timer.Stop();
			int size_buffer = 4096;
			byte[] buffer = new byte[size_buffer];
			StringBuilder read = new StringBuilder();
			try
			{
				//bool bGet_end = false;
				while(read.Length < cmd_length || read.ToString().IndexOf('\n', cmd_length) < 0)
				{
					int cnt = shell_stream.Read(buffer, 0, size_buffer);
					read.Append(Encoding.UTF8.GetString(buffer, 0, cnt));
				}
				string str = read.ToString();
				int idx_start = str.IndexOf('\n', 0) + 1;
				int idx_end = str.IndexOf('\r', idx_start);
				int cnt_ret = idx_end - idx_start;

				if(idx_start > 0 && idx_end > 0 && cnt_ret > 0)
				{
					env_co_home = str.Substring(idx_start, cnt_ret);
				}
				else
					env_co_home = "";

				Log.Print(str, "read2");

			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "read2", test4.m_wnd.richTextBox_status);
			}

			shell_stream_read_timer.Start();
			return read.ToString();
		}
		private static string read3(string cmd_send)
		{
			int cmd_length = cmd_send.Length + 2;
			shell_stream_read_timer.Stop();
			int size_buffer = 4096;
			byte[] buffer = new byte[size_buffer];
			StringBuilder read = new StringBuilder();
			try
			{
				//bool bGet_end = false;
				while(read.Length < cmd_length || read.ToString().IndexOf('\n', cmd_length) < 0)
				{
					int cnt = shell_stream.Read(buffer, 0, size_buffer);
					read.Append(Encoding.UTF8.GetString(buffer, 0, cnt));
				}
				string str = read.ToString().Substring(cmd_length + 2);

				Log.Print(str, "read3");

			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "read3", test4.m_wnd.richTextBox_status);
			}

			shell_stream_read_timer.Start();
			return read.ToString();
		}
		private static void Shell_stream_read_timer_Tick(object sender, EventArgs e)
		{
			if(LinuxTreeViewItem.shell_stream != null)
			{
				string str = read();
				if(str.Length > 0)
					;//LinuxTreeViewItem.ViewLog("[read] " + str);
			}
		}
		#endregion

		public static void Refresh()
		{
			if(test4.m_wnd == null)
				return;

			//// 연결 체크
			//if(!LinuxTreeViewItem.ReConnection())
			//	return;
			if(ssh != null && ssh.IsConnected)
				ssh.Disconnect();
			if(sftp != null && sftp.IsConnected)
				sftp.Disconnect();

			// 삭제
			test4.m_wnd.treeView_linux_directory.Items.Clear();

			// 추가
			string home_dir = "/";
			//string home_dir = sftp.WorkingDirectory;
			LinuxTreeViewItem.root = new LinuxTreeViewItem(home_dir, home_dir, true);
			test4.m_wnd.treeView_linux_directory.Items.Add(LinuxTreeViewItem.root);
			Log.Print("[refresh]");
		}
	}
}
