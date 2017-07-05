using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace ExcelTest
{
	class Program
	{
		static void Main(string[] args)
		{
			//Console.WriteLine("Start");
			//Data d = new Data();

			//Console.WriteLine("Table Loading..");
			//if(d.GetTable(AppDomain.CurrentDomain.BaseDirectory + @"test.db") < 0)
			//	return;

			//string path = AppDomain.CurrentDomain.BaseDirectory + @"test";
			//Console.WriteLine("Table xls Exporting..");
			//if(d.ExportExcel(true, path + ".xls") == 0)
			//{
			//	Console.WriteLine("Table xls Finish");
			//	Console.WriteLine();
			//}

			//Console.WriteLine("Table xml Exporting..");
			//if(d.ExportXml(path + ".xml") == 0)
			//{
			//	Console.WriteLine("Table xml Finish");
			//	Console.WriteLine();
			//}

			//Console.WriteLine("Table csv Exporting..");
			//if(d.ExportCSV(path + ".csv") == 0)
			//{
			//	Console.WriteLine("Table csv Finish");
			//	Console.WriteLine();
			//}

			//Console.WriteLine("Finish");
		}
	}

	class Data
	{
		DataTable current_table;
		DataTable Current_table { get { return current_table; } set { current_table = value; } }
		public int GetTable(string path)
		{
			try
			{
				string strConn = "Data Source=" + path;
				using(SQLiteConnection conn = new SQLiteConnection(strConn))
				{
					string sql = "SELECT * From log ORDER BY no DESC";
					SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(sql, conn);
					//DataTable[] tables = new DataTable[1] {new DataTable() };
					//dataAdapter.Fill(0, 20000, tables);
					//Current_table = tables[0];

					DataSet dataSet = new DataSet();
					dataAdapter.Fill(dataSet);
					Current_table = dataSet.Tables[0];
					conn.Close();
				}
				Console.WriteLine("Table Load Success");
				return 0;
			}
			catch(Exception e)
			{
				Console.Write("Table Load Fail : ");
				Console.WriteLine(e.Message);
				return -1;
			}
		}
		public int ExportXml(string path)
		{
			Current_table.WriteXml(path);
			return 0;
		}
		public int ExportCSV(string path)
		{
			string delimiter = ",";
			string newLine = Environment.NewLine;
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < Current_table.Rows.Count; i++)
			{
				sb.Append(Current_table.Rows[i].ItemArray[0].ToString());
				for(int j = 1; j < Current_table.Columns.Count; j++)
				{
					sb.Append(delimiter);
					sb.Append(Current_table.Rows[i].ItemArray[j].ToString());
				}
				sb.Append(newLine);
			}
			FileStream fs = File.Open(path, FileMode.Create, FileAccess.Write);
			StreamWriter sw = new StreamWriter(fs);
			sw.Write(sb);

			return 0;
		}

		public int ExportExcel(bool captions, string path)
		{
			//SaveFileDialog saveFileDialog = new SaveFileDialog();
			//this.saveFileDialog.FileName = "TempName";
			//this.saveFileDialog.DefaultExt = "xls";
			//this.saveFileDialog.Filter = "Excel files (*.xls)|*.xls";
			//this.saveFileDialog.InitialDirectory = "c:\\";

			//bool? result = saveFileDialog.ShowDialog();

			if(/*result == */true)
			{
				int num = 0;
				object missingType = Type.Missing;
				
				Excel.Application objApp;
				Excel._Workbook objBook;
				Excel.Workbooks objBooks;
				Excel.Sheets objSheets;
				Excel._Worksheet objSheet;
				Excel.Range range;

				string[] headers = new string[Current_table.Columns.Count];
				string[] columns = new string[Current_table.Columns.Count];
				
				for(int c = 0; c < Current_table.Columns.Count; c++)
				{
					headers[c] = Current_table.Columns[c].ToString();
					num = c + 65;
					columns[c] = Convert.ToString((char)num);
				}

				try
				{
					objApp = new Excel.Application();
					objBooks = objApp.Workbooks;
					objBook = objBooks.Add(Missing.Value);
					objSheets = objBook.Worksheets;
					objSheet = (Excel._Worksheet)objSheets.get_Item(1);
					
					if(captions)
					{
						for(int c = 0; c < Current_table.Columns.Count; c++)
						{
							range = objSheet.get_Range(columns[c] + "1", Missing.Value);
							range.set_Value(Missing.Value, headers[c]);
						}
					}
					

					int consolTop = Console.CursorTop;
					for(int i = 0; i < Current_table.Rows.Count - 1; i++)
					{
						for(int j = 0; j < Current_table.Columns.Count; j++)
						{
							range = objSheet.get_Range(columns[j] + Convert.ToString(i + 2),
																   Missing.Value);
							range.set_Value(Missing.Value,
												  Current_table.Rows[i].ItemArray[j].ToString());
						}
						Console.SetCursorPosition(0, consolTop);
						Console.WriteLine(string.Format("{0:n2}", ((double)i / Current_table.Rows.Count * 100)) + " %");
					}
					Console.SetCursorPosition(0, consolTop);
					Console.WriteLine(string.Format("{0:n2}", 100.00) + " %");

					objApp.Visible = false;
					objApp.UserControl = false;

					objBook.SaveAs(
						//@saveFileDialog.FileName,
						path,
						Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal,
						missingType, missingType, missingType, missingType,
						Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
						missingType, missingType, missingType, missingType, missingType);
					objBook.Close(false, missingType, missingType);


					Console.WriteLine("Table Export Success");
					return 0;
				}
				catch(Exception theException)
				{
					Console.WriteLine("Table Export Fail : " + theException.Message);
					Console.WriteLine("StackTrace = " + theException.StackTrace);
					Console.WriteLine("TargetSite = " + theException.TargetSite);
					Console.WriteLine("InnerException = " + theException.InnerException);
					Console.WriteLine("HelpLink = " + theException.HelpLink);
					Console.WriteLine("Source = " + theException.Source);
					Console.WriteLine("Data = " + theException.Data.ToString());
					return -1;
				}
			}
		}
	}
}
