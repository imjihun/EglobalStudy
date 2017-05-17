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
		
		public MainWindow()
		{
			
			InitializeComponent();

			//make a new source
			NewBinding(myText0);
			NewBinding(myText2);


			DispatcherTimer Timer = new DispatcherTimer();
			Timer.Interval = TimeSpan.FromSeconds(1);
			Timer.Tick += Timer_Tick;
			Timer.Start();

			/*
			 Here's how I handled the explicit binding tasks. First, I had an event handler for the TextChanged event which updated the source of the binding:
			 */

			myText1.TextChanged += MyText1_TextChanged;
		}
		void NewBinding(TextBox t)
		{
			Console.WriteLine(Binding.SourceUpdatedEvent.HandlerType + ", " + Binding.SourceUpdatedEvent.Name + ", " + Binding.SourceUpdatedEvent.OwnerType + ", " + Binding.SourceUpdatedEvent.RoutingStrategy);
			Console.WriteLine(Binding.TargetUpdatedEvent.HandlerType + ", " + Binding.TargetUpdatedEvent.Name + ", " + Binding.TargetUpdatedEvent.OwnerType + ", " + Binding.TargetUpdatedEvent.RoutingStrategy);
			Console.WriteLine("SourceUpdatedEvent = " + Binding.SourceUpdatedEvent + ", TargetUpdatedEvent = " + Binding.TargetUpdatedEvent);
			//make a new source
			Binding myBinding = new Binding("MyDataProperty");
			//myBinding.
			//myBinding.Source = myDataObject;
			t.SetBinding(TextBox.TextProperty, myBinding);

			Console.WriteLine("SourceUpdatedEvent = " + Binding.SourceUpdatedEvent + ", TargetUpdatedEvent = " + Binding.TargetUpdatedEvent);
		}

		private void MyText1_TextChanged(object sender, TextChangedEventArgs e)
		{
			// Push the text in the textbox to the bound property in the ViewModel
			TextBox textBox = sender as TextBox;
			textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			myDataObject.MyDataProperty = "Last bound time was " + DateTime.Now.ToLongTimeString();
			//Console.WriteLine(myDataObject.MyDataProperty);
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
			TextBox tb = sender as TextBox;
			if(tb == null)
				return;

			// This textbox only cares about the property it is bound to
			if(e.PropertyName != MyViewModel.ValueStrPropertyName)
				return;

			// "this" here refers to the actual textbox since I'm in a custom control
			//  that derives from TextBox
			BindingExpression bindingExp = this.GetBindingExpression(TextBox.TextProperty);
			// the version that the ViewModel has (a potentially modified version of the user's string)
			String viewModelValueStr;

			viewModelValueStr = (bindingExp.DataItem as MyViewModel).ValueStr;


			if(viewModelValueStr != tb.Text)
			{
				// Store the user's old caret position (relative to the end of the str) so we can restore it
				//  after updating the text from the ViewModel's corresponding property.
				int oldCaretFromEnd = tb.Text.Length - tb.CaretIndex;

				// Make the TextBox's Text get the updated value from the ViewModel
				this.GetBindingExpression(TextBox.TextProperty).UpdateTarget();

				// Restore the user's caret index (relative to the end of the str)
				tb.CaretIndex = tb.Text.Length - oldCaretFromEnd;
			}

		}

		class MyViewModel : INotifyPropertyChanged
		{
			public static string ValueStrPropertyName { get; private set; }
			public string ValueStr { get; private set; }

			public event PropertyChangedEventHandler PropertyChanged;

		}






		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
		}
		#endregion



	}
	class MyBinding : Binding
	{
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
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MyDataProperty"));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		//private void OnPropertyChanged(string info)
		//{
		//	//PropertyChangedEventHandler handler = PropertyChanged;
		//	//if(handler != null)
		//	//{
		//	//	handler(this, new PropertyChangedEventArgs(info));
		//	//}
		//	PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
		//}
	}
}
