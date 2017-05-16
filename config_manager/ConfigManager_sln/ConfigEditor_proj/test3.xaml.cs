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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ConfigEditor_proj
{
	/// <summary>
	/// test3.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class test3 : Window
	{
		class JSonFile
		{
			public string path;
			public string filename;
			public JToken jroot;
			public void Clear()
			{
				path = null;
				filename = null;
				jroot = null;
			}
		}
		string root_path = AppDomain.CurrentDomain.BaseDirectory;
		public static test3 m_wnd = null;
		JSonFile cur_jsonfile = new JSonFile();
		public test3()
		{
			InitializeComponent();

			refreshJsonFile();

			btn_view_filew.Click += Btn_view_filew_Click;
			treeView_jsonfile.SelectedItemChanged += TreeView_jsonfile_SelectedItemChanged;

			tb_root_path.Text = root_path;
			m_wnd = this;

			this.Closed += Test3_Closed;
			DispatcherTimer Timer = new DispatcherTimer();
			Timer.Interval = TimeSpan.FromSeconds(3);
			Timer.Tick += Timer_Tick;
			Timer.Start();
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			cur_jsonfile.jroot["type"] += "a";
			Console.WriteLine(cur_jsonfile.jroot["type"]);
		}

		private void Test3_Closed(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
		}


		private void Btn_view_filew_Click(object sender, RoutedEventArgs e)
		{
			new Window_ViewFile(FileContoller.read(cur_jsonfile.path), cur_jsonfile.path).Show();
		}
		#region json file
		private void TreeView_jsonfile_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			TreeViewItem selected_tvi = treeView_jsonfile.SelectedItem as TreeViewItem;
			if(selected_tvi == null)
			{
				return;
			}
			PathTextBlock  selected = selected_tvi.Header as PathTextBlock ;
			if(selected == null)
			{
				return;
			}

			cur_jsonfile.filename = selected.Text as string;
			cur_jsonfile.path = selected.path;
			refreshJsonItem();
			refreshCommonOption();
		}
		class PathTextBlock : TextBlock
		{
			public string path;
			public PathTextBlock(string _path)
			{
				path = _path;
			}
		}
		public void refreshJsonFile()
		{
			// 삭제
			treeView_jsonfile.Items.Clear();
			cur_jsonfile.Clear();

			// 추가
			//string[] files = Directory.GetFiles(@"D:\git\config_manager\ConfigManager_sln\ConfigEditor_proj\bin\Debug", "*.json");
			// 현재 application이 실행되는 경로의 json 파일을 찾아라
			string[] files = Directory.GetFiles(root_path, "*.json");
			string[] filepath_splited = root_path.Split('\\');

			//ItemsControl parent = treeView_jsonfile;
			//TreeViewItem child;

			//child = new TreeViewItem();
			//child.Header = "File Path";
			//child.IsExpanded = true;
			//parent.Items.Add(child);
			//parent = child;
			//for(int i = 0; i < filepath_splited.Length; i++)
			//{
			//	child = new TreeViewItem();
			//	child.Header = filepath_splited[i];
			//	child.IsExpanded = true;
			//	parent.Items.Add(child);

			//	parent = child;
			//}

			TreeViewItem tvi = new TreeViewItem();
			tvi.Header = filepath_splited[filepath_splited.Length - 2];
			tvi.IsExpanded = true;
			treeView_jsonfile.Items.Add(tvi);

			for(int i = 0; i < files.Length; i++)
			{

				string[] filename_splited = files[i].Split('\\');
				PathTextBlock  tblock = new PathTextBlock (root_path + filename_splited[filename_splited.Length - 1]);
				tblock.Text = filename_splited[filename_splited.Length - 1];
				
				TreeViewItem child = new TreeViewItem();
				child.Header = tblock;

				tvi.Items.Add(child);
			}
			cur_jsonfile.filename = ((tvi.Items.GetItemAt(0) as TreeViewItem).Header as PathTextBlock).Text as string;

			cur_jsonfile.path = ((tvi.Items.GetItemAt(0) as TreeViewItem).Header as PathTextBlock).path as string;
			refreshJsonItem();
			refreshCommonOption();

			(tvi.Items.GetItemAt(0) as TreeViewItem).IsSelected = true;
			//(tvi.Items.GetItemAt(0) as TreeViewItem).Focus();
		}

		#endregion

		#region json tree
		public void refreshJsonItem()
		{
			string json = FileContoller.read(cur_jsonfile.path);
			// 삭제
			treeView_json.Items.Clear();

			// 추가
			List<string> key_stack = new List<string>();
			TreeViewItem trvItem = new TreeViewItem();
			trvItem.Header = cur_jsonfile.filename;
			trvItem.IsExpanded = true;
			//trvItem.Tag = "0000";
			addButtonAddjson(trvItem, key_stack);

			treeView_json.Items.Add(trvItem);

			if(json != null)
			{
				cur_jsonfile.jroot = JsonController.parseJson(json);
				if(cur_jsonfile.jroot == null)
					MessageBox.Show(JsonController.Error_message,"JSon Context Error");
				else
					addJsonItem_recursive(trvItem, cur_jsonfile.jroot, key_stack);
			}
		}
		void addJsonItem_recursive(TreeViewItem cur_tvi, JToken jtok, List<string> key_stack)
		{
			if(jtok == null)
				return;

			TreeViewItem child_tvi = cur_tvi;
			if(jtok as JObject != null)
			{
				if(jtok.Parent as JArray != null)
				{
					key_stack.Add((jtok.Parent as JArray).IndexOf(jtok).ToString());
					child_tvi = addToJsonTree(cur_tvi, (JObject)jtok, key_stack);
				}
			}
			else if(jtok as JArray != null)
			{
				//// 실제로 실행이 안되는 코드 
				//if(v.Parent as JProperty == null)
				//{
				//	child_tvi = addToJsonTree(cur_tvi, (JArray)v, key_stack);
				//}
			}
			else if(jtok as JProperty != null)
			{
				key_stack.Add((jtok as JProperty).Name);
				child_tvi = addToJsonTree(cur_tvi, jtok as JProperty, key_stack);
			}
			else if(jtok as JValue != null)
			{
				child_tvi = addToJsonTree(cur_tvi, (JValue)jtok);
			}
			else
			{
				Console.WriteLine(jtok.GetType());
			}

			foreach(var v in jtok.Children())
			{
				addJsonItem_recursive(child_tvi, v, key_stack);
			}
			// 재귀가 끝나는 지점에 jobject가 끝나므로 스택에서 키를 빼준다.
			if(key_stack.Count > 0
				&& ((jtok is JObject && jtok.Parent is JArray)
					|| (jtok is JProperty)))
				key_stack.RemoveAt(key_stack.Count - 1);
		}
		//void addJsonItem_recursive(TreeViewItem cur_tvi, JToken jtok, List<string> key_stack)
		//{
		//	if(jtok == null)
		//		return;

		//	foreach(var v in jtok.Children())
		//	{
		//		TreeViewItem child_tvi = cur_tvi;
		//		if(v as JObject != null)
		//		{
		//			if(v.Parent as JArray != null)
		//			{
		//				key_stack.Add((v.Parent as JArray).IndexOf(v).ToString());
		//				child_tvi = addToJsonTree(cur_tvi, (JObject)v, key_stack);
		//			}
		//		}
		//		else if(v as JArray != null)
		//		{
		//			//// 실제로 실행이 안되는 코드 
		//			//if(v.Parent as JProperty == null)
		//			//{
		//			//	child_tvi = addToJsonTree(cur_tvi, (JArray)v, key_stack);
		//			//}
		//		}
		//		else if(v as JProperty != null)
		//		{
		//			key_stack.Add((v as JProperty).Name);
		//			child_tvi = addToJsonTree(cur_tvi, v as JProperty, key_stack);
		//		}
		//		else if(v as JValue != null)
		//		{
		//			child_tvi = addToJsonTree(cur_tvi, (JValue)v, key_stack);
		//			continue;
		//		}
		//		else
		//		{
		//			Console.WriteLine(v.GetType());
		//			continue;
		//		}

		//		addJsonItem_recursive(child_tvi, v, key_stack);
		//	}
		//	// 재귀가 끝나는 지점에 jobject가 끝나므로 스택에서 키를 빼준다.
		//	if(key_stack.Count > 0)
		//		key_stack.RemoveAt(key_stack.Count - 1);
		//}
		TreeViewItem addToJsonTree(TreeViewItem parent_tvi, JObject jobj, List<string> key_stack)
		{
			if(jobj.Parent as JProperty != null)
				return parent_tvi;

			TreeViewItem child_tvi = new TreeViewItem();
			addButtonAddjson(child_tvi, key_stack);
			if(parent_tvi.Items.Count > 0)
				parent_tvi.Items.Insert(parent_tvi.Items.Count - 1, child_tvi);
			else
				parent_tvi.Items.Add(child_tvi);

			StackPanel sp = new StackPanel();
			sp.Orientation = Orientation.Horizontal;
			child_tvi.Header = sp;

			//addTextBlock(sp, (jobj.Parent as JArray).IndexOf(jobj).ToString(), key_stack);
			addTextBlock_index(sp, (jobj.Parent as JArray).IndexOf(jobj));

			addButtonDeletejson(sp, key_stack);

			return child_tvi;
			//return parent_tvi;
		}
		TreeViewItem addToJsonTree(TreeViewItem parent_tvi, JArray jarr, List<string> key_stack)
		{
			if(jarr.Parent as JProperty != null)
				return parent_tvi;

			TreeViewItem child_tvi = new TreeViewItem();
			//addButtonAddjson(child_tvi, key_stack);
			//if(parent_tvi.Items.Count > 0)
			//	parent_tvi.Items.Insert(parent_tvi.Items.Count - 1, child_tvi);
			//else
			//	parent_tvi.Items.Add(child_tvi);

			//StackPanel sp = new StackPanel();
			//sp.Orientation = Orientation.Horizontal;
			//child_tvi.Header = sp;

			//addTextBlock(sp, "JArray",key_stack);

			//addButtonDeletejson(sp, key_stack);

			return child_tvi;
		}
		TreeViewItem addToJsonTree(TreeViewItem parent_tvi, JProperty jprop, List<string> key_stack)
		{
			TreeViewItem child_tvi = new TreeViewItem();
			addButtonAddjson(child_tvi, key_stack);
			if(parent_tvi.Items.Count > 0)
				parent_tvi.Items.Insert(parent_tvi.Items.Count - 1, child_tvi);
			else
				parent_tvi.Items.Add(child_tvi);

			StackPanel sp = new StackPanel();
			sp.Orientation = Orientation.Horizontal;
			child_tvi.Header = sp;

			addKeyTextBox(sp, jprop, key_stack);
			addButtonDeletejson(sp, key_stack);


			return child_tvi;
		}
		TreeViewItem addToJsonTree(TreeViewItem parent_tvi, JValue jval)
		{
			addValueTextBox(parent_tvi.Header as Panel, jval);
			parent_tvi.Items.Clear();
			return parent_tvi;
		}
		void addKeyTextBox(Panel pan, JProperty jprop, List<string> key_stack)
		{
			string key = jprop.Name;

			MyTextBox tb_key = new MyTextBox(key_stack);
			tb_key.Text = key;
			tb_key.Margin = new Thickness(5);
			tb_key.Width = 150;
			tb_key.IsEnabled = false;
			tb_key.TextChanged += delegate (object sender, TextChangedEventArgs e)
			{
				MyTextBox tb = sender as MyTextBox;
				if(tb == null)
					return;

				string[] arr_key_stack = tb.key_stack;

				JToken tmp = cur_jsonfile.jroot;
				for(int i = 0; i < arr_key_stack.Length; i++)
				{
					if(tmp is JObject)
						tmp = ((JObject)tmp).GetValue(arr_key_stack[i]);
					else if(tmp is JArray)
						tmp = ((JArray)tmp)[Convert.ToInt16(arr_key_stack[i])];
					else
						break;
				}
				Console.WriteLine("changed : " + e.Changes.Count);
				if(tmp is JObject || tmp is JValue)
				{
					tmp.Parent.Parent[tb_key.Text] = tmp;

					if(arr_key_stack.Length > 0)
						arr_key_stack[arr_key_stack.Length - 1] = tb_key.Text;

					if(tmp != null && tmp.Parent != null)
						tmp.Parent.Remove();
				}
				//(tmp[key] as JProperty).Remove();

				FileContoller.write(cur_jsonfile.filename, cur_jsonfile.jroot.ToString());
			};
			pan.Children.Add(tb_key);
			//tb_key.SetValue(Grid.ColumnProperty, 0);
		}
		void addTextBlock_index(Panel pan, int index)
		{
			TextBlock tb_key = new TextBlock();
			tb_key.Text = index.ToString();
			tb_key.Margin = new Thickness(5);
			tb_key.Width = 150;
			pan.Children.Add(tb_key);
			//tb_key.SetValue(Grid.ColumnProperty, 0);
		}
		void addValueTextBox(Panel pan, JValue jval)
		{
			string data = jval.ToString();

			TextBox tb_value = new TextBox();
			//tb_value.Text = data;
			tb_value.Margin = new Thickness(5);
			tb_value.Width = 150;
			
			Binding myBinding = new Binding("Value");
			myBinding.Source = jval;
			myBinding.Mode = BindingMode.TwoWay;
			myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			tb_value.SetBinding(TextBox.TextProperty, myBinding);

			tb_value.TextChanged += delegate (object sender, TextChangedEventArgs e)
			{
				//((TextBox)sender).GetBindingExpression(TextBox.TextProperty)
				//	 .UpdateTarget();
				Console.WriteLine(jval.Value + " / " + cur_jsonfile.jroot["type"]);
				Console.WriteLine();
				FileContoller.write(cur_jsonfile.filename, cur_jsonfile.jroot.ToString());
				//InvalidateVisual();
				//UpdateLayout();
				//InvalidateArrange();
			};
			if(pan.Children.Count > 1)
				pan.Children.Insert(pan.Children.Count - 1, tb_value);
			else
				pan.Children.Add(tb_value);
		}

		class MyTextBox : TextBox
		{
			public string[] key_stack;
			public MyTextBox(List<string> _key_stack)
			{
				key_stack = new string[_key_stack.Count];
				_key_stack.CopyTo(key_stack);
			}
		}
		class MyButton : Button
		{
			public string[] key_stack;
			public MyButton(List<string> _key_stack)
			{
				key_stack = new string[_key_stack.Count];
				_key_stack.CopyTo(key_stack);
			}
		}
		void addButtonAddjson(TreeViewItem viewitem, List<string> key_stack)
		{
			MyButton btn = new MyButton(key_stack);
			btn.Content = '+';
			btn.Background = Brushes.White;
			btn.BorderBrush = Brushes.Black;
			btn.Width = 20;
			btn.Height = 20;
			btn.VerticalContentAlignment = VerticalAlignment.Center;
			btn.HorizontalContentAlignment = HorizontalAlignment.Center;
			
			btn.Click += delegate (object sender, RoutedEventArgs e)
			{
				MyButton button = sender as MyButton;
				string[] arr_key_stack = button.key_stack;

				func(button, arr_key_stack, viewitem);
			};
			viewitem.Items.Add(btn);
		}
		void func(MyButton sender, string[] arr_key_stack, TreeViewItem tvi)
		{
			JToken tmp = cur_jsonfile.jroot;
			for(int i = 0; i < arr_key_stack.Length; i++)
			{
				if(tmp is JObject)
					tmp = ((JObject)tmp).GetValue(arr_key_stack[i]);
				else if(tmp is JArray)
					tmp = ((JArray)tmp)[Convert.ToInt16(arr_key_stack[i])];
				else
					break;
			}
			JToken add_jtok;
			if(tmp is JArray)
			{
				add_jtok = new JObject();
				(tmp as JArray).Add(add_jtok);
			}
			else
			{
				popup_AddJsonItem popup = new popup_AddJsonItem();
				Point pt = sender.PointToScreen(new Point(0, 0));
				popup.Left = pt.X;
				popup.Top = pt.Y;
				if(popup.ShowDialog() != true)
					;

				if(popup.return_ok == false)
					return;

				add_jtok = new JObject(new JProperty(popup.key, popup.value));
				(tmp as JObject).Add(add_jtok.Children());
			}
			addJsonItem_recursive(tvi, add_jtok, arr_key_stack.ToList());

			FileContoller.write(cur_jsonfile.filename, cur_jsonfile.jroot.ToString());
		}
		void addButtonDeletejson(Panel pan, List<string> key_stack)
		{
			MyButton btn = new MyButton(key_stack);
			btn.Content = '-';
			btn.Background = Brushes.White;
			btn.BorderBrush = Brushes.Black;
			btn.Width = 20;
			btn.Height = 20;
			btn.VerticalContentAlignment = VerticalAlignment.Center;
			btn.HorizontalContentAlignment = HorizontalAlignment.Center;
			
			btn.Click += delegate (object sender, RoutedEventArgs e)
			{
				MyButton button = sender as MyButton;
				string[] arr_key_stack = button.key_stack;

				// remove from treeview
				TreeViewItem parent = pan.Parent as TreeViewItem;
				if(parent != null)
				{
					if(parent.Header == pan)
					{
						TreeViewItem grandparent = parent.Parent as TreeViewItem;
						if(grandparent != null)
							grandparent.Items.Remove(parent);
					}
					else
						parent.Items.Remove(pan);
				}

				// remove from jroot
				JToken tmp = cur_jsonfile.jroot;
				for(int i = 0; i < arr_key_stack.Length; i++)
				{
					if(tmp is JObject)
						tmp = ((JObject)tmp).GetValue(arr_key_stack[i]);
					else if(tmp is JArray)
						tmp = ((JArray)tmp)[Convert.ToInt16(arr_key_stack[i])];
					else
						break;
				}
				if(tmp.Parent is JArray)
				{
					tmp.Remove();
					FileContoller.write(cur_jsonfile.filename, cur_jsonfile.jroot.ToString());
					refreshJsonItem();
					refreshCommonOption();
				}
				else
				{
					tmp.Parent.Remove();
					FileContoller.write(cur_jsonfile.filename, cur_jsonfile.jroot.ToString());
				}
				//FileContoller.write(cur_jsonfile.filename, cur_jsonfile.jroot.ToString());
			};
			pan.Children.Add(btn);
			//btn.SetValue(Grid.ColumnProperty, 2);
		}
		#endregion

		#region common option
		// to have to do after json tree refresh
		void refreshCommonOption()
		{
			// 삭제
			treeView_common_option.Items.Clear();

			// 추가
			List<string> key_stack = new List<string>();
			TreeViewItem trvItem = new TreeViewItem();
			trvItem.Header = cur_jsonfile.filename;
			trvItem.IsExpanded = true;

			treeView_common_option.Items.Add(trvItem);
			
			JToken jroot = cur_jsonfile.jroot;
			if(cur_jsonfile.jroot == null)
				return;

			JToken jroot_local = jroot;
			if(jroot["type"] != null)
			{
				//List<string> key_stack = new List<string>();
				jroot_local = jroot["type"];

				StackPanel sp = new StackPanel();
				sp.Orientation = Orientation.Horizontal;
				addKeyTextBox(sp, jroot_local.Parent as JProperty, key_stack);

				TreeViewItem child_tvi = new TreeViewItem();
				child_tvi.Header = sp;
				child_tvi.IsExpanded = true;
				trvItem.Items.Add(child_tvi);

				addJsonCommonOption(child_tvi, jroot_local);

			}
			if(jroot["comm_option"] != null)
			{
				jroot_local = jroot["comm_option"];

				StackPanel sp = new StackPanel();
				sp.Orientation = Orientation.Horizontal;
				addKeyTextBox(sp, jroot_local.Parent as JProperty, key_stack);

				TreeViewItem child_tvi = new TreeViewItem();
				child_tvi.Header = sp;
				child_tvi.IsExpanded = true;
				trvItem.Items.Add(child_tvi);

				addJsonCommonOption(child_tvi, jroot_local);

			}
			
		}


		void addJsonCommonOption(TreeViewItem tvi, JToken jtok)
		{
			if(jtok == null)
				return;

			TreeViewItem child_tvi = tvi;
			if(jtok as JObject != null)
			{
				if(jtok.Parent as JArray != null)
				{
					JObject jobj = jtok as JObject;
					child_tvi = new TreeViewItem();
					tvi.Items.Add(child_tvi);

					StackPanel sp = new StackPanel();
					sp.Orientation = Orientation.Horizontal;
					child_tvi.Header = sp;

					addTextBlock_index(sp, (jobj.Parent as JArray).IndexOf(jobj));
				}
			}
			else if(jtok as JProperty != null)
			{
				JProperty jprop = jtok as JProperty;
				child_tvi = new TreeViewItem();
				tvi.Items.Add(child_tvi);

				StackPanel sp = new StackPanel();
				sp.Orientation = Orientation.Horizontal;
				child_tvi.Header = sp;

				addKeyTextBox(sp, jprop);
			}
			else if(jtok as JValue != null)
			{
				JValue jval = jtok as JValue;
				addValueTextBox(tvi.Header as Panel, jval);
			}
			else
			{
				Console.WriteLine(jtok.GetType());
			}

			foreach(var v in jtok.Children())
			{
				addJsonCommonOption(child_tvi, v);
			}
		}

		void addKeyTextBox(Panel pan, JProperty jprop)
		{
			string key = jprop.Name;

			TextBox tb_key = new TextBox();
			tb_key.Text = key;
			tb_key.Margin = new Thickness(5);
			tb_key.Width = 150;
			tb_key.IsEnabled = false;
			pan.Children.Add(tb_key);
			//tb_key.SetValue(Grid.ColumnProperty, 0);
		}
		#endregion
	}
}