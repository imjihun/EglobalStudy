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
using System.Globalization;
using MahApps.Metro.Controls;
using CofileUI.Classes;
using static CofileUI.UserControls.ConfigOptions.ConfigOptionManager;

namespace CofileUI.UserControls.ConfigOptions.File
{
	/// <summary>
	/// comm_option.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class comm_option : UserControl
	{
		//public enum Options
		//{
		//	// comm_option
		//	sid = 0
		//	, item
		//	, encode_type
		//	, log_console_yn
		//	, header_file_save_yn
		//	, file_reserver_yn
		//	, dir_monitoring_yn
		//	, dir_monitoring_term
		//	, verify_yn
		//	, schedule_time
		//	, result_log_yn
		//	, thread_count
			
		//	, Length
		//}
		//public static string[] detailOptions = new string[(int)Options.Length]
		//	{
		//		// comm_option
		//		"DB SID 이름"
		//		, "암/복호화에 사용할 Item 명"
		//		, "암호화 인코딩 타입"
		//		, "암/복호화 진행사항을 화면에 출력"
		//		, "암호화에 관련된 Header 정보를 파일로 저장"
		//		, "암호화시 원본 파일 유지"
		//		, "폴더 감시 모드 (daemon)"
		//		, "폴더 감시 모드 주기"
		//		, "verify_yn"
		//		, "schedule_time"
		//		, "result_log_yn"
		//		, "thread_count"
		//	};
		//string[] _options = new string[]
		//	{
		//		// comm_option
		//		"sid"
		//		, "item"
		//		, "encode_type"
		//		, "log_console_yn"
		//		, "header_file_save_yn"
		//		, "file_reserver_yn"
		//		, "dir_monitoring_yn"
		//		, "dir_monitoring_term"
		//		, "verify_yn"
		//		, "schedule_time"
		//		, "result_log_yn"
		//		, "thread_count"
		//	};
		//public string[] OptionKeys { get { return _options; } }
		
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
					//ConfigOptionManager.InitCommonOption(grid, DataContext as JProperty, new FileOption());
					//FileOption.InitCommonOption(grid, DataContext as JProperty);
					bInit = true;
				}
			};
		}



		public enum Options
		{
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

			, Length
		}
		// Header 에 UI 를 빼던지, groups 를 static 변수로 선언 안하든지.
		Group[] groups = new Group[]
		{
			new Group()
			{
				Header = new Label() {Content = "Basic" },
				Arr = new int[]
				{
					(int)Options.sid
					, (int)Options.item
					, (int)Options.encode_type
					, (int)Options.log_console_yn
					, (int)Options.header_file_save_yn
					, (int)Options.file_reserver_yn
					, (int)Options.verify_yn
					, (int)Options.result_log_yn
					, (int)Options.thread_count
				}
			},
			new Group()
			{
				Arr = new int[]
				{
					(int)Options.dir_monitoring_yn
					, (int)Options.dir_monitoring_term
					, (int)Options.schedule_time
				}
			}
		};
		public static string[] detailOptions = new string[(int)Options.Length]
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
			};
		static JProperty GetJProperty(Options opt, JObject root)
		{
			JProperty retval;
			if(root[(opt).ToString()] == null)
			{
				object value = "";
				switch(opt)
				{
					case Options.sid:
					case Options.item:
					case Options.encode_type:
					case Options.schedule_time:
						value = "";
						break;
					case Options.log_console_yn:
					case Options.header_file_save_yn:
					case Options.file_reserver_yn:
					case Options.dir_monitoring_yn:
					case Options.verify_yn:
					case Options.result_log_yn:
						value = false;
						break;
					case Options.dir_monitoring_term:
					case Options.thread_count:
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

						{
							TextBlock tb = new TextBlock()
							{
								Text = detail
							};

							ret = tb;
						}
						break;

					// Optional
					case Options.schedule_time:
					case Options.thread_count:
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
									if(jprop.Parent[jprop.Name.TrimStart(ConfigOptionManager.StartDisableProperty)] != null)
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
									if(jprop.Parent[ConfigOptionManager.StartDisableProperty + jprop.Name] != null)
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


							cb.DataContext = jprop.Parent;
							Binding bd = new Binding(option.ToString());
							bd.Mode = BindingMode.TwoWay;
							bd.Converter = new StringToInt64Converter();
							cb.SetBinding(ComboBox.SelectedIndexProperty, bd);

							cb.SelectedIndex = Convert.ToInt32(jprop.Value);

							cb.SelectionChanged += delegate
							{
								ConfigOptionManager.bChanged = true;
							};
							ret = cb;
						}
						break;
					case Options.sid:
					case Options.item:
					case Options.schedule_time:
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
					case Options.log_console_yn:
					case Options.header_file_save_yn:
					case Options.file_reserver_yn:
					case Options.dir_monitoring_yn:
					case Options.verify_yn:
					case Options.result_log_yn:
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
					case Options.dir_monitoring_term:
					case Options.thread_count:
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



















		void ReMakeJObjectRoot(JObject root)
		{
			for(int i = 0; i < (int)Options.Length; i++)
			{
				if(root[((Options)i).ToString()] == null)
				{
					object value = "";
					switch((Options)i)
					{
						case Options.sid:
						case Options.item:
						case Options.encode_type:
						case Options.schedule_time:
							value = "";
							break;
						case Options.log_console_yn:
						case Options.header_file_save_yn:
						case Options.file_reserver_yn:
						case Options.dir_monitoring_yn:
						case Options.verify_yn:
						case Options.result_log_yn:
							value = false;
							break;
						case Options.dir_monitoring_term:
						case Options.thread_count:
							value = (Int64)0;
							break;
					}
					if(root[ConfigOptionManager.StartDisableProperty + ((Options)i).ToString()] != null)
					{
						JProperty jprop = root[ConfigOptionManager.StartDisableProperty + ((Options)i).ToString()].Parent as JProperty;
						if(jprop != null)
						{
							jprop.Replace(new JProperty(((Options)i).ToString(), jprop.Value));
						}
						else
						{
							root.Add(new JProperty(((Options)i).ToString(), value));
						}
					}
					else
					{
						root.Add(new JProperty(((Options)i).ToString(), value));
					}
				}
				Console.WriteLine(root[((Options)i).ToString()]);
			}
		}
		void ReBinding()
		{
			for(int i = 0; i < (int)Options.Length; i++)
			{
				for(int idx = i * 2; idx < i * 2 + 2; idx++)
				{
					TextBox tb = grid.Children[idx] as TextBox;
					if(tb != null && tb.GetBindingExpression(TextBox.TextProperty) == null)
					{
						tb.DataContext = this.DataContext;
						Binding bd = new Binding("Value." + ((Options)idx).ToString());
						bd.Mode = BindingMode.TwoWay;
						// TextBox.Text 의 UpdateSourceTrigger 의 기본속성은 LostFocus 이다.
						bd.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
						tb.SetBinding(TextBox.TextProperty, bd);
					}

					NumericUpDown nud = grid.Children[idx] as NumericUpDown;
					if(nud != null && nud.GetBindingExpression(NumericUpDown.ValueProperty) == null)
					{
						nud.DataContext = this.DataContext;
						Binding bd = new Binding("Value." + ((Options)idx).ToString());
						bd.Mode = BindingMode.TwoWay;
						bd.Converter = new OnlyInt64Converter();
						nud.SetBinding(NumericUpDown.ValueProperty, bd);
					}

					ToggleSwitch ts = grid.Children[idx] as ToggleSwitch;
					if(ts != null && ts.GetBindingExpression(ToggleSwitch.IsCheckedProperty) == null)
					{
						ts.DataContext = this.DataContext;
						Binding bd = new Binding("Value." + ((Options)idx).ToString());
						bd.Mode = BindingMode.TwoWay;
						bd.Converter = new OnlyBooleanConverter();
						ts.SetBinding(ToggleSwitch.IsCheckedProperty, bd);
					}

					CheckBox cb = grid.Children[idx] as CheckBox;
					if(cb != null && cb.GetBindingExpression(CheckBox.IsCheckedProperty) == null)
					{
						ts.DataContext = this.DataContext;
						Binding bd = new Binding("Value." + ((Options)idx).ToString());
						bd.Mode = BindingMode.TwoWay;
						bd.Converter = new StringToInt64Converter();
						cb.SetBinding(CheckBox.IsCheckedProperty, bd);
					}

					ComboBox comboBox = grid.Children[idx] as ComboBox;
					if(comboBox != null && comboBox.GetBindingExpression(ComboBox.SelectedIndexProperty) == null)
					{
						ts.DataContext = this.DataContext;
						Binding bd = new Binding("Value." + ((Options)idx).ToString());
						bd.Mode = BindingMode.TwoWay;
						bd.Converter = new StringToInt64Converter();
						comboBox.SetBinding(ComboBox.SelectedIndexProperty, bd);
					}
				}
			}
		}
	}
}
