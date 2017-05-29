using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace Manager_proj_3
{
	/// <summary>
	/// test.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class test : Window
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
		public test()
		{
			InitializeComponent();

			refreshJsonFile();
			button.Click += Button_Click;
			listView_json.SelectionChanged += ListView_json_SelectionChanged;
		}

		private void Button_Click(object sender, EventArgs e)
		{
			Console.WriteLine(cur_jsonfile.jroot.ToString());
			Console.WriteLine(cur_jsonfile.GetHashCode());
			FileContoller.write(cur_jsonfile.filename, cur_jsonfile.jroot.ToString());
		}
		private void OnTextChanged_value(object sender, TextChangedEventArgs e)
		{
			TextBox tb = sender as TextBox;
			Console.WriteLine(cur_jsonfile.jroot.ToString());
			Console.WriteLine(cur_jsonfile.GetHashCode());

			FileContoller.write(cur_jsonfile.filename, cur_jsonfile.jroot.ToString());
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
			listView_json.SelectedIndex = 0;
			ListView_json_SelectionChanged(listView_json, null);
			//var lv = listView_json;
			//lv.SelectedItems.Clear();
			//foreach(var item in lv.Items)
			//	lv.SelectedItems.Add(item);
			//cur_jsonfile.path = files[0];
			//cur_jsonfile.filename = (listView_json.Items.GetItemAt(0) as Label).Content as string;

			//string json = FileContoller.read(cur_jsonfile.path);
			//cur_jsonfile.jobj = JsonController.parseJson(json);
			//refreshJsonItem(json);

		}
		private void ListView_json_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Label selected = listView_json.SelectedItem as Label;
			if(selected != null)
			{
				cur_jsonfile.filename = selected.Content as string;
				string json = FileContoller.read(cur_jsonfile.filename);
				refreshJsonItem(json);
			}
		}

		public void refreshJsonItem(string jsonString)
		{
			cur_jsonfile.jroot = JsonController.parseJson(jsonString);

			var children = new List<JToken>();
			if(cur_jsonfile.jroot != null)
			{
				children.Add(cur_jsonfile.jroot);
			}

			treeView1.ItemsSource = null;
			treeView1.Items.Clear();
			treeView1.ItemsSource = children;
		}
	}
}
