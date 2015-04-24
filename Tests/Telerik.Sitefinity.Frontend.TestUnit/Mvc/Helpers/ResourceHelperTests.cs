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
        #region Scripts Tests

        /// <summary>
        /// The register script_ two times_ exception is thrown.
        /// </summary>
        [TestMethod]
        [Ignore]
        [Owner("Tihomir Petrov")]
        [Description("Ensures that exception is thrown when there is attempt for registering the same javascript twice.")]
        [ExpectedException(typeof(ArgumentException), "ArgumentException was not thrown in the case when one tries to register a javascript twice.")]
        public void RegisterScript_TwoTimes_ExceptionIsThrown()
        {
            var dummyHttpContext = new DummyHttpContext();
            var dummyViewContext = new ViewContext();
            dummyViewContext.HttpContext = dummyHttpContext;
            var dummyViewDataContainer = (IViewDataContainer)new ViewPage();
            var htmlHelper = new System.Web.Mvc.HtmlHelper(dummyViewContext, dummyViewDataContainer);

            var script = "Mvc/Scripts/Designer/modal-dialog.js";

            string expected = string.Format(System.Globalization.CultureInfo.InvariantCulture, "<script src=\"{0}\" type=\"text/javascript\"></script>", script);
            string result = htmlHelper.Script(script, null, throwException: true).ToString();
            Assert.AreEqual(expected, result);

            htmlHelper.Script(script, null, throwException: true);
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

            string expected1 = string.Format(System.Globalization.CultureInfo.InvariantCulture, "<script src=\"{0}\" type=\"text/javascript\"></script>", script);
            string result1 = htmlHelper.Script(script).ToString();
            Assert.AreEqual(expected1, result1);

            MvcHtmlString expected2 = MvcHtmlString.Empty;
            MvcHtmlString result2 = htmlHelper.Script(script);
            Assert.AreEqual(expected2, result2);
        }

        #endregion

        #region Stylesheets Tests

        /// <summary>
        /// The register stylesheet_ two times_ exception is thrown.
        /// </summary>
        [TestMethod]
        [Ignore]
        [Owner("Dzhenko Penev")]
        [Description("Ensures that exception is thrown when there is attempt for registering the same stylesheet twice.")]
        [ExpectedException(typeof(ArgumentException), "ArgumentException was not thrown in the case when one tries to register a stylesheet twice.")]
        public void RegisterStylesheet_TwoTimes_ExceptionIsThrown()
        {
            var dummyHttpContext = new DummyHttpContext();
            var dummyViewContext = new ViewContext();
            dummyViewContext.HttpContext = dummyHttpContext;
            var dummyViewDataContainer = (IViewDataContainer)new ViewPage();
            var htmlHelper = new System.Web.Mvc.HtmlHelper(dummyViewContext, dummyViewDataContainer);

            var stylesheet = "Mvc/Styles/Designer/modal-dialog.css";

            string expected = string.Format(System.Globalization.CultureInfo.InvariantCulture, "<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />", stylesheet);
            string result = htmlHelper.StyleSheet(stylesheet, null, throwException: true).ToString();
            Assert.AreEqual(expected, result);

            htmlHelper.StyleSheet(stylesheet, null, throwException: true);
        }

        /// <summary>
        /// The register stylesheet_ two times_ no duplicate registrations.
        /// </summary>
        [TestMethod]
        [Owner("Dzhenko Penev")]
        [Description("Ensures that a stylesheet is registered only once even if the Html.StyleSheet method is invoked multiple times.")]
        public void RegisterStylesheet_TwoTimes_NoDuplicateRegistrations()
        {
            var dummyHttpContext = new DummyHttpContext();
            var dummyViewContext = new ViewContext();
            dummyViewContext.HttpContext = dummyHttpContext;
            var dummyViewDataContainer = (IViewDataContainer)new ViewPage();
            var htmlHelper = new System.Web.Mvc.HtmlHelper(dummyViewContext, dummyViewDataContainer);

            var stylesheet = "Mvc/Styles/Designer/modal-dialog.css";

            string expected1 = string.Format(System.Globalization.CultureInfo.InvariantCulture, "<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />", stylesheet);
            string result1 = htmlHelper.StyleSheet(stylesheet).ToString();
            Assert.AreEqual(expected1, result1);

            MvcHtmlString expected2 = MvcHtmlString.Empty;
            MvcHtmlString result2 = htmlHelper.StyleSheet(stylesheet);
            Assert.AreEqual(expected2, result2);
        }

        #endregion
    }
}