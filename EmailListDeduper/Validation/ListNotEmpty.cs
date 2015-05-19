using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace EmailListDeduper
{
	public class ListNotEmpty : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value != null)
			{
				var list = (IList)value;

				if (list.Count > 0)
					return ValidationResult.Success;
			}
			
			return new ValidationResult(ErrorMessage, new string[] { validationContext.MemberName });
		}
	}
}
