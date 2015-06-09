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
        /// <param name="detailPageId">The id of the page where the detail of the item will be shown.</param>
        /// <param name="openInSamePage">Determines whether the detail of the item will be on the same page or in a other one.</param>
        /// <param name="templateName">Name of the template.</param>
        /// <param name="relatedDataViewModel">The related data view model. Contains settings needed to retrieve related data.</param>
        /// <param name="settingsViewModel">The settings view model. The widget's model will be initialized with them.</param>
        /// <param name="page">The current page.</param>
        /// <returns></returns>
        ActionResult RelatedData(
            IDataItem relatedItem,
            Guid detailPageId,
            bool openInSamePage,
            string templateName,
            RelatedDataViewModel relatedDataViewModel,
            ContentListSettingsViewModel settingsViewModel,
            int? page);
    }
}
