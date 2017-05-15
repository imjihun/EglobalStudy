using System;
using System.Collections.Generic;
using System.Data;
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

namespace TreeViewTest
{
	/// <summary>
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			this.MakeTreeView();
		}
		private void MakeTreeView()
		{
			this.AddRoot(this.trvTest, "UPIS");

			DataTable dtTree = this.GetMenuListAll();
			DataRow[] drSelected = dtTree.Select(" PMENU_ID = '00000' ");
			foreach(DataRow dr in drSelected)
			{
				this.AddSubNodeItem((TreeViewItem)this.trvTest.Items[0], dr["MENU_NAME"].ToString(), dr["MENU_ID"].ToString(), dtTree);
			}
		}
		private void AddRoot(TreeView trv, string header)
		{
			TreeViewItem trvItem = new TreeViewItem();
			trvItem.Header = header;
			trvItem.Tag = "0000";
			trv.Items.Add(trvItem);
		}

		private void AddSubNodeItem(TreeViewItem root, string header, string id, DataTable dtInfo)
		{
			DataRow[] drSelected = dtInfo.Select(" PMENU_ID = '" + id + "'");
			foreach(DataRow dr in drSelected)
			{
				TreeViewItem trvItem = new TreeViewItem();
				trvItem.Header = dr["MENU_NAME"].ToString();
				trvItem.Tag = dr["MENU_ID"].ToString();
				root.Items.Add(trvItem);
				this.AddSubNodeItem(trvItem, dr["MENU_NAME"].ToString(), dr["MENU_ID"].ToString(), dtInfo);
			}
		}
	}
}
