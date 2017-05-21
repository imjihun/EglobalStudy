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

namespace ConfigEditor_proj
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
		}


		private void Test3_Closed(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
		}

		#region json tree

		public void refreshJsonItem()
		{
			string json = FileContoller.read(JSonInfo.current.Path);
			string filename = JSonInfo.current.Filename;
			// 삭제
			json_tree_view.Items.Clear();

			// 추가
			JToken jtok_root;
			jtok_root = JsonController.parseJson(json);
			if(jtok_root != null)
			{
				JsonTreeViewItem.convertToTreeView(json_tree_view, jtok_root, filename);
			}
			else
			{
				jtok_root = new JObject();
				JsonTreeViewItem.convertToTreeView(json_tree_view, jtok_root, filename);
			}
		}


		private void OnClickButtonNewJsonFile(object sender, RoutedEventArgs e)
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
			new JSonInfo();
			refreshJsonItem();
		}
		private void OnClickButtonOpenJsonFile(object sender, RoutedEventArgs e)
		{
			new JSonInfo();
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
		private void OnClickButtonSaveJsonFile(object sender, RoutedEventArgs e)
		{
			if(JSonInfo.current == null)
				return;

			if(JSonInfo.current.Path == null)
			{
				OnClickButtonOtherSaveJsonFile(sender, e);
				return;
			}

			FileInfo f = new FileInfo(JSonInfo.current.Path);
			if(!f.Exists)
			{
				OnClickButtonOtherSaveJsonFile(sender, e);
				return;
			}

			JSonInfo.current.Jtok_root = JsonTreeViewItem.convertToJToken(JSonInfo.current.Jtree_root);
			FileContoller.write(JSonInfo.current.Path, JSonInfo.current.Jtok_root.ToString());
		}
		private void OnClickButtonOtherSaveJsonFile(object sender, RoutedEventArgs e)
		{
			if(JSonInfo.current == null)
				return;

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
				JSonInfo.current.Jtok_root = JsonTreeViewItem.convertToJToken(JSonInfo.current.Jtree_root);
				JSonInfo.current.Path = sfd.FileName;
				FileContoller.write(sfd.FileName, JSonInfo.current.Jtok_root.ToString());
				//refreshJsonItem();
				//refreshJsonFile();
			}
		}
		private void OnClickButtonViewJsonFile(object sender, RoutedEventArgs e)
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
		private void OnClickButtonCancelJsonFile(object sender, RoutedEventArgs e)
		{
			if(JSonInfo.current == null || JSonInfo.current.Path == null)
				return;

			string dir_path = JSonInfo.current.Path.Substring(0, JSonInfo.current.Path.LastIndexOf('\\') + 1);
			DirectoryInfo d = new DirectoryInfo(dir_path);
			if(!d.Exists)
				return;

			refreshJsonItem();
		}

		#endregion
	}
	class JSonInfo
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

				JsonTreeViewItemHeader pan = this.Jtree_root.Header as JsonTreeViewItemHeader;
				if(pan == null)
					return;

				JsonTextBox tb = pan.Children[0] as JsonTextBox;
				if(tb == null)
					return;

				tb.Text = Filename;
			}
		}
		private JToken jtok_root;
		public JToken Jtok_root
		{
			get { return jtok_root; }
			set
			{
				jtok_root = value;
				isModify_jtok = true;
			}
		}
		private bool isModify_jtok = false;
		public bool IsModify_jtok { get { return isModify_jtok; } }

		private JsonTreeViewItem jtree_root;
		public JsonTreeViewItem Jtree_root
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
	class JsonTreeViewItem : TreeViewItem
	{
		public enum JsonType
		{
			JValue = 0,
			JObject = 1,
			JArray = 2
		}
		// 0 = JValue, 1 = JObject, 2 = JArray
		public JsonType type = 0;

		// JObject, JArray, JProperty
		// now
		//        if(root)    -> JObject
		//        else        ->JProperty
		public JToken linked_jtoken = null;
		//public JProperty linked_jtoken = null

		JsonToggleButton button_more = null;
		const int IDX_MOREBUTTON = 3;

		public new JsonTreeViewItemHeader Header { get { return base.Header as JsonTreeViewItemHeader; } set { base.Header = value; } }
		static int count_tvi;

		// ItemsControl = TreeView 와 MyTreeViewItem 의 상위 클래스 ItemsControl.Items 요소만 쓴다.
		public JsonTreeViewItem(JToken _linked_jtoken)
		{
			linked_jtoken = _linked_jtoken;

			this.Header = new JsonTreeViewItemHeader();
			this.Name = "tvi_" + count_tvi++;
			this.AllowDrop = true;
			this.IsExpanded = true;
		}

		public void Remove()
		{
			//Console.WriteLine(linked_jtoken.Type);
			JsonTreeViewItem parent_tvi;
			parent_tvi = this.Parent as JsonTreeViewItem;
			// this 가 루트이거나 다른 예외상황이면 삭제하지 않는다.
			if(parent_tvi == null)
				return;

			// JArray 삭제시 인덱스 수정
			parent_tvi.refreshIndex();

			// Json Tree Remove
			parent_tvi.Items.Remove(this);
			// Json Token Remove
			//this.linked_jtoken.Remove();
		}
		
		public int getCountChildProperty()
		{
			int cnt_property = 0;
			for(int i = 0; i < this.Items.Count; i++)
			{
				if(this.Items[i] is JsonTreeViewItem)
					cnt_property++;
			}
			return cnt_property;
		}

		public void visibleMore()
		{
			for(int i = 0; i < this.Items.Count; i++)
			{
				JsonTreeViewItem ui = this.Items[i] as JsonTreeViewItem;
				if(ui == null)
					continue;

				ui.Visibility = Visibility.Visible;
			}
		}
		public void hideMore()
		{
			for(int i = IDX_MOREBUTTON; i < this.Items.Count; i++)
			{
				JsonTreeViewItem ui = this.Items[i] as JsonTreeViewItem;
				if(ui == null)
					continue;

				ui.Visibility = Visibility.Collapsed;
			}
		}
		void refreshMoreButton()
		{
			// 루트의 자식들에는 버튼생성 하지 않는다.
			if(!(this.Parent is JsonTreeViewItem))
				return;

			int cnt_property = this.getCountChildProperty();
			// 정상적인 위치에 morebutton이 존재한다.
			if(cnt_property > IDX_MOREBUTTON && this.button_more != null && this.Items.IndexOf(button_more) == IDX_MOREBUTTON)
			{
				return;
			}

			if(cnt_property <= IDX_MOREBUTTON)
			{
				this.Items.Remove(button_more);
				button_more = null;
				visibleMore();
			}
			else
			{
				bool ischecked = false;

				if(this.button_more != null)
				{
					ischecked = button_more.IsChecked.Value;
					
					// 삭제
					this.Items.Remove(button_more);
					button_more = null;
					Console.WriteLine(ischecked);
				}

				// 추가
				this.button_more = new JsonToggleButton(ischecked);
				this.Items.Insert(IDX_MOREBUTTON, this.button_more);
				visibleMore();
				if(!ischecked)
					hideMore();
			}
		}
		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnItemsChanged(e);

			// items 의 add 작업중 more button 의 add 가 아닐때,
			if(e.NewItems != null && e.NewItems.IndexOf(this.button_more) < 0)
			{
				Console.WriteLine(e.Action.ToString());
				refreshMoreButton();
			}

			// items 의 remove 작업중 more button 의 remove 가 아닐때, 
			if(e.OldItems != null && e.OldItems.IndexOf(this.button_more) < 0)
			{
				Console.WriteLine(e.Action.ToString());
				refreshMoreButton();
			}

			//Console.WriteLine("new = " + e.NewStartingIndex + ", " + e.NewItems.Count);
			//Console.WriteLine(",old = " + e.OldStartingIndex + ", " + e.OldItems.Count);
			//Console.WriteLine(e.NewItems[e.NewStartingIndex].GetType());
		}
		#region code for moving MyTreeViewItem in screen
		// 외부에서 수정 불가 이벤트로만 수정
		static JsonTreeViewItem selected_tvi = null;
		public static JsonTreeViewItem Selectred_tvi { get { return selected_tvi; } }

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			Console.WriteLine("OnMouseLeftButtonDown()");
			PublicMouseLeftButtonDown(e);
		}
		public void PublicMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			selected_tvi = this;
			e.Handled = true;
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			PublicMouseMove(e);
		}
		public void PublicMouseMove(MouseEventArgs e)
		{
			//Console.WriteLine(linked_jtoken);
			base.OnMouseMove(e);
			if(e.LeftButton == MouseButtonState.Pressed
				&& JsonTreeViewItem.selected_tvi != null)
			{
				Console.WriteLine(this.Name + ", called DoDragDrop(" + JsonTreeViewItem.selected_tvi.Name + ")");
				DataObject data = new DataObject();
				data.SetData("Object", JsonTreeViewItem.selected_tvi);
				JsonTreeViewItem.selected_tvi = null;
				Console.WriteLine(this.GetType());
				DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
			}

			e.Handled = true;
		}
		//// 옮길 위치(dst)가 선택된(src) 타겟의 자손인지 체크하기위해 tunneling 으로 위부터 접근.
		//protected override void OnPreviewMouseMove(MouseEventArgs e)
		//{
		//	//Console.WriteLine(this.Name + ", OnMouseMove()");
		//	base.OnMouseMove(e);

		//	// 조상부터 자손까지 순서대로 터널링 수행. 선택된 타겟이 자손중 하나라면 이벤트 수행 X
		//	if(this == JsonTreeViewItem.selected_tvi)
		//		e.Handled = true;
		//}

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

			// bubble (자식부터 부모요소로 이벤트 전달 방식) 이므로 처음 수행할때 (제일 자식에서 이벤트를 받았을 때), 이벤트 수행처리.
			e.Handled = true;

			// If the DataObject contains string data, extract it.
			if(e.Data.GetDataPresent("Object"))
			{
				Object data_obj = (Object)e.Data.GetData("Object");

				JsonTreeViewItem tvi_src = data_obj as JsonTreeViewItem;
				if(tvi_src == null)
					return;

				// dst 의 조상중에 src 가 있나 체크 & dst == src 인지 체크
				JsonTreeViewItem tmp_parent = this;
				while(tmp_parent != null)
				{
					if(tmp_parent == tvi_src)
						return;

					tmp_parent = tmp_parent.Parent as JsonTreeViewItem;
				}

				JsonTreeViewItem tvi_parent = this.Parent as JsonTreeViewItem;
				JsonTreeViewItem tvi_src_parent = tvi_src.Parent as JsonTreeViewItem;
				if(tvi_parent == null || tvi_src_parent == null)
					return;

				// JArray 타입과 다른타입과는 드래그앤드롭 불가. (src==JArray && dst==JArray) 이거나 (src != JArray && dst==JArray) 일떄 수행
				if((tvi_parent.type == JsonType.JArray && tvi_src_parent.type != JsonType.JArray)
						|| (tvi_parent.type != JsonType.JArray && tvi_src_parent.type == JsonType.JArray))
					return;

				if(tvi_parent.type != JsonType.JArray && tvi_src_parent.type != JsonType.JArray
					&& tvi_parent != tvi_src_parent)
				{
					// 추가할 지점에 중복 체크
					KeyTextBox tb_src = tvi_src.Header.Children[0] as KeyTextBox;
					for(int i = 0; i < tvi_parent.Items.Count; i++)
					{
						JsonTreeViewItem child_tvi = tvi_parent.Items[i] as JsonTreeViewItem;
						if(child_tvi == null)
							continue;
						KeyTextBox tb = child_tvi.Header.Children[0] as KeyTextBox;
						if(tb == null)
							continue;

						// 키 중복
						if(tb.Text == tb_src.Text)
							return;
					}
				}

				// remove
				bool ischecked_more_button = false;
				if(tvi_parent.button_more != null)
					ischecked_more_button = tvi_parent.button_more.IsChecked.Value;

				int idx_insert = tvi_parent.Items.IndexOf(this);
				tvi_src.Remove();

				// insert
				tvi_parent.AddItem(tvi_src, idx_insert);
				if(tvi_parent.button_more != null)
					tvi_parent.button_more.IsChecked = ischecked_more_button;
			}
		}
		#endregion

		#region modify
		public static void convertToTreeView(TreeView treeView, JToken jtok_root, string filename)
		{
			if(jtok_root == null)
			{
				MessageBox.Show(JsonController.Error_message, "JSon Context Error");
				return;
			}

			JsonTreeViewItem jtree_root;

			List<string> key_stack = new List<string>();

			jtree_root = new JsonTreeViewItem(jtok_root);
			treeView.Items.Add(jtree_root);



			KeyTextBox tb_key = new KeyTextBox(filename, false);
			jtree_root.Header.AddItem(tb_key);

			JSonInfo.current.Jtok_root = jtok_root;
			JSonInfo.current.Jtree_root = jtree_root;
			convertToTreeView_recursive(jtree_root, jtok_root);
		}
		public static void convertToTreeView_recursive(JsonTreeViewItem parent_tvi, JToken jtok)
		{
			if(jtok == null)
				return;

			JsonTreeViewItem child_tvi = addToJsonTree(parent_tvi, jtok);

			foreach(var v in jtok.Children())
			{
				convertToTreeView_recursive(child_tvi, v);
			}
		}
		static JsonTreeViewItem addToJsonTree(JsonTreeViewItem parent_tvi, JToken jtok)
		{
			if(jtok is JObject)
			{
				JObject jobj = jtok as JObject;
				if(jobj.Parent == null)
					return parent_tvi;
				if(jobj.Parent != null && jobj.Parent is JArray)
				{
					JsonTreeViewItem child_tvi = new JsonTreeViewItem(jobj);
					parent_tvi.AddItem(child_tvi);
					KeyTextBox tb_key = new KeyTextBox((jobj.Parent as JArray).IndexOf(jobj).ToString(), false);
					child_tvi.Header.AddItem(tb_key);
					child_tvi.type = JsonType.JObject;
					parent_tvi = child_tvi;
				}

				KeyTextBox tb_type = new KeyTextBox("object", false);
				parent_tvi.Header.AddItem(tb_type);
				parent_tvi.type = JsonType.JObject;
				return parent_tvi;
			}
			else if(jtok is JProperty)
			{
				JProperty jprop = jtok as JProperty;
				JsonTreeViewItem child_tvi = new JsonTreeViewItem(jprop);
				parent_tvi.AddItem(child_tvi);

				KeyTextBox tb_key = new KeyTextBox(jprop.Name);
				child_tvi.Header.AddItem(tb_key);

				return child_tvi;
			}
			else if(jtok is JValue)
			{
				JValue jval = jtok as JValue;

				string data = jval.ToString();
				ValueTextBox tb_value = new ValueTextBox(data);
				parent_tvi.Header.AddItem(tb_value);

				parent_tvi.Header.addButtn.Visibility = Visibility.Hidden;
				//parent_tvi.Items.Clear();
				return parent_tvi;
			}
			else if(jtok is JArray)
			{
				KeyTextBox tb_type = new KeyTextBox("array", false);
				parent_tvi.Header.AddItem(tb_type);
				parent_tvi.type = JsonType.JArray;
				return parent_tvi;
			}
			else
			{
				Console.WriteLine("[error] undefined type = " + jtok.GetType());
				return parent_tvi;
			}
		}

		public void AddItem(UIElement item, int idx = -1)
		{
			//JsonTreeViewItem add_tvi = item as JsonTreeViewItem;
			//if(add_tvi != null)
			//{
			//	JsonTextBox add_tb = add_tvi.Header.Children[0] as JsonTextBox;
			//	if(add_tb != null)
			//	{
			//		// 중복 비교
			//		// JsonTreeViewItem.Header.children
			//		for(int i = 0; i < this.Items.Count; i++)
			//		{
			//			JsonTreeViewItem tvi = this.Items[i] as JsonTreeViewItem;
			//			if(tvi == null)
			//				continue;

			//			JsonTextBox tb = tvi.Header.Children[0] as JsonTextBox;
			//			if(tb == null)
			//				continue;
			//			// 키중복이면 삽입 x
			//			if(tb.Text == add_tb.Text)
			//				return;
			//		}
			//	}
			//}
			if(idx > -1 && idx < this.Items.Count)
				this.Items.Insert(idx, item);
			else
				this.Items.Add(item);

			visibleMore();
			if(this.button_more != null && !this.button_more.IsChecked.Value)
				hideMore();


			// JArray 삽입시 인덱스 수정
			refreshIndex();
		}
		void refreshIndex()
		{
			if(this.type != JsonType.JArray)
				return;
			
			//if(start_idx < 0 || start_new_idx < 0)
			//	return;

			// JArray 삽입시 인덱스 수정
			int cnt = this.Items.Count;
			for(int i = 0, newidx = 0; i < cnt; i++)
			{
				JsonTreeViewItem tvi = this.Items[i] as JsonTreeViewItem;
				if(tvi == null)
					continue;

				KeyTextBox tb = tvi.Header.Children[0] as KeyTextBox;
				if(tb == null)
					continue;

				tb.Text = newidx.ToString();
				newidx++;
			}
		}

		public static JObject convertToJToken(JsonTreeViewItem root_tvi)
		{
			if(root_tvi == null)
				return null;

			/*
                MyTreeViewItem.Items     --N-> MyTreeViewItem * N + Add_Button
                MyTreeViewItem.Header     --1-> JsonPanel_Header
                JsonPanel_Header            --3-> TextBox * 2 (key, val or val.type) + Delete_Button
                MyTreeViewItem.Items => MyTreeViewItem 타입의 자식개념, MyTreeViewItem.Header => JsonPanel 타입의 해당 property
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
		public static void convertToJToken_recursive(JsonTreeViewItem tvi_cur, JObject jobj_cur)
		{
			if(tvi_cur == null || jobj_cur == null)
				return;

			JToken cur_jtok = jobj_cur;
			// 여기서부터는 property 로 시작
			foreach(var v in tvi_cur.Items)
			{
				JsonTreeViewItem tvi_child = v as JsonTreeViewItem;
				if(tvi_child == null)
					continue;

				JsonTreeViewItemHeader pan = tvi_child.Header as JsonTreeViewItemHeader;
				if(pan == null)
					continue;

				// pan(MyTreeViewItem.Header) = key, val or val.type, button
				JsonTextBox tb_key = pan.Children[0] as JsonTextBox;
				JsonTextBox tb_val = pan.Children[1] as JsonTextBox;

				if(tb_val is KeyTextBox)
				{
					KeyTextBox tb_type = tb_val as KeyTextBox;
					switch(tvi_child.type)
					{
						case JsonTreeViewItem.JsonType.JObject:
							{
								JObject jobj = new JObject();
								JProperty jprop = new JProperty(tb_key.Text, jobj);
								(cur_jtok as JObject).Add(jprop);
								convertToJToken_recursive(tvi_child, jobj);
							}
							break;
						case JsonTreeViewItem.JsonType.JArray:
							{
								JArray jarr = new JArray();
								JProperty jprop = new JProperty(tb_key.Text, jarr);
								(cur_jtok as JObject).Add(jprop);
								for(int i = 0; i < tvi_child.Items.Count; i++)
								{
									JObject jobj = new JObject();
									jarr.Add(jobj);
									convertToJToken_recursive(tvi_child.Items[i] as JsonTreeViewItem, jobj);
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
	}
	class JsonTreeViewItemHeader : StackPanel
	{
		public JsonAddButton addButtn;
		public JsonDeleteButton deleteButton;
		public JsonTreeViewItemHeader()
		{
			this.Height = 30;
			this.Orientation = Orientation.Horizontal;

			deleteButton = new JsonDeleteButton();
			AddItem(deleteButton);

			addButtn = new JsonAddButton();
			AddItem(addButtn);

		}
		public void AddItem(UIElement item)
		{
			int idx_insert = this.Children.Count;
			while(idx_insert > 0 && !(this.Children[idx_insert - 1] is JsonTextBox))
				idx_insert--;

			this.Children.Insert(idx_insert, item);
		}
	}
	class JsonButton : Button
	{
		public JsonButton()
		{
			this.BorderBrush = Brushes.Black;
			this.Margin = new Thickness(2.5);
			this.Width = 20;
			this.Height = 20;
			this.VerticalContentAlignment = VerticalAlignment.Center;
			this.HorizontalContentAlignment = HorizontalAlignment.Center;
		}
	}
	class JsonAddButton : JsonButton
	{
		public JsonAddButton()
		{
			this.Content = '+';
			this.Background = Brushes.LightGreen;

			// Json 버튼은 JsonTreeViewItem.Header.children 에 존재
			this.Click += delegate (object sender, RoutedEventArgs e)
			{
				JsonTreeViewItemHeader jtvih = this.Parent as JsonTreeViewItemHeader;
				if(jtvih == null)
					return;
				JsonTreeViewItem jtvi = jtvih.Parent as JsonTreeViewItem;
				if(jtvi == null)
					return;

				JsonButton button = sender as JsonButton;
				//tvi.Add(sender as JsonButton, convertToTreeView_recursive);
				//FileContoller.write(JSonInfo.current.filename, JSonInfo.current.jtok_root.ToString());
			};
		}
		// Json 버튼은 JsonTreeViewItem.Header.children 에 존재
		protected override void OnClick()
		{
			base.OnClick();
			JsonTreeViewItemHeader jtvih = this.Parent as JsonTreeViewItemHeader;
			if(jtvih == null)
				return;
			JsonTreeViewItem jtvi = jtvih.Parent as JsonTreeViewItem;
			if(jtvi == null)
				return;

			if(jtvi.type == JsonTreeViewItem.JsonType.JArray)
			{
				JsonTreeViewItem child_tvi = new JsonTreeViewItem(null);
				KeyTextBox tb_key = new KeyTextBox(jtvi.getCountChildProperty().ToString(), false);
				child_tvi.Header.AddItem(tb_key);
				KeyTextBox tb_type = new KeyTextBox("object", false);
				child_tvi.Header.AddItem(tb_type);
				child_tvi.type = JsonTreeViewItem.JsonType.JObject;

				jtvi.AddItem(child_tvi);
			}
			else
			{
				// window_addJson showdialog()
				popup_AddJsonItem popup = new popup_AddJsonItem();
				Point pt = this.PointToScreen(new Point(0, 0));
				popup.Left = pt.X;
				popup.Top = pt.Y;

				// cancel return
				if(popup.ShowDialog() != true)
					return;

				for(int i = 0; i < jtvi.Items.Count; i++)
				{
					JsonTreeViewItem child_tvi = jtvi.Items[i] as JsonTreeViewItem;
					if(child_tvi == null)
						continue;
					KeyTextBox tb = child_tvi.Header.Children[0] as KeyTextBox;
					if(tb == null)
						continue;

					// 키 중복
					if(tb.Text == popup.key)
						return;
				}

				JsonTreeViewItem.convertToTreeView_recursive(jtvi, new JProperty(popup.key, popup.value));
			}

			//JToken tmp = jtvi.Parent;
			//// tmp == root 일때
			//if(tmp is JObject)
			//	;
			//else
			//{
			//	JProperty jprop = tmp as JProperty;
			//	tmp = jprop.Value;
			//}
			//JToken add_jtok = null;
			//if(tmp is JArray)
			//{
			//	JArray jarr = tmp as JArray;
			//	// add Jtok_root
			//	add_jtok = new JObject();
			//	jarr.Add(add_jtok);
			//}
			//else if(tmp is JObject)
			//{
			//	JObject jobj = tmp as JObject;
			//	// window_addJson showdialog()
			//	popup_AddJsonItem popup = new popup_AddJsonItem();
			//	Point pt = sender.PointToScreen(new Point(0, 0));
			//	popup.Left = pt.X;
			//	popup.Top = pt.Y;

			//	// cancel return
			//	if(popup.ShowDialog() != true)
			//		return;

			//	foreach(JProperty v in jobj.Properties())
			//	{
			//		// 키 중복
			//		if(v.Name == popup.key)
			//			return;
			//	}

			//	// ok return & add Jtok_root
			//	add_jtok = new JProperty(popup.key, popup.value);
			//	jobj.Add(add_jtok);

			//}

			//// add to jtree_root
			//if(add_jtok != null)
			//	makeTree(this, add_jtok);
			////tvi.Add(sender as JsonButton, convertToTreeView_recursive);
			////FileContoller.write(JSonInfo.current.filename, JSonInfo.current.jtok_root.ToString());
		}
	}
	class JsonDeleteButton : JsonButton
	{
		public JsonDeleteButton()
		{
			this.Content = '-';
			this.Background = Brushes.LightPink;
			this.BorderBrush = Brushes.Black;
			this.Margin = new Thickness(2.5);
			this.Width = 20;
			this.Height = 20;
			this.VerticalContentAlignment = VerticalAlignment.Center;
			this.HorizontalContentAlignment = HorizontalAlignment.Center;
		}

		// Json 버튼은 JsonTreeViewItem.Header.children 에 존재
		protected override void OnClick()
		{
			base.OnClick();
			JsonTreeViewItemHeader jtvih = this.Parent as JsonTreeViewItemHeader;
			if(jtvih == null)
				return;
			JsonTreeViewItem jtvi = jtvih.Parent as JsonTreeViewItem;
			if(jtvi == null)
				return;

			jtvi.Remove();
			//FileContoller.write(JsonInfo.current.filename, JsonInfo.current.jtok_root.ToString());
		}
	}
	class JsonToggleButton : ToggleButton
	{
		public JsonToggleButton(bool _ischecked = false)
		{
			this.Content = "more..";
			this.Background = Brushes.White;
			this.BorderBrush = null;
			this.IsChecked = _ischecked;
			//this.Visibility = Visibility.Collapsed;
		}
		protected override void OnUnchecked(RoutedEventArgs e)
		{
			// this 는 JsonTreeViewItem.Children 에 위치
			JsonTreeViewItem tvi = this.Parent as JsonTreeViewItem;
			if(tvi == null)
				return;
			base.OnUnchecked(e);

			tvi.hideMore();
		}
		protected override void OnChecked(RoutedEventArgs e)
		{
			// this 는 JsonTreeViewItem.Children 에 위치
			JsonTreeViewItem tvi = this.Parent as JsonTreeViewItem;
			if(tvi == null)
				return;

			tvi.visibleMore();
		}
	}
	class JsonTextBox : TextBox
	{
		public JsonTextBox()
		{
			this.AllowDrop = true;

			this.Margin = new Thickness(5);
			this.Width = 150;
		}

		// JsonTextBox 는 JsonTreeViewItem.Header.Children 에 존재
		protected override void OnDragOver(DragEventArgs e)
		{
			JsonTreeViewItem tvi_parent = this.Parent as JsonTreeViewItem;
			if(tvi_parent == null)
				return;

			tvi_parent.PublicDragOver(e);
			Console.WriteLine("KeyTextBox.OnDragOver()");
			base.OnDragOver(e);
		}
		protected override void OnDrop(DragEventArgs e)
		{
			Console.WriteLine("KeyTextBox.OnDrop()");
			JsonTreeViewItem tvi_parent = this.Parent as JsonTreeViewItem;
			if(tvi_parent == null)
				return;

			tvi_parent.PublicDrop(e);
			base.OnDrop(e);
		}
		//protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
		//{
		//	Console.WriteLine("OnMouseLeftButtonDown");
		//	base.OnPreviewMouseLeftButtonDown(e);
		//}
		//protected override void OnDragLeave(DragEventArgs e)
		//{
		//	Console.WriteLine("OnDragLeave");
		//	base.OnDragLeave(e);
		//}
		//protected override void OnMouseLeave(MouseEventArgs e)
		//{
		//	Console.WriteLine("OnMouseLeave");
		//	base.OnMouseLeave(e);
		//}
		//protected override void OnTouchLeave(TouchEventArgs e)
		//{
		//	Console.WriteLine("OnTouchLeave");
		//	base.OnTouchLeave(e);
		//}
		//protected override void OnStylusLeave(StylusEventArgs e)
		//{
		//	Console.WriteLine("OnStylusLeave");
		//	base.OnStylusLeave(e);
		//}
		//protected override void OnPreviewMouseMove(MouseEventArgs e)
		//{
		//	base.OnMouseMove(e);

		//	//JsonTreeViewItemHeader header = this.Parent as JsonTreeViewItemHeader;
		//	//if(header == null)
		//	//	return;
		//	//JsonTreeViewItem tvi = header.Parent as JsonTreeViewItem;
		//	//if(tvi == null)
		//	//	return;

		//	//tvi.PublicMouseMove(e);
		//	//Console.WriteLine("KeyTextBox.OnPreviewMouseMove()");
		//}
	}
	class KeyTextBox : JsonTextBox
	{
		public KeyTextBox(string text, bool isModify = true)
		{
			this.Text = text;

			if(!isModify)
			{
				this.IsEnabled = false;
				this.BorderBrush = null;
			}
		}
	}
	class ValueTextBox : JsonTextBox
	{
		public ValueTextBox(string data)
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
		}
	}
}