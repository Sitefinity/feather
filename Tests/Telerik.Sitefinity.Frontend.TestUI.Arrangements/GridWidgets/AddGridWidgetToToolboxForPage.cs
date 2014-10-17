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
    /// AddGridWidgetToToolboxForPage arragement.
    /// </summary>
    public class AddGridWidgetToToolboxForPage : ITestArrangement
    {
        /// <summary>
        /// Server side set up. 
        /// </summary>
        [ServerSetUp]
        public void SetUp()
        {
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
            File.Delete(filePath);
            FeatherServerOperations.GridWidgets().RemoveGridControlFromToolboxesConfig(GridTitle);
        }

        private const string FileResource = "Telerik.Sitefinity.Frontend.TestUI.Arrangements.Data.grid-grid.html";
        private const string GridVirtualPath = "~/ResourcePackages/Bootstrap/GridSystem/Templates/grid-grid.html";
        private const string GridFileName = "grid-grid.html";
        private const string GridTitle = "grid-grid";
        private const string GridCss = "sfL25_75";
        private const string PageName = "GridPage";
        private const string PageTemplateName = "Bootstrap.default";
        private string gridPath = Path.Combine("ResourcePackages", "Bootstrap", "GridSystem", "Templates", GridFileName);
    }
}
