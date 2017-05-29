using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Collections.Specialized;
using Renci.SshNet;
using System.Reflection;

namespace Manager_proj_3
{
	/// <summary>
	/// test4.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class test4 : Window
	{
		public test4()
		{
			InitializeComponent();
			this.Closed += test4_Closed;

			InitServerTab();

			InitJsonFileView();
			InitServerCommandView();
		}

		private void test4_Closed(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
		}

		#region Server Menu Class
		void InitServerTab()
		{
			// UI 초기화
			ServerGrid servergrid = new ServerGrid();
			grid_server.Children.Add(servergrid);
			ServerMenuButton[] smbtn = new ServerMenuButton[ServerInfo.CATEGORIES.Length];
			for(int i = 0; i < ServerInfo.CATEGORIES.Length; i++)
			{
				ServerInfo.jobj_root.Add(new JProperty(ServerInfo.CATEGORIES[i], new JObject()));
				smbtn[i] = new ServerMenuButton(ServerInfo.CATEGORIES[i]);
				servergrid.Children.Add(smbtn[i]);
				ServerGrid.current.submenu.Children.Add(smbtn[i].child);
			}
			smbtn[0].IsChecked = true;


			// serverinfo.json 파일 로드
			FileInfo fi = new FileInfo(ServerInfo.PATH);
			if(fi.Exists)
			{
				string json = FileContoller.read(ServerInfo.PATH);
				try
				{
					ServerInfo.jobj_root = JObject.Parse(json);
					List<ServerInfo>[] serverinfos = ServerInfo.ConvertFromJson(ServerInfo.jobj_root);
					for(int i = 0; i < ServerInfo.CATEGORIES.Length; i++)
					{
						for(int j = 0; j < serverinfos[i].Count; j++)
						{
							smbtn[i].child.Items.Add(new ServerInfoTextBlock(serverinfos[i][j]));
						}
					}
					//for(int i = 0; i < ServerInfo.CATEGORIES.Length; i++)
					//{
					//	JObject jobj = ServerInfo.jobj_root[ServerInfo.CATEGORIES[i]] as JObject;
					//	ServerInfo si;
					//	foreach(var v in jobj.Properties())
					//	{
					//		si = ServerInfo.ConvertFromJson(v);
					//		smbtn[i].child.Items.Add(new ServerInfoTextBlock(si));
					//	}
					//}
				}
				catch(Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}
		}

		class ServerInfo
		{
			public static string[] CATEGORIES = new string[4] {"Real Server", "Devel Server", "DB Server", "WAS Server"};
			public static string PATH = AppDomain.CurrentDomain.BaseDirectory + "serverinfo.json";
			public static JObject jobj_root = new JObject();

			#region Instance Area
			public string name;
			public string ip;
			public string id;
			public string password;
			public ServerInfo(string _name, string _ip, string _id, string _password)
			{
				name = _name;
				ip = _ip;
				id = _id;
				password = _password;
			}
			private ServerInfo() { }
			#endregion
			
			public static void save()
			{
				FileContoller.write(ServerInfo.PATH, ServerInfo.jobj_root.ToString());
			}

			#region Class Area
			public static JObject ConvertToJson(ServerInfo[][] serverinfos)
			{
				/* 
				* 구조
					{
						"Real Server":	{
										serverinfo,
										serverinfo ...
										},
						"Devel Server":	{
										serverinfo,
										serverinfo ...
										},
						"DB Server":	{
										serverinfo,
										serverinfo ...
										},
						"WAS Server":	{
										serverinfo,
										serverinfo ...
										}
					}
				*/
				try
				{
					JObject jobj_root = new JObject();
					JProperty jprop;
					for(int i = 0; i < ServerInfo.CATEGORIES.Length; i++)
					{
						JObject jobj_serverinfos = new JObject();
						for(int j = 0; j < serverinfos[i].Length; j++)
						{
							jobj_serverinfos.Add(ConvertToJson(serverinfos[i][j]));
						}
						jprop = new JProperty(ServerInfo.CATEGORIES[i], jobj_serverinfos);
						jobj_root.Add(jprop);
					}
					return jobj_root;
				}
				catch(Exception e)
				{
					Console.WriteLine(e.Message);
				}

				return null;
			}
			public static JProperty ConvertToJson(ServerInfo serverinfo)
			{
				/* 
					구조
					"name" : { 
						"ip" : val,
						"id" : val,
						"password" : val
					}
				*/
				try
				{
					JObject jobj = new JObject();

					Type type = typeof(ServerInfo);
					FieldInfo[] f = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);

					for(int i = 1; i < f.Length; i++)
					{
						// property 만들기
						jobj.Add(new JProperty(f[i].Name, f[i].GetValue(serverinfo)));
					}
					JProperty jporp = new JProperty(serverinfo.name,  jobj);
					return jporp;
				}
				catch(Exception e)
				{
					Console.WriteLine(e.Message);
				}
				return null;
				//string json = "{\n";
				
				//Type type = typeof(ServerInfo);
				//FieldInfo[] f = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance);
				
				//for(int i = 0; ; i++)
				//{
				//	// property 만들기
				//	json += string.Format("    \"{0}\" : \"{1}\"", f[i].Name, f[i].GetValue(serverinfo));
				//	if(i >= f.Length - 1)
				//		break;
				//	json += ",\n";
				//}
				//json += "\n}";
				//return json;
			}

			public static List<ServerInfo>[] ConvertFromJson(JObject jobj_root)
			{
				if(jobj_root == null)
					return null;

				try
				{
					List<ServerInfo>[] serverinfos = new List<ServerInfo>[ServerInfo.CATEGORIES.Length];
					for(int i = 0; i < ServerInfo.CATEGORIES.Length; i++)
					{
						serverinfos[i] = new List<ServerInfo>();
					}

					Type type = typeof(ServerInfo);

					for(int idx_cate = 0; idx_cate < ServerInfo.CATEGORIES.Length; idx_cate++)
					{
						JObject jobj_serverinfos = jobj_root[ServerInfo.CATEGORIES[idx_cate]] as JObject;
						if(jobj_serverinfos == null)
							return null;

						foreach(var v in jobj_serverinfos.Properties())
						{
							ServerInfo serverinfo = ServerInfo.ConvertFromJson(v);
							serverinfos[idx_cate].Add(serverinfo);
						}
					}
					return serverinfos;
				}
				catch(Exception e)
				{
					Console.WriteLine(e.Message);
				}
				return null;
			}
			public static ServerInfo ConvertFromJson(JProperty jprop)
			{
				ServerInfo serverinfo = new ServerInfo();

				Type type = typeof(ServerInfo);

				JObject jobj_serverinfo = jprop.Value as JObject;
				if(jobj_serverinfo == null)
					return null;

				type.GetField("name").SetValue(serverinfo, jprop.Name);
				foreach(var v in jobj_serverinfo.Properties())
				{
					type.GetField(v.Name).SetValue(serverinfo, v.Value.ToString());
				}

				return serverinfo;
			}
			#endregion
		}
		class ServerInfoTextBlock : TextBlock
		{
			public ServerInfo serverinfo;
			public ServerInfoTextBlock(string _name, string _ip, string _id, string _pass)
			{
				serverinfo = new ServerInfo(_name, _ip, _id, _pass);
				this.HorizontalAlignment = HorizontalAlignment.Stretch;
				this.Text = _name;
			}
			public ServerInfoTextBlock(ServerInfo si)
			{
				serverinfo = si;
				this.HorizontalAlignment = HorizontalAlignment.Stretch;
				this.Text = si.name;
			}
		}
		class ServerMenuButtonChild : ListBox
		{
			public ServerMenuButton parent;
			public static ServerInfoTextBlock selected_server_info;

			public ServerMenuButtonChild()
			{
				this.Margin = new Thickness(20, 0, 0, 0);
				this.BorderBrush = null;

				this.ContextMenu = new ContextMenu();
				MenuItem item = new MenuItem();
				item.Click += BtnAdd_Click;
				item.Header = "Add";
				this.ContextMenu.Items.Add(item);

				item = new MenuItem();
				item.Click += BtnDel_Click;
				item.Header = "Delete";
				this.ContextMenu.Items.Add(item);


				//// 추가버튼
				//Button btn = new Button();
				//btn.Background = Brushes.White;
				//btn.Width = 20;
				//btn.Height = 20;
				//btn.VerticalAlignment = VerticalAlignment.Center;
				//btn.HorizontalAlignment = HorizontalAlignment.Left;
				//btn.Margin = new Thickness(5);
				//btn.Content = "+";
				//btn.Click += BtnAdd_Click;
				//this.Items.Add(btn);
			}
			private void BtnAdd_Click(object sender, RoutedEventArgs e)
			{
				Window_MakeSession wms = new Window_MakeSession();
				Point pt = this.PointToScreen(new Point(0, 0));
				wms.Left = pt.X;
				wms.Top = pt.Y;
				if(wms.ShowDialog() == true)
				{
					string name = wms.textBox_name.Text;
					string ip = wms.textBox_ip.Text;
					string id = wms.textBox_id.Text;
					string password = wms.textBox_password.Password;

					ServerInfoTextBlock si = new ServerInfoTextBlock(name, ip, id, password);
					this.Items.Add(si);

					JObject jobj = ServerInfo.jobj_root[parent.Content] as JObject;
					if(jobj == null)
						return;

					jobj.Add(ServerInfo.ConvertToJson(si.serverinfo));
					ServerInfo.save();
				}

			}
			private void BtnDel_Click(object sender, RoutedEventArgs e)
			{
				if(ServerMenuButtonChild.selected_server_info == null)
					return;

				JObject jobj = ServerInfo.jobj_root[parent.Content] as JObject;
				if(jobj == null)
					return;

				jobj.Remove(ServerMenuButtonChild.selected_server_info.serverinfo.name);
				ServerInfo.save();
				this.Items.Remove(ServerMenuButtonChild.selected_server_info);
				//ServerButtonChildren.selected_server_info.Remove();

			}

			protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
			{
				base.OnMouseDoubleClick(e);
				if(e.ChangedButton == MouseButton.Left)
				{
					ServerInfoTextBlock sitb = this.SelectedItem as ServerInfoTextBlock;
					if(sitb == null)
						return;

					Window_MakeSession wms = new Window_MakeSession();
					wms.textBox_name.Text = sitb.serverinfo.name;
					wms.textBox_ip.Text = sitb.serverinfo.ip;
					wms.textBox_id.Text = sitb.serverinfo.id;
					wms.textBox_password.Password = sitb.serverinfo.password;

					Point pt = this.PointToScreen(new Point(0, 0));
					wms.Left = pt.X;
					wms.Top = pt.Y;
					if(wms.ShowDialog() == true)
					{
						sitb.Text = sitb.serverinfo.name = wms.textBox_name.Text;
						sitb.serverinfo.ip = wms.textBox_ip.Text;
						sitb.serverinfo.id = wms.textBox_id.Text;
						sitb.serverinfo.password = wms.textBox_password.Password;

						ServerInfo.save();
					}
				}
			}
			protected override void OnSelectionChanged(SelectionChangedEventArgs e)
			{
				base.OnSelectionChanged(e);
				if(CommandView.current == null)
					return;

				selected_server_info = this.SelectedItem as ServerInfoTextBlock;
				if(selected_server_info == null)
				{
					CommandView.current.Visibility = Visibility.Hidden;
					return;
				}
				else
				{
					CommandView.current.Visibility = Visibility.Visible;
				}

				CommandView.refresh(selected_server_info);
			}
		}
		class ServerMenuButton : ToggleButton
		{
			static List<ServerMenuButton> group = new List<ServerMenuButton>();
			const double HEIGHT = 25;

			public ServerMenuButtonChild child;
			public ServerMenuButton(string header)
			{
				this.Content = header;
				this.Background = Brushes.White;
				this.Height = HEIGHT;
				this.HorizontalAlignment = HorizontalAlignment.Stretch;
				this.VerticalAlignment = VerticalAlignment.Bottom;

				this.child = new ServerMenuButtonChild();
				this.child.Visibility = Visibility.Collapsed;
				this.child.VerticalAlignment = VerticalAlignment.Top;
				this.child.parent = this;

				group.Add(this);
				for(int i = 0; i < group.Count; i++)
				{
					group[i].Margin = new Thickness(0, i * HEIGHT, 0, (group.Count - (i + 1)) * HEIGHT);
				}

				this.ContextMenu = new ContextMenu();
				MenuItem item = new MenuItem();
				item.Click += BtnAdd_Click;
				item.Header = "Add";
				this.ContextMenu.Items.Add(item);
			}
			private void BtnAdd_Click(object sender, RoutedEventArgs e)
			{
				Window_MakeSession wms = new Window_MakeSession();
				Point pt = this.PointToScreen(new Point(0, 0));
				wms.Left = pt.X;
				wms.Top = pt.Y;
				if(wms.ShowDialog() == true)
				{
					string name = wms.textBox_name.Text;
					string ip = wms.textBox_ip.Text;
					string id = wms.textBox_id.Text;
					string password = wms.textBox_password.Password;

					ServerInfoTextBlock si = new ServerInfoTextBlock(name, ip, id, password);
					this.child.Items.Add(si);

					JObject jobj = ServerInfo.jobj_root[this.Content] as JObject;
					if(jobj == null)
						return;

					jobj.Add(ServerInfo.ConvertToJson(si.serverinfo));
					ServerInfo.save();
				}

			}

			protected override void OnToggle()
			{
				for(int i = 0; i < group.Count; i++)
				{
					group[i].IsChecked = false;
				}
				base.OnToggle();
			}

			protected override void OnUnchecked(RoutedEventArgs e)
			{
				base.OnUnchecked(e);
				this.child.Visibility = Visibility.Collapsed;
			}
			protected override void OnChecked(RoutedEventArgs e)
			{
				base.OnChecked(e);
				int idx = group.IndexOf(this);
				int i;
				for(i = 0; i <= idx; i++)
				{
					group[i].VerticalAlignment = VerticalAlignment.Top;
				}
				for(; i < group.Count; i++)
				{
					group[i].VerticalAlignment = VerticalAlignment.Bottom;
				}

				ServerGrid.current.submenu.Margin = new Thickness(0, HEIGHT * (idx + 1), 0, HEIGHT * (group.Count - (idx + 1)));
				this.child.Visibility = Visibility.Visible;
				//Console.WriteLine(this.child.Items.Count);
			}
			protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
			{
				base.OnMouseRightButtonDown(e);
			}
		}
		class ServerGrid : Grid
		{
			public static ServerGrid current;
			public Grid submenu;
			public ServerGrid()
			{
				current = this;

				this.Margin = new Thickness(5);
				this.Background = null;

				Border bd = new Border();
				bd.BorderBrush = Brushes.Black;
				bd.BorderThickness = new Thickness(1);

				this.Children.Add(bd);

				submenu = new Grid();
				this.Children.Add(submenu);
			}
		}
		#endregion

		#region Server Command View
		void InitServerCommandView()
		{
			CommandView commandView;
			commandView = new CommandView();
			grid_second.Children.Add(commandView);
		}
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

				timer_read = new DispatcherTimer();
				timer_read.Interval = TimeSpan.FromSeconds(0.001);
				timer_read.Tick += Timer_read_Tick;
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
				if(ServerMenuButtonChild.selected_server_info == null)
					return;
				if(CommandView.current == null)
					return;

				string ip = ServerMenuButtonChild.selected_server_info.serverinfo.ip;
				string id = ServerMenuButtonChild.selected_server_info.serverinfo.id;
				string password = ServerMenuButtonChild.selected_server_info.serverinfo.password;
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
						//DownLoadJsonFile();
						UploadJsonFile();
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
				if(ServerMenuButtonChild.selected_server_info.serverinfo == null)
					return;

				string ip = ServerMenuButtonChild.selected_server_info.serverinfo.ip;
				string id = ServerMenuButtonChild.selected_server_info.serverinfo.id;
				string password = ServerMenuButtonChild.selected_server_info.serverinfo.password;
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
				if(ServerMenuButtonChild.selected_server_info.serverinfo == null)
					return;

				string ip = ServerMenuButtonChild.selected_server_info.serverinfo.ip;
				string id = ServerMenuButtonChild.selected_server_info.serverinfo.id;
				string password = ServerMenuButtonChild.selected_server_info.serverinfo.password;
				SftpClient sftp = new SftpClient(ip, id, password);
				sftp.Connect();

				string local_directory = AppDomain.CurrentDomain.BaseDirectory;
				string remote_directory = "/home/cofile/bin/";
				var files = sftp.ListDirectory(remote_directory);

				string[] paths = FileContoller.loadFile(local_directory, "*.json");
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

			static void clear()
			{
				if(current == null)
					return;

				current.textBlock_server_name.Text = "";
				current.textBox_command.Text = "";
				current.textBox_result.Text = "";
			}
			public static void refresh(ServerInfoTextBlock sitb)
			{
				if(current == null)
					return;

				CommandView.clear();
				current.textBlock_server_name.Text = sitb.serverinfo.name + " / " + sitb.serverinfo.ip + " / " + sitb.serverinfo.id /*+ " / " + sitb.serverinfo.password*/;
				
				if(CommandView.current == null)
					return;
				CommandView.current.textBox_command.Focus();
			}
		}
		#endregion


		#region Json Tree Area
		string root_path = AppDomain.CurrentDomain.BaseDirectory;

		void InitJsonFileView()
		{
			new JsonInfo(json_tree_view);
		}
		public void refreshJsonTree(JToken jtok_root)
		{
			JsonTreeViewItem root_jtree = JsonTreeViewItem.convertToTreeViewItem(jtok_root);
			if(root_jtree == null)
				return;

			// 삭제
			json_tree_view.Items.Clear();

			KeyTextBox tb = new KeyTextBox(JsonInfo.current.Filename, false);
			root_jtree.Header.Children.Insert(0, tb);
			json_tree_view.Items.Add(root_jtree);
		}

		private void OnClickButtonNewJsonFile(object sender, RoutedEventArgs e)
		{
			if(JsonInfo.current == null)
				return;

			JsonInfo.current.Clear();
			//if(JSonInfo.current.IsModify_tree)
			//{
			//	MessageBoxResult mbr = MessageBox.Show("변경사항을 저장하겠습니까?", "저장", MessageBoxButton.YesNoCancel);
			//	switch(mbr)
			//	{
			//		case MessageBoxResult.Yes:
			//			Btn_save_jtree_Click(sender, e);
			//			break;
			//		case MessageBoxResult.Cancel:
			//			return;
			//	}
			//}
			//JsonInfo.current.Jtok_root = new JObject();
			refreshJsonTree(new JObject());
		}
		private void OnClickButtonOpenJsonFile(object sender, RoutedEventArgs e)
		{
			if(JsonInfo.current == null)
				return;

			JsonInfo.current.Clear();
			OpenFileDialog ofd = new OpenFileDialog();

			ofd.InitialDirectory = root_path;

			if(JsonInfo.current.Path != null)
			{
				string dir_path = JsonInfo.current.Path.Substring(0, JsonInfo.current.Path.LastIndexOf('\\') + 1);
				DirectoryInfo d = new DirectoryInfo(dir_path);
				if(d.Exists)
					ofd.InitialDirectory = dir_path;
			}

			// 파일 열기
			ofd.Filter = "JSon Files (.json)|*.json";
			if(ofd.ShowDialog(this) == true)
			{
				Console.WriteLine(ofd.FileName);

				string json = FileContoller.read(ofd.FileName);
				refreshJsonTree(JsonController.parseJson(json));

				JsonInfo.current.Path = ofd.FileName;
			}
		}
		private void OnClickButtonSaveJsonFile(object sender, RoutedEventArgs e)
		{
			if(JsonInfo.current == null)
				return;

			if(JsonInfo.current.Path == null)
			{
				OnClickButtonOtherSaveJsonFile(sender, e);
				return;
			}

			FileInfo f = new FileInfo(JsonInfo.current.Path);
			if(!f.Exists)
			{
				OnClickButtonOtherSaveJsonFile(sender, e);
				return;
			}

			// root JsonTreeViewItem = TreeView.Items[0]
			JsonTreeViewItem root = json_tree_view.Items[0] as JsonTreeViewItem;
			if(root == null)
				return;

			JToken Jtok_root = JsonTreeViewItem.convertToJToken(root);
			FileContoller.write(JsonInfo.current.Path, Jtok_root.ToString());
		}
		private void OnClickButtonOtherSaveJsonFile(object sender, RoutedEventArgs e)
		{
			if(JsonInfo.current == null)
				return;

			SaveFileDialog sfd = new SaveFileDialog();

			sfd.InitialDirectory = root_path;

			if(JsonInfo.current.Path != null)
			{
				string dir_path = JsonInfo.current.Path.Substring(0, JsonInfo.current.Path.LastIndexOf('\\') + 1);
				DirectoryInfo d = new DirectoryInfo(dir_path);
				if(d.Exists)
					sfd.InitialDirectory = dir_path;
			}

			sfd.Filter = "JSon Files (.json)|*.json";
			if(sfd.ShowDialog(this) == true)
			{
				// root JsonTreeViewItem = TreeView.Items[0]
				JsonTreeViewItem root = json_tree_view.Items[0] as JsonTreeViewItem;
				if(root == null)
					return;

				JToken Jtok_root = JsonTreeViewItem.convertToJToken(root);
				FileContoller.write(sfd.FileName, Jtok_root.ToString());
				JsonInfo.current.Path = sfd.FileName;
			}
		}
		private void OnClickButtonViewJsonFile(object sender, RoutedEventArgs e)
		{
			if(JsonInfo.current == null || JsonInfo.current.Path == null)
				return;

			JsonTreeViewItem root = json_tree_view.Items[0] as JsonTreeViewItem;
			if(root == null)
				return;

			Window_ViewFile w = new Window_ViewFile(JsonTreeViewItem.convertToJToken(root).ToString(), JsonInfo.current.Path);
			//Window_ViewFile w = new Window_ViewFile(FileContoller.read(JsonInfo.current.Path), JsonInfo.current.Path);

			if(w.ShowDialog() == true)
			{
				refreshJsonTree(JsonController.parseJson(w.tb_file.Text));
			}
		}
		private void OnClickButtonCancelJsonFile(object sender, RoutedEventArgs e)
		{
			if(JsonInfo.current == null || JsonInfo.current.Path == null)
				return;

			string dir_path = JsonInfo.current.Path.Substring(0, JsonInfo.current.Path.LastIndexOf('\\') + 1);
			DirectoryInfo d = new DirectoryInfo(dir_path);
			if(!d.Exists)
				return;

			string json = FileContoller.read(JsonInfo.current.path);
			refreshJsonTree(JsonController.parseJson(json));
		}
		#endregion
	}

}