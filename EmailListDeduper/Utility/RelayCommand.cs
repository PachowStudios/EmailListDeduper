using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EmailListDeduper
{
	public class RelayCommand : ICommand
	{
		private Action<object> execute;
		private Predicate<object> canExecute;
		private event EventHandler canExecuteChanged;

		public event EventHandler CanExecuteChanged
		{
			add
			{
				CommandManager.RequerySuggested += value;
				this.canExecuteChanged += value;
			}
			remove
			{
				CommandManager.RequerySuggested -= value;
				this.canExecuteChanged -= value;
			}
		}

		public RelayCommand(Action<object> execute)
			: this(execute, DefaultCanExecute)
		{ }

		public RelayCommand(Action<object> execute, Predicate<object> canExecute)
		{
			this.execute = execute;
			this.canExecute = canExecute;
		}

		public void Execute(object parameter)
		{
			this.execute(parameter);
		}

		public bool CanExecute(object parameter)
		{
			return this.canExecute != null && this.canExecute(parameter);
		}

		public void OnCanExecuteChangeD()
		{
			EventHandler handler = this.canExecuteChanged;

			if (handler != null)
				handler.Invoke(this, EventArgs.Empty);
		}

		public void Destroy()
		{
			this.execute = _ => { return; };
			this.canExecute = _ => false;
		}

		private static bool DefaultCanExecute(object parameter)
		{ return true; }
	}
}
