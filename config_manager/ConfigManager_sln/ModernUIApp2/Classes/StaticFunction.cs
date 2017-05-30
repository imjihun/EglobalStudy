using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Manager_proj_4
{
	class StaticFunction
	{
		static ContentControl main;
		public static ContentControl Main { get { return main; } set { main = value; } }
	}
}
