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
			var people = new List<Person>();

			try
			{
				using (TextReader textReader = File.OpenText(fileName))
				{
					using (CsvReader csvReader = new CsvReader(textReader))
					{
						csvReader.Configuration.ApplySettings();
						
						while (csvReader.Read())
						{
							var person = csvReader.GetRecord<Person>();

							if (person.IsValid())
								people.Add(person);
						}
					}
				}
			}
			catch (Exception)
			{
				return null;
			}

			return people;
		}

		public static bool WritePeople(List<Person> people, string fileName)
		{
			try
			{
				using (TextWriter textWriter = File.CreateText(fileName))
				{
					using (CsvWriter csvWriter = new CsvWriter(textWriter))
					{
						csvWriter.Configuration.ApplySettings();
						csvWriter.WriteRecords(people);
					}
				}
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}

		private static void ApplySettings(this CsvConfiguration parent)
		{
			parent.DetectColumnCountChanges = true;
			parent.IgnoreHeaderWhiteSpace = true;
			parent.IsHeaderCaseSensitive = false;
			parent.QuoteNoFields = true;
			parent.SkipEmptyRecords = true;
		}
	}

	public sealed class PersonMapping : CsvClassMap<Person>
	{
		public PersonMapping()
		{
			Map(m => m.Name).Name("Name").Default("");
			Map(m => m.Email).Name("Email").Default("");
		}
	}
}
