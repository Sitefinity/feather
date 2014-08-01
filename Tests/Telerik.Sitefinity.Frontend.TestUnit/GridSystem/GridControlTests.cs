using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Controls;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.GridSystem;
using Telerik.Sitefinity.Utilities.HtmlParsing;

namespace Telerik.Sitefinity.Frontend.TestUnit.GridSystem
{
    /// <summary>
    /// Tests methods of GridControl class.
    /// </summary>
    [TestClass]
    public class GridControlTests
    {
        #region ProcessLayoutString

        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether a non-server div with class sf_cols will have runat='server' appended to it.")]
        public void ProcessLayoutString_NonServerControlCols_DivIsServerControl()
        {
            //Arrange
            var layoutControl = new DummyGridControl();

            //Act
            var result = layoutControl.PublicProcessLayoutString(@"<div class=""sf_cols""></div>", ensureSfColsWrapper: false);

            //Assert
            Assert.AreEqual(@"<div class=""sf_cols"" runat=""server""></div>", result, "Server tag is not appended correctly.");
        }

        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether a non-server div with class sf_colsIn will have runat='server' appended to it.")]
        public void ProcessLayoutString_NonServerControlColsIn_DivIsServerControl()
        {
            //Arrange
            var layoutControl = new DummyGridControl();

            //Act
            var result = layoutControl.PublicProcessLayoutString(@"<div class=""sf_colsIn""></div>", ensureSfColsWrapper: false);

            //Assert
            Assert.AreEqual(@"<div class=""sf_colsIn"" runat=""server""></div>", result, "Server tag is not appended correctly.");
        }

        [TestMethod]
        [Owner("Bonchev")]
        [Description(" Checks whether a non-server div with class sf_colsOut will have runat='server' appended to it.")]
        public void ProcessLayoutString_NonServerControlColsOut_DivIsServerControl()
        {
            //Arrange
            var layoutControl = new DummyGridControl();

            //Act
            var result = layoutControl.PublicProcessLayoutString(@"<div class=""sf_colsOut""></div>", ensureSfColsWrapper: false);

            //Assert
            Assert.AreEqual(@"<div class=""sf_colsOut"" runat=""server""></div>", result, "Server tag is not appended correctly.");
        }

        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether a non-server div with class sf_colsOut will have runat='server' appended to it and it will be wrapped with a sf_cols div when ensureSfColsWrapper is set.")]
        public void ProcessLayoutString_NonServerControlColsOutAndEnsureSfCols_DivIsServerControlAndWrapped()
        {
            //Arrange
            var layoutControl = new DummyGridControl();

            //Act
            var result = layoutControl.PublicProcessLayoutString(@"<div class=""sf_colsOut""></div>", ensureSfColsWrapper: true);

            //Assert
            Assert.AreEqual(@"<div runat=""server"" class=""sf_cols""><div class=""sf_colsOut"" runat=""server""></div></div>", result, "Sf_cols wrapper div is not added added.");
        }

        [TestMethod]
        [Description("Checks whether a server div with class sf_colsOut will be returned unchanged.")]
        public void PrecessLayoutString_ServerControlColsOut_ReturnsUnchangedTemplate()
        {
            //Arrange
            var layoutControl = new DummyGridControl();
            var template = @"<div class=""sf_colsOut"" runat=""server""></div>";

            //Act
            var result = layoutControl.PublicProcessLayoutString(template, ensureSfColsWrapper: false);

            //Assert
            Assert.AreEqual(template, result, "The template is not preserved.");
        }

        #endregion 

        #region GetAttributeValue

        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether the GetAttributeValue method properly returns the value of a given attribute")]
        public void GetAttributeValue_GetTheValueOfTheClassAttribute_VerifyTheMethodReturnsTheProperAttributeValue()
        {
            //Arrange: Initialize the GridControl, create a fake HTML template with attributes
            var layoutControl = new DummyGridControl();
            string expectedAttributeValue = "sf_colsOut";
            string actualAttributeValue = string.Empty;
            string attributeName = "class";
            var template = string.Format(@"<div {0}=""{1}"" runat=""server""></div>", attributeName, expectedAttributeValue);

            //Act: parse the HTML template and then get the value of the class attribute
            using (HtmlParser parser = new HtmlParser(template))
            {
                parser.SetChunkHashMode(false);
                parser.AutoExtractBetweenTagsOnly = false;
                parser.CompressWhiteSpaceBeforeTag = false;
                parser.KeepRawHTML = true;
                HtmlChunk chunk = parser.ParseNext();
                actualAttributeValue = layoutControl.PublicGetAttributeValue(chunk, attributeName);
            }

            //Assert: Verify the GetAttributeValue of the GridControl class is returning the correct attribute value
            Assert.AreEqual(expectedAttributeValue, actualAttributeValue, "The attribute value returned by the GetAttributeValue method is not correct.");
        }

        #endregion 
    }
}
