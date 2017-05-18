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
		public class JSonInfo
		{
			public static JSonInfo current = null;
			public string path;
			public string filename;
			public JToken jtok_root;
			public MyTreeViewItem jtree_root;
			public JSonInfo()
			{
				current = this;
			}
			public void Clear()
			{
				path = null;
				filename = null;
				jtok_root = null;
			}
		}
		string root_path = AppDomain.CurrentDomain.BaseDirectory;
		public static test3 m_wnd = null;
		JSonInfo cur_jsonfile = new JSonInfo();
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
			if(files == null)
				return;
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
		// class for JSonTree Linked JToken(JObject, JArray, JProperty)
		public class MyTreeViewItem : TreeViewItem
		{
			// JObject, JArray, JProperty
			// now
			//		if(root)	-> JObject
			//		else		->JProperty
			public JToken linked_jtoken = null;
			//public JProperty linked_jtoken = null
			
			public new StackPanel Header { get {return base.Header as StackPanel; } set {base.Header = value; } }
			static int count_tvi;

			// ItemsControl = TreeView 와 MyTreeViewItem 의 상위 클래스 ItemsControl.Items.Add 함수만 쓴다.
			public MyTreeViewItem(ItemsControl parent_tvi, JToken _linked_jtoken)
			{
				linked_jtoken = _linked_jtoken;
				CreateHeader();

				this.Name = "tvi_" + count_tvi++;
				this.AllowDrop = true;
				
				if(parent_tvi.Items.Count > 0)
					parent_tvi.Items.Insert(parent_tvi.Items.Count - 1, this);
				else
					parent_tvi.Items.Add(this);
			}
			private void CreateHeader()
			{
				StackPanel sp = new StackPanel();
				sp.Height = 30;
				sp.Orientation = Orientation.Horizontal;
				this.Header = sp;
			}

			public void Remove()
			{
				//Console.WriteLine(linked_jtoken.Type);
				MyTreeViewItem parent_tvi;
				parent_tvi = this.Parent as MyTreeViewItem;
				// this 가 루트이거나 다른 예외상황이면 삭제하지 않는다.
				if(parent_tvi == null)
					return;

				// JArray 삭제시 인덱스 수정
				if(this.linked_jtoken.Parent is JArray)
				{
					for(int i = parent_tvi.Items.IndexOf(this) + 1, newidx = parent_tvi.Items.IndexOf(this); i < parent_tvi.Items.Count; i++)
					{
						MyTreeViewItem child_tvi = parent_tvi.Items[i] as MyTreeViewItem;
						if(child_tvi == null)
							continue;

						// JArray 의 자식 TreeViewItem 은 헤더에 TextBox, Button 하나씩 들어있음.
						foreach(var v in child_tvi.Header.Children)
						{
							TextBox tb = v as TextBox;
							if(tb == null)
								continue;

							tb.Text = newidx.ToString();
							break;
						}

						newidx++;
					}
				}

				// Json Tree Remove
				parent_tvi.Items.Remove(this);
				
				// Json Token Remove
				this.linked_jtoken.Remove();
			}
			public delegate void MakeTree(MyTreeViewItem cur_tvi, JToken jtok);
			public void Add(Button sender, MakeTree makeTree)
			{
				JToken tmp = this.linked_jtoken;
				// tmp == root 일때
				if(tmp is JObject)
					;
				else
				{
					JProperty jprop = tmp as JProperty;
					tmp = jprop.Value;
				}
				JToken add_jtok;
				if(tmp is JArray)
				{
					// add Jtok_root
					add_jtok = new JObject();
					(tmp as JArray).Add(add_jtok);
				}
				else
				{
					// window_addJson showdialog()
					popup_AddJsonItem popup = new popup_AddJsonItem();

					Point pt = sender.PointToScreen(new Point(0, 0));
					popup.Left = pt.X;
					popup.Top = pt.Y;
					if(popup.ShowDialog() != true)
						;

					// cancel return
					if(popup.return_ok == false)
						return;

					// ok return & add Jtok_root
					add_jtok = new JProperty(popup.key, popup.value);
					(tmp as JObject).Add(add_jtok);
				}

				// add to jtree_root
				makeTree(this, add_jtok);
			}
			#region code for moving MyTreeViewItem in screen
			// 외부에서 수정 불가 이벤트로만 수정
			static MyTreeViewItem selected_tvi = null;
			public static MyTreeViewItem Selectred_tvi { get { return selected_tvi; } }

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
			#endregion
		}

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

				jtree_root = new MyTreeViewItem(treeView_json, jtok_root);
				jtree_root.IsExpanded = true;

				addButtonAddjson(jtree_root);
				addKeyTextBox(jtree_root.Header, cur_jsonfile.filename, false);

				cur_jsonfile.jtok_root = jtok_root;
				cur_jsonfile.jtree_root = jtree_root;
				if(jtok_root == null)
					MessageBox.Show(JsonController.Error_message,"JSon Context Error");
				else
					makeJsonItem_recursive(jtree_root, jtok_root);
			}
		}
		void makeJsonItem_recursive(MyTreeViewItem cur_tvi, JToken jtok)
		{
			if(jtok == null)
				return;

			MyTreeViewItem child_tvi = addToJsonTree(cur_tvi, jtok);

			foreach(var v in jtok.Children())
			{
				makeJsonItem_recursive(child_tvi, v);
			}
		}
		MyTreeViewItem addToJsonTree(MyTreeViewItem parent_tvi, JToken jtok)
		{
			if(jtok is JObject)
			{
				JObject jobj = jtok as JObject;
				if(jobj.Parent == null)
					return parent_tvi;
				if(jobj.Parent != null && jobj.Parent is JArray)
				{
					MyTreeViewItem child_tvi = new MyTreeViewItem(parent_tvi, jobj);

					addButtonAddjson(child_tvi);
					addKeyTextBox(child_tvi.Header, (jobj.Parent as JArray).IndexOf(jobj).ToString(), false);
					addButtonDeletejson(child_tvi);
					return child_tvi;
				}
				else
				{
					addKeyTextBox(parent_tvi.Header, "object", false);
					return parent_tvi;
				}
			}
			else if(jtok is JProperty)
			{
				JProperty jprop = jtok as JProperty;
				MyTreeViewItem child_tvi = new MyTreeViewItem(parent_tvi, jprop);

				addButtonAddjson(child_tvi);
				addKeyTextBox(child_tvi.Header, jprop);
				addButtonDeletejson(child_tvi);

				return child_tvi;
			}
			else if(jtok is JValue)
			{
				JValue jval = jtok as JValue;
				addValueTextBox(parent_tvi.Header, jval);
				//parent_tvi.Items.Clear();
				return parent_tvi;
			}
			else if(jtok is JArray)
			{
				addKeyTextBox(parent_tvi.Header, "array", false);
				return parent_tvi;
			}
			else
			{
				Console.WriteLine("[error] undefined type = " + jtok.GetType());
				return parent_tvi;
			}
		}
		
		class KeyTextBox : TextBox
		{
			public enum Type
			{
				JProperty = 0,
				JObject = 1,
				JArray = 2
			}
			// 0 = JProperty, 1 = JObject, 2 = JArray
			public Type type = 0;
			public KeyTextBox(Panel pan, string _type, bool isModify)
			{
				switch(_type)
				{
					case "object":
						type = Type.JObject;
						break;
					case "array":
						type = Type.JArray;
						break;
					default:
						type = Type.JProperty;
						break;
				}

				this.Text = _type;
				this.Margin = new Thickness(5);
				this.Width = 150;
				if(!isModify)
				{
					this.IsEnabled = false;
					this.BorderBrush = null;
				}

				if(pan.Children.Count > 0 && !(pan.Children[pan.Children.Count - 1] is TextBox))
					pan.Children.Insert(pan.Children.Count - 1, this);
				else
					pan.Children.Add(this);
			}
		}
		void addKeyTextBox(Panel pan, JProperty jprop, bool isModify = true)
		{
			string key = jprop.Name;
			addKeyTextBox(pan, key, isModify);
		}
		void addKeyTextBox(Panel pan, string key, bool isModify = true)
		{
			KeyTextBox tb_key = new KeyTextBox(pan, key, isModify);
			//tb_key.Text = key;
			//tb_key.Margin = new Thickness(5);
			//tb_key.Width = 150;
			//if(!isModify)
			//{
			//	tb_key.IsEnabled = false;
			//	tb_key.BorderBrush = null;
			//}
			////tb_key.TextChanged += delegate (object sender, TextChangedEventArgs e)
			////{
			////	KeyTextBox tb = sender as KeyTextBox;
			////	if(tb == null)
			////		return;

			////	string[] arr_key_stack = tb.key_stack;

			////	JToken tmp = cur_jsonfile.jtok_root;
			////	for(int i = 0; i < arr_key_stack.Length; i++)
			////	{
			////		if(tmp is JObject)
			////			tmp = ((JObject)tmp).GetValue(arr_key_stack[i]);
			////		else if(tmp is JArray)
			////			tmp = ((JArray)tmp)[Convert.ToInt16(arr_key_stack[i])];
			////		else
			////			break;
			////	}
			////	Console.WriteLine("changed : " + e.Changes.Count);
			////	if(tmp is JObject || tmp is JValue)
			////	{
			////		tmp.Parent.Parent[tb_key.Text] = tmp;

			////		if(arr_key_stack.Length > 0)
			////			arr_key_stack[arr_key_stack.Length - 1] = tb_key.Text;

			////		if(tmp != null && tmp.Parent != null)
			////			tmp.Parent.Remove();
			////	}
			////	//(tmp[key] as JProperty).Remove();

			////	FileContoller.write(cur_jsonfile.filename, cur_jsonfile.jtok_root.ToString());
			////};
			//if(pan.Children.Count > 0 && !(pan.Children[pan.Children.Count - 1] is TextBox))
			//	pan.Children.Insert(pan.Children.Count - 1, tb_key);
			//else
			//	pan.Children.Add(tb_key);
			////tb_key.SetValue(Grid.ColumnProperty, 0);
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

		class MyButton : Button
		{
		}
		void addButtonDeletejson(MyTreeViewItem tvi)
		{
			MyButton btn = new MyButton();
			btn.Content = '-';
			btn.Background = Brushes.White;
			btn.BorderBrush = Brushes.Black;
			btn.Width = 20;
			btn.Height = 20;
			btn.VerticalContentAlignment = VerticalAlignment.Center;
			btn.HorizontalContentAlignment = HorizontalAlignment.Center;
			
			btn.Click += delegate (object sender, RoutedEventArgs e)
			{
				tvi.Remove();
				FileContoller.write(cur_jsonfile.filename, cur_jsonfile.jtok_root.ToString());
			};

			Panel pan = tvi.Header as Panel;
			if(pan != null)
				pan.Children.Add(btn);
			//btn.SetValue(Grid.ColumnProperty, 2);
		}
		void addButtonAddjson(MyTreeViewItem tvi)
		{
			MyButton btn = new MyButton();
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

				//tvi.Add(button, makeJsonItem_recursive);
				//FileContoller.write(JSonInfo.current.filename, JSonInfo.current.jtok_root.ToString());
				btnClickAddJson(button, tvi);
			};
			tvi.Items.Add(btn);
		}
		void btnClickAddJson(MyButton sender, MyTreeViewItem tvi)
		{
			JToken tmp = tvi.linked_jtoken;
			// tmp == root 일때
			if(tmp is JObject)
				;
			else
			{
				JProperty jprop = tmp as JProperty;
				tmp = jprop.Value;
			}
			JToken add_jtok = null;
			if(tmp is JArray)
			{
				JArray jarr = tmp as JArray;
				// add Jtok_root
				add_jtok = new JObject();
				jarr.Add(add_jtok);
			}
			else if(tmp is JObject)
			{
				JObject jobj = tmp as JObject;
				// window_addJson showdialog()
				popup_AddJsonItem popup = new popup_AddJsonItem();
				Point pt = sender.PointToScreen(new Point(0, 0));
				popup.Left = pt.X;
				popup.Top = pt.Y;
				if(popup.ShowDialog() != true)
					;

				// cancel return
				if(popup.return_ok == false)
					return;

				foreach(JProperty v in jobj.Properties())
				{
					if(v.Name == popup.key)
						// 키 중복
						return;
				}

				// ok return & add Jtok_root
				add_jtok = new JProperty(popup.key, popup.value);
				jobj.Add(add_jtok);

			}

			// add to jtree_root
			if(add_jtok != null)
				makeJsonItem_recursive(tvi, add_jtok);
			FileContoller.write(JSonInfo.current.filename, JSonInfo.current.jtok_root.ToString());
		}

		private void Btn_view_filew_Click(object sender, RoutedEventArgs e)
		{
			new Window_ViewFile(FileContoller.read(cur_jsonfile.path), cur_jsonfile.path).Show();
		}
		private void Btn_save_jtree_Click(object sender, RoutedEventArgs e)
		{
			Window_Search wnd = new Window_Search();
			wnd.ShowDialog();

			if(wnd.path_return == null)
				return;

			cur_jsonfile.jtok_root = convertToJToken(cur_jsonfile.jtree_root);
			FileContoller.write(wnd.path_return, cur_jsonfile.jtok_root.ToString());
		}

		JObject convertToJToken(MyTreeViewItem root_tvi)
		{
			/*
				MyTreeViewItem.Items 	--N-> MyTreeViewItem * N + Add_Button
				MyTreeViewItem.Header 	--1-> Panel_Header
				Panel_Header			--3-> TextBox * 2 (key, val or val.type) + Delete_Button
				MyTreeViewItem.Items => MyTreeViewItem 타입의 자식개념, MyTreeViewItem.Header => Panel 타입의 해당 property
			*/
			/*
			 	JToken 상위 클래스
				JObject 		--N-> JProperty
				JProperty		--1->	{JObject}
										{JArray } 중 하나
										{JValue }
				JArray 			--N-> JObject
								children
			*/

			// tree root 는 object 로 시작
			JObject jtok_root = new JObject();

			convertToJToken_recursive(root_tvi, jtok_root);

			return jtok_root;
		}
		void convertToJToken_recursive(MyTreeViewItem tvi_cur, JObject jobj_cur)
		{
			if(tvi_cur == null || jobj_cur == null)
				return;

			JToken cur_jtok = jobj_cur;
			// 여기서부터는 property 로 시작
			foreach(var v in tvi_cur.Items)
			{
				MyTreeViewItem tvi_child = v as MyTreeViewItem;
				if(tvi_child == null)
					continue;

				Panel pan = tvi_child.Header as Panel;
				if(pan == null)
					continue;

				// pan(MyTreeViewItem.Header) = key, val or val.type, button
				KeyTextBox tb_key = pan.Children[0] as KeyTextBox;
				TextBox tb_val = pan.Children[1] as TextBox;

				if(tb_val is KeyTextBox)
				{
					KeyTextBox tb_type = tb_val as KeyTextBox;
					switch(tb_type.type)
					{
						case KeyTextBox.Type.JObject:
							{
								JObject jobj = new JObject();
								JProperty jprop = new JProperty(tb_key.Text, jobj);
								(cur_jtok as JObject).Add(jprop);
								convertToJToken_recursive(tvi_child, jobj);
							}
							break;
						case KeyTextBox.Type.JArray:
							{
								JArray jarr = new JArray();
								JProperty jprop = new JProperty(tb_key.Text, jarr);
								(cur_jtok as JObject).Add(jprop);
								for(int i = 0; i < tvi_child.Items.Count; i++)
								{
									JObject jobj = new JObject();
									jarr.Add(jobj);
									convertToJToken_recursive(tvi_child.Items[i] as MyTreeViewItem, jobj);
								}
							}
							break;
					}
				}
				else
				{
					JProperty jprop = new JProperty(tb_key.Text, tb_val.Text);
					(cur_jtok as JObject).Add(jprop);
				}
			}
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
				addKeyTextBox(sp, jtok_root_local.Parent as JProperty, false);

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
				addKeyTextBox(sp, jtok_root_local.Parent as JProperty, false);

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

					addKeyTextBox(sp, (jobj.Parent as JArray).IndexOf(jobj).ToString(), false);
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

				addKeyTextBox(sp, jprop, false);
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
		
		#endregion
	}
}