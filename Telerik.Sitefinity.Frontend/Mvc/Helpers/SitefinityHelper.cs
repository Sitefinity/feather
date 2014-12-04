using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// This class contains basic helper method used in Feather.
    /// </summary>
    public static class SitefinityHelper
    {
        /// <summary>
        /// Wrapper helper for Sitefinity.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <returns></returns>
        public static HtmlHelper Sitefinity(this HtmlHelper helper)
        {
            return helper;
        }
    }
}
