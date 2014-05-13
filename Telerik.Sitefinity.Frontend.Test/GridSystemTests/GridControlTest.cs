using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses;
using Telerik.Sitefinity.Utilities.HtmlParsing;

namespace Telerik.Sitefinity.Frontend.Test.Controls
{
    /// <summary>
    /// Tests methods of GridControl class.
    /// </summary>
    [TestClass]
    public class GridControlTest
    {
        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether a non-server div with class sf_cols will have runat='server' appended to it.")]
        public void ProcessLayoutString_NonServerControlCols_DivIsServerControl()
        {
            var layoutControl = new DummyGridControl();
            var result = layoutControl.PublicProcessLayoutString(@"<div class=""sf_cols""></div>", ensureSfColsWrapper: false);

            Assert.AreEqual(@"<div class=""sf_cols"" runat=""server""></div>", result);
        }

        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether a non-server div with class sf_colsIn will have runat='server' appended to it.")]
        public void ProcessLayoutString_NonServerControlColsIn_DivIsServerControl()
        {
            var layoutControl = new DummyGridControl();
            var result = layoutControl.PublicProcessLayoutString(@"<div class=""sf_colsIn""></div>", ensureSfColsWrapper: false);

            Assert.AreEqual(@"<div class=""sf_colsIn"" runat=""server""></div>", result);
        }

        [TestMethod]
        [Owner("Bonchev")]
        [Description(" Checks whether a non-server div with class sf_colsOut will have runat='server' appended to it.")]
        public void ProcessLayoutString_NonServerControlColsOut_DivIsServerControl()
        {
            var layoutControl = new DummyGridControl();
            var result = layoutControl.PublicProcessLayoutString(@"<div class=""sf_colsOut""></div>", ensureSfColsWrapper: false);

            Assert.AreEqual(@"<div class=""sf_colsOut"" runat=""server""></div>", result);
        }

        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether a non-server div with class sf_colsOut will have runat='server' appended to it and it will be wrapped with a sf_cols div when ensureSfColsWrapper is set.")]
        public void ProcessLayoutString_NonServerControlColsOutAndEnsureSfCols_DivIsServerControlAndWrapped()
        {
            var layoutControl = new DummyGridControl();
            var result = layoutControl.PublicProcessLayoutString(@"<div class=""sf_colsOut""></div>", ensureSfColsWrapper: true);

            Assert.AreEqual(@"<div runat=""server"" class=""sf_cols""><div class=""sf_colsOut"" runat=""server""></div></div>", result);
        }

        [TestMethod]
        [Description("Checks whether a server div with class sf_colsOut will be returned unchanged.")]
        public void PrecessLayoutString_ServerControlColsOut_ReturnsUnchangedTemplate()
        {
            var layoutControl = new DummyGridControl();
            var template = @"<div class=""sf_colsOut"" runat=""server""></div>";
            var result = layoutControl.PublicProcessLayoutString(template, ensureSfColsWrapper: false);

            Assert.AreEqual(template, result);
        }

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
    }
}
