﻿using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Manager_proj_4_net4.Windows
{
	/// <summary>
	/// Window_ViewFile.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class Window_ViewFile : MetroWindow
	{
		public Window_ViewFile(string text, string title)
		{
			InitializeComponent();
			this.Title = title;
			tblock.Text = text;
		}
	}
}
