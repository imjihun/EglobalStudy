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
using CofileUI.UserControls.ConfigOptions;

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
		#region

		JToken Root { get; set; }
		private void Refresh(JToken root)
		{
			if(root == null)
				return;
			Clear();

			Root = root;
			if(root["type"].ToString() == "file")
				grid.Children.Add(new ConfigOptions.File.FileOptions() { DataContext = Root });
			else if(root["type"].ToString() == "sam")
				grid.Children.Add(new ConfigOptions.Sam.SamOptions() { DataContext = Root });
			else if(root["type"].ToString() == "tail")
				grid.Children.Add(new ConfigOptions.Tail.TailOptions() { DataContext = Root });

			//if(root as JObject != null)
			//	treeView.ItemsSource = root.Children();
		}
		////private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		////{
		////	TreeView tv = sender as TreeView;
		////	if(tv == null)
		////		return;

		////	JProperty jprop_optionMenu = tv.SelectedItem as JProperty;
		////	if(jprop_optionMenu == null)
		////		return;

		////	panel_DetailOption.Children.Clear();
		////	panel_DetailOption.RowDefinitions.Clear();

		////	AddItem(panel_DetailOption, jprop_optionMenu);
		////}
		//Options options = null;
		//public int AddItem(Grid panel, JProperty root)
		//{
		//	string type = Convert.ToString(root.Root["type"]);
		//	if(type == "file")
		//		options = new ConfigOptions.FileOptions();
		//	else if(type == "sam")
		//		options = new ConfigOptions.SamOptions();
		//	else if(type == "tail")
		//		options = new ConfigOptions.FileOptions();
		//	else return -1;

		//	if(root.Value.Type == JTokenType.Object)
		//	{
		//		panel.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
		//		panel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

		//		return AddItemValueIsObject(panel, root, root);
		//	}
		//	else if(root.Value.Type == JTokenType.Array)
		//	{
		//		string key = root.Name.TrimStart('#');
		//		UIElement ui = null;
		//		switch(key)
		//		{
		//			case "col_var":
		//				ui = new ConfigOptions.Sam.col_var() { DataContext = root };
		//				//ui = Resources["DataGridResourceSamColVar"] as DataGrid;
		//				break;
		//			case "col_fix":
		//				ui = new ConfigOptions.Sam.col_fix() { DataContext = root };
		//				//ui = Resources["DataGridResourceSamColFix"] as DataGrid;
		//				break;
		//			case "enc_inform":
		//				ui = new ConfigOptions.Tail.enc_inform() { DataContext = root };
		//				//ui = Resources["DataGridResourceTailEncInform"] as DataGrid;
		//				break;
		//			default:
		//				break;
		//		}
		//		if(ui != null)
		//		{
		//			//ui.ItemsSource = root.Value;
		//			panel.Children.Add(ui);
		//		}

		//		return 0;
		//	}
		//	else
		//		return -1;
		//}
		//private int AddItemValueIsObject(Panel cur_panel_DetailOption, JProperty cur_jprop_optionMenu, JToken cur_jtok)
		//{
		//	foreach(var v in cur_jtok.Children())
		//	{
		//		JProperty jprop = cur_jprop_optionMenu;
		//		Panel pan = cur_panel_DetailOption;
		//		switch(v.Type)
		//		{
		//			case JTokenType.Boolean:
		//			case JTokenType.Integer:
		//			case JTokenType.String:
		//				{
		//					FrameworkElement ui = options.GetUIOptionValue(jprop, v);
		//					if(ui == null)
		//						break;

		//					pan.Children.Add(ui);
		//				}
		//				break;

		//			case JTokenType.Property:
		//				{
		//					Grid grid_key = new Grid();
		//					Grid grid_value = new Grid();
		//					FrameworkElement ui = options.GetUIOptionKey((JProperty)v, grid_value);
		//					if(ui == null)
		//						break;

		//					((Grid)pan).RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
		//					int idxRow = ((Grid)pan).RowDefinitions.Count - 1;

		//					Grid.SetRow(grid_key, idxRow);
		//					Grid.SetColumn(grid_key, 0);
		//					pan.Children.Add(grid_key);

		//					Grid.SetRow(grid_value, idxRow);
		//					Grid.SetColumn(grid_value, 1);
		//					pan.Children.Add(grid_value);

		//					grid_key.Children.Add(ui);

		//					jprop = (JProperty)v;
		//					pan = grid_value;
		//				}
		//				break;
		//			case JTokenType.Array:
		//			case JTokenType.Object:
		//			case JTokenType.Raw:
		//			default:
		//				break;
		//		}

		//		AddItemValueIsObject(pan, jprop, v);
		//	}
		//	return 0;
		//}

		#endregion
		#region Json Tree Area
		static string DIR = @"config\";
		private static string root_path_local = AppDomain.CurrentDomain.BaseDirectory + DIR;
		public static string curRootPathLocal = root_path_local;
		public static string CurRootPathLocal {
			get
			{
				curRootPathLocal = root_path_local;
				if(ServerList.selected_serverinfo_panel == null
					|| ServerList.selected_serverinfo_panel.Serverinfo == null)
				{ }
				else
				{
					if(ServerList.selected_serverinfo_panel.Serverinfo.name != null && ServerList.selected_serverinfo_panel.Serverinfo.name != "")
						curRootPathLocal += ServerList.selected_serverinfo_panel.Serverinfo.name + @"\";
					if(ServerList.selected_serverinfo_panel.Serverinfo.id != null && ServerList.selected_serverinfo_panel.Serverinfo.id != "")
						curRootPathLocal += ServerList.selected_serverinfo_panel.Serverinfo.id + @"\";
				}

				Log.PrintError("curRootPathLocal = " + curRootPathLocal, "Debug");
				return curRootPathLocal;
			}
		}

		bool InitJsonFileView()
		{
			return FileContoller.CreateDirectory(CurRootPathLocal);
		}
		int RemoveConfigFile(string path)
		{
			return FileContoller.DeleteDirectory(path);
		}
		public int InitOpenFile()
		{
			if(SSHController.ReConnect())
			{
				//if(RemoveConfigFile(CurRootPathLocal) != 0)
				//	return -2;

				if(SSHController.GetConfig(CurRootPathLocal) == null)
					return -1;

				return 0;
			}
			return -3;
		}
		//public void Refresh()
		//{
		//	Clear();
		//	//InitOpenFile();
		//}
		public void Clear()
		{
			//if(JsonTreeViewItem.Path != null)
			//{
			//	JsonTreeViewItem.Clear();
			//	json_tree_view.Items.Clear();
			//}
			//JsonTreeViewItem.Clear();
			//json_tree_view.Items.Clear();
			grid.Children.Clear();
			//treeView.ItemsSource = null;
			//treeView.Items.Clear();
		}
		//public void refreshJsonTree(JToken jtok_root)
		//{
		//	// 변환
		//	JsonTreeViewItem root_jtree = JsonTreeViewItem.convertToTreeViewItem(jtok_root);
		//	if(root_jtree == null)
		//		return;

		//	// 삭제
		//	string path = JsonTreeViewItem.Path;
		//	Clear();
		//	JsonTreeViewItem.Path = path;

		//	// 추가
		//	//TextBlock tblock = new TextBlock();
		//	//tblock.Text = JsonInfo.current.Filename;
		//	//root_jtree.Header.Children.Insert(0, tblock);
		//	//json_tree_view.Items.Add(root_jtree);

		//	// 추가
		//	Label label = new Label();
		//	label.VerticalAlignment = VerticalAlignment.Center;
		//	label.Content = JsonTreeViewItem.Filename;
		//	root_jtree.Header.Children.Insert(0, label);
		//	int MAX_WIDTH_TREE = JsonTreeViewItemSize.WIDTH_TEXTBOX + JsonTreeViewItemSize.MARGIN_TEXTBOX + JsonTreeViewItemSize.WIDTH_VALUEPANEL + 50;
		//	root_jtree.Header.Width = MAX_WIDTH_TREE;
		//	json_tree_view.Items.Add(root_jtree);

		//	JsonTreeViewItem.Root = root_jtree;
		//}

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
			Refresh(jtok);
		}
		private void NewJsonFile_Sam()
		{
			JsonTreeViewItem.Clear();
			JToken jtok = JsonController.ParseJson(Properties.Resources.sam_config_default);
			Refresh(jtok);
		}
		private void NewJsonFile_Tail()
		{
			JsonTreeViewItem.Clear();
			JToken jtok = JsonController.ParseJson(Properties.Resources.tail_config_default);
			Refresh(jtok);
		}
		private void OnClickButtonOpenJsonFile(object sender, RoutedEventArgs e)
		{
			//SftpFile[] files = SSHController.GetListConfigFile();
			//if(files != null)
			//{
			//	LinuxDirectoryViewer l = new LinuxDirectoryViewer(files);
			//	l.ShowDialog();
			//}
			if(InitOpenFile() != 0)
				return;

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
					Refresh(jtok);
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
			if(SaveFile(JsonTreeViewItem.Path) == 0)
			{ }
		}
		private void OnClickButtonOtherSaveJsonFile(object sender, RoutedEventArgs e)
		{
			if(!CheckJson())
				return;

			OtherSaveJsonFile();
		}
		private void OtherSaveJsonFile()
		{
			if(InitOpenFile() != 0)
				return;

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
				if(SaveFile(sfd.FileName) == 0)
					JsonTreeViewItem.Path = sfd.FileName;
			}
		}
		private bool CheckJson()
		{
			//// 로드된 오브젝트가 없으면 실행 x
			//if(json_tree_view.Items.Count < 1)
			//	return false;
			//JsonTreeViewItem root = json_tree_view.Items[0] as JsonTreeViewItem;
			//if(root == null)
			//	return false;

			// 로드된 오브젝트가 없으면 실행 x
			if(grid.Children.Count < 1)
				return false;

			return true;
		}
		private int SaveFile(string path_local)
		{
			if(!CheckJson())
				return -1;

			//JToken Jtok_root = JsonTreeViewItem.convertToJToken(json_tree_view.Items[0] as JsonTreeViewItem);
			JToken Jtok_root = Root;
			if(Jtok_root != null && FileContoller.Write(path_local, Jtok_root.ToString()))
			{
				string path_remote;
				if((path_remote = SSHController.SetConfig(path_local, CurRootPathLocal)) == null)
				{
					//FileContoller.FileDelete(path_local);

					string caption = "Save Error";
					string message = "서버 연결 에러";
					WindowMain.current.ShowMessageDialog(caption, message);
					Log.PrintError(message, "UserControls.ConfigJsonTree.SaveFile");
					return -3;
				}
				else
				{
					string message = path_remote + " 파일이 저장되었습니다.";
					WindowMain.current.ShowMessageDialog("Save", message);
					Log.PrintLog(message, "UserControls.ConfigJsonTree.SaveFile");

					ConfigOptions.ConfigOptionManager.bChanged = false;
					return 0;
				}
			}
			else
			{
				string caption = "Save Error";
				string message = path_local + " 파일을 저장하는데 문제가 생겼습니다.";
				WindowMain.current.ShowMessageDialog(caption, message);
				Log.PrintError(message, "UserControls.ConfigJsonTree.SaveFile");
				return -2;
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
				Refresh(JsonController.ParseJson(w.tb_file.Text));
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
			Refresh(JsonController.ParseJson(json));
			WindowMain.current.ShowMessageDialog("Cancel", "변경사항을 되돌렸습니다.", MessageDialogStyle.Affirmative);
		}
		#endregion
	}
	//public sealed class Int64ToStringConverter : IValueConverter
	//{
	//	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	//	{
	//		return System.Convert.ToString(value);
	//	}

	//	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	//	{
	//		return System.Convert.ToInt64(value);
	//	}
	//}
}
