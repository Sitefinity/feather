using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
using Telerik.Sitefinity.TestUI.Arrangements.Framework.Attributes;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements
{
    /// <summary>
    /// OldLayoutWidgetAndNewGridWidgetOnTheSamePage arragement.
    /// </summary>
    public class OldLayoutWidgetAndNewGridWidgetOnTheSamePage : ITestArrangement
    {
        /// <summary>
        /// Server side set up. 
        /// </summary>
        [ServerSetUp]
        public void SetUp()
        {
            Guid templateId = ServerOperations.Templates().GetTemplateIdByTitle(PageTemplateName);
            ServerOperations.Pages().CreatePage(PageName, templateId);
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        [ServerTearDown]
        public void TearDown()
        {
            ServerOperations.Pages().DeleteAllPages();
        }

        private const string PageName = "GridPage";
        private const string PageTemplateName = "Bootstrap.default";
    }
}
