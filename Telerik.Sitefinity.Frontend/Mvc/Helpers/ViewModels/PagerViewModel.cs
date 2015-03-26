using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Web.UI;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers.ViewModels
{
    /// <summary>
    /// This class represents the view model for the pagers.
    /// </summary>
    public class PagerViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PagerViewModel"/> class.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <param name="totalPagesCount">The total pages count.</param>
        /// <param name="redirectUrlTemplate">The redirect URL template.</param>
        public PagerViewModel(int currentPage, int totalPagesCount, string redirectUrlTemplate)
            : this(currentPage, totalPagesCount, redirectUrlTemplate, 10)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagerViewModel"/> class.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <param name="totalPagesCount">The total pages count.</param>
        /// <param name="redirectUrlTemplate">The redirect URL template.</param>
        /// <param name="redirectUrlTemplate">The amount of page nodes to render</param>
        public PagerViewModel(int currentPage, int totalPagesCount, string redirectUrlTemplate, int displayCount)
        {
            this.CurrentPage = currentPage;
            this.TotalPagesCount = totalPagesCount;
            this.RedirectUrlTemplate = redirectUrlTemplate;
            this.DisplayCount = displayCount;
        }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        /// <value>
        /// The current page.
        /// </value>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the total pages count.
        /// </summary>
        /// <value>
        /// The total pages count.
        /// </value>
        public int TotalPagesCount { get; set; }

        /// <summary>
        /// Gets or sets the total item count to render.
        /// </summary>
        /// <value>
        /// The total numbers that will render.
        /// </value>
        public int DisplayCount { get; set; }

        /// <summary>
        /// Gets or sets the redirect URL template.
        /// </summary>
        /// <value>
        /// The redirect URL template.
        /// </value>
        public string RedirectUrlTemplate { get; set; }

        /// <summary>
        /// Gets or sets the pager node
        /// </summary>
        /// <value>
        /// The amount of nodes to render
        /// </value>
        public IList<Pager.PagerNumericItem> PagerNodes
        {
            get
            {
                if (this.pagerNodes == null)
                    this.pagerNodes = new List<Pager.PagerNumericItem>();

                return this.pagerNodes;
            }
        }

        /// <summary>
        /// Gets or sets the previous node
        /// </summary>
        /// <value>
        /// The previous page
        /// </value>
        public Pager.PagerNumericItem PreviousNode { get; set; }

        /// <summary>
        /// Gets or sets the next node
        /// </summary>
        /// <value>
        /// The next page
        /// </value>
        public Pager.PagerNumericItem NextNode { get; set; }

        private IList<Pager.PagerNumericItem> pagerNodes;
    }
}
