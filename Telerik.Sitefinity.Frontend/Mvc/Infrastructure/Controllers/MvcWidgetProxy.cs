using System;
using System.ComponentModel;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc.Proxy;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers
{
    /// <summary>
    /// Web forms control that is used as a proxy for MVC controllers and has information about the specific widget it is used for.
    /// </summary>
    public class MvcWidgetProxy : MvcControllerProxy
    {
        /// <summary>
        /// Gets or sets the name of the widget.
        /// </summary>
        /// <value>The name of the widget.</value>
        [Browsable(true)]
        public string WidgetName 
        {
            get
            {
                return this.widgetName;
            }

            set
            {
                this.widgetName = value;
                UpdateControllerViewBag(this.Controller);
            }
        }

        /// <summary>
        /// Gets or sets the name of the module.
        /// </summary>
        /// <value>The name of the module.</value>
        [Browsable(true)]
        public string ModuleName
        {
            get
            {
                return this.moduleName;
            }

            set
            {
                this.moduleName = value;
                UpdateControllerViewBag(this.Controller);
            }
        }

        /// <inheritdoc />
        protected override void OnControllerInitialized(Controller controller)
        {
            UpdateControllerViewBag(controller);
        }

        private string widgetName;
        private string moduleName;

        private void UpdateControllerViewBag(Controller controller)
        {
            if (controller == null || controller.ViewBag == null) return;

            if (!this.WidgetName.IsNullOrEmpty())
            {
                controller.ViewBag.WidgetName = this.WidgetName;
            }

            if (!this.ModuleName.IsNullOrEmpty())
            {
                controller.ViewBag.ModuleName = this.ModuleName;
            }            
        }
    }
}
