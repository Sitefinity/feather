using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts;

namespace Telerik.Sitefinity.Frontend.TestUnit.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// Ensures that MasterPageBuilder class is working correctly.
    /// </summary>
    [TestClass]
    public class MasterPageBuilderTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The add master page directives_ custom string_ appends master directive.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether the html starts with the master page directive.")]
        public void AddMasterPageDirectives_CustomString_AppendsMasterDirective()
        {
            // Arrange
            var layoutTemplateHtmlProcessor = new MasterPageBuilder();
            var htmlString = "Some html string";

            // Act
            htmlString = layoutTemplateHtmlProcessor.AddMasterPageDirectives(htmlString);

            // Assert
            Assert.IsTrue(htmlString.StartsWith(MasterPageDirective, System.StringComparison.Ordinal), "Master page doesn't start with" + MasterPageDirective);
        }

        #endregion

        #region Constants

        private const string MasterPageDirective = "<%@ Master Language=\"C#\"";

        #endregion
    }
}