using System.Globalization;
using System.Linq;
using MbUnit.Framework;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Models;
using Telerik.Sitefinity.News.Model;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace Telerik.Sitefinity.Frontend.TestIntegration.Mvc.Models
{
    /// <summary>
    /// This class contains tests for the ContentModelBase class.
    /// </summary>
    [TestFixture]
    [Category(TestCategories.MvcCore)]
    [Description("This class contains tests for the ContentModelBase class.")]
    public class ContentModelBaseTests
    {
        [SetUp]
        public void SetUp()
        {
            ServerOperations.Taxonomies().CreateTag(ContentModelBaseTests.TagName);

            for (var i = 0; i < 10; i++)
            {
                var title = "news" + i.ToString(CultureInfo.InvariantCulture);

                // Tag is automatically linked.
                ServerOperations.News().CreateNewsItem(title);
            }
        }

        [TearDown]
        public void TearDown()
        {
            ServerOperations.News().DeleteAllNews();
            ServerOperations.Taxonomies().DeleteTags(new[] { ContentModelBaseTests.TagName });
        }

        /// <summary>
        /// Ensures that the ContentModelBase retrieves the correct data when second page of filtered by taxon items are requested.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Ensures that the ContentModelBase retrieves the correct data when second page of filtered by taxon items are requested.")]
        public void CreateListViewModel_SecondPageTaxonFilteredNews_ContainsCorrectData()
        {
            var newsContentModel = new NewsContentModel();
            newsContentModel.ContentType = typeof(NewsItem);
            newsContentModel.DisplayMode = Frontend.Mvc.Models.ListDisplayMode.Paging;
            newsContentModel.ItemsPerPage = 5;

            var tag = TaxonomyManager.GetManager().GetTaxa<FlatTaxon>().First();
            var viewModel = newsContentModel.CreateListViewModel(tag, page: 2);

            Assert.IsNotNull(viewModel, "CreateListViewModel returned null.");
            Assert.AreEqual(5, viewModel.Items.Count(), "Returned items does not have the expected count.");
            Assert.IsTrue(viewModel.ShowPager, "ShowPager is false when it was expected to be true.");
            Assert.AreEqual(2, viewModel.CurrentPage, "CurrentPage is not the requested one.");
            Assert.AreEqual(2, viewModel.TotalPagesCount, "TotalPagesCount is not correct.");
        }

        private const string TagName = "newsTag";
    }
}
