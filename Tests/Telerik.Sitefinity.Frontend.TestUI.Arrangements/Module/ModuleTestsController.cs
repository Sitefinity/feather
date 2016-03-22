using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Mvc;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements.Module
{
    [ControllerToolboxItem(Name = WidgetName, Title = WidgetName, SectionName = WidgetName)]
    [Localization(typeof(ModuleTestsResources))]
    public class ModuleTestsController : Controller
    {
        public string ResourceProperty
        {
            get { return Res.Get<ModuleTestsResources>(ModuleTestsResources.ResourceKey); }
        }

        public ActionResult Index()
        {
            return this.Content(this.ResourceProperty);
        }

        public const string WidgetName = "ModuleTestsWidget";
    }
}