using System.Web.UI;
using Telerik.Sitefinity.Web.UI;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers
{
    /// <summary>
    /// Extended control behavior class that is aware of the MvcWidgetProxy class.
    /// </summary>
    public class FrontendControlBehaviorResolver : ControlBehaviorResolver
    {
        /// <inheritdoc />
        public override object GetBehaviorObject(Control control)
        {
            var baseResult = base.GetBehaviorObject(control);
            var widgetProxy = control as MvcWidgetProxy;
            if (widgetProxy != null)
            {
                widgetProxy.Controller.ViewBag.WidgetName = widgetProxy.WidgetName;
            }

            return baseResult;
        }
    }
}
