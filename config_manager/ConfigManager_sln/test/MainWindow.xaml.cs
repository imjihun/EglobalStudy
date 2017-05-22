using Renci.SshNet;
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

namespace test
{
	/// <summary>
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			btn_start.Click += Btn_start_Click;
		}

		private void Btn_start_Click(object sender, RoutedEventArgs e)
		{
			string ip = tb_ip.Text;
			string name = tb_name.Text;
			string password = tb_password.Text;
			string command = tb_command.Text;

			Console.WriteLine("work");
			try
			{
				SshClient client = new SshClient(ip, 22, name, password);
				client.Connect();
				SshCommand x = client.RunCommand(command);
				client.Disconnect();

				Console.WriteLine();
				Console.WriteLine("CommandText = " + x.CommandText);
				Console.WriteLine("CommandTimeout = " + x.CommandTimeout);
				Console.WriteLine("Error = " + x.Error);
				Console.WriteLine("ExitStatus = " + x.ExitStatus);
				Console.WriteLine("Result = " + x.Result);
				Console.WriteLine("OutputStream = " + x.OutputStream);
				Console.WriteLine("ExtendedOutputStream = " + x.ExtendedOutputStream);
				Console.WriteLine();
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			Console.WriteLine("finish");
		}
	}
}
