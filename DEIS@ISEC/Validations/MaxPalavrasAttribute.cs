using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Deis.ValidationAttributes
{
    public class MaxPalavrasAttribute : ValidationAttribute
    {
        private readonly int _maxPalavras;

        public MaxPalavrasAttribute(int numPalavras) : base ("(0) tem demasiadas palavras")
        {
            _maxPalavras = numPalavras;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value != null)
            {
                var valorStr = value.ToString().Split(new char[] { '.', '?', '!', ' ', ';', ':', ',', }, StringSplitOptions.RemoveEmptyEntries);

                if(valorStr.Length > _maxPalavras)
                {
                    var msgErro = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(msgErro);
                }
            }
            return ValidationResult.Success;
        }
        
    }
}