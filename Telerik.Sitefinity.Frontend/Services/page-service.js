(function () {
    angular.module('services')
        .factory('sfPageService', ['serverContext', 'serviceHelper', function (serverContext, serviceHelper) {
            /* Private methods and variables */
            var serviceUrl = serverContext.getRootedUrl('Sitefinity/Services/Pages/PagesService.svc/hierarchy/');

            var getResource = function (parentId) {
                var url = serviceUrl + parentId + '/';

                return serviceHelper.getResource(url);
            };

            var getItems = function (parentId, siteId, provider, search) {
                return getResource(parentId).get({
                    provider: provider,
                    siteId: siteId,
                    filter: search
                }).$promise;
            }

            return {
                getItems: getItems
            };
        }]);
})();