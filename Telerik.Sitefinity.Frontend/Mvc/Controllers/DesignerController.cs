using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;
using Telerik.Sitefinity.Frontend.Mvc.Models;
using Telerik.Sitefinity.Frontend.Mvc.StringResources;

namespace Telerik.Sitefinity.Frontend.Mvc.Controllers
{
    /// <summary>
    /// This class contains logic for resolving the views of the MVC designer of a widget.
    /// </summary>
    [Localization(typeof(DesignerResources))]
    public class DesignerController : Controller
    {
        /// <summary>
        /// Returns the designer view which handles the property editing for a particular widget. 
        /// If there is custom designer for the particular widget it will be retrieved, otherwise it will fallback to the default designer.
        /// The default designer is located under <see cref="Telerik.Sitefinity.Frontend.Mvc.Views.Designer.Designer.cshtml"/>.
        /// </summary>
        /// <param name="widgetName">The name of the widget.</param>
        public virtual ActionResult Master(string widgetName)
        {
            FrontendManager.AuthenticationEvaluator.RequestBackendUserAuthentication();
            this.DisableViewLocationCache();

            this.ViewBag.ControlName = widgetName;

            var model = this.GetModel(widgetName);
            return this.View(DesignerController.defaultView, model);
        }

        /// <summary>
        /// Returns specific view associated with the widget designer. If such view is available for the particular widget it will be displayed,
        /// otherwise it will try to fallback to the default view with the specified type (located in Telerik.Sitefinity.Frontend.Mvc.Views.Designer),
        /// If default view for the specified view type is not available exception will be thrown.
        /// </summary>
        /// <param name="widgetName">The name of the widget.</param>
        /// <param name="viewType">Type of the view which is requested. For example Simple, Advanced</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">View cannot be found on the searched locations.</exception>
        public virtual ActionResult View(string widgetName, string viewType)
        {
            FrontendManager.AuthenticationEvaluator.RequestBackendUserAuthentication();
            this.DisableViewLocationCache();

            string viewName = DesignerController.designerViewTemplate.Arrange(viewType);
            return this.PartialView(viewName);
        }

        /// <summary>
        /// Returns a view containing client references for scripts and styles.
        /// </summary>
        public virtual ActionResult ClientReferences()
        {
            FrontendManager.AuthenticationEvaluator.RequestBackendUserAuthentication();
            this.DisableViewLocationCache();

            return this.View(DesignerController.clientReferencesView);
        }

        /// <summary>
        /// Gets the model of the designer.
        /// </summary>
        private IDesignerModel GetModel(string widgetName)
        {
            var constructorParameters = new Dictionary<string, object> 
                        {
                           {"views", this.GetPartialViews()}
                        };

            return ControllerModelFactory.GetModel<IDesignerModel>(typeof(DesignerController), constructorParameters);
        }

        private const string defaultView = "Designer";
        private const string designerViewTemplate = "DesignerView.{0}";
        private const string clientReferencesView = "Designer.ClientReferences";
    }
}
