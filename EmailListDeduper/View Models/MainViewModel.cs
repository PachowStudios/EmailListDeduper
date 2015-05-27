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
		public RelayCommand AddDedupeCommand { get; set; }
		public RelayCommand RemoveDedupeCommand { get; set; }
		public RelayCommand AddCompareCommand { get; set; }
		public RelayCommand RemoveCompareCommand { get; set; }
		public RelayCommand SelectOutputFolderCommand { get; set; }
		public RelayCommand RunCommand { get; set; }

		public bool CanRun
		{ get { return !HasErrors && CanInteract; } }

		public bool CanInteract 
		{
			get { return canInteract; }
			set { canInteract = value; CommandManager.InvalidateRequerySuggested(); }
		}

		private bool canInteract = true;
		#endregion

		#region Bindable Properties
		[ListNotEmpty]
		public ObservableCollection<string> FilesToDedupe { get; set; }

		public ObservableCollection<string> FilesToCompareAgainst { get; set; }

		[Required]
		public string OutputFolder
		{
			get { return outputFolder; }
			set { outputFolder = value; RaisePropertyChanged(); }
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
			AutoValidate = false;
		}
		#endregion

		#region Command Methods
		private void AddDedupe(object obj)
		{
			var fileNames = OpenCsvFiles();

			if (fileNames != null)
				foreach (var fileName in fileNames)
					FilesToDedupe.Add(fileName);
		}

		private void RemoveDedupe(object obj)
		{
			var selectedItems = ((IList)obj).Cast<string>().ToList();

			foreach (var item in selectedItems)
				FilesToDedupe.Remove(item);
		}

		private void AddCompare(object obj)
		{
			var fileNames = OpenCsvFiles();

			if (fileNames != null)
				foreach (var fileName in fileNames)
					FilesToCompareAgainst.Add(fileName);
		}

		private void RemoveCompare(object obj)
		{
			var selectedItems = ((IList)obj).Cast<string>().ToList();

			foreach (var item in selectedItems)
				FilesToCompareAgainst.Remove(item);
		}

		private void SelectOutputFolder(object obj)
		{
			var folderName = OpenFolder();

			if (folderName != null)
				OutputFolder = folderName;
		}

		private void Run(object obj)
		{
			Validate();
			AutoValidate = true;

			if (HasErrors)
				return;

			CanInteract = false;

			try
			{
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
					var comparer = new PersonComparer();

					if (fileContents != null)
						dedupedLists.Add(fileName, fileContents.Except(itemsToCompareAgainst, comparer).Distinct(comparer).ToList());
				}

				foreach (var dedupedList in dedupedLists)
				{
					var saveFile = OutputFolder + "/" + Path.GetFileNameWithoutExtension(dedupedList.Key) + " - Deduped.csv";
					CSV.WritePeople(dedupedList.Value, saveFile);
				}

				MessageBox.Show("Dedupe complete.",
								"Complete",
								MessageBoxButton.OK,
								MessageBoxImage.Information);
			}
			catch (Exception e)
			{
				MessageBox.Show("Dedupe error: " + e.Message,
								"Error!",
								MessageBoxButton.OK,
								MessageBoxImage.Error);
			}
			finally
			{
				CanInteract = true;
			}
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

		private string OpenFolder()
		{
			var folderDialog = new CommonOpenFileDialog();
			folderDialog.IsFolderPicker = true;
			folderDialog.EnsurePathExists = true;

			if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
				return folderDialog.FileName;
			else
				return null;
		}
		#endregion
	}
}
