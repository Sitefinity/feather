using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Frontend.Mvc.Helpers;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Mvc.Proxy;
using Telerik.Sitefinity.Web.UI.ControlDesign;

namespace Telerik.Sitefinity.Frontend.Designers
{
    /// <summary>
    /// This class contains logic for resolving the designer responsible for the property editing of a widget.
    /// </summary>
    internal class DesignerResolver : IDesignerResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DesignerResolver"/> class.
        /// </summary>
        /// <param name="packageManager">An instance of a package manager.</param>
        public DesignerResolver(PackageManager packageManager = null)
        {
            this.packageManager = packageManager ?? new PackageManager();
        }

        #region Public members

        /// <summary>
        /// Gets the widget designer URL based on the widget type. 
        /// If there is a record in the <see cref="DesignerResolver.Registry"/> for this widget type it would be retrieved with biggest priority.
        /// Otherwise the URL specified by <see cref="Telerik.Sitefinity.Frontend.Designer.DesignerUrlAttribute"/> will be retrieved.
        /// If the URL is not specified explicitly for a MVC widget this method will retrieve the default designer <see cref="Telerik.Sitefinity.Frontend.Mvc.Controllers.DesignerController"/>.
        /// If null then the default property editor URL should be used.
        /// </summary>
        /// <param name="widgetType">Type of the widget.</param>
        /// <exception cref="ArgumentNullException">widgetType</exception>
        public string GetUrl(Type widgetType)
        {
            if (widgetType == null)
                throw new ArgumentNullException("widgetType");

            if (this.HasCustomWebFormsDesigner(widgetType))
                return null;

            string designerUrl;
            if (!this.TryResolveUrlFromAttribute(widgetType, out designerUrl))
                designerUrl = this.GetDefaultUrl(widgetType);

            return this.packageManager.EnhanceUrl(designerUrl);
        }

        /// <summary>
        /// Checks if there are separate custom desiger views for the particular control type.
        /// </summary>
        /// <param name="controlType">The type of the control.</param>
        /// <returns></returns>
        public bool HasCustomDesigners(string controlType)
        {
            var mvcProxy = new MvcControllerProxy();
            mvcProxy.ControllerName = controlType;
            var controller = mvcProxy.GetController() as Controller;

            if (controller == null)
                return true;

            var designerNamePattern = "DesignerView.*";
            var views = controller.GetViews(null).Where(view => Regex.IsMatch(view, designerNamePattern));

            return views.Count() > 0;
        }

        /// <summary>
        /// Gets all view names that match a pattern.
        /// </summary>
        /// <param name="controlType">The control type.</param>
        /// <param name="viewNamePattern">The view name pattern</param>
        /// <returns></returns>
        public IEnumerable<string> GetViewNames(string controlType, string viewNamePattern)
        {
            var mvcProxy = new MvcControllerProxy();
            mvcProxy.ControllerName = controlType;
            var controller = mvcProxy.GetController() as Controller;

            if (controller == null)
                return new List<string>();

            return ViewSelectorHelpers.GetViewNames(null, controller, viewNamePattern);
        }

        #endregion

        #region Private members

        private bool HasCustomWebFormsDesigner(Type widgetType)
        {
            var attributes = widgetType.GetCustomAttributes(typeof(ControlDesignerAttribute), inherit: true);
            var designerAttr = attributes.FirstOrDefault() as ControlDesignerAttribute;

            if (designerAttr != null)
                return true;

            return false;
        }

        /// <summary>
        /// Resolve a designer URL for the specified widget type if such is specified with <see cref="DesignerUrlAttribute"/>.
        /// </summary>
        /// <param name="widgetType">Type of the widget.</param>
        /// <param name="designerUrl">The designer URL.</param>
        /// <returns></returns>
        private bool TryResolveUrlFromAttribute(Type widgetType, out string designerUrl)
        {
            bool designerAttrExists = false;
            designerUrl = null;
            var attributes = widgetType.GetCustomAttributes(typeof(DesignerUrlAttribute), inherit: true);
            var designerAttr = attributes.FirstOrDefault() as DesignerUrlAttribute;

            if (designerAttr != null)
            {
                designerUrl = designerAttr.Url;
                designerAttrExists = true;
            }

            return designerAttrExists;
        }

        /// <summary>
        /// Gets the default designer URL.
        /// </summary>
        /// <param name="widgetType">Type of the widget.</param>
        /// <returns></returns>
        private string GetDefaultUrl(Type widgetType)
        {
            string designerUrl = null;

            if (typeof(GridSystem.GridControl).IsAssignableFrom(widgetType))
                return DesignerResolver.DefaultGridActionUrlTemplate;

            var controllerRegistry = FrontendManager.ControllerFactory;
            bool isController = controllerRegistry.IsController(widgetType);

            if (isController)
            {
                string controllerName = controllerRegistry.GetControllerName(widgetType);
                designerUrl = string.Format(System.Globalization.CultureInfo.InvariantCulture, DesignerResolver.DefaultActionUrlTemplate, controllerName);
            }

            return designerUrl;
        }

        #endregion

        #region Constants

        private const string DefaultActionUrlTemplate = "~/Telerik.Sitefinity.Frontend/Designer/Master/{0}";
        private const string DefaultGridActionUrlTemplate = "~/Telerik.Sitefinity.Frontend/GridDesigner/Master/GridDesigner";
        private readonly PackageManager packageManager;

        public object ViewSelectorHelper { get; private set; }

        #endregion
    }
}
