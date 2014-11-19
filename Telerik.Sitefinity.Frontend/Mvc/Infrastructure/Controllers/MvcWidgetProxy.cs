using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Mvc.Proxy;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers
{
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
                this.Controller.ViewBag.WidgetName = this.widgetName;
            }
        }

        private string widgetName;
    }
}
