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
using CofileUI.Classes;
using CofileUI.UserControls;

namespace CofileUI.Windows
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
			//string[] dll_path = new string[] { @"bin\EntityFramework.dll"
			//									,@"bin\EntityFramework.SqlServer.dll"
			//									,@"bin\MahApps.Metro.dll"
			//									,@"bin\MahApps.Metro.IconPacks.dll"
			//									,@"bin\Newtonsoft.Json.dll"
			//									,@"bin\Renci.SshNet.dll"
			//									,@"bin\System.Data.SQLite.dll"
			//									,@"bin\System.Data.SQLite.EF6.dll"
			//									,@"bin\System.Data.SQLite.Linq.dll"
			//									,@"bin\System.Windows.Interactivity.dll"};

			current = this;
			InitializeComponent();
			this.Closed += test4_Closed;

			this.PreviewKeyDown += WindowMain_PreviewKeyDown;
			this.PreviewMouseDown += WindowMain_PreviewMouseDown;
			this.PreviewMouseWheel += WindowMain_PreviewMouseWheel;
			DispatcherTimer DisconnectTimeout = new DispatcherTimer();
			DisconnectTimeout.Interval = new TimeSpan(0, 0, 0, 5);
			DisconnectTimeout.Tick += DisconnectTimeout_Tick;
			DisconnectTimeout.Start();
		}


		static DateTime LastInputTime = DateTime.Now;
		private void WindowMain_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			LastInputTime = DateTime.Now;
		}
		private void WindowMain_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			LastInputTime = DateTime.Now;
		}
		private void WindowMain_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			LastInputTime = DateTime.Now;
		}
		
		private void DisconnectTimeout_Tick(object sender, EventArgs e)
		{
			if(MainSettings.IsTimeOut && SSHController.IsConnected
				&& LastInputTime.AddMinutes(MainSettings.SessionTimeOut) < DateTime.Now)
			//&& LastInputTime.AddSeconds(5) < DateTime.Now)
			{
				WindowMain.current.ShowMessageDialog("Session Timeout", MainSettings.SessionTimeOut + "분 간 입력이 없어 연결이 종료됩니다.", MessageDialogStyle.Affirmative, DisconnectTimeout);
			}
			//Console.WriteLine("LastInputTime = " + LastInputTime);
		}
		private void DisconnectTimeout()
		{
			SSHController.DisConnect();
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


		#region View Update
		bool bUpdateDataBase = true;
		bool bUpdateLinuxTree = true;
		bool bUpdateConfigFile = true;
		public void bUpdateInit(bool val)
		{
			bUpdateDataBase = val;
			bUpdateLinuxTree = val;
			bUpdateConfigFile = val;
			switch(tabControl.SelectedIndex)
			{
				case 1:
					bUpdateConfigFile = true;
					break;
				//case 0:
				//	bUpdateLinuxTree = true;
				//	break;
				//case 2:
				//case 3:
				//	bUpdateDataBase = true;
				//	break;
			}
		}
		string changed_server_name = "";
		public string Changed_server_name { get { return changed_server_name; }
			set
			{
				changed_server_name = value;
				if(value == "")
				{
					label_serverinfo.Content = "";

				}
				else
				{
					label_serverinfo.Content = "[ " + changed_server_name + " ] is Connected from [ " + ServerList.selected_serverinfo_panel.Serverinfo.id + " ]";
				}
			}
		}

		// 상단 탭이 바뀌었을때 작동
		int idx_tab_before_change = 0;
		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if(e.Source != tabControl)
				return;

			if(idx_tab_before_change == 1 && tabControl.SelectedIndex != 1)
				UserControls.ConfigOption.current.ConfirmSave();
			//Console.WriteLine("tabControl.SelectedIndex = " + tabControl.SelectedIndex);
			//Console.WriteLine("\te.OriginalSource = " + e.OriginalSource);
			//if(tabControl.SelectedIndex != 1)
			//{
			//	UserControls.ConfigOption.current.ConfirmSave();
			//	if(UserControls.ConfigOptions.ConfigOptions.bChanged)
			//	{
			//		tabControl.SelectedIndex = idx_tab_before_change;
			//		return;
			//	}
			//}

			TabUpdate();
			idx_tab_before_change = tabControl.SelectedIndex;
		}
		private void TabUpdate()
		{
			SSHController.ReConnect();
			if(SSHController.IsConnected)
			{
				if(!bUpdateLinuxTree && UserControls.Cofile.current != null && tabControl.SelectedIndex == 0)
				{
					UserControls.Cofile.current.Refresh();
					bUpdateLinuxTree = true;
				}
				if(!bUpdateDataBase && (tabControl.SelectedIndex == 2 || tabControl.SelectedIndex == 3))
				//&& idx_tab_before_change != 2 && idx_tab_before_change != 3
				//&& (tabControl.SelectedIndex == 2 || tabControl.SelectedIndex == 3))
				{
					//UserControls.DataBaseInfo.RefreshUi(Changed_server_name);
					UserControls.DataBaseInfo.RefreshUi();
					bUpdateDataBase = true;
				}
				if(!bUpdateConfigFile && UserControls.ConfigOption.current != null && tabControl.SelectedIndex == 1)
				{
					UserControls.ConfigOption.current.Clear();
					bUpdateConfigFile = true;
				}
			}
		}

		// 서버메뉴리스트에서 서버를 컨넥팅 동작을 할 때 작동
		public void Refresh(string _changed_server_name)
		{
			TabUpdate();
			if(!SSHController.IsConnected)
				bUpdateInit(true);
			//switch(tabControl.SelectedIndex)
			//{
			//	case 0:
			//		if(UserControls.Cofile.current != null)
			//		{
			//			UserControls.Cofile.current.Refresh();
			//			//bUpdateLinuxTree = true;
			//		}
			//		break;
			//	case 1:
			//		if(UserControls.ConfigJsonTree.current != null)
			//		{
			//			UserControls.ConfigJsonTree.current.Refresh();
			//			//bUpdateConfigFile = true;
			//		}
			//		break;
			//	case 2:
			//	case 3:
			//		//UserControls.DataBaseInfo.RefreshUi(_changed_server_name);
			//		UserControls.DataBaseInfo.RefreshUi();
			//		//bUpdateDataBase = true;
			//		break;
			//}
			//Changed_server_name = _changed_server_name;
		}
		public void Clear()
		{
			if(Cofile.current != null)
				Cofile.current.Clear();
			if(ConfigOption.current != null)
				ConfigOption.current.Clear();
			if(Sqlite_LogTable.current != null)
				Sqlite_LogTable.current.Clear();
			if(Sqlite_StatusTable.current != null)
				Sqlite_StatusTable.current.Clear();
			if(Status.current != null)
				Status.current.Clear();

			Changed_server_name = "";
		}
		#endregion

		public delegate void CallBack();
		public void ShowMessageDialog(string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, CallBack affirmative_callback = null, CallBack negative_callback = null, CallBack alwayse_callback = null, MetroDialogSettings settings = null)
		{
			//MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Theme;
			//var mySettings = new MetroDialogSettings()
			//{
			//	AffirmativeButtonText = "Ok",
			//	//NegativeButtonText = "Go away!",
			//	FirstAuxiliaryButtonText = "Cancel",
			//	//ColorScheme = UseAccentForDialogsMenuItem.IsChecked ? MetroDialogColorScheme.Accented : MetroDialogColorScheme.Theme
			//};

			MessageDialogResult result = this.ShowModalMessageExternal(title, message, style, settings);
			//MessageDialogResult result = await this.ShowMessageAsync(title, message, style);

			if(affirmative_callback != null && result == MessageDialogResult.Affirmative)
				affirmative_callback();

			if(negative_callback != null && result == MessageDialogResult.Negative)
				negative_callback();

			if(alwayse_callback != null)
				alwayse_callback();
		}

		private void OnClickMainSetting(object sender, RoutedEventArgs e)
		{
			Window_MainSetting wm = new Window_MainSetting();
			Point pt = this.PointToScreen(new Point(0, 0));
			wm.Left = pt.X + this.ActualWidth / 2 - wm.Width / 2;
			wm.Top = pt.Y + this.ActualHeight / 2 - wm.Height / 2;
			if(wm.ShowDialog() == true)
				;
		}
	}
}