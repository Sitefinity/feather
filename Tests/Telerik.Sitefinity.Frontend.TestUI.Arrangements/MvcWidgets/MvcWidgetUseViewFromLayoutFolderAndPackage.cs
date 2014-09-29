using System;
using System.IO;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
using Telerik.Sitefinity.TestUI.Arrangements.Framework.Attributes;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements
{
    public class MvcWidgetUseViewFromLayoutFolderAndPackage : ITestArrangement
    {
        [ServerSetUp]
        public void SetUp()
        {
            Guid templateId = ServerOperations.Templates().GetTemplateIdByTitle(BootstrapTemplate);
            Guid pageId = ServerOperations.Pages().CreatePage(PageName, templateId);
            Guid pageNodeId = ServerOperations.Pages().GetPageNodeId(pageId);
            FeatherServerOperations.Pages().AddMvcWidgetToPage(pageNodeId, typeof(MvcTestController).FullName, WidgetCaption, PlaceHolderId);
        }

        [ServerArrangement]
        public void AddNewViewToPackage()
        {
            string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageMvcViewDestinationFilePath(PackageName, WidgetName, ViewFileName);
            FeatherServerOperations.ResourcePackages().AddNewResource(FileResource, filePath);
        }

        [ServerArrangement]
        public void AddNewViewToLayoutsFolder()
        {
            var folderPath = Path.Combine(this.SfPath, "Mvc", "Views", WidgetName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, ViewFileName);

            FeatherServerOperations.ResourcePackages().AddNewResource(LayoutFileResource, filePath);
        }

        [ServerTearDown]
        public void TearDown()
        {
            ServerOperations.Pages().DeleteAllPages();

            string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageMvcViewDestinationFilePath(PackageName, WidgetName, ViewFileName);
            
            FileInfo fi = new FileInfo(filePath);
            DirectoryInfo di = fi.Directory;
          
            FeatherServerOperations.ResourcePackages().DeleteDirectory(di.FullName);

            var folderPath = Path.Combine(this.SfPath, "Mvc", "Views", WidgetName);
            FeatherServerOperations.ResourcePackages().DeleteDirectory(folderPath);
        }

        private string SfPath
        {
            get
            {
                return System.Web.Hosting.HostingEnvironment.MapPath("~/");
            }
        }

        private const string PageName = "FeatherPage";
        private const string WidgetCaption = "TestMvcWidget";
        private const string BootstrapTemplate = "Bootstrap.default";
        private const string PlaceHolderId = "Contentplaceholder1";
        private const string PackageName = "Bootstrap";
        private const string ViewFileName = "Default.cshtml";
        private const string WidgetName = "MvcTest";
        private const string FileResource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.Default.cshtml";
        private const string LayoutFileResource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.TestView.Default.cshtml";
    }
}
