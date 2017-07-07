﻿using CofileUI.Classes;
using CofileUI.Windows;
using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace CofileUI.UserControls.ConfigOptions
{
	public interface IOptions
	{
		FrameworkElement GetUIOptionKey(JProperty optionKey, Panel pan_value);
		FrameworkElement GetUIOptionValue(JProperty optionKey, JToken optionValue);
	}
	enum ConfigType
	{
		file = 0,
		sam,
		tail
	}

	public enum GroupBodyStyle
	{
		Basic = 0,
		Radio
	}
	static class ConfigOptionSize
	{
		public static int WIDTH_VALUE = 150;
	}
	static class ConfigOptionManager
	{
		public static char StartDisableProperty = '#';
		public static JToken Root;
		private static UserControl current;
		private static string path;
		public static string Path { get { return path; }
			set {
				path = value;
				if(current as ConfigOptions.File.FileOptions != null)
					((ConfigOptions.File.FileOptions)current).textBlock.Text = "File [ " + FileName + " ]";
				if(current as ConfigOptions.Sam.SamOptions != null)
					((ConfigOptions.Sam.SamOptions)current).textBlock.Text = "Sam [ " + FileName + " ]";
				if(current as ConfigOptions.Tail.TailOptions != null)
					((ConfigOptions.Tail.TailOptions)current).textBlock.Text = "Tail [ " + FileName + " ]";
			}
		}
		private static string FileName {
			get
			{
				string[] split = path.Split('\\');
				return split[split.Length - 1];
			}
		}
		private static bool _bChanged = false;
		public static bool bChanged
		{
			get { return _bChanged; }
			set
			{
				_bChanged = value;
				if(value)
					WindowMain.current.tabItem_Config.Header = "*" + Application.Current.FindResource("MainTab.Config") as string;
				else
					WindowMain.current.tabItem_Config.Header = Application.Current.FindResource("MainTab.Config") as string;

				Console.WriteLine(Root);
			}
		}
		public static void Clear()
		{
			current = null;

			Path = null;
			bChanged = false;
		}
		public static UserControl CreateOption(JToken token)
		{
			if(token["type"] == null)
			{
				Log.PrintError("type = " + "empty", "UserControls.ConfigOptions.ConfigOptionManager.CreateOption");
				WindowMain.current.ShowMessageDialog("Config", "type을 확인 할 수 없습니다.\ntype = " + "empty", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.Affirmative);
				return null;
			}
			else if(token["type"].ToString() == "file")
				current = new ConfigOptions.File.FileOptions() { DataContext = token };
			else if(token["type"].ToString() == "sam")
				current = new ConfigOptions.Sam.SamOptions() { DataContext = token };
			else if(token["type"].ToString() == "tail")
				current = new ConfigOptions.Tail.TailOptions() { DataContext = token };
			else
			{
				Log.PrintError("type = " + token["type"].ToString(), "UserControls.ConfigOptions.ConfigOptionManager.CreateOption");
				WindowMain.current.ShowMessageDialog("Config", "type을 확인 할 수 없습니다.\ntype = " + token["type"].ToString(), MahApps.Metro.Controls.Dialogs.MessageDialogStyle.Affirmative);
				return null;
			}

			Root = token;
			Log.PrintLog("type = " + Root["type"].ToString() , "UserControls.ConfigOptions.ConfigOptionManager.CreateOption");
			return current;
		}

		public class Group
		{
			public FrameworkElement Header { get; set; }
			public int[] Arr { get; set; }

			// Key UI 의 CheckBox 보다 우선순위가 높다.
			private GroupBodyStyle bodyStyle = GroupBodyStyle.Basic;
			public GroupBodyStyle BodyStyle { get { return bodyStyle; } set { bodyStyle = value; } }
			public string RadioButtonGroupName { get; set; }
		}

		private static T Find<T>(FrameworkElement ui) where T: FrameworkElement
		{
			T retval = ui as T;
			if(retval == null)
			{
				var list = ui.FindChildren<T>();
				foreach(var v in list)
				{
					retval = v;
					break;
				}
			}
			return retval;
		}
		public static void CheckedKey(ref JProperty jprop)
		{
			try
			{
				if(jprop.Parent[jprop.Name.TrimStart(ConfigOptionManager.StartDisableProperty)] != null)
				{
					if(jprop != jprop.Parent[jprop.Name.TrimStart(ConfigOptionManager.StartDisableProperty)].Parent)
						jprop.Parent[jprop.Name.TrimStart(ConfigOptionManager.StartDisableProperty)].Parent.Remove();
				}
				JProperty newJprop = new JProperty(jprop.Name.TrimStart(ConfigOptionManager.StartDisableProperty), jprop.Value);
				jprop.Replace(newJprop);
				Log.PrintLog(jprop + " -> " + newJprop, "UserControls.ConfigOptions.ConfigOptionManager.CheckedKey");
				// delegate 에 지역변수를 사용하면 지역변수를 메모리에서 계속 잡고있는다. (전역변수 화 (어디 소속으로 전역변수 인지 모르겠다.))
				jprop = newJprop;

			}
			catch(Exception ex)
			{
				Log.PrintError(ex.Message, "UserControls.ConfigOptions.ConfigOptionManager.CheckedKey");
			}
			ConfigOptionManager.bChanged = true;
		}
		public static void UncheckedKey(ref JProperty jprop)
		{
			try
			{
				if(jprop.Parent[ConfigOptionManager.StartDisableProperty + jprop.Name] != null)
				{
					if(jprop != jprop.Parent[ConfigOptionManager.StartDisableProperty + jprop.Name].Parent)
						jprop.Parent[ConfigOptionManager.StartDisableProperty + jprop.Name].Parent.Remove();
				}
				JProperty newJprop = new JProperty(ConfigOptionManager.StartDisableProperty + jprop.Name, jprop.Value);
				jprop.Replace(newJprop);
				Log.PrintLog(jprop + " -> " + newJprop, "UserControls.ConfigOptions.ConfigOptionManager.UncheckedKey");
				// delegate 에 지역변수를 사용하면 지역변수를 메모리에서 계속 잡고있는다. (전역변수 화 (어디 소속으로 전역변수 인지 모르겠다.))
				jprop = newJprop;
			}
			catch(Exception ex)
			{
				Log.PrintError(ex.Message, "UserControls.ConfigOptions.ConfigOptionManager.UncheckedKey");
			}
			ConfigOptionManager.bChanged = true;
		}

		public delegate FrameworkElement GetUI(int opt, JObject root);
		const int WIDTH_KEY = 450;
		public static void MakeUI(Grid grid, JObject root, string[] detailOptions, Group[] groups,
			GetUI GetUIOptionKey, GetUI GetUIOptionValue)
		{
			for(int i = 0; i < groups.Length; i++)
			{
				Border bd = new Border() {BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(1) };
				Grid grid_group = new Grid();
				grid_group.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
				grid_group.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
				bd.Child = grid_group;
				grid.Children.Add(bd);

				grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
				Grid.SetRow(bd, grid.RowDefinitions.Count - 1);

				Grid grid_group_body = new Grid();
				grid_group_body.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(WIDTH_KEY) });
				grid_group_body.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
				grid_group_body.Margin = new Thickness(15, 0, 0, 0);
				if(groups[i].Header != null)
				{
					grid_group.Children.Add(groups[i].Header);
					Grid.SetRow(groups[i].Header, 0);
				}

				grid_group.Children.Add(grid_group_body);
				Grid.SetRow(grid_group_body, 1);
				
				for(int j = 0; j < groups[i].Arr.Length; j++)
				{
					try
					{
						Grid grid_key = new Grid();
						Grid grid_value = new Grid();

						FrameworkElement ui_key = GetUIOptionKey(groups[i].Arr[j], root);
						if(ui_key == null)
							break;
						StackPanel sp = ui_key as StackPanel;
						if(sp != null && sp.Children.Count > 0)
						{
							CheckBox cb = sp.Children[0] as CheckBox;
							if(cb != null)
							{
								Binding _bd = new Binding("IsChecked") { Source = cb, Mode = BindingMode.OneWay, Converter = new OnlyBooleanConverter() };
								grid_value.SetBinding(Grid.IsEnabledProperty, _bd);
								//grid_value.IsEnabled = cb.IsChecked.Value;
								//cb.Checked += delegate { grid_value.IsEnabled = cb.IsChecked.Value; };
								//cb.Unchecked += delegate { grid_value.IsEnabled = cb.IsChecked.Value; };
							}

							RadioButton rb = sp.Children[0] as RadioButton;
							if(rb != null)
							{
								rb.GroupName = groups[i].RadioButtonGroupName;
								Binding _bd = new Binding("IsChecked") { Source = rb, Mode = BindingMode.OneWay, Converter = new OnlyBooleanConverter() };
								grid_value.SetBinding(Grid.IsEnabledProperty, _bd);

								var rg = grid_group_body.FindChildren<RadioButton>();
								foreach(var v in rg)
								{
									if(v.IsChecked == true)
									{
										rb.IsChecked = false;
										break;
									}
								}
							}
						}

						grid_key.Children.Add(ui_key);

						FrameworkElement ui_value = GetUIOptionValue(groups[i].Arr[j], root);
						if(ui_value == null)
							break;

						grid_value.Children.Add(ui_value);

						if(groups[i].Header == null)
						{
							Grid grid_group_header = new Grid();
							grid_group_header.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(WIDTH_KEY) });
							grid_group_header.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
							grid_group.Children.Add(grid_group_header);
							Grid.SetRow(grid_group_header, 0);

							Grid.SetColumn(grid_key, 0);
							grid_group_header.Children.Add(grid_key);

							Grid.SetColumn(grid_value, 1);
							grid_group_header.Children.Add(grid_value);

							groups[i].Header = grid_group_header;

							ToggleSwitch ts = ui_value as ToggleSwitch;
							if(ts != null)
							{
								Binding _bd = new Binding("IsChecked") { Source = ts, Mode = BindingMode.OneWay, Converter = new OnlyBooleanConverter() };
								grid_group_body.SetBinding(Grid.IsEnabledProperty, _bd);
								//ts.Checked += delegate { grid_group_body.IsEnabled = true; };
								//ts.Unchecked += delegate { grid_group_body.IsEnabled = false; };
							}

							ComboBox cb = ui_value as ComboBox;
							if(cb != null)
							{
								Binding _bd = new Binding("SelectedIndex") { Source = cb, Mode = BindingMode.OneWay, Converter = new Int32ToBooleanConverter(),
									ConverterParameter = Root };
								grid_group_body.SetBinding(Grid.IsEnabledProperty, _bd);
							}
						}
						else
						{
							grid_group_body.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
							int idxRow = grid_group_body.RowDefinitions.Count - 1;

							Grid.SetRow(grid_key, idxRow);
							Grid.SetColumn(grid_key, 0);
							grid_group_body.Children.Add(grid_key);

							Grid.SetRow(grid_value, idxRow);
							Grid.SetColumn(grid_value, 1);
							grid_group_body.Children.Add(grid_value);
						}
					}
					catch(Exception e)
					{
						Console.WriteLine(e.Message);
						continue;
					}
				}
			}
		}

	}

	#region Converter
	public sealed class MultiBinderConverter : IMultiValueConverter
	{
		public object Convert(object[] values, System.Type targetType, object parameter, CultureInfo culture)
		{
			if(values == null)
				return null;

			int i;
			for(i = 0; i < values.Length; i++)
			{
				if(values[i] != null)
					break;
			}

			return System.Convert.ToInt64(values[i]);
		}

		public object[] ConvertBack(object value, System.Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return new object[] { value, value };
		}
	}
	public sealed class OnlyStringConverter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToString(value);
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToString(value);
		}
	}
	public sealed class OnlyBooleanConverter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToBoolean(value);
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToBoolean(value);
		}
	}
	public sealed class OnlyInt64Converter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToInt64(value);
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToInt64(value);
		}
	}
	public sealed class OnlyInt64TailTypeConverter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToInt64(value) - 1;
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToInt64(value) + 1;
		}
	}

	public sealed class Int64ToStringConverter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToString(value);
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToInt64(value);
		}
	}
	public sealed class StringToInt64Converter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToInt64(value);
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToString(value);
		}
	}
	public sealed class Int32ToBooleanConverter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
		{
			JToken jtok = parameter as JToken;
			if(jtok == null || jtok["type"] == null)
				return false;

			if(jtok["type"].ToString() == "sam")
			{
				if(System.Convert.ToInt32(value) == 0)
					return true;
				else
					return false;
			}
			else if(jtok["type"].ToString() == "tail")
			{
				if(System.Convert.ToInt32(value) == 1)
					return true;
				else
					return false;
			}

			return false;
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
		{
			return 0;
		}
	}

	public sealed class MethodToValueConverter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
		{
			var methodName = parameter as string;
			if(value == null || methodName == null)
				return null;
			var methodInfo = value.GetType().GetMethod(methodName, new System.Type[0]);
			if(methodInfo == null)
				return null;
			return methodInfo.Invoke(value, new object[0]);
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException(GetType().Name + " can only be used for one way conversion.");
		}
	}
	#endregion
}
