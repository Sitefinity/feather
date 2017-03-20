using System;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.TestArrangementService.Attributes;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements
{
    /// <summary>
    /// DeactivateFeatherAndDeleteWidgetFromPurePageTemplate arragement.
    /// </summary>
    public class DeactivateFeatherAndDeleteWidgetFromPurePageTemplate : TestArrangementBase
    {
        /// <summary>
        /// Server side set up. 
        /// </summary>
        [ServerSetUp]
        public void SetUp()
        {
            AuthenticationHelper.AuthenticateUser(AdminEmail, AdminPass, true);
            Guid templateId = ServerOperations.Templates().CreatePureMVCPageTemplate(PageTemplateName);
            ServerOperations.Pages().CreatePage(PageName, templateId);
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        [ServerTearDown]
        public void TearDown()
        {
            AuthenticationHelper.AuthenticateUser(AdminEmail, AdminPass, true);
            ServerOperations.Pages().DeletePage(PageName);
            ServerOperations.Templates().DeletePageTemplate(PageTemplateName);
        }

        private const string AdminEmail = "admin@test.test";
        private const string AdminPass = "admin@2";
        private const string PageName = "Page_DeactivateFeatherAndDeleteWidgetFromPurePageTemplate";
        private const string PageTemplateName = "Template_DeactivateFeatherAndDeleteWidgetFromPurePageTemplate";
    }
}
