﻿using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace test2
{
	class Program
	{
		static void Main(string[] args)
		{
			func();
			Console.WriteLine("main finish");
		}
		async static Task<int> sum(int a, int b)
		{
			Console.WriteLine("sum");
			//Thread.Sleep(1000);
			await Task.Delay(1000);
			Console.WriteLine("sum finish");
			return a + b;
		}
		async static void func()
		{
			Console.WriteLine("func()");
			int re = await sum(10, 20);
			Console.WriteLine("re = " + re);
		}
	}
}
