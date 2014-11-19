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
        [Browsable(true)]
        public string WidgetName { get; set; }
    }
}
