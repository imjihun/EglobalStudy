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
		enum Options
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

			, undefined
		}
		class OptionInfo
		{
			string Key { get; set; }
			string Detail { get; set; }
		}
		//public static Dictionary<Options, OptionInfo> dic_options
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
												, ""
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
		public static FrameworkElement GetUIOptionValue(JProperty optionKey, JToken optionValue)
		{
			FrameworkElement ret = null;
			try
			{
				Options opt = GetOption(optionKey);
				switch(opt)
				{
					case Options.encode_type:
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
							
							ret = cb;
						}
						break;
					case Options.input_extension:
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
							
							ret = cb;
						}
						break;
					case Options.output_extension:
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
							
							ret = cb;
						}
						break;
					case Options.input_dir:
					case Options.output_dir:
						{
							ComboBox cb = new ComboBox() { Text = optionValue.ToString(), IsEditable = true };
							
							ret = cb;
						}
						break;
					case Options.input_filter:
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
							
							ret = cb;
						}
						break;
					case Options.log_console_yn:
					case Options.header_file_save_yn:
					case Options.file_reserver_yn:
					case Options.dir_monitoring_yn:
					case Options.verify_yn:
					case Options.result_log_yn:
						{
							ToggleSwitch ts = new ToggleSwitch() { IsChecked = (bool)optionValue };
							ts.Width = JsonTreeViewItemSize.WIDTH_TEXTBOX;
							ts.HorizontalAlignment = HorizontalAlignment.Left;

							ts.FontSize = 13;
							ts.OffLabel = "False";
							ts.OnLabel = "True";
							ts.Style = (Style)App.Current.Resources["MahApps.Metro.Styles.ToggleSwitch.Win10"];

							//if(panelDetailOption.RowDefinitions.Count > 0)
							//	Grid.SetRow(ts, panelDetailOption.RowDefinitions.Count - 1);
							//Grid.SetColumn(ts, 1);
							ret = ts;

							ts.Checked += delegate
							{
								((JValue)optionValue).Value = ts.IsChecked;
							};
							ts.Unchecked += delegate
							{
								((JValue)optionValue).Value = ts.IsChecked;
							};
						}
						break;
					case Options.dir_monitoring_term:
					case Options.thread_count:
						{
							NumericUpDown tb_integer = new NumericUpDown() {Value = (System.Int64)optionValue };
							tb_integer.Width = JsonTreeViewItemSize.WIDTH_TEXTBOX;
							tb_integer.HorizontalAlignment = HorizontalAlignment.Left;

							//if(panelDetailOption.RowDefinitions.Count > 0)
							//	Grid.SetRow(tb_integer, panelDetailOption.RowDefinitions.Count - 1);
							//Grid.SetColumn(tb_integer, 1);
							ret = tb_integer;

							tb_integer.ValueChanged += delegate
							{
								((JValue)optionValue).Value = (int)tb_integer.Value;
							};
						}
						break;
					default:
						TextBox tb = new TextBox() {Text = optionValue.ToString() };
						tb.Width = JsonTreeViewItemSize.WIDTH_TEXTBOX;
						tb.HorizontalAlignment = HorizontalAlignment.Left;
						ret = tb;

						tb.TextChanged += delegate
						{
							((JValue)optionValue).Value = tb.Text;
						};
						break;
				}
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "UserControls.ConfigOption.GetUIOptionValue");
			}
			if(ret != null)
			{
				ret.Width = 150;
				ret.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
			}
			return ret;
		}

		static Options GetOption(string stringkey)
		{
			Options opt = Options.undefined;
			try
			{
				opt = (Options)Enum.Parse(typeof(Options), stringkey, true);
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "UserControls.ConfigOption.GetStringOptionDetail");
			}
			return opt;
		}
		static Options GetOption(JProperty optionKey)
		{
			string stringkey = ((JProperty)optionKey).Name.TrimStart(StartDisableProperty);
			return GetOption(stringkey);
		}
		static char StartDisableProperty = '#';
		public static FrameworkElement GetUIOptionKey(JProperty optionKey, Panel pan_value)
		{
			FrameworkElement ret;
			Options opt = GetOption(optionKey);
			CheckBox cb = new CheckBox()
			{
				Content = detailOptions[(int)opt]
				, Margin = new Thickness(10,3,10,3)
			};
			if(((JProperty)optionKey).Name.Length > 0 && ((JProperty)optionKey).Name[0] != StartDisableProperty)
			{
				cb.IsChecked = true;
				cb.Foreground = Brushes.Black;
			}
			else
			{
				cb.IsChecked = false;
				cb.Foreground = Brushes.Gray;
			}

			pan_value.IsEnabled = cb.IsChecked.Value;
			// delegate 에 지역변수를 사용하면 지역변수를 메모리에서 계속 잡고있는다. (전역변수 화 (어디 소속으로 전역변수 인지 모르겠다.))
			JProperty jprop = optionKey;
			cb.Checked += delegate
			{
				JProperty newJprop = new JProperty(jprop.Name.TrimStart(StartDisableProperty), jprop.Value);
				jprop.Replace(newJprop);
				// delegate 에 지역변수를 사용하면 지역변수를 메모리에서 계속 잡고있는다. (전역변수 화 (어디 소속으로 전역변수 인지 모르겠다.))
				jprop = newJprop;

				pan_value.IsEnabled = cb.IsChecked.Value;
				cb.Foreground = Brushes.Black;
			};
			cb.Unchecked += delegate
			{
				JProperty newJprop = new JProperty(StartDisableProperty + jprop.Name, jprop.Value);
				jprop.Replace(newJprop);
				// delegate 에 지역변수를 사용하면 지역변수를 메모리에서 계속 잡고있는다. (전역변수 화 (어디 소속으로 전역변수 인지 모르겠다.))
				jprop = newJprop;

				pan_value.IsEnabled = cb.IsChecked.Value;
				cb.Foreground = Brushes.Gray;
			};
			ret = cb;

			return ret;
		}
	}
}
