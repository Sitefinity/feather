(function () {
    angular.module('sfServices')
        .factory('sfListsService', ['serviceHelper', 'serverContext', function (serviceHelper, serverContext) {
            /* Private methods and variables */

            var listSurrogateType = 'Telerik.Sitefinity.Lists.Model.List',
                serviceUrl = serverContext.getRootedUrl('Sitefinity/Services/Lists/ListService.svc/'),
                listRestApi = serverContext.getRootedUrl('restapi/lists-api/items/');

            var getResource = function () {
                var url = serviceUrl;

                return serviceHelper.getResource(url);
            };

            var getRestResource = function () {
                var url = listRestApi;

                return serviceHelper.getResource(url);
            };

            var getItems = function (provider, skip, take, search, frontendLanguages) {
                var filter = serviceHelper.filterBuilder()
                    .searchFilter(search, frontendLanguages)
                    .getFilter();

                return getResource().get({
                    provider: provider,
                    sortExpression: 'DateCreated DESC',
                    skip: skip,
                    take: take,
                    filter: filter
                }).$promise;
            };

            var getSpecificItems = function (ids, provider) {
                return getRestResource().put({
                    ids: ids,
                    provider: provider
                }).$promise;
            };

            return {
                /* Returns the data items. */

                getItems: getItems,
                getSpecificItems: getSpecificItems,
            };
        }]);
})();