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
			public JToken jtok_root;
			public MyTreeViewItem jtree_root;
			public void Clear()
			{
				path = null;
				filename = null;
				jtok_root = null;
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
			btn_save_jtree.Click += Btn_save_jtree_Click;
			treeView_jsonfile.SelectedItemChanged += TreeView_jsonfile_SelectedItemChanged;

			tb_root_path.Text = root_path;
			m_wnd = this;

			tabControl_json.SelectionChanged += TabControl_json_SelectionChanged;

			this.Closed += Test3_Closed;
			//DispatcherTimer Timer = new DispatcherTimer();
			//Timer.Interval = TimeSpan.FromSeconds(3);
			//Timer.Tick += Timer_Tick;
			//Timer.Start();
			convertToJToken(cur_jsonfile.jtree_root);
		}

		private void TabControl_json_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			switch(tabControl_json.SelectedIndex)
			{
				case 0:
					refreshJsonItem();
					refreshCommonOption();
					break;
				case 1:
					refreshJsonItem();
					break;
			}
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			cur_jsonfile.jtok_root["type"] += "a";
			Console.WriteLine(cur_jsonfile.jtok_root["type"]);
		}

		private void Test3_Closed(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
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
			string[] files = FileContoller.loadFile(root_path, "*.json");
			string[] filepath_splited = root_path.Split('\\');
			
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
		class MyTreeViewItem : TreeViewItem
		{
			public static MyTreeViewItem selected_tvi = null;

			// JObject, JArray, JProperty
			JToken linked_jtoken = null;
			public MyTreeViewItem(JToken _linked_jtoken)
			{
				linked_jtoken = _linked_jtoken;
			}
			
			protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
			{
				base.OnMouseLeftButtonDown(e);
				selected_tvi = this;
				e.Handled = true;
			}

			protected override void OnMouseMove(MouseEventArgs e)
			{
				//Console.WriteLine(linked_jtoken);
				base.OnMouseMove(e);
				if(e.LeftButton == MouseButtonState.Pressed
					&& MyTreeViewItem.selected_tvi != null && MyTreeViewItem.selected_tvi != this)
				{
					Console.WriteLine(this.Name + ", called DoDragDrop(" + MyTreeViewItem.selected_tvi.Name + ")");
					DataObject data = new DataObject();
					data.SetData("Object", MyTreeViewItem.selected_tvi);
					DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
				}

				e.Handled = true;
			}

			// 옮길 위치(dst)가 선택된(src) 타겟의 자손인지 체크하기위해 tunneling 으로 위부터 접근.
			protected override void OnPreviewMouseMove(MouseEventArgs e)
			{
				//Console.WriteLine(this.Name + ", OnMouseMove()");
				base.OnMouseMove(e);
				
				// 조상부터 자손까지 순서대로 터널링 수행. 선택된 타겟이 자손중 하나라면 이벤트 수행 X
				if(this == MyTreeViewItem.selected_tvi)
					e.Handled = true;
			}

			protected override void OnDragOver(DragEventArgs e)
			{
				this.IsSelected = true;
				e.Handled = true;
			}
			protected override void OnDrop(DragEventArgs e)
			{
				base.OnDrop(e);
				Console.WriteLine(this.Name + ", in OnDrop()");
				// If the DataObject contains string data, extract it.

				if(e.Data.GetDataPresent("Object"))
				{
					Object data_obj = (Object)e.Data.GetData("Object");

					MyTreeViewItem tvi_src = data_obj as MyTreeViewItem;
					if(tvi_src != null)
					{
						// dst 의 조상중에 src 가 있나 체크 & dst == src 인지 체크
						MyTreeViewItem tmp_parent = this;
						while(tmp_parent != null)
						{
							if(tmp_parent == tvi_src)
								return;

							tmp_parent = tmp_parent.Parent as MyTreeViewItem;
						}

						MyTreeViewItem tvi_parent = this.Parent as MyTreeViewItem;
						MyTreeViewItem tvi_src_parent = tvi_src.Parent as MyTreeViewItem;
						if(tvi_parent != null && tvi_src_parent != null)
						{
							tvi_src_parent.Items.Remove(tvi_src);

							if(linked_jtoken is JProperty)
							{
								// mouse pointer 에 따라 요소 위에 insert 할지 아래에 insert 할지 결정.
								// Mouse.GetPosition() 이상한 값 리턴
								int idx_insert = tvi_parent.Items.IndexOf(this);

								Point pointToWindow = e.GetPosition(this);
								double height = 0;
								if(this.Header is Panel)
									height = ((Panel)this.Header).Height;
								if(pointToWindow.Y > height / 2 && idx_insert < tvi_parent.Items.Count)
									idx_insert++;

								// insert
								tvi_parent.Items.Insert(idx_insert, tvi_src);
							}
							else
							{
								if(this.Items.Count > 0)
									this.Items.Insert(this.Items.Count - 1, tvi_src);
								else
									this.Items.Add(tvi_src);
							}

							Console.WriteLine("src = " + tvi_src.Name + ", src_parent = " + tvi_src_parent.Name + ", dst = " + this.Name + ", dst_parent = " + tvi_parent.Name);
							e.Handled = true;
						}
					}
				}
			}
		}
		int count_tvi = 0;
		public void refreshJsonItem()
		{
			string json = FileContoller.read(cur_jsonfile.path);
			// 삭제
			treeView_json.Items.Clear();

			// 추가

			if(json != null)
			{
				JToken jtok_root;
				MyTreeViewItem jtree_root;
				jtok_root = JsonController.parseJson(json);

				List<string> key_stack = new List<string>();

				jtree_root = addJsonItemMyTreeViewItem(treeView_json, jtok_root, key_stack);
				jtree_root.IsExpanded = true;
				Panel pan = addJsonItemPanel(jtree_root);
				addKeyTextBox(pan, cur_jsonfile.filename, key_stack, false);

				cur_jsonfile.jtok_root = jtok_root;
				cur_jsonfile.jtree_root = jtree_root;
				if(jtok_root == null)
					MessageBox.Show(JsonController.Error_message,"JSon Context Error");
				else
					addJsonItem_recursive(jtree_root, jtok_root, key_stack);
			}
		}
		void addJsonItem_recursive(MyTreeViewItem cur_tvi, JToken jtok, List<string> key_stack)
		{
			if(jtok == null)
				return;

			MyTreeViewItem child_tvi = cur_tvi;
			if(jtok as JObject != null)
			{
				if(jtok.Parent as JArray != null)
				{
					key_stack.Add((jtok.Parent as JArray).IndexOf(jtok).ToString());
					child_tvi = addToJsonTree(cur_tvi, (JObject)jtok, key_stack);
				}
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
		MyTreeViewItem addToJsonTree(MyTreeViewItem parent_tvi, JObject jobj, List<string> key_stack)
		{
			if(jobj.Parent as JProperty != null)
				return parent_tvi;

			MyTreeViewItem child_tvi = addJsonItemMyTreeViewItem(parent_tvi, jobj, key_stack);

			Panel pan = addJsonItemPanel(child_tvi);

			//addTextBlock(sp, (jobj.Parent as JArray).IndexOf(jobj).ToString(), key_stack);
			addKeyTextBox(pan, (jobj.Parent as JArray).IndexOf(jobj).ToString(), key_stack, false);

			addButtonDeletejson(pan, key_stack);

			return child_tvi;
			//return parent_tvi;
		}
		MyTreeViewItem addToJsonTree(MyTreeViewItem parent_tvi, JProperty jprop, List<string> key_stack)
		{
			MyTreeViewItem child_tvi = addJsonItemMyTreeViewItem(parent_tvi, jprop, key_stack);

			Panel pan = addJsonItemPanel(child_tvi);

			addKeyTextBox(pan, jprop, key_stack);
			addButtonDeletejson(pan, key_stack);
			
			return child_tvi;
		}
		MyTreeViewItem addToJsonTree(MyTreeViewItem parent_tvi, JValue jval)
		{
			addValueTextBox(parent_tvi.Header as Panel, jval);
			parent_tvi.Items.Clear();
			return parent_tvi;
		}

		// TreeView 와 MyTreeViewItem 의 상위 클래스 ItemsControl.Items 속성만 사용하기 때문에 상관없다.
		MyTreeViewItem addJsonItemMyTreeViewItem(ItemsControl parent_tvi, JToken linked_jtok, List<string> key_stack)
		{
			MyTreeViewItem child_tvi = new MyTreeViewItem(linked_jtok);
			addButtonAddjson(child_tvi, key_stack);
			if(parent_tvi.Items.Count > 0)
				parent_tvi.Items.Insert(parent_tvi.Items.Count - 1, child_tvi);
			else
				parent_tvi.Items.Add(child_tvi);

			//child_tvi.Tag = "0000";
			child_tvi.Name = "tvi_" + count_tvi++;
			
			child_tvi.AllowDrop = true;
			return child_tvi;
		}
		Panel addJsonItemPanel(MyTreeViewItem tvi)
		{
			StackPanel sp = new StackPanel();
			sp.Height = 30;
			sp.Orientation = Orientation.Horizontal;
			tvi.Header = sp;

			return sp;
		}

		void addKeyTextBox(Panel pan, JProperty jprop, List<string> key_stack, bool isModify = true)
		{
			string key = jprop.Name;
			addKeyTextBox(pan, key, key_stack, isModify);
		}
		void addKeyTextBox(Panel pan, string key, List<string> key_stack, bool isModify = true)
		{
			MyTextBox tb_key = new MyTextBox(key_stack);
			tb_key.Text = key;
			tb_key.Margin = new Thickness(5);
			tb_key.Width = 150;
			if(!isModify)
			{
				tb_key.IsEnabled = false;
				tb_key.BorderBrush = null;
			}
			//tb_key.TextChanged += delegate (object sender, TextChangedEventArgs e)
			//{
			//	MyTextBox tb = sender as MyTextBox;
			//	if(tb == null)
			//		return;

			//	string[] arr_key_stack = tb.key_stack;

			//	JToken tmp = cur_jsonfile.jtok_root;
			//	for(int i = 0; i < arr_key_stack.Length; i++)
			//	{
			//		if(tmp is JObject)
			//			tmp = ((JObject)tmp).GetValue(arr_key_stack[i]);
			//		else if(tmp is JArray)
			//			tmp = ((JArray)tmp)[Convert.ToInt16(arr_key_stack[i])];
			//		else
			//			break;
			//	}
			//	Console.WriteLine("changed : " + e.Changes.Count);
			//	if(tmp is JObject || tmp is JValue)
			//	{
			//		tmp.Parent.Parent[tb_key.Text] = tmp;

			//		if(arr_key_stack.Length > 0)
			//			arr_key_stack[arr_key_stack.Length - 1] = tb_key.Text;

			//		if(tmp != null && tmp.Parent != null)
			//			tmp.Parent.Remove();
			//	}
			//	//(tmp[key] as JProperty).Remove();

			//	FileContoller.write(cur_jsonfile.filename, cur_jsonfile.jtok_root.ToString());
			//};
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
				FileContoller.write(cur_jsonfile.filename, cur_jsonfile.jtok_root.ToString());
			};
			if(pan.Children.Count > 0 && !(pan.Children[pan.Children.Count - 1] is TextBox))
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
		void addButtonAddjson(MyTreeViewItem viewitem, List<string> key_stack)
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

				btnClickAddJson(button, arr_key_stack, viewitem);
			};
			viewitem.Items.Add(btn);
		}
		void btnClickAddJson(MyButton sender, string[] arr_key_stack, MyTreeViewItem tvi)
		{
			JToken tmp = cur_jsonfile.jtok_root;
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

			FileContoller.write(cur_jsonfile.filename, cur_jsonfile.jtok_root.ToString());
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
				MyTreeViewItem parent = pan.Parent as MyTreeViewItem;
				if(parent != null)
				{
					if(parent.Header == pan)
					{
						MyTreeViewItem grandparent = parent.Parent as MyTreeViewItem;
						if(grandparent != null)
							grandparent.Items.Remove(parent);
					}
					else
						parent.Items.Remove(pan);
				}

				// remove from jtok_root
				JToken tmp = cur_jsonfile.jtok_root;
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
					FileContoller.write(cur_jsonfile.filename, cur_jsonfile.jtok_root.ToString());
					refreshJsonItem();
					refreshCommonOption();
				}
				else
				{
					tmp.Parent.Remove();
					FileContoller.write(cur_jsonfile.filename, cur_jsonfile.jtok_root.ToString());
				}
				//FileContoller.write(cur_jsonfile.filename, cur_jsonfile.jtok_root.ToString());
			};
			pan.Children.Add(btn);
			//btn.SetValue(Grid.ColumnProperty, 2);
		}


		private void Btn_view_filew_Click(object sender, RoutedEventArgs e)
		{
			new Window_ViewFile(FileContoller.read(cur_jsonfile.path), cur_jsonfile.path).Show();
		}
		private void Btn_save_jtree_Click(object sender, RoutedEventArgs e)
		{
			cur_jsonfile.jtok_root = convertToJToken(cur_jsonfile.jtree_root);
		}

		JToken convertToJToken(MyTreeViewItem root_tvi)
		{
			/*
				MyTreeViewItem.Items 	--N-> MyTreeViewItem
										--1-> Add_Button
				MyTreeViewItem.Header 	--1-> Panel_Header
				Panel_Header			--N-> TextBlock
										--N-> TextBox
										--1-> Delete_Button
				MyTreeViewItem.Items => MyTreeViewItem 타입의 자식개념, MyTreeViewItem.Header => Panel 타입의 해당 property
			*/
			/*
			 	JToken 상위 클래스
				JObject 		--N-> JProperty
				JProperty		--1-> JObject
								--1-> JArray
								--1-> JValue
				JArray 			--N-> JObject
								children
			*/

			// tree root 는 object 로 시작
			MyTreeViewItem cur_tvi = root_tvi;
			JToken jtok_root = new JObject();

			foreach(var v in cur_tvi.Items)
			{
				MyTreeViewItem tvi = v as MyTreeViewItem;
				if(tvi == null)
					continue;

				//TextBox, TextBlock
			}

			return null;
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
			
			JToken jtok_root = cur_jsonfile.jtok_root;
			if(cur_jsonfile.jtok_root == null)
				return;

			JToken jtok_root_local = jtok_root;
			if(jtok_root["type"] != null)
			{
				//List<string> key_stack = new List<string>();
				jtok_root_local = jtok_root["type"];

				StackPanel sp = new StackPanel();
				sp.Orientation = Orientation.Horizontal;
				addKeyTextBox(sp, jtok_root_local.Parent as JProperty, key_stack);

				TreeViewItem child_tvi = new TreeViewItem();
				child_tvi.Header = sp;
				child_tvi.IsExpanded = true;
				trvItem.Items.Add(child_tvi);

				addJsonCommonOption(child_tvi, jtok_root_local);

			}
			if(jtok_root["comm_option"] != null)
			{
				jtok_root_local = jtok_root["comm_option"];

				StackPanel sp = new StackPanel();
				sp.Orientation = Orientation.Horizontal;
				addKeyTextBox(sp, jtok_root_local.Parent as JProperty, key_stack);

				TreeViewItem child_tvi = new TreeViewItem();
				child_tvi.Header = sp;
				child_tvi.IsExpanded = true;
				trvItem.Items.Add(child_tvi);

				addJsonCommonOption(child_tvi, jtok_root_local);

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

					addKeyTextBox(sp, (jobj.Parent as JArray).IndexOf(jobj).ToString(), null, false);
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