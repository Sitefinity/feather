using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telerik.Sitefinity.Frontend.Mvc
{
    /// <summary>
    /// Generic controller metadata attribute
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
        /// Gets or sets a value indicating whether the controller supports templateble control
        /// </summary>
        public bool IsTemplatableControl { get; set; }
    }
}
