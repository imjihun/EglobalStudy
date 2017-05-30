using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.IO;

namespace Manager_proj_4
{
	/// <summary>
	/// Window_Search.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class Window_Search : Window
	{
		public string path_return = null;
		public class DirTreeViewItem : TreeViewItem
		{
			public static string last_path = "";

			public string path;
			public DirTreeViewItem(string _path)
			{
				path = _path;
				this.Header = _path;
				
				refreshChild();
				this.IsExpanded = true;
			}

			protected override void OnSelected(RoutedEventArgs e)
			{
				DirTreeViewItem tvi;
				//if(this.Items.Count > 0)
				//{
				//	tvi = this;
				//}
				//else//(this.Items.Count == 0)
				//{
				//	tvi = this.Parent as DirTreeViewItem;
				//	if(tvi == null)
				//		return;
				//	m_wnd.setTextBox_name(this.Header.ToString());
				//}
				tvi = this;

				last_path = tvi.path;
				m_wnd.refreshListView_dir(tvi);
			}
			protected override void OnExpanded(RoutedEventArgs e)
			{
				base.OnExpanded(e);
				refreshGrandChild();
			}
			private DirTreeViewItem(DirTreeViewItem tvi_parent, string _path)
			{
				path = _path;
				string[] split = _path.Split('\\');
				this.Header = split[split.Length - 1];
				
				tvi_parent.Items.Add(this);
			}

			public static string[] loadFile(string path)
			{
				DirectoryInfo d = new DirectoryInfo(path);
				if(!d.Exists)
					return null;
				try
				{
					return Directory.GetFiles(path);
				}
				catch(Exception e)
				{
					Console.WriteLine(e.Message);
					return null;
				}
			}

			private void refreshChild()
			{
				this.unloadChild();
				DirectoryInfo d = new DirectoryInfo(path);
				if(!d.Exists)
					return;

				try
				{
					string[] arr_dir = Directory.GetDirectories(path);
					for(int i = 0; i < arr_dir.Length; i++)
					{
						DirTreeViewItem child_tvi = new DirTreeViewItem(this, arr_dir[i]);
					}
				}
				catch(Exception e)
				{
					Console.WriteLine("GetDirectories() = " + e.Message);
					Console.WriteLine(path);
					Console.WriteLine();

					DirTreeViewItem parent_tvi = this.Parent as DirTreeViewItem;
					if(parent_tvi == null)
						return;
					parent_tvi.Items.Remove(this);
				}
			}
			private void refreshGrandChild()
			{
				this.unloadGrandChild();
				for(int i = 0; i < this.Items.Count; i++)
				{
					DirTreeViewItem tvi_child = this.Items[i] as DirTreeViewItem;
					if(tvi_child == null)
						continue;

					tvi_child.refreshChild();
				}
			}
			private void unloadChild()
			{
				this.Items.Clear();
			}
			private void unloadGrandChild()
			{
				for(int i = 0; i < this.Items.Count; i++)
				{
					DirTreeViewItem tvi_child = this.Items[i] as DirTreeViewItem;
					if(tvi_child == null)
						continue;

					tvi_child.unloadChild();
				}
			}
		}
		public static Window_Search m_wnd = null;
		DirTreeViewItem root_tree;
		
		public Window_Search()
		{
			m_wnd = this;
			InitializeComponent();

			createDirTree();

			button_save.Click += Button_save_Click;
			button_cancel.Click += Button_cancel_Click;
			listView_dir.SelectionChanged += ListView_dir_SelectionChanged;
			listView_dir.MouseDoubleClick += ListView_dir_MouseDoubleClick;
		}


		void clearListView_dir()
		{
			listView_dir.Items.Clear();
		}

		public void refreshListView_dir(DirTreeViewItem tvi)
		{
			clearListView_dir();
			
			for(int i = 0; i < tvi.Items.Count; i++)
			{
				DirTreeViewItem child_tvi = tvi.Items[i] as DirTreeViewItem;
				if(child_tvi == null)
					continue;

				listView_dir.Items.Add(new MyListItem() { Type = "dir", Name = child_tvi.Header.ToString() });

			}
			string[] files = DirTreeViewItem.loadFile(tvi.path);
			if(files == null)
				return;

			for(int i = 0; i < files.Length; i++)
			{
				string[] split = files[i].Split('\\');
				listView_dir.Items.Add(new MyListItem() { Type = "file", Name = split[split.Length - 1]});
			}
		}

		private void ListView_dir_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ListView lv = sender as ListView;
			if(lv == null)
				return;

			if(lv.SelectedIndex < 0)
				return;

			MyListItem item = lv.Items[lv.SelectedIndex] as MyListItem;
			if(item == null)
				return;

			if(item.Type == "dir")
				return;
			textBox_name.Text = item.Name;
		}
		private void ListView_dir_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			ListView lv = sender as ListView;
			if(lv == null)
				return;

			if(lv.SelectedIndex < 0)
				return;

			MyListItem item = lv.Items[lv.SelectedIndex] as MyListItem;
			if(item == null)
				return;

			if(item.Type == "dir")
			{
				;
			}
			//textBox_name.Text = item.Name;
		}

		public void setTextBox_name(string str)
		{
			textBox_name.Text = str;
		}

		private void Button_save_Click(object sender, RoutedEventArgs e)
		{
			path_return = DirTreeViewItem.last_path + '\\' + textBox_name.Text;
			FileInfo f = new FileInfo(path_return);
			if(f.Exists)
			{
				MessageBoxResult mr = MessageBox.Show("파일이 이미 존재합니다\n 바꾸시겠습니까?", "파일 중복", MessageBoxButton.YesNoCancel);
				if(mr != MessageBoxResult.Yes)
					path_return = null;
			}
			this.Close();
		}

		private void Button_cancel_Click(object sender, RoutedEventArgs e)
		{
			path_return = null;
			this.Close();
		}
		
		void createDirTree()
		{
			string path = Directory.GetDirectoryRoot(AppDomain.CurrentDomain.BaseDirectory);
			//string path = AppDomain.CurrentDomain.BaseDirectory;
			root_tree = new DirTreeViewItem(path);
			treeView_dir.Items.Add(root_tree);
			curPathOpen();
		}
		void curPathOpen()
		{
			string path = AppDomain.CurrentDomain.BaseDirectory;

			string[] split = path.Split('\\');

			DirTreeViewItem cur_tvi = root_tree;
			DirTreeViewItem last_tvi = cur_tvi;
			for(int i = 1; i < split.Length; i++)
			{
				if(cur_tvi == null)
					break;

				for(int j = 0; j < cur_tvi.Items.Count; j++)
				{
					DirTreeViewItem tvi = cur_tvi.Items[j] as DirTreeViewItem;
					if(tvi == null)
						continue;

					if(tvi.Header.ToString() == split[i])
					{
						tvi.IsExpanded = true;
						cur_tvi = tvi;
						last_tvi = cur_tvi;
						break;
					}
				}
			}
			last_tvi.IsSelected = true;
		}
	}
	public class MyListItem
	{
		public string Type { get; set; }
		public string Name { get; set; }
	}
}
