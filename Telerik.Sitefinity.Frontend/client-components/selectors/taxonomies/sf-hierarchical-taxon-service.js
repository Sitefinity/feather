(function () {
    angular.module('sfServices')
        .factory('sfHierarchicalTaxonService', ['serviceHelper', 'serverContext', function (serviceHelper, serverContext) {
            /* Private methods and variables */
            var serviceUrl = serverContext.getRootedUrl('Sitefinity/Services/Taxonomies/HierarchicalTaxon.svc/'),
                dataItemPromise;

            var getResource = function (taxonomyId, queryParams, endpoint) {
                var url = serviceUrl;
                if (endpoint) {
                    url = url + endpoint + "/";
                }
                if (taxonomyId) {
                    url = url + taxonomyId + '/';
                }
                if (queryParams) {
                    url = url + '?' + queryParams;
                }
                return serviceHelper.getResource(url);
            };

            var getFilter = function (search, frontendLanguages) {
                var filter = serviceHelper.filterBuilder()
                                          .searchFilter(search, frontendLanguages)
                                          .getFilter();

                return filter;
            };

            var getTaxons = function (taxonomyId, skip, take, search, frontendLanguages) {
                var mode = 'mode=TitlePath';
                if (!search) {
                    mode += '&hierarchyMode=true';
                }
                var filter = getFilter(search, frontendLanguages);

                dataItemPromise = getResource(taxonomyId, mode).get(
                    {
                        sortExpression: 'Title ASC',
                        filter: filter
                    })
                    .$promise;

                return dataItemPromise;
            };

            var getChildTaxons = function (parentId, search) {
                var filter = getFilter(search);

                dataItemPromise = getResource(parentId, 'hierarchyMode=true&mode=TitlePath', 'subtaxa').get(
                    {
                        sortExpression: 'Title ASC',
                        filter: filter
                    })
                    .$promise;

                return dataItemPromise;
            };

            var getSpecificItems = function (taxonomyId, ids) {
                var filter = serviceHelper.filterBuilder()
                                          .getFilter();

                return getResource(null, null, 'batchpath')
                                    .put(ids)
                                    .$promise;
            };

            var getTaxon = function (taxonomyId, taxonId) {
                dataItemPromise = getResource(taxonomyId, taxonId)
                                            .get()
                                            .$promise;

                return dataItemPromise;
            };

            return {
                /* Returns the data items. */
                getTaxons: getTaxons,
                getChildTaxons: getChildTaxons,
                getSpecificItems: getSpecificItems,
                getTaxon: getTaxon
            };
        }]);
})();