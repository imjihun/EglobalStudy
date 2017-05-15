
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ConfigEditor_proj
{
	/// <summary>
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainWindow : Window
	{
		class JSonFile
		{
			public string path;
			public string filename;
			public JObject jobj;
			public void Clear()
			{
				path = null;
				filename = null;
				jobj = null;
			}
		}
		JSonFile cur_jsonfile = new JSonFile();
		public MainWindow()
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
			//trvItem.Tag = "0000";
			addButtonAddjson(trvItem, key_stack);

			treeView_json.Items.Add(trvItem);

			if(json != null)
			{
				cur_jsonfile.jobj = JsonController.parseJson(json);
				addJsonItem(trvItem, cur_jsonfile.jobj, key_stack);
			}
		}
		void addJsonItem(TreeViewItem cur, JObject jobj, List<string> key_stack)
		{
			foreach(JProperty property in jobj.Properties())
			{
				if(property.Value.Type == JTokenType.Object)
				//if(JToken.DeepEquals(property.GetType(), JTokenType.Object)
				{
					TreeViewItem newtrvitem = addTreeviewToJsonTree(cur, property.Name, key_stack);
					addJsonItem(newtrvitem, (JObject)property.Value, key_stack);
				}
				else if(property.Value.Type == JTokenType.Array)
				{
					TreeViewItem newtrvitem = addTreeviewToJsonTree(cur, property.Name, key_stack);
					addJsonItem(newtrvitem, (JArray)property.Value, key_stack);
				}
				else
				{
					addItemToJsonTree(cur, property.Name, property.Value.ToString(), key_stack);
				}
			}
			// 재귀가 끝나는 지점에 jobject가 끝나므로 스택에서 키를 빼준다.
			if(key_stack.Count > 0)
				key_stack.RemoveAt(key_stack.Count - 1);
		}
		void addJsonItem(TreeViewItem cur, JArray jarr, List<string> key_stack)
		{
			int idx = 0;
			foreach(JObject var_jobj in jarr.Children<JObject>())
			{
				TreeViewItem newtrvitem = addTreeviewToJsonTree(cur, idx.ToString(), key_stack);
				addJsonItem(newtrvitem, (JObject)var_jobj, key_stack);
				idx++;
			}
			// 재귀가 끝나는 지점에 jobject가 끝나므로 스택에서 키를 빼준다.
			if(key_stack.Count > 0)
				key_stack.RemoveAt(key_stack.Count - 1);
		}
		TreeViewItem addTreeviewToJsonTree(TreeViewItem viewitem, string key, List<string> key_stack)
		{
			// 계층이 하나 생긴다.
			StackPanel sp = new StackPanel();
			sp.Orientation = Orientation.Horizontal;
			viewitem.Items.Insert(viewitem.Items.Count - 1, sp);
			
			TreeViewItem trvItem = new TreeViewItem();
			trvItem.Header = key;
			addButtonAddjson(trvItem, key_stack);
			sp.Children.Add(trvItem);

			addButtonDeletejson(sp, key_stack);

			//viewitem.Items.Insert(viewitem.Items.Count - 1, trvItem);

			// 새로운 스택이 쌓이는 구간
			key_stack.Add(key);

			return trvItem;
		}
		void addItemToJsonTree(TreeViewItem viewitem, string key, string data, List<string> key_stack)
		{
			Grid grd = new Grid();
			ColumnDefinition cldf0 = new ColumnDefinition();
			cldf0.Width = new GridLength(1, GridUnitType.Star);
			grd.ColumnDefinitions.Add(cldf0);

			ColumnDefinition cldf1 = new ColumnDefinition();
			cldf1.Width = new GridLength(1, GridUnitType.Star);
			grd.ColumnDefinitions.Add(cldf1);

			ColumnDefinition cldf2 = new ColumnDefinition();
			cldf2.Width = new GridLength(1, GridUnitType.Star);
			grd.ColumnDefinitions.Add(cldf2);

			viewitem.Items.Insert(viewitem.Items.Count - 1, grd);

			string[] arr = new string[key_stack.Count];
			key_stack.CopyTo(arr);

			TextBox tb_key = new TextBox();
			tb_key.Text = key;
			tb_key.Margin = new Thickness(5);
			tb_key.IsEnabled = false;
			tb_key.TextChanged += delegate (object sender, TextChangedEventArgs e)
			{
				JToken tmp = cur_jsonfile.jobj;
				for(int i = 0; i < arr.Length; i++)
				{
					if(tmp is JObject)
						tmp = ((JObject)tmp).GetValue(arr[i]);
					else if(tmp is JArray)
						tmp = ((JArray)tmp)[Convert.ToInt16(arr[i])];
					else
						break;
				}
				//Console.WriteLine(tmp);
				Console.WriteLine(tmp[key]);
				tmp[key] = tb_key.Text;

				FileContoller.write(cur_jsonfile.filename, cur_jsonfile.jobj.ToString());
			};
			grd.Children.Add(tb_key);
			tb_key.SetValue(Grid.ColumnProperty, 0);

			TextBox tb_value = new TextBox();
			tb_value.Text = data;
			tb_value.Margin = new Thickness(5);
			tb_value.TextChanged += delegate (object sender, TextChangedEventArgs e)
			{
				JToken tmp = cur_jsonfile.jobj;
				for(int i = 0; i < arr.Length; i++)
				{
					if(tmp is JObject)
						tmp = ((JObject)tmp).GetValue(arr[i]);
					else if(tmp is JArray)
						tmp = ((JArray)tmp)[Convert.ToInt16(arr[i])];
					else
						break;
				}
				tmp[key] = tb_value.Text;
				
				FileContoller.write(cur_jsonfile.filename, cur_jsonfile.jobj.ToString());
			};
			//tb_value.TextChanged += Tb_value_TextChanged;
			grd.Children.Add(tb_value);
			tb_value.SetValue(Grid.ColumnProperty, 1);
			addButtonDeletejson(grd, key_stack);
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
			btn.Click += delegate (object sender, RoutedEventArgs e)
			{
				new popup_AddJsonItem().ShowDialog();
			};
			viewitem.Items.Add(btn);
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
			
			btn.Click += delegate (object sender, RoutedEventArgs e)
			{
				TreeViewItem parent = pan.Parent as TreeViewItem;
				if(parent != null)
				{
					TextBox tb = pan.Children[0] as TextBox;
					if(tb != null)
					{
						JToken tmp = cur_jsonfile.jobj;
						for(int i = 0; i < key_stack.Count; i++)
						{
							if(tmp is JObject)
								tmp = ((JObject)tmp).GetValue(key_stack[i]);
							else if(tmp is JArray)
								tmp = ((JArray)tmp)[Convert.ToInt16(key_stack[i])];
							else
								break;
						}
						tmp[tb.Text].Remove();
					}

					parent.Items.Remove(pan);
					FileContoller.write(cur_jsonfile.filename, cur_jsonfile.jobj.ToString());
				}
			};
			pan.Children.Add(btn);
			btn.SetValue(Grid.ColumnProperty, 2);
		}

	}

}
