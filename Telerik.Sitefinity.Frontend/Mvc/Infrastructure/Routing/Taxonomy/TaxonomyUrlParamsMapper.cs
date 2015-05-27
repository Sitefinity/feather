using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Web;
using Telerik.Sitefinity.Web.UrlEvaluation;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Instances of this class map URL parameters to Route Data following a provided route template and action name.
    /// </summary>
    internal class TaxonomyUrlParamsMapper : UrlParamsMapperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaxonomyUrlParamsMapper" /> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="taxonUrlEvaluator">The taxon URL evaluator.</param>
        /// <param name="actionName">Name of the action.</param>
        public TaxonomyUrlParamsMapper(ControllerBase controller, TaxonUrlMapper taxonUrlEvaluator, string actionName = TaxonomyUrlParamsMapper.DefaultActionName) : base(controller)
        {
            this.actionName = actionName;
            this.taxonUrlEvaluator = taxonUrlEvaluator;
            this.actionMethod = controller.GetType().GetMethod(this.actionName, BindingFlags.Instance | BindingFlags.Public);
        }

        /// <inheritdoc />
        protected override bool TryMatchUrl(string[] urlParams, RequestContext requestContext)
        {
            ITaxon taxon;
            int pageIndex;

            if (!this.taxonUrlEvaluator.TryMatch(urlParams, out taxon, out pageIndex))
            {
                return false;
            }

            this.SetControllerActionParams(requestContext, taxon, pageIndex);

            RouteHelper.SetUrlParametersResolved();

            return true;
        }

        private void SetControllerActionParams(RequestContext requestContext, ITaxon taxon, int pageIndex)
        {
            requestContext.RouteData.Values[UrlParamsMapperBase.ActionNameKey] = this.actionName;

            foreach (ParameterInfo paramInfo in this.actionMethod.GetParameters())
            {
                if (paramInfo.ParameterType == typeof(ITaxon))
                {
                    requestContext.RouteData.Values[paramInfo.Name] = taxon;
                }
                else if (paramInfo.ParameterType == typeof(int?) && pageIndex > 0)
                {
                    requestContext.RouteData.Values[paramInfo.Name] = pageIndex;
                }
            }
        }

        private readonly string actionName;
        private readonly MethodInfo actionMethod;
        private const string DefaultActionName = "ListByTaxon";
        private readonly TaxonUrlMapper taxonUrlEvaluator;
    }
}