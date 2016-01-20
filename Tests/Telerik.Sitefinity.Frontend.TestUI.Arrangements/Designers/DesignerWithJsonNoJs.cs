using System;
using System.IO;
using Telerik.Sitefinity.Frontend.TestUI.Arrangements.MvcWidgets;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.TestArrangementService.Attributes;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
using Telerik.Sitefinity.TestUtilities.CommonOperations;
using MvcServerOperations = Telerik.Sitefinity.Mvc.TestUtilities.CommonOperations.ServerOperations;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements
{
    /// <summary>
    /// DesignerWithJsonNoJs arrangement.
    /// </summary>
    public class DesignerWithJsonNoJs : ITestArrangement
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

            FeatherServerOperations.ResourcePackages().ImportDataForSelectorsTests(FileResource, DesignerViewFileName, FileResourceJson, JsonFileName, null, null);

            MvcServerOperations.Widgets().AddMvcWidgetToPage(pageId, typeof(DummyTextController).FullName, WidgetCaption);
        }

        [ServerTearDown]
        public void TearDown()
        {
            ServerOperations.Pages().DeleteAllPages();
            ServerOperations.News().DeleteAllNews();
            ServerOperations.Taxonomies().ClearAllTags(TaxonomiesConstants.TagsTaxonomyId);

            FeatherServerOperations.ResourcePackages().DeleteSelectorsData(DesignerViewFileName, JsonFileName, null);
        }

        private const string FileResource = "Telerik.Sitefinity.Frontend.TestUI.Arrangements.Data.DesignerView.Selector.cshtml";
        private const string FileResourceJson = "Telerik.Sitefinity.Frontend.TestUI.Arrangements.Data.DesignerView.Selector.json";

        private const string DesignerViewFileName = "DesignerView.Selector.cshtml";
        private const string JsonFileName = "DesignerView.Selector.json";

        private const string PageName = "FeatherPage";
        private const string WidgetCaption = "SelectorWidget";

        private const string NewsItemTitle = "News Item Title";
        private const string NewsItemContent = "This is a news item.";
        private const string NewsItemAuthor = "NewsWriter";

        private const string TagTitle = "Tag Title";
    }
}
