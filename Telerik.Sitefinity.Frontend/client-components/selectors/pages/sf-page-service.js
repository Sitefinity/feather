(function () {
    angular.module('sfServices')
        .factory('sfPageService', ['serverContext', 'serviceHelper', function (serverContext, serviceHelper) {
            /* Private methods and variables */
            var serviceUrl = serverContext.getRootedUrl('Sitefinity/Services/Pages/PagesService.svc/');

            var getResource = function (path) {
                var url = serviceUrl;
                if (path) {
                    url = url + path + '/';
                }

                return serviceHelper.getResource(url);
            };

            var getItems = function (parentId, siteId, provider, search) {
                if (search) {
                    var filter = serviceHelper.filterBuilder()
                                              .searchFilter(search)
                                              .getFilter();
                    return getResource().get({
                                            provider: provider,
                                            filter: filter
                                        }).$promise;
                }
                else {
                    return getResource('hierarchy/' + parentId).get({
                        provider: provider,
                        siteId: siteId,
                        filter: search
                    }).$promise;
                }
            };

            var getSpecificItems = function (ids, provider) {
                var filter = serviceHelper.filterBuilder()
                                          .specificItemsFilter(ids)
                                          .getFilter();

                return getResource().get({
                    provider: provider,
                    filter: filter
                }).$promise;
            };

            var getPredecessors = function (itemId, provider) {
                return getResource('predecessor/' + itemId).get({
                    provider: provider
                }).$promise;
            };

            return {
                getItems: getItems,
                getSpecificItems: getSpecificItems,
                getPredecessors: getPredecessors
            };
        }]);
})();