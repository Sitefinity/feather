using System;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Web.UrlEvaluation;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing.Date
{
    /// <inheritdoc />
    internal class DateUrlEvaluatorAdapter : IDateUrlEvaluatorAdapter
    {
        /// <summary>
        /// Initializes a new instance for <see cref="DateUrlEvaluatorAdapter"/>.
        /// </summary>
        public DateUrlEvaluatorAdapter()
        {
            this.dateEvaluator = this.GetDefaultEvaluator();
        }

        /// <inheritdoc />
        public bool TryGetDateFromUrl(string url, string urlKeyPrefix, out DateTime from, out DateTime to)
        {
            from = default(DateTime);
            to = default(DateTime);

            int datePrefixIndex = url.IndexOf(DatePrefix, StringComparison.InvariantCultureIgnoreCase);
            if (datePrefixIndex == -1)
            {
                return false;
            }

            url = url.Substring(datePrefixIndex + DatePrefix.Length);
            object[] values;

            string dateExpression = this.dateEvaluator.Evaluate(url, "PublicationDate", null, UrlEvaluationMode.UrlPath, null, out values);
            if (string.IsNullOrWhiteSpace(dateExpression))
            {
                return false;
            }

            from = (DateTime)values[0];
            to = (DateTime)values[1];

            return true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private DateEvaluator GetDefaultEvaluator()
        {
            try
            {
                return UrlEvaluator.GetEvaluator("Date") as DateEvaluator;
            }
            catch
            {
                return null;
            }
        }

        private const string DatePrefix = "archive";
        private readonly DateEvaluator dateEvaluator;
    }
}