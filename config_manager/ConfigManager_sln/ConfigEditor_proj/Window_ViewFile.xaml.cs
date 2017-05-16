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
	/// Window_ViewFile.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class Window_ViewFile : Window
	{
		string path_file;
		public Window_ViewFile(string str_file, string path)
		{
			InitializeComponent();

			tb_file.Text = str_file;
			path_file = path;

			btn_ok.Click += Btn_ok_Click;
			btn_cancel.Click += Btn_cancel_Click;

			string[] splited_path = path.Split('\\');
			this.Title = splited_path[splited_path.Length - 1];
		}
		
		private void Btn_ok_Click(object sender, RoutedEventArgs e)
		{
			FileContoller.write(path_file, tb_file.Text);
			test3.m_wnd.refreshJsonItem();
			this.Close();
		}

		private void Btn_cancel_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
