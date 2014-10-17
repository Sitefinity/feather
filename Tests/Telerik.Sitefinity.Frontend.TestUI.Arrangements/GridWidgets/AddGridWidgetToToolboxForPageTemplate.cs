using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Frontend.GridSystem;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Modules.Pages.Configuration;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
using Telerik.Sitefinity.TestUI.Arrangements.Framework.Attributes;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements
{
    /// <summary>
    /// AddGridWidgetToToolboxForPageTemplate arragement.
    /// </summary>
    public class AddGridWidgetToToolboxForPageTemplate : ITestArrangement
    {
        /// <summary>
        /// Server side set up. 
        /// </summary>
        [ServerSetUp]
        public void SetUp()
        {
            string templateFileOriginal = FileInjectHelper.GetDestinationFilePath(this.layoutTemplatePath);
            string templateFileCopy = FileInjectHelper.GetDestinationFilePath(this.newLayoutTemplatePath);

            PageManager pageManager = PageManager.GetManager();
            int templatesCount = pageManager.GetTemplates().Count();
            File.Copy(templateFileOriginal, templateFileCopy);
            FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);

            string filePath = FileInjectHelper.GetDestinationFilePath(this.gridPath);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            Stream destination = new FileStream(filePath, FileMode.Create, FileAccess.Write);

            var assembly = FileInjectHelper.GetArrangementsAssembly();
            Stream source = assembly.GetManifestResourceStream(FileResource);
            FileInjectHelper.CopyStream(source, destination);
            source.Close();
            destination.Close();

            FeatherServerOperations.GridWidgets().AddGridControlToToolboxesConfig(GridTitle, GridCss, GridVirtualPath);
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

            string filePath = FileInjectHelper.GetDestinationFilePath(this.gridPath);
            string templateFileCopy = FileInjectHelper.GetDestinationFilePath(this.newLayoutTemplatePath);
            File.Delete(filePath);
            File.Delete(templateFileCopy);

            ServerOperations.Templates().DeletePageTemplate(PageTemplateName);
            FeatherServerOperations.GridWidgets().RemoveGridControlFromToolboxesConfig(GridTitle);
        }

        private const string FileResource = "Telerik.Sitefinity.Frontend.TestUI.Arrangements.Data.grid-grid.html";
        private const string GridVirtualPath = "~/ResourcePackages/Bootstrap/GridSystem/Templates/grid-grid.html";
        private const string GridFileName = "grid-grid.html";
        private const string GridTitle = "grid-grid";
        private const string GridCss = "sfL25_75";
        private const string PageName = "GridPage";
        private const string PageTemplateName = "Bootstrap.defaultNew";
        private string layoutTemplatePath = Path.Combine("ResourcePackages", "Bootstrap", "MVC", "Views", "Layouts", "default.cshtml");
        private string newLayoutTemplatePath = Path.Combine("ResourcePackages", "Bootstrap", "MVC", "Views", "Layouts", "defaultNew.cshtml");
        private string gridPath = Path.Combine("ResourcePackages", "Bootstrap", "GridSystem", "Templates", GridFileName);
    }
}
