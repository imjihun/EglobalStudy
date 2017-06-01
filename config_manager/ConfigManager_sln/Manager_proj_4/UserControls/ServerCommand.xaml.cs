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

namespace Manager_proj_4.UserControls
{
	/// <summary>
	/// ServerCommand.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ServerCommand : UserControl
	{
		public ServerCommand()
		{
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
		#endregion
	}
}
