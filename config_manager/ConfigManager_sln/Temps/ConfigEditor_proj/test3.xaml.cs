using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Collections.Specialized;

namespace Manager_proj_4
{
	/// <summary>
	/// test3.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class test3 : Window
	{
		string root_path = AppDomain.CurrentDomain.BaseDirectory;
		public test3()
		{
			InitializeComponent();
			this.Closed += Test3_Closed;
			new JsonInfo(json_tree_view);
		}
		
		private void Test3_Closed(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
		}

		

		#region Json Event Function
		public void refreshJsonTree(JToken jtok_root)
		{
			JsonTreeViewItem root_jtree = JsonTreeViewItem.convertToTreeViewItem(jtok_root);
			if(root_jtree == null)
				return;

			// 삭제
			json_tree_view.Items.Clear();

			KeyTextBox tb = new KeyTextBox(JsonInfo.current.Filename, false);
			root_jtree.Header.Children.Insert(0, tb);
			json_tree_view.Items.Add(root_jtree);
		}
		
		private void OnClickButtonNewJsonFile(object sender, RoutedEventArgs e)
		{
			if(JsonInfo.current == null)
				return;

			JsonInfo.current.Clear();
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
			refreshJsonTree(new JObject());
		}
		private void OnClickButtonOpenJsonFile(object sender, RoutedEventArgs e)
		{
			if(JsonInfo.current == null)
				return;

			JsonInfo.current.Clear();
			OpenFileDialog ofd = new OpenFileDialog();

			ofd.InitialDirectory = root_path;

			if(JsonInfo.current.Path != null)
			{
				string dir_path = JsonInfo.current.Path.Substring(0, JsonInfo.current.Path.LastIndexOf('\\') + 1);
				DirectoryInfo d = new DirectoryInfo(dir_path);
				if(d.Exists)
					ofd.InitialDirectory = dir_path;
			}

			// 파일 열기
			ofd.Filter = "JSon Files (.json)|*.json";
			if(ofd.ShowDialog(this) == true)
			{
				Console.WriteLine(ofd.FileName);

				string json = FileContoller.read(ofd.FileName);
				refreshJsonTree(JsonController.parseJson(json));

				JsonInfo.current.Path = ofd.FileName;
			}
		}
		private void OnClickButtonSaveJsonFile(object sender, RoutedEventArgs e)
		{
			if(JsonInfo.current == null)
				return;

			if(JsonInfo.current.Path == null)
			{
				OnClickButtonOtherSaveJsonFile(sender, e);
				return;
			}

			FileInfo f = new FileInfo(JsonInfo.current.Path);
			if(!f.Exists)
			{
				OnClickButtonOtherSaveJsonFile(sender, e);
				return;
			}

			// root JsonTreeViewItem = TreeView.Items[0]
			JsonTreeViewItem root = json_tree_view.Items[0] as JsonTreeViewItem;
			if(root == null)
				return;

			JToken Jtok_root = JsonTreeViewItem.convertToJToken(root);
			FileContoller.write(JsonInfo.current.Path, Jtok_root.ToString());
		}
		private void OnClickButtonOtherSaveJsonFile(object sender, RoutedEventArgs e)
		{
			if(JsonInfo.current == null)
				return;

			SaveFileDialog sfd = new SaveFileDialog();

			sfd.InitialDirectory = root_path;

			if(JsonInfo.current.Path != null)
			{
				string dir_path = JsonInfo.current.Path.Substring(0, JsonInfo.current.Path.LastIndexOf('\\') + 1);
				DirectoryInfo d = new DirectoryInfo(dir_path);
				if(d.Exists)
					sfd.InitialDirectory = dir_path;
			}

			sfd.Filter = "JSon Files (.json)|*.json";
			if(sfd.ShowDialog(this) == true)
			{
				// root JsonTreeViewItem = TreeView.Items[0]
				JsonTreeViewItem root = json_tree_view.Items[0] as JsonTreeViewItem;
				if(root == null)
					return;

				JToken Jtok_root = JsonTreeViewItem.convertToJToken(root);
				FileContoller.write(sfd.FileName, Jtok_root.ToString());
				JsonInfo.current.Path = sfd.FileName;
			}
		}
		private void OnClickButtonViewJsonFile(object sender, RoutedEventArgs e)
		{
			if(JsonInfo.current == null || JsonInfo.current.Path == null)
				return;

			JsonTreeViewItem root = json_tree_view.Items[0] as JsonTreeViewItem;
			if(root == null)
				return;

			Window_ViewFile w = new Window_ViewFile(JsonTreeViewItem.convertToJToken(root).ToString(), JsonInfo.current.Path);
			//Window_ViewFile w = new Window_ViewFile(FileContoller.read(JsonInfo.current.Path), JsonInfo.current.Path);

			if(w.ShowDialog() == true)
			{
				refreshJsonTree(JsonController.parseJson(w.tb_file.Text));
			}
		}
		private void OnClickButtonCancelJsonFile(object sender, RoutedEventArgs e)
		{
			if(JsonInfo.current == null || JsonInfo.current.Path == null)
				return;

			string dir_path = JsonInfo.current.Path.Substring(0, JsonInfo.current.Path.LastIndexOf('\\') + 1);
			DirectoryInfo d = new DirectoryInfo(dir_path);
			if(!d.Exists)
				return;

			string json = FileContoller.read(JsonInfo.current.path);
			refreshJsonTree(JsonController.parseJson(json));
		}
		#endregion
	}
}