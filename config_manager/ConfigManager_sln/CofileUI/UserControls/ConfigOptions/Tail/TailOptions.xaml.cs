using CofileUI.Classes;
using CofileUI.Windows;
using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CofileUI.UserControls.ConfigOptions.Tail
{
	/// <summary>
	/// TailOptions.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class TailOptions : UserControl
	{
		public TailOptions()
		{
			//try
			//{
			//	JToken token = JsonController.ParseJson(Properties.Resources.tail_config_default);
			//	DataContext = token;
			//}
			//catch(Exception e)
			//{ }
			InitializeComponent();
			ConfigOptionManager.bChanged = false;
		}
	}
	class TailOption : IOptions
	{
		string[] _options = new string[]
			{
			// comm_option
				"input_dir"
				, "input_ext"
				, "output_dir"
				, "output_ext"
				, "sid"
				, "tail_type"
				, "interval"
				, "no_inform"
				, "input_filter"
				, "shutdown_time"
				, "zero_byte_yn"
				, "no_access_sentence"
				, "file_reserver_yn"
				, "reg_yn"
				
				// enc_inform
				, "item"
				, "enc_pattern"
				, "pattern"
				, "delimiter"
				, "sub_left_len"
				, "sub_right_len"
				, "jumin_check_yn"
			};
		string[] detailOptions = new string[]
			{
			// comm_option
				"암/복호화 할 입력 로그 파일이 위치하는 경로"
				, "암/복호화 할 입력 로그파일의 확장자"
				, "암/복호화 후 출력 경로"
				, "암/복호화 후 덧붙일 확장자"
				, "DB SID 이름"
				, "cofiletail 암/복호화 방식"
				, "암호화시, 입력 폴더의 감시하는 주기"
				, "tail_type이 PATTERN일 경우 패턴을 정하는 개수"
				, "암/복호화 할 파일에 대한 패턴\n (정규표현식 지원, input_ext의 옵션보다 우선순위가 높다)"
				, "자식 데몬들을 특정 시간 후 종료할 시간"
				, "데몬 시작시 파일크기가 0인 파일에 대해서 암/복호화 유/무\n (true면 0byte파일도 감시)"
				, "no_access_sentence"
				, "file_reserver_yn"
				, "reg_yn"
				
				// enc_inform
				, "암/복호화에 사용할 ITEM 명"
				, "enc_pattern"
				, "감시하고자 하는 pattern, 정규표현식으로 작성"
				, "구분자"
				, "감시한 패턴에서 왼쪽에서 제외할 크기"
				, "감시한 패턴에서 오른쪽에서 제외할 크기"
				, "jumin_check_yn"
			};
		enum Options
		{
			// comm_option
			input_dir = 0
			, input_ext
			, output_dir
			, output_ext
			, sid
			, tail_type
			, interval
			, no_inform
			, input_filter
			, shutdown_time
			, zero_byte_yn
			, no_access_sentence
			, file_reserver_yn
			, reg_yn

			// enc_inform
			, item
			, enc_pattern
			, pattern
			, delimiter
			, sub_left_len
			, sub_right_len
			, jumin_check_yn
		}
		class OptionInfo
		{
			public string Key { get; set; }
			public string Detail { get; set; }
			//public Options Number { get; set; }
			public Options Index { get; set; }
		}
		Dictionary<string, OptionInfo> dic_options = new Dictionary<string, OptionInfo>();
		public TailOption()
		{
			InitDic();
		}
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
		public static char StartDisableProperty = '#';
		public FrameworkElement GetUIOptionValue(JProperty optionKey, JToken optionValue)
		{
			FrameworkElement ret = null;
			try
			{
				Options opt = dic_options[GetStringKey(optionKey)].Index;
				switch(opt)
				{
					case Options.tail_type:
						{
							Dictionary<string, int> dic = new Dictionary<string, int>()
							{
								{ "LINE", 0 }
								, { "PATTERN", 1 }
							};
							ComboBox cb = new ComboBox() { SelectedIndex = 0 };
							var e = dic.GetEnumerator();
							while(e.MoveNext())
								cb.Items.Add(e.Current.Key);

							cb.DataContext = optionValue.Root;
							Binding bd = new Binding("comm_option." + opt.ToString());
							bd.Mode = BindingMode.TwoWay;
							bd.Converter = new OnlyInt64Converter();
							cb.SetBinding(ComboBox.SelectedIndexProperty, bd);

							cb.SelectionChanged += delegate {
								//((JValue)optionValue).Value = dic[cb.SelectedItem.ToString()];
								ConfigOptionManager.bChanged = true;
							};
							ret = cb;
						}
						break;
					//case Options.input_ext:
					//	{
					//		Dictionary<string, int> dic = new Dictionary<string, int>()
					//		{
					//			{ "*.coenc", 0 }
					//			, { "*.txt", 1 }
					//			, { "*.jpg", 3 }
					//			, { "*.jpe", 4 }
					//			, { "*.jpeg", 5 }
					//			, { "*.jfif", 6 }
					//			, { "*.gif", 7 }
					//			, { "*.png", 8 }
					//			, { "*.tif", 9 }
					//			, { "*.tiff", 10 }
					//			, { "*.bmp", 11 }
					//			, { "*.dib", 12 }
					//			, { "Any", 13 }
					//		};

					//		ComboBox cb = new ComboBox() { SelectedIndex = 0, IsEditable = true };
					//		var e = dic.GetEnumerator();
					//		while(e.MoveNext())
					//			cb.Items.Add(e.Current.Key);

					//		cb.DataContext = optionValue.Root;
					//		Binding bd = new Binding("comm_option." + opt.ToString());
					//		bd.Mode = BindingMode.TwoWay;
					//		cb.SetBinding(ComboBox.TextProperty, bd);
					//		//cb.SelectionChanged += delegate { ((JValue)optionValue).Value = cb.Text; };
					//		ret = cb;
					//	}
					//	break;
					//case Options.output_ext:
					//	{
					//		Dictionary<string, int> dic = new Dictionary<string, int>()
					//		{
					//			{ "*.txt", 1 }
					//			, { "*.codec", 2 }
					//			, { "*.jpg", 3 }
					//			, { "*.jpe", 4 }
					//			, { "*.jpeg", 5 }
					//			, { "*.jfif", 6 }
					//			, { "*.gif", 7 }
					//			, { "*.png", 8 }
					//			, { "*.tif", 9 }
					//			, { "*.tiff", 10 }
					//			, { "*.bmp", 11 }
					//			, { "*.dib", 12 }
					//			, { "Any", 13 }
					//		};

					//		ComboBox cb = new ComboBox() { SelectedIndex = 0, IsEditable = true };
					//		var e = dic.GetEnumerator();
					//		while(e.MoveNext())
					//			cb.Items.Add(e.Current.Key);

					//		cb.DataContext = optionValue.Root;
					//		Binding bd = new Binding("comm_option." + opt.ToString());
					//		bd.Mode = BindingMode.TwoWay;
					//		cb.SetBinding(ComboBox.TextProperty, bd);
					//		//cb.SelectionChanged += delegate { ((JValue)optionValue).Value = cb.Text; };
					//		ret = cb;
					//	}
					//	break;
					//case Options.input_dir:
					//case Options.output_dir:
					//	{
					//		ComboBox cb = new ComboBox() { Text = optionValue.ToString(), IsEditable = true };

					//		cb.DataContext = optionValue.Root;
					//		Binding bd = new Binding("comm_option." + opt.ToString());
					//		bd.Mode = BindingMode.TwoWay;
					//		cb.SetBinding(ComboBox.TextProperty, bd);
					//		//cb.SelectionChanged += delegate { ((JValue)optionValue).Value = cb.Text; };
					//		ret = cb;
					//	}
					//	break;
					// comm_option
					case Options.input_dir:
					case Options.input_ext:
					case Options.output_dir:
					case Options.output_ext:
					case Options.sid:
					case Options.interval:
					case Options.no_inform:
					case Options.input_filter:
					case Options.shutdown_time:
					case Options.zero_byte_yn:
					case Options.no_access_sentence:
					case Options.file_reserver_yn:
					case Options.reg_yn:

						switch(optionValue.Type)
						{
							case JTokenType.String:
								{
									TextBox tb = new TextBox() {Text = optionValue.ToString() };
									tb.Width = JsonTreeViewItemSize.WIDTH_TEXTBOX;
									tb.HorizontalAlignment = HorizontalAlignment.Left;
									ret = tb;

									tb.DataContext = optionValue.Root;
									Binding bd = new Binding("comm_option." + opt.ToString());
									bd.Mode = BindingMode.TwoWay;
									// TextBox.Text 의 UpdateSourceTrigger 의 기본속성은 LostFocus 이다.
									bd.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
									tb.SetBinding(TextBox.TextProperty, bd);

									tb.TextChanged += delegate
									{
										//((JValue)optionValue).Value = tb.Text;
										ConfigOptionManager.bChanged = true;
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

									tb_integer.DataContext = optionValue.Root;
									Binding bd = new Binding("comm_option." + opt.ToString());
									bd.Mode = BindingMode.TwoWay;
									bd.Converter = new OnlyInt64Converter();
									tb_integer.SetBinding(NumericUpDown.ValueProperty, bd);

									tb_integer.ValueChanged += delegate
									{
										//((JValue)optionValue).Value = (System.Int64)tb_integer.Value;
										ConfigOptionManager.bChanged = true;
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


									ts.DataContext = optionValue.Root;
									Binding bd = new Binding("comm_option." + opt.ToString());
									bd.Mode = BindingMode.TwoWay;
									bd.Converter = new OnlyBooleanConverter();
									ts.SetBinding(ToggleSwitch.IsCheckedProperty, bd);
									ts.Checked += delegate
									{
										ConfigOptionManager.bChanged = true;
									};
									ts.Unchecked += delegate
									{
										ConfigOptionManager.bChanged = true;
									};
								}
								break;
							default:
								break;
						}
						break;

					// enc_inform
					case Options.item:
					case Options.enc_pattern:
					case Options.pattern:
					case Options.delimiter:
					case Options.sub_left_len:
					case Options.sub_right_len:
					case Options.jumin_check_yn:

					default:
						break;
				}
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message + " (\"" + optionKey.Name + "\" : \"" + optionValue + "\")", "UserControls.ConfigOption.Tail.TailOption.GetUIOptionValue");
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
		public FrameworkElement GetUIOptionKey(JProperty optionKey, Panel pan_value)
		{
			FrameworkElement ret = null;
			try
			{
				Options opt = dic_options[GetStringKey(optionKey)].Index;
				string detail = dic_options[GetStringKey(optionKey)].Detail;
				switch(opt)
				{
					// comm_option
					case Options.input_dir:
					case Options.input_ext:
					case Options.output_dir:
					case Options.output_ext:
					case Options.sid:
					case Options.tail_type:
					case Options.interval:
					case Options.no_inform:
					case Options.zero_byte_yn:
					case Options.file_reserver_yn:
					case Options.reg_yn:

					// enc_inform
					case Options.item:
					case Options.enc_pattern:
					case Options.pattern:
					case Options.delimiter:
					case Options.sub_left_len:
					case Options.sub_right_len:
					case Options.jumin_check_yn:
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
					case Options.shutdown_time:
					case Options.no_access_sentence:
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
								ConfigOptionManager.bChanged = true;
							};
							cb.Unchecked += delegate
							{
								JProperty newJprop = new JProperty(StartDisableProperty + jprop.Name, jprop.Value);
								jprop.Replace(newJprop);
								// delegate 에 지역변수를 사용하면 지역변수를 메모리에서 계속 잡고있는다. (전역변수 화 (어디 소속으로 전역변수 인지 모르겠다.))
								jprop = newJprop;

								pan_value.IsEnabled = cb.IsChecked.Value;
								cb.Foreground = Brushes.Gray;
								ConfigOptionManager.bChanged = true;
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
				Log.PrintError(e.Message + " (" + optionKey + ")", "UserControls.ConfigOption.Tail.TailOption.GetUIOptionKey");
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
}