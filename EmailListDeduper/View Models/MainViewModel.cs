using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel.DataAnnotations;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

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
		{ get { return (!HasErrors || !HasValidated) && CanInteract; } }

		public bool CanInteract { get; set; }

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
			AddDedupeCommand = new RelayCommand(AddDedupe, param => CanInteract);
			RemoveDedupeCommand = new RelayCommand(RemoveDedupe, param => CanInteract);
			AddCompareCommand = new RelayCommand(AddCompare, param => CanInteract);
			RemoveCompareCommand = new RelayCommand(RemoveCompare, param => CanInteract);
			SelectOutputFolderCommand = new RelayCommand(SelectOutputFolder, param => CanInteract);
			RunCommand = new RelayCommand(Run, param => CanRun);

			FilesToDedupe = new ObservableCollection<string>();
			FilesToCompareAgainst = new ObservableCollection<string>();
			CanInteract = true;
		}
		#endregion

		#region Command Methods
		private void AddDedupe(object obj)
		{
			var fileNames = OpenCsvFiles();

			if (fileNames != null)
				foreach (var fileName in fileNames)
					FilesToDedupe.Add(fileName);

			RaisePropertyChanged("FilesToDedupe");
		}

		private void RemoveDedupe(object obj)
		{
			var selectedItems = ((IList)obj).Cast<string>().ToList();

			foreach (var item in selectedItems)
				FilesToDedupe.Remove(item);

			RaisePropertyChanged("FilesToDedupe");
		}

		private void AddCompare(object obj)
		{
			var fileNames = OpenCsvFiles();

			if (fileNames != null)
				foreach (var fileName in fileNames)
					FilesToCompareAgainst.Add(fileName);

			RaisePropertyChanged("FilesToCompareAgainst");
		}

		private void RemoveCompare(object obj)
		{
			var selectedItems = ((IList)obj).Cast<string>().ToList();

			foreach (var item in selectedItems)
				FilesToCompareAgainst.Remove(item);

			RaisePropertyChanged("FilesToCompareAgainst");
		}

		private void SelectOutputFolder(object obj)
		{
			var folderDialog = new CommonOpenFileDialog();
			folderDialog.IsFolderPicker = true;
			folderDialog.EnsurePathExists = true;
			
			if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
				OutputFolder = folderDialog.FileName;
		}

		private void Run(object obj)
		{
			Validate();
			HasValidated = true;

			if (HasErrors)
				return;

			CanInteract = false;

			var itemsToCompareAgainst = new List<Person>();
			var dedupedLists = new Dictionary<string, List<Person>>();

			foreach (var fileName in FilesToCompareAgainst)
			{
				var fileContents = CSV.ReadPeople(fileName);

				if (fileContents != null)
					itemsToCompareAgainst.AddRange(fileContents);
			}

			foreach (var fileName in FilesToDedupe)
			{
				var fileContents = CSV.ReadPeople(fileName);
	
				if (fileContents != null)
					dedupedLists.Add(fileName, fileContents.Except(itemsToCompareAgainst, new PersonComparer()).ToList());
			}

			foreach (var dedupedList in dedupedLists)
			{
				var saveFile = OutputFolder + "/" + Path.GetFileNameWithoutExtension(dedupedList.Key) + " - Deduped.csv";
				CSV.WritePeople(dedupedList.Value, saveFile);
			}

			CanInteract = true;

			MessageBox.Show("Dedupe complete.",
							"",
							MessageBoxButton.OK,
							MessageBoxImage.Information);
		}
		#endregion

		#region Internal Methods
		private string[] OpenCsvFiles()
		{
			var openDialog = new OpenFileDialog();
			openDialog.Filter = "CSV Files (*.csv)|*.csv";
			openDialog.Multiselect = true;
			openDialog.CheckPathExists = true;

			if (openDialog.ShowDialog() == true)
				return openDialog.FileNames;
			else
				return null;
		}
		#endregion
	}
}
