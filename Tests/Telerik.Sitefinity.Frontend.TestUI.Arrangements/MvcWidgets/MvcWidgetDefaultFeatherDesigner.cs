using System;
using Telerik.Sitefinity.Frontend.TestUI.Arrangements.MvcWidgets;
using Telerik.Sitefinity.Mvc.TestUtilities.CommonOperations;
using Telerik.Sitefinity.TestArrangementService.Attributes;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements
{
    /// <summary>
    /// MvcWidgetDefaultFeatherDesigner arragement.
    /// </summary>
    public class MvcWidgetDefaultFeatherDesigner : ITestArrangement
    {
        [ServerSetUp]
        public void SetUp()
        {
            AuthenticationHelper.AuthenticateUser(UserName, Password);
            Guid pageId = ServerOperations.Pages().CreatePage(PageName);
            ServerOperations.Widgets().AddMvcWidgetToPage(pageId, typeof(DummyTextController).FullName, WidgetCaption);
        }
        
        [ServerTearDown]
        public void TearDown()
        {
            ServerOperations.Pages().DeleteAllPages();
        }

        private const string PageName = "FeatherPage";
        private const string WidgetCaption = "DummyWidget";
        private const string UserName = "admin";
        private const string Password = "admin@2";
    }
}
