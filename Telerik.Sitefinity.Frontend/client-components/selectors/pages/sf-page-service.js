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
                        root: parentId,
                        provider: provider,
                        filter: filter
                    }).$promise;
                }
                else {
                    return getResource().get({
                        root: parentId,
                        hierarchyMode: true,
                        sf_site: siteId,
                        provider: provider,
                        filter: search
                    }).$promise;
                }
            };

            var getSpecificItems = function (ids, provider, rootId) {
                var filter = serviceHelper.filterBuilder()
                                          .specificItemsFilter(ids)
                                          .getFilter();

                return getResource().get({
                    root: rootId,
                    provider: provider,
                    filter: filter
                }).$promise;
            };

            var getPredecessors = function (itemId, provider, rootId) {
                return getResource('predecessor/' + itemId).get({
                    provider: provider,
                    root: rootId
                }).$promise;
            };

            var getPageTitleByCulture = function (page, culture) {
                if (page) {
                    if (page.Title && page.Title.ValuesPerCulture) {
                        for (var i = 0; i < page.Title.ValuesPerCulture.length; i++) {
                            var valuePerCulture = page.Title.ValuesPerCulture[i];
                            if (valuePerCulture.Key === culture) {
                                return valuePerCulture.Value;
                            }
                        }
                    }
                    if (!page.TitlesPath) {
                        return page.Id;
                    }
                    if (typeof page.TitlesPath === 'string') {
                        return page.TitlesPath;
                    }
                    else if (page.TitlesPath.Value) {
                        return page.TitlesPath.Value;
                    }
                }
            };

            return {
                getItems: getItems,
                getSpecificItems: getSpecificItems,
                getPredecessors: getPredecessors,
                getPageTitleByCulture: getPageTitleByCulture
            };
        }]);
})();