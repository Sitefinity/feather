(function () {
    angular.module('sfServices')
        .factory('sfListService', ['serviceHelper', 'serverContext', function (serviceHelper, serverContext) {
            /* Private methods and variables */

            var listSurrogateType = 'Telerik.Sitefinity.Lists.Model.List',
                serviceUrl = serverContext.getRootedUrl('Sitefinity/Services/Lists/ListService.svc/');

            var getResource = function (id) {
                var url = serviceUrl;
                if (id) {
                    url = serviceUrl + id + '/';
                }
                return serviceHelper.getResource(url);
            };
            
            var getItems = function (provider) {
                return getResource().get({
                    provider: provider
                }).$promise;
            };

            var getItem = function (id, provider) {
                return getResource(id).get({
                    provider: provider
                }).$promise;
            };

            return {
                /* Returns the data items. */

                getItems: getItems,
                getSpecificItems: getItem,
            };
        }]);
})();