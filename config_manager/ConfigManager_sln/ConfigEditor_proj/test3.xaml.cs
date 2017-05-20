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
			public string Path
			{
				get { return path; }
				set
				{
					path = value;

					if(this.Jtree_root == null)
						return;

					Panel pan = this.Jtree_root.Header as Panel;
					if(pan == null)
						return;

					TextBox tb = pan.Children[0] as TextBox;
					if(tb == null)
						return;

					tb.Text = Filename;
				}
			}
			private JToken jtok_root;
			public JToken Jtok_root
			{
				get {return jtok_root; }
				set
				{
					jtok_root = value;
					isModify_jtok = true;
				}
			}
			private bool isModify_jtok = false;
			public bool IsModify_jtok { get { return isModify_jtok; } }

			private MyTreeViewItem jtree_root;
			public MyTreeViewItem Jtree_root
			{
				get { return jtree_root; }
				set
				{
					jtree_root = value;
					isModify_tree = true;
				}
			}
			private bool isModify_tree = false;
			public bool IsModify_tree { get { return isModify_tree; } }

			public JSonInfo()
			{
				current = this;
			}

			public void Clear()
			{
				Path = null;
				Jtok_root = null;
				Jtree_root = null;
			}

			public string Filename
			{
				get
				{
					if(Path != null)
					{
						string[] split = Path.Split('\\');
						return split[split.Length - 1];
					}
					return null;
				}
			}
		}
		string root_path = AppDomain.CurrentDomain.BaseDirectory;
		public static test3 m_wnd = null;
		//JSonInfo cur_jsonfile = new JSonInfo();
		public test3()
		{
			InitializeComponent();

			//refreshJsonFile();

			//treeView_jsonfile.SelectedItemChanged += TreeView_jsonfile_SelectedItemChanged;

			tb_root_path.Text = root_path;
			m_wnd = this;

			//tabControl_json.SelectionChanged += TabControl_json_SelectionChanged;

			this.Closed += Test3_Closed;
			//convertToJToken(JsonInfo.current.jtree_root);
			new JSonInfo();
		}


		private void Test3_Closed(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
		}

		//private void TabControl_json_SelectionChanged(object sender, SelectionChangedEventArgs e)
		//{
		//	switch(tabControl_json.SelectedIndex)
		//	{
		//		case 0:
		//			refreshJsonItem();
		//			//refreshCommonOption();
		//			break;
		//		case 1:
		//			refreshJsonItem();
		//			break;
		//	}
		//}
		
		
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
			
			JSonInfo.current.Path = selected.path;
			refreshJsonItem();
			//refreshCommonOption();
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
			JSonInfo.current.Clear();

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
			JSonInfo.current.Path = ((tvi.Items.GetItemAt(0) as TreeViewItem).Header as PathTextBlock).path as string;
			refreshJsonItem();
			//refreshCommonOption();

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
			//        if(root)    -> JObject
			//        else        ->JProperty
			public JToken linked_jtoken = null;
			//public JProperty linked_jtoken = null

			ToggleButton button_more = null;
			const int IDX_MOREBUTTON = 3;

			public new StackPanel Header { get { return base.Header as StackPanel; } set { base.Header = value; } }
			static int count_tvi;

			// ItemsControl = TreeView 와 MyTreeViewItem 의 상위 클래스 ItemsControl.Items 요소만 쓴다.
			public MyTreeViewItem(ItemsControl parent_tvi, JToken _linked_jtoken)
			{
				linked_jtoken = _linked_jtoken;
				CreateHeader();

				this.Name = "tvi_" + count_tvi++;
				this.AllowDrop = true;
				this.IsExpanded = true;


				int idx_insert = parent_tvi.Items.Count;
				while(idx_insert > 0 && (parent_tvi.Items[idx_insert - 1] is MyButton))
					idx_insert--;
				parent_tvi.Items.Insert(idx_insert, this);

				MyTreeViewItem tvi = this.Parent as MyTreeViewItem;
				if(tvi == null)
					return;

				tvi.refreshMoreButtonPlace();
				//addMoreButton(parent_tvi as MyTreeViewItem);
				if(tvi.button_more != null && tvi.button_more.IsChecked == false)
				{
					tvi.hideMore();
				}
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
				parent_tvi.refreshMoreButtonPlace();
				// Json Token Remove
				this.linked_jtoken.Remove();
			}
			public delegate void MakeTree(MyTreeViewItem cur_tvi, JToken jtok);
			public void Add(Button sender, MakeTree makeTree)
			{
				if(sender == null)
					return;

				JToken tmp = this.linked_jtoken;
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

					// cancel return
					if(popup.ShowDialog() != true)
						return;

					foreach(JProperty v in jobj.Properties())
					{
						// 키 중복
						if(v.Name == popup.key)
							return;
					}

					// ok return & add Jtok_root
					add_jtok = new JProperty(popup.key, popup.value);
					jobj.Add(add_jtok);

				}

				// add to jtree_root
				if(add_jtok != null)
					makeTree(this, add_jtok);
			}

			protected override void OnExpanded(RoutedEventArgs e)
			{
				base.OnExpanded(e);
				this.hideMore();
			}

			public int getCountChildProperty()
			{
				int cnt_property = 0;
				for(int i = 0; i < this.Items.Count; i++)
				{
					if(this.Items[i] is MyTreeViewItem)
						cnt_property++;
				}
				return cnt_property;
			}
			private void addMoreButton(MyTreeViewItem tvi)
			{
				if(tvi == null)
					return;

				// 루트의 자식들에는 버튼생성 하지 않는다.
				if(!(tvi.Parent is MyTreeViewItem))
					return;

				// 에러체크 와 property 개수가 IDX_MOREBUTTON 를 넘으면 생성한다.
				if(tvi.button_more != null && IDX_MOREBUTTON >= tvi.getCountChildProperty())
					return;

				ToggleButton btn = new ToggleButton();
				btn.Content = "more..";
				btn.Background = Brushes.White;
				btn.BorderBrush = null;
				btn.Click += delegate (object sender, RoutedEventArgs e)
				{
					if(btn.IsChecked == true)
					{
						tvi.visibleMore();
					}
					else
					{
						tvi.hideMore();
					}
				};
				int idx_insert = IDX_MOREBUTTON > tvi.Items.Count ? tvi.Items.Count : IDX_MOREBUTTON;
				while(idx_insert > 0 && (tvi.Items[idx_insert - 1] is MyButton))
					idx_insert--;

				tvi.Items.Insert(idx_insert, btn);
				btn.IsChecked = false;
				tvi.button_more = btn;
			}
			public void visibleMore()
			{
				for(int i = 0; i < this.Items.Count; i++)
				{
					UIElement ui = this.Items[i] as UIElement;
					if(ui == null)
						continue;

					ui.Visibility = Visibility.Visible; ;
				}
			}
			public void hideMore()
			{
				for(int i = 4; i < this.Items.Count; i++)
				{
					UIElement ui = this.Items[i] as UIElement;
					if(ui == null)
						continue;

					ui.Visibility = Visibility.Collapsed;
				}
			}
			void refreshMoreButtonPlace()
			{
				int cnt_property = this.getCountChildProperty();
				if(cnt_property <= IDX_MOREBUTTON)
				{
					if(this.button_more != null)
					{
						this.Items.Remove(button_more);
						button_more = null;
					}
				}
				else
				{
					if(this.button_more == null)
					{
						addMoreButton(this);
					}
					else if (this.Items.IndexOf(button_more) != IDX_MOREBUTTON)
					{
						bool tmp = button_more.IsChecked.Value;
						this.Items.Remove(button_more);
						button_more = null;
						addMoreButton(this);
						button_more.IsChecked = tmp;
					}
				}
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
					MyTreeViewItem.selected_tvi = null;
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
				PublicDragOver(e);
			}
			public void PublicDragOver(DragEventArgs e)
			{
				this.IsSelected = true;
				e.Handled = true;
			}
			protected override void OnDrop(DragEventArgs e)
			{
				PublicDrop(e);
			}
			public void PublicDrop(DragEventArgs e)
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
							int idx_insert = tvi_parent.Items.IndexOf(this);
							tvi_src_parent.Items.Remove(tvi_src);
							if(linked_jtoken is JProperty)
							{
								//// mouse pointer 에 따라 요소 위에 insert 할지 아래에 insert 할지 결정.
								//// Mouse.GetPosition() 이상한 값 리턴
								//Point pointToWindow = e.GetPosition(this);
								//double height = 0;
								//if(this.Header is Panel)
								//    height = ((Panel)this.Header).Height;
								//if(pointToWindow.Y > height / 2 && idx_insert < tvi_parent.Items.Count)
								//    idx_insert++;

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

							//Console.WriteLine("src = " + tvi_src.Name + ", src_parent = " + tvi_src_parent.Name + ", dst = " + this.Name + ", dst_parent = " + tvi_parent.Name);
							e.Handled = true;
							MyTreeViewItem tvi = this.Parent as MyTreeViewItem;
							if(tvi != null)
								tvi.refreshMoreButtonPlace();
						}
					}
				}
			}
			#endregion
		}

		public void refreshJsonItem()
		{
			string json = FileContoller.read(JSonInfo.current.Path);
			string filename = JSonInfo.current.Filename;
			// 삭제
			treeView_json.Items.Clear();

			// 추가
			JToken jtok_root;
			jtok_root = JsonController.parseJson(json);
			if(jtok_root != null)
			{
				convertToTreeView(treeView_json, jtok_root, filename);
			}
			else
			{
				jtok_root = new JObject();
				convertToTreeView(treeView_json, jtok_root, filename);
			}
		}
		void convertToTreeView(TreeView treeView, JToken jtok_root, string filename)
		{
			if(jtok_root == null)
			{
				MessageBox.Show(JsonController.Error_message, "JSon Context Error");
				return;
			}

			MyTreeViewItem jtree_root;

			List<string> key_stack = new List<string>();

			jtree_root = new MyTreeViewItem(treeView, jtok_root);
			jtree_root.IsExpanded = true;

			addButtonAddjson(jtree_root);
			addKeyTextBox(jtree_root.Header, filename, false);

			JSonInfo.current.Jtok_root = jtok_root;
			JSonInfo.current.Jtree_root = jtree_root;
			makeJsonItem_recursive(jtree_root, jtok_root);
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
					addButtonAddjson(parent_tvi);
					return parent_tvi;
				}
			}
			else if(jtok is JProperty)
			{
				JProperty jprop = jtok as JProperty;
				MyTreeViewItem child_tvi = new MyTreeViewItem(parent_tvi, jprop);

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
				addButtonAddjson(parent_tvi);
				return parent_tvi;
			}
			else
			{
				Console.WriteLine("[error] undefined type = " + jtok.GetType());
				return parent_tvi;
			}
		}

		class MyTextBox : TextBox
		{
			public MyTextBox()
			{
				this.AllowDrop = true;

				this.Margin = new Thickness(5);
				this.Width = 150;
			}
			protected override void OnDragOver(DragEventArgs e)
			{
				MyTreeViewItem tvi_parent = this.Parent as MyTreeViewItem;
				if(tvi_parent == null)
					return;

				tvi_parent.PublicDragOver(e);
				Console.WriteLine("KeyTextBox.OnDragOver()");
				base.OnDragOver(e);
			}
			protected override void OnDrop(DragEventArgs e)
			{
				MyTreeViewItem tvi_parent = this.Parent as MyTreeViewItem;
				if(tvi_parent == null)
					return;

				tvi_parent.PublicDrop(e);
				Console.WriteLine("KeyTextBox.OnDrop()");
				base.OnDrop(e);
			}
		}
		class KeyTextBox : MyTextBox
		{
			public enum Type
			{
				JProperty = 0,
				JObject = 1,
				JArray = 2
			}
			// 0 = JProperty, 1 = JObject, 2 = JArray
			public Type type = 0;
			public KeyTextBox(Panel parent, string _type, bool isModify)
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
				
				if(!isModify)
				{
					this.IsEnabled = false;
					this.BorderBrush = null;
				}
				
				int idx_insert = parent.Children.Count;
				while(idx_insert > 0 && !(parent.Children[idx_insert - 1] is TextBox))
					idx_insert--;

				parent.Children.Insert(idx_insert, this);
			}

		}
		class ValueTextBox : MyTextBox
		{
			public ValueTextBox(Panel parent, string data)
			{
				this.Text = data;

				//Binding myBinding = new Binding("Value");
				//myBinding.Source = jval;
				//myBinding.Mode = BindingMode.TwoWay;
				//myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				//this.SetBinding(TextBox.TextProperty, myBinding);

				//this.TextChanged += delegate (object sender, TextChangedEventArgs e)
				//{
				//	//FileContoller.write(JsonInfo.current.filename, JsonInfo.current.jtok_root.ToString());
				//};

				int idx_insert = parent.Children.Count;
				while(idx_insert > 0 && !(parent.Children[idx_insert - 1] is TextBox))
					idx_insert--;

				parent.Children.Insert(idx_insert, this);
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
		}
		void addValueTextBox(Panel pan, JValue jval)
		{
			string data = jval.ToString();

			ValueTextBox tb_value = new ValueTextBox(pan, data);
		}

		class MyButton : Button
		{
		}
		void addButtonDeletejson(MyTreeViewItem tvi)
		{
			MyButton btn = new MyButton();
			btn.Content = '-';
			btn.Background = Brushes.LightPink;
			btn.BorderBrush = Brushes.Black;
			btn.Margin = new Thickness(2.5);
			btn.Width = 20;
			btn.Height = 20;
			btn.VerticalContentAlignment = VerticalAlignment.Center;
			btn.HorizontalContentAlignment = HorizontalAlignment.Center;

			btn.Click += delegate (object sender, RoutedEventArgs e)
			{
				tvi.Remove();
				//FileContoller.write(JsonInfo.current.filename, JsonInfo.current.jtok_root.ToString());
			};

			Panel pan = tvi.Header as Panel;
			if(pan != null)
				pan.Children.Add(btn);
		}
		void addButtonAddjson(MyTreeViewItem tvi)
		{
			MyButton btn = new MyButton();
			btn.Content = '+';
			btn.Background = Brushes.LightGreen;
			btn.BorderBrush = Brushes.Black;
			btn.Margin = new Thickness(2.5);
			btn.Width = 20;
			btn.Height = 20;
			btn.VerticalContentAlignment = VerticalAlignment.Center;
			btn.HorizontalContentAlignment = HorizontalAlignment.Center;

			btn.Click += delegate (object sender, RoutedEventArgs e)
			{
				MyButton button = sender as MyButton;
				tvi.Add(sender as Button, makeJsonItem_recursive);
				//FileContoller.write(JSonInfo.current.filename, JSonInfo.current.jtok_root.ToString());
			};
			Panel parent = tvi.Header as Panel;
			if(parent == null)
				return;

			int idx_insert = parent.Children.Count;
			while(idx_insert > 0 && !(parent.Children[idx_insert - 1] is TextBox))
				idx_insert--;

			parent.Children.Insert(idx_insert, btn);
			//pan.Children.Add(btn);
		}

		private void Btn_view_filew_Click(object sender, RoutedEventArgs e)
		{
			if(JSonInfo.current == null || JSonInfo.current.Path == null)
				return;

			Window_ViewFile w = new Window_ViewFile(FileContoller.read(JSonInfo.current.Path), JSonInfo.current.Path);

			if(w.ShowDialog() == true)
			{
				FileContoller.write(JSonInfo.current.Path, w.tb_file.Text);
				refreshJsonItem();
			}
		}
		private void Btn_save_jtree_Click(object sender, RoutedEventArgs e)
		{
			if(JSonInfo.current == null || JSonInfo.current.Path == null)
			{
				Btn_save_other_jsonfile_Click(sender, e);
				return;
			}

			FileInfo f = new FileInfo(JSonInfo.current.Path);
			if(!f.Exists)
			{
				Btn_save_other_jsonfile_Click(sender, e);
				return;
			}

			JSonInfo.current.Jtok_root = convertToJToken(JSonInfo.current.Jtree_root);
			FileContoller.write(JSonInfo.current.Path, JSonInfo.current.Jtok_root.ToString());
		}
		private void Btn_save_other_jsonfile_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();

			sfd.InitialDirectory = root_path;
			//sfd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

			if(JSonInfo.current != null && JSonInfo.current.Path != null)
			{
				string dir_path = JSonInfo.current.Path.Substring(0, JSonInfo.current.Path.LastIndexOf('\\') + 1);
				DirectoryInfo d = new DirectoryInfo(dir_path);
				if(d.Exists)
					sfd.InitialDirectory = dir_path;
			}

			sfd.Filter = "JSon Files (.json)|*.json";
			if(sfd.ShowDialog(this) == true)
			{
				//Console.WriteLine(sfd.FileName);
				JSonInfo.current.Jtok_root = convertToJToken(JSonInfo.current.Jtree_root);
				JSonInfo.current.Path = sfd.FileName;
				FileContoller.write(sfd.FileName, JSonInfo.current.Jtok_root.ToString());
				//refreshJsonItem();
				//refreshJsonFile();
			}
		}
		private void Btn_open_jsonfile_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();

			ofd.InitialDirectory = root_path;
			//sfd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

			if(JSonInfo.current != null && JSonInfo.current.Path != null)
			{
				string dir_path = JSonInfo.current.Path.Substring(0, JSonInfo.current.Path.LastIndexOf('\\') + 1);
				DirectoryInfo d = new DirectoryInfo(dir_path);
				if(d.Exists)
					ofd.InitialDirectory = dir_path;
			}

			ofd.Filter = "JSon Files (.json)|*.json";
			if(ofd.ShowDialog(this) == true)
			{
				Console.WriteLine(ofd.FileName);
				string json = FileContoller.read(ofd.FileName);
				JSonInfo.current.Path = ofd.FileName;
				string[] split = ofd.FileName.Split('\\');
				refreshJsonItem();
				//refreshJsonFile();
			}
		}
		private void Btn_new_jsonfile_Click(object sender, RoutedEventArgs e)
		{
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
			JSonInfo.current.Clear();
			refreshJsonItem();
		}
		private void Btn_cancel_jsonfile_Click(object sender, RoutedEventArgs e)
		{
			if(JSonInfo.current == null || JSonInfo.current.Path == null)
				return;

			string dir_path = JSonInfo.current.Path.Substring(0, JSonInfo.current.Path.LastIndexOf('\\') + 1);
			DirectoryInfo d = new DirectoryInfo(dir_path);
			if(!d.Exists)
				return;

			refreshJsonItem();
		}

		JObject convertToJToken(MyTreeViewItem root_tvi)
		{
			if(root_tvi == null)
				return null;

			/*
                MyTreeViewItem.Items     --N-> MyTreeViewItem * N + Add_Button
                MyTreeViewItem.Header     --1-> Panel_Header
                Panel_Header            --3-> TextBox * 2 (key, val or val.type) + Delete_Button
                MyTreeViewItem.Items => MyTreeViewItem 타입의 자식개념, MyTreeViewItem.Header => Panel 타입의 해당 property
            */
			/*
                 JToken 상위 클래스
                JObject         --N-> JProperty
                JProperty        --1->    {JObject}
                                        {JArray } 중 하나
                                        {JValue }
                JArray             --N-> JObject
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
		//void refreshCommonOption()
		//{
		//	// 삭제
		//	treeView_common_option.Items.Clear();

		//	// 추가
		//	List<string> key_stack = new List<string>();
		//	TreeViewItem trvItem = new TreeViewItem();
		//	trvItem.Header = JsonInfo.current.filename;
		//	trvItem.IsExpanded = true;

		//	treeView_common_option.Items.Add(trvItem);

		//	JToken jtok_root = JsonInfo.current.jtok_root;
		//	if(JsonInfo.current.jtok_root == null)
		//		return;

		//	JToken jtok_root_local = jtok_root;
		//	if(jtok_root["type"] != null)
		//	{
		//		//List<string> key_stack = new List<string>();
		//		jtok_root_local = jtok_root["type"];

		//		StackPanel sp = new StackPanel();
		//		sp.Orientation = Orientation.Horizontal;
		//		addKeyTextBox(sp, jtok_root_local.Parent as JProperty, false);

		//		TreeViewItem child_tvi = new TreeViewItem();
		//		child_tvi.Header = sp;
		//		child_tvi.IsExpanded = true;
		//		trvItem.Items.Add(child_tvi);

		//		addJsonCommonOption(child_tvi, jtok_root_local);

		//	}
		//	if(jtok_root["comm_option"] != null)
		//	{
		//		jtok_root_local = jtok_root["comm_option"];

		//		StackPanel sp = new StackPanel();
		//		sp.Orientation = Orientation.Horizontal;
		//		addKeyTextBox(sp, jtok_root_local.Parent as JProperty, false);

		//		TreeViewItem child_tvi = new TreeViewItem();
		//		child_tvi.Header = sp;
		//		child_tvi.IsExpanded = true;
		//		trvItem.Items.Add(child_tvi);

		//		addJsonCommonOption(child_tvi, jtok_root_local);

		//	}

		//}


		//void addJsonCommonOption(TreeViewItem tvi, JToken jtok)
		//{
		//	if(jtok == null)
		//		return;

		//	TreeViewItem child_tvi = tvi;
		//	if(jtok as JObject != null)
		//	{
		//		if(jtok.Parent as JArray != null)
		//		{
		//			JObject jobj = jtok as JObject;
		//			child_tvi = new TreeViewItem();
		//			tvi.Items.Add(child_tvi);

		//			StackPanel sp = new StackPanel();
		//			sp.Orientation = Orientation.Horizontal;
		//			child_tvi.Header = sp;

		//			addKeyTextBox(sp, (jobj.Parent as JArray).IndexOf(jobj).ToString(), false);
		//		}
		//	}
		//	else if(jtok as JProperty != null)
		//	{
		//		JProperty jprop = jtok as JProperty;
		//		child_tvi = new TreeViewItem();
		//		tvi.Items.Add(child_tvi);

		//		StackPanel sp = new StackPanel();
		//		sp.Orientation = Orientation.Horizontal;
		//		child_tvi.Header = sp;

		//		addKeyTextBox(sp, jprop, false);
		//	}
		//	else if(jtok as JValue != null)
		//	{
		//		JValue jval = jtok as JValue;
		//		addValueTextBox(tvi.Header as Panel, jval);
		//	}
		//	else
		//	{
		//		Console.WriteLine(jtok.GetType());
		//	}

		//	foreach(var v in jtok.Children())
		//	{
		//		addJsonCommonOption(child_tvi, v);
		//	}
		//}

		#endregion
	}
}