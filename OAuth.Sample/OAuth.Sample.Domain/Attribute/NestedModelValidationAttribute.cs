using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OAuth.Sample.Domain.Attribute
{
    /// <summary>
    /// 巢狀Model 驗證
    /// </summary>
    public class NestedModelValidationAttribute : ValidationAttribute
    {
        public NestedModelValidationAttribute() { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // 不做必填驗證
            if (value == null) return ValidationResult.Success;

            var validationResultList = new List<ValidationResult>();
            var result = Validator.TryValidateObject(value, new ValidationContext(value), validationResultList);

            if (validationResultList.Count == 0)
            {
                // valid
                return ValidationResult.Success;
            }
            else
            {
                // invalid
                var errorMsg = string.Join(",", validationResultList.Select(x => x.ErrorMessage));
                return new ValidationResult(errorMsg);
            }
        }

    }
}

