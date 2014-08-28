using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        {
            this.CurrentPage = currentPage;
            this.TotalPagesCount = totalPagesCount;
            this.RedirectUrlTemplate = redirectUrlTemplate;
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
        /// Gets or sets the redirect URL template.
        /// </summary>
        /// <value>
        /// The redirect URL template.
        /// </value>
        public string RedirectUrlTemplate { get; set; }
    }
}
