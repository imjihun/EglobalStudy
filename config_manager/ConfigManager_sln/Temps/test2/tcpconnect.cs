using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace test2
{
	class tcpconnect
	{

		public void btnTelnetConnect_Click()
		{
			TcpClient oClient = new TcpClient();
			try
			{
				Console.WriteLine("연결시도");
				oClient.Connect("192.168.233.133", 23);
				//l_TelnetState.Text = oClient.Connected.ToString();
				if(oClient.Connected)
				{
					NetworkStream ns = oClient.GetStream();
					read(ns).ToString();//처음읽으면 이상한값이... 나온다.. 어째서일까..
					read(ns).ToString();//로그인 입력하라는 메세지가 읽어진다.
					write(ns, "root");//아이디입력
					read(ns).ToString();//아이디 기록 되는지 확인
					write(ns, "root00");//암호입력
					read(ns).ToString();//비번 기록 되는지 확인
					string pc = read(ns);//접속되었는지 확인
					pc = pc.Substring(pc.Length - 2, 1);
					if(pc == "#") //내가 한 기계에 연결결과 로그인하고 나면 맨 마지막 커맨드에 #이붙어
								  //서 이것을 기준으로 접속 되는지 안되는지 확인함. 접속하는것 마다 다
								  //를거라고 생각함.
					{
						Console.WriteLine("연결됨");
						//l_TelnetState.ForeColor = Color.Green;
						//l_TelnetState.Text = "연결됨";
					}
					else
					{
						Console.WriteLine("연결안됨");
						//l_TelnetState.ForeColor = Color.Red;
						//l_TelnetState.Text = "연결안됨";
					}
					ns.Close();
					oClient.Close();
				}
				else
				{
					Console.WriteLine("연결안됨");
					//l_TelnetState.ForeColor = Color.Red;
					//l_TelnetState.Text = "연결안됨";
				}
			}
			catch(Exception)
			{
				Console.WriteLine("연결안됨");
				//l_TelnetState.ForeColor = Color.Red;
				//l_TelnetState.Text = "연결안됨";
				if(oClient.Connected)
					oClient.Close();
			}
		}

		private string read(NetworkStream ns)
		{
			StringBuilder sb = new StringBuilder();

			if(ns.CanRead)
			{

				byte[] readBuffer = new byte[1024];

				int numBytesRead = 0;

				do
				{
					numBytesRead = ns.Read(readBuffer, 0, readBuffer.Length);
					sb.AppendFormat(
					"{0}", Encoding.ASCII.GetString(readBuffer, 0, numBytesRead));
					sb.Replace(
					Convert.ToChar(24), ' ');
					sb.Replace(
					Convert.ToChar(255), ' ');
					sb.Replace(
					'?', ' ');
				}

				while(ns.DataAvailable);
			}
			Console.WriteLine("read = " + sb.ToString());
			return sb.ToString();
		}

		private void write(NetworkStream ns, string message)
		{
			Console.WriteLine("write = " + message.ToString());
			byte[] msg = Encoding.ASCII.GetBytes(message + Environment.NewLine);
			ns.Write(msg, 0, msg.Length);
		}

	}
}
