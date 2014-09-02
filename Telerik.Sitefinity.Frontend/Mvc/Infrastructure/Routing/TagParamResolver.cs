namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Instances of this class resolve tag by UrlName.
    /// </summary>
    public class TagParamResolver : TaxonParamResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TagParamResolver"/> class.
        /// </summary>
        public TagParamResolver() : base("Tags")
        {
        }
    }
}