using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;

namespace Telerik.Sitefinity.Frontend.Test.Resources.Resolvers
{
    /// <summary>
    /// Tests for the EmbeddedResourceResolver class.
    /// </summary>
    [TestClass]
    public class EmbeddedResourceResolverTest
    {
        /// <summary>
        /// Checks whether Exists will return true when called for an existing resource.
        /// </summary>
        [TestMethod]
        public void Exists_ResourceExists_ReturnsTrue()
        {
            var resolver = new EmbeddedResourceResolver();
            var result = resolver.Exists(new PathDefinition()
            {
                IsWildcard = true,
                VirtualPath = "~/Test/",
                ResourceLocation = Assembly.GetExecutingAssembly().CodeBase
            }, "~/Test/Resources/Master.Designer.cshtml");

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Checks whether Exists will return true when called for a non-existing resource.
        /// </summary>
        [TestMethod]
        public void Exists_ResourceDoesNotExist_ReturnsFalse()
        {
            var resolver = new EmbeddedResourceResolver();
            var result = resolver.Exists(new PathDefinition()
            {
                IsWildcard = true,
                VirtualPath = "~/Test/",
                ResourceLocation = Assembly.GetExecutingAssembly().CodeBase
            }, "~/Test/Resources/Imaginary/Master.Designer.cshtml");

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Checks whether the stream returned by Open will return the contents of the requested resource when read.
        /// </summary>
        [TestMethod]
        public void Open_ResourceExists_ReturnsContents()
        {
            var resolver = new EmbeddedResourceResolver();
            var result = resolver.Open(new PathDefinition()
            {
                IsWildcard = true,
                VirtualPath = "~/Test/",
                ResourceLocation = Assembly.GetExecutingAssembly().CodeBase
            }, "~/Test/Resources/Master.Designer.cshtml");

            string resultString;
            using (var sr = new StreamReader(result))
            {
                resultString = sr.ReadToEnd();
            }

            Assert.AreEqual(@"@model string
@{
    ViewBag.Title = ""DesignerView"";
}

My Content", resultString);
        }
    }
}
