using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace CofileUI.Classes
{
	class Log
	{
		private static void CheckNewLine(ref string str)
		{
			if(str[str.Length - 1] == '\n')
				str = str.TrimEnd('\n');

			if(str[str.Length - 1] != '\r')
				str += '\r';
		}

		public static void PrintError(string message, string caption = null, TextBoxBase output_ui = null)
		{
			string str = "[Error] ";

			if(caption != null)
				str += "[" + caption + "] ";

			str += message;

			//str += "\n";
			CheckNewLine(ref str);
			Console.WriteLine(str);

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
		public static void PrintConsole(string message, string caption = null)
		{
			string str = System.Environment.NewLine;

			if(caption != null)
				str += "[" + caption + "] ";

			str += message;

			Console.WriteLine(str);
		}

		public static void _ViewMessage(string message, string caption, TextBoxBase output_ui)
		{
			//string str = System.Environment.NewLine;
			string str = "";

			if(caption != null)
				str += "[" + caption + "] ";

			str += message;

			CheckNewLine(ref str);
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
		public static void ViewMessage(string message, string caption, TextBoxBase output_ui)
		{
			if(message == null || message == "")
				return;

			// 한줄씩 실행
			string[] split = message.Split('\n');
			for(int i = 0; i < split.Length; i++)
			{
				if(split[i].Length > 0)
					_ViewMessage(split[i], caption, output_ui);
			}

			// 마지막에 문단 나누기 추가
			//string str = "\r\n";
			string str = "";
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

		public static void ViewUndefine(string message, string caption, TextBoxBase output_ui)
		{
			//string str = System.Environment.NewLine;
			string str = "";

			if(caption != null)
				str += "[" + caption + "] ";

			str += message;

			//Console.WriteLine(str);
			CheckNewLine(ref str);
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
					rangeOfWord.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.LightGray);
					//rtb.AppendText(str);
					//rtb.Document.Blocks.Add(new Paragraph(new Inline()))
				}
			}
		}
	}
}
