using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Cache;

namespace Telerik.Sitefinity.Frontend.TestUnit.Resources
{
    /// <summary>
    /// Ensures that ServerContextHandler works correctly.
    /// </summary>
    [TestClass]
    public class ServerContextHandlerTests
    {
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Tests whether GetScript method replaces application path in the raw script.")]
        public void GetScript_ReplacesApplicationPath()
        {
            var handler = new DummyServerContextHandler();
            handler.GetRawScriptOverride = () => "var appPath = '{{applicationPath}}';";
            handler.GetApplicationPathOverride = () => "/myApp/";
            handler.GetCurrentSiteIdOverride = () => Guid.NewGuid();
            handler.GetCacheManagerOverride = () => new DummyCacheManager();
            handler.GetFrontendLanguagesOverride = () => @"[""en"", ""de""]";
            handler.GetCacheDependencyOverride = (key) => new SlidingTime(TimeSpan.FromMinutes(60)); 
            
            var result = handler.PublicGetScript();

            Assert.AreEqual("var appPath = '/myApp/';", result);
        }
    }
}
