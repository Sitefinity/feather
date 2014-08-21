using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;

namespace Telerik.Sitefinity.Frontend.TestUnit.Resources.Resolvers
{
    /// <summary>
    /// Tests for the EmbeddedResourceResolver class.
    /// </summary>
    [TestClass]
    public class EmbeddedResourceResolverTests
    {
        #region Public Methods and Operators

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether Exists will return false when called for a non-existing resource.")]
        public void Exists_ResourceDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var resolver = new EmbeddedResourceResolver();

            // Act
            var result =
                resolver.Exists(
                    new PathDefinition
                        {
                            IsWildcard = true, 
                            VirtualPath = "~/Test/", 
                            ResourceLocation = Assembly.GetExecutingAssembly().CodeBase
                        }, 
                    "~/Test/Resources/Imaginary/Designer.cshtml");

            // Assert
            Assert.IsFalse(result, "The method returns that resource exist when it doesn't.");
        }

        [TestMethod]
        [Description("Checks whether Exists will return true when called for an existing resource.")]
        [Owner("Boyko-Karadzhov")]
        public void Exists_ResourceExists_ReturnsTrue()
        {
            var resolver = new EmbeddedResourceResolver();
            var result =
                resolver.Exists(
                    new PathDefinition
                        {
                            IsWildcard = true, 
                            VirtualPath = "~/Test/", 
                            ResourceLocation = Assembly.GetExecutingAssembly().CodeBase
                        }, 
                    "~/Test/Resources/Designer.cshtml");

            Assert.IsTrue(result, "The method returns that resource doesn't exist when it does.");
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Reviewed. Suppression is OK here."), SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Reviewed. Suppression is OK here."), SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Reviewed. Suppression is OK here."), TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether the stream returned by Open will return the contents of the requested resource when read.")]
        public void Open_ResourceExists_ReturnsContents()
        {
            var resolver = new EmbeddedResourceResolver();
            var result =
                resolver.Open(
                    new PathDefinition
                        {
                            IsWildcard = true, 
                            VirtualPath = "~/Test/", 
                            ResourceLocation = Assembly.GetExecutingAssembly().CodeBase
                        }, 
                    "~/Test/Resources/Designer.cshtml");

            string resultString;
            using (var sr = new StreamReader(result))
            {
                resultString = sr.ReadToEnd();
            }

            Assert.AreEqual(@"@model string
@{
    ViewBag.Title = ""DesignerView"";
}

My Content", resultString, "The content of the file is not as expected.");
        }

        #endregion
    }
}