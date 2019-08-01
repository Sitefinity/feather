using System;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing.Date
{
    /// <summary>
    /// Instances of this class map URL parameters to Route Data following a provided route template and action name.
    /// </summary>
    internal class DateUrlParamsMapper : UrlParamsMapperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateUrlParamsMapper" /> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="dateUrlEvaluatorAdapter">The adapter for date URL evaluator.</param>
        /// <param name="actionName">Name of the action.</param>
        public DateUrlParamsMapper(ControllerBase controller, IDateUrlEvaluatorAdapter dateUrlEvaluatorAdapter, string actionName = DateUrlParamsMapper.DefaultActionName) : base(controller)
        {
            this.actionName = actionName;
            this.actionMethod = controller.GetType().GetMethod(this.actionName, BindingFlags.Instance | BindingFlags.Public);
            this.dateUrlEvaluatorAdapter = dateUrlEvaluatorAdapter;
        }

        /// <inheritdoc />
        protected override bool TryMatchUrl(string[] urlParams, RequestContext requestContext, string urlKeyPrefix)
        {
            if (urlParams == null || urlParams.Length == 0)
            {
                return false;
            }

            string url = string.Join(@"/", urlParams);
            DateTime from = default(DateTime);
            DateTime to = default(DateTime);

            if (!this.dateUrlEvaluatorAdapter.TryGetDateFromUrl(url, urlKeyPrefix, out from, out to))
            {
                return false;
            }

            this.SetControllerActionParams(requestContext, from, to);

            RouteHelper.SetUrlParametersResolved();

            return true;
        }

        private void SetControllerActionParams(RequestContext requestContext, DateTime from, DateTime to)
        {
            requestContext.RouteData.Values[UrlParamsMapperBase.ActionNameKey] = this.actionName;
            ParameterInfo[] parameters = this.actionMethod.GetParameters();

            foreach (ParameterInfo paramInfo in this.actionMethod.GetParameters())
            {
                if (paramInfo.ParameterType == typeof(DateTime))
                {
                    if (string.Equals(paramInfo.Name, "from", StringComparison.InvariantCultureIgnoreCase))
                    {
                        requestContext.RouteData.Values[paramInfo.Name] = from;
                    }
                    else if (string.Equals(paramInfo.Name, "to", StringComparison.InvariantCultureIgnoreCase))
                    {
                        requestContext.RouteData.Values[paramInfo.Name] = to;
                    }
                }
            }
        }

        private readonly string actionName;
        private readonly MethodInfo actionMethod;
        private const string DefaultActionName = "ListByDate";
        private readonly IDateUrlEvaluatorAdapter dateUrlEvaluatorAdapter;
    }
}