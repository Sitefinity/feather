using System;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing.Date
{
    /// <summary>
    /// Adapter for <see cref="Telerik.Sitefinity.Web.UrlEvaluation.DateEvaluator"/>
    /// </summary>
    internal interface IDateUrlEvaluatorAdapter
    {
        /// <summary>
        /// Will try to get date filter from URL and save result in 'from' date and 'to' date parameters.
        /// </summary>
        /// <param name="url">The url path that contains the date filter.</param>
        /// <param name="urlKeyPrefix">The url prefix that might be configured for the URL path expression.</param>
        /// <param name="from">The 'from' date in the filter.</param>
        /// <param name="to">The 'to' date in the filter.</param>
        /// <returns>True if date filter was successfully parsed, otherwise false.</returns>
        bool TryGetDateFromUrl(string url, string urlKeyPrefix, out DateTime from, out DateTime to);
    }
}