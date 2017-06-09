using MahApps.Metro.IconPacks;
using Manager_proj_4.Classes;
using Manager_proj_4.UserControls;
using Manager_proj_4_net4;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace Manager_proj_4.UserControls
{
	/// <summary>
	/// LinuxTreeViewItem	-> Header -> (file or directory)Name TextBox
	///						-> Items -> LinuxTreeViewItem
	/// </summary>

	public class LinuxDirectoryTree
	{
		public static LinuxDirectoryTree Root;

		public SftpFile fileinfo;
		List<LinuxDirectoryTree> Childs = new List<LinuxDirectoryTree>();

		private bool IsLoaded = false;

		public LinuxDirectoryTree(SftpFile _fileinfo)
		{
			fileinfo = _fileinfo;
		}

		private bool LoadChild()
		{
			if(!IsLoaded)
				return true;

			SftpFile[] files;
			// path 가 null 이라면 부모
			files = SSHController.PollListInDirectory(fileinfo.FullName);
			if(files == null)
				return false;

			this.Childs.Clear();
			foreach(var file in files)
				this.Childs.Add(new LinuxDirectoryTree(file));

			IsLoaded = true;
			return true;
		}
		public LinuxDirectoryTree[] GetChild()
		{
			LoadChild();
			return Childs.ToArray();
		}
		public bool ViewUpdate()
		{
			LoadChild();

			return true;
		}
	}
	public class LinuxTreeViewItem : TreeViewItem
	{
		static class _Color
		{
			#region Folder Color (directory)
			public static SolidColorBrush Folder_foreground = Brushes.DarkBlue;
			#endregion

			#region File Color
			public static SolidColorBrush File_foreground_selected = Brushes.White;
			public static SolidColorBrush File_foreground_unselected = Brushes.Black;
			#endregion

			#region Common Color
			public static SolidColorBrush Background_selected { get { return (SolidColorBrush)App.Current.Resources["AccentColorBrush"]; } }
			public static SolidColorBrush Background_unselected = null;
			#endregion
		}

		static string[] IGNORE_FILENAME = new string[] {".", ".."};
		public static LinuxTreeViewItem root;

		private string path;
		public string Path
		{
			get { return path; }
			set
			{
				path = value;
				if(this == LinuxTreeViewItem.root)
					this.Header.Text = value;
			}
		}
		private bool isDirectory = false;
		public bool IsDirectory
		{
			get { return isDirectory; }
			set
			{
				isDirectory = value;
				if(value)
				{
					TextBlock tb = this.Header.Children[0] as TextBlock;
					if(tb == null)
						return;

					tb.FontWeight = FontWeights.Bold;
					tb.Foreground = _Color.Folder_foreground;
				}
				else
				{
				}
			}
		}

		#region header
		public class Grid_Header : Grid
		{
			const int HEIGHT = 30;
			public string Text
			{
				get
				{
					TextBlock tb = this.Children[0] as TextBlock;
					if(tb == null)
						return null;
					return tb.Text;
					//tb.Text = newText;
				}
				set
				{
					TextBlock tb = this.Children[0] as TextBlock;
					if(tb == null)
						return;
					tb.Text = value;
				}
			}

			public Grid_Header(string header)
			{
				//this.Height = HEIGHT;

				TextBlock tb = new TextBlock();
				tb.Text = header;
				tb.VerticalAlignment = VerticalAlignment.Center;
				this.Children.Add(tb);
			}
		}
		// Casting ( Object To Grid_Header )
		public new Grid_Header Header { get { return base.Header as Grid_Header; } set { base.Header = value; } }
		#endregion

		public LinuxTreeViewItem(string _path, string header = null, bool _isDirectory = false)
		{
			if(header == null && _path != null)
			{
				string[] splited = _path.Split('/');
				header = splited[splited.Length - 1];
			}

			this.Header = new Grid_Header(header);
			this.Cursor = Cursors.Hand;
			this.Path = _path;

			InitContextMenu();

			this.IsDirectory = _isDirectory;

			if(this.IsDirectory)
			{
				// 임시
				Label dummy = new Label();
				this.Items.Add(dummy);
			}

			if(this.Header.Text.Length > 0 && this.Header.Text[0] == '.')
			{
				this.Header.Opacity = .5;
			}
		}

		#region Context Menu
		private void InitContextMenu()
		{
			this.ContextMenu = new ContextMenu();
			MenuItem item = new MenuItem();
			item.Header = "암호화";
			item.Icon = new PackIconFontAwesome()
			{
				Kind = PackIconFontAwesomeKind.Lock,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center
			};
			item.Click += OnClickEncrypt;
			this.ContextMenu.Items.Add(item);
			item = new MenuItem();
			item.Icon = new PackIconFontAwesome()
			{
				Kind = PackIconFontAwesomeKind.Unlock,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center
			};
			item.Header = "복호화";
			item.Click += OnClickDecrypt;
			this.ContextMenu.Items.Add(item);
		}
		private void OnClickEncrypt(object sender, RoutedEventArgs e)
		{
			WindowMain.current.ShowMessageDialog("Encrypt", "[Type = " + Cofile.current.comboBox_cofile_type.SelectedItem + "]\n선택된 파일들을 암호화 하시겠습니까?", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative, Encrypt);
		}
		private void Encrypt()
		{
			TextRange txt = new TextRange(Status.current.richTextBox_status.Document.ContentStart, Status.current.richTextBox_status.Document.ContentEnd);
			txt.Text = "";
			SSHController.view_message_caption = "Encrypt";
			SSHController.SendNRecvCofileCommand(selected_list.ToArray(), true);
			//LinuxTreeViewItem.Refresh();
		}
		private void OnClickDecrypt(object sender, RoutedEventArgs e)
		{
			WindowMain.current.ShowMessageDialog("Decrypt", "[Type = " + Cofile.current.comboBox_cofile_type.SelectedItem + "]\n선택된 파일들을 복호화 하시겠습니까?", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative, Decrypt);
		}
		private void Decrypt()
		{
			TextRange txt = new TextRange(Status.current.richTextBox_status.Document.ContentStart, Status.current.richTextBox_status.Document.ContentEnd);
			txt.Text = "";
			SSHController.view_message_caption = "Decrypt";
			SSHController.SendNRecvCofileCommand(selected_list.ToArray(), false);
			//LinuxTreeViewItem.Refresh();
		}
		#endregion

		#region Multi Select From Mouse Handle
		private static List<LinuxTreeViewItem> selected_list = new List<LinuxTreeViewItem>();
		private bool mySelected = false;
		public bool MySelected
		{
			get { return mySelected; }
			set
			{
				mySelected = value;

				// 색 변경
				if(value)
				{
					selected_list.Add(this);
					this.Background = _Color.Background_selected;
					if(!this.IsDirectory)
					{
						this.Foreground = _Color.File_foreground_selected;
					}
				}
				else
				{
					selected_list.Remove(this);
					this.Background = _Color.Background_unselected;
					if(!this.IsDirectory)
					{
						this.Foreground = _Color.File_foreground_unselected;
					}
				}
			}
		}
		private void MyFocuse()
		{
			// 다른 선택 해제
			while(selected_list.Count > 0)
			{
				selected_list[0].MySelected = false;
			}
			MySelected = true;
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if(WindowMain.bCtrl)
				MySelected = !MySelected;
			else if(WindowMain.bShift && selected_list.Count > 0)
			{
				LinuxTreeViewItem parent = this.Parent as LinuxTreeViewItem;
				if(parent != null)
				{
					int idx_start = parent.Items.IndexOf(selected_list[0]);
					int idx_end = parent.Items.IndexOf(this);

					// 선택 초기화
					while(selected_list.Count > 0)
						selected_list[0].MySelected = false;

					// 선택
					int add_i = idx_start < idx_end ? 1 : -1;
					for(int i = idx_start; i != idx_end + add_i; i += add_i)
					{
						LinuxTreeViewItem child = parent.Items[i] as LinuxTreeViewItem;
						if(child == null)
							continue;

						child.MySelected = true;
					}
				}
				else
					MyFocuse();
			}
			else
				MyFocuse();

			e.Handled = true;
		}
		protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
		{
			if(selected_list.IndexOf(this) < 0)
				this.MyFocuse();
			base.OnMouseRightButtonDown(e);
			e.Handled = true;
		}
		#endregion

		#region Load Directory On Expanded
		protected override void OnExpanded(RoutedEventArgs e)
		{
			RefreshChild();
		}
		public void RefreshChild()
		{
			if(IsDirectory)
			{
				loadDirectory();
				Filter_string = filter_string;
			}
		}
		public void RefreshChildFromParent()
		{
			LinuxTreeViewItem par = this.Parent as LinuxTreeViewItem;
			if(par == null)
				return;

			par.RefreshChild();
		}
		void loadDirectory()
		{
			SftpFile[] files;
			// path 가 null 이라면 부모
			files = SSHController.PollListInDirectory(this.path);
			if(files == null)
			{
				this.IsExpanded = false;
				return;
			}

			this.Items.Clear();

			Cofile.current.RefreshListView(files);

			int count_have_directory = 0;
			foreach(var file in files)
			{
				int i;
				for(i = 0; i < IGNORE_FILENAME.Length; i++)
				{
					if(file.Name == IGNORE_FILENAME[i])
						break;
				}
				if(i != IGNORE_FILENAME.Length)
					continue;

				LinuxTreeViewItem ltvi;
				if(file.IsDirectory)
				{
					//this.Items.Insert(0, new LinuxTreeViewItem(file.FullName, file.Name, true));
					//this.Items.Add(new LinuxTreeViewItem(file.FullName, file.Name, true));
					ltvi = new LinuxTreeViewItem(file.FullName, file.Name, true);
					this.Items.Insert(count_have_directory++, ltvi);
				}
				else
				{
					//this.Items.Insert(0, new LinuxTreeViewItem(file.FullName, file.Name, false));
					ltvi = new LinuxTreeViewItem(file.FullName, file.Name, false);
					this.Items.Add(ltvi);
					//if(file.Name.Substring(file.Name.Length - test_filter.Length, test_filter.Length) != test_filter)
					//	ltvi.Visibility = Visibility.Collapsed;
				}


			}
		}
		//void loadDirectory()
		//{
		//	SftpFile[] files;
		//	// path 가 null 이라면 부모
		//	files = SSHController.PollListInDirectory(this.path);
		//	if(files == null)
		//	{
		//		this.IsExpanded = false;
		//		return;
		//	}

		//	this.Items.Clear();

		//	Cofile.current.RefreshListView(files);

		//	int count_have_directory = 0;
		//	foreach(var file in files)
		//	{
		//		int i;
		//		for(i = 0; i < IGNORE_FILENAME.Length; i++)
		//		{
		//			if(file.Name == IGNORE_FILENAME[i])
		//				break;
		//		}
		//		if(i != IGNORE_FILENAME.Length)
		//			continue;

		//		LinuxTreeViewItem ltvi;
		//		if(file.IsDirectory)
		//		{
		//			//this.Items.Insert(0, new LinuxTreeViewItem(file.FullName, file.Name, true));
		//			//this.Items.Add(new LinuxTreeViewItem(file.FullName, file.Name, true));
		//			ltvi = new LinuxTreeViewItem(file.FullName, file.Name, true);
		//			this.Items.Insert(count_have_directory++, ltvi);
		//		}
		//		else
		//		{
		//			//this.Items.Insert(0, new LinuxTreeViewItem(file.FullName, file.Name, false));
		//			ltvi = new LinuxTreeViewItem(file.FullName, file.Name, false);
		//			this.Items.Add(ltvi);
		//			//if(file.Name.Substring(file.Name.Length - test_filter.Length, test_filter.Length) != test_filter)
		//			//	ltvi.Visibility = Visibility.Collapsed;
		//		}


		//	}
		//}
		#endregion

		#region Visible Filter Via Regular Expresion
		static string filter_except_hidden = @"(^[^\.])";
		static bool bool_except_hidden = true;
		public static bool Bool_except_hidden
		{
			get { return bool_except_hidden; }
			set
			{
				bool_except_hidden = value;

				Filter_string = filter_string;
			}
		}

		static string filter_string = "";
		public static string Filter_string
		{
			get { return filter_string; }
			set
			{
				filter_string = value;
				if(bool_except_hidden)
				{
					if(filter_string == "")
						LinuxTreeViewItem.filter(LinuxTreeViewItem.root, filter_except_hidden);
					else
						LinuxTreeViewItem.filter(LinuxTreeViewItem.root, filter_except_hidden + ".*" + filter_string);
				}
				else
				{
					LinuxTreeViewItem.filter(LinuxTreeViewItem.root, filter_string);
				}
			}
		}
		static void filter(LinuxTreeViewItem parent, string filter_string)
		{
			if(parent == null)
				return;
			try
			{
				Regex r = new Regex(filter_string);
				filter_recursive(parent, r);
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}
		static void filter_recursive(LinuxTreeViewItem cur, Regex filter_string)
		{
			for(int i = 0; i < cur.Items.Count; i++)
			{
				LinuxTreeViewItem child = cur.Items[i] as LinuxTreeViewItem;
				if(child == null)
					continue;

				string name = child.Header.Text;
				if(!filter_string.IsMatch(name))
					child.Visibility = Visibility.Collapsed;
				else
				{
					child.Visibility = Visibility.Visible;
					LinuxTreeViewItem parent = child.Parent as LinuxTreeViewItem;
					while(parent != null)
					{
						parent.Visibility = Visibility.Visible;
						parent = parent.Parent as LinuxTreeViewItem;
					}
				}

				if(child.IsDirectory)
					filter_recursive(child, filter_string);
			}
		}
		#endregion
	}
}
