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

        /// <summary>
        /// The MVC template condition
        /// </summary>
        /// <remarks>
        /// {Module type full name} AND MVC;
        /// </remarks>
        public static readonly string MvcTemplateCondition = "{0} AND MVC";

        /// <summary>
        /// The friendly control name template for list.
        /// </summary>
        /// <remarks>
        /// {Module title} - {ModuleType plural name} - list (MVC);
        /// </remarks>
        public static readonly string FriendlyControlDynamicListTemplate = "{0} - {1} - list (MVC)";

        /// <summary>
        /// The friendly control name template for detail.
        /// </summary>
        /// <remarks>
        /// {Module title} - {ModuleType plural name} - single (MVC);
        /// </remarks>
        public static readonly string FriendlyControlDynamicDetailTemplate = "{0} - {1} - single (MVC)";

        /// <summary>
        /// The detail template name
        /// </summary>
        public static readonly string DetailTemplateName = "Detail.{0}";

        /// <summary>
        /// The list template name
        /// </summary>
        public static readonly string ListTemplateName = "List.{0}";

        /// <summary>
        /// The area format.
        /// </summary>
        /// <remarks>
        /// {Module title} - {ModuleType display name};
        /// </remarks>
        public static readonly string DynamicAreaFormat = "{0} - {1}";
    }
}
