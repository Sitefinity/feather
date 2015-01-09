(function () {
    angular.module('sfServices')
        .factory('sfSearchService', ['serviceHelper', 'serverContext', function (serviceHelper, serverContext) {
            /* Private methods and variables */
            var serviceUrl = serverContext.getRootedUrl('Sitefinity/Services/Publishing/PublishingService.svc/'),
                dataItemPromise;
            var searchProviderName = "SearchPublishingProvider";
            var pipeName = "SearchIndex";

            var getResource = function (queryParams, endpoint) {
                var url = serviceUrl;
                if (endpoint) {
                    url = url + endpoint + "/";
                }
                if (queryParams) {
                    url = url + '?' + queryParams;
                }
                return serviceHelper.getResource(url);
            };

            var getFilter = function (search) {
                var filter = serviceHelper.filterBuilder()
                                          .searchFilter(search)
                                          .getFilter();

                return filter;
            };

            var getSearchIndexes = function (skip, take, search) {
                var queryParams =  String.format('providerName={0}&pipeTypeName={1}', searchProviderName, pipeName);
                var endpoint = "pipes";
                var filter = getFilter(search);

                dataItemPromise = getResource(queryParams, endpoint).get(
                    {
                        sort: 'Title ASC',
                        filter: filter,
                        skip: skip,
                        take: take
                    })
                    .$promise;

                return dataItemPromise;
            };
           
            return {
                /* Returns the data items. */
                getSearchIndexes: getSearchIndexes
            };
        }]);
})();