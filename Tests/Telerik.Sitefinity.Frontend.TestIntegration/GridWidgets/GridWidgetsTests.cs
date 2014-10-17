using System;
using System.IO;
using System.Linq;
using MbUnit.Framework;
using Telerik.Sitefinity.Frontend.GridSystem;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.TestIntegration.Data.Content;
using Telerik.Sitefinity.TestIntegration.Helpers;
using Telerik.Sitefinity.TestIntegration.SDK;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.TestIntegration.GridWidgets
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable"), TestFixture]
    [Description("This is a class with tests related to grid widgets.")]
    public class GridWidgetsTests
    {
        /// <summary>
        /// Grid widgets - edit grid widget on page from file system
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", 
            MessageId = "Telerik.Sitefinity.Frontend.TestIntegration.GridWidgets.GridWidgetsTests.AddGridControlToPage(System.Guid,System.String,System.String,System.String)"), Test]
        [Category(TestCategories.GridWidgets)]
        [Description("Grid widgets - edit grid widget on page from file system.")]
        public void GridWidget_EditGridWidgetOnPageFromFileSystem()
        {
            PageManager pageManager = PageManager.GetManager();

            var gridVirtualPath = "~/ResourcePackages/Bootstrap/GridSystem/Templates/grid-9+3.html";
            var gridTemplatePath = Path.Combine(this.SfPath, "ResourcePackages", "Bootstrap", "GridSystem", "Templates", "grid-9+3.html");
            var newGridTemplatePath = Path.Combine(this.SfPath, "ResourcePackages", "Bootstrap", "GridSystem", "Templates", "grid-9+3-New.html");

            File.Copy(gridTemplatePath, newGridTemplatePath);

            try
            {
                var template = pageManager.GetTemplates().Where(t => t.Title == TemplateTitle).FirstOrDefault();
                Assert.IsNotNull(template, "Template was not found");

                Guid pageId = FeatherServerOperations.Pages().CreatePageWithTemplate(template, PageNamePrefix, UrlNamePrefix);

                this.AddGridControlToPage(pageId, gridVirtualPath, PlaceHolder, Caption);

                using (StreamWriter output = File.AppendText(gridTemplatePath))
                {
                    output.WriteLine(GridTextEdited);
                }

                string pageContent = this.GetPageContent(pageId);

                Assert.IsTrue(pageContent.Contains(Row3), "Grid row not found in the page content");
                Assert.IsTrue(pageContent.Contains(Row9), "Grid row not found in the page content");
                Assert.IsTrue(pageContent.Contains(ParagraphText), "Grid row not found in the page content");
            }
            finally
            {
                File.Delete(gridTemplatePath);
                File.Move(newGridTemplatePath, gridTemplatePath);
                Telerik.Sitefinity.TestUtilities.CommonOperations.ServerOperations.Pages().DeleteAllPages();
            }
        }

        /// <summary>
        /// Grid widgets - edit grid widget on page template from file system
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
            MessageId = "Telerik.Sitefinity.Frontend.TestIntegration.GridWidgets.GridWidgetsTests.AddGridControlToPageTemplate(System.Guid,System.String,System.String,System.String)"), 
            System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", 
            MessageId = "Telerik.Sitefinity.Frontend.TestIntegration.GridWidgets.GridWidgetsTests.AddGridControlToPage(System.Guid,System.String,System.String,System.String)"), Test]
        [Category(TestCategories.GridWidgets)]
        [Description("Grid widgets - edit grid widget on page template from file system.")]
        public void GridWidget_EditGridWidgetOnPageTemplateFromFileSystem()
        {
            string templateTitle = "Bootstrap.defaultNew";

            PageManager pageManager = PageManager.GetManager();

            var gridVirtualPath = "~/ResourcePackages/Bootstrap/GridSystem/Templates/grid-9+3.html";
            var gridTemplatePath = Path.Combine(this.SfPath, "ResourcePackages", "Bootstrap", "GridSystem", "Templates", "grid-9+3.html");
            var newGridTemplatePath = Path.Combine(this.SfPath, "ResourcePackages", "Bootstrap", "GridSystem", "Templates", "grid-9+3-New.html");

            File.Copy(gridTemplatePath, newGridTemplatePath);

            int templatesCount = pageManager.GetTemplates().Count();

            var layoutTemplatePath = Path.Combine(this.SfPath, "ResourcePackages", "Bootstrap", "MVC", "Views", "Layouts", "default.cshtml");
            var newLayoutTemplatePath = Path.Combine(this.SfPath, "ResourcePackages", "Bootstrap", "MVC", "Views", "Layouts", "defaultNew.cshtml");

            File.Copy(layoutTemplatePath, newLayoutTemplatePath);

            FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);

            var template = pageManager.GetTemplates().Where(t => t.Title == templateTitle).FirstOrDefault();
            Assert.IsNotNull(template, "Template was not found");

            try
            {
                this.AddGridControlToPageTemplate(template.Id, gridVirtualPath, PlaceHolder, Caption);

                Guid pageId = FeatherServerOperations.Pages().CreatePageWithTemplate(template, PageNamePrefix, UrlNamePrefix);

                using (StreamWriter output = File.AppendText(gridTemplatePath))
                {
                    output.WriteLine(GridTextEdited);
                }

                string pageContent = this.GetPageContent(pageId);

                Assert.IsTrue(pageContent.Contains(Row3), "Grid row not found in the page content");
                Assert.IsTrue(pageContent.Contains(Row9), "Grid row not found in the page content");
                Assert.IsTrue(pageContent.Contains(ParagraphText), "Grid row not found in the page content");
            }
            finally
            {
                File.Delete(gridTemplatePath);
                File.Move(newGridTemplatePath, gridTemplatePath);
                File.Delete(newLayoutTemplatePath);
                Telerik.Sitefinity.TestUtilities.CommonOperations.ServerOperations.Pages().DeleteAllPages();
                Telerik.Sitefinity.TestUtilities.CommonOperations.ServerOperations.Templates().DeletePageTemplate(template.Id);               
            }
        }

        /// <summary>
        /// Grid widgets - delete grid widget from file system
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", 
            MessageId = "Telerik.Sitefinity.Frontend.TestIntegration.GridWidgets.GridWidgetsTests.AddGridControlToPageTemplate(System.Guid,System.String,System.String,System.String)"),
            System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", 
            MessageId = "Telerik.Sitefinity.Frontend.TestIntegration.GridWidgets.GridWidgetsTests.AddGridControlToPage(System.Guid,System.String,System.String,System.String)"), Test]
        [Category(TestCategories.GridWidgets)]
        [Description("Grid widgets - delete grid widget from file system.")]
        [Ignore("There is a bug - page stop working on the frontend, when delete the grid widget template.")]
        public void GridWidget_DeleteGridWidgetOnPageFromFileSystem()
        {
            PageManager pageManager = PageManager.GetManager();

            var gridVirtualPath = "~/ResourcePackages/Bootstrap/GridSystem/Templates/grid-9+3.html";
            var gridTemplatePath = Path.Combine(this.SfPath, "ResourcePackages", "Bootstrap", "GridSystem", "Templates", "grid-9+3.html");
            var newGridTemplatePath = Path.Combine(this.SfPath, "ResourcePackages", "Bootstrap", "GridSystem", "Templates", "grid-9+3-New.html");

            File.Copy(gridTemplatePath, newGridTemplatePath);

            try
            {
                var template = pageManager.GetTemplates().Where(t => t.Title == TemplateTitle).FirstOrDefault();
                Assert.IsNotNull(template, "Template was not found");

                Guid pageId = FeatherServerOperations.Pages().CreatePageWithTemplate(template, PageNamePrefix, UrlNamePrefix);

                this.AddGridControlToPage(pageId, gridVirtualPath, PlaceHolder, Caption);

                File.Delete(gridTemplatePath);

                string pageContent = this.GetPageContent(pageId);

                Assert.IsTrue(pageContent.Contains(Row3), "Grid row not found in the page content");
                Assert.IsTrue(pageContent.Contains(Row9), "Grid row not found in the page content");
            }
            finally
            {
               File.Move(newGridTemplatePath, gridTemplatePath);
               Telerik.Sitefinity.TestUtilities.CommonOperations.ServerOperations.Pages().DeleteAllPages();
            }
        }

        /// <summary>
        /// Grid widgets - delete grid widget on page template from file system
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Telerik.Sitefinity.Frontend.TestIntegration.GridWidgets.GridWidgetsTests.AddGridControlToPageTemplate(System.Guid,System.String,System.String,System.String)"), Test]
        [Category(TestCategories.GridWidgets)]
        [Description("Grid widgets - delete grid widget on page template from file system.")]
        [Ignore("There is a bug - page stop working on the frontend, when delete the grid widget template.")]
        public void GridWidget_DeleteGridWidgetOnPageTemplateFromFileSystem()
        {
            string templateTitle = "Bootstrap.defaultNew";
            PageManager pageManager = PageManager.GetManager();

            var gridVirtualPath = "~/ResourcePackages/Bootstrap/GridSystem/Templates/grid-9+3.html";
            var gridTemplatePath = Path.Combine(this.SfPath, "ResourcePackages", "Bootstrap", "GridSystem", "Templates", "grid-9+3.html");
            var newGridTemplatePath = Path.Combine(this.SfPath, "ResourcePackages", "Bootstrap", "GridSystem", "Templates", "grid-9+3-New.html");

            File.Copy(gridTemplatePath, newGridTemplatePath);

            int templatesCount = pageManager.GetTemplates().Count();

            var layoutTemplatePath = Path.Combine(this.SfPath, "ResourcePackages", "Bootstrap", "MVC", "Views", "Layouts", "default.cshtml");
            var newLayoutTemplatePath = Path.Combine(this.SfPath, "ResourcePackages", "Bootstrap", "MVC", "Views", "Layouts", "defaultNew.cshtml");

            File.Copy(layoutTemplatePath, newLayoutTemplatePath);

            FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);

            var template = pageManager.GetTemplates().Where(t => t.Title == templateTitle).FirstOrDefault();
            Assert.IsNotNull(template, "Template was not found");

            try
            {
                this.AddGridControlToPageTemplate(template.Id, gridVirtualPath, PlaceHolder, Caption);
                Guid pageId = FeatherServerOperations.Pages().CreatePageWithTemplate(template, PageNamePrefix, UrlNamePrefix);

                File.Delete(gridTemplatePath);

                string pageContent = this.GetPageContent(pageId);

                Assert.IsTrue(pageContent.Contains(Row3), "Grid row not found in the page content");
                Assert.IsTrue(pageContent.Contains(Row9), "Grid row not found in the page content");
            }
            finally
            {
                File.Move(newGridTemplatePath, gridTemplatePath);
                File.Delete(newLayoutTemplatePath);
                Telerik.Sitefinity.TestUtilities.CommonOperations.ServerOperations.Pages().DeleteAllPages();
                Telerik.Sitefinity.TestUtilities.CommonOperations.ServerOperations.Templates().DeletePageTemplate(template.Id);     
            }
        }

        #region Helper methods

        private void AddGridControlToPage(Guid pageId, string controlPath, string placeHolder, string caption)
        {
            var control = new GridControl();
            control.Layout = controlPath;

            PageContentGenerator.AddControlToPage(pageId, control, caption, placeHolder);
        }

        private void AddGridControlToPageTemplate(Guid pageId, string controlPath, string placeHolder, string caption)
        {
            var control = new GridControl();
            control.Layout = controlPath;

            SampleUtilities.AddControlToTemplate(pageId, control, placeHolder, caption);
        }

        private string SfPath
        {
            get
            {
                return System.Web.Hosting.HostingEnvironment.MapPath("~/");
            }
        }

        private string GetPageContent(Guid pageId)
        {
            PageManager pageManager = PageManager.GetManager();

            var page = pageManager.GetPageNode(pageId);
            var pageUrl = page.GetFullUrl();
            pageUrl = RouteHelper.GetAbsoluteUrl(pageUrl);

            string pageContent = WebRequestHelper.GetPageWebContent(pageUrl);

            return pageContent;
        }

        #endregion

        #region Fields and constants

        private const string TemplateTitle = "Bootstrap.default";
        private const string PageNamePrefix = "GridPage";
        private const string UrlNamePrefix = "grid-page";
        private const string PlaceHolder = "Contentplaceholder1";
        private const string Caption = "9 + 3";
        private const string Row9 = "col-md-9";
        private const string Row3 = "col-md-3";
        private const string GridTextEdited = "<p> Test paragraph </p>";
        private const string ParagraphText = "Test paragraph";

        #endregion
    }
}
