using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using System.Net;
//using System.IO;
using System.Net.Http;

namespace ConfigEditor_proj
{
	class Issue
	{
		public string Subject { get; set; }
		public string Done { get; set; }
		public string Author { get; set; }
	}
	class JsonController
	{
		async static Task<string> Request_Json()
		{
			//string result = null;
			//string url = "http://www.redmine.org/issues.json";
			//Console.WriteLine("url : " + url);

			//try
			//{
			//	HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			//	HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			//	Stream stream = response.GetResponseStream();
			//	StreamReader reader = new StreamReader(stream);
			//	result = reader.ReadToEnd();
			//	stream.Close();
			//	response.Close();
			//}
			//catch(Exception e)
			//{
			//	Console.WriteLine(e.Message);
			//}

			//return result;


			string url = "http://www.redmine.org/issues.json";
			HttpClient client = new HttpClient();
			Task<string> getStringTask = client.GetStringAsync(url);
			string result = await getStringTask;
			return result;
		}

		//public static void ParseJson(String json)
		//{
		//	List<Issue> issues = new List<Issue>();

		//	JObject obj = JObject.Parse(json);
		//	Console.WriteLine(obj);
		//	JArray array = JArray.Parse(obj["issues"].ToString());
		//	foreach(JObject itemObj in array)
		//	{
		//		Issue issue = new Issue();
		//		issue.Subject = itemObj["subject"].ToString();
		//		issue.Done = itemObj["done_ratio"].ToString();
		//		issue.Author = itemObj["author"]["name"].ToString();
		//		issues.Add(issue);
		//	}

		//	MainWindow.mWnd.IssueListView.ItemsSource = issues;
		//}
		static void ParseJson(String json)
		{
			List<Issue> issues = new List<Issue>();

			JObject obj = JObject.Parse(json);
			Console.WriteLine(obj.ToString());

			JArray array = JArray.Parse(obj["issues"].ToString());

			//Console.WriteLine(array);
			//foreach(JObject itemObj in array)
			//{
			//	Issue issue = new Issue();
			//	issue.Subject = itemObj["subject"].ToString();
			//	issue.Done = itemObj["done_ratio"].ToString();
			//	issue.Author = itemObj["author"]["name"].ToString();
			//	issues.Add(issue);
			//}

			//MainWindow.mWnd.IssueListView.ItemsSource = issues;
		}


		public static JToken parseJson(string json)
		{
			JToken obj = null;
			try
			{
				obj = JToken.Parse(json);
				Console.WriteLine(obj);
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}

			return obj;
		}


	}
}
