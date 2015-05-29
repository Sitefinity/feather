using System;
using System.Web.Mvc;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing;
using Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers;
using Telerik.Sitefinity.Taxonomies.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Test.Helpers
{
    /// <summary>
    /// Tests methods of the TaxonUrlMapper class.
    /// </summary>
    [TestClass]
    public class TaxonUrlMapperTests
    {
        #region Public Methods and Operators
        [TestMethod]
        [Owner("Manev")]
        [Description("Returns false when route params are empty collection.")]
        public void Return_False_When_Params_Are_Empty_Test()
        {
            var taxonUrlMapper = new TaxonUrlMapper(new MockedTaxonUrlEvaluatorAdapter());

            ITaxon taxon;
            int pageIndex;

            bool hasMatch = taxonUrlMapper.TryMatch(new string[1], out taxon, out pageIndex);

            Assert.IsFalse(hasMatch);
        }

        [TestMethod]
        [Owner("Manev")]
        [Description("Returns false when route params has less then tree items - [-in-tags/tag")]
        public void Return_False_When_Params_Has_Less_Then_Tree_Items()
        {
            var taxonUrlMapper = new TaxonUrlMapper(new MockedTaxonUrlEvaluatorAdapter());

            ITaxon taxon;
            int pageIndex;

            var urlParams = new[] { "-in-tags", "tag" };

            bool hasMatch = taxonUrlMapper.TryMatch(urlParams, out taxon, out pageIndex);

            Assert.IsFalse(hasMatch);
        }

        [TestMethod]
        [Owner("Manev")]
        [Description("Returns true with a valid taxon - [-in-tags/tag/tag1]")]
        public void Return_True_With_Valid_Taxon()
        {
            ITaxon taxon;
            int pageIndex;

            string urlPattern = "-in-tags/tag/tag1";

            var urlParams = urlPattern.Split('/');

            var tagTaxon = new FlatTaxon();

            var taxonUrlMapper = new TaxonUrlMapper(new MockedTaxonUrlEvaluatorAdapter(url => url.Contains(urlPattern) ? tagTaxon : null));

            bool hasMatch = taxonUrlMapper.TryMatch(urlParams, out taxon, out pageIndex);

            Assert.IsTrue(hasMatch);
            Assert.IsTrue(pageIndex == 0);
        }

        [TestMethod]
        [Owner("Manev")]
        [Description("Returns true with a valid taxon and pageIndex equals to 3 -[-in-tags/tag/tag1/3]")]
        public void Return_True_With_Valid_Taxon_And_PageIndex_Equals_To_3()
        {
            ITaxon taxon;
            int pageIndex;

            string urlWithTaxon = "-in-tags/tag/tag1";

            string urlPattern = "-in-tags/tag/tag1/3";

            var urlParams = urlPattern.Split('/');

            var tagTaxon = new FlatTaxon();

            var taxonUrlMapper = new TaxonUrlMapper(new MockedTaxonUrlEvaluatorAdapter(url => url.Contains(urlWithTaxon) ? tagTaxon : null));

            bool hasMatch = taxonUrlMapper.TryMatch(urlParams, out taxon, out pageIndex);

            Assert.IsTrue(hasMatch);
            Assert.IsTrue(pageIndex == 3);
        }

        [TestMethod]
        [Owner("Manev")]
        [Description("Returns false with a valid taxon - [-in-tags/tag/tag1/page]")]
        public void Return_False_With_Valid_Taxon_With_Page()
        {
            ITaxon taxon;
            int pageIndex;

            string validUrlPattern = "-in-tags/tag/tag1";

            string requestedUrl = "-in-tags/tag/tag1/page";

            var urlParams = requestedUrl.Split('/');

            var tagTaxon = new FlatTaxon();

            var taxonUrlMapper = new TaxonUrlMapper(new MockedTaxonUrlEvaluatorAdapter(url => url.Contains(validUrlPattern) ? tagTaxon : null));

            bool hasMatch = taxonUrlMapper.TryMatch(urlParams, out taxon, out pageIndex);

            Assert.IsFalse(hasMatch);
            Assert.IsTrue(pageIndex == 0);
        }

        [TestMethod]
        [Owner("Manev")]
        [Description("Returns false with a valid taxon - [-in-tags/tag/tag1/1/page]")]
        public void Return_False_With_Valid_Taxon_With_PageIndex_PageUrl()
        {
            ITaxon taxon;
            int pageIndex;

            string validUrlPattern = "-in-tags/tag/tag1";

            string requestedUrl = "-in-tags/tag/tag1/1/page";

            var urlParams = requestedUrl.Split('/');

            var tagTaxon = new FlatTaxon();

            var taxonUrlMapper = new TaxonUrlMapper(new MockedTaxonUrlEvaluatorAdapter(url => url.Contains(validUrlPattern) ? tagTaxon : null));

            bool hasMatch = taxonUrlMapper.TryMatch(urlParams, out taxon, out pageIndex);

            Assert.IsFalse(hasMatch);
            Assert.IsTrue(pageIndex == 0);
        }

        [TestMethod]
        [Owner("Manev")]
        [Description("Returns false with a valid taxon - [-in-tags/tag/tag1/1/page/1]")]
        public void Return_False_With_Valid_Taxon_With_PageIndex_PageUrl_PageIndex()
        {
            ITaxon taxon;
            int pageIndex;

            string validUrlPattern = "-in-tags/tag/tag1";

            string requestedUrl = "-in-tags/tag/tag1/1/page/1";

            var urlParams = requestedUrl.Split('/');

            var tagTaxon = new FlatTaxon();

            var taxonUrlMapper = new TaxonUrlMapper(new MockedTaxonUrlEvaluatorAdapter(url => url.Contains(validUrlPattern) ? tagTaxon : null));

            bool hasMatch = taxonUrlMapper.TryMatch(urlParams, out taxon, out pageIndex);

            Assert.IsFalse(hasMatch);
        }

        [TestMethod]
        [Owner("Manev")]
        [Description("Returns false with a valid taxon - [-in-tags/tag/tag/page/1/page]")]
        public void Return_False_With_Valid_Taxon_With_PageUrl_PageIndex_PageUrl()
        {
            ITaxon taxon;
            int pageIndex;

            string validUrlPattern = "-in-tags/tag/tag1";

            string requestedUrl = "-in-tags/tag/tag1/page/1/page";

            var urlParams = requestedUrl.Split('/');

            var tagTaxon = new FlatTaxon();

            var taxonUrlMapper = new TaxonUrlMapper(new MockedTaxonUrlEvaluatorAdapter(url => url.Contains(validUrlPattern) ? tagTaxon : null));

            bool hasMatch = taxonUrlMapper.TryMatch(urlParams, out taxon, out pageIndex);

            Assert.IsFalse(hasMatch);
        }

        [TestMethod]
        [Owner("Manev")]
        [Description("Returns true with a valid taxon - [-in-category/category/cat1/cat11/cat22/cat33/cat44]")]
        public void Return_True_With_Valid_HierarchicalTaxon_With_Deep_Hierarchy()
        {
            ITaxon taxon;
            int pageIndex;

            string requestedUrl = "-in-category/category/cat1/cat11/cat22/cat33/cat44";

            var urlParams = requestedUrl.Split('/');

            var tagTaxon = new HierarchicalTaxon();

            var taxonUrlMapper = new TaxonUrlMapper(new MockedTaxonUrlEvaluatorAdapter(url => url.Contains(requestedUrl) ? tagTaxon : null));

            bool hasMatch = taxonUrlMapper.TryMatch(urlParams, out taxon, out pageIndex);

            Assert.IsTrue(hasMatch);
        }

        [TestMethod]
        [Owner("Manev")]
        [Description("Returns false with a valid taxon - [-in-category/category/cat1/cat11/cat22/cat33/cat44/3]")]
        public void Return_False_With_Valid_HierarchicalTaxon_With_Deep_Hierarchy_PageIndex()
        {
            ITaxon taxon;
            int pageIndex;

            string validTaxonUrl = "-in-category/category/cat1/cat11/cat22/cat33/cat44";

            string requestedUrl = "-in-category/category/cat1/cat11/cat22/cat33/cat44/2";

            var urlParams = requestedUrl.Split('/');

            var tagTaxon = new HierarchicalTaxon();

            var taxonUrlMapper = new TaxonUrlMapper(new MockedTaxonUrlEvaluatorAdapter(url => url == validTaxonUrl ? tagTaxon : null));

            bool hasMatch = taxonUrlMapper.TryMatch(urlParams, out taxon, out pageIndex);

            Assert.IsFalse(hasMatch);
        }

        [TestMethod]
        [Owner("Manev")]
        [Description("Taxonomies the URL params mapper test.")]
        public void Taxonomy_UrlParams_Mapper_Test()
        {
            string urlPattern = "-in-tags/tag/tag1";

            var urlParams = urlPattern.Split('/');

            var controller = new TestableController();
            controller.ControllerContext = new ControllerContext();

            var tagTaxon = new FlatTaxon();

            var taxonomyUrlParamsMapper = new TaxonomyUrlParamsMapper(
                                          controller,
                                          new TaxonUrlMapper(new MockedTaxonUrlEvaluatorAdapter(url => url.Contains(urlPattern) ? tagTaxon : null)));

            taxonomyUrlParamsMapper.ResolveUrlParams(urlParams, controller.ControllerContext.RequestContext);

            Assert.IsTrue(controller.ControllerContext.RequestContext.RouteData.Values["action"] == "ListByTaxon");
            Assert.IsTrue(controller.ControllerContext.RequestContext.RouteData.Values["taxon"] == tagTaxon);
        }

        [TestMethod]
        [Owner("Manev")]
        [Description("Taxonomies the URL params mapper test with page index.")]
        public void Taxonomy_UrlParams_Mapper_Test_With_PageIndex()
        {
            string urlPattern = "-in-tags/tag/tag1/3";

            var urlParams = urlPattern.Split('/');

            var controller = new TestableController();
            controller.ControllerContext = new ControllerContext();

            var tagTaxon = new FlatTaxon();

            var taxonomyUrlParamsMapper = new TaxonomyUrlParamsMapper(
                                          controller,
                                          new TaxonUrlMapper(new MockedTaxonUrlEvaluatorAdapter(url => url.Contains(urlPattern) ? tagTaxon : null)));

            taxonomyUrlParamsMapper.ResolveUrlParams(urlParams, controller.ControllerContext.RequestContext);

            Assert.IsTrue(controller.ControllerContext.RequestContext.RouteData.Values["action"] == "ListByTaxon");
            Assert.IsTrue(controller.ControllerContext.RequestContext.RouteData.Values["taxon"] == tagTaxon);
            Assert.IsTrue(((int)controller.ControllerContext.RequestContext.RouteData.Values["page"]) == 3);
        }

        [TestMethod]
        [Owner("Manev")]
        [Description("Taxonomies the URL params mapper test with page index.")]
        public void Taxonomy_UrlParams_Mapper_Test_With_PageIndex_With_Incorrect_Route_Data()
        {
            string urlPattern = "-in-tags/tag/tag1/3";

            string requestRouteData = "-in-tags/tag/tag1/asdasd";

            var urlParams = requestRouteData.Split('/');

            var controller = new TestableController();
            controller.ControllerContext = new ControllerContext();

            var tagTaxon = new FlatTaxon();

            var taxonomyUrlParamsMapper = new TaxonomyUrlParamsMapper(
                                          controller,
                                          new TaxonUrlMapper(new MockedTaxonUrlEvaluatorAdapter(url => url.Contains(urlPattern) ? tagTaxon : null)));

            taxonomyUrlParamsMapper.ResolveUrlParams(urlParams, controller.ControllerContext.RequestContext);

            Assert.IsFalse(controller.ControllerContext.RequestContext.RouteData.Values["action"] == "ListByTaxon");
        }

        #endregion

        private class MockedTaxonUrlEvaluatorAdapter : ITaxonUrlEvaluatorAdapter
        {
            private readonly Func<string, ITaxon> matchFunc;

            public MockedTaxonUrlEvaluatorAdapter()
            {
            }

            public MockedTaxonUrlEvaluatorAdapter(Func<string, ITaxon> matchFunc)
            {
                this.matchFunc = matchFunc;
            }

            public bool TryGetTaxonFromUrl(string url, out ITaxon taxon)
            {
                taxon = this.matchFunc(url);

                return taxon != null;
            }
        }

        private class TestableController : Controller
        {
            /// <summary>
            /// Lists the by taxon.
            /// </summary>
            /// <param name="taxon">The taxon.</param>
            /// <param name="page">The page.</param>
            /// <returns></returns>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "page"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "taxon")]
            public ViewResult ListByTaxon(ITaxon taxon, int? page)
            {
                return new ViewResult();
            }
        }
    }
}