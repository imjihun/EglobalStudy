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
	/// enc_inform_line.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class enc_inform_line : UserControl
	{
		bool bInit = false;
		static JObject Root { get; set; }
		public enc_inform_line(JObject root)
		{
			Root = root;
			DataContext = root;
			InitializeComponent();

			ConfigOptionManager.MakeUI(grid, root, detailOptions, groups, GetUIOptionKey, GetUIOptionValue);
		}

		public enum Options
		{
			// comm_option
			item = 0

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
					(int)Options.item
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
				"암/복호화에 사용할 ITEM 명"
			};
		static JProperty GetJProperty(Options opt, JObject root)
		{
			JProperty retval;
			if(root[(opt).ToString()] == null)
			{
				object value = "";
				switch(opt)
				{
					case Options.item:
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
					case Options.item:
						{
							TextBlock tb = new TextBlock()
							{
								Text = detail
							};

							ret = tb;
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
					case Options.item:
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
