using CofileUI.Classes;
using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CofileUI.UserControls
{
	class ConfigOption
	{
		enum options
		{ sid = 0
			, item
			, encode_type
			, log_console_yn
			, header_file_save_yn
			, file_reserver_yn
			, dir_monitoring_yn
			, dir_monitoring_term
			, verify_yn
			, schedule_time
			, result_log_yn
			, thread_count
			, input_filter
			, output_suffix_head
			, output_suffix_tail
			, input_dir
			, output_dir
			, input_extension
			, output_extension
		}
		public static string[] detailOptions = new string[] {
												"DB SID 이름"
												, "암/복호화에 사용할 Item 명"
												, "암호화 인코딩 타입"
												, "암/복호화 진행사항을 화면에 출력"
												, "암호화에 관련된 Header 정보를 파일로 저장"
												, "암호화시 원본 파일 유지"
												, "폴더 감시 모드 (daemon)"
												, "폴더 감시 모드 주기"
												, "verify_yn"
												, "schedule_time"
												, "result_log_yn"
												, "thread_count"
												, "암/복호화 할 파일이름 규칙 정보 (정규표현식)"
												, "암/복호화 후 파일 저장시 머리말"
												, "암/복호화 후 파일 저장시 꼬릿말"
												, "암/복호화 할 원본파일 폴더 경로"
												, "암/복호화 후 저장될 폴더 경로"
												, "암/복호화 할 파일의 확장자"
												, "암/복호화 후 덧 붙일 확장자"
											};
		static string[] _options = new string[] {
												"sid"
												, "item"
												, "encode_type"
												, "log_console_yn"
												, "header_file_save_yn"
												, "file_reserver_yn"
												, "dir_monitoring_yn"
												, "dir_monitoring_term"
												, "verify_yn"
												, "schedule_time"
												, "result_log_yn"
												, "thread_count"
												, "input_filter"
												, "output_suffix_head"
												, "output_suffix_tail"
												, "input_dir"
												, "output_dir"
												, "input_extension"
												, "output_extension"
											};
		public static Grid GetUIOptionValue(string optionKey, JToken value)
		{
			Grid grid = new Grid();
			grid.Width = 150;
			grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
			try
			{
				options opt = (options)Enum.Parse(typeof(options), optionKey, true);
				switch(opt)
				{
					case options.encode_type:
						{
							Dictionary<int, string> dic = new Dictionary<int, string>()
							{
								{ 0, "binary"}
								, { 1, "ASCII" }
							};
							ComboBox cb = new ComboBox() { SelectedIndex = 0 };
							var e = dic.GetEnumerator();
							while(e.MoveNext())
								cb.Items.Add(e.Current.Value);

							grid.Children.Add(cb);
						}
						break;
					case options.input_extension:
						{
							Dictionary<int, string> dic = new Dictionary<int, string>()
							{
								{ 0, "*.coenc" }
								, { 1, "*.txt" }
								, { 3, "*.jpg" }
								, { 4, "*.jpe" }
								, { 5, "*.jpeg" }
								, { 6, "*.jfif" }
								, { 7, "*.gif" }
								, { 8, "*.png" }
								, { 9, "*.tif" }
								, { 10, "*.tiff" }
								, { 11, "*.bmp" }
								, { 12, "*.dib" }
								, { 13, "Any" }
							};

							ComboBox cb = new ComboBox() { SelectedIndex = 0, IsEditable = true };
							var e = dic.GetEnumerator();
							while(e.MoveNext())
								cb.Items.Add(e.Current.Value);

							grid.Children.Add(cb);
						}
						break;
					case options.output_extension:
						{
							Dictionary<int, string> dic = new Dictionary<int, string>()
							{
								{ 1, "*.txt" }
								, { 2, "*.codec" }
								, { 3, "*.jpg" }
								, { 4, "*.jpe" }
								, { 5, "*.jpeg" }
								, { 6, "*.jfif" }
								, { 7, "*.gif" }
								, { 8, "*.png" }
								, { 9, "*.tif" }
								, { 10, "*.tiff" }
								, { 11, "*.bmp" }
								, { 12, "*.dib" }
								, { 13, "Any" }
							};

							ComboBox cb = new ComboBox() { SelectedIndex = 0, IsEditable = true };
							var e = dic.GetEnumerator();
							while(e.MoveNext())
								cb.Items.Add(e.Current.Value);

							grid.Children.Add(cb);
						}
						break;
					case options.input_dir:
					case options.output_dir:
						{
							ComboBox cb = new ComboBox() { Text = value.ToString(), IsEditable = true };

							grid.Children.Add(cb);
						}
						break;
					case options.input_filter:
						{
							Dictionary<string, string> dic = new Dictionary<string, string>()
							{
								{ "[.]coenc$", "*.coenc" }
								, { "[.]txt$", "*.txt" }
							};
							ComboBox cb = new ComboBox() { SelectedIndex = 0, IsEditable = true };
							var e = dic.GetEnumerator();
							while(e.MoveNext())
								cb.Items.Add(e.Current.Value);

							grid.Children.Add(cb);
						}
						break;
					case options.log_console_yn:
					case options.header_file_save_yn:
					case options.file_reserver_yn:
					case options.dir_monitoring_yn:
					case options.verify_yn:
					case options.result_log_yn:
						{
							ToggleSwitch ts = new ToggleSwitch() { IsChecked = (bool)value };
							ts.Width = JsonTreeViewItemSize.WIDTH_TEXTBOX;
							ts.HorizontalAlignment = HorizontalAlignment.Left;

							ts.FontSize = 13;
							ts.OffLabel = "False";
							ts.OnLabel = "True";
							ts.Style = (Style)App.Current.Resources["MahApps.Metro.Styles.ToggleSwitch.Win10"];

							//if(panelDetailOption.RowDefinitions.Count > 0)
							//	Grid.SetRow(ts, panelDetailOption.RowDefinitions.Count - 1);
							//Grid.SetColumn(ts, 1);
							grid.Children.Add(ts);

							ts.Checked += delegate
							{
								((JValue)value).Value = ts.IsChecked;
							};
							ts.Unchecked += delegate
							{
								((JValue)value).Value = ts.IsChecked;
							};
						}
						break;
					case options.dir_monitoring_term:
					case options.thread_count:
						{
							NumericUpDown tb_integer = new NumericUpDown() {Value = (System.Int64)value };
							tb_integer.Width = JsonTreeViewItemSize.WIDTH_TEXTBOX;
							tb_integer.HorizontalAlignment = HorizontalAlignment.Left;

							//if(panelDetailOption.RowDefinitions.Count > 0)
							//	Grid.SetRow(tb_integer, panelDetailOption.RowDefinitions.Count - 1);
							//Grid.SetColumn(tb_integer, 1);
							grid.Children.Add(tb_integer);

							tb_integer.ValueChanged += delegate
							{
								((JValue)value).Value = (int)tb_integer.Value;
							};
						}
						break;
					default:
						TextBox tb = new TextBox() {Text = value.ToString() };
						tb.Width = JsonTreeViewItemSize.WIDTH_TEXTBOX;
						tb.HorizontalAlignment = HorizontalAlignment.Left;
						grid.Children.Add(tb);

						tb.TextChanged += delegate
						{
							((JValue)value).Value = tb.Text;
						};
						break;
				}
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "UserControls.ConfigOption.GetUIOptionValue");
			}
			return grid;
		}
		
		public static string GetStringOptionDetail(string optionKey)
		{
			string ret = "";
			try
			{
				options opt = (options)Enum.Parse(typeof(options), optionKey, true);
				ret = detailOptions[(int)opt];
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "UserControls.ConfigOption.GetStringOptionDetail");
			}
			return ret;
		}
		
	}
}
