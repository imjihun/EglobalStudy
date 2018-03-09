using System;
using System.Collections.ObjectModel;

namespace CofileUI.UserControls.CustomUI.Cofile
{
	class LinuxFileViewModel : ViewModelBase
	{
		public RelayCommand MouseDown { get; set; }

		ObservableCollection<LinuxFileModel> root = new ObservableCollection<LinuxFileModel>();
		public ObservableCollection<LinuxFileModel> Root
		{
			get { return root; }
			set { root = value; RaisePropertyChanged("Root"); }
		}

		public LinuxFileViewModel()
		{
			Root.Add(new LinuxFileModel(){ Path = "/", IsDirectory = true });
		}
	}
}
