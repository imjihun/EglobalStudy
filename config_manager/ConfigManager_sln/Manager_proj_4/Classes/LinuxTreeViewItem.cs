using MahApps.Metro.IconPacks;
using Manager_proj_4.UserControls;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace Manager_proj_4
{
	public class ConvertorCofileOptionToComboBoxItem : MarkupExtension
	{

		private readonly Type _type;

		public ConvertorCofileOptionToComboBoxItem(Type type)
		{
			_type = type;
		}
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return Enum.GetValues(_type)
				.Cast<object>()
				.Select(e => new { Value = (int)e, DisplayName = e.ToString() });
		}
	}
	public enum CofileOption
	{
		file = 0,
		sam,
		tail
	}

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
						shell_stream = ssh.CreateShellStream("customCommand", 80, 24, 800, 600, 1024);
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
					Log.Print(ip + " / " + port + " / " + id, "ReConnect"/*, test4.m_wnd.richTextBox_status*/);
					
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
						sendCommand(MakeCommandRunCofile(env_co_home + add_path_run_cofile, selected_type, isEncrypt, selected_list[i].Path, remote_directory + current_cofile_name));
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
				if(prevstr.IndexOf(cmd_send) > 2 &&  prevstr[startline - 2] != '$')
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
			
			string remote_path_file = remote_directory + add_path_database;
			if(downloadFile(local_folder, local_file, remote_path_file, db_name))
			{
				return local_folder + local_file;
			}
			return null;
		}
		#endregion
	}
	public class LinuxTreeViewItem : TreeViewItem
	{
		static string[] IGNORE_FILENAME = new string[] {".", ".."};
		public static LinuxTreeViewItem root;
		//public static SshClient ssh;
		//public static SftpClient sftp;
		//public static ShellStream shell_stream;
		//public static DispatcherTimer shell_stream_read_timer;

		#region header
		public class Grid_Header : Grid
		{
			const int HEIGHT = 30;
			public Grid_Header(string header)
			{
				//this.Height = HEIGHT;

				TextBlock tb = new TextBlock();
				tb.Text = header;
				tb.VerticalAlignment = VerticalAlignment.Center;
				this.Children.Add(tb);
			}
			public string Text
			{
				get
				{
					TextBlock tb = this.Children[0] as TextBlock;
					if(tb == null)
						return null;
					return tb.Text;
					//tb.Text = newText;
				}
				set
				{
					TextBlock tb = this.Children[0] as TextBlock;
					if(tb == null)
						return;
					tb.Text = value;
				}
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
		public string Path { get { return path; }
			set {
				path = value;
				if(this == LinuxTreeViewItem.root)
					this.Header.SetText(value);
			}
		}
		public bool isDirectory = false;
		#endregion

		private void InitContextMenu()
		{
			this.ContextMenu = new ContextMenu();
			MenuItem item = new MenuItem();
			item.Header = "암호화";
			item.Icon = new PackIconMaterial()
			{
				Kind = PackIconMaterialKind.KeyPlus,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center
			};
			item.Click += OnClickEncrypt;
			this.ContextMenu.Items.Add(item);
			item = new MenuItem();
			item.Icon = new PackIconMaterial()
			{
				Kind = PackIconMaterialKind.KeyMinus,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center
			};
			item.Header = "복호화";
			item.Click += OnClickDecrypt;
			this.ContextMenu.Items.Add(item);
		}
		public LinuxTreeViewItem(string _path, string header = null, bool _isDirectory = false)
		{
			if(header == null && _path != null)
			{
				string[] splited = _path.Split('/');
				header = splited[splited.Length - 1];
			}

			this.Header = new Grid_Header(header);
			this.Cursor = Cursors.Hand;
			this.Path = _path;

			InitContextMenu();

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
			WindowMain.current.ShowMessageDialog("Encrypt", "[Type = " + CofileOption.file + "] 선택된 파일들을 암호화 하시겠습니까?", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative, Encrypt);
			//if(MessageBox.Show("Encrypt?", "Encrypt", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
			//{
			//	//Log.ViewMessage("Encrypting..", "Encrypt", test4.m_wnd.richTextBox_status);
			//	TextRange txt = new TextRange(WindowMain.current.richTextBox_status.Document.ContentStart, WindowMain.current.richTextBox_status.Document.ContentEnd);
			//	txt.Text = "";
			//	view_message_caption = "Encrypt";
			//	sendCofileCommand(true);
			//}
		}
		private void Encrypt()
		{
			TextRange txt = new TextRange(WindowMain.current.richTextBox_status.Document.ContentStart, WindowMain.current.richTextBox_status.Document.ContentEnd);
			txt.Text = "";
			SSHController.view_message_caption = "Encrypt";
			SSHController.sendCofileCommand(selected_list.ToArray(), true);
		}
		private void OnClickDecrypt(object sender, RoutedEventArgs e)
		{
			WindowMain.current.ShowMessageDialog("Decrypt", "[Type = " + CofileOption.file + "] 선택된 파일들을 복호화 하시겠습니까?", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative, Decrypt);
			//if(MessageBox.Show("Decrypt?", "Decrypt", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
			//{
			//	//Log.ViewMessage("Decrypting..", "Decrypt", test4.m_wnd.richTextBox_status);
			//	TextRange txt = new TextRange(WindowMain.current.richTextBox_status.Document.ContentStart, WindowMain.current.richTextBox_status.Document.ContentEnd);
			//	txt.Text = "";
			//	view_message_caption = "Decrypt";
			//	sendCofileCommand(false);
			//}
		}
		public void Decrypt()
		{
			TextRange txt = new TextRange(WindowMain.current.richTextBox_status.Document.ContentStart, WindowMain.current.richTextBox_status.Document.ContentEnd);
			txt.Text = "";
			SSHController.view_message_caption = "Decrypt";
			SSHController.sendCofileCommand(selected_list.ToArray(), false);
		}

		#region mouse handle for multi select
		static List<LinuxTreeViewItem> selected_list = new List<LinuxTreeViewItem>();
		bool mySelected = false;

		static SolidColorBrush Background_selected { get { return (SolidColorBrush)App.Current.Resources["AccentColorBrush"]; } }
		static SolidColorBrush Foreground_selected = Brushes.White;
		static SolidColorBrush Foreground_unselected = Brushes.Black;
		bool MySelected
		{
			get { return mySelected; }
			set
			{
				mySelected = value;
				if(value)
				{
					selected_list.Add(this);
					this.Background = LinuxTreeViewItem.Background_selected;
					if(!this.isDirectory)
						this.Foreground = Foreground_selected;
				}
				else
				{
					selected_list.Remove(this);
					this.Background = Brushes.White;
					if(!this.isDirectory)
						this.Foreground = Foreground_unselected;
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
			if(WindowMain.bCtrl)
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
				loadDirectory();
				LinuxTreeViewItem.filter(this, filter_string);
			}
		}
		void loadDirectory()
		{
			SftpFile[] files;
			files = SSHController.PollListInDirectory(this.path);

			if(files == null)
			{
				this.IsExpanded = false;
				return;
			}

			this.Items.Clear();

			int count_have_directory = 0;
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

				LinuxTreeViewItem ltvi;
				if(file.IsDirectory)
				{
					//this.Items.Insert(0, new LinuxTreeViewItem(file.FullName, file.Name, true));
					//this.Items.Add(new LinuxTreeViewItem(file.FullName, file.Name, true));
					ltvi = new LinuxTreeViewItem(file.FullName, file.Name, true);
					this.Items.Insert(count_have_directory++, ltvi);
				}
				else
				{
					//this.Items.Insert(0, new LinuxTreeViewItem(file.FullName, file.Name, false));
					ltvi = new LinuxTreeViewItem(file.FullName, file.Name, false);
					this.Items.Add(ltvi);
					//if(file.Name.Substring(file.Name.Length - test_filter.Length, test_filter.Length) != test_filter)
					//	ltvi.Visibility = Visibility.Collapsed;
				}


			}
		}
		#endregion

		//#region linux ssh, sftp connection
		//public static string env_co_home = "";
		//public static string added_path_config_upload = "/var/conf";
		//public static string added_path_run_cofile = "/bin/cofile";
		//public static CofileOption selected_type = CofileOption.file;
		//private static string[] cofile_option = new string[] {"file", "sam", "tail" };
		//public static string MakeCommandRunCofile(string path_run, CofileOption type, bool isEncrypt, string path, string configname)
		//{
		//	string str = path_run;
		//	str += " " + cofile_option[(int)type];

		//	if(isEncrypt)
		//		str += " -e";
		//	else
		//		str += " -d";

		//	switch(type)
		//	{
		//		case CofileOption.sam:
		//			str += " -i " + path;
		//			if(isEncrypt)
		//				str += " -o " + path + ".coenc";
		//			else
		//				str += " -o " + path + ".codec";
		//			break;
		//		case CofileOption.file:
		//		case CofileOption.tail:
		//			str += " -f " + path;
		//			break;
		//	}

		//	if(configname != null)
		//		str += " -c " + configname;

		//	return str;
		//}
		//public static string GetConfigUploadDirectory()
		//{
		//	LinuxTreeViewItem.sendCommand(cmd_get_co_home);
		//	readCoHomeBlocking(cmd_get_co_home);

		//	if(env_co_home == "")
		//	{
		//		Log.PrintError("not defined $CO_HOME\r", "load $CO_HOME", WindowMain.current.richTextBox_status);
		//		return null;
		//	}

		//	string remote_directory = env_co_home + added_path_config_upload;

		//	if(remote_directory[remote_directory.Length - 1] != '/')
		//		remote_directory += '/';

		//	Log.Print("remote_directory = " + remote_directory, "load $CO_HOME");
		//	return remote_directory;
		//}
		////public static string remote_directory_upload = "/home/cofile/bin";

		//static bool CheckConnection(BaseClient client, string ip, int port, string id)
		//{
		//	if((client == null || !client.IsConnected)
		//			|| (client.ConnectionInfo.Host != ip
		//				|| client.ConnectionInfo.Port != port
		//				|| client.ConnectionInfo.Username != id))
		//		return false;

		//	return true;
		//}
		////static bool ReConnect()
		////{
		////	if(ServerList.selected_serverinfo_textblock == null)
		////		return false;

		////	string ip = ServerList.selected_serverinfo_textblock.serverinfo.ip;
		////	string id = ServerList.selected_serverinfo_textblock.serverinfo.id;
		////	string password = ServerList.selected_serverinfo_textblock.serverinfo.password;
		////	int port = 22;

		////	try
		////	{
		////		if(!CheckConnection(LinuxTreeViewItem.sftp, ip, port, id) || !CheckConnection(LinuxTreeViewItem.ssh, ip, port, id))
		////		{
		////			LinuxTreeViewItem.sftp = new SftpClient(ip, port, id, password);
		////			LinuxTreeViewItem.sftp.Connect();
		////			LinuxTreeViewItem.ssh = new SshClient(ip, port, id, password);
		////			LinuxTreeViewItem.ssh.Connect();

		////			if(LinuxTreeViewItem.ssh.IsConnected)
		////				LinuxTreeViewItem.shell_stream = ssh.CreateShellStream("customCommand", 80, 24, 800, 600, 1024);

		////			if(shell_stream_read_timer == null)
		////			{
		////				LinuxTreeViewItem.shell_stream_read_timer = new DispatcherTimer();
		////				LinuxTreeViewItem.shell_stream_read_timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
		////				LinuxTreeViewItem.shell_stream_read_timer.Tick += Shell_stream_read_timer_Tick;
		////			}
		////			LinuxTreeViewItem.shell_stream_read_timer.Stop();
		////			LinuxTreeViewItem.shell_stream_read_timer.Start();
		////			Log.Print(ip + " / " + port + " / " + id, "ReConnect"/*, test4.m_wnd.richTextBox_status*/);

		////			LinuxTreeViewItem.root.Path = sftp.WorkingDirectory;
		////			return true;
		////		}
		////	}
		////	catch(Exception e)
		////	{
		////		Log.PrintError(e.Message, "ReConnect", WindowMain.m_wnd.richTextBox_status);
		////		return false;
		////	}
		////	return false;
		////}
		//static bool ReConnect(int timeout_ms = -1)
		//{
		//	if(ServerList.selected_serverinfo_textblock == null)
		//		return false;

		//	string ip = ServerList.selected_serverinfo_textblock.serverinfo.ip;
		//	string id = ServerList.selected_serverinfo_textblock.serverinfo.id;
		//	string password = ServerList.selected_serverinfo_textblock.serverinfo.password;
		//	int port = 22;

		//	try
		//	{
		//		if(!CheckConnection(LinuxTreeViewItem.sftp, ip, port, id) || !CheckConnection(LinuxTreeViewItem.ssh, ip, port, id))
		//		{
		//			LinuxTreeViewItem.sftp = new SftpClient(ip, port, id, password);
		//			if(timeout_ms != -1)
		//				LinuxTreeViewItem.sftp.ConnectionInfo.Timeout = new TimeSpan(0, 0, 0, 0, timeout_ms);
		//			LinuxTreeViewItem.sftp.Connect();
		//			LinuxTreeViewItem.ssh = new SshClient(ip, port, id, password);
		//			if(timeout_ms != -1)
		//				LinuxTreeViewItem.ssh.ConnectionInfo.Timeout = new TimeSpan(0, 0, 0, 0, timeout_ms);
		//			LinuxTreeViewItem.ssh.Connect();

		//			if(LinuxTreeViewItem.ssh.IsConnected)
		//				LinuxTreeViewItem.shell_stream = ssh.CreateShellStream("customCommand", 80, 24, 800, 600, 1024);

		//			if(shell_stream_read_timer == null)
		//			{
		//				LinuxTreeViewItem.shell_stream_read_timer = new DispatcherTimer();
		//				LinuxTreeViewItem.shell_stream_read_timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
		//				LinuxTreeViewItem.shell_stream_read_timer.Tick += Shell_stream_read_timer_Tick;
		//			}
		//			LinuxTreeViewItem.shell_stream_read_timer.Stop();
		//			LinuxTreeViewItem.shell_stream_read_timer.Start();
		//			Log.Print(ip + " / " + port + " / " + id, "ReConnect"/*, test4.m_wnd.richTextBox_status*/);

		//			LinuxTreeViewItem.root.Path = sftp.WorkingDirectory;
		//			return true;
		//		}
		//	}
		//	catch(Exception e)
		//	{
		//		Log.PrintError(e.Message, "ReConnect", WindowMain.current.richTextBox_status);
		//		return false;
		//	}
		//	return true;
		//}
		//SftpFile[] PollListInDirectory()
		//{
		//	//LinuxTreeViewItem.ReconnectServer();
		//	//LinuxTreeViewItem.ReConnect();
		//	if(!LinuxTreeViewItem.ReConnect(1000))
		//		return null;
		
		//	IEnumerable<SftpFile> files = null;
		//	try
		//	{
		//		files = sftp.ListDirectory(this.Path).OrderBy(x=>x.FullName);
		//	}
		//	catch(Exception e)
		//	{
		//		Log.PrintError(e.Message, "load directory", WindowMain.current.richTextBox_status);
		//	}

		//	Log.Print(this.path, "load directory"/*, test4.m_wnd.richTextBox_status*/);
		//	return files.ToArray();
		//}

		//static string cmd_get_co_home = "echo $CO_HOME";
		//static string current_cofile_name = "";
		//bool UploadFile(string local_path, string remote_directory)
		//{
		//	//LinuxTreeViewItem.ReconnectServer();
		//	//LinuxTreeViewItem.ReConnect();
		//	LinuxTreeViewItem.ReConnect(1000);

		//	try
		//	{
		//		FileInfo fi = new FileInfo(local_path);
		//		if(fi.Exists)
		//		{
		//			FileStream fs = File.Open(local_path, FileMode.Open, FileAccess.Read);
		//			sftp.UploadFile(fs, remote_directory + fi.Name);
		//			Log.Print(fi.Name + " => " + remote_directory + fi.Name, "upload file"/*, test4.m_wnd.richTextBox_status*/);
		//			LinuxTreeViewItem.current_cofile_name = fi.Name;
		//			fs.Close();
		//		}
		//		else
		//		{
		//			Log.PrintError("check the config file path", "upload file", WindowMain.current.richTextBox_status);
		//			return false;
		//		}
		//	}
		//	catch(Exception e)
		//	{
		//		Log.PrintError(e.Message, "upload file", WindowMain.current.richTextBox_status);
		//		return false;
		//	}
		//	return true;
		//}
		//private static void sendCommand(string command)
		//{
		//	//LinuxTreeViewItem.ReconnectServer();
		//	//LinuxTreeViewItem.ReConnect();
		//	LinuxTreeViewItem.ReConnect(1000);

		//	try
		//	{
		//		// send
		//		if(shell_stream != null)
		//		{
		//			shell_stream.Write(command);
		//			shell_stream.Write("\n");
		//			shell_stream.Flush();
		//			Log.Print(command, "send command"/*, test4.m_wnd.richTextBox_status*/);
		//		}
		//	}
		//	catch(Exception ex)
		//	{
		//		Log.PrintError(ex.Message, "send command", WindowMain.current.richTextBox_status);
		//	}
		//}

		//static string read_line_ssh = "";
		//static string[] view_log_start = new string[] {"inform"};
		//static string[] view_error_start = new string[] {"error" };
		//static string view_message_caption = "";
		//private static string read()
		//{
		//	int size_buffer = 4096;
		//	byte[] buffer = new byte[size_buffer];
		//	try
		//	{
		//		int cnt = shell_stream.Read(buffer, 0, size_buffer);

		//		read_line_ssh += Encoding.UTF8.GetString(buffer, 0, cnt);
		//		if(read_line_ssh.Length > 0)
		//		{
		//			//Log.Print(read_line_ssh, "read"/*, test4.m_wnd.richTextBox_status*/);
		//			int idx_newline = 0;
		//			if((idx_newline = read_line_ssh.IndexOf('\n')) >= 0)
		//			{
		//				string line = read_line_ssh.Substring(0, idx_newline);
		//				int i;
		//				for(i = 0; i < view_log_start.Length; i++)
		//				{
		//					if(line.Length > view_log_start[i].Length && line.Substring(0, view_log_start[i].Length).ToLower() == view_log_start[i])
		//					{
		//						Log.ViewMessage(line, view_message_caption, WindowMain.current.richTextBox_status);
		//					}
		//				}
		//				if(i == view_log_start.Length)
		//				{
		//					for(i = 0; i < view_error_start.Length; i++)
		//					{
		//						if(line.Length > view_error_start[i].Length && line.Substring(0, view_error_start[i].Length).ToLower() == view_error_start[i])
		//							Log.PrintError(line, view_message_caption, WindowMain.current.richTextBox_status);
		//					}
		//					if(i == view_error_start.Length)
		//						Log.ViewUndefine(line, "__undefined][" + view_message_caption, WindowMain.current.richTextBox_status);
		//				}

		//				Log.Print(line, "Read");
		//				read_line_ssh = read_line_ssh.Substring(idx_newline + 1);
		//			}
		//		}

		//	}
		//	catch(Exception e)
		//	{
		//		Log.PrintError(e.Message, "read", WindowMain.current.richTextBox_status);
		//	}
		//	return read_line_ssh;
		//}
		//private static string readCoHomeBlocking(string cmd_send)
		//{
		//	cmd_send += "\r\n";
		//	int cmd_length = cmd_send.Length;
		//	shell_stream_read_timer.Stop();
		//	int size_buffer = 4096;
		//	byte[] buffer = new byte[size_buffer];
		//	string line = "";
		//	try
		//	{
		//		while(line.Length < cmd_length || line.IndexOf(cmd_send) < 0 || line.Length <= line.IndexOf(cmd_send) + cmd_length)
		//		{
		//			int cnt = shell_stream.Read(buffer, 0, size_buffer);
		//			line += Encoding.UTF8.GetString(buffer, 0, cnt);
		//		}
		//		line = line.Substring(line.IndexOf(cmd_send) + cmd_length);
		//		//bool bGet_end = false;
		//		while(line.Length < cmd_length || line.IndexOf('\n') < 0)
		//		{
		//			int cnt = shell_stream.Read(buffer, 0, size_buffer);
		//			line += Encoding.UTF8.GetString(buffer, 0, cnt);
		//		}
		//		string str = line.ToString();
		//		int idx_end = str.IndexOf('\r');

		//		if(idx_end > 0)
		//		{
		//			env_co_home = str.Substring(0, idx_end);
		//		}
		//		else
		//			env_co_home = "";

		//		Log.Print(str, "readCoHomeBlocking");

		//	}
		//	catch(Exception e)
		//	{
		//		Log.PrintError(e.Message, "readCoHomeBlocking", WindowMain.current.richTextBox_status);
		//	}

		//	shell_stream_read_timer.Start();
		//	return line.ToString();
		//}
		//private static string read3(string cmd_send)
		//{
		//	int cmd_length = cmd_send.Length + 2;
		//	shell_stream_read_timer.Stop();
		//	int size_buffer = 4096;
		//	byte[] buffer = new byte[size_buffer];
		//	StringBuilder read = new StringBuilder();
		//	try
		//	{
		//		//bool bGet_end = false;
		//		while(read.Length < cmd_length || read.ToString().IndexOf('\n', cmd_length) < 0)
		//		{
		//			int cnt = shell_stream.Read(buffer, 0, size_buffer);
		//			read.Append(Encoding.UTF8.GetString(buffer, 0, cnt));
		//		}
		//		string str = read.ToString().Substring(cmd_length + 2);

		//		Log.Print(str, "read3");

		//	}
		//	catch(Exception e)
		//	{
		//		Log.PrintError(e.Message, "read3", WindowMain.current.richTextBox_status);
		//	}

		//	shell_stream_read_timer.Start();
		//	return read.ToString();
		//}
		//private static void Shell_stream_read_timer_Tick(object sender, EventArgs e)
		//{
		//	if(LinuxTreeViewItem.shell_stream != null)
		//	{
		//		string str = read();
		//		if(str.Length > 0)
		//			;//LinuxTreeViewItem.ViewLog("[read] " + str);
		//	}
		//}
		//#endregion

		//public static List<BackgroundWorker> BackgroundReConnector = new List<BackgroundWorker>();
		//public static void ReconnectServer()
		//{
		//	if(LinuxTreeViewItem.root == null)
		//		return;

		//	LinuxTreeViewItem.root.IsEnabled = false;
		//	//ServerPanel.SubPanel.IsEnabled = false;
			
		//	for(int i = 0; i < BackgroundReConnector.Count; i++)
		//	{
		//		BackgroundReConnector[i].CancelAsync();
		//	}

		//	BackgroundWorker _BackgroundReConnector = new BackgroundWorker();
		//	_BackgroundReConnector.DoWork += LinuxTreeViewItem.BackgroundReConnect;
		//	_BackgroundReConnector.RunWorkerCompleted += LinuxTreeViewItem.BackgroundReConnectCallBack;
		//	_BackgroundReConnector.WorkerReportsProgress = true;
		//	_BackgroundReConnector.WorkerSupportsCancellation = true;
		//	_BackgroundReConnector.RunWorkerAsync();
		//	BackgroundReConnector.Add(_BackgroundReConnector);
		//}
		//public static void BackgroundReConnectCallBack(object sender, RunWorkerCompletedEventArgs e)
		//{
		//	if(LinuxTreeViewItem.root != null && sftp != null && sftp.IsConnected)
		//	{
		//		LinuxTreeViewItem.root.Path = sftp.WorkingDirectory;
		//		LinuxTreeViewItem.root.IsEnabled = true;
		//	}
		//	if(ServerPanel.SubPanel != null)
		//		ServerPanel.SubPanel.IsEnabled = true;

		//	if(sender is BackgroundWorker)
		//		BackgroundReConnector.Remove(sender as BackgroundWorker);

		//	if(shell_stream_read_timer == null)
		//	{
		//		LinuxTreeViewItem.shell_stream_read_timer = new DispatcherTimer();
		//		LinuxTreeViewItem.shell_stream_read_timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
		//		LinuxTreeViewItem.shell_stream_read_timer.Tick += Shell_stream_read_timer_Tick;
		//	}
		//}
		//public static void BackgroundReConnect(object sender, DoWorkEventArgs e)
		//{
		//	if(ServerList.selected_serverinfo_textblock == null)
		//		return;

		//	TimeSpan timeout = new TimeSpan(0,0,0,2,0);

		//	string ip = ServerList.selected_serverinfo_textblock.serverinfo.ip;
		//	string id = ServerList.selected_serverinfo_textblock.serverinfo.id;
		//	string password = ServerList.selected_serverinfo_textblock.serverinfo.password;
		//	int port = 22;

		//	try
		//	{
		//		if(!CheckConnection(LinuxTreeViewItem.sftp, ip, port, id) || !CheckConnection(LinuxTreeViewItem.ssh, ip, port, id))
		//		{
		//			LinuxTreeViewItem.sftp = new SftpClient(ip, port, id, password);
		//			LinuxTreeViewItem.sftp.ConnectionInfo.Timeout = timeout;
		//			LinuxTreeViewItem.sftp.Connect();
		//			LinuxTreeViewItem.ssh = new SshClient(ip, port, id, password);
		//			LinuxTreeViewItem.ssh.ConnectionInfo.Timeout = timeout;
		//			LinuxTreeViewItem.ssh.Connect();

		//			if(LinuxTreeViewItem.ssh.IsConnected)
		//				LinuxTreeViewItem.shell_stream = ssh.CreateShellStream("customCommand", 80, 24, 800, 600, 1024);

		//			LinuxTreeViewItem.shell_stream_read_timer.Stop();
		//			LinuxTreeViewItem.shell_stream_read_timer.Start();
		//			//LinuxTreeViewItem.root.Path = sftp.WorkingDirectory;
		//		}
		//	}
		//	catch(Exception ex)
		//	{
		//		Console.WriteLine("[Thread][Connection Error] = " + ex.Message);
		//	}
		//}

		public static void Refresh()
		{
			if(WindowMain.current == null)
				return;

			//if(ssh != null && ssh.IsConnected)
			//	ssh.Disconnect();
			//if(sftp != null && sftp.IsConnected)
			//	sftp.Disconnect();

			// 삭제
			Cofile.current.treeView_linux_directory.Items.Clear();

			// 추가
			//string home_dir = sftp.WorkingDirectory;
			LinuxTreeViewItem.root = new LinuxTreeViewItem(null, null, true);
			Cofile.current.treeView_linux_directory.Items.Add(LinuxTreeViewItem.root);
			Log.Print("[refresh]");

			//LinuxTreeViewItem.ReconnectServer();
		}

		static string filter_string = "";
		public static string Filter_string { get { return filter_string; } set { filter_string = value; LinuxTreeViewItem.filter(LinuxTreeViewItem.root, filter_string); } }
		static void filter(LinuxTreeViewItem parent, string filter_string)
		{
			if(parent == null)
				return;
			try
			{
				Regex r = new Regex(filter_string);
				filter_recursive(parent, r);
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}
		static void filter_recursive(LinuxTreeViewItem cur, Regex filter_string)
		{
			for(int i = 0; i < cur.Items.Count; i++)
			{
				LinuxTreeViewItem child = cur.Items[i] as LinuxTreeViewItem;
				if(child == null)
					continue;

				string name = child.Header.Text;
				if(!child.isDirectory &&
					!filter_string.IsMatch(name))
				{
					child.Visibility = Visibility.Collapsed;
				}
				else if(child.isDirectory)
					filter_recursive(child, filter_string);
				else
					child.Visibility = Visibility.Visible;
			}
		}
	}
}
