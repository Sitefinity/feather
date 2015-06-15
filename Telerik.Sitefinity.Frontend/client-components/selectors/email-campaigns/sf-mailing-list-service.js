(function () {
    angular.module('sfServices')
        .factory('sfMailingListService', ['serviceHelper', 'serverContext', function (serviceHelper, serverContext) {
            /* Private methods and variables */
            var dataItemPromise,
                serviceUrl = serverContext.getRootedUrl('Sitefinity/Services/Newsletters/MailingList.svc/');

            var getResource = function (itemId) {
                var url = serviceUrl;
                if (itemId && itemId !== serviceHelper.emptyGuid()) {
                    url = url + itemId + '/';
                }

                return serviceHelper.getResource(url);
            };

            var getItems = function (provider, skip, take, search, sortExpression) {
                var filter = serviceHelper.filterBuilder()
                    .searchFilter(search)
                    .getFilter();                

                dataItemPromise = getResource().get(
                    {
                        provider: provider,
                        sortExpression: sortExpression,
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
                        provider: provider
                    })
                    .$promise;

                return dataItemPromise;
            };

            var getSpecificItems = function (ids, provider) {
                var filter = serviceHelper.filterBuilder()
                    .specificItemsFilter(ids)
                    .getFilter();

                dataItemPromise = getResource().get(
                    {
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