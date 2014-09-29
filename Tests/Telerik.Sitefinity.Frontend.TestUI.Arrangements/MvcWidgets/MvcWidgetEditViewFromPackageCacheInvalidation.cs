using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
using Telerik.Sitefinity.TestUI.Arrangements.Framework.Attributes;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements
{
    public class MvcWidgetEditViewFromPackageCacheInvalidation : ITestArrangement
    {
        private const string PageName = "FeatherPage";
        private const string WidgetCaption = "TestMvcWidget";
        private const string BootstrapTemplate = "Bootstrap.default";
        private const string PlaceHolderId = "Contentplaceholder1";
        private const string PackageName = "Bootstrap";
        private const string ViewFileName = "Default.cshtml";
        private const string WidgetName = "MvcTest";
        private const string FileResource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.Default.cshtml";
        private const string TextToReplace = "This is a view from package.";
        private const string NewText = "This is a view from package after edit.";
        private const string UserName = "admin";
        private const string Password = "admin@2";

        [ServerSetUp]
        public void SetUp()
        {
            AuthenticationHelper.AuthenticateUser(UserName, Password);

            Guid templateId = ServerOperations.Templates().GetTemplateIdByTitle(BootstrapTemplate);
            Guid pageId = ServerOperations.Pages().CreatePage(PageName, templateId);
            Guid pageNodeId = ServerOperations.Pages().GetPageNodeId(pageId);

            FeatherServerOperations.Pages().AddMvcWidgetToPage(pageNodeId, typeof(MvcTestController).FullName, WidgetCaption, PlaceHolderId);

            string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageMvcViewDestinationFilePath(PackageName, WidgetName, ViewFileName);
            FeatherServerOperations.ResourcePackages().AddNewResource(FileResource, filePath);
        }

        [ServerArrangement]
        public void EditViewFromPackage()
        {
            string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageMvcViewDestinationFilePath(PackageName, WidgetName, ViewFileName);

            FeatherServerOperations.ResourcePackages().EditLayoutFile(filePath, TextToReplace, NewText);
        }

        [ServerTearDown]
        public void TearDown()
        {
            AuthenticationHelper.AuthenticateUser(UserName, Password);

            ServerOperations.Pages().DeleteAllPages();

            string filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageMvcViewDestinationFilePath(PackageName, WidgetName, ViewFileName);

            FileInfo fi = new FileInfo(filePath);
            DirectoryInfo di = fi.Directory;

            FeatherServerOperations.ResourcePackages().DeleteDirectory(di.FullName);
        }
    }
}
