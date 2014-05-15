using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
using Telerik.Sitefinity.TestUI.Arrangements.Framework.Attributes;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements.Pages
{
    /// <summary>
    /// Sample arragement that Creates and dletes a page.
    /// </summary>
    public class DeletePage : ITestArrangement
    {
        /// <summary>
        /// Sets up.
        /// </summary>
        [ServerSetUp]
        public void SetUp()
        {
            ServerOperations.Pages().CreatePage(PageName);
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        [ServerTearDown]
        public void TearDown()
        {
            ServerOperations.Pages().DeleteAllPages();
        }

        private const string PageName = "Feather Page";
    }
}