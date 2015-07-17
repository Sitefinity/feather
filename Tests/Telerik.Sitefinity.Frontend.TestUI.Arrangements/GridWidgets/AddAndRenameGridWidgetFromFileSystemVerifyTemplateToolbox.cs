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
    /// AutoGenerateGridWidgetToToolboxForPageTemplate arragement.
    /// </summary>
    public class AddAndRenameGridWidgetFromFileSystemVerifyTemplateToolbox : ITestArrangement
    {
        /// <summary>
        /// Server side set up. 
        /// </summary>
        [ServerSetUp]
        public void SetUp()
        {
            AuthenticationHelper.AuthenticateUser(AdminUserName, AdminPass, true);

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

            Guid templateId = ServerOperations.Templates().GetTemplateIdByTitle(PageTemplateName);
            ServerOperations.Pages().CreatePage(PageName, templateId);
        }

        /// <summary>
        /// Rename the grid widget file from the file system.
        /// </summary>
        [ServerArrangement]
        public void RenameGridWidgetFromFileSystem()
        {
            string filePath = FileInjectHelper.GetDestinationFilePath(this.gridPath);
            string newFilePath = FileInjectHelper.GetDestinationFilePath(this.newGridPath);

            File.Move(filePath, newFilePath);
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        [ServerTearDown]
        public void TearDown()
        {
            string filePath = FileInjectHelper.GetDestinationFilePath(this.gridPath);
            string templateFileCopy = FileInjectHelper.GetDestinationFilePath(this.newLayoutTemplatePath);
            string newFilePath = FileInjectHelper.GetDestinationFilePath(this.newGridPath);

            File.Delete(filePath);
            File.Delete(templateFileCopy);
            File.Delete(newFilePath);

            ServerOperations.Pages().DeleteAllPages();
            ServerOperations.Templates().DeletePageTemplate(PageTemplateName);
            FeatherServerOperations.GridWidgets().RemoveGridControlFromToolboxesConfig(GridTitle);
        }

        private const string AdminUserName = "admin";
        private const string AdminPass = "admin@2";
        private const string FileResource = "Telerik.Sitefinity.Frontend.TestUI.Arrangements.Data.grid-grid.html";
        private const string GridFileName = "grid-grid.html";
        private const string NewGridFileName = "renamed-grid.html";
        private const string GridTitle = "grid-grid";
        private const string GridCss = "sfL25_75";
        private const string PageName = "GridPage";
        private const string PageTemplateName = "defaultNew";
        private readonly string layoutTemplatePath = Path.Combine("ResourcePackages", "Bootstrap", "MVC", "Views", "Layouts", "default.cshtml");
        private readonly string newLayoutTemplatePath = Path.Combine("ResourcePackages", "Bootstrap", "MVC", "Views", "Layouts", "defaultNew.cshtml");
        private readonly string gridPath = Path.Combine("ResourcePackages", "Bootstrap", "GridSystem", "Templates", GridFileName);
        private readonly string newGridPath = Path.Combine("ResourcePackages", "Bootstrap", "GridSystem", "Templates", NewGridFileName);
    }
}
