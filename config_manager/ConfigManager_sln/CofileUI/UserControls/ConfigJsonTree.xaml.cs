using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using CofileUI.Classes;
using CofileUI.Windows;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CofileUI.UserControls
{
	/// <summary>
	/// ConfigJsonTree.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ConfigJsonTree : UserControl
	{
		public static ConfigJsonTree current;
		public ConfigJsonTree()
		{
			current = this;
			InitializeComponent();
			InitJsonFileView();
		}
		#region Json Tree Area
		static string DIR = @"config\";
		private static string root_path_local = AppDomain.CurrentDomain.BaseDirectory + DIR;
		public static string curRootPathLocal = root_path_local;
		public static string CurRootPathLocal {
			get
			{
				curRootPathLocal = root_path_local;
				if(ServerList.selected_serverinfo_textblock == null
					|| ServerList.selected_serverinfo_textblock.serverinfo == null)
				{ }
				else
				{
					if(ServerList.selected_serverinfo_textblock.serverinfo.name != null && ServerList.selected_serverinfo_textblock.serverinfo.name != "")
						curRootPathLocal += ServerList.selected_serverinfo_textblock.serverinfo.name + @"\";
					if(ServerList.selected_serverinfo_textblock.serverinfo.id != null && ServerList.selected_serverinfo_textblock.serverinfo.id != "")
						curRootPathLocal += ServerList.selected_serverinfo_textblock.serverinfo.id + @"\";
				}

				return curRootPathLocal;
			}
		}

		bool InitJsonFileView()
		{
			return FileContoller.CreateDirectory(CurRootPathLocal);
		}
		int RemoveConfigFile(string path)
		{
			FileContoller.DirectoryDelete(path);
			return 0;
		}
		public void InitOpenFile()
		{
			curRootPathLocal = root_path_local;
			if(ServerList.selected_serverinfo_textblock == null
				|| ServerList.selected_serverinfo_textblock.serverinfo == null)
			{ }
			else
				curRootPathLocal += ServerList.selected_serverinfo_textblock.serverinfo.name + @"\" + ServerList.selected_serverinfo_textblock.serverinfo.id + @"\";

			RemoveConfigFile(CurRootPathLocal);
			SSHController.GetConfig(CurRootPathLocal);
		}
		public void Refresh()
		{
			Clear();
			InitOpenFile();
		}
		public void Clear()
		{
			JsonTreeViewItem.Clear();
			json_tree_view.Items.Clear();
		}
		public void refreshJsonTree(JToken jtok_root)
		{
			// 변환
			JsonTreeViewItem root_jtree = JsonTreeViewItem.convertToTreeViewItem(jtok_root);
			if(root_jtree == null)
				return;

			// 삭제
			Clear();

			// 추가
			//TextBlock tblock = new TextBlock();
			//tblock.Text = JsonInfo.current.Filename;
			//root_jtree.Header.Children.Insert(0, tblock);
			//json_tree_view.Items.Add(root_jtree);

			// 추가
			Label label = new Label();
			label.VerticalAlignment = VerticalAlignment.Center;
			label.Content = JsonTreeViewItem.Filename;
			root_jtree.Header.Children.Insert(0, label);
			int MAX_WIDTH_TREE = JsonTreeViewItemSize.WIDTH_TEXTBOX + JsonTreeViewItemSize.MARGIN_TEXTBOX + JsonTreeViewItemSize.WIDTH_VALUEPANEL + 50;
			root_jtree.Header.Width = MAX_WIDTH_TREE;
			json_tree_view.Items.Add(root_jtree);

			JsonTreeViewItem.Root = root_jtree;
		}

		private void OnClickButtonNewJsonFile(object sender, RoutedEventArgs e)
		{
			if(JsonTreeViewItem.Path != null)
			{
				;
			}
			MenuItem mi = sender as MenuItem;
			if(mi.Header as string == "_File")
				WindowMain.current.ShowMessageDialog("New File Config", "새로만드시겠습니까?", MessageDialogStyle.AffirmativeAndNegative, NewJsonFile_File);
			if(mi.Header as string == "_Sam")
				WindowMain.current.ShowMessageDialog("New Sam Config", "새로만드시겠습니까?", MessageDialogStyle.AffirmativeAndNegative, NewJsonFile_Sam);
			if(mi.Header as string == "_Tail")
				WindowMain.current.ShowMessageDialog("New Tail Config", "새로만드시겠습니까?", MessageDialogStyle.AffirmativeAndNegative, NewJsonFile_Tail);
		}
		private void NewJsonFile_File()
		{
			JsonTreeViewItem.Clear();
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
			JToken jtok = JsonController.ParseJson(Properties.Resources.file_config_default);
			refreshJsonTree(jtok);
		}
		private void NewJsonFile_Sam()
		{
			JsonTreeViewItem.Clear();
			JToken jtok = JsonController.ParseJson(Properties.Resources.sam_config_default);
			refreshJsonTree(jtok);
		}
		private void NewJsonFile_Tail()
		{
			JsonTreeViewItem.Clear();
			JToken jtok = JsonController.ParseJson(Properties.Resources.tail_config_default);
			refreshJsonTree(jtok);
		}
		private void OnClickButtonOpenJsonFile(object sender, RoutedEventArgs e)
		{
			//SftpFile[] files = SSHController.GetListConfigFile();
			//if(files != null)
			//{
			//	LinuxDirectoryViewer l = new LinuxDirectoryViewer(files);
			//	l.ShowDialog();
			//}
			InitOpenFile();

			OpenFileDialog ofd = new OpenFileDialog();

			ofd.InitialDirectory = CurRootPathLocal;

			if(JsonTreeViewItem.Path != null)
			{
				string dir_path = JsonTreeViewItem.Path.Substring(0, JsonTreeViewItem.Path.LastIndexOf('\\') + 1);
				DirectoryInfo d = new DirectoryInfo(dir_path);
				if(d.Exists)
					ofd.InitialDirectory = dir_path;
			}

			// 파일 열기
			ofd.Filter = "JSon Files (.json)|*.json|All Files (*.*)|*.*";
			if(ofd.ShowDialog() == true)
			{
				Log.PrintConsole(ofd.FileName, "UserControls.ConfigJsonTree.OnClickButtonOpenJsonFile");
				string json = FileContoller.Read(ofd.FileName);
				JToken jtok = JsonController.ParseJson(json);
				if(jtok != null)
				{
					refreshJsonTree(jtok);
				}
				JsonTreeViewItem.Path = ofd.FileName;
				//refreshJsonTree(JsonController.parseJson(json));
				//JsonTreeViewItem.Path = ofd.FileName;
			}
		}
		private void OnClickButtonSaveJsonFile(object sender, RoutedEventArgs e)
		{
			if(!CheckJson())
				return;

			if(JsonTreeViewItem.Path == null)
			{
				OtherSaveJsonFile();
				return;
			}

			FileInfo f = new FileInfo(JsonTreeViewItem.Path);
			if(!f.Exists)
			{
				OtherSaveJsonFile();
				return;
			}

			WindowMain.current.ShowMessageDialog("Save", "저장하시겠습니까?", MessageDialogStyle.AffirmativeAndNegative, SaveJsonFile);
		}
		private void SaveJsonFile()
		{
			SaveFile(JsonTreeViewItem.Path);
		}
		private void OnClickButtonOtherSaveJsonFile(object sender, RoutedEventArgs e)
		{
			if(!CheckJson())
				return;

			OtherSaveJsonFile();
		}
		private void OtherSaveJsonFile()
		{
			SaveFileDialog sfd = new SaveFileDialog();

			sfd.InitialDirectory = CurRootPathLocal;

			if(JsonTreeViewItem.Path != null)
			{
				string dir_path = JsonTreeViewItem.Path.Substring(0, JsonTreeViewItem.Path.LastIndexOf('\\') + 1);
				DirectoryInfo d = new DirectoryInfo(dir_path);
				if(d.Exists)
					sfd.InitialDirectory = dir_path;
			}

			sfd.Filter = "JSon Files (.json)|*.json";
			if(sfd.ShowDialog() == true)
			{
				SaveFile(sfd.FileName);
				JsonTreeViewItem.Path = sfd.FileName;
			}
		}
		private bool CheckJson()
		{
			// 로드된 오브젝트가 없으면 실행 x
			if(json_tree_view.Items.Count < 1)
				return false;
			JsonTreeViewItem root = json_tree_view.Items[0] as JsonTreeViewItem;
			if(root == null)
				return false;

			return true;
		}
		private void SaveFile(string path_local)
		{
			if(!CheckJson())
				return;

			JToken Jtok_root = JsonTreeViewItem.convertToJToken(json_tree_view.Items[0] as JsonTreeViewItem);
			if(Jtok_root != null && FileContoller.Write(path_local, Jtok_root.ToString()))
			{
				string path_remote;
				if((path_remote = SSHController.SetConfig(path_local, CurRootPathLocal)) == null)
				{
					string message = "서버 연결 에러";
					WindowMain.current.ShowMessageDialog("Save", message);
					Log.PrintError(message, "UserControls.ConfigJsonTree.SaveFile");
				}
				else
				{
					string message = path_remote + " 파일이 저장되었습니다.";
					WindowMain.current.ShowMessageDialog("Save", message);
					Log.PrintLog(message, "UserControls.ConfigJsonTree.SaveFile");
				}
			}
			else
			{
				string caption = "Save Error";
				string message = path_local + " 파일을 저장하는데 문제가 생겼습니다.";
				WindowMain.current.ShowMessageDialog(caption, message);
				Log.PrintError(message, "UserControls.ConfigJsonTree.SaveFile");
			}
		}
		private void OnClickButtonViewJsonFile(object sender, RoutedEventArgs e)
		{
			if(JsonTreeViewItem.Path == null)
				return;

			//if(json_tree_view.Items.Count <= 0)
			//	return;
			//JsonTreeViewItem root = json_tree_view.Items[0] as JsonTreeViewItem;
			//if(root == null)
			//	return;

			//Window_EditFile w = new Window_EditFile(JsonTreeViewItem.convertToJToken(root).ToString(), JsonTreeViewItem.Path);
			Window_EditFile w = new Window_EditFile(FileContoller.Read(JsonTreeViewItem.Path), JsonTreeViewItem.Path);

			if(w.ShowDialog() == true)
			{
				refreshJsonTree(JsonController.ParseJson(w.tb_file.Text));
			}
		}
		private void OnClickButtonCancelJsonFile(object sender, RoutedEventArgs e)
		{
			if(JsonTreeViewItem.Path == null)
				return;

			string dir_path = JsonTreeViewItem.Path.Substring(0, JsonTreeViewItem.Path.LastIndexOf('\\') + 1);
			DirectoryInfo d = new DirectoryInfo(dir_path);
			if(!d.Exists)
				return;

			WindowMain.current.ShowMessageDialog("Cancel", "변경사항을 되돌리시겠습니까?", MessageDialogStyle.AffirmativeAndNegative, CalcelJsonFile);
		}
		private void CalcelJsonFile()
		{
			string json = FileContoller.Read(JsonTreeViewItem.Path);
			refreshJsonTree(JsonController.ParseJson(json));
			WindowMain.current.ShowMessageDialog("Cancel", "변경사항을 되돌렸습니다.", MessageDialogStyle.Affirmative);
		}
		#endregion
	}
}
