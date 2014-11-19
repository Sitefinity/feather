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
                if (this.Controller != null && this.Controller.ViewBag != null)
                    this.Controller.ViewBag.WidgetName = this.widgetName;
            }
        }

        /// <summary>
        /// Gets or sets the instance of <see cref="SitefinityController"/> that
        /// is being proxied.
        /// </summary>
        public override Controller Controller
        {
            get
            {
                var controller = base.Controller;
                if (controller != null && controller.ViewBag != null && !this.WidgetName.IsNullOrEmpty())
                    controller.ViewBag.WidgetName = this.WidgetName;

                return controller;
            }
        }

        private string widgetName;
    }
}
