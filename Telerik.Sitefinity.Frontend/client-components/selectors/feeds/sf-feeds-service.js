(function () {
    angular.module('sfServices')
        .factory('sfFeedsService', ['serviceHelper', 'serverContext', function (serviceHelper, serverContext) {
            /* Private methods and variables */

            var defaultProviderName = "OAPublishingProvider";
            var defaultPipeName = "RSSOutboundPipe";
            var serviceUrl = serverContext.getRootedUrl('Sitefinity/Services/Publishing/PublishingService.svc/');

            var getResource = function (uriTemplate) {
                var url = serviceUrl;

                if (uriTemplate) {
                    url += uriTemplate + '/';
                }

                return serviceHelper.getResource(url);
            };

            var mapPromiseItemId = function (promise) {
                promise.then(function (data) {
                    data.Items.map(function (item) {
                        item.Id = item.ID;
                    });
                });

                return promise;
            };

            var getFeeds = function (provider, skip, take, search, sort) {
                var filter = serviceHelper.filterBuilder()
                    .searchFilter(search, '', 'PublishingPoint.Name')
                    .getFilter();

                var promise = getResource('pipes').get({
                    providerName: provider || defaultProviderName,
                    pipeTypeName: defaultPipeName,
                    sort: sort,
                    skip: skip,
                    take: take,
                    filter: filter
                }).$promise;

                return mapPromiseItemId(promise);
            };

            var getSpecificFeed = function (id, provider) {
                var promise = getResource('pipes').get({
                    providerName: provider || defaultProviderName,
                    pipeTypeName: defaultPipeName,
                    filter: '(Id.Equals("' + id + '"))'
                }).$promise;

                return mapPromiseItemId(promise);
            };

            return {
                /* Returns the data items. */

                getItems: getFeeds,
                getSpecificItems: getSpecificFeed,
            };
        }]);
})();