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
	//interface Options
	//{
	//	FrameworkElement GetUIOptionKey(JProperty optionKey, Panel pan_value);
	//	FrameworkElement GetUIOptionValue(JProperty optionKey, JToken optionValue);
	//}
	class Options
	{
		//public delegate FrameworkElement _GetUIOptionKey(JProperty optionKey, Panel pan_value);
		//public delegate FrameworkElement _GetUIOptionValue(JProperty optionKey, JToken optionValue);
		//public static _GetUIOptionKey GetUIOptionKey = null;
		//public static _GetUIOptionValue GetUIOptionValue = null;

		protected static char StartDisableProperty = '#';

		public virtual FrameworkElement GetUIOptionKey(JProperty optionKey, Panel pan_value) { return null; }
		public virtual FrameworkElement GetUIOptionValue(JProperty optionKey, JToken optionValue) { return null; }
	}

	class SamOptions : Options
	{
		string[] _options = new string[]
			{
			// comm_option
				"sam_type"
				, "no_col"
				, "sid"
				, "delimiter"
				, "trim"
				, "skip_header"
				, "record_len"
				, "input_filter"
				, "input_dir"
				, "input_ext"
				, "output_dir"
				, "output_ext"
				, "dir_monitoring_yn"
				, "dir_monitoring_term"
				, "no_access_sentence"
				
				// col_var
				, "item"
				, "column_pos"
				, "wrap_char"
				
				// col_fix
				//, "item"
				, "start_pos"
				, "size"
				, "col_size"
			};
		string[] detailOptions = new string[]
			{
			// comm_option
				"SAM file type"
				, "암호화 대상 컬럼 수"
				, "암호화 대상 DB SID"
				, "Column 구분자"
				, "공백제거 사용 유무"
				, "암/복호화 제외 라인 수"
				, "New Line 구분이 없을 때 라인 길이"
				, "Debug 용 로그파일 경로"
				, "암/복호화 할 대상 폴더"
				, "암/복호화 할 대상 확장자명"
				, "암/복호화 후 저장할 폴더"
				, "암/복호화 후 덧붙일 확장자명"
				, "폴더 감시 모드 (daemon)"
				, "폴더 감시 모드일 때, 감시 주기"
				, "no_access_sentence"

				// col_var
				, "암/복호화에 사용할 Item명"
				, "암/복호화 대상 컬럼 위치"
				, "암/복호화시 제외할 문자(호환성 유지용도)"

				// col_fix
				//, "암/복호화에 사용할 Item명"
				, "암/복호화 대상 컬럼 시작 위치"
				, "암/복호화 대상 컬럼 크기"
				, "암/복호화 후 데이터 크기"
			};
		enum Options
		{
			sam_type = 0
			, no_col
			, sid
			, delimiter
			, trim
			, skip_header
			, record_len
			, input_filter
			, input_dir
			, input_ext
			, output_dir
			, output_ext
			, dir_monitoring_yn
			, dir_monitoring_term
			, no_access_sentence

			// col_var
			, item
			//, col_var_item
			, column_pos
			, wrap_char

			// col_fix
			//, col_fix_item
			, start_pos
			, size
			, col_size
		}
		//static class Options
		//{
		//	// comm_option
		//	static class comm_option
		//	{
		//		public const int sam_type = 0;
		//		public const int no_col = 1;
		//		public const int sid = 2;
		//		public const int delimiter = 3;
		//		public const int trim = 4;
		//		public const int skip_header = 5;
		//		public const int record_len = 6;
		//		public const int input_filter = 7;
		//		public const int input_dir = 8;
		//		public const int input_ext = 9;
		//		public const int output_dir = 10;
		//		public const int output_ext = 11;
		//		public const int dir_monitoring_yn = 12;
		//		public const int dir_monitoring_term = 13;
		//		public const int no_access_sentence = 14;
		//	}

		//	// col_var
		//	static class col_var
		//	{
		//		public const int item = 15;
		//		public const int column_pos = 16;
		//		public const int wrap_char = 17;
		//	}

		//	// col_fix
		//	static class col_fix
		//	{
		//		public const int item = 18;
		//		public const int start_pos = 19;
		//		public const int size = 20;
		//		public const int col_size = 21;
		//	}
		//}
		class OptionInfo
		{
			public string Key { get; set; }
			public string Detail { get; set; }
			//public Options Number { get; set; }
			public Options Index { get; set; }
		}
		Dictionary<string, OptionInfo> dic_options = new Dictionary<string, OptionInfo>();
		public void InitDic()
		{
			for(int i = 0; i < _options.Length; i++)
			{
				dic_options.Add(_options[i], new OptionInfo()
				{
					Key = _options[i]
						,
					Detail = detailOptions[i]
						//, Number = GetOption(_options[i])
						,
					Index = (Options)i
				}
				);
			}
		}
		public override FrameworkElement GetUIOptionValue(JProperty optionKey, JToken optionValue)
		{
			FrameworkElement ret = null;
			try
			{
				Options opt = dic_options[GetStringKey(optionKey)].Index;
				switch(opt)
				{
					case Options.sam_type:
						{
							Dictionary<string, int> dic = new Dictionary<string, int>()
							{
								{ "var", 0 }
								, { "fixed", 1 }
							};
							ComboBox cb = new ComboBox() { SelectedIndex = 0 };
							var e = dic.GetEnumerator();
							while(e.MoveNext())
								cb.Items.Add(e.Current.Key);

							ret = cb;
						}
						break;
					case Options.trim:
						{
							Dictionary<string, int> dic = new Dictionary<string, int>()
							{
								{ "None", 0 }
								, { "Right", 1 }
								, { "Left", 2 }
								, { "Both", 3 }
							};
							ComboBox cb = new ComboBox() { SelectedIndex = 0 };
							var e = dic.GetEnumerator();
							while(e.MoveNext())
								cb.Items.Add(e.Current.Key);

							ret = cb;
						}
						break;
						
					case Options.input_ext:
						{
							Dictionary<string, int> dic = new Dictionary<string, int>()
							{
								{ "*.coenc", 0 }
								, { "*.txt", 1 }
								, { "*.jpg", 3 }
								, { "*.jpe", 4 }
								, { "*.jpeg", 5 }
								, { "*.jfif", 6 }
								, { "*.gif", 7 }
								, { "*.png", 8 }
								, { "*.tif", 9 }
								, { "*.tiff", 10 }
								, { "*.bmp", 11 }
								, { "*.dib", 12 }
								, { "Any", 13 }
							};

							ComboBox cb = new ComboBox() { SelectedIndex = 0, IsEditable = true };
							var e = dic.GetEnumerator();
							while(e.MoveNext())
								cb.Items.Add(e.Current.Key);

							ret = cb;
						}
						break;
					case Options.output_ext:
						{
							Dictionary<string, int> dic = new Dictionary<string, int>()
							{
								{ "*.txt", 1 }
								, { "*.codec", 2 }
								, { "*.jpg", 3 }
								, { "*.jpe", 4 }
								, { "*.jpeg", 5 }
								, { "*.jfif", 6 }
								, { "*.gif", 7 }
								, { "*.png", 8 }
								, { "*.tif", 9 }
								, { "*.tiff", 10 }
								, { "*.bmp", 11 }
								, { "*.dib", 12 }
								, { "Any", 13 }
							};

							ComboBox cb = new ComboBox() { SelectedIndex = 0, IsEditable = true };
							var e = dic.GetEnumerator();
							while(e.MoveNext())
								cb.Items.Add(e.Current.Key);

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
								{ "*.coenc", "[.]coenc$" }
								, { "*.txt", "[.]txt$"}
							};
							ComboBox cb = new ComboBox() { SelectedIndex = 0, IsEditable = true };
							var e = dic.GetEnumerator();
							while(e.MoveNext())
								cb.Items.Add(e.Current.Key);

							ret = cb;
						}
						break;
					case Options.no_col:
					case Options.sid:
					case Options.delimiter:
					case Options.skip_header:
					case Options.record_len:
					case Options.dir_monitoring_yn:
					case Options.dir_monitoring_term:
					case Options.no_access_sentence:

					//case Options.col_var_item:
					case Options.item:
					case Options.column_pos:
					case Options.wrap_char:
						
					//case Options.col_fix_item:
					case Options.start_pos:
					case Options.size:
					case Options.col_size:
						switch(optionValue.Type)
						{
							case JTokenType.String:
								{
									TextBox tb = new TextBox() {Text = optionValue.ToString() };
									tb.Width = JsonTreeViewItemSize.WIDTH_TEXTBOX;
									tb.HorizontalAlignment = HorizontalAlignment.Left;
									ret = tb;

									tb.TextChanged += delegate
									{
										((JValue)optionValue).Value = tb.Text;
									};
								}
								break;
							case JTokenType.Integer:
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
							case JTokenType.Boolean:
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
							default:
								break;
						}
						break;

					default:
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
				ret.Margin = new Thickness(10, 3, 10, 3);
				ret.VerticalAlignment = VerticalAlignment.Center;
				ret.HorizontalAlignment = HorizontalAlignment.Left;
			}
			return ret;
		}

		string GetStringKey(JProperty key)
		{
			return key.Name.TrimStart(StartDisableProperty);
		}
		public override FrameworkElement GetUIOptionKey(JProperty optionKey, Panel pan_value)
		{
			FrameworkElement ret = null;
			try
			{
				Options opt = dic_options[GetStringKey(optionKey)].Index;
				string detail = dic_options[GetStringKey(optionKey)].Detail;
				switch(opt)
				{
					// Basical
					case Options.sam_type:
					case Options.no_col:
					case Options.sid:
					case Options.delimiter:
					case Options.trim:
					case Options.skip_header:
					case Options.record_len:
					case Options.input_dir:
					case Options.input_ext:
					case Options.output_dir:
					case Options.output_ext:
					case Options.dir_monitoring_yn:
					case Options.dir_monitoring_term:

					//case Options.col_var_item:
					case Options.column_pos:

					case Options.item:
					//case Options.col_fix_item:
					case Options.start_pos:
					case Options.size:
					case Options.col_size:
						{
							TextBlock cb = new TextBlock()
							{
								Text = detail
							};

							ret = cb;
						}
						break;

					// Optional
					case Options.input_filter:
					case Options.no_access_sentence:

					case Options.wrap_char:

						{
							CheckBox cb = new CheckBox()
							{
								Content = detail
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
						}
						break;
					default:
						break;
				}
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "UserControls.ConfigOption.GetUIOptionKey");
			}

			if(ret != null)
			{
				ret.Margin = new Thickness(10, 3, 10, 3);
				ret.VerticalAlignment = VerticalAlignment.Center;
				ret.HorizontalAlignment = HorizontalAlignment.Left;
			}
			return ret;
		}
	}
	class FileOptions : Options
	{
		//static class Options
		//{
		//	// comm_option
		//	public const int sid = 0;
		//	public const int item = 1;
		//	public const int encode_type = 2;
		//	public const int log_console_yn = 3;
		//	public const int header_file_save_yn = 4;
		//	public const int file_reserver_yn = 5;
		//	public const int dir_monitoring_yn = 6;
		//	public const int dir_monitoring_term = 7;
		//	public const int verify_yn = 8;
		//	public const int schedule_time = 9;
		//	public const int result_log_yn = 10;
		//	public const int thread_count = 11;

		//	// enc_option | dec_option
		//	public const int input_filter = 12;
		//	public const int output_suffix_head = 13;
		//	public const int output_suffix_tail = 14;
		//	public const int input_dir = 15;
		//	public const int output_dir = 16;
		//	public const int input_extension = 17;
		//	public const int output_extension = 18;
		//}
		enum Options
		{
			// comm_option
			sid = 0
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

			// enc_option | dec_option
			, input_filter
			, output_suffix_head
			, output_suffix_tail
			, input_dir
			, output_dir
			, input_extension
			, output_extension
		}
		public string[] detailOptions = new string[] 
			{
				// comm_option
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
				
				// enc_option | dec_option
				, "암/복호화 할 파일이름 규칙 정보 (정규표현식)"
				, "암/복호화 후 파일 저장시 머리말"
				, "암/복호화 후 파일 저장시 꼬릿말"
				, "암/복호화 할 원본파일 폴더 경로"
				, "암/복호화 후 저장될 폴더 경로"
				, "암/복호화 할 파일의 확장자"
				, "암/복호화 후 덧 붙일 확장자"
			};
		string[] _options = new string[]
			{
				// comm_option
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
				
				// enc_option | dec_option
				, "input_filter"
				, "output_suffix_head"
				, "output_suffix_tail"
				, "input_dir"
				, "output_dir"
				, "input_extension"
				, "output_extension"
			};
		class OptionInfo
		{
			public string Key { get; set; }
			public string Detail { get; set; }
			//public Options Number { get; set; }
			public Options Index { get; set; }
		}
		Dictionary<string, OptionInfo> dic_options = new Dictionary<string, OptionInfo>();
		public void InitDic()
		{
			for(int i = 0; i < _options.Length; i++)
			{
				dic_options.Add(_options[i], new OptionInfo()
					{
						Key = _options[i]
						, Detail = detailOptions[i]
						//, Number = GetOption(_options[i])
						, Index = (Options)i
				}
				);
			}
		}

		public override FrameworkElement GetUIOptionValue(JProperty optionKey, JToken optionValue)
		{
			FrameworkElement ret = null;
			try
			{
				Options opt = dic_options[GetStringKey(optionKey)].Index;
				switch(opt)
				{
					case Options.encode_type:
						{
							Dictionary<string, int> dic = new Dictionary<string, int>()
							{
								{ "binary", 0}
								, { "ASCII", 1 }
							};
							ComboBox cb = new ComboBox() { SelectedIndex = 0 };
							var e = dic.GetEnumerator();
							while(e.MoveNext())
								cb.Items.Add(e.Current.Key);
							
							ret = cb;
						}
						break;
					case Options.input_extension:
						{
							Dictionary<string, int> dic = new Dictionary<string, int>()
							{
								{ "*.coenc", 0 }
								, { "*.txt", 1 }
								, { "*.jpg", 3 }
								, { "*.jpe", 4 }
								, { "*.jpeg", 5 }
								, { "*.jfif", 6 }
								, { "*.gif", 7 }
								, { "*.png", 8 }
								, { "*.tif", 9 }
								, { "*.tiff", 10 }
								, { "*.bmp", 11 }
								, { "*.dib", 12 }
								, { "Any", 13 }
							};

							ComboBox cb = new ComboBox() { SelectedIndex = 0, IsEditable = true };
							var e = dic.GetEnumerator();
							while(e.MoveNext())
								cb.Items.Add(e.Current.Key);
							
							ret = cb;
						}
						break;
					case Options.output_extension:
						{
							Dictionary<string, int> dic = new Dictionary<string, int>()
							{
								{ "*.txt", 1 }
								, { "*.codec", 2 }
								, { "*.jpg", 3 }
								, { "*.jpe", 4 }
								, { "*.jpeg", 5 }
								, { "*.jfif", 6 }
								, { "*.gif", 7 }
								, { "*.png", 8 }
								, { "*.tif", 9 }
								, { "*.tiff", 10 }
								, { "*.bmp", 11 }
								, { "*.dib", 12 }
								, { "Any", 13 }
							};

							ComboBox cb = new ComboBox() { SelectedIndex = 0, IsEditable = true };
							var e = dic.GetEnumerator();
							while(e.MoveNext())
								cb.Items.Add(e.Current.Key);
							
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
								{ "*.coenc", "[.]coenc$" }
								, { "*.txt", "[.]txt$" }
							};
							ComboBox cb = new ComboBox() { SelectedIndex = 0, IsEditable = true };
							var e = dic.GetEnumerator();
							while(e.MoveNext())
								cb.Items.Add(e.Current.Key);
							
							ret = cb;
						}
						break;
					case Options.log_console_yn:
					case Options.header_file_save_yn:
					case Options.file_reserver_yn:
					case Options.dir_monitoring_yn:
					case Options.verify_yn:
					case Options.result_log_yn:

					case Options.dir_monitoring_term:
					case Options.thread_count:

					case Options.item:
					case Options.output_suffix_head:
					case Options.output_suffix_tail:
					case Options.schedule_time:
					case Options.sid:
						switch(optionValue.Type)
						{
							case JTokenType.String:
								{
									TextBox tb = new TextBox() {Text = optionValue.ToString() };
									tb.Width = JsonTreeViewItemSize.WIDTH_TEXTBOX;
									tb.HorizontalAlignment = HorizontalAlignment.Left;
									ret = tb;

									tb.TextChanged += delegate
									{
										((JValue)optionValue).Value = tb.Text;
									};
								}
								break;
							case JTokenType.Integer:
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
							case JTokenType.Boolean:
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
							default:
								break;
						}
						break;
					default:
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
				ret.Margin = new Thickness(10, 3, 10, 3);
				ret.VerticalAlignment = VerticalAlignment.Center;
				ret.HorizontalAlignment = HorizontalAlignment.Left;
			}
			return ret;
		}

		string GetStringKey(JProperty key)
		{
			return key.Name.TrimStart(StartDisableProperty);
		}
		public override FrameworkElement GetUIOptionKey(JProperty optionKey, Panel pan_value)
		{
			FrameworkElement ret = null;
			try
			{
				Options opt = dic_options[GetStringKey(optionKey)].Index;
				string detail = dic_options[GetStringKey(optionKey)].Detail;
				switch(opt)
				{
					// Basical
					case Options.sid:
					case Options.item:
					case Options.encode_type:
					case Options.log_console_yn:
					case Options.header_file_save_yn:
					case Options.file_reserver_yn:
					case Options.dir_monitoring_yn:
					case Options.dir_monitoring_term:
					case Options.verify_yn:
					case Options.result_log_yn:

					case Options.input_dir:
					case Options.output_dir:
					case Options.input_extension:
					case Options.output_extension:
						{
							TextBlock cb = new TextBlock()
							{
								Text = detail
							};

							ret = cb;
						}
						break;

					// Optional
					case Options.input_filter:
					case Options.output_suffix_head:
					case Options.output_suffix_tail:
					case Options.schedule_time:
					case Options.thread_count:
						{
							CheckBox cb = new CheckBox()
							{
								Content = detail
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
						}
						break;
					default:
						break;
				}
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message, "UserControls.ConfigOption.GetUIOptionKey");
			}

			if(ret != null)
			{
				ret.Margin = new Thickness(10, 3, 10, 3);
				ret.VerticalAlignment = VerticalAlignment.Center;
				ret.HorizontalAlignment = HorizontalAlignment.Left;
			}
			return ret;
		}


		//public static TreeView GetUIOptionMenus(JObject optionMenus)
		//{
		//	TreeView treeView = new TreeView();
		//	foreach(var v in optionMenus.Children<JProperty>())
		//	{
		//		if(v.Name == "type")
		//			continue;

		//		treeView.Items.Add(v);
		//	}

		//	return treeView;
		//}


		//static int GetIndex(string str)
		//{
		//	int index = 0;
		//	try
		//	{
		//		int i=0;
		//		foreach(var v in typeof(Options).GetFields())
		//		{
		//			if(v.Name == str.ToLower())
		//				break;
		//			i++;
		//		}
		//		index = i;
		//	}
		//	catch(Exception e)
		//	{
		//		Log.PrintError(e.Message, "UserControls.ConfigOption.GetStringOptionDetail");
		//	}
		//	return index;
		//}
		//static Options GetOption(string stringkey)
		//{
		//	Options opt = Options.sid;
		//	try
		//	{
		//		opt = (Options)Class.Parse(typeof(Options), stringkey, true);
		//	}
		//	catch(Exception e)
		//	{
		//		Log.PrintError(e.Message, "UserControls.ConfigOption.GetStringOptionDetail");
		//	}
		//	return opt;
		//}
		//static Options GetOption(JProperty optionKey)
		//{
		//	string stringkey = ((JProperty)optionKey).Name.TrimStart(StartDisableProperty);
		//	return GetOption(stringkey);
		//}
	}
}
