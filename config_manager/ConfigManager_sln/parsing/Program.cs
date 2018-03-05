using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace parsing
{
	class Coin
	{
		const int MAX_LIST = 1024;

		string coin_str = "";
		string err = "";
		string url = "https://api.bithumb.com/public/ticker/";
		string path;

		double aver_rate = 0;
		int cnt = 0;
		int interval = 30;
		int cnt_interval = 0;
		double prev_closing_price = 0;
		double cur_closing_price = 0;
		double cur_rate = 0;
		List<double> list_rate = new List<double>();

		double cash = 1000000;
		double coin_cnt = 0;

		public Coin(string _coin_str, string _url)
		{
			coin_str = _coin_str;
			url = _url;
		}

		public int PrintCoinStatus()
		{
			FileStream fs = null;
			byte[] buffer/* = new byte[MAX_BUFFER]*/;
			int size_write = 0;

			string str = url + coin_str;
			try
			{
				path = DateTime.Now.ToString("yyyyMMdd") + coin_str + ".txt";
				// 경로에 파일 쓰기.
				fs = new FileStream(path, FileMode.Append);

				//string Text = httpWebGET(str, str);
				string text = fetch(str);
				JObject jobj = JObject.Parse(text);
				jobj.Add("time", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
				buffer = Encoding.UTF8.GetBytes(jobj.ToString() + ",\r\n");
				size_write = buffer.Length;
				fs.Write(buffer, 0, size_write);
				Console.Write("[" + coin_str + "] ");
				Console.Write("closing_price = " + jobj["data"]["closing_price"]);
				Console.Write(", min_price = " + jobj["data"]["min_price"]);
				Console.WriteLine(", max_price = " + jobj["data"]["max_price"]);
				//Console.WriteLine(jobj.ToString());
				Console.WriteLine("aver_rate(" + cnt + ") : " + aver_rate);
				Console.WriteLine("cur_rate");
				list_rate.ForEach((rate) => { Console.Write(rate + " "); });
				Console.WriteLine("\n");
				//Console.WriteLine(err);
				JValue jval = jobj["data"]["closing_price"] as JValue;
				cur_closing_price = Double.Parse(jval.ToString());

				//Console.WriteLine(cash + ", " + coin_cnt * cur_closing_price + ", " + cash + coin_cnt * cur_closing_price);
				//if(jval != null && cur_closing_price > (double)17500)
				//{
				//	System.Windows.Forms.MessageBox.Show(
				//		new Form() { WindowState = FormWindowState.Maximized, TopMost = true },
				//		jval.ToString()
				//	);
				//}
				//if(jval != null && cur_closing_price < (double)1950)
				//{
				//	System.Windows.Forms.MessageBox.Show(
				//		new Form() { WindowState = FormWindowState.Maximized, TopMost = true },
				//		jval.ToString()
				//	);
				//}
				cnt_interval++;
				if(cnt_interval > interval)
				{
					if(prev_closing_price != 0)
					{
						cur_rate = (cur_closing_price - prev_closing_price);
						list_rate.Add(cur_rate);
						if(list_rate.Count > MAX_LIST)
							list_rate.RemoveAt(0);

						if(cur_rate < 0)
							cur_rate = -cur_rate;

						aver_rate = aver_rate * cnt + cur_rate;
						cnt++;
						aver_rate /= cnt;

						if(aver_rate * 5 < cur_rate)
						{
							System.Windows.Forms.MessageBox.Show(
								new Form() { WindowState = FormWindowState.Maximized, TopMost = true },
								cur_rate + ", " + jval.ToString()
							);
						}
					}

					prev_closing_price = cur_closing_price;
					cnt_interval = 0;
				}
				fs.Close();

			}
			catch(Exception e)
			{
				if(fs != null)
					fs.Close();
				Console.WriteLine(e.Message);
				err += "[" + DateTime.Now.ToString("yyyyMMdd") + "] : [ERROR] " + e.Message + "\n";
			}
			return 0;
		}
		static string fetch(string url)
		{
			var json = new WebClient().DownloadString(url);
			return json;
		}
		//void buyCheck(double cur_price)
		//{
		//	if(is_buy == 0 && prev_price - cur_price > 0)
		//	{
		//		cnt_xrp = coin / cur_closing_price * (1 - fees);
		//		coin = 0;
		//		is_buy = 1;
		//	}
		//}
		//void selCheck(double cur_price)
		//{
		//	if(is_buy == 1 && prev_price - cur_price < 0)
		//	{
		//		coin = cnt_xrp * cur_closing_price * (1 - fees);
		//		cnt_xrp = 0;
		//		is_buy = 0;
		//	}
		//}
	}
	class Program
	{
		static HttpWebRequest request;
		static HttpWebResponse response;
		static StreamReader readerPost;
		static string resResult;
		

		static double coin = 1000000;
		static double fees = 0.0015;
		static double max_waiting_sec = 600;
		static double upper_limit;
		static double upper_limit_rate;
		static double lower_limit;
		static double lower_limit_rate;
		static double increase_rete;
		static double decrease_rete;
		static string err = "";


		static double buy_price = 0;
		static double prev_price = 0;
		static int is_buy= 0;

		static double cnt_xrp;


		

		static void Main(string[] args)
		{
			string url = "https://api.bithumb.com/public/ticker/";
			Coin btc = new Coin("BTC", url);
			Coin eth = new Coin("ETH", url);
			Coin xrp = new Coin("XRP", url);
			Coin eos = new Coin("EOS", url);

			while(true)
			{
				Console.Clear();

				btc.PrintCoinStatus();
				eth.PrintCoinStatus();
				xrp.PrintCoinStatus();
				eos.PrintCoinStatus();

				Thread.Sleep(1000);
			}
			//Console.WriteLine(text.Length);
		}
		static string httpWebGET(string url, string Referer)
		{
			try
			{
				string[] arr =  { "http://", "/"};
				request = (HttpWebRequest)WebRequest.Create(url);
				request.Method = "GET";
				request.Referer = Referer;
				request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)";
				request.ContentType = "application/x-www-form-urlencoded";
				request.Host = url.Split(arr, StringSplitOptions.RemoveEmptyEntries)[0];
				request.KeepAlive = true;
				request.AllowAutoRedirect = false;

				response = (HttpWebResponse)request.GetResponse();
				readerPost = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8, true);   // Encoding.GetEncoding("EUC-KR")
				string resResult = readerPost.ReadToEnd();
				//Getcookie = response.GetResponseHeader("Set-Cookie"); // 쿠키정보 값을 확인하기 위해서
				readerPost.Close();
				response.Close();
				return resResult;
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}
			return null;
		}
	}
}
