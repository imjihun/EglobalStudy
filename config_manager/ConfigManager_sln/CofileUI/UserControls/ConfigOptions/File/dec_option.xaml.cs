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

namespace CofileUI.UserControls.ConfigOptions.File
{
	/// <summary>
	/// dec_option.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class dec_option : UserControl
	{
		bool bInit = false;
		public dec_option()
		{
			InitializeComponent();
			this.Loaded += delegate
			{
				if(!bInit)
				{
					Common.InitCommonOption(grid, DataContext as JProperty, new FileOption());
					bInit = true;
				}
			};
		}
	}
}
