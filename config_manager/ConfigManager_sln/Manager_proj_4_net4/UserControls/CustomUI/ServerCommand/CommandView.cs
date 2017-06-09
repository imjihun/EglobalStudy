using Manager_proj_4.Classes;
using Renci.SshNet;
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

namespace Manager_proj_4.UserControls
{
	/// <summary>
	///  빠진 UI
	/// </summary>
	class CommandView : Grid
	{
		int PORT = 22;
		SshClient sshclient;

		public static CommandView current;
		public TextBlock textBlock_server_name;
		public TextBox textBox_command;
		public TextBox textBox_result;

		DispatcherTimer timer_read;
		ShellStream shell_stream;

		public new Visibility Visibility
		{
			get { return base.Visibility; }
			set
			{
				base.Visibility = value;
				if(value == Visibility.Hidden
					&& timer_read != null)
					timer_read.Stop();
			}
		}
		public CommandView()
		{
			current = this;
			this.Visibility = Visibility.Hidden;

			textBlock_server_name = new TextBlock();
			textBlock_server_name.Margin = new Thickness(5, 5, 95, 5);
			textBlock_server_name.VerticalAlignment = VerticalAlignment.Top;
			this.Children.Add(textBlock_server_name);

			TextBlock command = new TextBlock();
			command.Text = "Command :";
			command.Margin = new Thickness(5, 35, 5, 5);
			command.Width = 70;
			command.VerticalAlignment = VerticalAlignment.Top;
			command.HorizontalAlignment = HorizontalAlignment.Left;
			this.Children.Add(command);

			textBox_command = new TextBox();
			textBox_command.Margin = new Thickness(80, 35, 95, 5);
			textBox_command.VerticalAlignment = VerticalAlignment.Top;
			textBox_command.HorizontalAlignment = HorizontalAlignment.Stretch;
			textBox_command.KeyDown += TextBox_command_KeyDown;
			textBox_command.FontFamily = new FontFamily("Consolas");
			this.Children.Add(textBox_command);

			Button btn_send = new Button();
			btn_send.Background = Brushes.White;
			btn_send.Content = "Send(or Run)";
			btn_send.Margin = new Thickness(5, 35, 5, 5);
			btn_send.VerticalAlignment = VerticalAlignment.Top;
			btn_send.HorizontalAlignment = HorizontalAlignment.Right;
			btn_send.Width = 85;
			btn_send.Click += Button_send_Click;
			this.Children.Add(btn_send);

			textBox_result = new TextBox();
			textBox_result.IsReadOnly = true;
			textBox_result.Margin = new Thickness(5, 60, 5, 5);
			textBox_result.TextWrapping = TextWrapping.Wrap;
			textBox_result.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
			textBox_result.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
			textBox_result.FontFamily = new FontFamily("Consolas");
			textBox_result.TextChanged += TextBox_result_TextChanged;
			this.Children.Add(textBox_result);

			//timer_read = new DispatcherTimer();
			//timer_read.Interval = TimeSpan.FromSeconds(0.001);
			//timer_read.Tick += Timer_read_Tick;
		}
		private void TextBox_result_TextChanged(object sender, TextChangedEventArgs e)
		{
			textBox_result.ScrollToEnd();
		}
		private void TextBox_command_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key != Key.Enter)
				return;

			SendCommand();
		}
		private void Button_send_Click(object sender, RoutedEventArgs e)
		{
			SendCommand();
		}

		#region ssh remote command
		//async private void SendCommand()
		private void SendCommand()
		{
			if(ServerList.selected_serverinfo_textblock == null)
				return;
			if(CommandView.current == null)
				return;

			string ip = ServerList.selected_serverinfo_textblock.serverinfo.ip;
			string id = ServerList.selected_serverinfo_textblock.serverinfo.id;
			string password = ServerList.selected_serverinfo_textblock.serverinfo.password;
			string command = textBox_command.Text;
			textBox_command.Text = "";

			//// 비동기
			//string ret = await Task.Run(() => SendCommand(ip, id, password, command));
			// 동기
			string ret = _SendCommand(ip, id, password, command);

			//textBox_result.Text = ret;
			//Console.WriteLine("awaik finish");
		}
		private string _SendCommand(string ip, string id, string password, string command)
		{
			string ret = "";
			Console.WriteLine("\n[ start ]");
			try
			{
				// 연결 체크
				if((CommandView.current.sshclient == null || !CommandView.current.sshclient.IsConnected)
					|| (CommandView.current.sshclient.ConnectionInfo.Host != ip
						|| CommandView.current.sshclient.ConnectionInfo.Port != PORT
						|| CommandView.current.sshclient.ConnectionInfo.Username != id))
				{
					if(CommandView.current.sshclient != null && CommandView.current.sshclient.IsConnected)
						CommandView.current.sshclient.Disconnect();

					CommandView.current.sshclient = new SshClient(ip, PORT, id, password);
					CommandView.current.sshclient.Connect();
					shell_stream = sshclient.CreateShellStream("customCommand", 80, 24, 800, 600, 1024);
					timer_read.Start();

					Console.Write("[ ReConnection ] ");
					Console.WriteLine(CommandView.current.sshclient.ConnectionInfo.Host + " / " + CommandView.current.sshclient.ConnectionInfo.Port + " / " + CommandView.current.sshclient.ConnectionInfo.Username + " / " + CommandView.current.sshclient.ConnectionInfo.ProxyUsername + " / " + CommandView.current.sshclient.ConnectionInfo.ProxyPassword);
				}
				// send
				if(shell_stream != null)
				{
					shell_stream.Write(command);
					shell_stream.Write("\n");
					shell_stream.Flush();
					//textBox_result.Text += read2();
				}
			}
			catch(Exception ex)
			{
				ret = "Error = " + ex.Message;
				Console.WriteLine(ex.Message);
			}
			Console.WriteLine("[ finish ]\n");

			if(command == "exit")
				CommandView.current.sshclient.Disconnect();
			return ret;
		}
		private void Timer_read_Tick(object sender, EventArgs e)
		{
			if(shell_stream != null)
			{
				//string str = await read();
				string str = read();
				if(str.Length > 0)
					textBox_result.Text += str;
			}
		}
		//async Task<string> read()
		string read()
		{
			int size_buffer = 4096;
			byte[] buffer = new byte[size_buffer];

			int cnt = shell_stream.Read(buffer, 0, size_buffer);

			return Encoding.UTF8.GetString(buffer, 0, cnt);
		}
		string read2()
		{
			StringBuilder retval = new StringBuilder();
			string line;
			while((line = shell_stream.ReadLine(new TimeSpan(0, 0, 1))) != null)
				retval.Append(line + "\n");

			return retval.ToString();
		}
		void print(SshCommand x)
		{
			Console.WriteLine("### CommandText = " + x.CommandText);
			Console.WriteLine("### CommandTimeout = " + x.CommandTimeout);
			Console.WriteLine("### Error = " + x.Error);
			Console.WriteLine("### ExitStatus = " + x.ExitStatus);
			Console.WriteLine("### Result = " + x.Result);
			byte[] buffer = new byte[4096];
			int cnt = x.OutputStream.Read(buffer, 0, 4096);
			Console.WriteLine("### OutputStream = " + Encoding.UTF8.GetString(buffer, 0, cnt));
			cnt = x.ExtendedOutputStream.Read(buffer, 0, 4096);
			Console.WriteLine("### ExtendedOutputStream = " + Encoding.UTF8.GetString(buffer, 0, cnt));
		}
		#endregion

		#region sftp
		void DownLoadJsonFile()
		{
			if(ServerList.selected_serverinfo_textblock.serverinfo == null)
				return;

			string ip = ServerList.selected_serverinfo_textblock.serverinfo.ip;
			string id = ServerList.selected_serverinfo_textblock.serverinfo.id;
			string password = ServerList.selected_serverinfo_textblock.serverinfo.password;
			SftpClient sftp = new SftpClient(ip, id, password);
			sftp.Connect();

			string local_directory = AppDomain.CurrentDomain.BaseDirectory;
			string remote_directory = "/home/cofile/bin/";
			var files = sftp.ListDirectory(remote_directory);
			foreach(var file in files)
			{
				if(file.Name.Length > 4 && file.Name.Substring(file.Name.Length - 5) == ".json")
				{
					FileStream fs = new FileStream(local_directory + file.Name, FileMode.Create);
					sftp.DownloadFile(remote_directory + file.Name, fs);
					Console.WriteLine("[ download ] " + file.Name);
				}
			}
		}
		void UploadJsonFile()
		{
			if(ServerList.selected_serverinfo_textblock.serverinfo == null)
				return;

			string ip = ServerList.selected_serverinfo_textblock.serverinfo.ip;
			string id = ServerList.selected_serverinfo_textblock.serverinfo.id;
			string password = ServerList.selected_serverinfo_textblock.serverinfo.password;
			SftpClient sftp = new SftpClient(ip, id, password);
			sftp.Connect();

			string local_directory = AppDomain.CurrentDomain.BaseDirectory;
			string remote_directory = "/home/cofile/bin/";

			string[] paths = FileContoller.LoadFile(local_directory, "*.json");
			foreach(var path in paths)
			{
				FileInfo fi = new FileInfo(path);
				Console.WriteLine("[ upload ] " + fi);
				FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read);
				sftp.UploadFile(fs, remote_directory + fi.Name);
			}
		}
		#endregion

		#region telnet
		void test()
		{

		}
		#endregion

		public static void clear()
		{
			if(current == null)
				return;

			current.textBlock_server_name.Text = "";
			current.textBox_command.Text = "";
			current.textBox_result.Text = "";
		}
	}

}
