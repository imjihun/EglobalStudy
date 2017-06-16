using Manager_proj_4_net4.Classes;
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
using Manager_proj_4_net4.Windows;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Globalization;
using System.Windows.Controls.Primitives;
using Newtonsoft.Json.Linq;

namespace Manager_proj_4_net4.UserControls
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

			listView_work_files.Drop += ListView_work_files_Drop;
			listView_work_files.AllowDrop = true;
			
			listView_linux_files.MouseDoubleClick += ListView_linux_files_MouseDoubleClick;
			listView_linux_files.MouseMove += ListView_linux_files_MouseMove;
		}

		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			listView_linux_files.View = Resources["GridView_ListViewLinux_Detail"] as GridView;
		}
		private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			listView_linux_files.View = null;
		}
		private void ListView_linux_files_MouseMove(object sender, MouseEventArgs e)
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
		private void ListView_linux_files_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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
					RefreshListView(selected.LinuxTVI);
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
		
		static ulong Idx_Encrypt_File_Open = 0;
		void EncryptFileOpen()
		{
			if(listView_linux_files.SelectedItems.Count < 1)
				return;

			LinuxListViewItem llvi = listView_linux_files.SelectedItems[0] as LinuxListViewItem;
			if(llvi == null)
				return;

			if(SSHController.SendNRecvCofileCommand(listView_linux_files.SelectedItems.Cast<Object>(), false))
			{
				string remote_path = llvi.LinuxTVI.Path;
				JToken jtok = JsonController.parseJson(FileContoller.Read(current.Selected_config_file_path));
				if(jtok == null)
					return;

				JValue jval_input_extansion = jtok["dec_option"]["input_extension"] as JValue;
				JValue jval_output_extension = jtok["dec_option"]["output_extension"] as JValue;

				if(remote_path.Length > jval_input_extansion.Value.ToString().Length + 1
					&& remote_path.Substring(remote_path.Length - jval_input_extansion.Value.ToString().Length) == jval_input_extansion.Value.ToString())
					remote_path = remote_path.Substring(0, remote_path.Length - jval_input_extansion.Value.ToString().Length - 1);

				string remote_dec_path = remote_path + "." + jval_output_extension.Value.ToString();
				string[] split_remote_path = remote_path.Split('/');
				string local_filename = split_remote_path[split_remote_path.Length - 1] + Idx_Encrypt_File_Open++;

				if(SSHController.moveFileToLocal(root_path, remote_dec_path, local_filename))
				{
					llvi.LinuxTVI.RefreshChildFromParent();
					Cofile.current.RefreshListView(Cofile.cur_LinuxTreeViewItem);

					string url_localfile = root_path + local_filename;
					string[] split = remote_path.Split('.');
					if(split[split.Length - 1] == "gif"
						|| split[split.Length - 1] == "bmp"
						|| split[split.Length - 1] == "jpg"
						|| split[split.Length - 1] == "png"
						)
					{
						Window_ViewImage wvi = new Window_ViewImage(LoadImage(url_localfile), llvi.LinuxTVI.FileInfo.Name);
						File.Delete(url_localfile);

						wvi.ShowDialog();
					}
					else
					{

						string str = FileContoller.Read(url_localfile);
						File.Delete(url_localfile);

						Window_ViewFile wvf = new Window_ViewFile(str, llvi.LinuxTVI.FileInfo.Name);
						wvf.ShowDialog();
					}
				}
				else
				{
					Log.PrintError("Decryption Failed\r\tCheck the config file", "ListView_linux_files_MouseDoubleClick", Status.current.richTextBox_status);
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
		private void ListView_work_files_Drop(object sender, DragEventArgs e)
		{
			// If the DataObject contains string data, extract it.
			if(e.Data.GetDataPresent("Object"))
			{
				Object data_obj = (Object)e.Data.GetData("Object");

				List<LinuxTreeViewItem> list_ltvi = data_obj as List<LinuxTreeViewItem>;
				ObservableCollection<object> list_llvi = data_obj as ObservableCollection<object>;
				//Console.WriteLine(data_obj.GetType());
				if(list_ltvi != null)
				{
					for(int i = 0; i < list_ltvi.Count; i++)
					{
						listView_work_files.Items.Add(new LinuxListViewItem() { BindingName = list_ltvi[i].Header.Text, IsDirectory = list_ltvi[i].IsDirectory, LinuxTVI = list_ltvi[i] });
					}
				}
				else if(list_llvi != null)
				{
					for(int i = 0; i < list_llvi.Count; i++)
					{
						listView_work_files.Items.Add(list_llvi[i]);
					}
				}
			}
		}

		public void Refresh()
		{
			if(WindowMain.current == null)
				return;

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
			LinuxTreeViewItem.root.RefreshChild(SSHController.WorkingDirectory);
			Cofile.current.RefreshListView(LinuxTreeViewItem.Last_Refresh_Parent);
			Log.PrintConsole("[refresh]");

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
			OpenFileDialog ofd = new OpenFileDialog();

			// 초기경로 지정
			ofd.InitialDirectory = ConfigJsonTree.root_path;

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
				Selected_config_file_path = ofd.FileName;
				Console.WriteLine(Selected_config_file_path);
				//Console.WriteLine(ofd.FileName);

				//string json = FileContoller.read(ofd.FileName);
				//refreshJsonTree(JsonController.parseJson(json));

				//JsonInfo.current.Path = ofd.FileName;
			}
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
			public string Type {
				get
				{
					if(isDirectory)
						return "Directory";
					else
						return "File";
				}
			}

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
			label_listView.Content = cur.Path;

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
			//listView_linux_files.ItemsSource = list_LinuxListViewItem;


			//listView_linux_files.Items.Clear();
			//listView_linux_files.Items.Add(new LinuxListViewItem() { BindingName = "..", IsDirectory = true, LinuxTVI = cur.Parent as LinuxTreeViewItem });
			//for(int i = 0; i < cur.Items.Count; i++)
			//{
			//	LinuxTreeViewItem ltvi = cur.Items[i] as LinuxTreeViewItem;
			//	if(ltvi == null)
			//		continue;
			//	listView_linux_files.Items.Add(new LinuxListViewItem() { BindingName = ltvi.Header.Text, IsDirectory = ltvi.IsDirectory, LinuxTVI = ltvi });
			//}
		}
		//public void RefreshListView(string path, SftpFile[] files = null)
		//{
		//	if(files == null)
		//	{
		//		files = SSHController.PollListInDirectory(path);
		//		if(files == null)
		//			return;
		//	}

		//	label_listView.Content = path;
		//	listView_linux_files.Items.Clear();

		//	int count_dir = 0;
		//	foreach(var file in files)
		//	{
		//		Regex r = new Regex(@"(^\.)([^\.])");
		//		if(r.IsMatch(file.Name) || file.Name == ".")
		//			continue;

		//		if(file.IsDirectory)
		//			listView_linux_files.Items.Insert(count_dir++, file);
		//		else
		//			listView_linux_files.Items.Add(file);
		//	}
		//	//listView_linux_directory.ItemsSource = files;
		//}

		private void OnClickDelWorkList(object sender, RoutedEventArgs e)
		{
			LinuxListViewItem llit = listView_work_files.SelectedItem as LinuxListViewItem;
			if(llit == null)
				return;

			listView_work_files.Items.Remove(llit);
		}
		private void OnClickEncrypt(object sender, RoutedEventArgs e)
		{
			SSHController.SendNRecvCofileCommand(listView_work_files.Items.Cast<Object>(), true);
		}
		private void OnClickDecrypt(object sender, RoutedEventArgs e)
		{
			SSHController.SendNRecvCofileCommand(listView_work_files.Items.Cast<Object>(), true);
		}

	}





	public class HideStringToDoubleConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			string str = value.ToString();
			if(str[0] == '.')
				return .5;
			return 1;
		}

		public object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			throw new Exception();
		}
	}

}
