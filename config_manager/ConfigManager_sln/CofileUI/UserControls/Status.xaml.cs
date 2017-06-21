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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CofileUI.UserControls
{
	/// <summary>
	/// Status.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class Status : UserControl
	{
		public static Status current;
		public Status()
		{
			current = this;

			InitializeComponent();
		}
		private void TextBox_TextChanged_ScrollToEnd(object sender, TextChangedEventArgs e)
		{
			//textBox_status.ScrollToEnd();
			richTextBox_status.ScrollToEnd();
		}
		public void Clear()
		{
			TextRange txt = new TextRange(Status.current.richTextBox_status.Document.ContentStart, Status.current.richTextBox_status.Document.ContentEnd);
			txt.Text = "";
		}
	}
}
