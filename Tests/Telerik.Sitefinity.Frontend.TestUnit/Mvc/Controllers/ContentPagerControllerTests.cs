using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.Mvc.Controllers;

namespace Telerik.Sitefinity.Frontend.TestUnit.Mvc.Controllers
{
    /// <summary>
    /// Unit tests for ContentPagerController class.
    /// </summary>
    [TestClass]
    public class ContentPagerControllerTests
    {
        /// <summary>
        /// Ensures that reflected CanonicalUrl API is still available.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Api")]
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Ensures that reflected CanonicalUrl API is still available.")]
        public void CanonicalUrlApi_Reflection_NotNull()
        {
            var paginationUrls = ContentPagerController.GetPaginationUrls("next", "prev");
            Assert.IsNotNull(paginationUrls, "Could not get an instance of PaginationUrls.");

            var tryStorePaginationUrlsMethod = ContentPagerController.GetTryStorePaginationUrlsMethod();
            Assert.IsNotNull(tryStorePaginationUrlsMethod);
            Assert.AreEqual(2, tryStorePaginationUrlsMethod.GetParameters().Length);
            Assert.AreEqual(typeof(System.Web.UI.Page), tryStorePaginationUrlsMethod.GetParameters()[0].ParameterType);
            Assert.AreEqual(paginationUrls.GetType(), tryStorePaginationUrlsMethod.GetParameters()[1].ParameterType);
        }
    }
}
