using System;
using System.ComponentModel.DataAnnotations;

public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(!(value is DateTime))
                return new ValidationResult("Not a valid date");
                
            DateTime date = Convert.ToDateTime(value);

            if(date < DateTime.Now)
                return new ValidationResult("Date must be in the future!");

            return ValidationResult.Success;

        }
    }