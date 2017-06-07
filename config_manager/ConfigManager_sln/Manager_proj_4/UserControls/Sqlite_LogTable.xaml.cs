using Manager_proj_4.Classes;
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
		//private const string tmp_filename = "cofile.db";
		public static string Path = @"D:\git\config_manager\ConfigManager_sln\Manager_proj_4\bin\Debug\system\tmp\cofile.db";
		//public static string LoadDataBase(string local_file_name)
		//{
		//	Path = SSHController.GetDataBase(AppDomain.CurrentDomain.BaseDirectory, local_file_name);
		//	return Path;
		//}

		static string DIR = @"system\tmp\";
		static string path_root = AppDomain.CurrentDomain.BaseDirectory + DIR;
		public static void RefreshUi(string changed_server_name)
		{
			string db_name = changed_server_name + ".db";
			Path = SSHController.GetDataBase(path_root, db_name);

			if(Sqlite_LogTable.current != null)
				Sqlite_LogTable.current.Refresh();
			if(Sqlite_StatusTable.current != null)
				Sqlite_StatusTable.current.Refresh();
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
			
			comboBox_type.Items.Add("All");
			comboBox_type.SelectedIndex = 0;
			comboBox_action.Items.Add("All");
			comboBox_action.SelectedIndex = 0;
			comboBox_result.Items.Add("All");
			comboBox_result.SelectedIndex = 0;

			foreach(var v in log_type)
				comboBox_type.Items.Add(v);
			foreach(var v in log_action)
				comboBox_action.Items.Add(v);
			foreach(var v in log_result)
				comboBox_result.Items.Add(v);

			comboBox_change_count_page.ItemsSource = arr_cnt_page;
			comboBox_change_count_page.SelectedIndex = 0;
			comboBox_change_count_page.SelectionChanged += ComboBox_change_count_page_SelectionChanged;
		}

		DataTable table;
		public void Refresh()
		{
			try
			{
				table = new DataTable();
				dataGrid.ItemsSource = table.DefaultView;
				idx_page = 0;
				cnt_page = arr_cnt_page[0];
				max_idx_page = 0;
				//dataGrid.Columns.Clear();
				////dataGrid.Items.Clear();
				//dataGrid.Items.Refresh();

				//string path = DataBaseInfo.LoadDataBase("log.db");
				//if(path == null)
				//	return;

				if(DataBaseInfo.Path == null)
					return;
				string strConn = "Data Source=" + DataBaseInfo.Path;
				using(SQLiteConnection conn = new SQLiteConnection(strConn))
				{
					UpdateDataGrid(conn, "SELECT * From log");
					conn.Close();
				}

				//Log.ViewMessage("Loaded", "Log File", Status.current.richTextBox_status);
				Log.PrintConsole("Loaded", "Sqlite_LogTable");
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "Sqlite_LogTable][Refresh", Status.current.richTextBox_status);
				//Console.WriteLine("[Sqlite_LogTable] " + e.Message);
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

				table = dataSet.Tables[0];

				ChangeColumnIntToString(log_type, table, "type");
				ChangeColumnIntToString(log_action, table, "action");
				ChangeColumnIntToString(log_result, table, "result");
				
				dataGrid.ItemsSource = table.DefaultView;

				max_idx_page = table.Rows.Count / cnt_page;
				RefreshDataView(idx_page, cnt_page);
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "UpdateDataGrid", Status.current.richTextBox_status);
				//Console.WriteLine("[Sqlite_LogTable] " + e.Message);
				//MessageBox.Show(e.Message, "Sqlite_LogTable");
			}
		}
		void ChangeColumnIntToString(string[] source, DataTable _table, string column_name)
		{
			// 열안에 데이터가 있으면 타입 변경 안됨.
			//table.Columns["type"].DataType = typeof(string)

			// 새로운 열을 추가하는 방식
			// 맨 앞 밑 줄은 DataGrid Column명에 표시 X
			string add_column_name = "_" + column_name;
			DataColumn new_type = new DataColumn(add_column_name, typeof(string));
			_table.Columns.Add(new_type);
			int new_idx = _table.Columns.IndexOf(column_name);
			new_type.SetOrdinal(new_idx);

			foreach(DataRow v in _table.Rows)
			{
				// integer 중 가장 큰 64비트로 캐스팅이 되는지 확인후 캐스팅진행.
				// int a = (short)2; 는 에러가 나지 않음.
				if(typeof(System.Int64).IsAssignableFrom(v[column_name].GetType()))
				{
					System.Int64 idx = (System.Int64)v[column_name];
					v[add_column_name] = source[idx];
				}
			}

			_table.Columns.RemoveAt(new_idx + 1);
		}


		#region First Prev Next Last Button click
		static int[] arr_cnt_page = new int[] {10,15,20 };
		private int idx_page = 0;
		private int cnt_page = arr_cnt_page[0];
		private int max_idx_page = 0;

		private void RefreshDataView(int _idx_page, int _cnt_page)
		{
			try
			{
				table.DefaultView.RowFilter = _idx_page * _cnt_page + " < no and no <= " + (_idx_page + 1) * _cnt_page;
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "RefreshDataView");
			}
			label_inform_page.Content = _idx_page * _cnt_page + " ~ " + (_idx_page + 1) * _cnt_page;
			label_total_count.Content = table.Rows.Count;
		}
		private void btnFirst_Click(object sender, RoutedEventArgs e)
		{
			idx_page = 0;
			RefreshDataView(idx_page, cnt_page);
		}
		private void btnPrev_Click(object sender, RoutedEventArgs e)
		{
			idx_page--;
			if(idx_page < 0)
				idx_page = 0;

			RefreshDataView(idx_page, cnt_page);
		}
		private void btnNext_Click(object sender, RoutedEventArgs e)
		{
			idx_page++;
			if(idx_page > max_idx_page)
				idx_page = max_idx_page;
			RefreshDataView(idx_page, cnt_page);
		}
		private void btnLast_Click(object sender, RoutedEventArgs e)
		{
			idx_page = max_idx_page;
			RefreshDataView(idx_page, cnt_page);
		}
		private void ComboBox_change_count_page_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			int prev_idx_row;
			if(idx_page == max_idx_page)
				prev_idx_row = table.Rows.Count;
			else
				prev_idx_row = idx_page * cnt_page;

			cnt_page = arr_cnt_page[comboBox_change_count_page.SelectedIndex];
			max_idx_page = table.Rows.Count / cnt_page;
			idx_page = prev_idx_row / cnt_page;
			RefreshDataView(idx_page, cnt_page);
		}
		#endregion

	}
}
