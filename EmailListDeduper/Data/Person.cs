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
}
