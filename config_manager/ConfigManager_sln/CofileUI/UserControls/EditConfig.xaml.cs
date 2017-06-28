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

			FileOptions.InitDic();
			SamOptions.InitDic();
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

			panelDetailOption.Children.Clear();
			panelDetailOption.RowDefinitions.Clear();
			//if(jprop_optionMenu.Name == "type")
			//	return;

			AddItem(panelDetailOption, jprop_optionMenu, jprop_optionMenu);
		}
		
		private int AddItem(Panel cur_panel_DetailOption, JProperty cur_jprop_optionMenu, JToken cur_jtok)
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
							FrameworkElement ui = SamOptions.GetUIOptionValue(jprop, v);
							if(ui == null)
								break;

							cur_panel_DetailOption.Children.Add(ui);
						}
						break;

					case JTokenType.Property:
						{
							Grid grid_key = new Grid();
							Grid grid_value = new Grid();
							FrameworkElement ui = SamOptions.GetUIOptionKey((JProperty)v, grid_value);
							if(ui == null)
								break;

							((Grid)pan).RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
							int idxRow = ((Grid)pan).RowDefinitions.Count - 1;
							
							Grid.SetRow(grid_key, idxRow);
							Grid.SetColumn(grid_key, 0);
							cur_panel_DetailOption.Children.Add(grid_key);
							
							Grid.SetRow(grid_value, idxRow);
							Grid.SetColumn(grid_value, 1);
							cur_panel_DetailOption.Children.Add(grid_value);

							grid_key.Children.Add(ui);

							jprop = (JProperty)v;
							pan = grid_value;
						}
						break;
					case JTokenType.Array:
						break;
					case JTokenType.Object:
						{
							((Grid)pan).RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
							int idxRow = ((Grid)pan).RowDefinitions.Count - 1;

							Grid grid = new Grid();
							grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
							grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

							grid.Margin = new Thickness(50, 0, 0, 0);
							Grid.SetRow(grid, idxRow);
							Grid.SetColumn(grid, 0);
							Border bd = new Border() { BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(2) };
							Grid.SetColumnSpan(bd, 2);
							Grid.SetRowSpan(bd, 100);
							grid.Children.Add(bd);

							pan.Children.Add(grid);

							pan = grid;
						}
						break;
					case JTokenType.Raw:
					default:
						break;
				}
				AddItem(pan, jprop, v);
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
