(function () {
    angular.module('services')
        .factory('hierarchicalTaxonService', ['serviceHelper', 'serverContext', function (serviceHelper, serverContext) {
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
                var hierarchyMode;
                if (!search) {
                    hierarchyMode = 'hierarchyMode=true';
                }
                var filter = getFilter(search, frontendLanguages);

                dataItemPromise = getResource(taxonomyId, hierarchyMode).get(
                    {
                        sortExpression: 'Title ASC',
                        skip: skip,
                        take: take,
                        filter: filter
                    })
                    .$promise;

                return dataItemPromise;
            };

            var getChildTaxons = function (parentId, search) {
                var filter = getFilter(search);

                dataItemPromise = getResource(parentId, 'hierarchyMode=true', 'subtaxa').get(
                    {
                        sortExpression: 'Title ASC',
                        filter: filter
                    })
                    .$promise;

                return dataItemPromise;
            }

            var getSpecificItems = function (taxonomyId, ids) {
                var filter = serviceHelper.filterBuilder()
                                          .getFilter();

                var promises = [];

                angular.forEach(ids, function (id) {
                    promises.push(getResource(id, 'onlyPath=true', 'predecessor').get(
                    {
                        sortExpression: 'Title ASC',
                        skip: 0,
                        take: 100,
                        filter: filter
                    })
                    .$promise);
                });

                return promises;
            };

            var getTaxon = function (taxonomyId, taxonId) {
                dataItemPromise = getResource(taxonomyId, taxonId).get().$promise;

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