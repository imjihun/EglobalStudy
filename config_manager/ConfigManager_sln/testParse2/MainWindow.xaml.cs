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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace testParse2
{
	/// <summary>
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			wepBrowser1.Navigate("http://blog.danawa.com/prod/?prod_c=1132675&cate_c1=861&cate_c2=873&cate_c3=959&cate_c4=0");
		}
		private void viewPrice()
		{
			HtmlDocument doc = webBrowser1.Document;
			string tableId = "cm";
			HtmlElement cm = doc.GetElementById(tableId);
			if(cm == null)
			{
				//MessageBox.Show("cm을 찾을수 없음");
				return;
			}
			HtmlElementCollection trs = cm.GetElementsByTagName("TR");
			HtmlElementCollection tds = trs[1].GetElementsByTagName("TD");
			foreach(HtmlElement el in tds)
			{
				if(el.GetAttribute("className") == "price_Point")
				{
					el.Focus();
					MessageBox.Show(el.InnerText);
					break;
				}
			}
		}

		private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			if(e.Url.AbsoluteUri == webBrowser1.Url.AbsoluteUri)
				viewPrice();
		}
	}
}