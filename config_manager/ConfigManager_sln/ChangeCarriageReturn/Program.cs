using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ChangeCarriageReturn
{
	class Program
	{
		const string TMPFILE_STRING = "_tmp";
		static void convertCheck(string str)
		{
			Encoding encKr = Encoding.GetEncoding("euc-kr");
			EncodingInfo[] encods = Encoding.GetEncodings();
			Encoding destEnc = Encoding.UTF8;

			foreach(EncodingInfo ec in encods)
			{
				Encoding enc = ec.GetEncoding();
				byte[] sorceBytes = enc.GetBytes(str);
				byte[] encBytes = Encoding.Convert(encKr, destEnc, sorceBytes);

				System.Diagnostics.Debug.WriteLine(string.Format("{0}({1}) : {2}", enc.EncodingName, enc.BodyName, destEnc.GetString(encBytes)));
			}
		}

		static int ChangeString(string[] files, string org_str, string new_str)
		{
			try
			{
				for(int i = 0; i < files.Length; i++)
				{
					//string[] str_arr = File.ReadAllLines(files[i], Encoding.UTF8);
					//Console.WriteLine("************************************************************************************");
					//for(int j = 0; j < str_arr.Length; j++)
					//{
					//	Console.WriteLine(str_arr[j]);
					//}
					//Console.WriteLine("************************************************************************************");


					FileStream fs_in, fs_out;
					StreamReader sr;
					char[] buffer = new char[4096];
					int read_len;

					fs_in = File.Open(files[i], FileMode.Open);
					sr = new StreamReader(fs_in, Encoding.UTF8, true);
					fs_out = File.Open(files[i] + TMPFILE_STRING, FileMode.Create);
					while((read_len = sr.Read(buffer, 0, 4096)) > 0)
					{
						//Console.WriteLine(read_len);
						//for(int j = 0; j < read_len; j++)
						//{
						//	Console.Write(buffer[j] + " ");
						//}
						//Console.WriteLine();

						string str = new string(buffer, 0, read_len);
						str = str.Replace(org_str, new_str);
						//Console.WriteLine(str.Length);
						//for(int j = 0; j < str.Length; j++)
						//{
						//	Console.Write(str[j] + " ");
						//}
						//Console.WriteLine();

						byte[] new_buffer = Encoding.UTF8.GetBytes(str);
						fs_out.Write(new_buffer, 0, new_buffer.Length);
						//Console.WriteLine(new_buffer.Length);
						//for(int j = 0; j < new_buffer.Length; j++)
						//{
						//	Console.Write(new_buffer[j] + " ");
						//}
						//Console.WriteLine();
					}
					fs_out.Close();
					fs_in.Close();

					File.Delete(files[i]);
					File.Move(files[i] + TMPFILE_STRING, files[i]);
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
				return -1;
			}
			return 0;
		}

		static int SearchFolder(string folder_path)
		{
			int retval = 0;
			string[] files = Directory.GetFiles(folder_path);
			retval = ChangeString(files, "\r", "");
			if(retval < 0)
				return -1;

			string[] sub_folders = Directory.GetDirectories(folder_path);
			for(int i = 0; i < sub_folders.Length; i++)
			{
				retval = SearchFolder(sub_folders[i]);
				if(retval < 0)
					return -1;
			}
			return 0;
		}
		static void Main(string[] args)
		{
			//if(args.Length < 1)
			//	return;
			int retval = 0;
			retval = SearchFolder("D:\\git\\server");
			//retval = SearchFolder("tmp");
		}
	}
}
