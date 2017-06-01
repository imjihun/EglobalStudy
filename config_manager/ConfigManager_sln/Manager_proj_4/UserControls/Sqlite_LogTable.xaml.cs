using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
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

namespace Manager_proj_4.UserControls
{
	/// <summary>
	/// UserControl1.xaml에 대한 상호 작용 논리
	/// </summary>

	public class DataBaseInfo
	{
		private const string tmp_filename = "cofile.db";
		public static string Path = @"D:\git\config_manager\ConfigManager_sln\Manager_proj_4\bin\Debug\cofile.db";
		public static string LoadDataBase(string local_file_name)
		{
			Path = SSHController.GetDataBase(AppDomain.CurrentDomain.BaseDirectory, local_file_name);
			return Path;
		}
		public static void RefreshUi(string changed_server_name)
		{
			Path = SSHController.GetDataBase(AppDomain.CurrentDomain.BaseDirectory, changed_server_name);
			if(Sqlite_LogTable.current != null)
				Sqlite_LogTable.current.refresh();
			if(Sqlite_StatusTable.current != null)
				Sqlite_StatusTable.current.refresh();
		}
	}

	public partial class Sqlite_LogTable : UserControl
	{
		public static Sqlite_LogTable current;
		public Sqlite_LogTable()
		{
			current = this;
			InitializeComponent();
			//refresh();
		}
		public void refresh()
		{
			//string strConn = @"Data Source=D:\git\config_manager\ConfigManager_sln\Tests\Test_Sqlite\bin\Debug\cofile.db";
			//using(SQLiteConnection conn = new SQLiteConnection(strConn))
			//{
			//	conn.Open();
			//	string sql = "SELECT * FROM log";

			//	//SQLiteDataReader를 이용하여 연결 모드로 데이타 읽기
			//	SQLiteCommand cmd = new SQLiteCommand(sql, conn);
			//	SQLiteDataReader rdr = cmd.ExecuteReader();
			//	while(rdr.Read())
			//	{
			//		Console.WriteLine(rdr["source"]);
			//	}
			//	rdr.Close();
			//	//DataClasses1DataContext d = new DataClasses1DataContext(conn);
			//}

			//// construct the dataset
			//NorthwindDataSet dataset = new NorthwindDataSet();

			//// use a table adapter to populate the Customers table
			//CustomersTableAdapter adapter = new CustomersTableAdapter();
			//adapter.Fill(dataset.Customers);

			//// use the Customer table as the DataContext for this Window
			//this.DataContext = dataset.Customers.DefaultView;

			try
			{
				dataGrid.Columns.Clear();
				//dataGrid.Items.Clear();
				dataGrid.Items.Refresh();

				//string path = DataBaseInfo.LoadDataBase("log.db");
				//if(path == null)
				//	return;

				string strConn = "Data Source=" + DataBaseInfo.Path;
				using(SQLiteConnection conn = new SQLiteConnection(strConn))
				{
					UpdateDataGrid(conn, "SELECT * From log");
					conn.Close();
				}
				Console.WriteLine("########################################loaded Sqlite_LogTable");
			}
			catch(Exception e)
			{
				Console.WriteLine("[Sqlite_LogTable] " + e.Message);
				//MessageBox.Show(e.Message, "Sqlite_LogTable");
			}

		}


		string[] log_type = new string[] {"sam", "tail", "file" };
		string[] log_action = new string[] {"Encrypt", "Decrypt" };
		string[] log_result = new string[] {"Success", "Fail" };
		private void UpdateDataGrid(SQLiteConnection con, string sql)
		{
			try
			{
				DataSet dataSet = new DataSet();
				SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(sql, con);
				dataAdapter.Fill(dataSet);

				DataTable table = dataSet.Tables[0];

				ChangeColumnIntToString(log_type, table, "type");
				ChangeColumnIntToString(log_action, table, "action");
				ChangeColumnIntToString(log_result, table, "result");

				dataGrid.ItemsSource = dataSet.Tables[0].DefaultView;
			}
			catch(Exception e)
			{
				Console.WriteLine("[Sqlite_LogTable] " + e.Message);
				//MessageBox.Show(e.Message, "Sqlite_LogTable");
			}
		}
		void ChangeColumnIntToString(string[] source, DataTable table, string column_name)
		{
			// 열안에 데이터가 있으면 타입 변경 안됨.
			//table.Columns["type"].DataType = typeof(string)

			// 새로운 열을 추가하는 방식
			// 맨 앞 밑 줄은 DataGrid Column명에 표시 X
			string add_column_name = "_" + column_name;
			DataColumn new_type = new DataColumn(add_column_name, typeof(string));
			table.Columns.Add(new_type);
			int new_idx = table.Columns.IndexOf(column_name);
			new_type.SetOrdinal(new_idx);

			foreach(DataRow v in table.Rows)
			{
				// integer 중 가장 큰 64비트로 캐스팅이 되는지 확인후 캐스팅진행.
				// int a = (short)2; 는 에러가 나지 않음.
				if(typeof(System.Int64).IsAssignableFrom(v[column_name].GetType()))
				{
					System.Int64 idx = (System.Int64)v[column_name];
					v[add_column_name] = source[idx];
				}

				//switch((System.Int64)v[column_name])
				//{
				//	case 0:
				//		v[add_column_name] = "sam";
				//		break;
				//	case 1:
				//		v[add_column_name] = "tail";
				//		break;
				//	case 2:
				//		v[add_column_name] = "file";
				//		break;
				//}
			}

			table.Columns.RemoveAt(new_idx + 1);
		}
	}
}
