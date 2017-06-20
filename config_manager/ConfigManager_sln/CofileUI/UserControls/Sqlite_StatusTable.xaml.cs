using CofileUI.Classes;
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

namespace CofileUI.UserControls
{
	/// <summary>
	/// Sqlite_StatusTable.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class Sqlite_StatusTable : UserControl
	{
		public static Sqlite_StatusTable current;
		public Sqlite_StatusTable()
		{
			current = this;
			InitializeComponent();
		}
		public void Refresh()
		{
			try
			{
				dataGrid.ItemsSource = new DataTable().DefaultView;
				//dataGrid.Columns.Clear();
				////dataGrid.Items.Clear();
				//dataGrid.Items.Refresh();

				//string path = DataBaseInfo.LoadDataBase("status.db");
				//if(path == null)
				//	return;

				if(DataBaseInfo.Path == null)
					return;
				string strConn = "Data Source=" + DataBaseInfo.Path;
				using(SQLiteConnection conn = new SQLiteConnection(strConn))
				{
					UpdateDataGrid(conn, "SELECT * From status");
				}
				//Log.ViewMessage("Loaded", "Status File", Status.current.richTextBox_status);
				Log.PrintConsole("Loaded", "Sqlite_StatusTable");
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "Sqlite_StatusTable][Refresh", Status.current.richTextBox_status);
			}

			//Console.WriteLine(this.);
		}

		string[] status_type = new string[] {"sam", "tail", "file" };
		private void UpdateDataGrid(SQLiteConnection con, string sql)
		{
			try
			{
				DataSet dataSet = new DataSet();
				SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(sql, con);
				dataAdapter.Fill(dataSet);

				DataTable table = dataSet.Tables[0];

				ChangeColumnIntToString(status_type, table, "type");

				dataGrid.ItemsSource = table.DefaultView;
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "UpdateDataGrid", Status.current.richTextBox_status);
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
			}

			table.Columns.RemoveAt(new_idx + 1);
		}


	}
}
