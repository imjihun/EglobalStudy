using System;
using System.Collections.Generic;
using System.IO;
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

namespace WpfApplication1
{
	/// <summary>
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainWindow : Window
	{
		private static bool Write(string path, string str)
		{
			if(path == null || str == null)
				return false;
			try
			{
				FileStream fs = new FileStream(path, FileMode.Append);

				byte[] buffer/* = new byte[MAX_BUFFER]*/;
				int size_write = 0;

				buffer = Encoding.UTF8.GetBytes(str);
				size_write = buffer.Length;

				fs.Write(buffer, 0, size_write);

				fs.Close();
				return true;
			}
			catch(Exception e)
			{
				Console.WriteLine("[Classes.Log.Write] " + e.Message);
			}
			return false;
		}
		public MainWindow()
		{
			InitializeComponent();
			string str = "1234567890\r\n\r";
			char[] newLines = new char[] {'\n', '\r' };

			string retval = str.Substring(1, str.Length - 1).Trim(newLines);
			Write("test.txt", retval);
			for(int i=0; i< retval.Length;i++)
				Console.WriteLine("{0} {1:x}", i, retval[i]);
		}
	}
}
