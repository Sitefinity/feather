using System;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Models;
using Telerik.Sitefinity.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Controllers
{
    /// <summary>
    /// Controllers that implement this interface should be able to display related data of given item.
    /// </summary>
    public interface IRelatedDataController
    {
        /// <summary>
        /// Action that returns view with the related items of the given item.
        /// </summary>
        /// <param name="relatedItem">The related item.</param>
        /// <param name="relatedDataViewModel">The related data view model.</param>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        ActionResult RelatedData(IDataItem relatedItem, RelatedDataViewModel relatedDataViewModel, int? page);
    }
}
