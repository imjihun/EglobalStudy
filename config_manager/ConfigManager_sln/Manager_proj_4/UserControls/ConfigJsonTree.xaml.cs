﻿using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using Manager_proj_4.Classes;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Manager_proj_4.UserControls
{
	/// <summary>
	/// ConfigJsonTree.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ConfigJsonTree : UserControl
	{
		public ConfigJsonTree()
		{
			InitializeComponent();
			InitJsonFileView();
		}

		#region Json Tree Area
		static string DIR = @"config\";
		public static string root_path = AppDomain.CurrentDomain.BaseDirectory + DIR;

		void InitJsonFileView()
		{
			Directory.CreateDirectory(root_path);
			new JsonInfo(json_tree_view);
		}
		public void refreshJsonTree(JToken jtok_root)
		{
			// 변환
			JsonTreeViewItem root_jtree = JsonTreeViewItem.convertToTreeViewItem(jtok_root);
			if(root_jtree == null)
				return;

			// 삭제
			json_tree_view.Items.Clear();

			// 추가
			//TextBlock tblock = new TextBlock();
			//tblock.Text = JsonInfo.current.Filename;
			//root_jtree.Header.Children.Insert(0, tblock);
			//json_tree_view.Items.Add(root_jtree);

			Label label = new Label();
			label.VerticalAlignment = VerticalAlignment.Center;
			label.Content = JsonInfo.current.Filename;
			root_jtree.Header.Children.Insert(0, label);
			json_tree_view.Items.Add(root_jtree);
		}

		private void OnClickButtonNewJsonFile(object sender, RoutedEventArgs e)
		{
			if(JsonInfo.current == null)
				return;

			if(JsonInfo.current.Path != null)
				;

			JsonInfo.current.Clear();

			WindowMain.current.ShowMessageDialog("New", "새로만드시겠습니까?", MessageDialogStyle.AffirmativeAndNegative, NewJsonFile);
		}
		private void NewJsonFile()
		{

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
			if(ofd.ShowDialog() == true)
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

			if(!CheckJson())
				return;

			if(JsonInfo.current.Path == null)
			{
				OtherSaveJsonFile();
				return;
			}

			FileInfo f = new FileInfo(JsonInfo.current.Path);
			if(!f.Exists)
			{
				OtherSaveJsonFile();
				return;
			}

			WindowMain.current.ShowMessageDialog("Save", "저장하시겠습니까?", MessageDialogStyle.AffirmativeAndNegative, SaveJsonFile);
		}
		private void SaveJsonFile()
		{
			SaveFile(JsonInfo.current.Path);
		}
		private void OnClickButtonOtherSaveJsonFile(object sender, RoutedEventArgs e)
		{
			if(JsonInfo.current == null)
				return;

			if(!CheckJson())
				return;

			OtherSaveJsonFile();
		}
		private void OtherSaveJsonFile()
		{
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
			if(sfd.ShowDialog() == true)
			{
				SaveFile(sfd.FileName);
				JsonInfo.current.Path = sfd.FileName;
			}
		}
		private bool CheckJson()
		{
			// 로드된 오브젝트가 없으면 실행 x
			if(json_tree_view.Items.Count < 1)
				return false;
			JsonTreeViewItem root = json_tree_view.Items[0] as JsonTreeViewItem;
			if(root == null)
				return false;

			return true;
		}
		private void SaveFile(string path)
		{
			if(!CheckJson())
				return;

			JToken Jtok_root = JsonTreeViewItem.convertToJToken(json_tree_view.Items[0] as JsonTreeViewItem);
			if(Jtok_root != null && FileContoller.write(path, Jtok_root.ToString()))
				WindowMain.current.ShowMessageDialog("Save", path + " 파일이 저장되었습니다.");
			else
			{
				string caption = "Save Error";
				string message = path + " 파일을 저장하는데 문제가 생겼습니다.";
				WindowMain.current.ShowMessageDialog(caption, message);
				Console.WriteLine("[" + caption + "] " + message);
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

			WindowMain.current.ShowMessageDialog("Cancel", "변경사항을 되돌리시겠습니까?", MessageDialogStyle.AffirmativeAndNegative, CalcelJsonFile);
		}
		private void CalcelJsonFile()
		{
			string json = FileContoller.read(JsonInfo.current.path);
			refreshJsonTree(JsonController.parseJson(json));
			WindowMain.current.ShowMessageDialog("Cancel", "변경사항을 되돌렸습니다.", MessageDialogStyle.Affirmative);
		}
		#endregion

	}


	//class JsonController
	//{
	//	static string error_message = "";
	//	public static string Error_message { get { return error_message; } }
	//	public static JToken parseJson(string json)
	//	{
	//		if(json == null)
	//			return null;
	//		JToken obj = null;
	//		try
	//		{
	//			obj = JToken.Parse(json);
	//			//Log.Print(obj);
	//		}
	//		catch(Exception e)
	//		{
	//			error_message = e.Message;
	//			Log.Print(e.Message);
	//		}

	//		return obj;
	//	}
	//}
	//class JsonInfo
	//{
	//	public static JsonInfo current = null;
	//	public string path;
	//	public string Path
	//	{
	//		get { return path; }
	//		set
	//		{
	//			path = value;

	//			// filename = treeView.Items[0].Header.Children[0].Text
	//			if(treeView == null)
	//				return;

	//			if(treeView.Items.Count < 1)
	//				return;

	//			JsonTreeViewItem tvi = treeView.Items[0] as JsonTreeViewItem;
	//			if(tvi == null)
	//				return;

	//			//JsonTextBox tb = tvi.Header.Children[0] as JsonTextBox;
	//			//if(tb == null)
	//			//	return;
	//			//tb.Text = Filename;

	//			Label label = tvi.Header.Children[0] as Label;
	//			if(label == null)
	//				return;
	//			label.Content = Filename;
	//		}
	//	}
	//	//private JToken jtok_root;
	//	//public JToken Jtok_root
	//	//{
	//	//	get { return jtok_root; }
	//	//	set
	//	//	{
	//	//		jtok_root = value;
	//	//		isModify_jtok = true;
	//	//	}
	//	//}
	//	//private bool isModify_jtok = false;
	//	//public bool IsModify_jtok { get { return isModify_jtok; } }

	//	//private JsonTreeViewItem jtree_root;
	//	//public JsonTreeViewItem Jtree_root
	//	//{
	//	//	get { return jtree_root; }
	//	//	set
	//	//	{
	//	//		jtree_root = value;
	//	//		isModify_tree = true;
	//	//	}
	//	//}
	//	//private bool isModify_tree = false;
	//	//public bool IsModify_tree { get { return isModify_tree; } }

	//	TreeView treeView = null;
	//	public JsonInfo(TreeView tv)
	//	{
	//		current = this;
	//		treeView = tv;
	//	}

	//	public void Clear()
	//	{
	//		Path = null;
	//		//Jtok_root = null;
	//		//Jtree_root = null;
	//	}

	//	public string Filename
	//	{
	//		get
	//		{
	//			if(Path != null)
	//			{
	//				string[] split = Path.Split('\\');
	//				return split[split.Length - 1];
	//			}
	//			return null;
	//		}
	//	}
	//}
	
	//#region Override Class UI

	//class Size
	//{
	//	public const int HEIGHT_HEADER = 40;
	//	//public const int WIDTH = 30;

	//	public const int MARGIN_TEXTBOX = 5;
	//	public const int WIDTH_TEXTBOX = 150;
	//	//public const int HEIGHT_TEXTBOX = 25;

	//	public const double MARGIN_BUTTON = 2.5;
	//	public const int HEIGHT_BUTTON = 30;
	//	public const int WIDTH_BUTTON = 30;

	//	public const int HEIGHT_MOREBUTTON = 30;
	//	public const int WIDTH_MOREBUTTON = 35;
	//}
	//class JsonTreeViewItem : TreeViewItem
	//{
	//	//public enum JTokenType
	//	//{
	//	//	String = 0,
	//	//	Boolean,
	//	//	Null,
	//	//	Integer,
	//	//	JObject,
	//	//	JArray,
	//	//}
	//	//// 0 = JValue, 1 = JObject, 2 = JArray

	//	public JTokenType value_token_type = 0;
	//	//public JTokenType type;
	//	//// JObject, JArray, JProperty
	//	//// now
	//	////        if(root)    -> JObject
	//	////        else        ->JProperty
	//	//public JProperty linked_jtoken = null

	//	JsonToggleButton button_more = null;
	//	const int IDX_MOREBUTTON = 3;

	//	public new JsonTreeViewItemHeader Header { get { return base.Header as JsonTreeViewItemHeader; } set { base.Header = value; } }
	//	static int count_tvi;

	//	// ItemsControl = TreeView 와 MyTreeViewItem 의 상위 클래스 ItemsControl.Items 요소만 쓴다.
	//	public JsonTreeViewItem()
	//	{
	//		this.Header = new JsonTreeViewItemHeader();
	//		this.Name = "tvi_" + count_tvi++;
	//		this.AllowDrop = true;
	//		this.IsExpanded = true;
	//	}

	//	public void Remove()
	//	{
	//		JsonTreeViewItem parent_tvi;
	//		parent_tvi = this.Parent as JsonTreeViewItem;
	//		// this 가 루트이면 다른 방법으로 삭제한다.
	//		if(parent_tvi == null)
	//		{
	//			this.Items.Clear();
	//			return;
	//		}

	//		// Json Tree Remove
	//		parent_tvi.Items.Remove(this);

	//		// JArray 삭제시 인덱스 수정
	//		if(this.value_token_type == JTokenType.Array)
	//			parent_tvi.refreshIndex();
	//	}

	//	public int getCountChildProperty()
	//	{
	//		// 자식중에 more... 버튼이 있을수도 없을수도 있기 때문에 실제 자식을 리턴 받고 싶을때
	//		int cnt_property = 0;
	//		for(int i = 0; i < this.Items.Count; i++)
	//		{
	//			if(this.Items[i] is JsonTreeViewItem)
	//				cnt_property++;
	//		}
	//		return cnt_property;
	//	}
	//	public void visibleMore()
	//	{
	//		for(int i = 0; i < this.Items.Count; i++)
	//		{
	//			JsonTreeViewItem ui = this.Items[i] as JsonTreeViewItem;
	//			if(ui == null)
	//				continue;

	//			ui.Visibility = Visibility.Visible;
	//		}
	//	}
	//	public void hideMore()
	//	{
	//		for(int i = IDX_MOREBUTTON; i < this.Items.Count; i++)
	//		{
	//			JsonTreeViewItem ui = this.Items[i] as JsonTreeViewItem;
	//			if(ui == null)
	//				continue;

	//			ui.Visibility = Visibility.Collapsed;
	//		}
	//	}
	//	protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
	//	{
	//		base.OnItemsChanged(e);

	//		// items 의 add 작업중 more button 의 add 가 아닐때,
	//		if(e.NewItems != null && e.NewItems.IndexOf(this.button_more) < 0)
	//		{
	//			//Log.Print(e.Action.ToString());
	//			refreshMoreButton();
	//		}

	//		// items 의 remove 작업중 more button 의 remove 가 아닐때, 
	//		if(e.OldItems != null && e.OldItems.IndexOf(this.button_more) < 0)
	//		{
	//			//Log.Print(e.Action.ToString());
	//			refreshMoreButton();
	//		}

	//		//Log.Print("new = " + e.NewStartingIndex + ", " + e.NewItems.Count);
	//		//Log.Print(",old = " + e.OldStartingIndex + ", " + e.OldItems.Count);
	//		//Log.Print(e.NewItems[e.NewStartingIndex].GetType());
	//	}
	//	void refreshMoreButton()
	//	{
	//		// 루트의 자식들에는 버튼생성 하지 않는다.
	//		if(!(this.Parent is JsonTreeViewItem))
	//			return;

	//		int cnt_property = this.getCountChildProperty();
	//		// 정상적인 위치에 morebutton이 존재한다.
	//		if(cnt_property > IDX_MOREBUTTON && this.button_more != null && this.Items.IndexOf(button_more) == IDX_MOREBUTTON)
	//		{
	//			return;
	//		}

	//		if(cnt_property <= IDX_MOREBUTTON)
	//		{
	//			this.Items.Remove(button_more);
	//			button_more = null;
	//			visibleMore();
	//		}
	//		else
	//		{
	//			bool ischecked = false;

	//			if(this.button_more != null)
	//			{
	//				ischecked = button_more.IsChecked.Value;

	//				// 삭제
	//				this.Items.Remove(button_more);
	//				button_more = null;
	//				Log.Print(ischecked.ToString());
	//			}

	//			// 추가
	//			this.button_more = new JsonToggleButton(ischecked);
	//			this.Items.Insert(IDX_MOREBUTTON, this.button_more);
	//			visibleMore();
	//			if(!ischecked)
	//				hideMore();
	//		}
	//	}

	//	#region event code for moving MyTreeViewItem in screen
	//	// 외부에서 수정 불가 이벤트로만 수정
	//	static JsonTreeViewItem selected_tvi = null;
	//	static JsonTreeViewItem Selectred_tvi { get { return selected_tvi; } }

	//	protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
	//	{
	//		Log.Print("OnMouseLeftButtonDown()");
	//		PublicMouseLeftButtonDown(e);
	//	}
	//	public void PublicMouseLeftButtonDown(MouseButtonEventArgs e)
	//	{
	//		base.OnMouseLeftButtonDown(e);
	//		selected_tvi = this;
	//		e.Handled = true;
	//	}
	//	protected override void OnMouseMove(MouseEventArgs e)
	//	{
	//		PublicMouseMove(e);
	//	}
	//	public void PublicMouseMove(MouseEventArgs e)
	//	{
	//		//Log.Print(linked_jtoken);
	//		base.OnMouseMove(e);
	//		if(e.LeftButton == MouseButtonState.Pressed
	//			&& JsonTreeViewItem.selected_tvi != null)
	//		{
	//			Log.Print(this.Name + ", called DoDragDrop(" + JsonTreeViewItem.selected_tvi.Name + ")");
	//			DataObject data = new DataObject();
	//			data.SetData("Object", JsonTreeViewItem.selected_tvi);
	//			JsonTreeViewItem.selected_tvi = null;
	//			Log.Print(this.GetType().ToString());
	//			DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
	//		}

	//		e.Handled = true;
	//	}
	//	//// 옮길 위치(dst)가 선택된(src) 타겟의 자손인지 체크하기위해 tunneling 으로 위부터 접근.
	//	//protected override void OnPreviewMouseMove(MouseEventArgs e)
	//	//{
	//	//	//Log.Print(this.Name + ", OnMouseMove()");
	//	//	base.OnMouseMove(e);

	//	//	// 조상부터 자손까지 순서대로 터널링 수행. 선택된 타겟이 자손중 하나라면 이벤트 수행 X
	//	//	if(this == JsonTreeViewItem.selected_tvi)
	//	//		e.Handled = true;
	//	//}

	//	protected override void OnDragOver(DragEventArgs e)
	//	{
	//		PublicDragOver(e);
	//	}
	//	public void PublicDragOver(DragEventArgs e)
	//	{
	//		this.IsSelected = true;
	//		e.Handled = true;
	//	}
	//	protected override void OnDrop(DragEventArgs e)
	//	{
	//		PublicDrop(e);
	//	}
	//	public void PublicDrop(DragEventArgs e)
	//	{
	//		base.OnDrop(e);

	//		// bubble (자식부터 부모요소로 이벤트 전달 방식) 이므로 처음 수행할때 (제일 자식에서 이벤트를 받았을 때), 이벤트 수행처리.
	//		e.Handled = true;

	//		// If the DataObject contains string data, extract it.
	//		if(e.Data.GetDataPresent("Object"))
	//		{
	//			Object data_obj = (Object)e.Data.GetData("Object");

	//			JsonTreeViewItem tvi_src = data_obj as JsonTreeViewItem;
	//			if(tvi_src == null)
	//				return;

	//			// dst 의 조상중에 src 가 있나 체크 & dst == src 인지 체크
	//			JsonTreeViewItem tmp_parent = this;
	//			while(tmp_parent != null)
	//			{
	//				if(tmp_parent == tvi_src)
	//					return;

	//				tmp_parent = tmp_parent.Parent as JsonTreeViewItem;
	//			}

	//			JsonTreeViewItem tvi_parent = this.Parent as JsonTreeViewItem;
	//			JsonTreeViewItem tvi_src_parent = tvi_src.Parent as JsonTreeViewItem;
	//			if(tvi_parent == null || tvi_src_parent == null)
	//				return;

	//			// JArray 타입과 다른타입과는 드래그앤드롭 불가. (src==JArray && dst==JArray) 이거나 (src != JArray && dst==JArray) 일떄 수행
	//			if((tvi_parent.value_token_type == JTokenType.Array && tvi_src_parent.value_token_type != JTokenType.Array)
	//					|| (tvi_parent.value_token_type != JTokenType.Array && tvi_src_parent.value_token_type == JTokenType.Array))
	//				return;

	//			// 중복 체크
	//			if(tvi_parent.value_token_type != JTokenType.Array && tvi_src_parent.value_token_type != JTokenType.Array
	//				&& tvi_parent != tvi_src_parent
	//				&& tvi_parent.checkDuplication(tvi_src) < 0)
	//				return;

	//			// remove
	//			bool ischecked_more_button = false;
	//			if(tvi_parent.button_more != null)
	//				ischecked_more_button = tvi_parent.button_more.IsChecked.Value;

	//			int idx_insert = tvi_parent.Items.IndexOf(this);
	//			tvi_src.Remove();

	//			// insert
	//			tvi_parent.AddItem(tvi_src, idx_insert);
	//			if(tvi_parent.button_more != null)
	//				tvi_parent.button_more.IsChecked = ischecked_more_button;
	//		}
	//	}
	//	#endregion

	//	#region modify
	//	public static JsonTreeViewItem convertToTreeViewItem(JToken jtok_root)
	//	{
	//		if(jtok_root == null)
	//		{
	//			WindowMain.current.ShowMessageDialog("Json Context Error!!", JsonController.Error_message, MahApps.Metro.Controls.Dialogs.MessageDialogStyle.Affirmative);
	//			//MessageBox.Show(JsonController.Error_message, "JSon Context Error");
	//			return null;
	//		}
	//		// object 로 시작.
	//		JsonTreeViewItem jtree_root = new JsonTreeViewItem();

	//		//JsonInfo.current.Jtok_root = jtok_root;
	//		//JsonInfo.current.Jtree_root = jtree_root;
	//		convertToTreeView_recursive(jtree_root, jtok_root);
	//		return jtree_root;
	//	}
	//	//public static void convertToTreeViewItem(TreeView treeView, JToken jtok_root, string filename)
	//	//{
	//	//	if(jtok_root == null)
	//	//	{
	//	//		MessageBox.Show(JsonController.Error_message, "JSon Context Error");
	//	//		return;
	//	//	}

	//	//	JsonTreeViewItem jtree_root;

	//	//	List<string> key_stack = new List<string>();

	//	//	jtree_root = new JsonTreeViewItem(jtok_root);
	//	//	treeView.Items.Add(jtree_root);



	//	//	KeyTextBox tb_key = new KeyTextBox(filename, false);
	//	//	jtree_root.Header.AddItem(tb_key);

	//	//	JSonInfo.current.Jtok_root = jtok_root;
	//	//	JSonInfo.current.Jtree_root = jtree_root;
	//	//	convertToTreeView_recursive(jtree_root, jtok_root);
	//	//}
	//	public static void convertToTreeView_recursive(JsonTreeViewItem parent_tvi, JToken jtok)
	//	{
	//		if(jtok == null)
	//			return;

	//		JsonTreeViewItem child_tvi = addUIToJsonTreeItem(parent_tvi, jtok);

	//		foreach(var v in jtok.Children())
	//		{
	//			convertToTreeView_recursive(child_tvi, v);
	//		}
	//	}
	//	static JsonTreeViewItem addUIToJsonTreeItem(JsonTreeViewItem parent_tvi, JToken jtok)
	//	{
	//		if(jtok is JObject)
	//		{
	//			JObject jobj = jtok as JObject;
	//			if(jobj.Parent == null)
	//				return parent_tvi;
	//			if(jobj.Parent != null && jobj.Parent is JArray)
	//			{
	//				JsonTreeViewItem child_tvi = new JsonTreeViewItem();
	//				parent_tvi.AddItem(child_tvi);
	//				KeyTextBox tb_key = new KeyTextBox((jobj.Parent as JArray).IndexOf(jobj).ToString(), false);
	//				child_tvi.Header.AddItem(tb_key);
	//				child_tvi.value_token_type = JTokenType.Object;
	//				parent_tvi = child_tvi;
	//			}

	//			//KeyTextBox tb_type = new KeyTextBox("object", false);
	//			//parent_tvi.Header.AddItem(tb_type);
	//			//parent_tvi.value_token_type = JTokenType.Object;
	//			//return parent_tvi;
	//			ValuePanel panel_value = new ValuePanel(JTokenType.Object, "Object");
	//			parent_tvi.Header.AddItem(panel_value);
	//			parent_tvi.value_token_type = JTokenType.Object;
	//			return parent_tvi;
	//		}
	//		else if(jtok is JProperty)
	//		{
	//			JProperty jprop = jtok as JProperty;
	//			JsonTreeViewItem child_tvi = new JsonTreeViewItem();
	//			parent_tvi.AddItem(child_tvi);

	//			KeyTextBox tb_key = new KeyTextBox(jprop.Name);
	//			child_tvi.Header.AddItem(tb_key);

	//			return child_tvi;
	//		}
	//		else if(jtok is JValue)
	//		{
	//			JValue jval = jtok as JValue;
	//			ValuePanel panel_value = new ValuePanel(jval.Type, jval.Value);
	//			parent_tvi.Header.AddItem(panel_value);
	//			parent_tvi.value_token_type = jval.Type;

	//			parent_tvi.Header.addButtn.Visibility = Visibility.Hidden;
	//			//parent_tvi.Items.Clear();
	//			return parent_tvi;
	//		}
	//		else if(jtok is JArray)
	//		{
	//			//KeyTextBox tb_type = new KeyTextBox("array", false);
	//			//parent_tvi.Header.AddItem(tb_type);
	//			//parent_tvi.value_token_type = JTokenType.Array;
	//			//return parent_tvi;
	//			ValuePanel panel_value = new ValuePanel(JTokenType.Array, "Array");
	//			parent_tvi.Header.AddItem(panel_value);
	//			parent_tvi.value_token_type = JTokenType.Array;
	//			return parent_tvi;
	//		}
	//		else
	//		{
	//			Log.Print("[error] undefined type = " + jtok.GetType());
	//			return parent_tvi;
	//		}
	//	}

	//	int checkDuplication(JsonTreeViewItem add_tvi)
	//	{
	//		if(add_tvi == null)
	//			return 1;

	//		JsonTextBox add_tb = add_tvi.Header.Children[0] as JsonTextBox;
	//		if(add_tb == null)
	//			return 1;

	//		// 중복 비교
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
	//				return -1;
	//		}
	//		return 0;
	//	}
	//	public int AddItem(UIElement item, int idx = -1)
	//	{
	//		// 키중복 체크
	//		if(checkDuplication(item as JsonTreeViewItem) < 0)
	//			return -1;

	//		if(idx > -1 && idx < this.Items.Count)
	//			this.Items.Insert(idx, item);
	//		else
	//			this.Items.Add(item);

	//		visibleMore();
	//		if(this.button_more != null && !this.button_more.IsChecked.Value)
	//			hideMore();


	//		// JArray 삽입시 인덱스 수정
	//		if(this.value_token_type == JTokenType.Array)
	//			refreshIndex();
	//		return 0;
	//	}
	//	void refreshIndex()
	//	{
	//		if(this.value_token_type != JTokenType.Array)
	//			return;

	//		//if(start_idx < 0 || start_new_idx < 0)
	//		//	return;

	//		// JArray 삽입시 인덱스 수정
	//		int cnt = this.Items.Count;
	//		for(int i = 0, newidx = 0; i < cnt; i++)
	//		{
	//			JsonTreeViewItem tvi = this.Items[i] as JsonTreeViewItem;
	//			if(tvi == null)
	//				continue;

	//			KeyTextBox tb = tvi.Header.Children[0] as KeyTextBox;
	//			if(tb == null)
	//				continue;

	//			tb.Text = newidx.ToString();
	//			newidx++;
	//		}
	//	}

	//	public static JObject convertToJToken(JsonTreeViewItem root_tvi)
	//	{
	//		if(root_tvi == null)
	//			return null;

	//		/*
	//               MyTreeViewItem.Items     --N-> MyTreeViewItem * N + Add_Button
	//               MyTreeViewItem.Header     --1-> JsonPanel_Header
	//               JsonPanel_Header            --3-> TextBox * 2 (key, val or val.type) + Delete_Button
	//               MyTreeViewItem.Items => MyTreeViewItem 타입의 자식개념, MyTreeViewItem.Header => JsonPanel 타입의 해당 property
	//           */
	//		/*
	//                JToken 상위 클래스
	//               JObject         --N-> JProperty
	//               JProperty        --1->    {JObject}
	//                                       {JArray } 중 하나
	//                                       {JValue }
	//               JArray             --N-> JObject
	//                               children
	//           */

	//		// tree root 는 object 로 시작
	//		JObject jtok_root = new JObject();

	//		convertToJToken_recursive(root_tvi, jtok_root);

	//		return jtok_root;
	//	}
	//	static void convertToJToken_recursive(JsonTreeViewItem tvi_cur, JObject jobj_cur)
	//	{
	//		if(tvi_cur == null || jobj_cur == null)
	//			return;

	//		JToken cur_jtok = jobj_cur;
	//		// 여기서부터는 property 로 시작
	//		foreach(var v in tvi_cur.Items)
	//		{
	//			JsonTreeViewItem tvi_child = v as JsonTreeViewItem;
	//			if(tvi_child == null)
	//				continue;

	//			JsonTreeViewItemHeader pan = tvi_child.Header as JsonTreeViewItemHeader;
	//			if(pan == null)
	//				continue;

	//			// pan(MyTreeViewItem.Header) = key, val or val.type, button
	//			JsonTextBox tb_key = pan.Children[0] as JsonTextBox;
	//			ValuePanel panel_value = pan.Children[1] as ValuePanel;

	//			switch(panel_value.type)
	//			{
	//				case JTokenType.Object:
	//					{
	//						JObject jobj = new JObject();
	//						JProperty jprop = new JProperty(tb_key.Text, jobj);
	//						(cur_jtok as JObject).Add(jprop);
	//						convertToJToken_recursive(tvi_child, jobj);
	//					}
	//					break;
	//				case JTokenType.Array:
	//					{
	//						JArray jarr = new JArray();
	//						JProperty jprop = new JProperty(tb_key.Text, jarr);
	//						(cur_jtok as JObject).Add(jprop);
	//						for(int i = 0; i < tvi_child.Items.Count; i++)
	//						{
	//							JObject jobj = new JObject();
	//							jarr.Add(jobj);
	//							convertToJToken_recursive(tvi_child.Items[i] as JsonTreeViewItem, jobj);
	//						}
	//					}
	//					break;
	//				default:
	//					{
	//						JProperty jprop = new JProperty(tb_key.Text, new JValue(panel_value.value));
	//						(cur_jtok as JObject).Add(jprop);
	//					}
	//					break;
	//			}
	//		}
	//	}
	//	#endregion
	//}
	//class JsonTreeViewItemHeader : Grid
	//{
	//	public JsonAddButton addButtn;
	//	public JsonDeleteButton deleteButton;
	//	public JsonTreeViewItemHeader()
	//	{
	//		this.Height = Size.HEIGHT_HEADER;
	//		this.HorizontalAlignment = HorizontalAlignment.Stretch;
	//		//this.Orientation = Orientation.Horizontal;

	//		deleteButton = new JsonDeleteButton();
	//		AddItem(deleteButton);

	//		addButtn = new JsonAddButton();
	//		AddItem(addButtn);

	//	}
	//	public void AddItem(FrameworkElement item)
	//	{
	//		int idx_insert = this.Children.Count;
	//		while(idx_insert > 0 && !(this.Children[idx_insert - 1] is JsonTextBox))
	//			idx_insert--;

	//		this.Children.Insert(idx_insert, item);

	//		if(item is JsonButton)
	//		{
	//			if(item is JsonDeleteButton)
	//			{
	//				item.Margin = new Thickness(Size.MARGIN_BUTTON, Size.MARGIN_BUTTON, Size.MARGIN_BUTTON, Size.MARGIN_BUTTON);
	//			}
	//			else if(item is JsonAddButton)
	//			{
	//				item.Margin = new Thickness(Size.MARGIN_BUTTON, Size.MARGIN_BUTTON, Size.MARGIN_BUTTON + Size.WIDTH_BUTTON + Size.MARGIN_BUTTON, Size.MARGIN_BUTTON);
	//			}
	//		}
	//		else
	//		{
	//			double left = item.Margin.Left;
	//			for(int i = 0; i < idx_insert; i++)
	//			{
	//				left += Size.WIDTH_TEXTBOX + Size.MARGIN_TEXTBOX;
	//			}
	//			item.Margin = new Thickness(left, Size.MARGIN_TEXTBOX, Size.MARGIN_TEXTBOX, Size.MARGIN_TEXTBOX);
	//		}
	//	}
	//}

	//class JsonButton : Button
	//{
	//	public JsonButton()
	//	{
	//		//this.BorderBrush = null;
	//		//this.Background = Brushes.White;
	//		//this.Margin = new Thickness(Size.MARGIN_BUTTON, Size.MARGIN_BUTTON, 150, Size.MARGIN_BUTTON);
	//		this.Width = Size.WIDTH_BUTTON;
	//		this.Height = Size.HEIGHT_BUTTON;
	//		this.VerticalContentAlignment = VerticalAlignment.Center;
	//		this.HorizontalAlignment = HorizontalAlignment.Right;
	//	}
	//}
	//class JsonAddButton : JsonButton
	//{
	//	public JsonAddButton()
	//	{
	//		this.Content = new PackIconModern()
	//		{
	//			Kind = PackIconModernKind.Add,
	//			//Height = Size.HEIGHT_BUTTON-15,
	//			VerticalAlignment = VerticalAlignment.Stretch,
	//			HorizontalAlignment = HorizontalAlignment.Stretch
	//		};
	//		//this.Background = Brushes.LightGreen;
	//	}
	//	// Json 버튼은 JsonTreeViewItem.Header.children 에 존재
	//	protected override void OnClick()
	//	{
	//		base.OnClick();
	//		JsonTreeViewItemHeader jtvih = this.Parent as JsonTreeViewItemHeader;
	//		if(jtvih == null)
	//			return;
	//		JsonTreeViewItem jtvi = jtvih.Parent as JsonTreeViewItem;
	//		if(jtvi == null)
	//			return;

	//		if(jtvi.value_token_type == JTokenType.Array)
	//		{
	//			JsonTreeViewItem child_tvi = new JsonTreeViewItem();
	//			KeyTextBox tb_key = new KeyTextBox(jtvi.getCountChildProperty().ToString(), false);
	//			child_tvi.Header.AddItem(tb_key);
	//			KeyTextBox tb_type = new KeyTextBox("object", false);
	//			child_tvi.Header.AddItem(tb_type);
	//			child_tvi.value_token_type = JTokenType.Object;

	//			jtvi.AddItem(child_tvi);
	//		}
	//		else
	//		{
	//			// window_addJson showdialog()
	//			Window_AddJsonItem wnd_add = new Window_AddJsonItem();
	//			Point pt = this.PointToScreen(new Point(0, 0));
	//			wnd_add.Left = pt.X;
	//			wnd_add.Top = pt.Y;

	//			// cancel return
	//			if(wnd_add.ShowDialog() != true)
	//				return;

	//			JsonTreeViewItem.convertToTreeView_recursive(jtvi, new JProperty(wnd_add.key, wnd_add.value));
	//		}
	//	}
	//}
	//class JsonDeleteButton : JsonButton
	//{
	//	public JsonDeleteButton()
	//	{
	//		//this.Content = App.Current.Resources["appbar_close"];
	//		//this.Content = new MahApps.Metro.IconPacks.PackIconModern();
	//		this.Content = new PackIconModern()
	//		{
	//			Kind = PackIconModernKind.Minus,
	//			VerticalAlignment = VerticalAlignment.Stretch,
	//			HorizontalAlignment = HorizontalAlignment.Stretch
	//		};
	//		//this.Background = Brushes.LightPink;
	//		//< Controls:PackIconModern Width="20" Height = "20" Kind = "Close" />
	//	}

	//	// Json 버튼은 JsonTreeViewItem.Header.children 에 존재
	//	protected override void OnClick()
	//	{
	//		base.OnClick();
	//		JsonTreeViewItemHeader jtvih = this.Parent as JsonTreeViewItemHeader;
	//		if(jtvih == null)
	//			return;
	//		JsonTreeViewItem jtvi = jtvih.Parent as JsonTreeViewItem;
	//		if(jtvi == null)
	//			return;

	//		jtvi.Remove();
	//		//FileContoller.write(JsonInfo.current.filename, JsonInfo.current.jtok_root.ToString());
	//	}
	//}
	//class JsonToggleButton : ToggleButton
	//{
	//	public JsonToggleButton(bool _ischecked = false)
	//	{
	//		this.Content = new PackIconFontAwesome()
	//		{
	//			Kind = PackIconFontAwesomeKind.AngleDown,
	//			VerticalAlignment = VerticalAlignment.Stretch,
	//			HorizontalAlignment = HorizontalAlignment.Stretch
	//		};
	//		this.Width = Size.WIDTH_MOREBUTTON;
	//		this.Height = Size.HEIGHT_MOREBUTTON;
	//		this.HorizontalAlignment = HorizontalAlignment.Left;
	//		//this.Background = Brushes.White;
	//		//this.BorderBrush = null;
	//		this.IsChecked = _ischecked;
	//		//this.Visibility = Visibility.Collapsed;
	//	}
	//	protected override void OnUnchecked(RoutedEventArgs e)
	//	{
	//		// this 는 JsonTreeViewItem.Children 에 위치
	//		JsonTreeViewItem tvi = this.Parent as JsonTreeViewItem;
	//		if(tvi == null)
	//			return;
	//		base.OnUnchecked(e);

	//		tvi.hideMore();
	//	}
	//	protected override void OnChecked(RoutedEventArgs e)
	//	{
	//		// this 는 JsonTreeViewItem.Children 에 위치
	//		JsonTreeViewItem tvi = this.Parent as JsonTreeViewItem;
	//		if(tvi == null)
	//			return;

	//		tvi.visibleMore();
	//	}
	//}
	//class JsonTextBox : TextBox
	//{
	//	public JsonTextBox()
	//	{
	//		this.AllowDrop = true;

	//		this.Width = Size.WIDTH_TEXTBOX;
	//		//this.Height = Size.HEIGHT_TEXTBOX;
	//		this.HorizontalAlignment = HorizontalAlignment.Left;
	//	}

	//	// JsonTextBox 는 JsonTreeViewItem.Header.Children 에 존재
	//	protected override void OnDragOver(DragEventArgs e)
	//	{
	//		JsonTreeViewItem tvi_parent = this.Parent as JsonTreeViewItem;
	//		if(tvi_parent == null)
	//			return;

	//		tvi_parent.PublicDragOver(e);
	//		Log.Print("KeyTextBox.OnDragOver()");
	//		base.OnDragOver(e);
	//	}
	//	protected override void OnDrop(DragEventArgs e)
	//	{
	//		Log.Print("KeyTextBox.OnDrop()");
	//		JsonTreeViewItem tvi_parent = this.Parent as JsonTreeViewItem;
	//		if(tvi_parent == null)
	//			return;

	//		tvi_parent.PublicDrop(e);
	//		base.OnDrop(e);
	//	}
	//}
	//class KeyTextBox : JsonTextBox
	//{
	//	public KeyTextBox(string text, bool isModify = true)
	//	{
	//		this.Text = text;
	//		this.Margin = new Thickness(Size.MARGIN_TEXTBOX);

	//		if(!isModify)
	//		{
	//			this.IsEnabled = false;
	//			this.BorderBrush = null;
	//		}
	//	}
	//}
	//class ValuePanel : StackPanel
	//{
	//	// UI input => Grid.children[0]
	//	public JTokenType type;
	//	public object value;
	//	public ValuePanel(JTokenType _type, object _value)
	//	{
	//		this.AllowDrop = true;
	//		this.HorizontalAlignment = HorizontalAlignment.Left;
	//		this.VerticalAlignment = VerticalAlignment.Stretch;
	//		this.Margin = new Thickness(Size.MARGIN_TEXTBOX);
	//		this.Width = Size.WIDTH_TEXTBOX + Size.WIDTH_TEXTBOX + Size.MARGIN_TEXTBOX;
	//		this.Orientation = Orientation.Horizontal;

	//		type = _type;
	//		value = _value;

	//		switch(type)
	//		{
	//			case JTokenType.Array:
	//				break;
	//			case JTokenType.Boolean:
	//				//TextBlock tb_boolean = new TextBlock();
	//				//tb_boolean.Text = value.ToString();
	//				//tb_boolean.VerticalAlignment = VerticalAlignment.Center;
	//				//tb_boolean.Background = null;

	//				//CheckBox cb = new CheckBox();
	//				//cb.IsChecked = (bool)value;
	//				//this.Children.Add(cb);
	//				//cb.Checked += delegate { tb_boolean.Text = cb.IsChecked.ToString(); this.value = cb.IsChecked; };
	//				//cb.Unchecked += delegate { tb_boolean.Text = cb.IsChecked.ToString(); this.value = cb.IsChecked; };

	//				//this.Children.Add(tb_boolean);

	//				ToggleSwitch ts = new ToggleSwitch();
	//				ts.IsChecked = (bool)value;
	//				ts.Width = Size.WIDTH_TEXTBOX;
	//				ts.FontSize = 13;
	//				ts.OffLabel = "False";
	//				ts.OnLabel = "True";
	//				ts.Style = (Style)App.Current.Resources["MahApps.Metro.Styles.ToggleSwitch.Win10"];
	//				ts.Checked += delegate { this.value = ts.IsChecked; };
	//				ts.Unchecked += delegate { this.value = ts.IsChecked; };
	//				this.Children.Add(ts);
	//				break;
	//			case JTokenType.Bytes:
	//				break;
	//			case JTokenType.Comment:
	//				break;
	//			case JTokenType.Constructor:
	//				break;
	//			case JTokenType.Date:
	//				break;
	//			case JTokenType.Float:
	//				break;
	//			case JTokenType.Guid:
	//				break;
	//			case JTokenType.Integer:
	//				//ValueTextBox tb_integer = new ValueTextBox(value.ToString());
	//				////tb_integer.Text = value.ToString();
	//				////tb_integer.Width = this.Width;
	//				//tb_integer.TextChanged += TextBox_TextChanged;
	//				//this.Children.Add(tb_integer);

	//				NumericUpDown tb_integer = new NumericUpDown();
	//				tb_integer.Width = Size.WIDTH_TEXTBOX;
	//				//Console.WriteLine("\t\tval = " + this.value.GetType());
	//				tb_integer.Value = (System.Int64)this.value;
	//				tb_integer.HorizontalContentAlignment = HorizontalAlignment.Left;
	//				tb_integer.ValueChanged += delegate { this.value = (int)tb_integer.Value; };
	//				this.Children.Add(tb_integer);
	//				break;
	//			case JTokenType.None:
	//				break;
	//			case JTokenType.Null:
	//				break;
	//			case JTokenType.Object:
	//				break;
	//			case JTokenType.Property:
	//				break;
	//			case JTokenType.Raw:
	//				break;
	//			case JTokenType.String:
	//				ValueTextBox tb_string = new ValueTextBox(value.ToString());
	//				//ValueTextBox tb_string = new ValueTextBox("\"" + value.ToString() + "\"");
	//				//tb_string.Text = "\"" + value.ToString() + "\"";
	//				tb_string.Width = Size.WIDTH_TEXTBOX;
	//				tb_string.TextChanged += delegate { this.value = tb_string.Text; };
	//				this.Children.Add(tb_string);
	//				break;
	//			case JTokenType.TimeSpan:
	//				break;
	//			case JTokenType.Undefined:
	//				break;
	//			case JTokenType.Uri:
	//				break;
	//		}

	//		Label label = new Label();
	//		label.VerticalAlignment = VerticalAlignment.Center;
	//		label.Foreground = Brushes.Gray;
	//		label.FontSize = 10;
	//		label.Content = type.ToString();
	//		this.Children.Add(label);
	//	}

	//	private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
	//	{
	//		TextBox tb = sender as TextBox;
	//		if(tb == null)
	//			return;

	//		//this.value = tb.Text;
	//		string str = tb.Text;
	//		if(str.Length > 1 && str[0] == str[str.Length - 1])
	//		{
	//			if(str[0] == '\'')
	//				this.value = str.Trim('\'');
	//			else if(str[0] == '\"')
	//				this.value = str.Trim('\"');
	//			else
	//				Log.Print("[TextBox_TextChanged] error");
	//		}
	//		else
	//		{
	//			try
	//			{
	//				this.value = Convert.ToInt64(str);
	//			}
	//			catch(Exception ex)
	//			{
	//				Log.Print("[TextBox_TextChanged] " + ex.Message);
	//			}
	//		}
	//	}


	//	// JsonTextBox 는 JsonTreeViewItem.Header.Children 에 존재
	//	protected override void OnDragOver(DragEventArgs e)
	//	{
	//		JsonTreeViewItem tvi_parent = this.Parent as JsonTreeViewItem;
	//		if(tvi_parent == null)
	//			return;

	//		tvi_parent.PublicDragOver(e);
	//		Log.Print("KeyTextBox.OnDragOver()");
	//		base.OnDragOver(e);
	//	}
	//	protected override void OnDrop(DragEventArgs e)
	//	{
	//		Log.Print("KeyTextBox.OnDrop()");
	//		JsonTreeViewItem tvi_parent = this.Parent as JsonTreeViewItem;
	//		if(tvi_parent == null)
	//			return;

	//		tvi_parent.PublicDrop(e);
	//		base.OnDrop(e);
	//	}
	//}
	//class ValueTextBox : JsonTextBox
	//{
	//	public ValueTextBox(string str, bool isModify = true)
	//	{
	//		this.Text = str;
	//		if(!isModify)
	//		{
	//			this.IsEnabled = false;
	//			this.BorderBrush = null;
	//			this.Background = null;
	//		}
	//	}
	//}
	////class ValueTextBox : JsonTextBox
	////{
	////	public JValue val;
	////	public ValueTextBox(JValue _val)
	////	{
	////		val = _val;
	////		this.Text = val.Value.ToString();
	////		//Binding myBinding = new Binding("Value");
	////		//myBinding.Source = jval;
	////		//myBinding.Mode = BindingMode.TwoWay;
	////		//myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
	////		//this.SetBinding(TextBox.TextProperty, myBinding);

	////		//this.TextChanged += delegate (object sender, TextChangedEventArgs e)
	////		//{
	////		//	//FileContoller.write(JsonInfo.current.filename, JsonInfo.current.jtok_root.ToString());
	////		//};
	////	}
	////}

	//#endregion
}