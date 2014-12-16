using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;

namespace Telerik.Sitefinity.Frontend.Mvc.Controllers
{
    // Just a generic controller for the view resolving purposes
    [ControllerMetadataAttribute(IsTemplatableControl = false)]
    internal class GenericController : Controller
    {
    }
}
