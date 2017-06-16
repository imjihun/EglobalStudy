using Manager_proj_4_net4.Classes;
using Manager_proj_4_net4.Windows;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager_proj_4_net4.Classes
{
	class JsonController
	{
		static string error_message = "";
		public static string Error_message { get { return error_message; } }
		public static JToken parseJson(string json)
		{
			if(json == null)
				return null;
			JToken obj = null;
			try
			{
				obj = JToken.Parse(json);
				//Log.Print(obj);
			}
			catch(Exception e)
			{
				error_message = e.Message;
				Log.PrintConsole(e.Message);
				WindowMain.current.ShowMessageDialog("Json Context Error!!", JsonController.Error_message, MahApps.Metro.Controls.Dialogs.MessageDialogStyle.Affirmative);
			}

			return obj;
		}
	}
}
