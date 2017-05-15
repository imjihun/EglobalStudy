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
		public async static Task<string> Request_Json()
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
		public static void ParseJson(String json)
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


		public static JObject parseJson(string json)
		{
			JObject obj = JObject.Parse(json);

			Console.WriteLine(obj);

			return obj;





			//Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
			//Console.WriteLine(values["type"]);
			//Console.WriteLine(values["comm_option"]);

			//Console.WriteLine((values["comm_option"] as JObject).Last);
			//Console.WriteLine(values["type"].GetType());
			//Console.WriteLine(values["comm_option"].GetType());

			//object obj = values["comm_option"];
			//Console.WriteLine(obj.GetType());

			//// dynamic .net 4.0 이상 지원
			//dynamic stuff = JObject.Parse(json);

			//Console.WriteLine(stuff["type"]);
			//Console.WriteLine(stuff["comm_option"]["sam_type"]);
			//Console.WriteLine(stuff["type"].GetType());
			//Console.WriteLine(stuff["comm_option"].GetType());
			//Console.WriteLine(stuff["comm_option"]["sam_type"].GetType());


			//Dictionary<string, object> values = new Dictionary<string, object>();
			//values.Add("key1", "value1");
			//values.Add("key2", "value2");

			//string json2 = JsonConvert.SerializeObject(values);
			//Console.WriteLine(json2);
		}
	}
}
