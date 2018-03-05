using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace ConsoleApplication4
{
	class Program
	{
		public static int SaveMailAttach(string folder_path, Outlook.OlDefaultFolders opt)
		{
			try
			{
				// Create the Outlook application.
				// in-line initialization
				Outlook.Application oApp = new Outlook.Application();

				// Get the MAPI namespace.
				Outlook.NameSpace oNS = oApp.GetNamespace("mapi");

				// Log on by using the default profile or existing session (no dialog box).
				oNS.Logon(Missing.Value, Missing.Value, false, true);

				// Alternate logon method that uses a specific profile name.
				// TODO: If you use this logon method, specify the correct profile name
				// and comment the previous Logon line.
				//oNS.Logon("profilename",Missing.Value,false,true);

				//Get the Inbox folder.
				Outlook.MAPIFolder oInbox = oNS.GetDefaultFolder(opt);

				//Get the Items collection in the Inbox folder.
				Outlook.Items oItems = oInbox.Items;

				foreach(var item in oItems)
				{
					Outlook.MailItem msg = item as Outlook.MailItem;
					if(msg == null)
						continue;

					for(int i = 1; i <= msg
					   .Attachments.Count; i++)
					{
						try
						{
							msg.Attachments[i].SaveAsFile
							(folder_path +
							msg.Attachments[i].FileName);
						}

						//Error handler.
						catch(Exception e)
						{
							Console.WriteLine("{0} Exception caught: ", e);
						}
					}
				}

				oNS.Logoff();

				oItems = null;
				oInbox = null;
				oNS = null;
				oApp = null;
			}

			//Error handler.
			catch(Exception e)
			{
				Console.WriteLine("{0} Exception caught: ", e);
			}
			return 0;
		}
		public static int SaveOutbox(string save_path)
		{
			try
			{
				Directory.CreateDirectory(save_path + @"\send\");
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}
			return SaveMailAttach(save_path + @"\send\", Outlook.OlDefaultFolders.olFolderSentMail);
		}
		public static int SaveInbox(string save_path)
		{
			try
			{
				Directory.CreateDirectory(save_path + @"\receive\");
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}
			return SaveMailAttach(save_path + @"\receive\", Outlook.OlDefaultFolders.olFolderInbox);
		}
		public static int Main(string[] args)
		{
			string save_path = @"D:\tmp\mail\";
			SaveOutbox(save_path);
			SaveOutbox(save_path);
			return 0;

		}
	}
}