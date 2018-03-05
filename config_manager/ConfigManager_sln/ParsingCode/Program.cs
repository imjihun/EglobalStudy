using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ParsingCode
{
	class Program
	{
		static void Main(string[] args)
		{
		}
	}


	/**
	 * no type parser
	 * 
	 * [], {}, ()
	 * 
	 * C
	 *		filename .*[.].*c, .*[.]h
	 *		#
	 *			#include
	 *			#if, #ifdef, #ifndef ( #else, #endif )
	 *			#define
	 *			나머지 #으로 시작하는건 무시 (#pragma...)
	 *		keyword
	 *			[unsigned] void, char, short, int, long, long long
	 *				{} depth == 0	: 함수 리턴형.
	 *					다음 문자열 함수명.
	 *				{} depth > 0	: 변수 타입.
	 *			if (else, else if), while, for, switch (case, default), continue, break, goto
	 *		주석
	 *			//, /*, #if 0
	 *			
	 *			
	 *		기능 키워드 검색.
	 */
	class Parse
	{
		const int MAX_BUFFER = 4096;
		int Search(string input_path, string output_path, string str_math, bool is_comment = false)
		{
			try
			{
				FileStream fs_in = new FileStream(input_path, FileMode.Open);
				FileStream fs_out = new FileStream(input_path, FileMode.Append);

				while(true)
				{
					byte[] buffer_in = new byte[MAX_BUFFER];
					int offset_buf_in = 0;
					int len_read = 0;
					len_read = fs_in.Read(buffer_in, offset_buf_in, MAX_BUFFER - offset_buf_in);
					if(len_read > 0)
					{
						byte[] buffer_out = new byte[MAX_BUFFER];
						int offset_buf_out = 0;
						int len_write = 0;

						buffer_out = buffer_in.Clone() as byte[];
						fs_out.Write(buffer_out, offset_buf_out, len_write);
					}
				}
				fs_in.Close();
				fs_out.Close();
			}
			catch(Exception e)
			{
				Console.WriteLine("[Parse.Search] : " + e.Message);
			}
			return 0;
		}
	}

	class FileContoller
	{
		const int MAX_BUFFER = 4096;
		public static int DeleteDirectory(string path)
		{
			try
			{
				Directory.Delete(path, true);
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message + "<path = " + path + ">", "Classes.FileContoller.DeleteDirectory");
			}
			return 0;
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
				Console.WriteLine(e.Message + "<path = " + path + ">", "Classes.FileContoller.Read");
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
				if(!FileContoller.CreateDirectory(dir))
					return false;

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
				Console.WriteLine(e.Message + "<path = " + path + ">", "Classes.FileContoller.Write");
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
				Console.WriteLine(e.Message + "<path = " + path + ">", "Classes.FileContoller.LoadFile");
				return null;
			}
		}
		public static int DeleteFilesInDirectory(string path_dir)
		{
			string[] files;

			DirectoryInfo d = new DirectoryInfo(path_dir);
			if(!d.Exists)
				return 0;

			try
			{
				files = Directory.GetFiles(path_dir);
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message + "<path = " + path_dir + ">", "Classes.FileContoller.DeleteFilesInDirectory");
				return -1;
			}

			for(int i = 0; i < files.Length; i++)
			{
				try
				{
					File.Delete(files[i]);
				}
				catch(Exception e)
				{
					;
				}
			}
			return 0;
		}
	}
}
