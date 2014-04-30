//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Web.Mvc;
//using Telerik.Microsoft.Practices.Unity;
//using Telerik.Sitefinity.Abstractions;
//using Telerik.Sitefinity.Frontend.Mvc.Controllers;
//using Telerik.Sitefinity.Frontend.Mvc.Helpers;
//using Telerik.Sitefinity.Frontend.Test.DummyClasses;
//using Telerik.Sitefinity.Frontend.Test.TestUtilities;
//using Telerik.Sitefinity.Frontend.Resources.Resolvers;

//namespace Telerik.Sitefinity.Frontend.Test.Helpers
//{
//    /// <summary>
//    /// Tests methods of the UrlHelpers class.
//    /// </summary>
//    [TestClass]
//    public class UrlHelpersTest
//    {
//        /// <summary>
//        /// Checks whether application relative paths are resolved as usual.
//        /// </summary>
//        [TestMethod]
//        public void WidgetContent_AppRelativePath_ResolvesAsNormalContent()
//        {
//            var fakeHttpContext = new FakeHttpContext();
//            var urlHelper = new UrlHelper(fakeHttpContext.Request.RequestContext);

//            var expected = urlHelper.Content("~/Test");
//            var result = urlHelper.WidgetContent("~/Test");

//            Assert.AreEqual(expected, result);
//        }

//        /// <summary>
//        /// Checks whether absolute URLs are not changed by WidgetContent.
//        /// </summary>
//        [TestMethod]
//        public void WidgetContent_AbsolutePath_Unchanged()
//        {
//            var fakeHttpContext = new FakeHttpContext();
//            var urlHelper = new UrlHelper(fakeHttpContext.Request.RequestContext);

//            var absUrl = "http://www.sitefinity.com/";
//            var result = urlHelper.WidgetContent(absUrl);

//            Assert.AreEqual(absUrl, result);
//        }

//        /// <summary>
//        /// Checks whether WidgetContent throws exception for relative paths when no RouteData is available.
//        /// </summary>
//        [TestMethod]
//        [ExpectedException(typeof(InvalidOperationException))]
//        public void WidgetContent_RelativePathNoRouteData_ThrowsException()
//        {
//            var fakeHttpContext = new FakeHttpContext();
//            var urlHelper = new UrlHelper(fakeHttpContext.Request.RequestContext);
//            urlHelper.WidgetContent("Test/MyScript.js");
//        }

//        /// <summary>
//        /// Checks whether WidgetContent will return a URL starting with the widget's virtual path given a relative URL and correct RouteData.
//        /// </summary>
//        [TestMethod]
//        public void WidgetContent_RelativePathAndRouteData_AppendsWidgetVirtualPath()
//        {
//            var fakeHttpContext = new FakeHttpContext();
//            fakeHttpContext.Request.RequestContext.RouteData.Values.Add("controller", "Dummy");
//            var urlHelper = new UrlHelper(fakeHttpContext.Request.RequestContext);

//            string result;
//            using (var factoryReg = new ControllerFactoryRegion<DummyControllerFactory>())
//            {
//                factoryReg.Factory.ControllerRegistry["Dummy"] = typeof(DummyController);

//                result = urlHelper.WidgetContent("Test/MyScript.js");
//            }

//            Assert.AreEqual("/MvcWidgetAssembly/Telerik.Sitefinity.Frontend.Test/Test/MyScript.js", result);
//        }

//        /// <summary>
//        /// Checks whether WidgetContent will return a URL containing widget resource fallback mechanisms 
//        /// given relative URL and widgetName in the RouteData.
//        /// </summary>
//        [TestMethod]
//        public void WidgetContent_RelativePathAndWidgetRouteData_AppendsWidgetVirtualPath()
//        {
//            var fakeHttpContext = new FakeHttpContext();
//            fakeHttpContext.Request.RequestContext.RouteData.Values.Add("widgetName", "Dummy");
//            fakeHttpContext.Request.RequestContext.RouteData.Values.Add("controller", "MvcControlDesigner");
//            var urlHelper = new UrlHelper(fakeHttpContext.Request.RequestContext);

//            string result;
//            using (var factoryReg = new ControllerFactoryRegion<DummyControllerFactory>())
//            {
//                using (new ObjectFactoryContainerRegion())
//                {
//                    ObjectFactory.Container.RegisterType<IVirtualFileSearchPathResolver, VirtualFileSearchPathResolver>();

//                    factoryReg.Factory.ControllerRegistry["Dummy"] = typeof(DummyController);
//                    factoryReg.Factory.ControllerRegistry["MvcControlDesigner"] = typeof(MvcControlDesignerController);

//                    result = urlHelper.WidgetContent("Test/MyScript.js");
//                }
//            }

//            Assert.AreEqual("/MvcWidgetAssembly/Telerik.Sitefinity.Frontend.Test/Test/MyScript.js", result);
//        }
//    }
//}
