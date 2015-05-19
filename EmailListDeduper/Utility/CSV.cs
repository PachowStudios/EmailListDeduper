using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace EmailListDeduper
{
	public static class CSV
	{
		public static List<Person> ReadPeople(string fileName)
		{
			try
			{
				using (TextReader textReader = File.OpenText(fileName))
					using (CsvReader csvReader = new CsvReader(textReader))
						return csvReader.GetRecords<Person>().ToList();
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static bool WritePeople(List<Person> people, string fileName)
		{
			try
			{
				using (TextWriter textWriter = File.CreateText(fileName))
					using (CsvWriter csvWriter = new CsvWriter(textWriter))
						csvWriter.WriteRecords(people);
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}
	}

	public sealed class PersonMapping : CsvClassMap<Person>
	{
		public PersonMapping()
		{
			Map(m => m.Name).Name("Name");
			Map(m => m.Email).Name("Email");
		}
	}
}
