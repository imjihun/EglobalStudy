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
			//textBox_linux_directory_filter.Text = @"^[^\.]";
		}

		#region Linux Directory

		void InitLinuxDirectory()
		{
			textBox_linux_directory_filter.TextChanged += delegate { LinuxTreeViewItem.Filter_string = textBox_linux_directory_filter.Text; };
			checkBox_hidden.Checked += delegate { LinuxTreeViewItem.Bool_hidden = false; };
			checkBox_hidden.Unchecked += delegate { LinuxTreeViewItem.Bool_hidden = true; };
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
			LinuxTreeViewItem.Refresh();
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

			if(JsonInfo.current != null && JsonInfo.current.Path != null)
			{
				string dir_path = JsonInfo.current.Path.Substring(0, JsonInfo.current.Path.LastIndexOf('\\') + 1);
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
			SSHController.selected_type = (Manager_proj_4.CofileOption)comboBox_cofile_type.SelectedIndex;
			e.Handled = true;
		}
		#endregion
	}
}
