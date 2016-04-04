using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;
using Telerik.Sitefinity.Frontend.Mvc.Models;
using Telerik.Sitefinity.Frontend.Mvc.StringResources;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.Mvc.Controllers
{
    /// <summary>
    /// This class contains logic for resolving the views of the MVC designer of a grid.
    /// </summary>
    [Localization(typeof(GridDesignerResources))]
    [RequestBackendUserAuthentication]
    public class GridDesignerController : Controller
    {
        /// <summary>
        /// Returns the designer view which handles the property editing for a particular grid. 
        /// If there is custom designer for the particular widget it will be retrieved, otherwise it will fallback to the default designer.
        /// The default designer is located under <see cref="Telerik.Sitefinity.Frontend.Mvc.Views.GridDesigner.Designer.cshtml"/>.
        /// </summary>
        /// <param name="widgetName">The name of the widget.</param>
        public virtual ActionResult Master(string widgetName)
        {
            this.GetHttpContext().Items[SystemManager.IsBackendRequestKey] = true;

            var controlId = this.Request != null ? this.Request["controlId"] ?? Guid.Empty.ToString() : Guid.Empty.ToString();
            var gridTitle = this.Request != null ? this.Request["gridTitle"] ?? Guid.Empty.ToString() : Guid.Empty.ToString();

            var model = this.GetModel(gridTitle, controlId);

            return this.View(GridDesignerController.DefaultView, model);
        }

        /// <summary>
        /// Gets the HTTP context.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected virtual HttpContextBase GetHttpContext()
        {
            return this.HttpContext;
        }

        /// <summary>
        /// Gets the model of the designer.
        /// </summary>
        private IGridDesignerModel GetModel(string gridTitle, string controlId)
        {
            var constructorParameters = new Dictionary<string, object> 
                        {
                           { "gridTitle", gridTitle },
                           { "controlId", controlId }
                        };

            return ControllerModelFactory.GetModel<IGridDesignerModel>(typeof(GridDesignerController), constructorParameters);
        }

        private const string DefaultView = "Designer";
    }
}
