using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Pages.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// This class contains extension methods for the IPageTemplate.
    /// </summary>
    internal static class PageTemplateExtensions
    {
        /// <summary>
        /// Gets the template framework.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns></returns>
        public static PageTemplateFramework GetTemplateFramework(this IPageTemplate template)
        {
            if (template is IFrameworkSpecificPageTemplate)
            {
                return (template as IFrameworkSpecificPageTemplate).Framework;
            }

            return PageTemplateFramework.Hybrid;
        }
    }
}
