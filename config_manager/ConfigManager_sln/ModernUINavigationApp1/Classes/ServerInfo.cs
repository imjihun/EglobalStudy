﻿using ModernUINavigationApp1.Pages;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Manager_proj_3
{
	#region Server Panel
	public class ServerInfo
	{
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
			if(!FileContoller.write(ServerInfo.PATH, ServerInfo.jobj_root.ToString()))
			{
				string caption = "save error";
				string message = "serverinfo.json 파일을 저장하는데 문제가 생겼습니다.";
				MessageBox.Show(message, caption);
				Console.WriteLine("[" + caption + "] " + message);
			}
		}

		#region Class Area
		public static JObject ConvertToJson(ServerInfo[][] serverinfos)
		{
			/* 
			* 구조
				{
					"server tab":	{
									serverinfo,
									serverinfo ...
									},
					"server tab":	{
									serverinfo,
									serverinfo ...
									}
									...
				}
			*/
			try
			{
				JObject jobj_root = new JObject();
				JProperty jprop;
				for(int i = 0; i < ServerPanel.current.Children.Count; i++)
				{
					ServerMenuButton smbtn = ServerPanel.current.Children[i] as ServerMenuButton;
					if(smbtn == null)
						continue;

					JObject jobj_serverinfos = new JObject();
					for(int j = 0; j < serverinfos[i].Length; j++)
					{
						jobj_serverinfos.Add(ConvertToJson(serverinfos[i][j]));
					}
					jprop = new JProperty(smbtn.Content.ToString(), jobj_serverinfos);
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

		public static ServerPanel ConvertFromJson(JObject jobj_root)
		{
			if(jobj_root == null)
				return null;

			ServerPanel servergrid = new ServerPanel();
			try
			{
				foreach(var v in jobj_root.Properties())
				{
					JObject jobj_server_menu = v.Value as JObject;
					if(jobj_server_menu == null)
						continue;

					ServerMenuButton smbtn = new ServerMenuButton(v.Name);
					servergrid.Children.Add(smbtn);
					ServerPanel.SubPanel.Children.Add(smbtn.child);

					foreach(var jprop_server_info in jobj_server_menu.Properties())
					{
						ServerInfo serverinfo = ServerInfo.ConvertFromJson(jprop_server_info);
						smbtn.child.Items.Add(new ServerInfoTextBlock(serverinfo));
					}
				}
				return servergrid;
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

	public class ServerInfoTextBlock : TextBlock
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

	public class ServerList : ListBox
	{
		public ServerMenuButton parent;
		public static ServerInfoTextBlock selected_serverinfo_textblock;

		public ServerList()
		{
			this.Margin = new Thickness(20, 0, 0, 0);
			this.BorderBrush = null;

			this.ContextMenu = new ContextMenu();
			MenuItem item = new MenuItem();
			item.Click += BtnAddServer_Click;
			item.Header = "Add Server";
			this.ContextMenu.Items.Add(item);

			item = new MenuItem();
			item.Click += BtnDelServer_Click;
			item.Header = "Delete Server";
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
		private void BtnAddServer_Click(object sender, RoutedEventArgs e)
		{
			Window_AddServer wms = new Window_AddServer();
			Point pt = this.PointToScreen(new Point(0, 0));
			wms.Left = pt.X;
			wms.Top = pt.Y;
			if(wms.ShowDialog() == true)
			{
				string name = wms.textBox_name.Text;
				string ip = wms.textBox_ip.Text;
				string id = wms.textBox_id.Text;
				string password = wms.textBox_password.Password;

				try
				{
					JObject jobj = ServerInfo.jobj_root[parent.Content] as JObject;
					if(jobj == null)
						return;

					ServerInfoTextBlock si = new ServerInfoTextBlock(name, ip, id, password);
					jobj.Add(ServerInfo.ConvertToJson(si.serverinfo));
					this.Items.Add(si);
					ServerInfo.save();
				}
				catch(Exception ex)
				{
					//Log.PrintError(ex.Message, "Add Server", Home.m_wnd.richTextBox_status);
					Log.PrintError("서버 이름이 중복됩니다.\r", "Add Server", Home.m_wnd.richTextBox_status);
				}
			}

		}
		private void BtnDelServer_Click(object sender, RoutedEventArgs e)
		{
			if(ServerList.selected_serverinfo_textblock == null)
				return;

			try
			{
				JObject jobj = ServerInfo.jobj_root[parent.Content] as JObject;
				if(jobj == null)
					return;

				jobj.Remove(ServerList.selected_serverinfo_textblock.serverinfo.name);
				ServerInfo.save();
				this.Items.Remove(ServerList.selected_serverinfo_textblock);
				//ServerButtonChildren.selected_server_info.Remove();
			}
			catch(Exception ex)
			{
				Log.PrintError(ex.Message, "Del Server", Home.m_wnd.richTextBox_status);
			}

		}

		protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
		{
			base.OnMouseDoubleClick(e);
			if(e.ChangedButton == MouseButton.Left)
			{
				ServerInfoTextBlock sitb = this.SelectedItem as ServerInfoTextBlock;
				if(sitb == null)
					return;

				Window_AddServer wms = new Window_AddServer();
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

			selected_serverinfo_textblock = this.SelectedItem as ServerInfoTextBlock;
			if(selected_serverinfo_textblock == null)
			{
				CommandView.current.Visibility = Visibility.Hidden;
				return;
			}
			else
			{
				CommandView.current.Visibility = Visibility.Visible;
				LinuxTreeViewItem.Refresh();
			}

			CommandView.refresh(selected_serverinfo_textblock);
		}
	}

	public class ServerMenuButton : ToggleButton
	{
		public static List<ServerMenuButton> group = new List<ServerMenuButton>();
		const double HEIGHT = 25;
		public ServerList child;
		public ServerMenuButton(string header)
		{
			this.Content = header;
			this.Background = Brushes.White;
			this.Height = HEIGHT;
			this.HorizontalAlignment = HorizontalAlignment.Stretch;
			this.VerticalAlignment = VerticalAlignment.Bottom;

			this.child = new ServerList();
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
			item.Click += BtnAddServer_Click;
			item.Header = "Add Server";
			this.ContextMenu.Items.Add(item);

			item = new MenuItem();
			item.Click += BtnAddServerMenu_Click;
			item.Header = "Add Server Menu";
			this.ContextMenu.Items.Add(item);

			item = new MenuItem();
			item.Click += BtnDelServerMenu_Click;
			item.Header = "Del Server Menu";
			this.ContextMenu.Items.Add(item);
		}
		private void BtnAddServer_Click(object sender, RoutedEventArgs e)
		{
			Window_AddServer wms = new Window_AddServer();
			Point pt = this.PointToScreen(new Point(0, 0));
			wms.Left = pt.X;
			wms.Top = pt.Y;
			if(wms.ShowDialog() == true)
			{
				string name = wms.textBox_name.Text;
				string ip = wms.textBox_ip.Text;
				string id = wms.textBox_id.Text;
				string password = wms.textBox_password.Password;


				try
				{
					JObject jobj = ServerInfo.jobj_root[this.Content] as JObject;
					if(jobj == null)
						return;

					ServerInfoTextBlock si = new ServerInfoTextBlock(name, ip, id, password);
					jobj.Add(ServerInfo.ConvertToJson(si.serverinfo));
					this.child.Items.Add(si);
					ServerInfo.save();
				}
				catch(Exception ex)
				{
					//Log.PrintError(ex.Message, "Add Server", Home.m_wnd.richTextBox_status);
					Log.PrintError("서버 이름이 중복됩니다.\r", "Add Server", Home.m_wnd.richTextBox_status);
				}
			}
		}
		private void BtnAddServerMenu_Click(object sender, RoutedEventArgs e)
		{
			Window_AddServerMenu wms = new Window_AddServerMenu();
			Point pt = this.PointToScreen(new Point(0, 0));
			wms.Left = pt.X;
			wms.Top = pt.Y;
			if(wms.ShowDialog() == true)
			{
				string server_menu_name = wms.textBox_name.Text;


				if(ServerInfo.jobj_root == null)
					return;
				try
				{
					ServerInfo.jobj_root.Add(new JProperty(server_menu_name, new JObject()));
					ServerInfo.save();

					ServerMenuButton smbtn = new ServerMenuButton(server_menu_name);
					ServerPanel.current.Children.Add(smbtn);
					ServerPanel.SubPanel.Children.Add(smbtn.child);
				}
				catch(Exception ex)
				{
					//Log.PrintError(ex.Message, "Add Server Menu", Home.m_wnd.richTextBox_status);
					Log.PrintError("서버 메뉴 이름이 중복됩니다.\r", "Add Server Menu", Home.m_wnd.richTextBox_status);
				}
			}
		}
		private void BtnDelServerMenu_Click(object sender, RoutedEventArgs e)
		{
			if(ServerInfo.jobj_root == null)
				return;

			try
			{
				ServerInfo.jobj_root.Remove(this.Content.ToString());
				ServerInfo.save();

				ServerPanel.SubPanel.Children.Remove(this.child);
				ServerPanel.current.Children.Remove(this);
				group.Remove(this);

				for(int i = 0; i < group.Count; i++)
				{
					group[i].Margin = new Thickness(0, i * HEIGHT, 0, (group.Count - (i + 1)) * HEIGHT);
				}
			}
			catch(Exception ex)
			{
				Log.PrintError(ex.Message, "Del Server Menu", Home.m_wnd.richTextBox_status);
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
			Console.WriteLine(idx);
			int i;
			for(i = 0; i <= idx; i++)
			{
				group[i].VerticalAlignment = VerticalAlignment.Top;
			}
			for(; i < group.Count; i++)
			{
				group[i].VerticalAlignment = VerticalAlignment.Bottom;
			}

			ServerPanel.SubPanel.Margin = new Thickness(0, HEIGHT * (idx + 1), 0, HEIGHT * (group.Count - (idx + 1)));
			this.child.Visibility = Visibility.Visible;
			//Console.WriteLine(this.child.Items.Count);
		}
		protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseRightButtonDown(e);
		}
	}

	public class ServerPanel : Grid
	{
		public static ServerPanel current;

		#region ServerInfoPanel
		public class ServerInfoPanel : Grid
		{
			public ServerInfoPanel()
			{
			}
		}
		public static ServerInfoPanel SubPanel;
		#endregion

		public ServerPanel()
		{
			current = this;
			this.Background = null;

			SubPanel = new ServerInfoPanel();
			this.Children.Add(SubPanel);
		}
	}
	#endregion
}
