using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure
{
    /// <summary>
    /// This class contains commonly used MVC words, phrases and extensions.
    /// </summary>
    public static class MvcConstants
    {
        /// <summary>
        /// Initialize dependant static variables.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static MvcConstants()
        {
            MvcConstants.MvcFieldControlNameTemplate = string.Concat("{0} ", MvcConstants.MvcSuffix);
        }

        /// <summary>Template for <see cref="ControlPresentation"/>'s fields. Used by MVC widgets.</summary>
        public static readonly string MvcFieldControlNameTemplate;

        /// <summary>Suffix for MVC widgets friendly control name</summary>
        public static readonly string MvcSuffix = "(MVC)";

        /// <summary>Filename extension used by Razor views</summary>
        public static readonly string RazorFileNameExtension = ".cshtml";
    }
}
