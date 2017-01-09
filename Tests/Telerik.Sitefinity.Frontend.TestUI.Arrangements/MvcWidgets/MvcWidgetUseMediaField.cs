using System;
using System.IO;
using SitefinityWebApp.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers;
using Telerik.Sitefinity.TestArrangementService.Attributes;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements
{
    public class MvcWidgetUseMediaField : ITestArrangement
    {
        [ServerSetUp]
        public void SetUp()
        {
            ServerOperations.Documents().Upload(DefaultLibraryTitle, DocumentTitle, DocumentResource);

            Guid templateId = ServerOperations.Templates().GetTemplateIdByTitle(BootstrapTemplate);
            Guid pageId = ServerOperations.Pages().CreatePage(PageName, templateId);
            Guid pageNodeId = ServerOperations.Pages().GetPageNodeId(pageId);
            FeatherServerOperations.Pages().AddMvcWidgetToPage(pageNodeId, typeof(TestMediaSelectorFieldController).FullName, WidgetCaption, PlaceHolderId);
        }        

        [ServerTearDown]
        public void TearDown()
        {
            ServerOperations.Documents().DeleteAllDocuments();
            ServerOperations.Pages().DeleteAllPages();
        }

        private const string PageName = "FeatherPage";
        private const string WidgetCaption = "MediaWidget";
        private const string BootstrapTemplate = "Bootstrap.default";
        private const string PlaceHolderId = "Contentplaceholder1";
        private const string DocumentTitle = "Document2";
        private const string DocumentResource = "Telerik.Sitefinity.TestUtilities.Data.Documents.Document2.pdf";
        private const string DefaultLibraryTitle = "Default Library";
    }
}
