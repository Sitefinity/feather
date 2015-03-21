using System.Web.Mvc;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.Mvc.Helpers;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Helpers;

namespace Telerik.Sitefinity.Frontend.Mvc.Test.Helpers
{
    /// <summary>
    /// Tests methods of the LayoutsHelpers class.
    /// </summary>
    [TestClass]
    public class LayoutsHelpersTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The layouts helpers tests_ get content placeholder with default id_ ensures proper result is returned.
        /// </summary>
        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether  the LayoutsHelper returns a proper content place holder with an default id of 'Body'.")]
        public void LayoutsHelpersTests_GetContentPlaceholderWithDefaultId_EnsuresProperResultIsReturned()
        {
            // Arrange
            var context = new ViewContext();
            var contentPlaceHolderString = string.Format(System.Globalization.CultureInfo.InvariantCulture, this.placeHolderHtmlFormatString, "Body");
            var urlHelper = new HtmlHelper(context, new DummyViewDataContainer());

            // Act
            var resultString = urlHelper.SfPlaceHolder().ToHtmlString();

            // Assert
            Assert.AreEqual(contentPlaceHolderString, resultString);
        }

        /// <summary>
        /// The layouts helpers tests_ get content placeholder_ ensures proper result is returned.
        /// </summary>
        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether  the LayoutsHelper returns a proper content place holder.")]
        public void LayoutsHelpersTests_GetContentPlaceholder_EnsuresProperResultIsReturned()
        {
            var contentPlaceHolderTagId = "FakeId";

            // Arrange
            var context = new ViewContext();
            var contentPlaceHolderString = string.Format(System.Globalization.CultureInfo.InvariantCulture, this.placeHolderHtmlFormatString, contentPlaceHolderTagId);
            var urlHelper = new HtmlHelper(context, new DummyViewDataContainer());

            // Act
            var resultString = urlHelper.SfPlaceHolder(contentPlaceHolderTagId).ToHtmlString();

            // Assert
            Assert.AreEqual(contentPlaceHolderString, resultString);
        }

        #endregion

        #region Fields

        private readonly string placeHolderHtmlFormatString = "<asp:contentplaceholder ID='{0}' runat='server' />";

        #endregion
    }
}