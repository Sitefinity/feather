using System;
using System.Linq;
using Telerik.Sitefinity.Frontend.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.HttpContext;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Controllers
{
    public class DummyDesignerController : DesignerController
    {
        protected override System.Web.HttpContextBase GetHttpContext()
        {
            return new DummyHttpContext();
        }
    }
}
