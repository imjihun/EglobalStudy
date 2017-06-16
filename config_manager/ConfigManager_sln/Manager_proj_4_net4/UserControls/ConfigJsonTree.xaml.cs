using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using Manager_proj_4_net4.Classes;
using Manager_proj_4_net4.Windows;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
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

namespace Manager_proj_4_net4.UserControls
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
		public static string root_path = AppDomain.CurrentDomain.BaseDirectory + DIR;

		void InitJsonFileView()
		{
			try
			{
				FileContoller.CreateDirectory(root_path);
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "CreateDirectory", Status.current.richTextBox_status);
			}
		}
		public void Refresh()
		{
			string path = root_path;
			path += ServerList.selected_serverinfo_textblock.serverinfo.name + @"\";
			SSHController.GetConfig(path);
		}
		public void refreshJsonTree(JToken jtok_root)
		{
			// 변환
			JsonTreeViewItem root_jtree = JsonTreeViewItem.convertToTreeViewItem(jtok_root);
			if(root_jtree == null)
				return;

			// 삭제
			json_tree_view.Items.Clear();

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

			WindowMain.current.ShowMessageDialog("New", "새로만드시겠습니까?", MessageDialogStyle.AffirmativeAndNegative, NewJsonFile);
		}
		private void NewJsonFile()
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
			JToken jtok = JsonController.parseJson(Properties.Resources.file_config_default);
			refreshJsonTree(jtok);
		}
		private void OnClickButtonOpenJsonFile(object sender, RoutedEventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();

			ofd.InitialDirectory = root_path;

			if(JsonTreeViewItem.Path != null)
			{
				string dir_path = JsonTreeViewItem.Path.Substring(0, JsonTreeViewItem.Path.LastIndexOf('\\') + 1);
				DirectoryInfo d = new DirectoryInfo(dir_path);
				if(d.Exists)
					ofd.InitialDirectory = dir_path;
			}

			// 파일 열기
			ofd.Filter = "JSon Files (.json)|*.json";
			if(ofd.ShowDialog() == true)
			{
				Console.WriteLine(ofd.FileName);

				string json = FileContoller.Read(ofd.FileName);
				refreshJsonTree(JsonController.parseJson(json));

				JsonTreeViewItem.Path = ofd.FileName;
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

			sfd.InitialDirectory = root_path;

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
		private void SaveFile(string path)
		{
			if(!CheckJson())
				return;

			JToken Jtok_root = JsonTreeViewItem.convertToJToken(json_tree_view.Items[0] as JsonTreeViewItem);
			if(Jtok_root != null && FileContoller.Write(path, Jtok_root.ToString()))
				WindowMain.current.ShowMessageDialog("Save", path + " 파일이 저장되었습니다.");
			else
			{
				string caption = "Save Error";
				string message = path + " 파일을 저장하는데 문제가 생겼습니다.";
				WindowMain.current.ShowMessageDialog(caption, message);
				Console.WriteLine("[" + caption + "] " + message);
			}
		}
		private void OnClickButtonViewJsonFile(object sender, RoutedEventArgs e)
		{
			if(JsonTreeViewItem.Path == null)
				return;

			JsonTreeViewItem root = json_tree_view.Items[0] as JsonTreeViewItem;
			if(root == null)
				return;

			Window_EditFile w = new Window_EditFile(JsonTreeViewItem.convertToJToken(root).ToString(), JsonTreeViewItem.Path);
			//Window_ViewFile w = new Window_ViewFile(FileContoller.read(JsonInfo.current.Path), JsonInfo.current.Path);

			if(w.ShowDialog() == true)
			{
				refreshJsonTree(JsonController.parseJson(w.tb_file.Text));
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
			string json = FileContoller.Read(JsonTreeViewItem.path);
			refreshJsonTree(JsonController.parseJson(json));
			WindowMain.current.ShowMessageDialog("Cancel", "변경사항을 되돌렸습니다.", MessageDialogStyle.Affirmative);
		}
		#endregion

	}

	
}
