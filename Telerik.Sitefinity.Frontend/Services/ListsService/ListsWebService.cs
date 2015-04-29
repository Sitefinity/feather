using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ServiceStack;
using Telerik.Sitefinity.Frontend.Services.ListsService.DTO;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Lists.Model;
using Telerik.Sitefinity.Model.Localization;
using Telerik.Sitefinity.Modules.Lists;
using Telerik.Sitefinity.Modules.Lists.Web.Services;
using Telerik.Sitefinity.Modules.Lists.Web.Services.Data;
using Telerik.Sitefinity.Utilities;
using Telerik.Sitefinity.Web.Services;

namespace Telerik.Sitefinity.Frontend.Services.ListsService
{
    /// <summary>
    /// This class provides methods, which are not currently presented in Sitefinity ListService.
    /// </summary>
    internal class ListsWebService : Service
    {
        /// <summary>
        /// Get specifi lists based on provided ids and provider name.
        /// </summary>
        /// <param name="listsRequest">
        /// The list requests object from which the content ought to be retrieved.
        /// </param>
        /// <returns>        
        /// An enumerable oflist view model objects.
        /// </returns>
        [AddHeader(ContentType = MimeTypes.Json)]
        public CollectionContext<ListViewModel> Put(SpecificListsGetRequest listsRequest)
        {
            ServiceUtility.RequestBackendUserAuthentication();

            var listService = new ListService();

            var listManager = listService.GetManager(listsRequest.Provider);

            if (listsRequest.Ids != null && listsRequest.Ids.Length > 0)
            {
                var lists = listService.GetContentItems(listsRequest.Provider)
                                       .Where(l => listsRequest.Ids.Contains(l.Id) && l.Status == ContentLifecycleStatus.Master)
                                       .ToList();

                var liveContentDictionary = this.GetRelevantItemsList(lists, listManager, ContentLifecycleStatus.Live);
                var tempContentDictionary = this.GetRelevantItemsList(lists, listManager, ContentLifecycleStatus.Temp);

                var result = listService.GetViewModelList(lists, listManager.Provider, liveContentDictionary, tempContentDictionary).ToList();

                this.LoadLifecycleStatus(result, liveContentDictionary, tempContentDictionary, listManager);

                ServiceUtility.DisableCache();
                
                return new CollectionContext<ListViewModel>(result) { TotalCount = result.Count };
            }

            return new CollectionContext<ListViewModel>();
        }
 
        private void LoadLifecycleStatus(List<ListViewModel> result, IDictionary<Guid, List> liveContentDictionary, IDictionary<Guid, List> tempContentDictionary, ListsManager listManager)
        {
            CultureInfo culture = null;
            var currentCulture = culture.GetSitefinityCulture();

            foreach (var item in result)
            {
                List live = liveContentDictionary.GetValueOrDefault(item.Id);
                List temp = tempContentDictionary.GetValueOrDefault(item.Id);

                item.LifecycleStatus =
                    WcfContentLifecycleStatusFactory.Create<List>((List)item.ContentItem, listManager, live, temp, currentCulture);
            }
        }

        private IDictionary<Guid, List> GetRelevantItemsList(List<List> contentList, ListsManager manager, ContentLifecycleStatus status)
        {
            Dictionary<Guid, List> result = new Dictionary<Guid, List>();

            ////get all master content items ids
            var contentItemsIds = contentList.Select(cl => cl.Id).ToArray<Guid>();

            if (contentItemsIds.Length > 0)
            {
                ////get all items in the specified status related  to the master items in the list 
                var relatedContentItems = manager.GetItems<List>().Where(t => contentItemsIds.Contains(t.OriginalContentId) && t.Status == status);

                foreach (var item in relatedContentItems)
                    result.Add(item.OriginalContentId, item);
            }

            return result.Count > 0 ? result : null;
        }
    }
}
