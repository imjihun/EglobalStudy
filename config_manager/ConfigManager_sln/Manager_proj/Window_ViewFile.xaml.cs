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

namespace Manager_proj
{
	/// <summary>
	/// Window_ViewFile.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class Window_ViewFile : Window
	{
		public Window_ViewFile(string str_file, string filename)
		{
			InitializeComponent();

			tb_file.Text = str_file;

			btn_ok.Click += Btn_ok_Click;
			btn_cancel.Click += Btn_cancel_Click;
			
			this.Title = filename;

			tb_file.Focus();
		}
		
		private void Btn_ok_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			this.Close();
		}

		private void Btn_cancel_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}
	}
}
