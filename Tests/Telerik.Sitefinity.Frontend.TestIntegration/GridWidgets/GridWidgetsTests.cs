using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.UI;
using MbUnit.Framework;
using Telerik.Sitefinity.Frontend.GridSystem;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Mvc.Proxy;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.TestIntegration.Data.Content;
using Telerik.Sitefinity.TestIntegration.Helpers;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.TestIntegration.GridWidgets
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable"), TestFixture]
    [Description("This is a class with tests related to grid widgets.")]
    public class GridWidgetsTests
    {
        /// <summary>
        /// Tears down.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            Telerik.Sitefinity.TestUtilities.CommonOperations.ServerOperations.Pages().DeleteAllPages();
        }

        /// <summary>
        /// Grid widgets - edit grid widget from file system
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Telerik.Sitefinity.Frontend.TestIntegration.GridWidgets.GridWidgetsTests.AddGridControlToPage(System.Guid,System.String,System.String,System.String)"), Test]
        [Category(TestCategories.GridWidgets)]
        [Description("Grid widgets - edit grid widget from file system.")]
        public void GridWidget_EditGridWidgetFromFileSystem()
        {
            string templateTitle = "Bootstrap.default";
            string pageNamePrefix = "GridPage";
            string urlNamePrefix = "grid-page";
            string placeHolder = "Contentplaceholder1";
            string caption = "9 + 3";
            string row9 = "col-md-9";
            string row3 = "col-md-3";
            string gridTextEdited = "<p> Test paragraph </p>";
            string paragraphText = "Test paragraph";
            PageManager pageManager = PageManager.GetManager();

            var gridVirtualPath = "~/ResourcePackages/Bootstrap/GridSystem/Templates/grid-9+3.html";
            var gridTemplatePath = Path.Combine(this.SfPath, "ResourcePackages", "Bootstrap", "GridSystem", "Templates", "grid-9+3.html");
            var newGridTemplatePath = Path.Combine(this.SfPath, "ResourcePackages", "Bootstrap", "GridSystem", "Templates", "grid-9+3-New.html");

            File.Copy(gridTemplatePath, newGridTemplatePath);

            try
            {
                var template = pageManager.GetTemplates().Where(t => t.Title == templateTitle).FirstOrDefault();
                Assert.IsNotNull(template, "Template was not found");

                Guid pageId = FeatherServerOperations.Pages().CreatePageWithTemplate(template, pageNamePrefix, urlNamePrefix);

                this.AddGridControlToPage(pageId, gridVirtualPath, placeHolder, caption);

                var page = pageManager.GetPageNode(pageId);
                var pageUrl = page.GetFullUrl();
                pageUrl = RouteHelper.GetAbsoluteUrl(pageUrl);

                using (StreamWriter output = File.AppendText(gridTemplatePath))
                {
                    output.WriteLine(gridTextEdited);
                }  

                string pageContent = WebRequestHelper.GetPageWebContent(pageUrl);

                Assert.IsTrue(pageContent.Contains(row3), "Grid row not found in the page content");
                Assert.IsTrue(pageContent.Contains(row9), "Grid row not found in the page content");
                Assert.IsTrue(pageContent.Contains(paragraphText), "Grid row not found in the page content");
            }
            finally
            {
                File.Delete(gridTemplatePath);
                File.Move(newGridTemplatePath, gridTemplatePath);
            }
        }

        private string SfPath
        {
            get
            {
                return System.Web.Hosting.HostingEnvironment.MapPath("~/");
            }
        }

        public void AddGridControlToPage(Guid pageId, string controlPath, string placeHolder, string caption)
        {
            var control = new GridControl();
            control.Layout = controlPath;

            PageContentGenerator.AddControlToPage(pageId, control, caption, placeHolder);
        }
    }
}
