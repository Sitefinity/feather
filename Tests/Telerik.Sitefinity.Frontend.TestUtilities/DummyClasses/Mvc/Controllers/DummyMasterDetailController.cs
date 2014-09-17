using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Taxonomies.Model;

namespace Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers
{
    /// <summary>
    /// This is a dummy controller that complies with the convention for a Master/Detail widget.
    /// </summary>
    public class DummyMasterDetailController : Controller
    {
        /// <summary>
        /// Gets the name of the provider.
        /// </summary>
        /// <value>
        /// The name of the provider.
        /// </value>
        public string ProviderName
        {
            get
            {
                return "OpenAccessDataProvider";
            }
        }

        /// <summary>
        /// Master action with paging.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>Master view result.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "page")]
        public ActionResult Index(int? page)
        {
            return null;
        }

        /// <summary>
        /// Details action.
        /// </summary>
        /// <param name="contentItem">The content item.</param>
        /// <returns>Details view result.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "contentItem")]
        public ActionResult Details(ContentItem contentItem)
        {
            return null;
        }

        /// <summary>
        /// Lists items filtered by taxon.
        /// </summary>
        /// <param name="taxonFilter">The taxon filter.</param>
        /// <param name="page">The page.</param>
        /// <returns>Master view filtered by taxon.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "taxonFilter")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "page")]
        public ActionResult ListByTaxon(ITaxon taxonFilter, int? page)
        {
            return null;
        }
    }
}
