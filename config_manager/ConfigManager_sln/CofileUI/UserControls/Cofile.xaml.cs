﻿using CofileUI.Classes;
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
			//Console.WriteLine("ItemContainerStyle = " + listView_linux_files.ItemContainerStyle);

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
					if(selected.LinuxTVI == null)
						return;

					selected.LinuxTVI.RefreshChild();
					//RefreshListView(selected.LinuxTVI);

					//llvi.LinuxTVI.IsExpanded = false;
					//llvi.LinuxTVI.IsExpanded = true;
					selected.LinuxTVI.Focus();
				}
				else
				{
					EncryptFileOpen();
				}
			}
		}

		public static string PreviewExtension = "preview_extension_ll";
		static ulong Idx_Encrypt_File_Open = 0;
		string FindDecryptFile(string remote_path_enc_file)
		{
			JToken jtok = JsonController.parseJson(FileContoller.Read(current.Selected_config_file_path));
			if(jtok == null)
				return null;
			
			JValue jval_input_extansion = jtok["dec_option"]["input_extension"] as JValue;
			string output_extension = PreviewExtension;

			string remote_path = remote_path_enc_file;
			if(remote_path.Length > jval_input_extansion.Value.ToString().Length + 1)
				//&& remote_path.Substring(remote_path.Length - jval_input_extansion.Value.ToString().Length) == jval_input_extansion.Value.ToString())
				remote_path = remote_path.Substring(0, remote_path.Length - jval_input_extansion.Value.ToString().Length - 1);

			string remote_path_dec_file = remote_path + "." + output_extension;

			return remote_path_dec_file;
		}
		void EncryptFileOpen()
		{
			if(listView_linux_files.SelectedItems.Count < 1)
				return;

			LinuxListViewItem llvi = listView_linux_files.SelectedItems[0] as LinuxListViewItem;
			if(llvi == null)
				return;

			if(SSHController.SendNRecvCofileCommandPreview(listView_linux_files.SelectedItems.Cast<Object>(), false))
			{
				string remote_path = llvi.LinuxTVI.Path;

				string local_filename = "temp" + Idx_Encrypt_File_Open++;
				string remote_path_dec_file  = FindDecryptFile(remote_path);
				if(SSHController.moveFileToLocal(root_path, remote_path_dec_file, local_filename))
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
						FileContoller.Delete(url_localfile);

						wvi.ShowDialog();
					}
					else
					{

						string str = FileContoller.Read(url_localfile);
						FileContoller.Delete(url_localfile);

						Window_ViewFile wvf = new Window_ViewFile(str, llvi.LinuxTVI.FileInfo.Name);
						wvf.ShowDialog();
					}
				}
				else
				{
					Log.PrintError("Decryption Failed\r\tCheck the config file", "ListView_linux_files_MouseDoubleClick", Status.current.richTextBox_status);
				}
			}


			//string remote_path_enc_file = llvi.LinuxTVI.Path;
			//string[] split = remote_path_enc_file.Split('/');
			//if(remote_path_enc_file.Length <= split[split.Length - 1].Length)
			//	return;

			//string remote_path_configfile = SSHController.GetRemoteConfigFilePath(Selected_config_file_path);
			//string remote_path_enc_file_base_dir = remote_path_enc_file.Substring(0, remote_path_enc_file.Length - split[split.Length - 1].Length);
			////string command = "cofile file -d -f " + remote_path_enc_file + " -c " + remote_path_configfile + " -id / -od " + remote_path_enc_file_base_dir;
			//// # cofile file -d -f /home/cofile/TestOutput/tail_config_default.json.coenc -c /home/cofile/var/conf/cofile/file_config_default1.json -id / -od /home/cofile/TestOutput/
			//// 위의 커맨드를 실행시키니 output_dir 경로가 중복된다.
			//string command = "cofile file -d -f " + remote_path_enc_file + " -c " + remote_path_configfile + " -id / -od /";
			//SSHController.RunCofileCommand(command);
			//Console.WriteLine("command = " + command);

			//string remote_path_dec_file  = FindDecryptFile(remote_path_enc_file);
			//Console.WriteLine("remote_path_dec_file  = " + remote_path_dec_file);

			//string local_filename = "tmp" + Idx_Encrypt_File_Open++;
			//if(SSHController.moveFileToLocal(root_path, remote_path_dec_file, local_filename))
			//{
			//	llvi.LinuxTVI.RefreshChildFromParent();
			//	Cofile.current.RefreshListView(Cofile.cur_LinuxTreeViewItem);

			//	string url_localfile = root_path + local_filename;
			//	string[] split_expansion = remote_path_dec_file.Split('.');
			//	string expansion = split_expansion[split_expansion.Length - 2];
			//	if(expansion == "gif"
			//		|| expansion == "bmp"
			//		|| expansion == "jpg"
			//		|| expansion == "png"
			//		)
			//	{
			//		Window_ViewImage wvi = new Window_ViewImage(LoadImage(url_localfile), llvi.LinuxTVI.FileInfo.Name);
			//		File.Delete(url_localfile);

			//		wvi.ShowDialog();
			//	}
			//	else
			//	{

			//		string str = FileContoller.Read(url_localfile);
			//		File.Delete(url_localfile);

			//		Window_ViewFile wvf = new Window_ViewFile(str, llvi.LinuxTVI.FileInfo.Name);
			//		wvf.ShowDialog();
			//	}
			//}
			//else
			//{
			//	Log.PrintError("Decryption Failed\r\tCheck the config file", "ListView_linux_files_MouseDoubleClick", Status.current.richTextBox_status);
			//}


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
			treeView_linux_directory.Items.Clear();
			listView_linux_files.Items.Clear();

			// 추가
			//string home_dir = sftp.WorkingDirectory;
			// root 의 path 는 null 로 초기화
			LinuxTreeViewItem.root = new LinuxTreeViewItem("/", null, "/", true, null);
			treeView_linux_directory.Items.Add(LinuxTreeViewItem.root);
			string working_dir = SSHController.WorkingDirectory;
			if(working_dir == null)
				return -1;

			LinuxTreeViewItem.root.RefreshChild(working_dir, false);
			Cofile.current.RefreshListView(LinuxTreeViewItem.Last_Refresh);
			Log.PrintConsole("[refresh]");

			return 0;
			//LinuxTreeViewItem.ReconnectServer();
		}
		private string selected_config_file_path = "Not Selected";
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

			OpenFileDialog ofd = new OpenFileDialog();

			// 초기경로 지정
			ConfigJsonTree.current.Refresh();
			ofd.InitialDirectory = ConfigJsonTree.cur_root_path;

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
			public string Size { get
				{
					float size = linuxTVI.FileInfo.Length;
					string str_unit;

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

					return string.Format("{0} {1}", Math.Round(size), str_unit);
					//return string.Format("{0:N2} {1}", size, str_unit);
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

		public void ConfirmEncDec(IEnumerable<Object> selected_list, bool isEncrypt)
		{
			string title = "", message = "";

			var enumerator = selected_list.GetEnumerator();
			for(int i = 0; enumerator.MoveNext(); i++)
			{
				LinuxTreeViewItem ltvi = enumerator.Current as LinuxTreeViewItem;

				CofileUI.UserControls.Cofile.LinuxListViewItem llvi = enumerator.Current as CofileUI.UserControls.Cofile.LinuxListViewItem;
				if(llvi != null)
					ltvi = llvi.LinuxTVI as LinuxTreeViewItem;

				if(ltvi == null)
					break;

				message += ltvi.Path + "\n";
			}

			message += "\n";
			if(isEncrypt)
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

			WindowMain.current.ShowMessageDialog(title
												, message
												, MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative
												, delegate
												{
													//TextRange txt = new TextRange(Status.current.richTextBox_status.Document.ContentStart, Status.current.richTextBox_status.Document.ContentEnd);
													//txt.Text = "";
													SSHController.SendNRecvCofileCommand(selected_list, isEncrypt);
													//LinuxTreeViewItem.Refresh();
												});
		}
		//public void AllEncDec_(IEnumerable<Object> selected_list, bool isEncrypt)
		//{
		//	string title, message;
		//	if(isEncrypt)
		//	{
		//		title = "All Encrypt";
		//		message = "모두 암호화 수행하시겠습니까?";
		//	}
		//	else
		//	{
		//		title = "All Decrypt";
		//		message = "모두 복호화 수행하시겠습니까?";
		//	}

		//	WindowMain.current.ShowMessageDialog(title
		//											, message
		//											, MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative
		//											, delegate
		//											{
		//												SSHController.SendNRecvCofileCommand(selected_list, isEncrypt);
		//											});
		//}
		//public void SelectedEncDec_(IEnumerable<Object> selected_list, bool isEncrypt)
		//{
		//	string title, message;
		//	if(isEncrypt)
		//	{
		//		title = "Selected Encrypt";
		//		message = "선택된 항목을 암호화 수행하시겠습니까?";
		//	}
		//	else
		//	{
		//		title = "Selected Decrypt";
		//		message = "선택된 항목을 복호화 수행하시겠습니까?";
		//	}

		//	WindowMain.current.ShowMessageDialog(title
		//										, message
		//										, MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative
		//										, delegate 
		//										{
		//											SSHController.SendNRecvCofileCommand(selected_list, isEncrypt);
		//										});
		//}

		private void OnButtonClickRefresh(object sender, RoutedEventArgs e)
		{
			LinuxTreeViewItem.root.RefreshChild(LinuxTreeViewItem.Last_Refresh.Path, false);
			RefreshListView(LinuxTreeViewItem.Last_Refresh);
		}

		private void label_listView_linux_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter)
			{
				LinuxTreeViewItem.root.RefreshChild(comboBox_listView_linuxpath.Text, false);
				RefreshListView(LinuxTreeViewItem.Last_Refresh);
			}
		}

		public void Clear()
		{
			treeView_linux_directory.Items.Clear();
			listView_linux_files.Items.Clear();
			listView_work_files.Items.Clear();
		} 
	}
}
