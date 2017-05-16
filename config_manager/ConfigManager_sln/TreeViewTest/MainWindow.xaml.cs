using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using System.Windows.Threading;

namespace TreeViewTest
{
	/// <summary>
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainWindow : Window
	{
		MyData myDataObject = new MyData(DateTime.Now);

		//ViewModel MyViewModel = 

		public MainWindow()
		{
			InitializeComponent();
			
			//make a new source
			Binding myBinding = new Binding("MyDataProperty");
			myBinding.Source = myDataObject;
			myText.SetBinding(TextBlock.TextProperty, myBinding);


			DispatcherTimer Timer = new DispatcherTimer();
			Timer.Interval = TimeSpan.FromSeconds(1);
			Timer.Tick += Timer_Tick;
			Timer.Start();

			/*
			 Here's how I handled the explicit binding tasks. First, I had an event handler for the TextChanged event which updated the source of the binding:
			 */
			// Push the text in the textbox to the bound property in the ViewModel
			//myText.GetBindingExpression(TextBox.TextProperty).UpdateSource();
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			Console.WriteLine(myDataObject.MyDataProperty);
		}

		#region .net 3.5 TextBox.Text binding error solve

		/*
		 Second, in I had an event handler for the TextBox's Loaded event. In that handler, I registered a handler for the PropertyChanged event of my ViewModel (the ViewModel was the "DataContext" here):
		*/
		private void ExplicitBindingTextBox_Loaded(object sender, RoutedEventArgs e)
		{
			TextBox textBox = sender as TextBox;

			if(textBox.DataContext as INotifyPropertyChanged == null)
				throw new InvalidOperationException("...");

			(textBox.DataContext as INotifyPropertyChanged).PropertyChanged +=
						  new PropertyChangedEventHandler(ViewModel_PropertyChanged);
		}

		/*
		Finally, in the PropertyChanged handler, I cause the TextBox to grab the value from the ViewModel (by initiating the UpdateTarget()). This makes the TextBox get the modified string from the ViewModel (in your case the one with replaced characters). In my case I also had to handle restoring the user's caret position after refreshing the text (from the UpdateTarget()). That part may or may not apply to your situation though. 
		*/
		void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			//	// This textbox only cares about the property it is bound to
			//	if(e.PropertyName != MyViewModel.ValueStrPropertyName)
			//		return;

			//	// "this" here refers to the actual textbox since I'm in a custom control
			//	//  that derives from TextBox
			//	BindingExpression bindingExp = this.GetBindingExpression(TextBox.TextProperty);
			//	// the version that the ViewModel has (a potentially modified version of the user's string)
			//	String viewModelValueStr;

			//	viewModelValueStr = (bindingExp.DataItem as MyViewModel).ValueStr;


			//	if(viewModelValueStr != this.Text)
			//	{
			//		// Store the user's old caret position (relative to the end of the str) so we can restore it
			//		//  after updating the text from the ViewModel's corresponding property.
			//		int oldCaretFromEnd = this.Text.Length - this.CaretIndex;

			//		// Make the TextBox's Text get the updated value from the ViewModel
			//		this.GetBindingExpression(TextBox.TextProperty).UpdateTarget();

			//		// Restore the user's caret index (relative to the end of the str)
			//		this.CaretIndex = this.Text.Length - oldCaretFromEnd;
			//	}

		}






		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
		}
		#endregion



	}
	public class MyData : INotifyPropertyChanged
	{
		private string myDataProperty;

		public MyData() { }

		public MyData(DateTime dateTime)
		{
			myDataProperty = "Last bound time was " + dateTime.ToLongTimeString();
		}

		public String MyDataProperty
		{
			get { return myDataProperty; }
			set
			{
				myDataProperty = value;
				OnPropertyChanged("MyDataProperty");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string info)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if(handler != null)
			{
				handler(this, new PropertyChangedEventArgs(info));
			}
			Console.WriteLine(info);
		}
	}
}
