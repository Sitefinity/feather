using System;
using System.IO;
using Telerik.Sitefinity.Frontend.TestUI.Arrangements.MvcWidgets;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
using Telerik.Sitefinity.TestUI.Arrangements.Framework.Attributes;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements
{
    /// <summary>
    /// MvcSelectorTest arragement.
    /// </summary>
    public class MvcSelectorTest : ITestArrangement
    {
        [ServerSetUp]
        public void SetUp()
        {
            for (int i = 0; i < 3; i++)
            {
                ServerOperations.News().CreatePublishedNewsItem(newsTitle: NewsItemTitle + i, newsContent: NewsItemContent + i, author: NewsItemAuthor + i);                
            }
            
            for (int i = 0; i < 4; i++)
            {
                ServerOperations.Taxonomies().CreateTag(TagTitle + i);
            }

            Guid pageId = ServerOperations.Pages().CreatePage(PageName);

            FeatherServerOperations.ResourcePackages().ImportDataForSelectorsTests(FileResource, DesignerViewFileName, FileResourceJson, JsonFileName, ControllerFileResource, ControllerFileName);            

            ServerOperations.Widgets().AddMvcWidgetToPage(pageId, typeof(DummyTextController).FullName, WidgetCaption);
        }
 
        [ServerTearDown]
        public void TearDown()
        {
            ServerOperations.Pages().DeleteAllPages();
            ServerOperations.News().DeleteAllNews();
            ServerOperations.Taxonomies().ClearAllTags(TaxonomiesConstants.TagsTaxonomyId);

            FeatherServerOperations.ResourcePackages().DeleteSelectorsData(DesignerViewFileName, JsonFileName, ControllerFileName);
        }
     
        private const string FileResource = "Telerik.Sitefinity.Frontend.TestUI.Arrangements.Data.DesignerView.Selector.cshtml";
        private const string FileResourceJson = "Telerik.Sitefinity.Frontend.TestUI.Arrangements.Data.DesignerView.Selector.json";
        private const string ControllerFileResource = "Telerik.Sitefinity.Frontend.TestUI.Arrangements.Data.designerview-selector.js";

        private const string DesignerViewFileName = "DesignerView.Selector.cshtml";
        private const string JsonFileName = "DesignerView.Selector.json";
        private const string ControllerFileName = "designerview-selector.js";

        private const string PageName = "FeatherPage";
        private const string WidgetCaption = "SelectorWidget";

        private const string NewsItemTitle = "News Item Title";
        private const string NewsItemContent = "This is a news item.";
        private const string NewsItemAuthor = "NewsWriter";

        private const string TagTitle = "Tag Title";
    }
}
