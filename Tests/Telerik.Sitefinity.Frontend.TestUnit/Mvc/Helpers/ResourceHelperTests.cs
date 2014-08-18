using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.Mvc.Helpers;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.HttpContext;

namespace Telerik.Sitefinity.Frontend.TestUnit.Mvc.Helpers
{
    /// <summary>
    /// This class contains tests methods for the <see cref="ResourceHelper" />
    /// </summary>
    [TestClass]
    public class ResourceHelperTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The register script_ two times_ exception is thrown.
        /// </summary>
        [TestMethod]
        [Owner("Tihomir Petrov")]
        [Description("Ensures that exception is thrown when there is attempt for registering the same javascript twice.")]
        [ExpectedException(typeof(ArgumentException), "ArgumentException was not thrown in the case when one tries to register a javascript twice.")]
        public void RegisterScript_TwoTimes_ExceptionIsThrown()
        {
            var dummyHttpContext = new DummyHttpContext();
            var dummyViewContext = new ViewContext();
            dummyViewContext.HttpContext = dummyHttpContext;
            var dummyViewDataContainer = (IViewDataContainer)new ViewPage();
            var script = "Mvc/Scripts/Designer/modal-dialog.js";

            var htmlHelper = new System.Web.Mvc.HtmlHelper(dummyViewContext, dummyViewDataContainer);

            var result = htmlHelper.Script(script, true);
            Assert.AreEqual(result.ToString(), string.Format("<script src=\"{0}\" type=\"text/javascript\"></script>", script));

            result = htmlHelper.Script(script, true);
        }

        /// <summary>
        /// The register script_ two times_ no duplicate registrations.
        /// </summary>
        [TestMethod]
        [Owner("Tihomir Petrov")]
        [Description("Ensures that a javascript is registered only once even if the Html.Script method is invoked multiple times.")]
        public void RegisterScript_TwoTimes_NoDuplicateRegistrations()
        {
            var dummyHttpContext = new DummyHttpContext();
            var dummyViewContext = new ViewContext();
            dummyViewContext.HttpContext = dummyHttpContext;
            var dummyViewDataContainer = (IViewDataContainer)new ViewPage();
            var script = "Mvc/Scripts/Designer/modal-dialog.js";

            var htmlHelper = new System.Web.Mvc.HtmlHelper(dummyViewContext, dummyViewDataContainer);

            MvcHtmlString result = htmlHelper.Script(script);
            Assert.AreEqual(result.ToString(), string.Format("<script src=\"{0}\" type=\"text/javascript\"></script>", script));

            result = htmlHelper.Script(script);
            Assert.AreEqual(result, MvcHtmlString.Empty);
        }

        #endregion
    }
}