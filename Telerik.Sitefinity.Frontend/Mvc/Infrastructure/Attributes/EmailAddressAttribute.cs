using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Attributes
{
    /// <summary>
    /// This class represents <see cref="ValidationAttribute"/> and should be used when one needs to validate email property.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class EmailAddressAttribute : ValidationAttribute, IClientValidatable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAddressAttribute"/> class with a custom regex pattern matcher.
        /// </summary>
        public EmailAddressAttribute(string regexPattern)
        {
            this.regex = new Regex(regexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAddressAttribute"/> class the default regex pattern matcher.
        /// </summary>
        public EmailAddressAttribute()
            : this(EmailAddressAttribute.DefaultRegexPattern)
        {
        }

        /// <inheritDoc/>
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ValidationType = "email",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
            };
        }

        /// <inheritDoc/>
        public override bool IsValid(object value)
        {
            if (value != null)
            {
                var stringValue = value as string;
                if (!string.IsNullOrEmpty(stringValue))
                {
                    if (this.regex.Match(stringValue).Success)
                    {
                        return true;
                    }
                }
            }

            return false;            
        }

        /// <inheritDoc/>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return this.IsValid(value) ? ValidationResult.Success : new ValidationResult(this.ErrorMessage);
        }

        private const string DefaultRegexPattern = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$";
        private Regex regex;
    }
}
