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

using System.Net.Http;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.Collections;
using PrimS.Telnet;

namespace test
{
	/// <summary>
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainWindow : Window
	{
		SshClient sshClient;
		Shell shell;
		ShellStream input;
		ShellStream output;
		ShellStream exoutput;
		ShellStream stream;
		public MainWindow()
		{
			InitializeComponent();
			btn_start.Click += Btn_start_Click;
			textBox_input.KeyDown += TextBox_input_KeyDown;
			textBox_result.TextChanged += TextBox_result_TextChanged;

			DispatcherTimer Timer = new DispatcherTimer();
			Timer.Interval = TimeSpan.FromSeconds(0.001);
			Timer.Tick += Timer_Tick;
			Timer.Start();

			btn_start.Focus();


			telnet();
		}

		async void telnet()
		{
			string ip = "192.168.233.133";
			int TimeoutMs = 1000;

			PrimS.Telnet.Client client = new Client(ip, 23, new System.Threading.CancellationToken());
			await client.TryLoginAsync("oracle", "oracle", TimeoutMs);
			await client.WriteLine("whatever command you want to send");
			string response = await client.TerminatedReadAsync(">", TimeSpan.FromMilliseconds(TimeoutMs));
			Console.WriteLine("respons = " + response);
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			if(stream != null)
			{
				//Console.WriteLine("\n\nresult\n" + stream.Read());
				//string str = await read();
				//string str = read();
				//textBox_result.Text += str;
			}
		}
		//async Task<string> read()
		string read()
		{
			int size_buffer = 4096;
			byte[] buffer = new byte[size_buffer];

			//int cnt = stream.Read(buffer, 0, size_buffer);
			//return Encoding.UTF8.GetString(buffer, 0, cnt);

			string line;
			while((line = stream.ReadLine(new TimeSpan(0, 0, 0, 0, 1))) != null)
				Console.WriteLine(line);
			//line = stream.ReadLine(new TimeSpan(0, 0, 1));

			return line;
		}

		private void TextBox_result_TextChanged(object sender, TextChangedEventArgs e)
		{
			textBox_result.ScrollToEnd();
		}
		private void TextBox_input_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key != Key.Enter)
				return;


			//var x = client.RunCommand(textBox_input.Text);
			//Console.WriteLine(x.Result);
			//textBox_result.Text += SendCommand(stream, textBox_input.Text);
			if(sshClient.IsConnected)
			{
				stream.Write(textBox_input.Text);
				stream.Write("\n");
				stream.Flush();
				string str = read();
				textBox_result.Text += str;
				//string str;
				//while((str = stream.ReadLine()) != null)
				//{
				//	Console.WriteLine("read" + str);
				//}
			}
			//input.Write(textBox_input.Text);
			//input.Flush();

			//if(input != null)
			//	Console.WriteLine("input = " + input.Read());
			//if(output != null)
			//	Console.WriteLine("output = " + output.Read());
			//if(exoutput != null)
			//	Console.WriteLine("exoutput = " + exoutput.Read());

			//textBox_input.Text = "";
			//textBox_result.Text += "result\n" + output.Read() + "\n";


			textBox_input.Text = "";
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
				sshClient = new SshClient(ip, 22, name, password);
				sshClient.Connect();
				textBox_input.IsEnabled = true;
				textBox_input.Focus();


				//stream = client.CreateShellStream("xterm", 80, 24, 800, 600, 1024);
				stream = sshClient.CreateShellStream("customCommand", 80, 24, 800, 600, 4096);
				string str = read();
				textBox_result.Text += str;
				//stream = client.CreateShellStream("vt100", 80, 24, 800, 600, 1024);
				//stream = client.CreateShellStream("dumb", 80, 24, 800, 600, 1024);
				//SendCommand(stream, "");
				//test
				//uint[] a = new uint[] {80, 24, 800, 600 };

				//input = client.CreateShellStream("a", a[0], a[1], a[2], a[3], 1024);
				//output = client.CreateShellStream("b", a[0], a[1], a[2], a[3], 1024);
				//exoutput = client.CreateShellStream("c", a[0], a[1], a[2], a[3], 1024);
				//shell = client.CreateShell(input, output, exoutput);
				//shell.Stopped += delegate { Console.WriteLine("shell stoped!!"); };
				//shell.Start();

				//if(input != null)
				//	Console.WriteLine("input = " + input.Read());
				//if(output != null)
				//	Console.WriteLine("output = " + output.Read());
				//if(exoutput != null)
				//	Console.WriteLine("exoutput = " + exoutput.Read());
				////input.Write("ls");
				////Console.WriteLine("result = " + output.Read());

				//SshCommand x = client.RunCommand(command);
				//client.Disconnect();

				//Console.WriteLine();
				//Console.WriteLine("CommandText = " + x.CommandText);
				//Console.WriteLine("CommandTimeout = " + x.CommandTimeout);
				//Console.WriteLine("Error = " + x.Error);
				//Console.WriteLine("ExitStatus = " + x.ExitStatus);
				//Console.WriteLine("Result = " + x.Result);
				//Console.WriteLine("OutputStream = " + x.OutputStream);
				//Console.WriteLine("ExtendedOutputStream = " + x.ExtendedOutputStream);
				//Console.WriteLine();
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			Console.WriteLine("finish");
		}






		//private void btnTelnetConnect_Click(object sender, EventArgs e)
		//{
		//	try
		//	{
		//		TcpClient oClient = new TcpClient("192.168.100.254", 6000);
		//		//l_TelnetState.Text = oClient.Connected.ToString();
		//		if(oClient.Connected)
		//		{
		//			NetworkStream ns = new NetworkStream(oClient.GetStream());
		//			read(ns).ToString();//처음읽으면 이상한값이... 나온다.. 어째서일까..
		//			read(ns).ToString();//로그인 입력하라는 메세지가 읽어진다.
		//			write(ns, "admin");//아이디입력
		//			read(ns).ToString();//아이디 기록 되는지 확인
		//			write(ns, "admin");//암호입력
		//			read(ns).ToString();//비번 기록 되는지 확인
		//			string pc = read(ns);//접속되었는지 확인
		//			pc = pc.Substring(pc.Length - 2, 1);
		//			if(pc == "#") //내가 한 기계에 연결결과 로그인하고 나면 맨 마지막 커맨드에 #이붙어
		//						  //서 이것을 기준으로 접속 되는지 안되는지 확인함. 접속하는것 마다 다
		//						  //를거라고 생각함.
		//			{
		//				l_TelnetState.ForeColor = Color.Green;
		//				l_TelnetState.Text = "연결됨";
		//			}
		//			else
		//			{
		//				l_TelnetState.ForeColor = Color.Red;
		//				l_TelnetState.Text = "연결안됨";
		//			}
		//			ns.Close();
		//			oClient.Close();
		//		}
		//		else
		//		{
		//			l_TelnetState.ForeColor = Color.Red;
		//			l_TelnetState.Text = "연결안됨";
		//		}
		//	}
		//	catch(Exception)
		//	{
		//		l_TelnetState.ForeColor = Color.Red;
		//		l_TelnetState.Text = "연결안됨";
		//		if(oClient.Connected)
		//			oClient.Close();
		//	}
		//}

		//private string read(NetworkStream ns)
		//{
		//	StringBuilder sb = new StringBuilder();

		//	if(ns.CanRead)
		//	{

		//		byte[] readBuffer = new byte[1024];

		//		int numBytesRead = 0;

		//		do
		//		{
		//			numBytesRead = ns.Read(readBuffer, 0, readBuffer.Length);
		//			sb.AppendFormat(
		//			"{0}", Encoding.ASCII.GetString(readBuffer, 0, numBytesRead));
		//			sb.Replace(
		//			Convert.ToChar(24), ' ');
		//			sb.Replace(
		//			Convert.ToChar(255), ' ');
		//			sb.Replace(
		//			'?', ' ');
		//		}

		//		while(ns.DataAvailable);
		//	}

		//	return sb.ToString();
		//}

		//private void write(NetworkStream ns, string message)
		//{

		//	byte[] msg = Encoding.ASCII.GetBytes(message + Environment.NewLine);
		//	ns.Write(msg, 0, msg.Length);
		//}










		private static string SendCommand(ShellStream stream, string customCMD)
		{
			StringBuilder strAnswer = new StringBuilder();

			var reader = new StreamReader(stream);
			var writer = new StreamWriter(stream);
			//writer.AutoFlush = true;
			WriteStream(customCMD, writer, stream);

			strAnswer.AppendLine(ReadStream(reader));
			stream.Flush();

			string answer = strAnswer.ToString();
			return answer;
		}

		private static void WriteStream(string cmd, StreamWriter writer, ShellStream stream)
		{
			writer.WriteLine(cmd);
			writer.Flush();
			while(stream.Length == 0)
				Thread.Sleep(500);
		}

		private static string ReadStream(StreamReader reader)
		{
			StringBuilder result = new StringBuilder();

			string line;
			while((line = reader.ReadLine()) != null)
				result.AppendLine(line);

			return result.ToString();
		}
	}
}
