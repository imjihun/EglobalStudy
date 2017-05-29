﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace Manager_proj_3
{
	class Log
	{
		public static void PrintError(string message, string caption = null, TextBoxBase output_ui = null)
		{
			string str = "[Error] ";

			if(caption != null)
				str += "[" + caption + "] ";

			str += message;

			Console.WriteLine(str);
			//str += "\n";

			if(output_ui != null)
			{
				TextBox tb = output_ui as TextBox;
				if(tb != null)
					tb.Text += str;


				RichTextBox rtb = output_ui as RichTextBox;
				if(rtb != null)
				{
					TextRange rangeOfWord = new TextRange(rtb.Document.ContentEnd, rtb.Document.ContentEnd);
					rangeOfWord.Text = str;
					rangeOfWord.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
					//rangeOfWord.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Regular);
				}
			}
		}
		public static void Print(string message, string caption = null, TextBoxBase output_ui = null)
		{
			string str = System.Environment.NewLine;

			if(caption != null)
				str += "[" + caption + "] ";

			str += message;

			Console.WriteLine(str);
			//str += "\n";

			//if(output_ui != null)
			//{
			//	TextBox tb = output_ui as TextBox;
			//	if(tb != null)
			//		tb.Text += str;


			//	RichTextBox rtb = output_ui as RichTextBox;
			//	if(rtb != null)
			//		rtb.AppendText(str);
			//}
		}
		public static void ViewMessage(string message, string caption, TextBoxBase output_ui)
		{
			//string str = System.Environment.NewLine;
			string str = "";

			if(caption != null)
				str += "[" + caption + "] ";

			str += message;

			Console.WriteLine(str);
			//str += System.Environment.NewLine;

			if(output_ui != null)
			{
				TextBox tb = output_ui as TextBox;
				if(tb != null)
					tb.Text += str;


				RichTextBox rtb = output_ui as RichTextBox;
				if(rtb != null)
				{
					TextRange rangeOfWord = new TextRange(rtb.Document.ContentEnd, rtb.Document.ContentEnd);
					rangeOfWord.Text = str;
					rangeOfWord.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
					//rtb.AppendText(str);
					//rtb.Document.Blocks.Add(new Paragraph(new Inline()))
				}
			}
		}
	}
}
