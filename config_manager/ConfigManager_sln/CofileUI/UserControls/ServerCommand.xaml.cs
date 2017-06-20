using System;
using System.Collections.Generic;
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

namespace CofileUI.UserControls
{
	/// <summary>
	/// ServerCommand.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ServerCommand : UserControl
	{
		public static ServerCommand current;
		public ServerCommand()
		{
			current = this;
			InitializeComponent();
			InitServerCommandView();
		}

		#region Server Command View
		void InitServerCommandView()
		{
			CommandView commandView;
			commandView = new CommandView();
			//grid_second.Children.Add(commandView);
		}
		public void Refresh(ServerInfo si)
		{
			if(CommandView.current == null)
				return;

			CommandView.clear();
			CommandView.current.textBlock_server_name.Text = si.name + " / " + si.ip /*+ " / " + si.id + " / " + si.password*/;

			if(CommandView.current == null)
				return;
			CommandView.current.textBox_command.Focus();
		}
		#endregion
	}
}
