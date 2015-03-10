using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes
{
    /// <summary>
    /// Controller attribute for determining whether the controller is templatable control
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes"), AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ControllerMetadataAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerMetadataAttribute"/> class.
        /// </summary>
        public ControllerMetadataAttribute()
        {
            this.IsTemplatableControl = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the controller is templatable control
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Templatable")]
        public bool IsTemplatableControl { get; set; }
    }
}
