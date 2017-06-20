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
		bool bUpdateDataBase = false;
		bool bUpdateLinuxTree = false;
		bool bUpdateConfigFile = false;
		string changed_server_name = "";
		string Changed_server_name { get { return changed_server_name; } set {
				changed_server_name = value;
				label_serverinfo.Content = "[" + changed_server_name + "] is Connected from " + ServerList.selected_serverinfo_textblock.serverinfo.id; } }

		// 상단 탭이 바뀌었을때 작동
		int idx_tab_before_change = 0;
		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if(e.Source != tabControl)
				return;

			if(!bUpdateLinuxTree && UserControls.Cofile.current != null)
			{
				UserControls.Cofile.current.Refresh();
				bUpdateLinuxTree = true;
			}
			if(!bUpdateDataBase && idx_tab_before_change != 2 && idx_tab_before_change != 3
				&& (tabControl.SelectedIndex == 2 || tabControl.SelectedIndex == 3))
			{
				UserControls.DataBaseInfo.RefreshUi(Changed_server_name);
				bUpdateDataBase = true;
			}
			if(!bUpdateConfigFile && UserControls.ConfigJsonTree.current != null)
			{
				UserControls.ConfigJsonTree.current.Refresh();
				bUpdateConfigFile = true;
			}
			idx_tab_before_change = tabControl.SelectedIndex;
		}

		// 서버메뉴리스트 선택이 바뀌었을때 작동 View Update
		public void Refresh(string _changed_server_name)
		{
			bUpdateDataBase = false;
			bUpdateLinuxTree = false;
			bUpdateConfigFile = false;

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
					if(UserControls.ConfigJsonTree.current != null)
					{
						UserControls.ConfigJsonTree.current.Refresh();
						bUpdateConfigFile = true;
					}
					break;
				case 2:
				case 3:
					UserControls.DataBaseInfo.RefreshUi(_changed_server_name);
					bUpdateDataBase = true;
					break;
			}
			Changed_server_name = _changed_server_name;
		}
		public void Clear()
		{
			Cofile.current.Clear();
		}
		#endregion

		public delegate void CallBack();
		public void ShowMessageDialog(string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, CallBack affirmative_callback = null, CallBack alwayse_callback = null)
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

		









		//public void initTest()
		//{
		//	SftpFile[] files = SSHController.PullListInDirectory(@"/home/cofile/");
		//	for(int i = 0; i < files.Length; i++)
		//	{
		//		if(files[i].Name == "var")
		//		{
		//			LinuxTree lt = new LinuxTree(files[i]);
		//			trv_test.Items.Add(lt);
		//			lt.LoadChild();
		//			Console.WriteLine("lt.Childs.Count = " + lt.Childs.Count);
		//		}
		//	}
		//}

		//private void trv_test_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		//{
		//	TreeView tv = sender as TreeView;
		//	LinuxTree lt = tv.SelectedItem as LinuxTree;
		//	lt.LoadChild();
		//	//Console.WriteLine();
		//}
	}
}