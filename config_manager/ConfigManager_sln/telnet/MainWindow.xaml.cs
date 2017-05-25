using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
using System.Windows.Threading;

namespace telnet
{
	/// <summary>
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainWindow : Window
	{
		Socket client;
		public MainWindow()
		{
			InitializeComponent();

			btn_start.Click += Btn_start_Click;
			btn_start.Focus();

			DispatcherTimer timer_read = new DispatcherTimer();
			timer_read.Interval = TimeSpan.FromSeconds(0.001);
			timer_read.Tick += Timer_read_Tick;
			timer_read.Start();
		}

		private void Timer_read_Tick(object sender, EventArgs e)
		{
			if(client == null || !client.Connected)
				return;

			client.

		}

		private void Btn_start_Click(object sender, RoutedEventArgs e)
		{
			string ip = tb_ip.Text;
			int port = 23;
			string id = tb_name.Text;
			string password = tb_password.Text;

			try
			{
				client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				client.Connect(ip, port);
				if(!client.Connected)
					return;


			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			tb_command.Focus();
		}
	}
}
