using CofileUI.Classes;
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
		JToken token;
		public SamOptions()
		{
			try
			{
				token = JsonController.ParseJson(Properties.Resources.sam_config_default);
				DataContext = token;
			}
			catch(Exception e)
			{ }
			InitializeComponent();
			//grid1.Children.Add(new comm_option() { DataContext = token["comm_option"].Parent });
			//grid2.Children.Add(new col_var() { DataContext = token["col_var"].Parent });
			//grid3.Children.Add(new col_fix() { DataContext = token["#col_fix"].Parent });

			DispatcherTimer dt = new DispatcherTimer();
			dt.Interval = new TimeSpan(0, 0, 0, 1);
			dt.Tick += Dt_Tick;
			dt.Start();
		}

		private void Dt_Tick(object sender, EventArgs e)
		{
			Console.WriteLine(token["comm_option"]);
		}
	}
}
