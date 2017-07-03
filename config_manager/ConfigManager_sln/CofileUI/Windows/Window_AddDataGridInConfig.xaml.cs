using CofileUI.UserControls;
using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace CofileUI.Windows
{

	/// <summary>
	/// Window_AddDataGridInConfig.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class Window_AddDataGridInConfig : Window
	{
		public object[] Value { get; set; }
		public Window_AddDataGridInConfig(string[] detail, object[] InitValue)
		{
			InitializeComponent();

			Value = InitValue;
			this.DataContext = this;
			int i;
			for(i = 0; i < InitValue.Length; i++)
			{
				TextBlock tblock = new TextBlock()
				{
					Text = detail[i],
					Margin = new Thickness(0, i * 30 + 5, 5, 5),
					VerticalAlignment = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Left,
					TextAlignment = TextAlignment.Center,
					Height = 30
				};
				grid.Children.Add(tblock);
				Grid.SetColumn(tblock, 0);

				//Console.WriteLine("InitValue[i] = " + InitValue[i].GetType());
				if(InitValue[i] == null)
					continue;

				if(InitValue[i].GetType() == typeof(bool)
					|| (InitValue[i] as JValue != null && ((JValue)InitValue[i]).Type == JTokenType.Boolean))
				{
					CheckBox cb = new CheckBox()
					{
						IsChecked = Convert.ToBoolean(InitValue[i]),
						Margin = new Thickness(0, i * 30 + 5, 5, 5),
						VerticalAlignment = VerticalAlignment.Top,
						HorizontalAlignment = HorizontalAlignment.Stretch,
						Height = 30
					};
					Binding bd = new Binding("Value[" + i + "]");
					cb.SetBinding(CheckBox.IsCheckedProperty, bd);

					grid.Children.Add(cb);
					Grid.SetColumn(cb, 1);
				}
				else if(InitValue[i].GetType() == typeof(Int64)
					|| (InitValue[i] as JValue != null && ((JValue)InitValue[i]).Type == JTokenType.Integer))
				{
					NumericUpDown nud = new NumericUpDown()
					{
						Value = Convert.ToInt64(InitValue[i]),
						Margin = new Thickness(0, i * 30 + 5, 5, 5),
						VerticalAlignment = VerticalAlignment.Top,
						HorizontalAlignment = HorizontalAlignment.Stretch,
						Height = 30
					};
					Binding bd = new Binding("Value[" + i + "]");
					bd.Converter = new OnlyInt64Converter();
					nud.SetBinding(NumericUpDown.ValueProperty, bd);

					grid.Children.Add(nud);
					Grid.SetColumn(nud, 1);
				}
				else if(InitValue[i].GetType() == typeof(string)
					|| (InitValue[i] as JValue != null && ((JValue)InitValue[i]).Type == JTokenType.String))
				{
					TextBox tb = new TextBox()
					{
						Text = Convert.ToString(InitValue[i]),
						Margin = new Thickness(0, i * 30 + 5, 5, 5),
						VerticalAlignment = VerticalAlignment.Top,
						HorizontalAlignment = HorizontalAlignment.Stretch,
						Height = 30
					};
					Binding bd = new Binding("Value[" + i + "]");
					tb.SetBinding(TextBox.TextProperty, bd);

					grid.Children.Add(tb);
					Grid.SetColumn(tb, 1);
				}

			}
		}
		private void TextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key != Key.Enter)
				return;

			this.DialogResult = true;
			this.Close();
		}

		private void OnButtonClickOk(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			this.Close();
		}
		private void OnButtonClickCancel(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}
	}
}