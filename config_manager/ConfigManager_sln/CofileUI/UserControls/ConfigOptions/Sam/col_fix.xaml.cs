using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CofileUI.UserControls.ConfigOptions.Sam
{
	/// <summary>
	/// col_fix.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class col_fix : UserControl
	{
		public col_fix()
		{
			InitializeComponent();
			//dataGrid.InitializingNewItem += delegate (object sender, InitializingNewItemEventArgs e)
			//{
			//	Console.WriteLine("new");
			//};
			//dataGrid.ItemTemplate = new DataTemplate(typeof(JObject));
			//this.Loaded += Col_fix_Loaded;
		}

		enum Option
		{
			item = 0,
			start_pos,
			size,
			col_size,
			Length
		}
		string[] detail = new string[(int)Option.Length] { "암/복호화에 사용할 Item명", "암/복호화 대상 컬럼 시작 위치", "암/복호화 대상 컬럼 크기" , "암/복호화 후 데이터 크기" };
		object[] initvalue = new object[(int)Option.Length] {"", (Int64)0, (Int64)0, (Int64)0 };
		private void OnClickAdd(object sender, RoutedEventArgs e)
		{
			JProperty jprop = this.DataContext as JProperty;
			if(jprop == null)
				return;
			JArray jarr = jprop.Value as JArray;
			if(jarr == null)
				return;
			Button btn = sender as Button;
			if(btn == null)
				return;

			Windows.Window_AddDataGridInConfig wa = new Windows.Window_AddDataGridInConfig(detail, initvalue);
			Point pt = this.PointToScreen(new Point(0, 0));
			wa.Left = pt.X + this.ActualWidth / 2 - wa.Width / 2;
			wa.Top = pt.Y + this.ActualHeight / 2 - wa.Height / 2;
			if(wa.ShowDialog() != true)
				return;

			JObject jobj = new JObject();
			for(int i = 0; i < wa.Value.Length; i++)
				jobj.Add(new JProperty(((Option)i).ToString(), wa.Value[i]));
			jarr.Add(jobj);

			ConfigOptionManager.bChanged = true;
		}
		private void OnClickDelete(object sender, RoutedEventArgs e)
		{
			JProperty jprop = this.DataContext as JProperty;
			if(jprop == null)
				return;
			JArray jarr = jprop.Value as JArray;
			if(jarr == null)
				return;
			JObject jobj = dataGrid.SelectedItem as JObject;
			if(jobj == null)
				return;

			jarr.Remove(jobj);

			ConfigOptionManager.bChanged = true;
		}
		private void OnClickModify(object sender, RoutedEventArgs e)
		{
			JProperty jprop = this.DataContext as JProperty;
			if(jprop == null)
				return;
			JArray jarr = jprop.Value as JArray;
			if(jarr == null)
				return;
			JObject jobj = dataGrid.SelectedItem as JObject;
			if(jobj == null)
				return;
			object[] _initvalue = new object[initvalue.Length];
			for(int i = 0; i < _initvalue.Length; i++)
			{
				string key = ((Option)i).ToString();
				JValue jval = jobj[key] as JValue;
				if(jval != null)
					_initvalue[i] = jval.Value;
				else
					_initvalue[i] = initvalue[i];
			}

			Button btn = sender as Button;
			if(btn == null)
				return;

			Windows.Window_AddDataGridInConfig wa = new Windows.Window_AddDataGridInConfig(detail, _initvalue);
			Point pt = this.PointToScreen(new Point(0, 0));
			wa.Left = pt.X + this.ActualWidth / 2 - wa.Width / 2;
			wa.Top = pt.Y + this.ActualHeight / 2 - wa.Height / 2;
			if(wa.ShowDialog() != true)
				return;


			for(int i = 0; i < wa.Value.Length; i++)
			{
				string key = ((Option)i).ToString();
				JValue jval = jobj[key] as JValue;
				if(jval == null)
				{
					jobj.Add(new JProperty(key, wa.Value[i]));

					jval = jobj[SamOption.StartDisableProperty + key] as JValue;
					if(jval != null)
						jobj.Remove(SamOption.StartDisableProperty + key);
				}
				else
					jval.Value = wa.Value[i];
			}

			DataContext = null;
			DataContext = jprop;

			ConfigOptionManager.bChanged = true;
		}

		bool b = false;
		private void Col_fix_Loaded(object sender, RoutedEventArgs e)
		{
			//if(b)
			//	return;
			//b = !b;

			//Console.WriteLine(this.DataContext.GetType());
			//JProperty jprop = this.DataContext as JProperty;
			//if(jprop == null)
			//	return;

			//Console.WriteLine("type = " + jprop.Value.Type);
			//JArray jarr = jprop.Value as JArray;
			//jarr.AddingNew += Jarr_AddingNew;
			//jarr.AddingNew += delegate { Console.WriteLine("AddingNew"); };
			//jarr.CollectionChanged += delegate { Console.WriteLine("CollectionChanged"); };
			//jarr.ListChanged += delegate { Console.WriteLine("ListChanged"); };
			//jarr.Add(new JObject());
			//jarr.Add(new JObject());
			//jarr.Add(new JObject());
			//this.DataContext = jarr.Cast<JObject>();
			//foreach(var v in jarr.Cast<JObject>())
			//	Console.WriteLine(v);

			//List<MyClass> list = new List<MyClass>();
			////Data data = new Sam.Data() {Value = new ObservableCollection<MyClass>() };

			//int i=0;
			//foreach(var v in jprop.Value.Children())
			//{
			//	MyClass mc = new MyClass()
			//	{
			//		item = Convert.ToString(v["item"]),
			//		start_pos = Convert.ToInt64(v["start_pos"]),
			//		size = Convert.ToInt64(v["size"]),
			//		col_size = Convert.ToInt64(v["col_size"])
			//	};
			//	list.Add(mc);
			//	//data.Value.Add(mc);
			//	Console.WriteLine(i++ + "\t" + v);
			//}
			//this.DataContext = list;
			////this.DataContext = data;
			//Console.WriteLine("data = " + this.DataContext);
		}

		private void Jarr_AddingNew(object sender, System.ComponentModel.AddingNewEventArgs e)
		{
			((JArray)sender).Add(new JObject());
			Console.WriteLine("e.NewObject = " + e.NewObject);
		}
	}
	class Data
	{
		public ObservableCollection<MyClass> Value { get; set; }
	}
	class MyClass
	{
		public string item { get; set; }
		public Int64 start_pos { get; set; }
		public Int64 size { get; set; }
		public Int64 col_size { get; set; }
	}
}
