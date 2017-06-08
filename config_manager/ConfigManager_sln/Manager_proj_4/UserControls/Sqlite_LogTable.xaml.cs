using Manager_proj_4.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
			
			foreach(var v in log_type)
				comboBox_type.Items.Add(v);
			foreach(var v in log_action)
				comboBox_action.Items.Add(v);
			foreach(var v in log_result)
				comboBox_result.Items.Add(v);

			comboBox_type.Items.Add("All");
			comboBox_type.SelectedIndex = comboBox_type.Items.Count - 1;
			comboBox_action.Items.Add("All");
			comboBox_action.SelectedIndex = comboBox_action.Items.Count - 1;
			comboBox_result.Items.Add("All");
			comboBox_result.SelectedIndex = comboBox_result.Items.Count - 1;

			comboBox_type.DataContext = log_type;
			comboBox_action.DataContext = log_action;
			comboBox_result.DataContext = log_result;

			comboBox_change_count_page.ItemsSource = arr_cnt_page;
			comboBox_change_count_page.SelectedIndex = 0;
			comboBox_change_count_page.SelectionChanged += ComboBox_change_count_page_SelectionChanged;
		}
		DataTable current_table;
		DataTable Current_table { get { return current_table; } set { current_table = value; } }
		DataRow[] filtering_table;
		DataRow[] Filtering_row
		{
			get { return filtering_table; }
			set
			{
				filtering_table = value;
				max_idx_page = (Filtering_row.Length - 1) / cnt_page;
			}
		}
		public void Refresh()
		{
			try
			{
				Current_table = new DataTable();
				dataGrid.ItemsSource = Current_table.DefaultView;
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
				SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(sql, con);

				DataTable[] tables = new DataTable[1] {new DataTable() };
				dataAdapter.Fill(0, 600, tables);
				Current_table = tables[0];

				//DataSet dataSet = new DataSet();
				//dataAdapter.Fill(dataSet);
				//current_table = dataSet.Tables[0];

				ChangeColumnIntToString(log_type, Current_table, "type");
				ChangeColumnIntToString(log_action, Current_table, "action");
				ChangeColumnIntToString(log_result, Current_table, "result");
				
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

		string filter_total = "";
		bool bFiltering = false;
		enum Filter_Idx
		{
			source_target = 0,
			type,
			action,
			result,
			MemberCount
		}
		string[] filter_string = new string[(int)Filter_Idx.MemberCount];
		int backup_idx_page = 0;
		private void OnKeyDownFilter(object sender, KeyEventArgs e)
		{
			if(e.Key != Key.Enter)
				return;

			if(textBox_filter.Text == "")
			{
				if(bFiltering)
				{
					idx_page = backup_idx_page;
					filter_string[(int)Filter_Idx.source_target] = null;
					bFiltering = false;

					RefreshDataView(idx_page, cnt_page);
				}
			}
			else
			{
				backup_idx_page = idx_page;
				idx_page = 0;
				//filter = "and (source LIKE '*" + textBox_filter.Text + "*' or target LIKE '*" + textBox_filter.Text + "*')";
				filter_string[(int)Filter_Idx.source_target] = "(source LIKE '*" + textBox_filter.Text + "*' or target LIKE '*" + textBox_filter.Text + "*')";
				bFiltering = true;

				RefreshDataView(idx_page, cnt_page);
			}
		}
		private void OnComboBoxChangedType(object sender, SelectionChangedEventArgs e)
		{
			if(comboBox_type.SelectedIndex < log_type.Length)
			{
				filter_string[(int)Filter_Idx.type] = "(_type = '" + log_type[comboBox_type.SelectedIndex] + "')";
				backup_idx_page = idx_page;
				idx_page = 0;
				RefreshDataView(idx_page, cnt_page);
			}
			else
			{
				filter_string[(int)Filter_Idx.type] = null;
				RefreshDataView(idx_page, cnt_page);
			}
			e.Handled = true;
		}
		private void OnComboBoxChangedAction(object sender, SelectionChangedEventArgs e)
		{
			if(comboBox_action.SelectedIndex < log_action.Length)
			{
				filter_string[(int)Filter_Idx.action] = "(_action = '" + log_action[comboBox_action.SelectedIndex] + "')";
				backup_idx_page = idx_page;
				idx_page = 0;
				RefreshDataView(idx_page, cnt_page);
			}
			else
			{
				filter_string[(int)Filter_Idx.action] = null;
				RefreshDataView(idx_page, cnt_page);
			}
			e.Handled = true;
		}
		private void OnComboBoxChangedResult(object sender, SelectionChangedEventArgs e)
		{
			if(comboBox_result.SelectedIndex < log_result.Length)
			{
				filter_string[(int)Filter_Idx.result] = "(_result = '" + log_result[comboBox_result.SelectedIndex] + "')";
				backup_idx_page = idx_page;
				idx_page = 0;
				RefreshDataView(idx_page, cnt_page);
			}
			else
			{
				filter_string[(int)Filter_Idx.result] = null;
				RefreshDataView(idx_page, cnt_page);
			}
			e.Handled = true;
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
				//string filter_page = _idx_page * _cnt_page + " < no and no <= " + (_idx_page + 1) * _cnt_page;
				//if(bFiltering)
				//{
				//	//current_table.DefaultView.RowFilter = filter_page + filter;
				//}
				//else
				//{
				//	//DataView dv = (from t in current_table.AsEnumerable()
				//	//			   //where r.IsMatch(t.Field<string>("SOME_COL"))
				//	//			   select t).AsDataView();
				//	//dataGrid.ItemsSource = current_table.AsEnumerable().Take(10).CopyToDataTable().DefaultView;
				//	DataView dv = (from t in current_table.AsEnumerable()
				//				   select t).AsDataView();
				//	//current_table.DefaultView.RowFilter = filter_page;

				//	//ObservableCollection<object> ItemsSource = new ObservableCollection<object>();
				////	for(int i = 0; i < cnt_page; i++)
				////	{
				////		ItemsSource.Add(_newTable.Rows[idx_page * cnt_page + i]);
				////	}
				////	dataGrid.ItemsSource = ItemsSource.CopyToDataTable().DefaultView;
				//}
				dataGrid.ItemsSource = new DataView();

				Filtering_row = Current_table.Select();
				for(int i = 0; i < (int)Filter_Idx.MemberCount && Filtering_row.Length > 0; i++)
				{
					if(filter_string[i] == null || filter_string[i] == "")
						continue;

					Filtering_row = Filtering_row.CopyToDataTable().Select(filter_string[i]);
					//current_table.DefaultView.RowFilter = filter;
					//_newTable = current_table.DefaultView.ToTable();
				}

				List<DataRow> ItemsSource = new List<DataRow>();
				int start_idx = idx_page * cnt_page;
				for(int i = start_idx; i < start_idx + cnt_page && i < Filtering_row.Length; i++)
				{
					ItemsSource.Add(Filtering_row[i]);
				}
				if(ItemsSource.Count > 0)
					dataGrid.ItemsSource = ItemsSource.CopyToDataTable().DefaultView;
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "RefreshDataView");
			}

			if(Filtering_row == null)
				return;

			label_inform_page.Content = _idx_page * _cnt_page + " ~ " + (_idx_page + 1) * _cnt_page;
			label_total_count.Content = Filtering_row.Length;
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
				prev_idx_row = Filtering_row.Length;
			else
				prev_idx_row = idx_page * cnt_page;

			cnt_page = arr_cnt_page[comboBox_change_count_page.SelectedIndex];
			idx_page = prev_idx_row / cnt_page;
			RefreshDataView(idx_page, cnt_page);
		}
		#endregion
	}
}
