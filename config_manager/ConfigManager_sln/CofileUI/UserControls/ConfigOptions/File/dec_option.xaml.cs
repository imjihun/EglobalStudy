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

namespace CofileUI.UserControls.ConfigOptions.File
{
	/// <summary>
	/// dec_option.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class dec_option : UserControl
	{
		bool bInit = false;
		static JObject Root { get; set; }
		public dec_option(JObject root)
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
					bInit = true;
				}
			};
		}



		public enum Options
		{
			input_filter = 0
			, output_suffix_head
			, output_suffix_tail
			, input_dir
			, output_dir
			, input_extension
			, output_extension

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
					(int)Options.input_dir,
					(int)Options.output_dir,
					(int)Options.output_extension,
					(int)Options.output_suffix_head,
					(int)Options.output_suffix_tail
				}
			},
			new Group()
			{
				Header = new Label() {Content = "복호화 대상 규칙" },
				RadioButtonGroupName = "Input",
				Arr = new int[]
				{
					(int)Options.input_extension,
					(int)Options.input_filter
				}
			}
		};
		public static string[] detailOptions = new string[(int)Options.Length]
			{
				"복호화 대상 파일 필터(정규표현식)"
				, "복호화 후 파일 저장시 머리말"
				, "복호화 후 파일 저장시 꼬릿말"
				, "복호화 할 원본파일 폴더 경로"
				, "복호화 후 저장될 폴더 경로"
				, "복호화 할 파일의 확장자"
				, "복호화 후 덧 붙일 확장자"
			};
		static JProperty GetJProperty(Options opt, JObject root)
		{
			JProperty retval;
			if(root[(opt).ToString()] == null)
			{
				object value = "";
				switch(opt)
				{
					case Options.input_filter:
					case Options.output_suffix_head:
					case Options.output_suffix_tail:
					case Options.input_dir:
					case Options.output_dir:
					case Options.input_extension:
					case Options.output_extension:
						value = "";
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
					case Options.output_dir:
						{
							TextBlock tb = new TextBlock()
							{
								Text = detail
							};

							ret = tb;
						}
						break;
					// Optional
					case Options.output_extension:
					case Options.output_suffix_head:
					case Options.output_suffix_tail:
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
								ConfigOptionManager.CheckedKey(ref jprop);
							};
							cb.Unchecked += delegate
							{
								ConfigOptionManager.UncheckedKey(ref jprop);
							};
							ret = sp;
						}
						break;
					case Options.input_extension:
					case Options.input_filter:
						{
							StackPanel sp = new StackPanel() {Orientation = Orientation.Horizontal };

							RadioButton cb = new RadioButton();
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
								ConfigOptionManager.CheckedKey(ref jprop);
							};
							cb.Unchecked += delegate
							{
								ConfigOptionManager.UncheckedKey(ref jprop);
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
					case Options.input_filter:
					case Options.output_suffix_head:
					case Options.output_suffix_tail:
					case Options.input_dir:
					case Options.output_dir:
					case Options.input_extension:
					case Options.output_extension:
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
