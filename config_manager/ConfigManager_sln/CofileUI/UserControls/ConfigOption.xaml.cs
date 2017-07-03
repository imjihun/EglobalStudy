using CofileUI.Classes;
using CofileUI.UserControls.ConfigOptions;
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
	public partial class ConfigOption : UserControl
	{
		Options options;
		public ConfigOption()
		{
			InitializeComponent();

			//FileOptions.InitDic();
			//SamOptions.InitDic();
			FileOptions fo = new FileOptions();
			SamOptions so = new SamOptions();

			options = so;
			Refresh(Properties.Resources.sam_config_default);
		}

		private void Refresh(string Json)
		{
			JToken token = JsonController.ParseJson(Json);

			//print(token);

			var children = new List<JToken>();
			if(token != null)
			{
				children.Add(token);
			}

			treeView.ItemsSource = null;
			treeView.Items.Clear();
			//treeView1.ItemsSource = children;
			
			if(token as JObject != null)
				treeView.ItemsSource = token.Children();

			//TreeView tv = ConfigOption.GetUIOptionMenus((JObject)token);
			//tv.SelectedItemChanged += treeView_SelectedItemChanged;
			//panelOptionMenu.Children.Add(tv);
		}
		private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			TreeView tv = sender as TreeView;
			if(tv == null)
				return;
			
			JProperty jprop_optionMenu = tv.SelectedItem as JProperty;
			if(jprop_optionMenu == null)
				return;

			panel_DetailOption.Children.Clear();
			panel_DetailOption.RowDefinitions.Clear();

			AddItem(panel_DetailOption, jprop_optionMenu);
		}
		public int AddItem(Grid panel, JProperty root)
		{
			if(root.Value.Type == JTokenType.Object)
			{
				//panel.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
				//panel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

				Panel _pan = GetUIOptionPanel();
				panel.Children.Add(_pan);
				return AddItemValueIsObject(_pan, root, root);
			}
			else if(root.Value.Type == JTokenType.Array)
			{
				//StackPanel sp = new StackPanel();
				//panel.Children.Add(sp);
				//return AddItemValueIsArray(sp, root, root, false);

				string key = root.Name.TrimStart('#');
				UIElement ui = null;
				switch(key)
				{
					case "col_var":
						ui = new ConfigOptions.Sam.col_var() { DataContext = root };
						//ui = Resources["DataGridResourceSamColVar"] as DataGrid;
						break;
					case "col_fix":
						ui = new ConfigOptions.Sam.col_fix() { DataContext = root };
						//ui = Resources["DataGridResourceSamColFix"] as DataGrid;
						break;
					case "enc_inform":
						ui = new ConfigOptions.Sam.col_fix() { DataContext = root };
						//ui = Resources["DataGridResourceTailEncInform"] as DataGrid;
						break;
					default:
						break;
				}
				if(ui != null)
				{
					//ui.ItemsSource = root.Value;
					panel.Children.Add(ui);
				}

				return 0;
			}
			else
				return -1;
		}
		Panel GetUIOptionPanel()
		{
			Grid grid = new Grid();
			//Border bd = new Border() {BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(2) };
			//Grid.SetColumnSpan(bd, 50);
			//Grid.SetRowSpan(bd, 50);
			grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
			grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
			//grid.Children.Add(bd);
			return grid;
		}
		private int AddItemValueIsObject(Panel cur_panel_DetailOption, JProperty cur_jprop_optionMenu, JToken cur_jtok)
		{
			foreach(var v in cur_jtok.Children())
			{
				JProperty jprop = cur_jprop_optionMenu;
				Panel pan = cur_panel_DetailOption;
				switch(v.Type)
				{
					case JTokenType.Boolean:
					case JTokenType.Integer:
					case JTokenType.String:
						{
							FrameworkElement ui = options.GetUIOptionValue(jprop, v);
							if(ui == null)
								break;

							pan.Children.Add(ui);
						}
						break;

					case JTokenType.Property:
						{
							Grid grid_key = new Grid();
							Grid grid_value = new Grid();
							FrameworkElement ui = options.GetUIOptionKey((JProperty)v, grid_value);
							if(ui == null)
								break;

							((Grid)pan).RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
							int idxRow = ((Grid)pan).RowDefinitions.Count - 1;

							Grid.SetRow(grid_key, idxRow);
							Grid.SetColumn(grid_key, 0);
							pan.Children.Add(grid_key);

							Grid.SetRow(grid_value, idxRow);
							Grid.SetColumn(grid_value, 1);
							pan.Children.Add(grid_value);

							grid_key.Children.Add(ui);

							jprop = (JProperty)v;
							pan = grid_value;
						}
						break;
					case JTokenType.Array:
					case JTokenType.Object:
					case JTokenType.Raw:
					default:
						break;
				}

				AddItemValueIsObject(pan, jprop, v);
			}
			return 0;
		}
		private int AddItemValueIsArray(Panel cur_panel_DetailOption, JProperty cur_jprop_optionMenu, JToken cur_jtok, bool cur_bArray)
		{
			foreach(var v in cur_jtok.Children())
			{
				JProperty jprop = cur_jprop_optionMenu;
				Panel pan = cur_panel_DetailOption;
				bool bArray = cur_bArray;
				switch(v.Type)
				{
					case JTokenType.Boolean:
					case JTokenType.Integer:
					case JTokenType.String:
						{
							FrameworkElement ui = options.GetUIOptionValue(jprop, v);
							if(ui == null)
								break;

							pan.Children.Add(ui);
						}
						break;

					case JTokenType.Property:
						{
							Grid grid_key = new Grid();
							Grid grid_value = new Grid();
							FrameworkElement ui = options.GetUIOptionKey((JProperty)v, grid_value);
							if(ui == null)
								break;

							((Grid)pan).RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
							int idxRow = ((Grid)pan).RowDefinitions.Count - 1;

							Grid.SetRow(grid_key, idxRow);
							Grid.SetColumn(grid_key, 0);
							pan.Children.Add(grid_key);

							Grid.SetRow(grid_value, idxRow);
							Grid.SetColumn(grid_value, 1);
							pan.Children.Add(grid_value);

							grid_key.Children.Add(ui);

							jprop = (JProperty)v;
							pan = grid_value;
						}
						break;
					case JTokenType.Object:
						if(bArray)
						{
							//Border bd = new Border() {BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(2) };
							//Grid grid = new Grid();
							//grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
							//grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
							//bd.Child = grid;
							Panel _pan = GetUIOptionPanel();
							pan.Children.Add(_pan);
							pan = _pan;
						}
						break;
					case JTokenType.Array:
					case JTokenType.Raw:
					default:
						break;
				}
				if(v.Type == JTokenType.Array)
					bArray = true;
				else
					bArray = false;

				AddItemValueIsArray(pan, jprop, v, bArray);
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

	public class OnlyStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToString(value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToString(value);
		}
	}
	public class OnlyBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToBoolean(value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToBoolean(value);
		}
	}
	public class OnlyInt64Converter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToInt64(value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToInt64(value);
		}
	}
	public sealed class Int64ToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToString(value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToInt64(value);
		}
	}
	public sealed class StringToInt64Converter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToInt64(value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToString(value); 
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
