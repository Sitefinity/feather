using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using MbUnit.Framework;
using Telerik.Sitefinity.Frontend.Mvc.Helpers;
using Telerik.Sitefinity.Frontend.TestIntegration;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations.ActionFilters;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Controllers;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Mvc.Proxy;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.TestIntegration.Data.Content;
using Telerik.Sitefinity.TestIntegration.Helpers;
using Telerik.Sitefinity.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.TestIntegration.Mvc.Helpers
{
    /// <summary>
    /// This class contains tests methods for the <see cref="ResourceHelper" />
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable"), TestFixture]
    [Category(TestCategories.MvcCore)]
    [Description("This is a class with tests related to resource helpers.")]
    public class ResourceHelperTests
    {
        /// <summary>
        /// Tears down.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            ActionExecutionRegister.ExecutedActionInfos.Clear();

            ServerOperations.Pages().DeleteAllPages();
        }

        #region Public Methods and Operators

        /// <summary>
        /// Ensures that a JavaScript is registered only in ScriptManager for hybrid pages.
        /// </summary>
        [Test]
        [Author("EGaneva")]
        [Description("Ensures that a JavaScript is registered only in ScriptManager for hybrid pages.")]
        public void RegisterScript_HybridPage_AddedInScriptManagerOnly()
        {
            string testName = "RegisterScript";
            string pageTitle = testName + "WebFormsPage";
            string urlName = testName + "WebFormsPage";
            string pageUrl = UrlPath.ResolveAbsoluteUrl("~/" + urlName);

            this.AddDummyScriptControllerToPage(pageTitle, urlName, null);

            WebRequestHelper.GetPageWebContent(pageUrl);

            Assert.AreEqual(2, ActionExecutionRegister.ExecutedActionInfos.Count, "The actions are not executed correctly.");

            for (int i = 0; i < 2; i++)
            {
                var result = ActionExecutionRegister.ExecutedActionInfos[i].Result as ContentResult;
                Assert.IsTrue(string.IsNullOrEmpty(result.Content), "The script should not be added outside of ScriptManager.");

                var httpContext = ActionExecutionRegister.ExecutedActionInfos[i].CurrentHttpContext;
                var scriptManagerScripts = System.Web.UI.ScriptManager.GetCurrent(httpContext.Handler as System.Web.UI.Page).Scripts;
                Assert.AreEqual(2, scriptManagerScripts.Count(), "The script is not added correctly");
            }          
        }

        /// <summary>
        /// Ensures that a JavaScript is registered only in ScriptManager for pure MVC pages.
        /// </summary>
        [Test]
        [Author("EGaneva")]
        [Description("Ensures that a JavaScript is registered only in ScriptManager for pure MVC pages.")]
        [Ignore("Ignored due to infrastructural changes, until the test is fixed")]
        public void RegisterScript_PureMvcPage_AddedOnce()
        {
            string testName = "RegisterScript";
            string pageTitle = testName + "PureMvcPage";
            string urlName = testName + "PureMvcPage";
            string pageUrl = UrlPath.ResolveAbsoluteUrl("~/" + urlName);

            var pageManger = PageManager.GetManager();
            var template = pageManger.GetTemplates().Where(t => t.Title == "Foundation.default").FirstOrDefault();

            Assert.IsNotNull(template, "Template was not found");

            this.AddDummyScriptControllerToPage(pageTitle, urlName, template, "Contentplaceholder1");

            WebRequestHelper.GetPageWebContent(pageUrl);

            Assert.AreEqual(2, ActionExecutionRegister.ExecutedActionInfos.Count, "The actions are not executed correctly.");

            var result1 = ActionExecutionRegister.ExecutedActionInfos[0].Result as ContentResult;
            Assert.IsTrue(Regex.IsMatch(result1.Content, "<script src=\".*\" type=\"text/javascript\"></script>"), "The script is not added.");

            var result2 = ActionExecutionRegister.ExecutedActionInfos[1].Result as ContentResult;
            Assert.IsTrue(string.IsNullOrEmpty(result2.Content), "The script should not be added twice.");
        }

        #endregion

        #region Helper methods

        private void AddDummyScriptControllerToPage(string pageTitlePrefix, string urlNamePrefix, PageTemplate template, string placeHolder = "Body")
        {
            var controls = new List<System.Web.UI.Control>();
            var mvcProxy1 = new MvcControllerProxy();
            mvcProxy1.ControllerName = typeof(DummyScriptController).FullName;
            var newsController1 = new DummyScriptController();
            mvcProxy1.Settings = new ControllerSettings(newsController1);
            controls.Add(mvcProxy1);

            var mvcProxy2 = new MvcControllerProxy();
            mvcProxy2.ControllerName = typeof(DummyScriptController).FullName;
            var newsController2 = new DummyScriptController();
            mvcProxy2.Settings = new ControllerSettings(newsController2);
            controls.Add(mvcProxy2);

            Guid pageId = FeatherServerOperations.Pages().CreatePageWithTemplate(
                template,
                pageTitlePrefix,
                urlNamePrefix);

            PageContentGenerator.AddControlsToPage(pageId, controls, placeHolder);
        }

        #endregion
    }
}