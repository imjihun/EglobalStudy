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

namespace Manager_proj_4
{
	/// <summary>
	/// Window_MakeSession.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class Window_AddServer : Window
	{
		public Window_AddServer()
		{
			InitializeComponent();
			textBox_name.Focus();
			textBox_name.KeyDown += TextBox_KeyDown;
			textBox_ip.KeyDown += TextBox_KeyDown;
			textBox_id.KeyDown += TextBox_KeyDown;
			textBox_password.KeyDown += TextBox_KeyDown;
			//textBox_password.PasswordChanged += TextBox_password_PasswordChanged;
		}

		//private void TextBox_password_PasswordChanged(object sender, RoutedEventArgs e)
		//{
		//	if(textBox_password.Password.Length == 0)
		//	{
		//		textBlock_password_hint.Visibility = Visibility.Visible;
		//	}
		//	else
		//	{
		//		textBlock_password_hint.Visibility = Visibility.Hidden;
		//	}
		//}

		private void TextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key != Key.Enter)
				return;

			this.DialogResult = true;
			this.Close();
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
