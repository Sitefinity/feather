using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.TestArrangementService.Attributes;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
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
            AuthenticationHelper.AuthenticateUser(AdminUserName, AdminPass, true);

            ServerOperations.Pages().CreatePage(PageName);
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        [ServerTearDown]
        public void TearDown()
        {
            AuthenticationHelper.AuthenticateUser(AdminUserName, AdminPass, true);

            ServerOperations.Pages().DeleteAllPages();
        }

        private const string PageName = "GridPage";
        private const string AdminUserName = "admin";
        private const string AdminPass = "admin@2";
    }
}
