using System;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes
{
    /// <summary>
    /// This attribute is used to specify how <see cref="IController"/> views are localized.
    /// </summary>
    /// <remarks>Apply on controllers.</remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class LocalizationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationAttribute"/> class.
        /// </summary>
        /// <param name="resourceClass">The resource class.</param>
        public LocalizationAttribute(Type resourceClass)
        {
            this.ResourceClass = resourceClass;
        }

        /// <summary>
        /// Gets or sets the resource class.
        /// </summary>
        public Type ResourceClass { get; set; }
    }
}
