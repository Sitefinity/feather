using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.RelatedData;

namespace Telerik.Sitefinity.Frontend.Mvc.Models.Fields
{
    /// <summary>
    /// This class represents view model for related pages.
    /// </summary>
    public class RelatedPageViewModel : RelatedViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedPageViewModel"/> class.
        /// </summary>
        public RelatedPageViewModel()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedPageViewModel"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public RelatedPageViewModel(IDataItem item) : base(item)
        {
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get
            {
                var dynamicFieldContainer = (IDynamicFieldsContainer)this.Item;
                var title = dynamicFieldContainer.GetValue("Title").ToString();

                return title;
            }
        }

        /// <summary>
        /// Gets the default URL.
        /// </summary>
        /// <value>
        /// The default URL.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string DefaultUrl
        {
            get 
            {
                return ((object)this.Item).GetDefaultUrl();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the page should open in new window.
        /// </summary>
        /// <value>
        ///   <c>true</c> if should open new window; otherwise, <c>false</c>.
        /// </value>
        public bool OpenNewWindow
        {
            get 
            {
                var openNewWinow = false;

                var pageNode = this.Item as PageNode;
                if (pageNode != null)
                {
                    openNewWinow = pageNode.OpenNewWindow;
                }

                return openNewWinow;
            }
        }

        /// <summary>
        /// Gets the link target attribute.
        /// </summary>
        /// <value>
        /// The link target attribute.
        /// </value>
        public string LinkTargetAttribute
        {
            get
            {
                var attr = this.OpenNewWindow ? "target='_blank'" : string.Empty;

                return attr;
            }
        }
    }
}
