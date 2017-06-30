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
	/// col_fix.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class col_fix : UserControl
	{
		public col_fix()
		{
			InitializeComponent();
			this.Loaded += Col_fix_Loaded;
		}

		private void Col_fix_Loaded(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("data = " + this.DataContext);
		}
	}
}
