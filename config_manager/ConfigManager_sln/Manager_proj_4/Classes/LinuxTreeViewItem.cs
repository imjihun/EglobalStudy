using MahApps.Metro.IconPacks;
using Manager_proj_4.Classes;
using Manager_proj_4.UserControls;
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

namespace Manager_proj_4
{
	public class ConvertorCofileOptionToComboBoxItem : MarkupExtension
	{

		private readonly Type _type;

		public ConvertorCofileOptionToComboBoxItem(Type type)
		{
			_type = type;
		}
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return Enum.GetValues(_type)
				.Cast<object>()
				.Select(e => new { Value = (int)e, DisplayName = e.ToString() });
		}
	}
	public enum CofileOption
	{
		file = 0,
		sam,
		tail
	}

	public class LinuxTreeViewItem : TreeViewItem
	{
		static string[] IGNORE_FILENAME = new string[] {".", ".."};
		public static LinuxTreeViewItem root;
		//public static SshClient ssh;
		//public static SftpClient sftp;
		//public static ShellStream shell_stream;
		//public static DispatcherTimer shell_stream_read_timer;

		#region header
		public class Grid_Header : Grid
		{
			const int HEIGHT = 30;
			public Grid_Header(string header)
			{
				//this.Height = HEIGHT;

				TextBlock tb = new TextBlock();
				tb.Text = header;
				tb.VerticalAlignment = VerticalAlignment.Center;
				this.Children.Add(tb);
			}
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
			public void SetText(string newText)
			{
				TextBlock tb = this.Children[0] as TextBlock;
				if(tb == null)
					return;
				tb.Text = newText;
			}
			public void SetBold()
			{
				TextBlock tb = this.Children[0] as TextBlock;
				if(tb == null)
					return;

				tb.FontWeight = FontWeights.Bold;
			}
		}
		public new Grid_Header Header { get { return base.Header as Grid_Header; } set { base.Header = value; } }
		#endregion

		#region variable
		private string path;
		public string Path { get { return path; }
			set {
				path = value;
				if(this == LinuxTreeViewItem.root)
					this.Header.SetText(value);
			}
		}
		public bool isDirectory = false;
		#endregion

		private void InitContextMenu()
		{
			this.ContextMenu = new ContextMenu();
			MenuItem item = new MenuItem();
			item.Header = "암호화";
			item.Icon = new PackIconMaterial()
			{
				Kind = PackIconMaterialKind.KeyPlus,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center
			};
			item.Click += OnClickEncrypt;
			this.ContextMenu.Items.Add(item);
			item = new MenuItem();
			item.Icon = new PackIconMaterial()
			{
				Kind = PackIconMaterialKind.KeyMinus,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center
			};
			item.Header = "복호화";
			item.Click += OnClickDecrypt;
			this.ContextMenu.Items.Add(item);
		}
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

			this.isDirectory = _isDirectory;

			if(this.isDirectory)
			{
				this.Foreground = Brushes.DarkBlue;
				this.Header.SetBold();

				// 임시
				Label dummy = new Label();
				this.Items.Add(dummy);
			}

			if(this.Header.Text.Length > 0 && this.Header.Text[0] == '.')
			{
				this.Header.Opacity = .5;
			}

		}
		private void OnClickEncrypt(object sender, RoutedEventArgs e)
		{
			WindowMain.current.ShowMessageDialog("Encrypt", "[Type = " + CofileOption.file + "] 선택된 파일들을 암호화 하시겠습니까?", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative, Encrypt);
			//if(MessageBox.Show("Encrypt?", "Encrypt", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
			//{
			//	//Log.ViewMessage("Encrypting..", "Encrypt", test4.m_wnd.richTextBox_status);
			//	TextRange txt = new TextRange(WindowMain.current.richTextBox_status.Document.ContentStart, WindowMain.current.richTextBox_status.Document.ContentEnd);
			//	txt.Text = "";
			//	view_message_caption = "Encrypt";
			//	sendCofileCommand(true);
			//}
		}
		private void Encrypt()
		{
			TextRange txt = new TextRange(WindowMain.current.richTextBox_status.Document.ContentStart, WindowMain.current.richTextBox_status.Document.ContentEnd);
			txt.Text = "";
			SSHController.view_message_caption = "Encrypt";
			SSHController.sendCofileCommand(selected_list.ToArray(), true);
			//LinuxTreeViewItem.Refresh();
		}
		private void OnClickDecrypt(object sender, RoutedEventArgs e)
		{
			WindowMain.current.ShowMessageDialog("Decrypt", "[Type = " + CofileOption.file + "] 선택된 파일들을 복호화 하시겠습니까?", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative, Decrypt);
			//if(MessageBox.Show("Decrypt?", "Decrypt", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
			//{
			//	//Log.ViewMessage("Decrypting..", "Decrypt", test4.m_wnd.richTextBox_status);
			//	TextRange txt = new TextRange(WindowMain.current.richTextBox_status.Document.ContentStart, WindowMain.current.richTextBox_status.Document.ContentEnd);
			//	txt.Text = "";
			//	view_message_caption = "Decrypt";
			//	sendCofileCommand(false);
			//}
		}
		public void Decrypt()
		{
			TextRange txt = new TextRange(WindowMain.current.richTextBox_status.Document.ContentStart, WindowMain.current.richTextBox_status.Document.ContentEnd);
			txt.Text = "";
			SSHController.view_message_caption = "Decrypt";
			SSHController.sendCofileCommand(selected_list.ToArray(), false);
			//LinuxTreeViewItem.Refresh();
		}

		#region mouse handle for multi select
		static List<LinuxTreeViewItem> selected_list = new List<LinuxTreeViewItem>();
		bool mySelected = false;

		static SolidColorBrush Background_selected { get { return (SolidColorBrush)App.Current.Resources["AccentColorBrush"]; } }
		static SolidColorBrush Foreground_selected = Brushes.White;
		static SolidColorBrush Foreground_unselected = Brushes.Black;
		bool MySelected
		{
			get { return mySelected; }
			set
			{
				mySelected = value;
				if(value)
				{
					selected_list.Add(this);
					this.Background = LinuxTreeViewItem.Background_selected;
					if(!this.isDirectory)
					{
						this.Foreground = Foreground_selected;
					}
				}
				else
				{
					selected_list.Remove(this);
					this.Background = Brushes.White;
					if(!this.isDirectory)
					{
						this.Foreground = Foreground_unselected;
					}
				}
			}
		}
		private void MyFocuse()
		{
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

		#region linux directory
		protected override void OnExpanded(RoutedEventArgs e)
		{
			if(isDirectory)
			{
				loadDirectory();
				Filter_string = filter_string;
			}
		}
		void loadDirectory()
		{
			SftpFile[] files;
			files = SSHController.PollListInDirectory(this.path);

			if(files == null)
			{
				this.IsExpanded = false;
				return;
			}

			this.Items.Clear();

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
		#endregion

		public static void Refresh()
		{
			if(WindowMain.current == null)
				return;

			//if(ssh != null && ssh.IsConnected)
			//	ssh.Disconnect();
			//if(sftp != null && sftp.IsConnected)
			//	sftp.Disconnect();

			// 삭제
			Cofile.current.treeView_linux_directory.Items.Clear();

			// 추가
			//string home_dir = sftp.WorkingDirectory;
			LinuxTreeViewItem.root = new LinuxTreeViewItem(null, null, true);
			Cofile.current.treeView_linux_directory.Items.Add(LinuxTreeViewItem.root);
			Log.Print("[refresh]");

			//LinuxTreeViewItem.ReconnectServer();
		}
		static string filter_hidden = @"(^[^\.])";
		static bool bool_hidden = true;
		public static bool Bool_hidden { get { return bool_hidden; } set
			{
				bool_hidden = value;
				
				Filter_string = filter_string;
			}
		}

		static string filter_string = "";
		public static string Filter_string { get { return filter_string; } set
			{
				filter_string = value;
				if(bool_hidden)
				{
					if(filter_string == "")
						LinuxTreeViewItem.filter(LinuxTreeViewItem.root, filter_hidden);
					else
						LinuxTreeViewItem.filter(LinuxTreeViewItem.root, filter_hidden + "*(" + filter_string + ")");
				}
				else
					LinuxTreeViewItem.filter(LinuxTreeViewItem.root, filter_string);
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
				if(/*(!child.isDirectory || (child.isDirectory && !child.IsExpanded)) && */!filter_string.IsMatch(name))
					child.Visibility = Visibility.Collapsed;
				else
					child.Visibility = Visibility.Visible;

				if(child.isDirectory)
					filter_recursive(child, filter_string);
			}
		}
	}
}
