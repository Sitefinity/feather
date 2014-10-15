using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements.MvcWidgets
{
    /// <summary>
    /// This class represents a dummy MVC widget which has an action that returns simple text.
    /// </summary>
    public class DummyTextController : Controller
    {
        /// <summary>
        /// Gets or sets the dummy text.
        /// </summary>
        /// <value>
        /// The dummy text.
        /// </value>
        public string DummyText { get; set; }

        /// <summary>
        /// Gets or sets the selected id news item.
        /// </summary>
        /// <value>The selected id news item.</value>
        public string SelectedIdNewsItem { get; set; }

        /// <summary>
        /// Gets or sets the selected id taxon item.
        /// </summary>
        /// <value>The selected id taxon item.</value>
        public string SelectedIdTaxonItem { get; set; }

        /// <summary>
        /// Gets or sets the selected id dynamic item.
        /// </summary>
        /// <value>The selected id dynamic item.</value>
        public string SelectedIdDynamicItem { get; set; }

        /// <summary>
        /// Gets or sets the selected news multi.
        /// </summary>
        /// <value>The selected news multi.</value>
        public string SelectedIdsNewsItems { get; set; }

        /// <summary>
        /// Gets or sets the selected ids tags.
        /// </summary>
        /// <value>The selected ids tags.</value>
        public string SelectedIdsTags { get; set; }

        /// <summary>
        /// Gets or sets the selected ids dynamic items.
        /// </summary>
        /// <value>The selected ids dynamic items.</value>
        public string SelectedIdsDynamicItems { get; set; }

        /// <summary>
        /// Gets the test dynamic modules.
        /// </summary>
        /// <value>The test dynamic modules.</value>
        public string TestDynamicModules
        {
            get
            {
                return this.testDynamicModules;
            }
        }

        /// <summary>
        /// Gets or sets the selected news.
        /// </summary>
        /// <value>The selected news.</value>
        public string SelectedNews
        {
            get
            {
                return this.selectedNews;
            }

            set
            {
                this.selectedNews = value;
            }
        }

        /// <summary>
        /// Gets the dummy sample text
        /// </summary>
        /// <returns>The dummy text.</returns>
        public string Index()
        {
            return this.DummyText;
        }

        private readonly string testDynamicModules = "Telerik.Sitefinity.DynamicTypes.Model.DuplicateRelatedDataModule.Duplicaterelateddata";
        private string selectedNews = string.Empty;
    }
}
