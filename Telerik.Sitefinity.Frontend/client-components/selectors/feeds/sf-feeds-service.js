(function () {
    angular.module('sfServices')
        .factory('sfFeedsService', ['serviceHelper', 'serverContext', function (serviceHelper, serverContext) {
            /* Private methods and variables */

            var serviceUrl = serverContext.getRootedUrl('Sitefinity/Services/Publishing/PublishingService.svc/');

            var getResource = function (uriTemplate) {
                var url = serviceUrl;

                if (uriTemplate) {
                    url += uriTemplate + '/';
                }

                return serviceHelper.getResource(url);
            };

            var defaultProviderName = "OAPublishingProvider";
            var pipeName = "RSSOutboundPipe";

            var getFeeds = function (provider, skip, take, search, sort) {
                var filter = serviceHelper.filterBuilder()
                    .searchFilter(search)
                    .getFilter();

                return getResource('pipes').get({
                    providerName: provider || defaultProviderName,
                    pipeTypeName: pipeName,
                    sort: sort,
                    skip: skip,
                    take: take,
                    filter: filter
                }).$promise;
            };

            var getSpecificItems = function (id, provider) {
                return getResource(id).put({
                    providerName: provider,
                    createNew: false
                }).$promise;
            };

            return {
                /* Returns the data items. */

                getItems: getFeeds,
                getSpecificItems: getSpecificItems,
            };
        }]);
})();