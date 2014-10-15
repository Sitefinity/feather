(function () {
    angular.module('services')
        .factory('newsItemService', ['serviceHelper', 'serverContext', function (serviceHelper, serverContext) {
            /* Private methods and variables */
            var dataItemPromise,
                contentType = 'Telerik.Sitefinity.GenericContent.Model.Content',
                serviceUrl = serverContext.getRootedUrl('Sitefinity/Services/Content/NewsItemService.svc/');

            var getResource = function (itemId) {
                var url = serviceUrl;
                if (itemId && itemId !== serviceHelper.emptyGuid()) {
                    url = url + itemId + '/';
                }

                return serviceHelper.getResource(url);
            };            

            var getItems = function (provider, skip, take, search) {
                var filter = serviceHelper.filterBuilder()
                    .lifecycleFilter()
                    .and()
                    .cultureFilter()
                    .and()
                    .searchFilter(search)
                    .getFilter();

                dataItemPromise = getResource().get(
                    {
                        itemType: contentType,
                        itemSurrogateType: contentType,
                        provider: provider,
                        skip: skip,
                        take: take,
                        filter: filter
                    })
                    .$promise;

                return dataItemPromise;
            };

            var getItem = function (itemId, provider) {
                dataItemPromise = getResource(itemId).get(
                    {
                        provider: provider,
                        published: true
                    })
                    .$promise;

                return dataItemPromise;
            };

            var getSpecificItems = function (ids, provider) {
                var filter = serviceHelper.filterBuilder()
                    .lifecycleFilter()
                    .and()
                    .cultureFilter()
                    .and()
                    .specificItemsFilter(ids)
                    .getFilter();

                dataItemPromise = getResource().get(
                    {
                        itemType: contentType,
                        itemSurrogateType: contentType,
                        provider: provider,
                        skip: 0,
                        take: 100,
                        filter: filter
                    })
                    .$promise;

                return dataItemPromise;
            };

            return {
                /* Returns the data items. */
                getItems: getItems,
                getSpecificItems: getSpecificItems,
                getItem: getItem
            };
        }]);
})();