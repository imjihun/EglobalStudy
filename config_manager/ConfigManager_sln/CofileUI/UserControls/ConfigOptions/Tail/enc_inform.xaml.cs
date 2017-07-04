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

namespace CofileUI.UserControls.ConfigOptions.Tail
{
	/// <summary>
	/// enc_inform.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class enc_inform : UserControl
	{
		public enc_inform()
		{
			InitializeComponent();
		}
		enum Option
		{
			item = 0,
			enc_pattern,
			pattern,
			delimiter,
			sub_left_len,
			sub_right_len,
			jumin_check_yn,
			Length
		}
		string[] detail = new string[(int)Option.Length] { "암/복호화에 사용할 Item명", "enc_pattern", "감시하고자 하는 pattern, 정규표현식으로 작성", "구분자", "감시한 패턴에서 왼쪽에서 제외할 크기", "감시한 패턴에서 오른쪽에서 제외할 크기", "jumin_check_yn" };
		object[] initvalue = new object[(int)Option.Length] {"", "", "", "", (Int64)0, (Int64)0, false};
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
			Point pt = btn.PointToScreen(new Point(0, 0));
			wa.Left = pt.X;
			wa.Top = pt.Y;
			if(wa.ShowDialog() != true)
				return;


			for(int i = 0; i < wa.Value.Length; i++)
			{
				string key = ((Option)i).ToString();
				JValue jval = jobj[key] as JValue;
				if(jval == null)
				{
					jobj.Add(new JProperty(key, wa.Value[i]));

					jval = jobj[TailOption.StartDisableProperty + key] as JValue;
					if(jval != null)
						jobj.Remove(TailOption.StartDisableProperty + key);
				}
				else
					jval.Value = wa.Value[i];
			}

			DataContext = null;
			DataContext = jprop;
			ConfigOptionManager.bChanged = true;
		}
	}
}
