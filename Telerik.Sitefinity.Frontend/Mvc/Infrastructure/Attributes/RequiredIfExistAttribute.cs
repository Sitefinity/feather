using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Attributes
{
    /// <summary>
    /// This class represents <see cref="ValidationAttribute"/> and should be used when one needs to validate property only if another property has value.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes")]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RequiredIfExistAttribute : System.ComponentModel.DataAnnotations.ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredIfExistAttribute"/> class.
        /// </summary>
        public RequiredIfExistAttribute(string dependentPropertyName)
            : base()
        {
            this.innerAttribute = new RequiredAttribute();
            this.dependentPropertyName = dependentPropertyName;
        }

        /// <inheritDoc/>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo dependentProperty = validationContext.ObjectInstance.GetType().GetProperty(this.dependentPropertyName);
            
            if (dependentProperty != null)
            {
                var dependentPropertyValue = dependentProperty.GetValue(validationContext.ObjectInstance, null);

                if (dependentPropertyValue != null)
                {
                    if (!this.innerAttribute.IsValid(value))
                        return new ValidationResult(this.ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
                        
        private string dependentPropertyName;
        private RequiredAttribute innerAttribute;
    }
}
