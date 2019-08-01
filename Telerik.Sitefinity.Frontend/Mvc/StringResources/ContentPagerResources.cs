using Telerik.Sitefinity.Localization;

namespace Telerik.Sitefinity.Frontend.Mvc.StringResources
{
    /// <summary>
    /// Localizable strings for the Widget designer.
    /// </summary>
    [ObjectInfo(typeof(ContentPagerResources), Title = "ContentPagerResourcesTitle", Description = "ContentPagerResourcesDescription")]
    public class ContentPagerResources : Resource
    {
        /// <summary>
        /// Title for the widgets content pager resources class.
        /// </summary>
        [ResourceEntry("ContentPagerResourcesTitle",
            Value = "Widgets content pager resources",
            Description = "Title for the widgets content pager resources class.",
            LastModified = "2014/05/20")]
        public string ContentPagerResourcesTitle
        {
            get
            {
                return this["ContentPagerResourcesTitle"];
            }
        }

        /// <summary>
        /// Description for the widgets content designer resources class.
        /// </summary>
        [ResourceEntry("ContentPagerResourcesDescription",
            Value = "Localizable strings for the widgets content pager.",
            Description = "Description for the widgets content pager resources class.",
            LastModified = "2018/09/13")]
        public string ContentPagerResourcesDescription
        {
            get
            {
                return this["ContentPagerResourcesDescription"];
            }
        }

        /// <summary>
        /// word: Pagination
        /// </summary>
        [ResourceEntry("Pagination",
            Value = "Pagination",
            Description = "word: Pagination",
            LastModified = "2018/09/13")]
        public string Pagination
        {
            get
            {
                return this["Pagination"];
            }
        }

        /// <summary>
        /// word: Page
        /// </summary>
        [ResourceEntry("Page",
            Value = "Page",
            Description = "word: Page",
            LastModified = "2018/09/13")]
        public string Page
        {
            get
            {
                return this["Page"];
            }
        }

        /// <summary>
        /// phrase: Go to previous page
        /// </summary>
        [ResourceEntry("GoToPreviousPage",
            Value = "GoToPreviousPage",
            Description = "phrase: Go to previous page",
            LastModified = "2018/09/13")]
        public string GoToPreviousPage
        {
            get
            {
                return this["GoToPreviousPage"];
            }
        }

        /// <summary>
        /// phrase: Go to page
        /// </summary>
        [ResourceEntry("GoToPage",
            Value = "Go to page",
            Description = "phrase: Go to page",
            LastModified = "2018/09/13")]
        public string GoToPage
        {
            get
            {
                return this["GoToPage"];
            }
        }

        /// <summary>
        /// phrase: Go to next page
        /// </summary>
        [ResourceEntry("GoToNextPage",
            Value = "Go to next page",
            Description = "phrase: Go to next page",
            LastModified = "2018/09/13")]
        public string GoToNextPage
        {
            get
            {
                return this["GoToNextPage"];
            }
        }
    }
}
