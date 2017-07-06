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
using System.Windows.Threading;

namespace CofileUI.UserControls.ConfigOptions.Sam
{
	/// <summary>
	/// SamOptions.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class SamOptions : UserControl
	{
		bool bInit = false;
		public static SamOptions current;
		JObject Root { get; set; }
		public SamOptions()
		{
			current = this;
			InitializeComponent();
			ConfigOptionManager.bChanged = false;
			this.Loaded += delegate
			{
				if(!bInit)
				{
					Root = DataContext as JObject;
					if(Root == null)
						return;

					grid1.Children.Add(new comm_option(Root["comm_option"] as JObject));
					ChangeSecondGrid();
					bInit = true;
				}
			};
		}
		public void ChangeSecondGrid()
		{
			JObject root = DataContext as JObject;
			if(root == null)
				return;
			JObject jobj = root["comm_option"] as JObject;
			if(jobj == null)
				return;
			JValue jval = jobj["sam_type"] as JValue;
			if(jval == null)
				return;

			grid2.Children.Clear();

			if(Convert.ToInt64(jval.Value) == 0)
			{
				ChangeBySamType(root, "col_var", "col_fix");
				if(root["col_var"] == null)
					return;
				grid2.Children.Add(new col_var() { DataContext = root["col_var"].Parent });
			}
			else if(Convert.ToInt64(jval.Value) == 1)
			{
				ChangeBySamType(root, "col_fix", "col_var");
				if(root["col_fix"] == null)
					return;
				grid2.Children.Add(new col_fix() { DataContext = root["col_fix"].Parent });
			}
		}
		static void ChangeBySamType(JObject root, string enableKey, string disableKey)
		{
			if(root == null || enableKey == null || disableKey == null)
				return;

			if(root[disableKey] != null)
			{
				root[disableKey].Parent.Replace(new JProperty(ConfigOptionManager.StartDisableProperty + disableKey, root[disableKey]));
				ConfigOptionManager.bChanged = true;
			}
			if(root[enableKey] == null)
			{
				if(root[ConfigOptionManager.StartDisableProperty + enableKey] != null)
					root[ConfigOptionManager.StartDisableProperty + enableKey].Parent.Replace(new JProperty(enableKey, root[ConfigOptionManager.StartDisableProperty + enableKey]));
				else
					root.Add(new JProperty(enableKey, new JArray()));
				ConfigOptionManager.bChanged = true;
			}
		}
	}

	class SamOption : IOptions
	{
		//string[] _options = new string[]
		//	{
		//	// comm_option
		//		"sam_type"
		//		, "no_col"
		//		, "sid"
		//		, "delimiter"
		//		, "trim"
		//		, "skip_header"
		//		, "record_len"
		//		, "input_filter"
		//		, "input_dir"
		//		, "input_ext"
		//		, "output_dir"
		//		, "output_ext"
		//		, "dir_monitoring_yn"
		//		, "dir_monitoring_term"
		//		, "no_access_sentence"
				
		//		// col_var
		//		, "item"
		//		, "column_pos"
		//		, "wrap_char"
				
		//		// col_fix
		//		//, "item"
		//		, "start_pos"
		//		, "size"
		//		, "col_size"
		//	};
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
			, file_reserver_yn
			, no_access_sentence
			, log_file

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

			, Length
		}
		class OptionInfo
		{
			public string Key { get; set; }
			public string Detail { get; set; }
			//public Options Number { get; set; }
			public Options Index { get; set; }
		}
		Dictionary<string, OptionInfo> dic_options = new Dictionary<string, OptionInfo>();
		public SamOption()
		{
			InitDic();
		}
		public void InitDic()
		{
			for(int i = 0; i < (int)Options.Length; i++)
			{
				dic_options.Add(((Options)i).ToString(), new OptionInfo()
				{
					Key = ((Options)i).ToString()
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

							cb.DataContext = optionValue.Root;
							Binding bd = new Binding("comm_option." + opt.ToString());
							bd.Mode = BindingMode.TwoWay;
							bd.Converter = new OnlyInt64Converter();
							cb.SetBinding(ComboBox.SelectedIndexProperty, bd);

							JObject root = optionValue.Root as JObject;
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
					//		//cb.SelectionChanged += delegate { ((JValue)optionValue).Value = cb.SelectedItem.ToString(); };
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
					//		//cb.SelectionChanged += delegate { ((JValue)optionValue).Value = cb.SelectedItem.ToString(); };
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
					//		//cb.SelectionChanged += delegate { ((JValue)optionValue).Value = cb.SelectedItem.ToString(); };
					//		ret = cb;
					//	}
					//	break;
					//case Options.input_filter:
					//	{
					//		Dictionary<string, string> dic = new Dictionary<string, string>()
					//		{
					//			{ "*[.]sam$", "[.]sam$" }
					//			, { "[.]txt$", "[.]txt$"}
					//		};
					//		ComboBox cb = new ComboBox() { SelectedIndex = 0, IsEditable = true };
					//		var e = dic.GetEnumerator();
					//		while(e.MoveNext())
					//			cb.Items.Add(e.Current.Key);

					//		cb.DataContext = optionValue.Root;
					//		Binding bd = new Binding("comm_option." + opt.ToString());
					//		bd.Mode = BindingMode.TwoWay;
					//		cb.SetBinding(ComboBox.TextProperty, bd);
					//		ret = cb;
					//	}
					//	break;

					case Options.no_col:
					case Options.sid:
					case Options.delimiter:
					case Options.skip_header:
					case Options.record_len:
					case Options.input_filter:
					case Options.input_dir:
					case Options.input_ext:
					case Options.output_dir:
					case Options.output_ext:
					case Options.dir_monitoring_yn:
					case Options.dir_monitoring_term:
					case Options.file_reserver_yn:
					case Options.no_access_sentence:
					case Options.log_file:
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
							default:
								break;
						}
						break;

					//case Options.col_var_item:
					case Options.item:
					case Options.column_pos:
					case Options.wrap_char:

					//case Options.col_fix_item:
					case Options.start_pos:
					case Options.size:
					case Options.col_size:
					default:
						break;
				}
			}
			catch(Exception e)
			{
				Log.PrintError(e.Message + " (\"" + optionKey.Name + "\" : \"" + optionValue + "\")", "UserControls.ConfigOption.Sam.SamOption.GetUIOptionValue");
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

					//case Options.col_var_item:
					case Options.column_pos:

					case Options.item:
					//case Options.col_fix_item:
					case Options.start_pos:
					case Options.size:
					case Options.col_size:
						{
							if(optionKey.Name[0] == '#')
								break;

							TextBlock cb = new TextBlock()
							{
								Text = detail
								//Text = _options[(int)opt]
							};

							ret = cb;
						}
						break;

					// Optional
					case Options.input_filter:
					case Options.no_access_sentence:
					case Options.log_file:

					case Options.wrap_char:

						{
							//if(optionKey.Name[0] != '#')
							//	break;

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
								Console.WriteLine("jprop = " + jprop.Value);
								JProperty newJprop = new JProperty(jprop.Name.TrimStart(StartDisableProperty), jprop.Value);

								jprop.Replace(newJprop);
								// delegate 에 지역변수를 사용하면 지역변수를 메모리에서 계속 잡고있는다. (전역변수 화 (어디 소속으로 전역변수 인지 모르겠다.))
								jprop = newJprop;

								pan_value.IsEnabled = cb.IsChecked.Value;
								cb.Foreground = Brushes.Black;

								Console.WriteLine("jprop = " + jprop);

								ConfigOptionManager.bChanged = true;
							};
							cb.Unchecked += delegate
							{
								Console.WriteLine("jprop = " + jprop.Value);
								JProperty newJprop = new JProperty(StartDisableProperty + jprop.Name, jprop.Value);
								jprop.Replace(newJprop);
								// delegate 에 지역변수를 사용하면 지역변수를 메모리에서 계속 잡고있는다. (전역변수 화 (어디 소속으로 전역변수 인지 모르겠다.))
								jprop = newJprop;

								pan_value.IsEnabled = cb.IsChecked.Value;
								cb.Foreground = Brushes.Gray;
								Console.WriteLine("jprop = " + jprop);

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
				Log.PrintError(e.Message + " (" + optionKey + ")", "UserControls.ConfigOption.Sam.SamOption.GetUIOptionKey");
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
