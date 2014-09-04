using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Pages.Model;

namespace Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations
{
    /// <summary>
    /// Provides common pages operations
    /// </summary>
    public class PagesOperations
    {
        /// <summary>
        /// Creates a page with template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="pageTitle">The page title.</param>
        /// <param name="pageUrlName">Name of the page URL.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#")]
        public Guid CreatePageWithTemplate(PageTemplate template, string pageTitle, string pageUrlName)
        {
            Guid pageId = Guid.Empty;
            App.WorkWith()
               .Page()
               .CreateNewStandardPage()
               .Do(p =>
               {
                   p.GetPageData().Template = template;
                   p.Title = pageTitle;
                   p.UrlName = pageUrlName;
                   pageId = p.Id;
               })
               .CheckOut()
               .Publish(CultureInfo.InvariantCulture)
               .SaveChanges();
            return pageId;
        }
    }
}
