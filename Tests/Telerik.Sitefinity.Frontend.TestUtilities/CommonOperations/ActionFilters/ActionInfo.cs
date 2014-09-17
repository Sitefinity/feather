using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations.ActionFilters
{
    /// <summary>
    /// This class represents DTO for containing action information needed for testing purposes.
    /// </summary>
    public class ActionInfo
    {
        /// <summary>
        /// Gets or sets the name of the action.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the action route data.
        /// </summary>
        /// <value>
        /// The action route data.
        /// </value>
        public RouteData ActionRouteData { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public ActionResult Result { get; set; }

        /// <summary>
        /// Gets or sets the current HTTP context.
        /// </summary>
        /// <value>
        /// The current HTTP context.
        /// </value>
        public HttpContextBase CurrentHttpContext { get; set; }
    }
}
