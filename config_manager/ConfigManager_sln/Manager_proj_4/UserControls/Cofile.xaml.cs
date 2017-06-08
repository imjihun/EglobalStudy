using Manager_proj_4.Classes;
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

namespace Manager_proj_4.UserControls
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
		void InitLinuxDirectory()
		{
			textBox_linux_directory_filter.TextChanged += delegate { LinuxTreeViewItem.Filter_string = textBox_linux_directory_filter.Text; };
			checkBox_hidden.Checked += delegate { LinuxTreeViewItem.Bool_except_hidden = false; };
			checkBox_hidden.Unchecked += delegate { LinuxTreeViewItem.Bool_except_hidden = true; };
			checkBox_hidden.IsChecked = false;

			//// BackgroundWorker의 이벤트 처리기
			//LinuxTreeViewItem.BackgroundReConnector.DoWork += LinuxTreeViewItem.BackgroundReConnect;
			//LinuxTreeViewItem.BackgroundReConnector.RunWorkerCompleted += LinuxTreeViewItem.BackgroundReConnectCallBack;
			//LinuxTreeViewItem.BackgroundReConnector.WorkerReportsProgress = true;
			//LinuxTreeViewItem.BackgroundReConnector.WorkerSupportsCancellation = true;
			//LinuxTreeViewItem.BackgroundReConnector.ProgressChanged += new ProgressChangedEventHandler(_backgroundWorker_ProgressChanged);
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
			listView_linux_directory.Items.Clear();

			// 추가
			//string home_dir = sftp.WorkingDirectory;
			// root 의 path 는 null 로 초기화
			LinuxTreeViewItem.root = new LinuxTreeViewItem(null, null, true);
			treeView_linux_directory.Items.Add(LinuxTreeViewItem.root);
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


		public void RefreshListView(string path)
		{
			SftpFile[] files = SSHController.PollListInDirectory(path);
			if(files == null)
				return;

			label_listView.Content = path;
			RefreshListView(files);
		}
		public void RefreshListView(SftpFile[] files)
		{
			listView_linux_directory.Items.Clear();

			int count_dir = 0;
			foreach(var file in files)
			{
				Regex r = new Regex(@"(^\.)([^\.])");
				if(r.IsMatch(file.Name) || file.Name == ".")
					continue;

				if(file.IsDirectory)
					listView_linux_directory.Items.Insert(count_dir++, file);
				else
					listView_linux_directory.Items.Add(file);
			}
			//listView_linux_directory.ItemsSource = files;
		}
		private void OnMouseLeftButtonDownListViewItems(object sender, MouseButtonEventArgs e)
		{
			if(e.ClickCount > 1)
			{
				Console.WriteLine(((Grid)sender).DataContext.GetType());
				SftpFile file = ((Grid)sender).DataContext as SftpFile;
				if(file == null)
					return;

				if(file.IsDirectory)
				{
					RefreshListView(file.FullName);
				}
			}
		}

		private void OnMouseEnterSplitView(object sender, MouseEventArgs e)
		{
			splitView.IsPaneOpen = true;
		}
		private void OnMouseDownBackground(object sender, MouseButtonEventArgs e)
		{
			splitView.IsPaneOpen = false;
		}
	}
}
