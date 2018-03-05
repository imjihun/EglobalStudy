using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace testParse
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			webBrowser1.Navigate("http://www.g2b.go.kr/index.jsp");
		}
		private void viewPrice()
		{
			HtmlDocument doc = webBrowser1.Document;
			string tableId = "cm";
			HtmlElement cm = doc.GetElementById(tableId);
			if(cm == null)
			{
				MessageBox.Show("cm을 찾을수 없음");
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