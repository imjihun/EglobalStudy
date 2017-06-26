using CofileUI.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MS.Internal.AppModel;
using MS.Win32;
using MahApps.Metro.IconPacks;
using Renci.SshNet.Sftp;
using System.Text.RegularExpressions;
using CofileUI.Windows;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Globalization;
using System.Windows.Controls.Primitives;
using Newtonsoft.Json.Linq;
using MahApps.Metro.Controls.Dialogs;

namespace CofileUI.UserControls
{
	/// <summary>
	/// Cofile.xaml에 대한 상호 작용 논리
	/// </summary>
	public enum CofileOption
	{
		file = 0,
		sam,
		tail
	}
	public partial class Cofile : UserControl
	{
		public static Cofile current;
		public Cofile()
		{
			current = this;
			InitializeComponent();
			InitLinuxDirectory();
		}

		#region Linux Directory

		static bool bool_show_hidden = false;
		public static bool Bool_show_hidden
		{
			get { return bool_show_hidden; }
			set
			{
				bool_show_hidden = value;
				LinuxTreeViewItem.Filter(LinuxTreeViewItem.root, Filter_string, bool_show_hidden);
			}
		}

		static string filter_string = "";
		public static string Filter_string
		{
			get { return filter_string; }
			set
			{
				filter_string = value;

				LinuxTreeViewItem.Filter(LinuxTreeViewItem.root, filter_string, Bool_show_hidden);
			}
		}
		void InitLinuxDirectory()
		{
			textBox_linux_directory_filter.TextChanged += delegate { Filter_string = textBox_linux_directory_filter.Text; RefreshListView(cur_LinuxTreeViewItem); };
			checkBox_hidden.Checked += delegate { Bool_show_hidden = true; RefreshListView(cur_LinuxTreeViewItem); };
			checkBox_hidden.Unchecked += delegate { Bool_show_hidden = false; RefreshListView(cur_LinuxTreeViewItem); };
			checkBox_hidden.IsChecked = false;

			checkBox_linux_detail.IsChecked = true;
			checkBox_work_detail.IsChecked = true;
		}

		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			if(sender == checkBox_linux_detail)
			{
				listView_linux_files.ItemsPanel = Resources["ItemsPanelTemplate_ListView_Detail"] as ItemsPanelTemplate;
				listView_linux_files.View = Resources["GridView_ListViewLinux_Detail"] as GridView;
			}
			else if(sender == checkBox_work_detail)
			{
				listView_work_files.ItemsPanel = Resources["ItemsPanelTemplate_ListView_Detail"] as ItemsPanelTemplate;
				listView_work_files.View = Resources["GridView_ListViewWork_Detail"] as GridView;
			}
		}
		private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			if(sender == checkBox_linux_detail)
			{
				listView_linux_files.View = null;
				listView_linux_files.ItemsPanel = Resources["ItemsPanelTemplate_ListView_Icon"] as ItemsPanelTemplate;
			}
			else if(sender == checkBox_work_detail)
			{
				listView_work_files.View = null;
				listView_work_files.ItemsPanel = Resources["ItemsPanelTemplate_ListView_Icon"] as ItemsPanelTemplate;
			}
		}
		private void OnMouseMoveLinuxFile(object sender, MouseEventArgs e)
		{
			if(e.LeftButton == MouseButtonState.Pressed
				&& listView_linux_files.SelectedItems.Count > 0)
			{
				DataObject data = new DataObject();
				data.SetData("Object", listView_linux_files.SelectedItems);
				DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
			}
		}

		static string DIR = @"tmp\";
		string root_path = AppDomain.CurrentDomain.BaseDirectory + DIR;
		private void OnMouseDoubleClickLinuxFile(object sender, MouseButtonEventArgs e)
		{
			if(e.LeftButton == MouseButtonState.Pressed)
			{
				ListView lv = sender as ListView;
				if(lv == null)
					return;

				LinuxListViewItem selected = lv.SelectedItem as LinuxListViewItem;
				if(selected == null)
					return;

				if(selected.IsDirectory)
				{
					OpenDirectory(selected);
				}
				else
				{
					OpenEncryptFile();
				}
			}
		}
		private int OpenDirectory(LinuxListViewItem llvi)
		{
			if(llvi.IsDirectory)
			{
				if(llvi.LinuxTVI == null)
					return -1;

				llvi.LinuxTVI.RefreshChild();
				//RefreshListView(selected.LinuxTVI);

				//llvi.LinuxTVI.IsExpanded = false;
				//llvi.LinuxTVI.IsExpanded = true;
				llvi.LinuxTVI.Focus();
			}
			return 0;
		}

		public static string PreviewExtension = "preview_extension_ll";
		static ulong Idx_Encrypt_File_Open = 0;
		string FindDecryptFile(string remote_path_enc_file)
		{
			JToken jtok = JsonController.ParseJson(FileContoller.Read(current.Selected_config_file_path));
			if(jtok == null)
			{
				Log.ErrorIntoUI("Check Config File", "Decrypt", Status.current.richTextBox_status);
				return null;
			}

			if(jtok["dec_option"] == null || jtok["dec_option"]["input_extension"] == null)
			{
				Log.ErrorIntoUI("Check the dec_option.input_extension in Cofile Config file", "Decrypt Failed", Status.current.richTextBox_status);
				return null;
			}

			JValue jval_input_extansion = jtok["dec_option"]["input_extension"] as JValue;
			string output_extension = PreviewExtension;

			string remote_path = remote_path_enc_file;
			if(remote_path.Length > jval_input_extansion.Value.ToString().Length + 1)
				//&& remote_path.Substring(remote_path.Length - jval_input_extansion.Value.ToString().Length) == jval_input_extansion.Value.ToString())
				remote_path = remote_path.Substring(0, remote_path.Length - jval_input_extansion.Value.ToString().Length - 1);

			string remote_path_dec_file = remote_path + "." + output_extension;

			return remote_path_dec_file;
		}
		void OpenEncryptFile()
		{
			if(listView_linux_files.SelectedItems.Count < 1)
				return;

			LinuxListViewItem llvi = listView_linux_files.SelectedItems[0] as LinuxListViewItem;
			if(llvi == null)
				return;

			string remote_path = llvi.LinuxTVI.Path;

			string local_filename = "temp" + Idx_Encrypt_File_Open++;
			string remote_path_dec_file  = FindDecryptFile(remote_path);
			if(remote_path_dec_file == null)
				return;

			if(SSHController.SendNRecvCofileCommandPreview(listView_linux_files.SelectedItems.Cast<Object>(), false))
			{
				if(SSHController.MoveFileToLocal(root_path, remote_path_dec_file, local_filename))
				{
					llvi.LinuxTVI.RefreshChildFromParent();
					Cofile.current.RefreshListView(Cofile.cur_LinuxTreeViewItem);

					string url_localfile = root_path + local_filename;
					string[] split = remote_path.Split('.');
					string expansion = split[split.Length - 2];
					if(expansion == "png"
						|| expansion == "dib"
						|| expansion == "bmp"
						|| expansion == "jpg"
						|| expansion == "jpeg"
						|| expansion == "jpe"
						|| expansion == "jfif"
						|| expansion == "gif"
						|| expansion == "tif"
						|| expansion == "tiff"
						)
					{
						Window_ViewImage wvi = new Window_ViewImage(LoadImage(url_localfile), llvi.LinuxTVI.FileInfo.Name);
						FileContoller.DeleteFile(url_localfile);

						wvi.ShowDialog();
					}
					else
					{

						string str = FileContoller.Read(url_localfile);
						FileContoller.DeleteFile(url_localfile);

						Window_ViewFile wvf = new Window_ViewFile(str, llvi.LinuxTVI.FileInfo.Name);
						wvf.ShowDialog();
					}
				}
				else
				{
					Log.ErrorIntoUI("Check the Cofile Config File or Check File To Decrypt", "Decrypt Failed", Status.current.richTextBox_status);
					Log.PrintError("Cant Download Decrypt File", "UserControls.Cofile.OpenEncryptFile");
				}
			}
		}
		private BitmapImage LoadImage(string uri_ImageFile)
		{
			BitmapImage myRetVal = null;
			if(uri_ImageFile != null)
			{
				BitmapImage image = new BitmapImage();
				using(FileStream stream = File.OpenRead(uri_ImageFile))
				{
					image.BeginInit();
					image.CacheOption = BitmapCacheOption.OnLoad;
					image.StreamSource = stream;
					image.EndInit();
				}
				myRetVal = image;
			}
			return myRetVal;
		}
		private void OnDropWorkFile(object sender, DragEventArgs e)
		{
			// If the DataObject contains string data, extract it.
			if(e.Data.GetDataPresent("Object"))
			{
				Object data_obj = (Object)e.Data.GetData("Object");

				ObservableCollection<object> list_llvi = data_obj as ObservableCollection<object>;
				List<LinuxTreeViewItem> list_ltvi = data_obj as List<LinuxTreeViewItem>;

				if(list_ltvi != null)
				{
					for(int i = 0; i < list_ltvi.Count; i++)
					{
						// 중복 체크
						int idx_dup;
						for(idx_dup = 0; idx_dup < listView_work_files.Items.Count; idx_dup++)
						{
							LinuxListViewItem llvi = listView_work_files.Items[idx_dup] as LinuxListViewItem;
							if(llvi.LinuxTVI.Path == list_ltvi[i].Path)
								break;
						}
						if(listView_work_files.Items.Count > 0 && idx_dup != listView_work_files.Items.Count)
							continue;

						listView_work_files.Items.Add(new LinuxListViewItem() { BindingName = list_ltvi[i].Header.Text, IsDirectory = list_ltvi[i].IsDirectory, LinuxTVI = list_ltvi[i] });
					}
				}
				else if(list_llvi != null)
				{
					for(int i = 0; i < list_llvi.Count; i++)
					{
						LinuxListViewItem llvi = list_llvi[i] as LinuxListViewItem;

						// 중복 체크
						int idx_dup;
						for(idx_dup = 0; idx_dup < listView_work_files.Items.Count; idx_dup++)
						{
							LinuxListViewItem llvi_for_dup_check = listView_work_files.Items[idx_dup] as LinuxListViewItem;
							if(llvi_for_dup_check.LinuxTVI.Path == llvi.LinuxTVI.Path)
								break;
						}
						if(listView_work_files.Items.Count > 0 && idx_dup != listView_work_files.Items.Count)
							continue;

						// 상위폴더 체크
						if(llvi.BindingName == "..")
						{
							llvi = new LinuxListViewItem() { BindingName = llvi.LinuxTVI.Header.Text, IsDirectory = llvi.IsDirectory, LinuxTVI = llvi.LinuxTVI };
						}

						listView_work_files.Items.Add(llvi);
					}
				}
			}
		}

		public int Refresh()
		{
			if(WindowMain.current == null)
				return -1;

			//if(ssh != null && ssh.IsConnected)
			//	ssh.Disconnect();
			//if(sftp != null && sftp.IsConnected)
			//	sftp.Disconnect();

			// 삭제
			//treeView_linux_directory.Items.Clear();
			//listView_linux_files.Items.Clear();
			Clear();

			// 추가
			//string home_dir = sftp.WorkingDirectory;
			// root 의 path 는 null 로 초기화
			string working_dir = SSHController.WorkingDirectory;
			if(working_dir == null)
				return -1;

			LinuxTreeViewItem.root = new LinuxTreeViewItem("/", null, "/", true, null);
			treeView_linux_directory.Items.Add(LinuxTreeViewItem.root);
			LinuxTreeViewItem.root.RefreshChild(working_dir, false);
			Cofile.current.RefreshListView(LinuxTreeViewItem.Last_Refresh);
			Log.PrintLog("[refresh]", "UserControls.Cofile.Refresh");

			return 0;
			//LinuxTreeViewItem.ReconnectServer();
		}
		private string selected_config_file_path = null;
		public string Selected_config_file_path
		{
			get { return selected_config_file_path; }
			set
			{
				selected_config_file_path = value;
				string[] splited = selected_config_file_path.Split('\\');
				textBlock_selected_config_file_name.Text = splited[splited.Length - 1];
			}
		}
		private void OnClickButtonSelectConfigFile(object sender, EventArgs e)
		{
			//SftpFileTree root = SSHController.GetListConfigFile();
			//if(root == null)
			//	return;

			//Window_SelectJsonFile ws = new Window_SelectJsonFile(root);
			//if(ws.ShowDialog() == true)
			//	Selected_config_file_path = ws.FilePathRemote;

			if(ConfigJsonTree.current.InitOpenFile() != 0)
				return;

			OpenFileDialog ofd = new OpenFileDialog();
			// 초기경로 지정
			ofd.InitialDirectory = ConfigJsonTree.CurRootPathLocal;

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
				Selected_config_file_path = ofd.FileName;
		}
		private void OnChangeComboBoxCofileType(object sender, SelectionChangedEventArgs e)
		{
			SSHController.selected_type = (CofileOption)comboBox_cofile_type.SelectedIndex;
			e.Handled = true;
		}
		#endregion

		public class LinuxListViewItem
		{
			private string bindingName = "";
			public string BindingName { get { return bindingName; } set { bindingName = value; } }
			private bool isDirectory = false;
			public bool IsDirectory { get { return isDirectory; } set { isDirectory = value; } }
			private static string[] STR_UNITS = new string[] {"Bytes", "KB", "MB", "GB", "TB", "max" };
			public string Size {
				get
				{
					if(bindingName == "..")
						return null;

					float size = 0;
					string str_unit = STR_UNITS[0];

					// FileInfo == null 이면 '/' 인 최상루트 디렉토리
					if(linuxTVI.FileInfo != null)
					{
						size = linuxTVI.FileInfo.Length;
						float _size;
						int i;
						for(i = 0; i < STR_UNITS.Length; i++)
						{
							_size = size / 1024;
							if(_size < 1)
								break;
							size = _size;
						}
						str_unit = STR_UNITS[i];
					}
					return string.Format("{0} {1}", Math.Round(size), str_unit);
					//return string.Format("{0:N2} {1}", size, str_unit);
				}
			}
			public string LastWriteTime {
				get
				{
					if(bindingName == "..")
						return null;

					string str = "";
					if(linuxTVI.FileInfo != null)
					{
						str = linuxTVI.FileInfo.LastWriteTime.ToString();
					}
					return str;
				}
			}
			public string Type {
				get
				{
					if(isDirectory)
						return "Directory";
					else
					{
						return "File";
					}
				}
			}
			//public string Owner
			//{
			//	get
			//	{
			//		string str = "";

			//		return str;
			//	}
			//}

			private LinuxTreeViewItem linuxTVI = null;
			public LinuxTreeViewItem LinuxTVI { get { return linuxTVI; } set { linuxTVI = value; } }
		}

		static List<LinuxListViewItem> list_LinuxListViewItem = new List<LinuxListViewItem>();
		public static LinuxTreeViewItem cur_LinuxTreeViewItem = null;
		public void RefreshListView(LinuxTreeViewItem cur)
		{
			if(cur == null)
				return;

			cur_LinuxTreeViewItem = cur;
			comboBox_listView_linuxpath.Text = cur.Path;

			list_LinuxListViewItem.Clear();
			LinuxListViewItem llvi = new LinuxListViewItem() { BindingName = "..", IsDirectory = true, LinuxTVI = cur.Parent as LinuxTreeViewItem };
			list_LinuxListViewItem.Add(llvi);
			for(int i = 0; i < cur.Items.Count; i++)
			{
				LinuxTreeViewItem ltvi = cur.Items[i] as LinuxTreeViewItem;
				if(ltvi == null)
					continue;
				llvi = new LinuxListViewItem() { BindingName = ltvi.Header.Text, IsDirectory = ltvi.IsDirectory, LinuxTVI = ltvi };
				list_LinuxListViewItem.Add(llvi);
			}

			listView_linux_files.Items.Clear();
			for(int i = 0; i < list_LinuxListViewItem.Count; i++)
			{
				if(list_LinuxListViewItem[i].LinuxTVI != null && list_LinuxListViewItem[i].LinuxTVI.Visibility == Visibility.Visible)
				{
					listView_linux_files.Items.Add(list_LinuxListViewItem[i]);
				}
			}
		}

		private void OnClickLinuxFileEncrypt(object sender, RoutedEventArgs e)
		{
			ConfirmEncDec(listView_linux_files.SelectedItems.Cast<Object>(), true);
		}
		private void OnClickLinuxFileDecrypt(object sender, RoutedEventArgs e)
		{
			ConfirmEncDec(listView_linux_files.SelectedItems.Cast<Object>(), false);
		}

		private void OnClickWorkFileDelete(object sender, RoutedEventArgs e)
		{
			WorkFileDelete();
		}
		private void OnKeyDownWorkFiles(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Delete)
			{
				WorkFileDelete();
			}
		}
		private void WorkFileDelete()
		{
			var sel_items = listView_work_files.SelectedItems;
			LinuxListViewItem llit;
			for(int i = sel_items.Count - 1; i >= 0; i--)
			{
				llit = sel_items[i] as LinuxListViewItem;
				if(llit == null)
					continue;

				listView_work_files.Items.Remove(llit);
			}
		}
		private void OnClickWorkFileAllEncrypt(object sender, RoutedEventArgs e)
		{
			ConfirmEncDec(listView_work_files.Items.Cast<Object>(), true);
		}
		private void OnClickWorkFileAllDecrypt(object sender, RoutedEventArgs e)
		{
			ConfirmEncDec(listView_work_files.Items.Cast<Object>(), false);
		}
		private void OnClickWorkFileSelectedEncrypt(object sender, RoutedEventArgs e)
		{
			ConfirmEncDec(listView_work_files.SelectedItems.Cast<Object>(), true);
		}
		private void OnClickWorkFileSelectedDecrypt(object sender, RoutedEventArgs e)
		{
			ConfirmEncDec(listView_work_files.SelectedItems.Cast<Object>(), false);
		}
		bool CheckHaveDirectory(IEnumerable<Object> selected_list)
		{
			var enumerator = selected_list.GetEnumerator();
			bool haveDirectory = true;
			for(int i = 0; haveDirectory = enumerator.MoveNext(); i++)
			{
				LinuxTreeViewItem ltvi = enumerator.Current as LinuxTreeViewItem;

				CofileUI.UserControls.Cofile.LinuxListViewItem llvi = enumerator.Current as CofileUI.UserControls.Cofile.LinuxListViewItem;
				if(llvi != null)
					ltvi = llvi.LinuxTVI as LinuxTreeViewItem;

				if(ltvi == null)
					break;

				if(ltvi.IsDirectory)
					break;
			}

			return haveDirectory;
		}
		string GetFileListString(IEnumerable<Object> selected_list)
		{
			string str = "";
			var enumerator = selected_list.GetEnumerator();
			for(int i = 0; enumerator.MoveNext(); i++)
			{
				LinuxTreeViewItem ltvi = enumerator.Current as LinuxTreeViewItem;

				CofileUI.UserControls.Cofile.LinuxListViewItem llvi = enumerator.Current as CofileUI.UserControls.Cofile.LinuxListViewItem;
				if(llvi != null)
					ltvi = llvi.LinuxTVI as LinuxTreeViewItem;

				if(ltvi == null)
					break;

				str += ltvi.Path + "\n";
			}
			return str;
		}

		public void ConfirmEncDec(IEnumerable<Object> selected_list, bool isEncrypt)
		{
			string title = "", message = "";
			MessageDialogStyle dialog_style = MessageDialogStyle.AffirmativeAndNegative;
			WindowMain.CallBack affirmative_callback = delegate
			{
				//TextRange txt = new TextRange(Status.current.richTextBox_status.Document.ContentStart, Status.current.richTextBox_status.Document.ContentEnd);
				//txt.Text = "";
				SSHController.SendNRecvCofileCommand(selected_list, isEncrypt, true);
				//LinuxTreeViewItem.Refresh();
			};
			WindowMain.CallBack negative_callback = null;
			MetroDialogSettings settings = null;

			message += GetFileListString(selected_list);
			message += "\n";

			if(isEncrypt 
				&& SSHController.selected_type == CofileOption.tail 
				&& !CheckHaveDirectory(selected_list))
			{
				SSHController.view_message_caption = "Encrypt";
				title += Resources["String.MainDialog.Encrypt.Title"];
				message += Resources["String.MainDialog.Encrypt.Message.Tail"];

				dialog_style = MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary;
				negative_callback = delegate
				{
					SSHController.SendNRecvCofileCommand(selected_list, isEncrypt, false);
				};
				settings = new MetroDialogSettings()
				{
					AffirmativeButtonText = "Yes"
					, NegativeButtonText = "No"
					, FirstAuxiliaryButtonText = "Cancel"
					//, ColorScheme = MetroDialogOptions.ColorScheme
				};
			}
			else if(isEncrypt)
			{
				SSHController.view_message_caption = "Encrypt";
				title += Resources["String.MainDialog.Encrypt.Title"];
				message += Resources["String.MainDialog.Encrypt.Message"];
			}
			else
			{
				SSHController.view_message_caption = "Decrypt";
				title += Resources["String.MainDialog.Decrypt.Title"];
				message += Resources["String.MainDialog.Decrypt.Message"];
			}
			message += "\n [ Type = " + SSHController.selected_type.ToString().ToUpper() + " ]";

			WindowMain.current.ShowMessageDialog(title, message, dialog_style, affirmative_callback, negative_callback, settings: settings);
			
		}

		private void OnButtonClickRefresh(object sender, RoutedEventArgs e)
		{
			if(LinuxTreeViewItem.root == null)
				Refresh();

			else if(LinuxTreeViewItem.root != null)
			{
				string path = null;
				if(LinuxTreeViewItem.Last_Refresh == null)
					path = SSHController.WorkingDirectory;
				else
					path = LinuxTreeViewItem.Last_Refresh.Path;

				if(path != null)
				{
					LinuxTreeViewItem.root.RefreshChild(path, false);
					RefreshListView(LinuxTreeViewItem.Last_Refresh);
				}
			}
		}

		private void OnKeyDownLinuxPath(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter)
			{
				LinuxTreeViewItem.root.RefreshChild(comboBox_listView_linuxpath.Text, false);
				RefreshListView(LinuxTreeViewItem.Last_Refresh);
			}
		}

		public void Clear()
		{
			LinuxTreeViewItem.Clear();
			treeView_linux_directory.Items.Clear();
			listView_linux_files.Items.Clear();
			listView_work_files.Items.Clear();
			textBlock_selected_config_file_name.Text = "Not Selected";
		}

		DateTime LastKeyDonwTime = DateTime.Now;
		double SecKeyDownInterval = .2;
		StringBuilder sb_keydown = new StringBuilder();
		private void OnKeyDownLinuxFiles(object sender, KeyEventArgs e)
		{
			LinuxListViewItem llvi;
			switch(e.Key)
			{
				case Key.Enter:
					llvi = listView_linux_files.SelectedItem as LinuxListViewItem;
					if(llvi == null)
						break;
					OpenDirectory(llvi);
					break;
				case Key.Back:
					if(listView_linux_files.Items.Count > 0)
					{
						llvi = listView_linux_files.Items[0] as LinuxListViewItem;
						if(llvi == null)
							break;

						OpenDirectory(llvi);
					}
					break;
				default:
					if((e.Key >= Key.A && e.Key <= Key.Z )
						|| e.Key == Key.OemPeriod)
					{
						if(LastKeyDonwTime.AddSeconds(SecKeyDownInterval) < DateTime.Now)
						{
							sb_keydown.Clear();
						}

						sb_keydown.Append(KeyCodeToChar(e.Key));
						FocusListView(listView_linux_files, sb_keydown.ToString());

						LastKeyDonwTime = DateTime.Now;

						// 'c' 입력시 넥스트 아이템으로 가게되어서 처리됨을 true로 줌
						e.Handled = true;
					}
					break;
			}
			e.Handled = true;
		}
		public char KeyCodeToChar(Key keyCode)
		{
			char keyChar = ' ';
			switch(keyCode)
			{
				case Key.OemPeriod:
					keyChar = '.';
					break;
				default:
					keyChar = keyCode.ToString().ToLower()[0];
					break;
			}
			return keyChar;
		}
		int FocusListView(ListView lv, string find)
		{
			int idx_start = lv.SelectedIndex;

			// find.Length == 1 일때는 lv.SelectedIndex == -1 일때도 포함.
			// 처음 키보드찾기를 할 때, find.Length 는 1부터 시작할 수 밖에 없음.
			if(find.Length == 1 || idx_start == -1)
			{
				idx_start++;
				if(idx_start >= lv.Items.Count)
					idx_start = 0;
			}

			int idx = idx_start;
			for(int cnt = 0; cnt < lv.Items.Count; cnt++)
			{
				LinuxListViewItem llvi = lv.Items[idx] as LinuxListViewItem;
				if(llvi == null)
					continue;
				string name = llvi.BindingName.ToLower();
				int j;
				for(j = 0; j < name.Length && j < find.Length; j++)
				{
					if(name[j] != find[j])
						break;
				}

				if(j == find.Length)
				{
					((UIElement)lv.ItemContainerGenerator.ContainerFromItem(lv.Items[idx])).Focus();
					break;
				}

				idx++;
				if(idx >= lv.Items.Count)
					idx = 0;
			}
			return 0;
		}

	}
}
