using System;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing;
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

        /// <summary>
        /// Returns false when route params are empty collection.
        /// </summary>
        [TestMethod]
        [Owner("Manev")]
        [Description("Returns false when route params are empty collection.")]
        public void Return_False_When_Params_Are_Empty_Test()
        {
            var taxonUrlMapper = new TaxonUrlMapper(new TestableTaxonUrlEvaluatorAdapter());

            ITaxon taxon;
            int pageIndex;

            bool hasMatch = taxonUrlMapper.TryMatch(new string[1], out taxon, out pageIndex);

            Assert.IsFalse(hasMatch);
        }

        /// <summary>
        /// Returns false when route params has less then tree items.
        /// </summary>
        [TestMethod]
        [Owner("Manev")]
        [Description("Returns false when route params has less then tree items.")]
        public void Return_False_When_Params_Has_Less_Then_Tree_Items()
        {
            var taxonUrlMapper = new TaxonUrlMapper(new TestableTaxonUrlEvaluatorAdapter());

            ITaxon taxon;
            int pageIndex;

            var urlParams = new[] { "1", "2" };

            bool hasMatch = taxonUrlMapper.TryMatch(urlParams, out taxon, out pageIndex);

            Assert.IsFalse(hasMatch);
        }

        /// <summary>
        /// Returns true with a valid taxon.
        /// </summary>
        [TestMethod]
        [Owner("Manev")]
        [Description("Returns true with a valid taxon.")]
        public void Return_True_With_Valid_Taxon()
        {
            var tagTaxon = new FlatTaxon();

            var taxonUrlMapper = new TaxonUrlMapper(new TestableTaxonUrlEvaluatorAdapter(tagTaxon));

            ITaxon taxon;
            int pageIndex;

            var urlParams = new[] { "-in-tags", "tag", "tag1" };

            bool hasMatch = taxonUrlMapper.TryMatch(urlParams, out taxon, out pageIndex);

            Assert.IsFalse(hasMatch);
        }

        #endregion

        private class TestableTaxonUrlEvaluatorAdapter : ITaxonUrlEvaluatorAdapter
        {
            private readonly ITaxon initTaxon;

            public TestableTaxonUrlEvaluatorAdapter()
            {
            }

            public TestableTaxonUrlEvaluatorAdapter(ITaxon taxon)
            {
                this.initTaxon = taxon;
            }

            public bool TryGetTaxonFromUrl(string url, out ITaxon taxon)
            {
                taxon = this.initTaxon;

                return taxon != null;
            }
        }
    }
}