using System;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.TestArrangementService.Attributes;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements
{
    /// <summary>
    /// DeactivateFeatherAndDeleteWidgetFromPurePage arragement.
    /// </summary>
    public class DeactivateFeatherAndDeleteWidgetFromPurePage : ITestArrangement
    {
        /// <summary>
        /// Server side set up. 
        /// </summary>
        [ServerSetUp]
        public void SetUp()
        {
            AuthenticationHelper.AuthenticateUser(AdminUserName, AdminPass, true);
            Guid templateId = ServerOperations.Templates().CreatePureMVCPageTemplate(PageTemplateName);
            ServerOperations.Pages().CreatePage(PageName, templateId);
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        [ServerTearDown]
        public void TearDown()
        {
            AuthenticationHelper.AuthenticateUser(AdminUserName, AdminPass, true);
            ServerOperations.Pages().DeletePage(PageName);
            ServerOperations.Templates().DeletePageTemplate(PageTemplateName);
        }

        private const string AdminUserName = "admin";
        private const string AdminPass = "admin@2";
        private const string PageName = "Page_DeactivateFeatherAndDeleteWidgetFromPurePage";
        private const string PageTemplateName = "Template_DeactivateFeatherAndDeleteWidgetFromPurePage";
    }
}
