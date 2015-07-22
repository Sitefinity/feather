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
    /// AutoGenerateGridWidgetToToolboxForPage arragement.
    /// </summary>
    public class AddAndDeleteGridWidgetFromFileSystemVerifyPageToolbox : ITestArrangement
    {
        /// <summary>
        /// Server side set up. 
        /// </summary>
        [ServerSetUp]
        public void SetUp()
        {
            AuthenticationHelper.AuthenticateUser(AdminUserName, AdminPass, true);

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
        /// Removes the grid widget file from the file system.
        /// </summary>
        [ServerArrangement]
        public void DeleteGridWidgetFromFileSystem()
        {
            string filePath = FileInjectHelper.GetDestinationFilePath(this.gridPath);
            File.Delete(filePath);
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        [ServerTearDown]
        public void TearDown()
        {
            ServerOperations.Pages().DeleteAllPages();
            string filePath = FileInjectHelper.GetDestinationFilePath(this.gridPath);
            File.Delete(filePath);
            FeatherServerOperations.GridWidgets().RemoveGridControlFromToolboxesConfig(GridTitle);
        }

        private const string AdminUserName = "admin";
        private const string AdminPass = "admin@2";
        private const string FileResource = "Telerik.Sitefinity.Frontend.TestUI.Arrangements.Data.grid-grid.html";
        private const string GridFileName = "grid-grid.html";
        private const string GridTitle = "grid-grid";
        private const string PageName = "GridPage";
        private const string PageTemplateName = "Bootstrap.default";
        private string gridPath = Path.Combine("ResourcePackages", "Bootstrap", "GridSystem", "Templates", GridFileName);
    }
}
