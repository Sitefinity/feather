﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts;

namespace Telerik.Sitefinity.Frontend.Test.Mvc.Infrastructure.Layouts
{    
    /// <summary>
    /// Ensures that TemplateTitleParser class is working correctly.
    /// </summary>
    [TestClass]
    public class TemplateTitleParserTests
    {
        #region GetLayoutName

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether GetLayoutName method replaces invalid characters.")]
        public void GetLayoutName_InvalidCharacters_ReplaceIvalidCharacters() 
        {
            //Arrange
            var templateTitle = "Some<>*Test:?Title";
            var expectedLayoutFileName = "Some_Test_Title";

            //Act
            var templateTitleParser = new TemplateTitleParser();
            var resultLayoutName = templateTitleParser.GetLayoutName(templateTitle);

            //Assert
            Assert.AreEqual(expectedLayoutFileName, resultLayoutName, "The invalid characters are not stripped correctly.");
        }

        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether GetLayoutName method returns full template title if can't resolve existing package folder.")]
        public void GetLayoutName_NonExistingPackage_ReturnsInitialTemplateTitle()
        {
            //Arrange
            var templateTitle = "TestPackage.TestTemplateName";

            //Act
            var templateTitleParser = new TemplateTitleParser();
            var resultLayoutName = templateTitleParser.GetLayoutName(templateTitle);

            //Assert
            Assert.AreEqual(templateTitle, resultLayoutName, "The initial template title should be preserved.");
        }

        #endregion
    }
}
