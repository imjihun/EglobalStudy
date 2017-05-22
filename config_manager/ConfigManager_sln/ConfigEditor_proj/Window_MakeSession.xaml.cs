using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ConfigEditor_proj
{
	/// <summary>
	/// Window_MakeSession.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class Window_MakeSession : Window
	{
		public Window_MakeSession()
		{
			InitializeComponent();
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
