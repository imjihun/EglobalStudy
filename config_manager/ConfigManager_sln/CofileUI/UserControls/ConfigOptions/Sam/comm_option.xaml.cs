﻿using CofileUI.Classes;
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
using System.Globalization;
using static CofileUI.UserControls.ConfigOptions.ConfigOptionManager;

namespace CofileUI.UserControls.ConfigOptions.Sam
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
					//ConfigOptionManager.InitCommonOption(grid, DataContext as JProperty, new SamOption());

					bInit = true;
				}
			};
		}



		public enum Options
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
			, file_reserver_yn
			, no_access_sentence
			, log_file

			, Length
		}
		// Header 에 UI 를 빼던지, groups 를 static 변수로 선언 안하든지.
		Group[] groups = new Group[]
		{
			new Group()
			{
				Arr = new int[]
				{
					(int)Options.sam_type,
					(int)Options.delimiter
				}
			},new Group()
			{
				Header = new Label() {Content = "Basic" },
				Arr = new int[]
				{
					(int)Options.no_col
					, (int)Options.sid
					, (int)Options.trim
					, (int)Options.skip_header
					, (int)Options.record_len
					, (int)Options.input_dir
					, (int)Options.output_dir
					, (int)Options.output_ext
					, (int)Options.file_reserver_yn
					, (int)Options.no_access_sentence
					, (int)Options.log_file
				}
			},new Group()
			{
				Arr = new int[]
				{
					(int)Options.dir_monitoring_yn,
					(int)Options.dir_monitoring_term
				}
			},new Group()
			{
				Header = new Label() {Content = "Basic" },
				Arr = new int[]
				{
					(int)Options.input_filter,
					(int)Options.input_ext
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
					//case Options.sam_type:
					//case Options.no_col:
					//case Options.sid:
					//case Options.delimiter:
					//case Options.trim:
					//case Options.skip_header:
					//case Options.record_len:
					//case Options.input_filter:
					//case Options.input_dir:
					//case Options.input_ext:
					//case Options.output_dir:
					//case Options.output_ext:
					//case Options.dir_monitoring_yn:
					//case Options.dir_monitoring_term:
					//case Options.file_reserver_yn:
					//case Options.no_access_sentence:
					//case Options.log_file:
		public static string[] detailOptions = new string[(int)Options.Length]
			{
			// comm_option
				"SAM file type"
				, "암호화 대상 컬럼 수"
				, "암호화 대상 DB SID"
				, "Column 구분자"
				, "공백제거 사용 유무"
				, "암/복호화 제외 라인 수"
				, "New Line 구분이 없을 때 라인 길이"
				, "암/복호화 할 파일에 대한 패턴\n (정규표현식 지원, '암/복호화 할 대상 확장자명' 옵션보다 우선순위가 높다)"
				, "암/복호화 할 대상 폴더"
				, "암/복호화 할 대상 확장자명"
				, "암/복호화 후 저장할 폴더"
				, "암/복호화 후 덧붙일 확장자명"
				, "폴더 감시 모드 (daemon)"
				, "폴더 감시 모드일 때, 감시 주기"
				, "원본 파일 유지 여부"
				, "no_access_sentence"
				, "log_file"
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
					case Options.delimiter:
					case Options.input_filter:
					case Options.input_dir:
					case Options.input_ext:
					case Options.output_dir:
					case Options.output_ext:
					case Options.no_access_sentence:
					case Options.log_file:
						value = "";
						break;
					case Options.dir_monitoring_yn:
					case Options.file_reserver_yn:
						value = false;
						break;
					case Options.sam_type:
					case Options.no_col:
					case Options.trim:
					case Options.skip_header:
					case Options.record_len:
					case Options.dir_monitoring_term:
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
					case Options.file_reserver_yn:
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
					case Options.no_access_sentence:
					case Options.log_file:
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
					case Options.sam_type:
						{

							Dictionary<string, int> dic = new Dictionary<string, int>()
							{
								{ "var", 0 }
								, { "fixed", 1 }
							};
							ComboBox cb = new ComboBox();
							var e = dic.GetEnumerator();
							while(e.MoveNext())
								cb.Items.Add(e.Current.Key);
							
							cb.DataContext = jprop.Parent;
							Binding bd = new Binding(option.ToString());
							bd.Mode = BindingMode.TwoWay;
							bd.Converter = new OnlyInt64Converter();
							cb.SetBinding(ComboBox.SelectedIndexProperty, bd);

							cb.SelectedIndex = Convert.ToInt32(jprop.Value);

							cb.SelectionChanged += delegate {
								//((JValue)optionValue).Value = dic[cb.SelectedItem.ToString()];
								ConfigOptionManager.bChanged = true;
								SamOptions.current.ChangeSecondGrid();
							};
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
							ComboBox cb = new ComboBox();
							var e = dic.GetEnumerator();
							while(e.MoveNext())
								cb.Items.Add(e.Current.Key);

							cb.DataContext = jprop.Parent;
							Binding bd = new Binding(option.ToString());
							bd.Mode = BindingMode.TwoWay;
							bd.Converter = new OnlyInt64Converter();
							cb.SetBinding(ComboBox.SelectedIndexProperty, bd);

							cb.SelectedIndex = Convert.ToInt32(jprop.Value);

							cb.SelectionChanged += delegate {
								//((JValue)optionValue).Value = dic[cb.SelectedItem.ToString()];
								ConfigOptionManager.bChanged = true;
							};
							ret = cb;
						}
						break;
					case Options.sid:
					case Options.delimiter:
					case Options.input_filter:
					case Options.input_dir:
					case Options.input_ext:
					case Options.output_dir:
					case Options.output_ext:
					case Options.no_access_sentence:
					case Options.log_file:
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
					case Options.dir_monitoring_yn:
					case Options.file_reserver_yn:
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
					case Options.no_col:
					case Options.skip_header:
					case Options.record_len:
					case Options.dir_monitoring_term:
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
