using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace EmailListDeduper
{
	public class Person
	{
		private const string EmailRegexPattern = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";

		public string Name 
		{
			get { return name; }
			set { name = FilterData(value); }
		}
		public string Email
		{
			get { return email; }
			set { email = FilterData(value); }
		}

		private string name;
		private string email;

		public Person()
			: this("", "")
		{ }

		public Person(string name, string email)
		{
			this.Name = name;
			this.Email = email;
		}

		public bool IsValid()
		{
			if (Name == "" || Email == "")
				return false;

			if (!Regex.IsMatch(Email, EmailRegexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled))
				return false;

			return true;
		}

		private string FilterData(string input)
		{
			if (input == null)
				return string.Empty;

			input = input.Trim();

			StringBuilder stringBuilder = new StringBuilder(input.Length);

			for (int i = 0; i < input.Length; i++)
			{
				char c = input[i];

				if (c == ',')
					c = '.';

				if ((c != ' ' && c != '\t' && c != '\'' && c != (char)160 && c != (char)65533) || 
					(c == ' ' && input[i + 1] != ' '))
					stringBuilder.Append(c);
			}

			return stringBuilder.ToString();
		}
	}

	public class PersonComparer : IEqualityComparer<Person>
	{
		public bool Equals(Person a, Person b)
		{
			if (a == null || b == null)
				return false;

			if (object.ReferenceEquals(a, b))
				return true;

			return a.Email.ToLower() == b.Email.ToLower();
		}

		public int GetHashCode(Person person)
		{
			if (person == null)
				return 0;

			return person.Email.ToLower().GetHashCode();
		}
	}
}
