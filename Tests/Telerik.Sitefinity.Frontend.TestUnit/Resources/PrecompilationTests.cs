using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.TestUtilities;

namespace Telerik.Sitefinity.Frontend.TestUnit.Resources
{
    /// <summary>
    /// Tests precompilation.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Precompilation")]
    [TestClass]
    public class PrecompilationTests
    {
        /// <summary>
        /// Tests whether the views of Telerik.Sitefinity.Frontend are precompiled.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Tests whether the views of Telerik.Sitefinity.Frontend are precompiled.")]
        public void FrontendAssembly_HasPrecompiledViews()
        {
            string[] failedViews;
            Assert.IsTrue(AssemblyLoaderHelper.EnsurePrecompiledRazorViews(typeof(FrontendModule).Assembly, out failedViews), "Some views are not precompiled: " + string.Join(", ", failedViews));
        }
    }
}
