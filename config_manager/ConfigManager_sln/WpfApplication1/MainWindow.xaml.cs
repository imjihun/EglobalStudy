using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
	/// <summary>
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainWindow : MetroWindow
	{
		public MainWindow()
		{
			InitializeComponent();
			
			Button btn = new Button();
			btn.Width = 100;
			btn.Height = 25;
			btn.Margin = new Thickness(0, 200, 0, 0);
			btn.Content = "test1";
			grid.Children.Add(btn);

			Button btn2 = MyButton.Create();
			btn2.Margin = new Thickness(0, 250, 0, 0);
			btn2.Content = "test2";
			grid.Children.Add(btn2);

			MyButton2 mbtn = new MyButton2();
			mbtn.Margin = new Thickness(0, 300, 0, 0);
			mbtn.Content = "test3";
			grid.Children.Add(mbtn);
			
			ToggleSwitch ts = new ToggleSwitch();
			ts.Margin = new Thickness(100, 0, 0, 100);
			ts.Header = "ToggleSwitch";
			ts.IsChecked = true;
			ts.OffLabel = "uncheck";
			ts.OnLabel = "check";
			ts.Style = (Style)App.Current.Resources["MahApps.Metro.Styles.ToggleSwitch.Win10"];
			grid.Children.Add(ts);
		}

		public void click(object sender, RoutedEventArgs e)
		{
			Console.WriteLine(sender.GetType());
		}
	}
	class MyButton : ToggleButton
	{
		public MyButton()
			: base()
		{
			Width = 100;
			Height = 25;
		}
		public static Button Create()
		{
			Button btn = new Button();
			btn.Width = 100;
			btn.Height = 25;
			return btn;
		}
	}
	class MyButton2 : MyButton
	{
		public MyButton2()
			: base()
		{
			Width = 100;
			Height = 25;
		}
		public static new Button Create()
		{
			Button btn = new Button();
			btn.Width = 100;
			btn.Height = 25;
			return btn;
		}
	}
}
