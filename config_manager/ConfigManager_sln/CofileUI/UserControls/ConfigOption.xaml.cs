using CofileUI.Classes;
using CofileUI.UserControls.ConfigOptions;
using CofileUI.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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
	/// EditConfig.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ConfigOption : UserControl
	{
		public static ConfigOption current;
		public ConfigOption()
		{
			current = this;
			InitializeComponent();
			InitJsonFileView();

		}
		#region
		
		private void Refresh(JToken root)
		{
			if(root == null)
				return;
			grid.Children.Clear();

			//UserControl retval = null;
			//if(root["type"].ToString() == "file")
			//	retval = new ConfigOptions.File.FileOptions() { DataContext = root };
			//else if(root["type"].ToString() == "sam")
			//	retval = new ConfigOptions.Sam.SamOptions() { DataContext = root };
			//else if(root["type"].ToString() == "tail")
			//	retval = new ConfigOptions.Tail.TailOptions() { DataContext = root };
			//grid.Children.Add(retval);

			UserControl ui = ConfigOptionManager.CreateOption(root);
			if(ui != null)
				grid.Children.Add(ui);
		}
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
		public static string CurRootPathLocal
		{
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
			grid.Children.Clear();
			ConfigOptions.ConfigOptionManager.Clear();
		}

		private void OnClickButtonNewJsonFile(object sender, RoutedEventArgs e)
		{
			if(ConfigOptionManager.Path != null)
			{
				;
			}
			WindowMain.CallBack afterSave_callback = null;
			MenuItem mi = sender as MenuItem;
			if(mi.Header as string == "_File")
				afterSave_callback = delegate { WindowMain.current.ShowMessageDialog("New File Config", "새로만드시겠습니까?", MessageDialogStyle.AffirmativeAndNegative, NewJsonFile_File); };
			if(mi.Header as string == "_Sam")
				afterSave_callback = delegate { WindowMain.current.ShowMessageDialog("New File Config", "새로만드시겠습니까?", MessageDialogStyle.AffirmativeAndNegative, NewJsonFile_Sam); };
			if(mi.Header as string == "_Tail")
				afterSave_callback = delegate { WindowMain.current.ShowMessageDialog("New File Config", "새로만드시겠습니까?", MessageDialogStyle.AffirmativeAndNegative, NewJsonFile_Tail); };

			ConfirmSave(afterSave_callback);
		}
		private void NewJsonFile_File()
		{
			ConfigOptionManager.Clear();

			JToken jtok = JsonController.ParseJson(Properties.Resources.file_config_default);
			Refresh(jtok);

			UserControls.ConfigOptions.ConfigOptionManager.bChanged = true;
		}
		private void NewJsonFile_Sam()
		{
			ConfigOptionManager.Clear();
		
			JToken jtok = JsonController.ParseJson(Properties.Resources.sam_config_default);
			Refresh(jtok);

			UserControls.ConfigOptions.ConfigOptionManager.bChanged = true;
		}
		private void NewJsonFile_Tail()
		{
			ConfigOptionManager.Clear();
			
			JToken jtok = JsonController.ParseJson(Properties.Resources.tail_config_default);
			Refresh(jtok);

			UserControls.ConfigOptions.ConfigOptionManager.bChanged = true;
		}
		private void OnClickButtonOpenJsonFile(object sender, RoutedEventArgs e)
		{
			ConfirmSave(OpenJsonFile);
		}
		private void OpenJsonFile()
		{
			if(InitOpenFile() != 0)
				return;

			OpenFileDialog ofd = new OpenFileDialog();

			ofd.InitialDirectory = CurRootPathLocal;

			if(ConfigOptionManager.Path != null)
			{
				string dir_path = ConfigOptionManager.Path.Substring(0, ConfigOptionManager.Path.LastIndexOf('\\') + 1);
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
				ConfigOptionManager.Path = ofd.FileName;
				//refreshJsonTree(JsonController.parseJson(json));
				//ConfigOptionManager.Path = ofd.FileName;
			}
		}
		private void OnClickButtonSaveJsonFile(object sender, RoutedEventArgs e)
		{
			ConfirmSave();
		}
		public void ConfirmSave(WindowMain.CallBack afterSave_callback = null)
		{
			if(!CheckJson())
			{
				afterSave_callback?.Invoke();
				return;
			}

			if(!UserControls.ConfigOptions.ConfigOptionManager.bChanged)
			{
				afterSave_callback?.Invoke();
				return;
			}

			MahApps.Metro.Controls.Dialogs.MetroDialogSettings settings = new MahApps.Metro.Controls.Dialogs.MetroDialogSettings()
			{
				AffirmativeButtonText = "Yes",
				NegativeButtonText = "No",
				FirstAuxiliaryButtonText = "Cancel"
			};
			WindowMain.current.ShowMessageDialog("Save", "변경된 Config File 을 저장하시겠습니까?", 
				MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, 
				affirmative_callback: delegate { SaveJsonFile(); afterSave_callback?.Invoke(); }, 
				negative_callback: delegate { afterSave_callback?.Invoke(); }, 
				settings: settings);
		}
		private void SaveJsonFile()
		{
			if(ConfigOptionManager.Path == null)
			{
				OtherSaveJsonFile();
				return;
			}

			FileInfo f = new FileInfo(ConfigOptionManager.Path);
			if(!f.Exists)
			{
				OtherSaveJsonFile();
				return;
			}

			if(SaveFile(ConfigOptionManager.Path) == 0)
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

			if(ConfigOptionManager.Path != null)
			{
				string dir_path = ConfigOptionManager.Path.Substring(0, ConfigOptionManager.Path.LastIndexOf('\\') + 1);
				DirectoryInfo d = new DirectoryInfo(dir_path);
				if(d.Exists)
					sfd.InitialDirectory = dir_path;
			}

			sfd.Filter = "JSon Files (.json)|*.json";
			if(sfd.ShowDialog() == true)
			{
				if(SaveFile(sfd.FileName) == 0)
					ConfigOptionManager.Path = sfd.FileName;
			}
		}
		private bool CheckJson()
		{
			// 로드된 오브젝트가 없으면 실행 x
			if(grid.Children.Count < 1)
				return false;

			return true;
		}
		private int SaveFile(string path_local)
		{
			if(!CheckJson())
				return -1;
			
			JToken Jtok_root = ConfigOptionManager.Root;
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
			if(ConfigOptionManager.Path == null)
				return;
			
			Window_EditFile w = new Window_EditFile(FileContoller.Read(ConfigOptionManager.Path), ConfigOptionManager.Path);

			if(w.ShowDialog() == true)
			{
				Refresh(JsonController.ParseJson(w.tb_file.Text));
				ConfigOptionManager.bChanged = true;
			}
		}
		private void OnClickButtonCancelJsonFile(object sender, RoutedEventArgs e)
		{
			if(ConfigOptionManager.Path == null)
				return;

			string dir_path = ConfigOptionManager.Path.Substring(0, ConfigOptionManager.Path.LastIndexOf('\\') + 1);
			DirectoryInfo d = new DirectoryInfo(dir_path);
			if(!d.Exists)
				return;

			WindowMain.current.ShowMessageDialog("Cancel", "변경사항을 되돌리시겠습니까?", MessageDialogStyle.AffirmativeAndNegative, CalcelJsonFile);
		}
		private void CalcelJsonFile()
		{
			string json = FileContoller.Read(ConfigOptionManager.Path);
			Refresh(JsonController.ParseJson(json));
			WindowMain.current.ShowMessageDialog("Cancel", "변경사항을 되돌렸습니다.", MessageDialogStyle.Affirmative);
		}
		#endregion
	}

}
