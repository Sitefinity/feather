(function () {
    angular.module('sfServices')
        .factory('sfDataService', ['serviceHelper', 'serverContext', function (serviceHelper, serverContext) {
            /* Private methods and variables */
            var serviceUrl = serverContext.getRootedUrl('Sitefinity/Services/DynamicModules/Data.svc/'),
                dataItemPromise;

            var getResource = function (itemId) {
                var url = serviceUrl;
                if (itemId && itemId !== serviceHelper.emptyGuid()) {
                    url = url + itemId + '/';
                }

                return serviceHelper.getResource(url);
            };

            var getLiveItems = function (itemType, provider, skip, take, search, searchField) {
                var filter = serviceHelper.filterBuilder()
                    .searchFilter(search, null, searchField)
                    .and()
                    .cultureFilter()
                    .getFilter();

                dataItemPromise = getResource('live').get(
                    {
                        itemType: itemType,
                        itemSurrogateType: itemType,
                        provider: provider,
                        skip: skip,
                        take: take,
                        filter: filter
                    })
                    .$promise;

                return dataItemPromise;
            };

            var getItems = function (itemType, provider, skip, take, search, searchField) {
                var filter = serviceHelper.filterBuilder()
                    .searchFilter(search, null, searchField)
                    .and()
                    .cultureFilter()
                    .getFilter();

                dataItemPromise = getResource().get(
                    {
                        itemType: itemType,
                        itemSurrogateType: itemType,
                        provider: provider,
                        skip: skip,
                        take: take,
                        filter: filter
                    })
                    .$promise;

                return dataItemPromise;
            };

            var getSpecificLiveItems = function (ids, itemType, provider) {
                var filter = serviceHelper.filterBuilder()
                    .specificItemsFilter(ids)
                    .and()
                    .cultureFilter()
                    .getFilter();

                dataItemPromise = getResource('live').get(
                    {
                        itemType: itemType,
                        itemSurrogateType: itemType,
                        provider: provider,
                        skip: 0,
                        take: 100,
                        filter: filter
                    })
                    .$promise;

                return dataItemPromise;
            };

            var getSpecificItems = function (ids, itemType, provider) {
                var filter = serviceHelper.filterBuilder()
                    .specificItemsFilter(ids)
                    .and()
                    .cultureFilter()
                    .getFilter();

                dataItemPromise = getResource().get(
                    {
                        itemType: itemType,
                        itemSurrogateType: itemType,
                        provider: provider,
                        skip: 0,
                        take: 100,
                        filter: filter
                    })
                    .$promise;

                return dataItemPromise;
            };

            var getItem = function (itemId, itemType, provider) {
                dataItemPromise = getResource(itemId).get(
                    {
                        itemType: itemType,
                        provider: provider
                    })
                    .$promise;

                return dataItemPromise;
            };

            return {
                /* Returns the data items. */
                getLiveItems: getLiveItems,
                getSpecificLiveItems: getSpecificLiveItems,
                getItems: getItems,
                getSpecificItems: getSpecificItems,
                getItem: getItem
            };
        }]);
})();