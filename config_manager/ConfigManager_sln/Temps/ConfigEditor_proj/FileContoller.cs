using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager_proj_4
{
	class FileContoller
	{
		const int MAX_BUFFER = 4096;
		public static string read(string path)
		{
			if(path == null)
				return null;

			StringBuilder ret_str = new StringBuilder("");
			try
			{
				FileStream fs = new FileStream(path, FileMode.Open);

				byte[] buffer = new byte[MAX_BUFFER];
				int size_read = 0;
				ret_str = new StringBuilder();

				while((size_read = fs.Read(buffer, 0, MAX_BUFFER)) > 0)
					ret_str.Append(Encoding.UTF8.GetString(buffer, 0, size_read));

				fs.Close();
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}

			return ret_str.ToString();
		}
		public static void write(string path, string str)
		{
			if(path == null || str == null)
				return;
			try
			{
				FileStream fs = new FileStream(path, FileMode.Create);

				byte[] buffer/* = new byte[MAX_BUFFER]*/;
				int size_write = 0;

				buffer = Encoding.UTF8.GetBytes(str);
				size_write = buffer.Length;

				fs.Write(buffer, 0, size_write);

				fs.Close();
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}
		public static string[] loadFile(string path, string searchPattern)
		{
			DirectoryInfo d = new DirectoryInfo(path);
			if(!d.Exists)
				return null;
			try
			{
				return Directory.GetFiles(path, searchPattern);
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
				return null;
			}
		}
	}
}
