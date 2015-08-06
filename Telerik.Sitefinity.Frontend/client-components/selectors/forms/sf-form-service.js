(function () {
    angular.module('sfServices').factory('sfFormService', ['serviceHelper', 'serverContext', function (serviceHelper, serverContext) {
        var contentType = 'Telerik.Sitefinity.GenericContent.Model.Content',
            serviceUrl = serverContext.getRootedUrl('Sitefinity/Services/Forms/FormsService.svc/');

        var getResource = function (itemId) {
            var url = serviceUrl;
            if (itemId && itemId !== serviceHelper.emptyGuid()) {
                url = url + itemId + '/';
            }

            return serviceHelper.getResource(url);
        };

        var buildOptions = function (provider, skip, take, search, frontendLanguages, formFramework) {
            var filter = serviceHelper
                .filterBuilder()
                .searchFilter(search, frontendLanguages)
                .getFilter();

            if (formFramework) {
                if (formFramework.toLowerCase() === 'mvc') {
                    if (filter) {
                        filter += ' AND ';
                    }

                    filter += 'Framework == Mvc';
                }
                else if (formFramework.toLowerCase() === 'webforms') {
                    if (filter) {
                        filter += ' AND ';
                    }

                    filter += 'Framework == WebForms';
                }
            }

            return {
                itemType: contentType,
                itemSurrogateType: contentType,
                provider: provider,
                sortExpression: 'DateCreated DESC',
                skip: skip,
                take: take,
                filter: filter
            }
        };

        var getItems = function (provider, skip, take, search, frontendLanguages, formFramework) {
            var options = buildOptions(provider, skip, take, search, frontendLanguages, formFramework || 'mvc');
            return getResource().get(options).$promise;
        };

        return {
            getItems: getItems
        };
    }]);
})();