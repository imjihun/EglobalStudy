using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace test
{
	/// <summary>
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}
		private void ellipse_MouseMove(object sender, MouseEventArgs e)
		{
			Ellipse ellipse = sender as Ellipse;
			if(ellipse != null && e.LeftButton == MouseButtonState.Pressed)
			{
				DragDrop.DoDragDrop(ellipse,
									 ellipse.Fill.ToString(),
									 DragDropEffects.Copy);
			}
		}
		private Brush _previousFill = null;
		private void ellipse_DragEnter(object sender, DragEventArgs e)
		{
			Console.WriteLine("ellipse_DragEnter");
			Ellipse ellipse = sender as Ellipse;
			if(ellipse != null)
			{
				// Save the current Fill brush so that you can revert back to this value in DragLeave.
				_previousFill = ellipse.Fill;

				// If the DataObject contains string data, extract it.
				if(e.Data.GetDataPresent(DataFormats.StringFormat))
				{
					string dataString = (string)e.Data.GetData(DataFormats.StringFormat);

					// If the string can be converted into a Brush, convert it.
					BrushConverter converter = new BrushConverter();
					if(converter.IsValid(dataString))
					{
						Brush newFill = (Brush)converter.ConvertFromString(dataString);
						ellipse.Fill = newFill;
					}
				}
			}
		}
		private void ellipse_DragOver(object sender, DragEventArgs e)
		{
			Console.WriteLine("ellipse_DragOver");
			e.Effects = DragDropEffects.None;

			// If the DataObject contains string data, extract it.
			if(e.Data.GetDataPresent(DataFormats.StringFormat))
			{
				string dataString = (string)e.Data.GetData(DataFormats.StringFormat);

				// If the string can be converted into a Brush, allow copying.
				BrushConverter converter = new BrushConverter();
				if(converter.IsValid(dataString))
				{
					e.Effects = DragDropEffects.Copy | DragDropEffects.Move;
				}
			}
		}
		private void ellipse_DragLeave(object sender, DragEventArgs e)
		{
			Console.WriteLine("ellipse_DragLeave");
			Ellipse ellipse = sender as Ellipse;
			if(ellipse != null)
			{
				ellipse.Fill = _previousFill;
			}
		}
		private void ellipse_Drop(object sender, DragEventArgs e)
		{
			Console.WriteLine("ellipse_Drop");
			Ellipse ellipse = sender as Ellipse;
			if(ellipse != null)
			{
				// If the DataObject contains string data, extract it.
				if(e.Data.GetDataPresent(DataFormats.StringFormat))
				{
					string dataString = (string)e.Data.GetData(DataFormats.StringFormat);

					// If the string can be converted into a Brush, 
					// convert it and apply it to the ellipse.
					BrushConverter converter = new BrushConverter();
					if(converter.IsValid(dataString))
					{
						Brush newFill = (Brush)converter.ConvertFromString(dataString);
						ellipse.Fill = newFill;
					}
				}
			}
		}
		//protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
		//{
		//	base.OnGiveFeedback(e);
		//	// These Effects values are set in the drop target's
		//	// DragOver event handler.
		//	if(e.Effects.HasFlag(DragDropEffects.Copy))
		//	{
		//		Mouse.SetCursor(Cursors.Cross);
		//	}
		//	else if(e.Effects.HasFlag(DragDropEffects.Move))
		//	{
		//		Mouse.SetCursor(Cursors.Pen);
		//	}
		//	else
		//	{
		//		Mouse.SetCursor(Cursors.No);
		//	}
		//	e.Handled = true;
		//}
	}
}
