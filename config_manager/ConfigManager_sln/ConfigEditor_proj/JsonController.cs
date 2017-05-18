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
	class JsonController
	{
		static string error_message = "";
		public static string Error_message { get { return error_message; } }
		public static JToken parseJson(string json)
		{
			JToken obj = null;
			try
			{
				obj = JToken.Parse(json);
				//Console.WriteLine(obj);
			}
			catch(Exception e)
			{
				error_message = e.Message;
				Console.WriteLine(e.Message);
			}

			return obj;
		}


	}
}
