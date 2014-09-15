namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Instances of this class resolve category by UrlName.
    /// </summary>
    internal class CategoryParamResolver : TaxonParamResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryParamResolver"/> class.
        /// </summary>
        public CategoryParamResolver() : base("Categories")
        {
        }
    }
}