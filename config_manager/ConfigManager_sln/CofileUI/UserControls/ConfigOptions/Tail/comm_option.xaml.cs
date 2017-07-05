using CofileUI.Classes;
using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
using static CofileUI.UserControls.ConfigOptions.ConfigOptionManager;

namespace CofileUI.UserControls.ConfigOptions.Tail
{
	/// <summary>
	/// comm_option.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class comm_option : UserControl
	{
		bool bInit = false;
		static JObject Root { get; set; }
		public comm_option(JObject root)
		{
			Root = root;
			DataContext = root;
			InitializeComponent();

			ConfigOptionManager.MakeUI(grid, root, detailOptions, groups, GetUIOptionKey, GetUIOptionValue);
			this.Loaded += delegate
			{
				if(!bInit)
				{
					//ConfigOptionManager.InitCommonOption(grid, DataContext as JProperty, new TailOption());
					bInit = true;
				}
			};
		}


		public enum Options
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

			, Length
		}
		// Header 에 UI 를 빼던지, groups 를 static 변수로 선언 안하든지.
		Group[] groups = new Group[]
		{
			new Group()
			{
				Arr = new int[]
				{
					(int)Options.tail_type
					,(int)Options.no_inform
					,(int)Options.reg_yn
				}
			},
			new Group()
			{
				Header = new Label() {Content = "Basic" },
				Arr = new int[]
				{
					(int)Options.input_dir
					,(int)Options.output_dir
					,(int)Options.output_ext
					,(int)Options.sid
					,(int)Options.interval
					,(int)Options.shutdown_time
					,(int)Options.zero_byte_yn
					,(int)Options.no_access_sentence
					,(int)Options.file_reserver_yn
				}
			},
			new Group()
			{
				Header = new Label() {Content = "Basic" },
				Arr = new int[]
				{
					(int)Options.input_ext
					,(int)Options.input_filter
				}
			}
			//new Group()
			//{
			//	Arr = new int[]
			//	{
			//		(int)Options.dir_monitoring_yn
			//		, (int)Options.dir_monitoring_term
			//		, (int)Options.verify_yn
			//		, (int)Options.schedule_time
			//	}
			//},
			//new Group()
			//{
			//	Header = new Label() {Content = "Etc" },
			//	Arr = new int[]
			//	{
			//		(int)Options.result_log_yn
			//		, (int)Options.thread_count
			//	}
			//}
		};
					//case Options.input_dir:
					//case Options.input_ext:
					//case Options.output_dir:
					//case Options.output_ext:
					//case Options.sid:
					//case Options.tail_type:
					//case Options.interval:
					//case Options.no_inform:
					//case Options.input_filter:
					//case Options.shutdown_time:
					//case Options.zero_byte_yn:
					//case Options.no_access_sentence:
					//case Options.file_reserver_yn:
					//case Options.reg_yn:
		public static string[] detailOptions = new string[(int)Options.Length]
			{
			// comm_option
				"암/복호화 할 입력 로그 파일이 위치하는 경로"
				, "암/복호화 할 입력 로그파일의 확장자"
				, "암/복호화 후 출력 경로"
				, "암/복호화 후 덧붙일 확장자"
				, "DB SID 이름"
				, "cofile tail 암/복호화 방식"
				, "암호화시, 입력 폴더의 감시하는 주기"
				, "tail type이 PATTERN일 경우 패턴을 정하는 개수"
				, "암/복호화 할 파일에 대한 패턴\n (정규표현식 지원, input ext의 옵션보다 우선순위가 높다)"
				, "자식 데몬들을 특정 시간 후 종료할 시간"
				, "데몬 시작시 파일크기가 0인 파일에 대해서 암/복호화 유/무\n (true면 0byte파일도 감시)"
				, "no_access_sentence"
				, "원본 파일 유지 여부"
				, "정규표현식 사용 여부"
			};
		static JProperty GetJProperty(Options opt, JObject root)
		{
			JProperty retval;
			if(root[(opt).ToString()] == null)
			{
				object value = "";
				switch(opt)
				{
					case Options.input_dir:
					case Options.input_ext:
					case Options.output_dir:
					case Options.output_ext:
					case Options.sid:
					case Options.input_filter:
					case Options.no_access_sentence:
						value = "";
						break;
					case Options.zero_byte_yn:
					case Options.file_reserver_yn:
					case Options.reg_yn:
						value = false;
						break;
					case Options.tail_type:
					case Options.interval:
					case Options.no_inform:
					case Options.shutdown_time:
						value = (Int64)0;
						break;
				}
				if(root[ConfigOptionManager.StartDisableProperty + (opt).ToString()] != null)
				{
					JProperty jprop = root[ConfigOptionManager.StartDisableProperty + (opt).ToString()].Parent as JProperty;
					if(jprop != null)
					{
						retval = jprop;
						//jprop.Replace(new JProperty((opt).ToString(), jprop.Value));
					}
					else
					{
						retval = new JProperty((opt).ToString(), value);
						root.Add(retval);
					}
				}
				else
				{
					retval = new JProperty((opt).ToString(), value);
					root.Add(retval);
				}
			}
			else
				retval = root[(opt).ToString()].Parent as JProperty;

			return retval;
		}
		static FrameworkElement GetUIOptionKey(int opt, JObject root)
		{
			Options option = (Options)opt;
			FrameworkElement ret = null;
			try
			{
				string detail = detailOptions[(int)opt];
				switch(option)
				{
					// Basical
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
						{
							TextBlock tb = new TextBlock()
							{
								Text = detail
							};

							ret = tb;
						}
						break;

					// Optional
					case Options.input_filter:
					case Options.shutdown_time:
					case Options.no_access_sentence:
						{
							StackPanel sp = new StackPanel() {Orientation = Orientation.Horizontal };

							CheckBox cb = new CheckBox();
							JProperty jprop = GetJProperty(option, root);
							cb.IsChecked = !(jprop.Name[0] == ConfigOptionManager.StartDisableProperty);
							sp.Children.Add(cb);
							TextBlock tb = new TextBlock()
							{
								Text = detail
							};
							sp.Children.Add(tb);

							// delegate 에 지역변수를 사용하면 지역변수를 메모리에서 계속 잡고있는다. (전역변수 화 (어디 소속으로 전역변수 인지 모르겠다.))
							cb.Checked += delegate
							{
								try
								{
									if(jprop.Parent != null 
										&& jprop.Parent[jprop.Name.TrimStart(ConfigOptionManager.StartDisableProperty)] != null)
									{
										jprop.Parent[jprop.Name.TrimStart(ConfigOptionManager.StartDisableProperty)].Parent.Remove();
									}
									JProperty newJprop = new JProperty(jprop.Name.TrimStart(ConfigOptionManager.StartDisableProperty), jprop.Value);
									jprop.Replace(newJprop);
									Log.PrintConsole(jprop + " -> " + newJprop, "Debug");
									// delegate 에 지역변수를 사용하면 지역변수를 메모리에서 계속 잡고있는다. (전역변수 화 (어디 소속으로 전역변수 인지 모르겠다.))
									jprop = newJprop;

								}
								catch(Exception ex)
								{
									Log.PrintError(ex.Message, "UserControls.ConfigOptions.File.comm_option.GetUIOptionKey");
								}
								ConfigOptionManager.bChanged = true;
							};
							cb.Unchecked += delegate
							{
								try
								{
									if(jprop.Parent != null
										&& jprop.Parent[ConfigOptionManager.StartDisableProperty + jprop.Name] != null)
									{
										jprop.Parent[ConfigOptionManager.StartDisableProperty + jprop.Name].Parent.Remove();
									}
									JProperty newJprop = new JProperty(ConfigOptionManager.StartDisableProperty + jprop.Name, jprop.Value);
									jprop.Replace(newJprop);
									Log.PrintConsole(jprop + " -> " + newJprop, "Debug");
									// delegate 에 지역변수를 사용하면 지역변수를 메모리에서 계속 잡고있는다. (전역변수 화 (어디 소속으로 전역변수 인지 모르겠다.))
									jprop = newJprop;
								}
								catch(Exception ex)
								{
									Log.PrintError(ex.Message, "UserControls.ConfigOptions.File.comm_option.GetUIOptionKey");
								}
								ConfigOptionManager.bChanged = true;
							};
							ret = sp;
						}
						break;
					default:
						break;
				}
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message + " (" + option.ToString() + ")", "UserControls.ConfigOption.File.FileOption.GetUIOptionKey");
			}

			if(ret != null)
			{
				ret.Margin = new Thickness(10, 3, 10, 3);
				ret.VerticalAlignment = VerticalAlignment.Center;
				ret.HorizontalAlignment = HorizontalAlignment.Left;
			}
			return ret;
		}
		static FrameworkElement GetUIOptionValue(int opt, JObject root)
		{
			Options option = (Options)opt;

			JProperty jprop = GetJProperty(option, root);
			FrameworkElement ret = null;
			try
			{
				switch(option)
				{
					case Options.tail_type:
						{
							Dictionary<string, int> dic = new Dictionary<string, int>()
							{
								{ "LINE", 1 }
								, { "PATTERN", 2 }
							};
							ComboBox cb = new ComboBox() { SelectedIndex = 0 };
							var e = dic.GetEnumerator();
							while(e.MoveNext())
								cb.Items.Add(e.Current.Key);

							cb.DataContext = jprop.Parent;
							Binding bd = new Binding(option.ToString());
							bd.Mode = BindingMode.TwoWay;
							bd.Converter = new OnlyInt64TailTypeConverter();
							cb.SetBinding(ComboBox.SelectedIndexProperty, bd);

							cb.SelectedIndex = Convert.ToInt32(jprop.Value) - 1;

							cb.SelectionChanged += delegate {
								//((JValue)optionValue).Value = dic[cb.SelectedItem.ToString()];
								ConfigOptionManager.bChanged = true;
								TailOptions.current.ChangeSecondGrid();
							};
							ret = cb;
						}
						break;
					case Options.input_dir:
					case Options.input_ext:
					case Options.output_dir:
					case Options.output_ext:
					case Options.sid:
					case Options.input_filter:
					case Options.no_access_sentence:
						{
							TextBox tb = new TextBox() {/*Text = optionValue.ToString()*/ };
							tb.Width = JsonTreeViewItemSize.WIDTH_TEXTBOX;
							tb.HorizontalAlignment = HorizontalAlignment.Left;
							ret = tb;

							tb.DataContext = jprop.Parent;
							Binding bd = new Binding(option.ToString());
							bd.Mode = BindingMode.TwoWay;
							// TextBox.Text 의 UpdateSourceTrigger 의 기본속성은 LostFocus 이다.
							bd.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
							tb.SetBinding(TextBox.TextProperty, bd);

							tb.Text = Convert.ToString(jprop.Value);

							tb.TextChanged += delegate
							{
								//((JValue)optionValue).Value = tb.Text;
								ConfigOptionManager.bChanged = true;
							};
						}
						break;
					case Options.zero_byte_yn:
					case Options.file_reserver_yn:
					case Options.reg_yn:
						{
							ToggleSwitch ts = new ToggleSwitch() { /*IsChecked = (bool)optionValue*/ };
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

							ts.DataContext = jprop.Parent;
							Binding bd = new Binding(option.ToString());
							bd.Mode = BindingMode.TwoWay;
							bd.Converter = new OnlyBooleanConverter();
							ts.SetBinding(ToggleSwitch.IsCheckedProperty, bd);

							ts.IsChecked = Convert.ToBoolean(jprop.Value);

							ts.Checked += delegate
							{
								//((JValue)optionValue).Value = ts.IsChecked;
								ConfigOptionManager.bChanged = true;
							};
							ts.Unchecked += delegate
							{
								//((JValue)optionValue).Value = ts.IsChecked;
								ConfigOptionManager.bChanged = true;
							};
						}
						break;
					case Options.interval:
					case Options.no_inform:
					case Options.shutdown_time:
						{
							NumericUpDown tb_integer = new NumericUpDown() {/*Value = (System.Int64)optionValue*/ };
							tb_integer.Width = JsonTreeViewItemSize.WIDTH_TEXTBOX;
							tb_integer.HorizontalAlignment = HorizontalAlignment.Left;

							//if(panelDetailOption.RowDefinitions.Count > 0)
							//	Grid.SetRow(tb_integer, panelDetailOption.RowDefinitions.Count - 1);
							//Grid.SetColumn(tb_integer, 1);
							ret = tb_integer;

							tb_integer.DataContext = jprop.Parent;
							Binding bd = new Binding(option.ToString());
							bd.Mode = BindingMode.TwoWay;
							bd.Converter = new OnlyInt64Converter();
							tb_integer.SetBinding(NumericUpDown.ValueProperty, bd);

							tb_integer.Value = Convert.ToInt64(jprop.Value);

							tb_integer.ValueChanged += delegate
							{
								//((JValue)optionValue).Value = (System.Int64)tb_integer.Value;
								ConfigOptionManager.bChanged = true;
							};

						}
						break;
					default:
						break;
				}
				if(jprop != null && jprop.Name[0] == ConfigOptionManager.StartDisableProperty
					&& Root[jprop.Name.TrimStart(ConfigOptionManager.StartDisableProperty)] != null
					&& Root[jprop.Name.TrimStart(ConfigOptionManager.StartDisableProperty)].Parent != null)
				{
					Root[jprop.Name.TrimStart(ConfigOptionManager.StartDisableProperty)].Parent.Remove();
				}
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message + " (\"" + option.ToString() + "\" : \"" + jprop + "\")", "UserControls.ConfigOption.File.FileOption.GetUIOptionValue");
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

	}
}
