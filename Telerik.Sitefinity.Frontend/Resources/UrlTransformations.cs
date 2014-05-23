using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Telerik.Sitefinity.Frontend.Resources
{
    /// <summary>
    /// This is a static class that contains methods for URL manipulations.
    /// </summary>
    public static class UrlTransformations
    {
        /// <summary>
        /// Appends a parameter to a give url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="paramaterName">Name of the paramater.</param>
        /// <param name="paramterValue">The paramter value.</param>
        /// <returns></returns>
        public static string AppendParam(string url, string paramaterName, string paramterValue)
        {
            if (paramterValue.IsNullOrEmpty())
            {
                return url;
            }
            else if (url.Contains("?"))
            {
                return string.Format("{0}&{1}={2}", url, paramaterName, HttpUtility.UrlEncode(paramterValue));
            }
            else
            {
                return string.Format("{0}?{1}={2}", url, paramaterName, HttpUtility.UrlEncode(paramterValue));
            }
        }
    }
}
