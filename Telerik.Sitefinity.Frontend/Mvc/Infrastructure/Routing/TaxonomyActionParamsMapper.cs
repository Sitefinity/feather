using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Web.UrlEvaluation;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Instances of this class map URL parameters to Route Data following a provided route template and action name.
    /// </summary>
    internal class TaxonomyActionParamsMapper : CustomActionParamsMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaxonomyActionParamsMapper"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="routeTemplateResolver">This function should return the route template that the mapper will use.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <exception cref="System.ArgumentNullException">routeTemplateResolver</exception>
        public TaxonomyActionParamsMapper(ControllerBase controller, Func<string> routeTemplateResolver, string actionName)
            : base(controller, routeTemplateResolver, actionName)
        {
        }

        /// <inheritdoc />
        protected override IList<UrlSegmentInfo> MapParams(MethodInfo actionMethod, string[] metaParams, string[] urlParams)
        {
            if (metaParams.Length > urlParams.Length)
                return null;

            string url = string.Join(@"/", urlParams);

            ITaxon taxon;

            if (!this.TryGetTaxonFromUrl(url, out taxon))
            {
                return base.MapParams(actionMethod, metaParams, urlParams);
            }

            var parameterInfos = new List<UrlSegmentInfo>();

            for (int i = 0; i < metaParams.Length; i++)
            {
                if (metaParams[i].Length > 2 && metaParams[i].First() == '{' && metaParams[i].Last() == '}')
                {
                    var routeParam = metaParams[i].Sub(1, metaParams[i].Length - 2);

                    var parts = routeParam.Split(':');
                    var paramName = parts[0];

                    if (parts.Length >= 2)
                    {
                        parameterInfos.Add(new UrlSegmentInfo(paramName, taxon.Taxonomy.TaxonName, taxon));
                    }
                    else
                    {
                        parameterInfos.Add(new UrlSegmentInfo
                        {
                            ParameterName = paramName,
                            ParameterValue = urlParams[i],
                        });
                    }
                }
            }

            return parameterInfos;
        }

        /// <summary>
        /// Tries the get taxon from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="taxon">The taxon.</param>
        /// <returns></returns>
        private bool TryGetTaxonFromUrl(string url, out ITaxon taxon)
        {
            taxon = null;

            var evaluator = UrlEvaluator.GetEvaluator("Taxonomy") as TaxonomyEvaluator;

            string taxonomyName;
            string taxonName;

            evaluator.ParseTaxonomyParams(UrlEvaluationMode.UrlPath, url, null, out taxonName, out taxonomyName);

            if (!string.IsNullOrEmpty(taxonName) && !string.IsNullOrEmpty(taxonomyName))
            {
                taxon = evaluator.GetTaxonByName(taxonomyName, taxonName);
            }

            return taxon != null;
        }
    }
}