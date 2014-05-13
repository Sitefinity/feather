using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts;

namespace Telerik.Sitefinity.Frontend.Test.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// Ensures that MasterPageBuilder class is working correctly.
    /// </summary>
    [TestClass]
    public class MasterPageBuilderTests
    {
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether the html starts with the master page directive.")]
        public void AddMasterPageDirectivesMethod_CustomString_AppendsMasterDirective()
        {
            //Arrange
            var layoutTemplateHtmlProcessor = new MasterPageBuilder();
            var htmlString = "Some html string";

            //Act
            htmlString = layoutTemplateHtmlProcessor.AddMasterPageDirectives(htmlString);

            //Assert
            Assert.IsTrue(htmlString.StartsWith(MasterPageBuilderTests.masterPageDirective), "Master page doesn't start with" + MasterPageBuilderTests.masterPageDirective);
        }

        #region Private fields and constants

        private const string masterPageDirective = "<%@ Master Language=\"C#\"";

        #endregion
    }
}
