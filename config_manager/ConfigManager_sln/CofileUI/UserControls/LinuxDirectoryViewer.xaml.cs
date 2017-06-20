using CofileUI.Classes;
using CofileUI.Windows;
using Newtonsoft.Json.Linq;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using static CofileUI.UserControls.Cofile;

namespace CofileUI.UserControls
{
	/// <summary>
	/// LinuxDirectoryViewer.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class LinuxDirectoryViewer : Window
	{
		public static LinuxDirectoryViewer current;
		public delegate void _MouseDoubleClick();
		public static _MouseDoubleClick FileDoubleClick = null;
		public LinuxDirectoryViewer(SftpFile[] files, _MouseDoubleClick _FileDoubleClick = null)
		{
			current = this;
			InitializeComponent();
			FileDoubleClick = _FileDoubleClick;

			treeView_linux_directory.ItemsSource = files;
		}

		private void OnButtonClickRefresh(object sender, RoutedEventArgs e)
		{
			LinuxTreeViewItem.root.RefreshChild(LinuxTreeViewItem.Last_Refresh.Path, false);
			RefreshListView(LinuxTreeViewItem.Last_Refresh);
		}
		private void OnCheckBoxCheckedFileDetail(object sender, RoutedEventArgs e)
		{
			if(sender == checkBox_linux_detail)
				listView_linux_files.View = Resources["GridView_ListViewLinux_Detail"] as GridView;
		}
		private void OnCheckBoxUncheckedFileDetail(object sender, RoutedEventArgs e)
		{
			if(sender == checkBox_linux_detail)
				listView_linux_files.View = null;
		}
		private void OnMouseDoubleClickLinuxListView(object sender, MouseButtonEventArgs e)
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
					FileDoubleClick?.Invoke();
				}
			}
		}
		private void OnMouseMoveLinuxListView(object sender, MouseEventArgs e)
		{
			if(e.LeftButton == MouseButtonState.Pressed
				&& listView_linux_files.SelectedItems.Count > 0)
			{
				DataObject data = new DataObject();
				data.SetData("Object", listView_linux_files.SelectedItems);
				DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
			}
		}

		static List<LinuxListViewItem> list_LinuxListViewItem = new List<LinuxListViewItem>();
		public static LinuxTreeViewItem cur_LinuxTreeViewItem = null;
		public void RefreshListView(LinuxTreeViewItem cur)
		{
			if(cur == null)
				return;

			cur_LinuxTreeViewItem = cur;
			label_listView_linux.Content = cur.Path;

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
	public class BoolToColorConverter : IValueConverter
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
