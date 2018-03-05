using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RemoveDupLine
{
	class Program
	{
		static void Main(string[] args)
		{
			string file_path = "";
			if(args.Length > 0)
				file_path = args[0];

			try
			{
				func(file_path);
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		const int MAX_BUFFER = 4096;
		static void func(string file_path)
		{
			FileStream fs = null;
			byte[] buffer = new byte[MAX_BUFFER];
			string str_buf = "";
			string[] line = new string[MAX_BUFFER];
			string[] new_line = new string[] { "\n", "\r\n" };
			int cnt_read;

			fs = File.Open(file_path, FileMode.Open);
			cnt_read = fs.Read(buffer, 0, MAX_BUFFER);
			fs.Seek(cnt_read - Encoding.Default.GetString(buffer).LastIndexOf('\n'), SeekOrigin.Current);
			line = Encoding.Default.GetString(buffer).Split(new_line, StringSplitOptions.RemoveEmptyEntries);

			fs.Close();
		}
	}
}
