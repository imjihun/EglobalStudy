using CofileUI.Classes;
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
			JToken token = JsonController.ParseJson(Properties.Resources.sam_config_default);
			//treeView.Items.Add(JsonTreeViewItem.convertToTreeViewItem(jtok));

			print(token);

			var children = new List<JToken>();
			if(token != null)
			{
				children.Add(token);
			}

			treeView1.ItemsSource = null;
			treeView1.Items.Clear();
			//treeView1.ItemsSource = children;
			treeView1.ItemsSource = token.Children<JProperty>();
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
