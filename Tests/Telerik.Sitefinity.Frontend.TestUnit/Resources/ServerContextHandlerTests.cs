using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses;

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
            
            var result = handler.PublicGetScript();

            Assert.AreEqual("var appPath = '/myApp/';", result);
        }
    }
}
