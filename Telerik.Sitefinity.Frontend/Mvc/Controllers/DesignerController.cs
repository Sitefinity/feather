﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;
using Telerik.Sitefinity.Frontend.Mvc.Models;
using Telerik.Sitefinity.Frontend.Mvc.StringResources;
using Telerik.Sitefinity.Modules;
using Telerik.Sitefinity.Modules.Forms;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.Mvc.Controllers
{
    /// <summary>
    /// This class contains logic for resolving the views of the MVC designer of a widget.
    /// </summary>
    [Localization(typeof(DesignerResources))]
    [RequestBackendUserAuthentication]
    [ControllerMetadataAttribute(IsTemplatableControl = false)]
    public class DesignerController : Controller
    {
        /// <summary>
        /// Returns the designer view which handles the property editing for a particular widget. 
        /// If there is custom designer for the particular widget it will be retrieved, otherwise it will fallback to the default designer.
        /// The default designer is located under <see cref="Telerik.Sitefinity.Frontend.Mvc.Views.Designer.Designer.cshtml"/>.
        /// </summary>
        /// <param name="widgetName">The name of the widget.</param>
        /// <param name="moduleName">The name of the widget dynamic module (if any). Default value is null.</param>
        public virtual ActionResult Master(string widgetName, string moduleName = null)
        {
            this.GetHttpContext().Items[SystemManager.IsBackendRequestKey] = true;

            var controlId = this.Request != null ? this.Request["controlId"] ?? Guid.Empty.ToString() : Guid.Empty.ToString();
            var mediaType = this.Request != null ? this.Request["mediaType"] ?? DesignMediaType.Page.ToString() : DesignMediaType.Page.ToString();

            this.ViewBag.ControlName = widgetName;
            this.ViewBag.ControlId = controlId;
            this.ViewBag.MediaType = mediaType;

            var model = this.GetModel(widgetName, Guid.Parse(controlId), moduleName, (DesignMediaType)Enum.Parse(typeof(DesignMediaType), mediaType));
            return this.View(DesignerController.DefaultView, model);
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
        public new virtual ActionResult View(string widgetName, string viewType)
        {
            this.GetHttpContext().Items[SystemManager.IsBackendRequestKey] = true;

            var viewName = DesignerController.DesignerViewTemplate.Arrange(viewType);
            var mediaType = this.Request != null ? this.Request["mediaType"] ?? DesignMediaType.Page.ToString() : DesignMediaType.Page.ToString();
            var designMediaType = (DesignMediaType)Enum.Parse(typeof(DesignMediaType), mediaType);

            var model = this.GetViewModel(designMediaType);

            // Passing the DesignerModel to the view model 	 	
            var controlIdParam = this.GetControlIdParam(); 	 	
            if (controlIdParam.HasValue) 	 	
            { 	 	
                ViewBag.DesignerModel = this.GetModel(widgetName, controlIdParam.Value, designMediaType: designMediaType); 	 	
            } 

            return this.PartialView(viewName, model);
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

        private Guid? GetControlIdParam() 	 	
        { 	 	
            if (this.Request == null) 	 	
                return null; 	 	
 	 	
            var controlIdParam = this.Request["controlId"]; 	 	
            if (controlIdParam == null) 	 	
                return null; 	 	
 	 	
            Guid controlIdParamAsGuid; 	 	
            if (!Guid.TryParse(controlIdParam.ToString(), out controlIdParamAsGuid)) 	 	
                return null; 	 	
 	 	
            return controlIdParamAsGuid; 	 	
        } 

        /// <summary>
        /// Gets the model of the designer.
        /// </summary>
        private IDesignerModel GetModel(string widgetName, Guid controlId, string moduleName = null, DesignMediaType designMediaType = DesignMediaType.Page)
        {
            var viewFilesMappgings = new Dictionary<string, string>();

            var constructorParameters = new Dictionary<string, object> 
            {
                { "views", this.GetPartialViews(ref viewFilesMappgings, moduleName) },
                { "viewLocations", this.GetPartialViewLocations() },
                { "widgetName", widgetName },
                { "controlId", controlId },
                { "preselectedView", this.Request != null ? this.Request["view"] : null },
                { "viewFilesMappings", viewFilesMappgings },
                { "mediaType", designMediaType }
            };

            return ControllerModelFactory.GetModel<IDesignerModel>(typeof(DesignerController), constructorParameters);
        }

        private Control GetViewModel(DesignMediaType designMediaType)
        {
            var controlIdParam = this.GetControlIdParam();

            if (!controlIdParam.HasValue)
                return null;

            var controlId = controlIdParam.Value;
            var manager = this.GetControlManager(designMediaType);
            var viewModel = manager.LoadControl(manager.GetControl<ObjectData>(controlId));

            var widgetProxy = viewModel as MvcWidgetProxy;
            if (widgetProxy != null)
            {
                if (widgetProxy.Controller.RouteData == null)
                    widgetProxy.Controller.ControllerContext = new ControllerContext(new RequestContext(this.HttpContext, new RouteData()), widgetProxy.Controller);

                widgetProxy.Controller.RouteData.Values["controller"] = widgetProxy.WidgetName;
            }

            return viewModel;
        }

        private IControlManager GetControlManager(DesignMediaType designMediaType)
        {
            if (designMediaType == DesignMediaType.Form)
            {
                return FormsManager.GetManager();
            } 
            else 
            {
                return PageManager.GetManager();
            }
        }

        private const string DefaultView = "Designer";
        private const string DesignerViewTemplate = "DesignerView.{0}";
    }
}
