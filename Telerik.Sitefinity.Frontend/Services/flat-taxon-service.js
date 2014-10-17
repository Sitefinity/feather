(function () {
    angular.module('services')
        .factory('flatTaxonService', ['serviceHelper', 'serverContext', function (serviceHelper, serverContext) {
            /* Private methods and variables */
            var serviceUrl = serverContext.getRootedUrl('Sitefinity/Services/Taxonomies/FlatTaxon.svc/'),
                dataItemPromise;

            var getResource = function (taxonomyId, taxonId) {
                if (taxonomyId && taxonomyId !== "") {
                    var url = serviceUrl + taxonomyId + '/';

                    if (taxonId && taxonId !== "") {
                        url = url + taxonId + '/';
                    }
                }

                return serviceHelper.getResource(url);
            };

            var getTaxons = function (taxonomyId, skip, take, search) {
                var filter = serviceHelper.filterBuilder()
                    .searchFilter(search)
                    .getFilter();

                dataItemPromise = getResource(taxonomyId).get(
                    {
                        sortExpression: 'Title ASC',
                        skip: skip,
                        take: take,
                        filter: filter
                    })
                    .$promise;

                return dataItemPromise;
            };

            var getSpecificItems = function (taxonomyId, ids) {
                var filter = serviceHelper.filterBuilder()
                    .specificItemsFilter(ids)
                    .getFilter();

                dataItemPromise = getResource(taxonomyId).get(
                    {
                        sortExpression: 'Title ASC',
                        skip: 0,
                        take: 100,
                        filter: filter
                    })
                    .$promise;

                return dataItemPromise;
            };

            var getTaxon = function (taxonomyId, taxonId) {
                dataItemPromise = getResource(taxonomyId, taxonId).get()
                    .$promise;

                return dataItemPromise;
            };

            return {
                /* Returns the data items. */
                getTaxons: getTaxons,
                getSpecificItems: getSpecificItems,
                getTaxon: getTaxon
            };
        }]);
})();