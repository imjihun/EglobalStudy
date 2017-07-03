using CofileUI.Classes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
	/// col_var.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class col_var : UserControl
	{
		public col_var()
		{
			InitializeComponent();
		}
		enum Option
		{
			item=0,
			column_pos,
			wrap_char
		}
		//string[] _option = new string[] { "item", "column_pos", "wrap_char" };
		string[] detail = new string[] { "암/복호화에 사용할 Item명", "암/복호화 대상 컬럼 위치", "암/복호화시 제외할 문자(호환성 유지용도)" };
		object[] initvalue = new object[] {"", (Int64)0, "" };
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
			Point pt = btn.PointToScreen(new Point(0, 0));
			wa.Left = pt.X;
			wa.Top = pt.Y;
			if(wa.ShowDialog() != true)
				return;

			JObject jobj = new JObject();
			for(int i = 0; i < wa.Value.Length; i++)
				jobj.Add(new JProperty(((Option)i).ToString(), wa.Value[i]));
			jarr.Add(jobj);
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
			int i=0;
			try
			{
				foreach(var v in jobj.Children())
				{
					JProperty jp = v as JProperty;
					if(jp == null)
						continue;
					JValue jv = jp.Value as JValue;
					if(jv == null)
						continue;

					int idx = (int)Enum.Parse(typeof(Option), jp.Name);
					_initvalue[idx] = jv.Value;
				}
			}
			catch(Exception ex)
			{
				Log.PrintError(ex.Message, "UserControls.ConfigOptions.Sam.col_var.OnClickModify");
			}
			for(int _i = 0; _i < _initvalue.Length; _i++)
			{
				if(_initvalue[_i] == null)
				{
					switch((Option)_i)
					{
						case Option.item:

							break;
						case Option.column_pos:
							break;
						case Option.wrap_char:
							break;
					}
				}
			}

			Button btn = sender as Button;
			if(btn == null)
				return;

			Windows.Window_AddDataGridInConfig wa = new Windows.Window_AddDataGridInConfig(detail, _initvalue);
			Point pt = btn.PointToScreen(new Point(0, 0));
			wa.Left = pt.X;
			wa.Top = pt.Y;
			if(wa.ShowDialog() != true)
				return;

			i=0;
			foreach(var v in jobj.Children())
			{
				JProperty jp = v as JProperty;
				if(jp == null)
					continue;
				JValue jv = jp.Value as JValue;
				if(jv == null)
					continue;

				jv.Value = wa.Value[i++];
			}
			DataContext = null;
			DataContext = jprop;
		}
	}
}
