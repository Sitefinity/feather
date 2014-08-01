using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Web.Mvc;
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
        private readonly string placeHolderHtmlFormatString = "<div class='sfPublicWrapper' id='PublicWrapper' runat='server'><asp:contentplaceholder id='{0}' runat='server' /></div>";

        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether  the LayoutsHelper returns a proper content place holder with an default id of 'Body'.")]
        public void LayoutsHelpersTests_GetContentPlaceholderWithDefaultId_EnsuresProperResultIsReturned()
        {
            //Arrange
            ViewContext context = new ViewContext();
            var contentPlaceHolderString = String.Format(this.placeHolderHtmlFormatString, "Body");
            var urlHelper = new HtmlHelper(context, new DummyViewDataContainer());

            //Act
            var resultString = urlHelper.SfPlaceHolder().ToHtmlString();

            //Assert
            Assert.AreEqual<string>(contentPlaceHolderString, resultString);
        }

        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether  the LayoutsHelper returns a proper content place holder.")]
        public void LayoutsHelpersTests_GetContentPlaceholder_EnsuresProperResultIsReturned()
        {
            var contentPlaceHolderTagId = "FakeId";
            //Arrange
            ViewContext context = new ViewContext();
            var contentPlaceHolderString = String.Format(this.placeHolderHtmlFormatString, contentPlaceHolderTagId);
            var urlHelper = new HtmlHelper(context, new DummyViewDataContainer());

            //Act
            var resultString = urlHelper.SfPlaceHolder(contentPlaceHolderTagId).ToHtmlString();

            //Assert
            Assert.AreEqual<string>(contentPlaceHolderString, resultString);
        }
    }


}
