using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;

namespace Test_Sqlite
{
	class Program
	{
		static void Main(string[] args)
		{
			string strConn = @"Data Source=D:\git\config_manager\ConfigManager_sln\Tests\Test_Sqlite\bin\Debug\cofile.db";
			using(SQLiteConnection conn = new SQLiteConnection(strConn))
			{
				conn.Open();
				string sql = "SELECT * FROM log";

				//SQLiteDataReader를 이용하여 연결 모드로 데이타 읽기
				SQLiteCommand cmd = new SQLiteCommand(sql, conn);
				SQLiteDataReader rdr = cmd.ExecuteReader();
				while(rdr.Read())
				{
					Console.WriteLine(rdr["source"]);
				}
				rdr.Close();
				//DataClasses1DataContext d = new DataClasses1DataContext(conn);

			}
		}
		private static DataSet Select_Adapter()
		{
			DataSet ds = new DataSet();
			string connStr = @"Data Source=C:\Temp\mydb.db";

			//SQLiteDataAdapter 클래스를 이용 비연결 모드로 데이타 읽기
			string sql = "SELECT * FROM member";
			var adpt = new SQLiteDataAdapter(sql, connStr);
			adpt.Fill(ds);

			return ds;
		}
	}
}
