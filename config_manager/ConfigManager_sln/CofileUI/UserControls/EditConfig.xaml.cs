using CofileUI.Classes;
using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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

namespace CofileUI.UserControls
{
	/// <summary>
	/// EditConfig.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class EditConfig : UserControl
	{
		public EditConfig()
		{
			InitializeComponent();
			// Object 밑에 
			JToken token = JsonController.ParseJson(Properties.Resources.file_config_default);
			//treeView.Items.Add(JsonTreeViewItem.convertToTreeViewItem(jtok));

			print(token);

			var children = new List<JToken>();
			if(token != null)
			{
				children.Add(token);
			}

			treeView.ItemsSource = null;
			treeView.Items.Clear();
			//treeView1.ItemsSource = children;
			treeView.ItemsSource = token.Children<JProperty>();
		}

		private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			TreeView tv = sender as TreeView;
			if(tv == null)
				return;
			JToken jtok = tv.SelectedItem as JToken;
			if(jtok == null)
				return;

			panelDetailOption.Children.Clear();
			panelDetailOption.RowDefinitions.Clear();
			if(((JProperty)jtok).Name == "type")
				return;

			AddItem(panelDetailOption, jtok);
		}

		string _key = "";
		char StartDisableProperty = '#';
		private int AddItem(Panel pan, JToken jtok)
		{
			foreach(var v in jtok.Children())
			{
				Panel panToAdd = pan;
				switch(v.Type)
				{
					case JTokenType.Boolean:
					case JTokenType.Integer:
					case JTokenType.String:
							pan.Children.Add(ConfigOption.GetUIOptionValue(_key, v));
						break;

					case JTokenType.Property:
						{
							panelDetailOption.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
							CheckBox cb = new CheckBox()
							{
								Content = ConfigOption.GetStringOptionDetail(((JProperty)v).Name.TrimStart(StartDisableProperty))
								,
								Margin = new Thickness(10,3,10,3)
							};
							if(((JProperty)v).Name.Length > 0 && ((JProperty)v).Name[0] != StartDisableProperty)
							{
								cb.IsChecked = true;
								cb.Foreground = Brushes.Black;
							}
							else
							{
								cb.IsChecked = false;
								cb.Foreground = Brushes.Gray;
							}

							int idxRow = panelDetailOption.RowDefinitions.Count - 1;
							Grid.SetRow(cb, idxRow);
							Grid.SetColumn(cb, 0);
							pan.Children.Add(cb);


							Grid grid_value = new Grid();
							grid_value.IsEnabled = cb.IsChecked.Value;
							Grid.SetRow(grid_value, idxRow);
							Grid.SetColumn(grid_value, 1);
							pan.Children.Add(grid_value);
							panToAdd = grid_value;

							// delegate 에 지역변수를 사용하면 지역변수를 메모리에서 계속 잡고있는다. (전역변수 화 (어디 소속으로 전역변수 인지 모르겠다.))
							JProperty jprop = v as JProperty;
							cb.Checked += delegate
							{
								JProperty newJprop = new JProperty(jprop.Name.TrimStart(StartDisableProperty), jprop.Value);
								jprop.Replace(newJprop);
								// delegate 에 지역변수를 사용하면 지역변수를 메모리에서 계속 잡고있는다. (전역변수 화 (어디 소속으로 전역변수 인지 모르겠다.))
								jprop = newJprop;

								grid_value.IsEnabled = cb.IsChecked.Value;
								cb.Foreground = Brushes.Black;
							};
							cb.Unchecked += delegate
							{
								JProperty newJprop = new JProperty(StartDisableProperty + jprop.Name, jprop.Value);
								jprop.Replace(newJprop);
								// delegate 에 지역변수를 사용하면 지역변수를 메모리에서 계속 잡고있는다. (전역변수 화 (어디 소속으로 전역변수 인지 모르겠다.))
								jprop = newJprop;

								grid_value.IsEnabled = cb.IsChecked.Value;
								cb.Foreground = Brushes.Gray;
							};
							_key = jprop.Name.TrimStart(StartDisableProperty);
						}
						break;
					case JTokenType.Array:
					case JTokenType.Object:
					case JTokenType.Raw:
					default:
						break;
				}
				AddItem(panToAdd, v);
			}
			return 0;
		}





		int depth = -1;
		void print(JToken cur)
		{
			depth++;
			foreach(var v in cur.Children())
			{
				Console.Write("\t" + depth + " : ");
				for(int i = 0; i < depth; i++)
					Console.Write("  ");
				Console.WriteLine(v.Type);
				print(v);
			}
			depth--;
		}

	}
	public sealed class MethodToValueConverter2 : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			JProperty jprop = value as JProperty;

			var methodName = parameter as string;
			if(value == null || methodName == null)
				return null;
			var methodInfo = value.GetType().GetMethod(methodName, new Type[0]);
			if(methodInfo == null)
				return null;

			// value 가 JProperty 이고, JProperty.value 가 JObject 일 때 리턴
			JToken j = value as JToken;
			if(j == null)
				return null;
			if(j.Children().Children().Children().Children().Count() <= 0)
				return null;

			return j.Children().Children();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException(GetType().Name + " can only be used for one way conversion.");
		}
	}
	public sealed class MethodToValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var methodName = parameter as string;
			if(value == null || methodName == null)
				return null;
			var methodInfo = value.GetType().GetMethod(methodName, new Type[0]);
			if(methodInfo == null)
				return null;
			return methodInfo.Invoke(value, new object[0]);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException(GetType().Name + " can only be used for one way conversion.");
		}
	}
}
