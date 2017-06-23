using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CofileUI.Classes
{
	class FileContoller
	{
		const int MAX_BUFFER = 4096;
		public static bool CreateDirectory(string path)
		{
			try
			{
				Directory.CreateDirectory(path);
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "Classes.FileContoller.CreateDirectory");
			}
			return true;
		}
		public static void FileDelete(string path)
		{
			try
			{
				File.Delete(path);
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "Classes.FileContoller.Delete");
			}
		}
		public static void DirectoryDelete(string path)
		{
			try
			{
				Directory.Delete(path, true);
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "Classes.FileContoller.Delete");
			}
		}
		public static string Read(string path)
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
				Log.PrintError(e.Message, "Classes.FileContoller.Read");
			}

			return ret_str.ToString();
		}
		public static bool Write(string path, string str)
		{
			if(path == null || str == null)
				return false;
			try
			{
				// 경로에 디렉토리가 없으면 생성
				string dir = path;
				if(path[path.Length - 1] != '\\')
					dir = path.Substring(0, path.LastIndexOf('\\') + 1);
				FileContoller.CreateDirectory(dir);

				// 경로에 파일 쓰기.
				FileStream fs = new FileStream(path, FileMode.Create);

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
				Log.PrintError(e.Message, "Classes.FileContoller.Write");
			}
			return false;
		}
		public static string[] LoadFile(string path, string searchPattern)
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
				Log.PrintError(e.Message, "Classes.FileContoller.LoadFile");
				return null;
			}
		}
	}
}
