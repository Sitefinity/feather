using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using MbUnit.Framework;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Mvc.Proxy;
using Telerik.Sitefinity.TestIntegration.Data.Content;
using Telerik.Sitefinity.TestUtilities;
using Telerik.Sitefinity.TestUtilities.Core.Mvc;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.TestIntegration.Mvc.Helpers
{
    /// <summary>
    /// This class contains integration tests which ensure that the TempData accessible in a MVC Controller is persisted properly between actions.
    /// </summary>
    [TestFixture]
    [Category(TestCategories.MvcCore)]
    [Author(TestAuthor.Team2)]
    [Description("Integration tests that ensure that the TempData accessible in a MVC Controller is persisted properly between actions.")]
    public class ControllerTempDataTests
    {
        /// <summary>
        /// Registers our test controller, since it is in the Telerik.Sitefinity.TestUtilities.Core namespace and it will not be registered, which will fail when redirect with only action name occurs.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            var sitefinityControllerFactory = (SitefinityControllerFactory)ControllerBuilder.Current.GetControllerFactory();
            sitefinityControllerFactory.RegisterController(typeof(ActionRedirectController).Name, typeof(ActionRedirectController));
        }

        /// <summary>
        /// Removes our test controller from the controller factory.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            var sitefinityControllerFactory = (SitefinityControllerFactory)ControllerBuilder.Current.GetControllerFactory();
            sitefinityControllerFactory.UnregisterController(typeof(ActionRedirectController).Name);
        }

        /// <summary>
        /// Verifies that when an action adds a temp data key/value and redirects to another action the temp data key/value is actually present.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "pageNodeId"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings"), Test]
        [Description("Verifies that when an action adds a temp data key/value and redirects to another action the temp data key/value is actually present")]
        public void AddTempDataAndRedirectToActionThatHasThisTempData_TempData_TempDataExists()
        {
            var testName = MethodInfo.GetCurrentMethod().Name;
            string pageNamePrefix = testName + "MvcPage";
            string pageTitlePrefix = testName + "Mvc Page";
            string urlNamePrefix = testName + "mvc-page";
            int index = 1;

            var actionName = ActionRedirectController.SetAndShowTempDataActionName;
            string url = UrlPath.ResolveAbsoluteUrl("~/" + urlNamePrefix + index + "/" + actionName);

            var mvcProxy = new MvcControllerProxy();
            mvcProxy.ControllerName = typeof(ActionRedirectController).FullName;

            PageContentGenerator contentGenerator = new PageContentGenerator();

            try
            {
                var pageNodeId = contentGenerator.CreatePageWithWidget(mvcProxy, string.Empty, pageNamePrefix, pageTitlePrefix, urlNamePrefix, index);

                var webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url);
                webRequest.AllowAutoRedirect = true;
                webRequest.CookieContainer = new CookieContainer();
                webRequest.Timeout = 120 * 1000;
                var webResponse = (HttpWebResponse)webRequest.GetResponse();

                string responseContent;

                using (var responseStream = new System.IO.StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8))
                {
                    responseContent = responseStream.ReadToEnd();
                }

                var expectedResult = ActionRedirectController.TempDataValuePrefix + ActionRedirectController.TempDataValue;
                Assert.IsTrue(responseContent.Contains(expectedResult), "The temp data is not properly passed between actions");
            }
            finally
            {
                contentGenerator.Dispose();
            }
        }

        /// <summary>
        /// Verifies that when an action adds a temp data key/value and redirects to another action that updates this key/value, in a third action the temp data key/value is actually updated.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "pageNodeId"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings"), Test]
        [Description("Verifies that when an action adds a temp data key/value and redirects to another action that updates this key/value, in a third action the temp data key/value is actually updated")]
        public void AddTempDataAndRedirectToActionThatUpdatesThisTempDataAndRedirectToActionThatHasThisTempData_TempData_TempDataIsUpdated()
        {
            var testName = MethodInfo.GetCurrentMethod().Name;
            string pageNamePrefix = testName + "MvcPage";
            string pageTitlePrefix = testName + "Mvc Page";
            string urlNamePrefix = testName + "mvc-page";
            int index = 1;

            var actionName = ActionRedirectController.SetAndUpdateTempDataActionName;
            string url = UrlPath.ResolveAbsoluteUrl("~/" + urlNamePrefix + index + "/" + actionName);

            var mvcProxy = new MvcControllerProxy();
            mvcProxy.ControllerName = typeof(ActionRedirectController).FullName;

            PageContentGenerator contentGenerator = new PageContentGenerator();

            try
            {
                var pageNodeId = contentGenerator.CreatePageWithWidget(mvcProxy, string.Empty, pageNamePrefix, pageTitlePrefix, urlNamePrefix, index);

                var webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url);
                webRequest.AllowAutoRedirect = true;
                webRequest.CookieContainer = new CookieContainer();
                webRequest.Timeout = 120 * 1000;
                var webResponse = (HttpWebResponse)webRequest.GetResponse();

                string responseContent;

                using (var responseStream = new System.IO.StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8))
                {
                    responseContent = responseStream.ReadToEnd();
                }

                var expectedResult = ActionRedirectController.TempDataValuePrefix + ActionRedirectController.UpdatedTempDataValue;
                Assert.IsTrue(responseContent.Contains(expectedResult), "The temp data is not properly updated between actions");
            }
            finally
            {
                contentGenerator.Dispose();
            }
        }

        /// <summary>
        /// Verifies that when an action adds a temp data key/value and redirects to another action that deletes this key/value, in a third action the temp data key/value is actually deleted.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "pageNodeId"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings"), Test]
        [Description("Verifies that when an action adds a temp data key/value and redirects to another action that deletes this key/value, in a third action the temp data key/value is actually deleted")]
        public void AddTempDataAndRedirectToActionThatDeletesThisTempDataAndRedirectToActionThatHasThisTempData_TempData_TempDataIsDeleted()
        {
            var testName = MethodInfo.GetCurrentMethod().Name;
            string pageNamePrefix = testName + "MvcPage";
            string pageTitlePrefix = testName + "Mvc Page";
            string urlNamePrefix = testName + "mvc-page";
            int index = 1;

            var actionName = ActionRedirectController.SetAndDeleteTempDataActionName;
            string url = UrlPath.ResolveAbsoluteUrl("~/" + urlNamePrefix + index + "/" + actionName);

            var mvcProxy = new MvcControllerProxy();
            mvcProxy.ControllerName = typeof(ActionRedirectController).FullName;

            PageContentGenerator contentGenerator = new PageContentGenerator();

            try
            {
                var pageNodeId = contentGenerator.CreatePageWithWidget(mvcProxy, string.Empty, pageNamePrefix, pageTitlePrefix, urlNamePrefix, index);

                var webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url);
                webRequest.AllowAutoRedirect = true;
                webRequest.CookieContainer = new CookieContainer();
                webRequest.Timeout = 120 * 1000;
                var webResponse = (HttpWebResponse)webRequest.GetResponse();

                string responseContent;

                using (var responseStream = new System.IO.StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8))
                {
                    responseContent = responseStream.ReadToEnd();
                }

                var expectedResult = ActionRedirectController.TempDataValuePrefix + string.Empty;
                Assert.IsTrue(responseContent.Contains(expectedResult), "The temp data is not properly deleted between actions");
            }
            finally
            {
                contentGenerator.Dispose();
            }
        }

        /// <summary>
        /// This class represents a dummy MVC widget which has actions that redirect to other actions.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public class ActionRedirectController : Controller
        {
            /// <summary>
            /// The temporary data value prefix used for inserting before the content returned from ShowData ActionResult.
            /// </summary>
            public const string TempDataValuePrefix = "ActionRedirectControllerTempDataValuePrefix#";

            /// <summary>
            /// The temporary data value.
            /// </summary>
            public const string TempDataValue = "TempDataValueToUse";

            /// <summary>
            /// The updated temporary data value.
            /// </summary>
            public const string UpdatedTempDataValue = "UpdatedTempDataValueToUse";

            /// <summary>
            /// The set and show temporary data action name.
            /// </summary>
            public const string SetAndShowTempDataActionName = "SetAndShowTempData";

            /// <summary>
            /// The set and update temporary data action name.
            /// </summary>
            public const string SetAndUpdateTempDataActionName = "SetAndUpdateTempData";

            /// <summary>
            /// The set and delete temporary data action name.
            /// </summary>
            public const string SetAndDeleteTempDataActionName = "SetAndDeleteTempData";

            private const string TempDataKey = "TempDataKeyToUse";

            /// <summary>
            /// The SetAndShowTempData action of the <see cref="ActionRedirectController"/>.
            /// </summary>
            /// <returns>
            /// Redirects to ShowTempData action of the <see cref="ActionRedirectController"/>.
            /// </returns>
            public ActionResult SetAndShowTempData()
            {
                this.TempData.Add(TempDataKey, TempDataValue);
                return this.RedirectToAction("ShowTempData");
            }

            /// <summary>
            /// The SetAndUpdateTempData action of the <see cref="ActionRedirectController"/>.
            /// </summary>
            /// <returns>
            /// Redirects to UpdateTempData action of the <see cref="ActionRedirectController"/>.
            /// </returns>
            public ActionResult SetAndUpdateTempData()
            {
                this.TempData.Add(TempDataKey, TempDataValue);
                return this.RedirectToAction("UpdateTempData");
            }

            /// <summary>
            /// The SetAndDeleteTempData action of the <see cref="ActionRedirectController"/>.
            /// </summary>
            /// <returns>
            /// Redirects to DeleteTempData action of the <see cref="ActionRedirectController"/>.
            /// </returns>
            public ActionResult SetAndDeleteTempData()
            {
                this.TempData.Add(TempDataKey, TempDataValue);
                return this.RedirectToAction("DeleteTempData");
            }

            /// <summary>
            /// The UpdateTempData action of the <see cref="ActionRedirectController"/>.
            /// </summary>
            /// <returns>
            /// Redirects to ShowTempData action of the <see cref="ActionRedirectController"/>.
            /// </returns>
            public ActionResult UpdateTempData()
            {
                this.TempData[TempDataKey] = UpdatedTempDataValue;
                return this.RedirectToAction("ShowTempData");
            }

            /// <summary>
            /// The DeleteTempData action of the <see cref="ActionRedirectController"/>.
            /// </summary>
            /// <returns>
            /// Redirects to ShowTempData action of the <see cref="ActionRedirectController"/>.
            /// </returns>
            public ActionResult DeleteTempData()
            {
                this.TempData.Remove(TempDataKey);
                return this.RedirectToAction("ShowTempData");
            }

            /// <summary>
            /// The ShowTempData action of the <see cref="ActionRedirectController"/>.
            /// </summary>
            /// <returns>
            /// The temp data value for the default key as a content.
            /// </returns>
            public ActionResult ShowTempData()
            {
                return this.Content(string.Format("{0}{1}", ActionRedirectController.TempDataValuePrefix, this.TempData[TempDataKey]));
            }
        }
    }
}