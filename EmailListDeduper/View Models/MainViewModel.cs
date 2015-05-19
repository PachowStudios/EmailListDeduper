using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel.DataAnnotations;

namespace EmailListDeduper
{
	public class MainViewModel : ViewModelBase
	{
		public ObservableCollection<string> FilesToDedupe { get; set; }
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

		private string outputFolder;
	}
}
