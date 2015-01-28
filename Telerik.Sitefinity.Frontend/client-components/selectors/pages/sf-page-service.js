(function () {
    angular.module('sfServices')
        .factory('sfPageService', ['serverContext', 'serviceHelper', '$injector', function (serverContext, serviceHelper, $injector) {
            /* Private methods and variables */
            var serviceUrl = serverContext.getRootedUrl('Sitefinity/Services/Pages/PagesService.svc/');

            var getResource = function (path, culture) {
                var url = serviceUrl;
                if (path) {
                    url = url + path + '/';
                }

                if (culture) {
                    var headerData = { SF_UI_CULTURE: culture };

                    return serviceHelper.getResource(url, null, headerData);
                }
                return serviceHelper.getResource(url);
            };

            var getItems = function (parentId, siteId, provider, search, culture) {
                if (search) {
                    var filter = serviceHelper.filterBuilder()
                                              .searchFilter(search)
                                              .getFilter();

                    return getResource(null, culture).get({
                        root: parentId,
                        provider: provider,
                        filter: filter
                    }).$promise;
                }
                else {
                    var promise = getResource().get({
                        root: parentId,
                        hierarchyMode: true,
                        sf_site: siteId,
                        provider: provider,
                        filter: search
                    }).$promise;

                    patchBugInSitefinityEndPointService(promise, siteId);

                    return promise;
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

            var getPredecessors = function (itemId, provider, siteId) {
                var promise = getResource('predecessor/' + itemId).get({
                    provider: provider,
                    site_Id: siteId
                }).$promise;

                patchBugInSitefinityEndPointService(promise, siteId);

                return promise;
            };

            var getPageTitleByCulture = function (page, culture) {
                if (page) {
                    if (page.Title && page.Title.ValuesPerCulture && culture) {
                        for (var i = 0; i < page.Title.ValuesPerCulture.length; i++) {
                            var valuePerCulture = page.Title.ValuesPerCulture[i];
                            if (valuePerCulture.Key === culture) {
                                return valuePerCulture.Value;
                            }
                        }
                    }
                    if (!page.Title) {
                        return page.Id;
                    }
                    if (typeof page.Title === 'string') {
                        return page.Title;
                    }
                    else if (page.Title.Value) {
                        return page.Title.Value;
                    }
                }
            };

            //// HACK: Need to reset to the default backend site so we need to make an additional service call.
            //// Remove this method once it is resolve in the page service!
            var patchBugInSitefinityEndPointService = function (promise, siteId) {
                if (!siteId) {
                    return;
                }

                if (!$injector.has('sfMultiSiteService')) {
                    return;
                }

                var multiSiteService = $injector.get('sfMultiSiteService');

                var currentSite = multiSiteService.getSiteByRootNoteId(serverContext.getCurrentFrontendRootNodeId());

                if (!currentSite) {
                    return;
                }

                var shouldResetSite = currentSite.Id !== siteId;

                if (shouldResetSite) {
                    promise.then(function () {
                        getResource().get({
                            sf_site: currentSite.Id
                        });
                    });
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