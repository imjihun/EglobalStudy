using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StackTrace
{
	class Program
	{
		static void printMyName()
		{
			StackFrame stackFrame = new StackFrame();
			MethodBase methodBase = stackFrame.GetMethod();
			Console.WriteLine(methodBase.Name); // Displays "WhatsmyName"
		}
		static void Main(string[] args)
		{
			string str = "tail -n10 /home/cofile/var/log/event_log\r\n2017-06-22 18:10:54 : Start Encrypting using /home/cofile/var/conf/cofile/tail.json...\r\n2017-06-22 18:10:55 : Error: Can't open file //file_config_default.json.coenc for read with errno=[2].\r\n\r\n2017-06-22 18:10:55 : Inform: End encryption file [//file_config_default.json.coenc].[-102]\r\n\r\n2017-06-22 18:10:56 : Start Encrypting using /home/cofile/var/conf/cofile/tail.json...\r\n2017-06-22 18:10:56 : Error: Can't open file //file_config_default.json.coenc.tmp for read with errno=[2].\r\n\r\n2017-06-22 18:10:56 : Inform: End encryption file [//file_config_default.json.coenc.tmp].[-102]\r\n\r\n";
			Console.WriteLine(str.Split('\n').Length);
			//Call0();
		}
		// function to display its name
		private static void Call0()
		{
			StackFrame stackFrame = new StackFrame();
			MethodBase methodBase = stackFrame.GetMethod();
			Console.WriteLine(methodBase.Name); // Displays "WhatsmyName"
			Call1();
		}
		private static void Call1()
		{
			StackFrame stackFrame = new StackFrame();
			MethodBase methodBase = stackFrame.GetMethod();
			Console.WriteLine(methodBase.Name); // Displays "WhatsmyName"
			Call2();
		}
		private static void Call2()
		{
			StackFrame stackFrame = new StackFrame();
			MethodBase methodBase = stackFrame.GetMethod();
			Console.WriteLine(methodBase.Name); // Displays "WhatsmyName"
			Call3();
		}
		private static void Call3()
		{
			StackFrame stackFrame = new StackFrame();
			MethodBase methodBase = stackFrame.GetMethod();
			Console.WriteLine(methodBase.Name); // Displays "WhatsmyName"
			print();
		}
		// Function to display parent function
		private static void print()
		{
			StackFrame stackFrame = new StackFrame();
			MethodBase methodBase = stackFrame.GetMethod();
			Console.WriteLine(methodBase.Name); // Displays "WhatsmyName"

			System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
			//Console.WriteLine("FrameCoount = " + stackTrace.FrameCount);
			for(int i = 0; i < stackTrace.FrameCount; i++)
			{
				stackFrame = stackTrace.GetFrame(i);
				methodBase = stackFrame.GetMethod();
				// Displays "WhatsmyName"
				Console.WriteLine(" Parent Method Name {0} ", methodBase.Name);
			}
		}
	}
}
