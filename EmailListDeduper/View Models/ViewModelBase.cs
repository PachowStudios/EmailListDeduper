using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EmailListDeduper
{
	public abstract class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		private ConcurrentDictionary<string, List<string>> errors =
			new ConcurrentDictionary<string, List<string>>();
		private object threadLock = new object();

		public bool HasErrors
		{
			get { return errors.Any(kv => kv.Value != null && kv.Value.Count > 0); }
		}

		public void RaisePropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;

			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));

			ValidateAsync();
		}

		public void OnErrorsChanged(string propertyName)
		{
			var handler = ErrorsChanged;

			if (handler != null)
				handler(this, new DataErrorsChangedEventArgs(propertyName));
		}

		public IEnumerable GetErrors(string propertyName)
		{
			List<string> errorsForName;

			errors.TryGetValue(propertyName, out errorsForName);

			return errorsForName;
		}

		public Task ValidateAsync()
		{
			return Task.Run(() => Validate());
		}

		protected void Validate()
		{
			lock (threadLock)
			{
				var validationContext = new ValidationContext(this, null, null);
				var validationResults = new List<ValidationResult>();

				Validator.TryValidateObject(this, validationContext, validationResults, true);

				foreach (var kv in errors.ToList())
				{
					if (validationResults.All(r => r.MemberNames.All(m => m != kv.Key)))
					{
						List<string> outList;
						errors.TryRemove(kv.Key, out outList);
						OnErrorsChanged(kv.Key);
					}
				}

				var q = from r in validationResults
						from m in r.MemberNames
						group r by m into g
						select g;

				foreach (var property in q)
				{
					var messages = property.Select(r => r.ErrorMessage).ToList();

					if (errors.ContainsKey(property.Key))
					{
						List<string> outList;
						errors.TryRemove(property.Key, out outList);
					}

					errors.TryAdd(property.Key, messages);
					OnErrorsChanged(property.Key);
				}
			}
		}
	}
}
