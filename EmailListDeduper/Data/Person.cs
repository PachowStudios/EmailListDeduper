using System.Collections.Generic;

namespace EmailListDeduper
{
	public class Person
	{
		public string Name { get; set; }
		public string Email { get; set; }

		public Person()
			: this("", "")
		{ }

		public Person(string name, string email)
		{
			this.Name = name;
			this.Email = email;
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
