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

namespace Test_Parent
{
	/// <summary>
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			TreeViewItem tvi1 = new TreeViewItem();
			TreeViewItem tvi2 = new TreeViewItem();
			TreeViewItem tvi3 = new TreeViewItem();
			tvi1.Items.Add(tvi2);
			tvi1.Items.Add(tvi3);

			TreeViewItem tvi4 = tvi2;
			Console.WriteLine("tvi4 = " + tvi4.Parent);
			tvi4 = tvi3;
			Console.WriteLine("tvi4 = " + tvi4.Parent);
			tvi4 = tvi1;
			Console.WriteLine("tvi4 = " + tvi4.Parent);

			Console.WriteLine("tvi1 = " + tvi1.Parent);
			Console.WriteLine("tvi2 = " + tvi2.Parent);
			Console.WriteLine("tvi3 = " + tvi3.Parent);
		}
	}
}
