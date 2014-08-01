using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.WebPages.Html;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.HttpContext;
using Telerik.Sitefinity.Frontend.Mvc.Helpers;

namespace Telerik.Sitefinity.Frontend.TestUnit.Mvc.Helpers
{
    /// <summary>
    /// This class contains tests methods for the <see cref="ResourceHelper"/>
    /// </summary>
    [TestClass]
    public class ResourceHelperTests
    {
        [TestMethod]
        [Owner("Tihomir Petrov")]
        [Description("Ensures that a javascript is registered only once even if the Html.Script method is invoked multiple times.")]
        public void RegisterScript_TwoTimes_NoDuplicateRegistrations()
        {
            var dummyHttpContext = new DummyHttpContext();
            var dummyViewContext = new ViewContext();
            dummyViewContext.HttpContext = dummyHttpContext;
            var dummyViewDataContainer = (IViewDataContainer)new System.Web.Mvc.ViewPage();
            var script = "Mvc/Scripts/Designer/modal-dialog.js";

            var htmlHelper = new System.Web.Mvc.HtmlHelper(dummyViewContext, dummyViewDataContainer);
            
            var result = ResourceHelper.Script(htmlHelper, script);
            Assert.AreEqual(result.ToString(), string.Format("<script src=\"{0}\" type=\"text/javascript\"></script>", script));

            result = ResourceHelper.Script(htmlHelper, script);
            Assert.AreEqual(result, MvcHtmlString.Empty);
        }

        [TestMethod]
        [Owner("Tihomir Petrov")]
        [Description("Ensures that exception is thrown when there is attempt for registering the same javascript twice.")]
        [ExpectedException(typeof(ArgumentException), "ArgumentException was not thrown in the case when one tries to register a javascript twice.")]
        public void RegisterScript_TwoTimes_ExceptionIsThrown()
        {
            var dummyHttpContext = new DummyHttpContext();
            var dummyViewContext = new ViewContext();
            dummyViewContext.HttpContext = dummyHttpContext;
            var dummyViewDataContainer = (IViewDataContainer)new System.Web.Mvc.ViewPage();
            var script = "Mvc/Scripts/Designer/modal-dialog.js";

            var htmlHelper = new System.Web.Mvc.HtmlHelper(dummyViewContext, dummyViewDataContainer);

            var result = ResourceHelper.Script(htmlHelper, script, true);
            Assert.AreEqual(result.ToString(), string.Format("<script src=\"{0}\" type=\"text/javascript\"></script>", script));

            result = ResourceHelper.Script(htmlHelper, script, true);
        }
    }
}
