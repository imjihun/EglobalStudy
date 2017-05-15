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
		JSonFile cur_jsonfile = new JSonFile();
		public test3()
		{
			InitializeComponent();
			refreshJsonFile();
		}
		public void refreshJsonFile()
		{
			// 삭제
			listView_json.Items.Clear();
			cur_jsonfile.Clear();

			// 추가
			//string[] files = Directory.GetFiles(@"D:\git\config_manager\ConfigManager_sln\ConfigEditor_proj\bin\Debug", "*.json");
			// 현재 application이 실행되는 경로의 json 파일을 찾아라
			string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.json");

			for(int i = 0; i < files.Length; i++)
			{
				Label lb = new Label();
				string[] filename_splited = files[i].Split('\\');
				lb.Content = filename_splited[filename_splited.Length - 1];
				listView_json.Items.Add(lb);
			}
			cur_jsonfile.path = files[0];
			cur_jsonfile.filename = (listView_json.Items.GetItemAt(0) as Label).Content as string;

			string json = FileContoller.read(cur_jsonfile.path);
			refreshJsonItem(json, cur_jsonfile.filename);

			listView_json.SelectionChanged += ListView_json_SelectionChanged;
		}
		private void ListView_json_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

			Label selected = listView_json.SelectedItem as Label;
			if(selected != null)
			{
				cur_jsonfile.filename = selected.Content as string;
				string json = FileContoller.read(cur_jsonfile.filename);
				refreshJsonItem(json, cur_jsonfile.filename);
			}
		}


		public void refreshJsonItem(string json, string filenama)
		{
			// 삭제
			treeView_json.Items.Clear();

			// 추가
			List<string> key_stack = new List<string>();
			TreeViewItem trvItem = new TreeViewItem();
			trvItem.Header = filenama;
			trvItem.IsExpanded = true;
			//trvItem.Tag = "0000";
			addButtonAddjson(trvItem, key_stack);

			treeView_json.Items.Add(trvItem);

			if(json != null)
			{
				cur_jsonfile.jroot = JsonController.parseJson(json);
				addJsonItem_recursive(trvItem, cur_jsonfile.jroot, key_stack);
			}
		}
		void addJsonItem_recursive(TreeViewItem cur_tvi, JToken jtok, List<string> key_stack)
		{
			foreach(var v in jtok.Children())
			{
				TreeViewItem child_tvi = cur_tvi;
				if(v as JObject != null)
				{
					if(v.Parent as JProperty == null)
					{
						child_tvi = addToJsonTree(cur_tvi, (JObject)v, key_stack);
						key_stack.Add((cur_tvi.Items.Count - 2).ToString());
					}
				}
				//// 실제로 실행이 안되는 코드 
				//else if(v as JArray != null)
				//{
				//	if(v.Parent as JProperty == null)
				//	{
				//		child_tvi = addToJsonTree(cur_tvi, (JArray)v, key_stack);
				//	}
				//}
				else if(v as JProperty != null)
				{
					child_tvi = addToJsonTree(cur_tvi, v as JProperty, key_stack);
					key_stack.Add((v as JProperty).Name);
				}
				else if(v as JValue != null)
				{
					child_tvi = addToJsonTree(cur_tvi, (JValue)v, key_stack);
					continue;
				}
				else
				{
					Console.WriteLine(v.GetType());
					continue;
				}

				addJsonItem_recursive(child_tvi, v, key_stack);
			}
			// 재귀가 끝나는 지점에 jobject가 끝나므로 스택에서 키를 빼준다.
			if(key_stack.Count > 0)
				key_stack.RemoveAt(key_stack.Count - 1);
		}
		TreeViewItem addToJsonTree(TreeViewItem parent_tvi, JObject jobj, List<string> key_stack)
		{
			if(jobj.Parent as JProperty != null)
				return parent_tvi;

			TreeViewItem child_tvi = new TreeViewItem();
			addButtonAddjson(child_tvi, key_stack);
			parent_tvi.Items.Insert(parent_tvi.Items.Count - 1, child_tvi);

			StackPanel sp = new StackPanel();
			sp.Orientation = Orientation.Horizontal;
			child_tvi.Header = sp;

			addTextBlock(sp, (parent_tvi.Items.Count - 2).ToString(), key_stack);

			addButtonDeletejson(sp, key_stack);

			return child_tvi;
			//return parent_tvi;
		}
		TreeViewItem addToJsonTree(TreeViewItem parent_tvi, JArray jarr, List<string> key_stack)
		{
			if(jarr.Parent as JProperty != null)
				return parent_tvi;

			TreeViewItem child_tvi = new TreeViewItem();
			addButtonAddjson(child_tvi, key_stack);
			parent_tvi.Items.Insert(parent_tvi.Items.Count - 1, child_tvi);

			StackPanel sp = new StackPanel();
			sp.Orientation = Orientation.Horizontal;
			child_tvi.Header = sp;

			addTextBlock(sp, "JArray",key_stack);

			addButtonDeletejson(sp, key_stack);

			return child_tvi;
		}
		TreeViewItem addToJsonTree(TreeViewItem parent_tvi, JProperty jprop, List<string> key_stack)
		{
			TreeViewItem child_tvi = new TreeViewItem();
			addButtonAddjson(child_tvi, key_stack);
			parent_tvi.Items.Insert(parent_tvi.Items.Count - 1, child_tvi);

			StackPanel sp = new StackPanel();
			sp.Orientation = Orientation.Horizontal;
			child_tvi.Header = sp;

			addKeyTextBox(sp, jprop, key_stack);
			addButtonDeletejson(sp, key_stack);


			return child_tvi;
		}
		TreeViewItem addToJsonTree(TreeViewItem parent_tvi, JValue jval, List<string> key_stack)
		{
			addValueTextBox(parent_tvi.Header as Panel, jval, key_stack);
			parent_tvi.Items.Clear();
			return parent_tvi;
		}

		void addKeyTextBox(Panel pan, JProperty jprop, List<string> key_stack)
		{
			string key = jprop.Name;
			string[] arr = new string[key_stack.Count];
			key_stack.CopyTo(arr);

			TextBox tb_key = new TextBox();
			tb_key.Text = key;
			tb_key.Margin = new Thickness(5);
			tb_key.Width = 150;
			tb_key.IsEnabled = false;
			tb_key.TextChanged += delegate (object sender, TextChangedEventArgs e)
			{
				Console.WriteLine(arr.Length);
				JToken tmp = cur_jsonfile.jroot;
				for(int i = 0; i < arr.Length; i++)
				{
					if(tmp is JObject)
						tmp = ((JObject)tmp).GetValue(arr[i]);
					else if(tmp is JArray)
						tmp = ((JArray)tmp)[Convert.ToInt16(arr[i])];
					else
						break;
				}
				Console.WriteLine(key);
				tmp[tb_key.Text] = tmp[key];
				
				if(tmp[key] != null && tmp[key].Parent != null)
					tmp[key].Parent.Remove();
				//(tmp[key] as JProperty).Remove();

				FileContoller.write(cur_jsonfile.filename, cur_jsonfile.jroot.ToString());
			};
			pan.Children.Add(tb_key);
			//tb_key.SetValue(Grid.ColumnProperty, 0);
		}
		void addTextBlock(Panel pan, string key, List<string> key_stack)
		{
			string[] arr = new string[key_stack.Count];
			key_stack.CopyTo(arr);

			TextBlock tb_key = new TextBlock();
			tb_key.Text = key;
			tb_key.Margin = new Thickness(5);
			tb_key.Width = 150;
			pan.Children.Add(tb_key);
			//tb_key.SetValue(Grid.ColumnProperty, 0);
		}
		void addValueTextBox(Panel pan, JValue jval, List<string> key_stack)
		{
			string data = jval.ToString();
			string[] arr = new string[key_stack.Count];
			key_stack.CopyTo(arr);

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
				Console.WriteLine("called TextChanged");
				Console.WriteLine(jval);

				FileContoller.write(cur_jsonfile.filename, cur_jsonfile.jroot.ToString());
			};
			pan.Children.Insert(pan.Children.Count - 1 ,tb_value);
		}
		

		void addButtonAddjson(TreeViewItem viewitem, List<string> key_stack)
		{
			Button btn = new Button();
			btn.Content = '+';
			btn.Background = Brushes.White;
			btn.BorderBrush = Brushes.Black;
			btn.Width = 20;
			btn.Height = 20;
			btn.VerticalContentAlignment = VerticalAlignment.Center;
			btn.HorizontalContentAlignment = HorizontalAlignment.Center;

			string[] arr_key_stack = new string[key_stack.Count];
			key_stack.CopyTo(arr_key_stack);
			btn.Click += delegate (object sender, RoutedEventArgs e)
			{
				JProperty jprop = new JProperty("key", "value");

				Console.WriteLine(arr_key_stack.Length);
				Console.WriteLine(key_stack.Count);

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
				tmp[jprop.Name] = jprop.Value;

				TreeViewItem child_tvi = addToJsonTree(viewitem, jprop, key_stack);
				addToJsonTree(child_tvi, jprop.Value as JValue, key_stack);
				FileContoller.write(cur_jsonfile.filename, cur_jsonfile.jroot.ToString());
				//addItemToJsonTree(viewitem, "", "", key_stack);
				//new popup_AddJsonItem().ShowDialog();
			};
			viewitem.Items.Add(btn);
		}
		void func(string[] arr_key_stack, TreeViewItem tvi)
		{
			JProperty jprop = new JProperty("key", "value");


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
			tmp[jprop.Name] = jprop.Value;

			//TreeViewItem child_tvi = addToJsonTree(tvi, jprop, key_stack);
			//addToJsonTree(child_tvi, jprop.Value as JValue, key_stack);
			FileContoller.write(cur_jsonfile.filename, cur_jsonfile.jroot.ToString());
		}
		void addButtonDeletejson(Panel pan, List<string> key_stack)
		{
			Button btn = new Button();
			btn.Content = '-';
			btn.Background = Brushes.White;
			btn.BorderBrush = Brushes.Black;
			btn.Width = 20;
			btn.Height = 20;
			btn.VerticalContentAlignment = VerticalAlignment.Center;
			btn.HorizontalContentAlignment = HorizontalAlignment.Center;

			string[] arr_key_stack = new string[key_stack.Count];
			key_stack.CopyTo(arr_key_stack);
			btn.Click += delegate (object sender, RoutedEventArgs e)
			{
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

				// search from jroot
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
				// remove from jroot
				string key;
				TextBox tb = pan.Children[0] as TextBox;
				if(tb != null)
				{
					key = tb.Text;
					JToken jt = tmp[key];
					if(jt != null)
						tmp[key].Parent.Remove();
				}

				FileContoller.write(cur_jsonfile.filename, cur_jsonfile.jroot.ToString());
			};
			pan.Children.Add(btn);
			//btn.SetValue(Grid.ColumnProperty, 2);
		}

	}
}