using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AutoExchange
{
	class Program
	{
		static HttpWebRequest request;
		static HttpWebResponse response;
		static StreamReader readerPost;
		static string resResult;


		static void Main(string[] args)
		{
			//string api_url = "https://api.bithumb.com/public/ticker/XRP";
			string api_url = "https://api.bithumb.com/trade/place";
			string path = "XRP.txt";

			double coin = 1000000;
			double fees = 0.0015;
			double max_waiting_sec = 600;
			double upper_limit;
			double upper_limit_rate;
			double lower_limit;
			double lower_limit_rate;
			double increase_rete;
			double decrease_rete;
			double aver_rate = 0;
			int cnt = 0;
			int interval = 30;
			int cnt_interval = 0;
			double prev_closing_price = 0;
			double cur_closing_price = 0;
			double cur_rate = 0;
			List<double> list_rate = new List<double>();
			const int MAX_LIST = 1024;

			path = DateTime.Now.ToString("yyyyMMdd") + path;
			// 경로에 파일 쓰기.
			FileStream fs = new FileStream(path, FileMode.Append);

			byte[] buffer/* = new byte[MAX_BUFFER]*/;
			int size_write = 0;
			buffer = Encoding.UTF8.GetBytes("[\r\n");
			size_write = buffer.Length;
			fs.Write(buffer, 0, size_write);
			
			try
			{
				//string Text = httpWebGET(str, str);
				string text = fetch(api_url);
				JObject jobj = JObject.Parse(text);
				jobj.Add("time", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
				buffer = Encoding.UTF8.GetBytes(jobj.ToString() + ",\r\n");
				size_write = buffer.Length;
				fs.Write(buffer, 0, size_write);
				//Console.Clear();
				Console.WriteLine(jobj.ToString());
			}
			catch(Exception e)
			{
				fs.Close();
				Console.WriteLine(e.Message);
			}
		}
		static string fetch(string url)
		{
			WebClient web = new WebClient();
			NameValueCollection query = new NameValueCollection();
			/*
			 * apiKey	String	apiKey
secretKey	String	scretKey
order_currency	String	BTC, ETH, DASH, LTC, ETC, XRP, BCH, XMR, ZEC, QTUM, BTG, EOS (기본값: BTC)
Payment_currency	String	KRW (기본값)
units	Float	주문 수량

- 1회 최소 수량 (BTC: 0.001 | ETH: 0.01 | DASH: 0.01 | LTC: 0.1 | ETC: 0.1 | XRP: 10 | BCH: 0.001 | XMR: 0.01 | ZEC: 0.001 | QTUM: 0.1 | BTG: 0.01 | EOS: 1)
- 1회 최대 수량 (BTC: 300 | ETH: 2,500 | DASH: 4,000 | LTC: 15,000 | ETC: 30,000 | XRP: 2,500,000 | BCH: 1,200 | XMR: 10,000 | ZEC: 2,500 | QTUM: 30,000 | BTG: 1,200 | EOS: 100,000)
price	Int	1Currency당 거래금액 (BTC, ETH, DASH, LTC, ETC, XRP, BCH, XMR, ZEC, QTUM, BTG, EOS)
type	String	거래유형 (bid : 구매, ask : 판매)
			 * 
			 */

			//query.Add("apiKey", "bbd0550b96ed3feb605a6bbc2d815ed3");
			//query.Add("secretKey", "6787748d4a989b81b44f627b9f24350e");
			//query.Add("order_currency", "XRP");
			//query.Add("Payment_currency", "KRW");
			//query.Add("units", "10");
			//query.Add("price", "3000");
			//query.Add("type", "ask");
			//web.QueryString = query;
			//var retval = new WebClient().UploadString(url, "POST", query.ToString());

			var retval = "";
			var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.ContentType = "text/json";
			httpWebRequest.Method = "POST";
			using(var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				JObject data = new JObject();
				data.Add("apiKey", "bbd0550b96ed3feb605a6bbc2d815ed3");
				data.Add("secretKey", "6787748d4a989b81b44f627b9f24350e");
				data.Add("order_currency", "XRP");
				data.Add("payment_currency", "KRW");
				data.Add("units", (double)10);
				data.Add("price", (int)3000);
				data.Add("type", "ask");
				streamWriter.Write(data.ToString());
				Console.WriteLine(data.ToString());
				streamWriter.Flush();
				streamWriter.Close();
				var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				using(var streamReader = new StreamReader(httpResponse.GetResponseStream()))
				{
					retval = streamReader.ReadToEnd();
				}
			}
			return retval;
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
