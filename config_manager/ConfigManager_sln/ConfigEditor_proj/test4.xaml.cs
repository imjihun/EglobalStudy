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

namespace ConfigEditor_proj
{
	/// <summary>
	/// test4.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class test4 : Window
	{
		public static test4 wnd;
		public test4()
		{
			wnd = this;

			InitializeComponent();
			this.Closed += test4_Closed;

			initJsonTab();
			initServerTab();

			test4.wnd.grid_second.Visibility = Visibility.Hidden;
		}

		private void test4_Closed(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
		}

		#region Server Area
		void initServerTab()
		{
			ServerMenuButton smbtn;
			smbtn = new ServerMenuButton("Real Server");
			grid_server.Children.Add(smbtn);
			grid_server.Children.Add(smbtn.child);

			smbtn = new ServerMenuButton("Devel Server");
			grid_server.Children.Add(smbtn);
			grid_server.Children.Add(smbtn.child);

			smbtn = new ServerMenuButton("DB Server");
			grid_server.Children.Add(smbtn);
			grid_server.Children.Add(smbtn.child);

			smbtn = new ServerMenuButton("WAS Server");
			grid_server.Children.Add(smbtn);
			grid_server.Children.Add(smbtn.child);

			button_send.Click += Button_send_Click;
		}

		private void Button_send_Click(object sender, RoutedEventArgs e)
		{
			if(ServerButtonChildren.selected_server_info == null)
				return;

			string ip = ServerButtonChildren.selected_server_info.ip;
			string id = ServerButtonChildren.selected_server_info.id;
			string password = ServerButtonChildren.selected_server_info.password;
			string command = textBox_command.Text;

			Console.WriteLine("work");
			try
			{
				SshClient client = new SshClient(ip, 22, id, password);
				client.Connect();
				SshCommand x = client.RunCommand(command);
				client.Disconnect();

				textBlock_result.Text = "";
				if(x.ExitStatus == 0)
				{
					textBlock_result.Text = x.Result;
				}
				else
				{
					textBlock_result.Text = "ExitStatus = " + x.ExitStatus + "\n";
					textBlock_result.Text += "Error = " + x.Error;
				}
				Console.WriteLine();
				Console.WriteLine("CommandText = " + x.CommandText);
				Console.WriteLine("CommandTimeout = " + x.CommandTimeout);
				Console.WriteLine("Error = " + x.Error);
				Console.WriteLine("ExitStatus = " + x.ExitStatus);
				Console.WriteLine("Result = " + x.Result);
				Console.WriteLine("OutputStream = " + x.OutputStream);
				Console.WriteLine("ExtendedOutputStream = " + x.ExtendedOutputStream);
				Console.WriteLine();
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			Console.WriteLine("finish");
		}

		class ServerInfoTextBlock : TextBlock
		{
			public string ip;
			public string id;
			public string password;
			public ServerInfoTextBlock(string _name, string _ip, string _id, string _pass)
			{
				this.Text = _name;
				ip = _ip;
				id = _id;
				password = _pass;
			}
		}
		class ServerButtonChildren : ListBox
		{
			public static ServerInfoTextBlock selected_server_info;

			public ServerButtonChildren()
			{
				this.Margin = new Thickness(20, 0, 0, 0);
				this.BorderBrush = null;

				Button btn = new Button();
				btn.Background = Brushes.White;
				btn.Width = 50;
				btn.Height = 20;
				btn.VerticalAlignment = VerticalAlignment.Center;
				btn.HorizontalAlignment = HorizontalAlignment.Left;
				btn.Margin = new Thickness(5);
				btn.Content = "Add";
				btn.Click += Btn_Click;

				this.Items.Add(btn);
			}

			private void Btn_Click(object sender, RoutedEventArgs e)
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
					string password = wms.textBox_password.Text;

					ServerInfoTextBlock si = new ServerInfoTextBlock(name, ip, id, password);
					this.Items.Add(si);
				}

			}

			protected override void OnSelectionChanged(SelectionChangedEventArgs e)
			{
				//ServerInfoTextBlock tb = this.SelectedItem as ServerInfoTextBlock;
				//if(tb == null)
				//	return;

				//selectedItem = tb;
				selected_server_info = this.SelectedItem as ServerInfoTextBlock;
				if(selected_server_info == null)
				{
					test4.wnd.grid_second.Visibility = Visibility.Hidden;
					return;
				}
				else
				{
					test4.wnd.grid_second.Visibility = Visibility.Visible;
				}
			}
		}
		class ServerMenuButton : ToggleButton
		{
			static List<ServerMenuButton> group = new List<ServerMenuButton>();

			const double HEIGHT = 20;

			public ServerButtonChildren child;
			public ServerMenuButton(string header)
			{
				this.Content = header;
				this.Background = Brushes.White;
				this.Height = HEIGHT;
				this.HorizontalAlignment = HorizontalAlignment.Stretch;
				this.VerticalAlignment = VerticalAlignment.Bottom;

				this.child = new ServerButtonChildren();
				this.child.Visibility = Visibility.Collapsed;

				group.Add(this);
				for(int i = 0; i < group.Count; i++)
				{
					group[i].Margin = new Thickness(0, i * HEIGHT, 0, (group.Count - (i + 1)) * HEIGHT);
					Console.WriteLine(group[i].Margin.Top);
				}
				Console.WriteLine();
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

				this.child.Margin = new Thickness(0, HEIGHT * (idx + 1), 0, HEIGHT * (group.Count - (idx + 1)));
				this.child.Visibility = Visibility.Visible;
				Console.WriteLine(this.child.Items.Count);
			}
		}



		class ServerGrid : Grid
		{
			public ServerGrid()
			{
				Border bd = new Border();
				bd.BorderBrush = Brushes.Black;
				bd.BorderThickness = new Thickness(1);

				this.Children.Add(bd);
			}
		}
		#endregion


		#region Json Area
		string root_path = AppDomain.CurrentDomain.BaseDirectory;

		void initJsonTab()
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