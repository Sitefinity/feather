(function () {
    angular.module('sfServices')
    .factory('sfBlogService', ['serviceHelper', 'serverContext', function (serviceHelper, serverContext) {
        var dataItemPromise,
            contentType = 'Telerik.Sitefinity.GenericContent.Model.Content',
            serviceUrl = serverContext.getRootedUrl('Sitefinity/Services/Content/BlogService.svc/');

        var getResource = function (itemId) {
            var url = serviceUrl;
            if (itemId && itemId !== serviceHelper.emptyGuid()) {
                url = url + itemId + '/';
            }

            return serviceHelper.getResource(url);
        };

        var getItems = function (provider, skip, take, search, frontendLanguages) {
            var filter = serviceHelper.filterBuilder()
                .searchFilter(search, frontendLanguages)
                .getFilter();

            dataItemPromise = getResource().get(
                {
                    itemType: contentType,
                    itemSurrogateType: contentType,
                    provider: provider,
                    sortExpression: 'DateCreated DESC',
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
                    provider: provider,
                    published: true
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
                    itemType: contentType,
                    itemSurrogateType: contentType,
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