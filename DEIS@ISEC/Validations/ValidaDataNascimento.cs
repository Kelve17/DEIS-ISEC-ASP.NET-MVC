using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Deis.ValidationAttributes
{
    [AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ValidaDataNascimento : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                DateTime _nascimento = Convert.ToDateTime(value);
                if (_nascimento > DateTime.Now)
                {
                    return new ValidationResult("Data Atual nao pode ser superior à data de hoje....");
                }
            }
            return ValidationResult.Success;
        }
    }
}