using System.Linq;
using Telerik.Sitefinity.Frontend.Mvc.Models;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules.News;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Models
{
    /// <summary>
    /// Content model for news items. Used for testing purposes.
    /// </summary>
    public class NewsContentModel : ContentModelBase
    {
        /// <summary>
        /// Gets an active query for all items.
        /// </summary>
        /// <returns>
        /// The query.
        /// </returns>
        protected override IQueryable<IDataItem> GetItemsQuery()
        {
            return NewsManager.GetManager().GetNewsItems();
        }
    }
}
