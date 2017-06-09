using Manager_proj_4.Classes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Manager_proj_4.UserControls
{
	/// <summary>
	/// ServerMenu.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ServerMenu : UserControl
	{
		public ServerMenu()
		{
			InitializeComponent();
			InitServerTab();
		}
		#region Server Menu Class
		void InitServerTab()
		{
			// serverinfo.json 파일 로드
			FileInfo fi = new FileInfo(ServerInfo.PATH);
			if(fi.Exists)
			{
				string json = FileContoller.Read(ServerInfo.PATH);
				try
				{
					ServerInfo.jobj_root = JObject.Parse(json);
					ServerPanel panel_server = ServerInfo.ConvertFromJson(ServerInfo.jobj_root);

					grid_server.Children.Add(panel_server);
				}
				catch(Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}
			else
			{
				try
				{
					ServerInfo.jobj_root = new JObject(new JProperty("Server", new JObject()));
					ServerPanel panel_server = ServerInfo.ConvertFromJson(ServerInfo.jobj_root);

					grid_server.Children.Add(panel_server);
				}
				catch(Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}

			if(ServerMenuButton.group.Count > 0)
				ServerMenuButton.group[0].IsChecked = true;
		}

		#endregion
	}
}
