using System;
using System.Web;

namespace Telerik.Sitefinity.Frontend.Resources
{
    /// <summary>
    /// This is a static class that contains methods for URL manipulations.
    /// </summary>
    internal static class UrlTransformations
    {
        /// <summary>
        /// Appends a parameter to a give url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="parameterName">Name of the paramater.</param>
        /// <param name="parameterValue">The paramter value.</param>
        /// <returns></returns>
        public static string AppendParam(string url, string parameterName, string parameterValue)
        {
            if (parameterValue.IsNullOrEmpty())
                return url;

            if (url.Contains("?"))
                return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}&{1}={2}", url, parameterName, HttpUtility.UrlEncode(parameterValue));

            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}?{1}={2}", url, parameterName, HttpUtility.UrlEncode(parameterValue));
        }
    }
}
