using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test_DateTime
{
	class Program
	{
		static void Main(string[] args)
		{
			DateTime dt = new DateTime(2017,06,20,12,17,33,10);
			Console.WriteLine(dt.ToString(".yyyy.MM.dd.hh.mm.ss."));

			DateTime dt2 = new DateTime(2017,06,21,0,0,0,0);
			if(dt > dt2)
				Console.WriteLine(dt.ToString() + " > " + dt2.ToString());
			else
				Console.WriteLine(dt.ToString() + " < " + dt2.ToString());
		}
	}
}
