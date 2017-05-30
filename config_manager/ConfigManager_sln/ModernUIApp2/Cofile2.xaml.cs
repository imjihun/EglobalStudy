using Manager_proj_4;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
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

namespace ModernUIApp2
{
	/// <summary>
	/// Cofile2.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class Cofile2 : UserControl
	{
		public static Cofile2 current;
		public Cofile2()
		{
			current = this;
			InitializeComponent();
			//this.Closed += test4_Closed;

			InitServerTab();
			
			//InitServerCommandView();
			InitLinuxDirectory();
		}

		private void TextBox_TextChanged_ScrollToEnd(object sender, TextChangedEventArgs e)
		{
			//textBox_status.ScrollToEnd();
			richTextBox_status.ScrollToEnd();
		}

		public static bool bCtrl = false;
		static bool bShift = false;
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

		#region Server Menu Class
		void InitServerTab()
		{
			// serverinfo.json 파일 로드
			FileInfo fi = new FileInfo(ServerInfo.PATH);
			if(fi.Exists)
			{
				string json = FileContoller.read(ServerInfo.PATH);
				try
				{
					ServerInfo.jobj_root = JObject.Parse(json);
					ServerPanel panel_server = ServerInfo.ConvertFromJson(ServerInfo.jobj_root);

					grid_server.Children.Add(panel_server);
				}
				catch(Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}
			else
			{
				try
				{
					ServerInfo.jobj_root = new JObject(new JProperty("Server", new JObject()));
					ServerPanel panel_server = ServerInfo.ConvertFromJson(ServerInfo.jobj_root);

					grid_server.Children.Add(panel_server);
				}
				catch(Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}
			if(ServerMenuButton.group.Count > 0)
				ServerMenuButton.group[0].IsChecked = true;
		}
		#endregion

		#region Cofile
		string root_path = AppDomain.CurrentDomain.BaseDirectory;
		void InitLinuxDirectory()
		{
			textBox_linux_directory_filter.TextChanged += delegate { LinuxTreeViewItem.Filter_string = textBox_linux_directory_filter.Text; };

			//// BackgroundWorker의 이벤트 처리기
			//LinuxTreeViewItem.BackgroundReConnector.DoWork += LinuxTreeViewItem.BackgroundReConnect;
			//LinuxTreeViewItem.BackgroundReConnector.RunWorkerCompleted += LinuxTreeViewItem.BackgroundReConnectCallBack;
			//LinuxTreeViewItem.BackgroundReConnector.WorkerReportsProgress = true;
			//LinuxTreeViewItem.BackgroundReConnector.WorkerSupportsCancellation = true;
			//LinuxTreeViewItem.BackgroundReConnector.ProgressChanged += new ProgressChangedEventHandler(_backgroundWorker_ProgressChanged);
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
			ofd.InitialDirectory = root_path;

			if(JsonInfo.current != null && JsonInfo.current.Path != null)
			{
				string dir_path = JsonInfo.current.Path.Substring(0, JsonInfo.current.Path.LastIndexOf('\\') + 1);
				DirectoryInfo d = new DirectoryInfo(dir_path);
				if(d.Exists)
					ofd.InitialDirectory = dir_path;
			}

			// 파일 열기
			ofd.Filter = "JSon Files (.json)|*.json";
			if(ofd.ShowDialog(/*this*/) == true)
			{
				Selected_config_file_path = ofd.FileName;
				Console.WriteLine(Selected_config_file_path);
				//Console.WriteLine(ofd.FileName);

				//string json = FileContoller.read(ofd.FileName);
				//refreshJsonTree(JsonController.parseJson(json));

				//JsonInfo.current.Path = ofd.FileName;
			}
		}
		#endregion

		#region Server Command View
		void InitServerCommandView()
		{
			CommandView commandView;
			commandView = new CommandView();
			//grid_second.Children.Add(commandView);
		}
		#endregion
		
	}
}