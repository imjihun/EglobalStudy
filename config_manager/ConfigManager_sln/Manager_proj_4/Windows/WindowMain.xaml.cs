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
using Renci.SshNet.Sftp;
using MahApps.Metro.Controls;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using Manager_proj_4.Classes;

namespace Manager_proj_4
{
	/// <summary>
	/// test4.xaml에 대한 상호 작용 논리
	/// </summary>
	public static class style
	{
		public static Accent currentAccent = ThemeManager.GetAccent("Blue");
		public static AppTheme currentAppTheme = ThemeManager.GetAppTheme("White");
	}
	public partial class WindowMain : MetroWindow
	{
		public static WindowMain current;
		public WindowMain()
		{
			current = this;
			InitializeComponent();
			this.Closed += test4_Closed;

			InitServerTab();
			DispatcherTimer tm = new DispatcherTimer();
		}
		private void TextBox_TextChanged_ScrollToEnd(object sender, TextChangedEventArgs e)
		{
			//textBox_status.ScrollToEnd();
			richTextBox_status.ScrollToEnd();
		}

		public static bool bCtrl = false;
		public static bool bShift = false;
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if(e.Key == Key.LeftCtrl)
				bCtrl = true;
			else if(e.Key == Key.LeftShift)
				bShift = true;
		}
		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			if(e.Key == Key.LeftCtrl)
				bCtrl = false;
			else if(e.Key == Key.LeftShift)
				bShift = false;
		}
		
		private void test4_Closed(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
		}

		#region Server Menu Class
		void InitServerTab()
		{
			// serverinfo.json 파일 로드
			FileInfo fi = new FileInfo(ServerInfo.PATH);
			if(fi.Exists)
			{
				string json = FileContoller.read(ServerInfo.PATH);
				try
				{
					ServerInfo.jobj_root = JObject.Parse(json);
					ServerPanel panel_server = ServerInfo.ConvertFromJson(ServerInfo.jobj_root);

					grid_server.Children.Add(panel_server);
				}
				catch(Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}
			else
			{
				try
				{
					ServerInfo.jobj_root = new JObject(new JProperty("Server", new JObject()));
					ServerPanel panel_server = ServerInfo.ConvertFromJson(ServerInfo.jobj_root);

					grid_server.Children.Add(panel_server);
				}
				catch(Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}

			if(ServerMenuButton.group.Count > 0)
				ServerMenuButton.group[0].IsChecked = true;
		}

		#endregion

		#region View Update
		bool bUpdateDataBase = false;
		bool bUpdateLinuxTree = false;
		string changed_server_name = "";

		// 상단 탭이 바뀌었을때 작동
		int idx_tab_before_change = 0;
		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if(!bUpdateLinuxTree && UserControls.Cofile.current != null)
			{
				UserControls.Cofile.current.Refresh();
				bUpdateLinuxTree = true;
			}
			if(!bUpdateDataBase && idx_tab_before_change != 2 && idx_tab_before_change != 3
				&& (tabControl.SelectedIndex == 2 || tabControl.SelectedIndex == 3))
			{
				UserControls.DataBaseInfo.RefreshUi(changed_server_name);
				bUpdateDataBase = true;
			}
			idx_tab_before_change = tabControl.SelectedIndex;
		}

		// 서버메뉴리스트 선택이 바뀌었을때 작동
		public void Refresh(string _changed_server_name)
		{
			bUpdateDataBase = false;
			bUpdateLinuxTree = false;
			switch(tabControl.SelectedIndex)
			{
				case 0:
					if(UserControls.Cofile.current != null)
					{
						UserControls.Cofile.current.Refresh();
						bUpdateLinuxTree = true;
					}
					break;
				case 1:
					break;
				case 2:
				case 3:
					UserControls.DataBaseInfo.RefreshUi(_changed_server_name);
					bUpdateDataBase = true;
					break;
			}
			changed_server_name = _changed_server_name;
		}
		#endregion

		public delegate void CallBack();
		public async void ShowMessageDialog(string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, CallBack affirmative_callback = null, CallBack alwayse_callback = null)
		{
			//MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Theme;
			//var mySettings = new MetroDialogSettings()
			//{
			//	AffirmativeButtonText = "Ok",
			//	//NegativeButtonText = "Go away!",
			//	FirstAuxiliaryButtonText = "Cancel",
			//	//ColorScheme = UseAccentForDialogsMenuItem.IsChecked ? MetroDialogColorScheme.Accented : MetroDialogColorScheme.Theme
			//};

			MessageDialogResult result = this.ShowModalMessageExternal(title, message, style);
			//MessageDialogResult result = await this.ShowMessageAsync(title, message, style);

			if(affirmative_callback != null && result == MessageDialogResult.Affirmative)
				affirmative_callback();

			if(alwayse_callback != null)
				alwayse_callback();
		}
	}
}