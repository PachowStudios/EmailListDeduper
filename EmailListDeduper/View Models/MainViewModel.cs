using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel.DataAnnotations;
using Microsoft.Win32;

namespace EmailListDeduper
{
	public class MainViewModel : ViewModelBase
	{
		#region Commands
		public ICommand AddDedupeCommand { get; set; }
		public ICommand RemoveDedupeCommand { get; set; }
		public ICommand AddCompareCommand { get; set; }
		public ICommand RemoveCompareCommand { get; set; }
		public ICommand SelectOutputFolderCommand { get; set; }
		public ICommand RunCommand { get; set; }

		public bool CanRun
		{ get { return !HasErrors || !HasValidated; } }

		public bool HasValidated { get; set; }
		#endregion

		#region Bindable Properties
		[ListNotEmpty]
		public ObservableCollection<string> FilesToDedupe { get; set; }

		[ListNotEmpty]
		public ObservableCollection<string> FilesToCompareAgainst { get; set; }

		[Required]
		public string OutputFolder
		{
			get { return outputFolder; }
			set
			{
				outputFolder = value;
				RaisePropertyChanged("OutputFolder");
			}
		}
		#endregion

		#region Internal Properties
		private string outputFolder;
		#endregion

		#region Constructor
		public MainViewModel()
		{
			AddDedupeCommand = new RelayCommand(AddDedupe);
			RemoveDedupeCommand = new RelayCommand(RemoveDedupe);
			AddCompareCommand = new RelayCommand(AddCompare);
			RemoveCompareCommand = new RelayCommand(RemoveCompare);
			SelectOutputFolderCommand = new RelayCommand(SelectOutputFolder);
			RunCommand = new RelayCommand(Run, param => CanRun);

			FilesToDedupe = new ObservableCollection<string>();
			FilesToCompareAgainst = new ObservableCollection<string>();
		}
		#endregion

		#region Command Methods
		private void AddDedupe(object obj)
		{
			string fileName = OpenCsvFile();

			if (fileName != null)
				FilesToDedupe.Add(fileName);
		}

		private void RemoveDedupe(object obj)
		{
		}

		private void AddCompare(object obj)
		{
			string fileName = OpenCsvFile();

			if (fileName != null)
				FilesToCompareAgainst.Add(fileName);
		}

		private void RemoveCompare(object obj)
		{
		}

		private void SelectOutputFolder(object obj)
		{
		}

		private void Run(object obj)
		{
			Validate();
			HasValidated = true;

			if (HasErrors)
				return;
		}
		#endregion

		#region Internal Methods
		private string OpenCsvFile()
		{
			OpenFileDialog openDialog = new OpenFileDialog();
			openDialog.Filter = "CSV Files (*.csv)|*.csv";

			if (openDialog.ShowDialog() == true)
				return openDialog.FileName;
			else
				return null;
		}
		#endregion
	}
}
