(function () {
    angular.module('sfServices')
        .factory('sfFlatTaxonService', ['serviceHelper', 'serverContext', function (serviceHelper, serverContext) {
            /* Private methods and variables */
            var serviceUrl = serverContext.getRootedUrl('Sitefinity/Services/Taxonomies/FlatTaxon.svc/'),
                dataItemPromise;

            var getResource = function (taxonomyId, taxonId) {
                var url;
                if (taxonomyId && taxonomyId !== "") {
                    url = serviceUrl + taxonomyId + '/';

                    if (taxonId && taxonId !== "") {
                        url = url + taxonId + '/';
                    }
                }

                return serviceHelper.getResource(url);
            };

            var putResource = function (taxonomyId) {
                var url;
                if (taxonomyId && taxonomyId !== "") {
                    url = serviceUrl + taxonomyId + '/';
                }
                url += "ensure/";

                return serviceHelper.getResource(url, null, null, true);
            };

            var getTaxons = function (taxonomyId, skip, take, search, frontendLanguages) {
                var filter = serviceHelper.filterBuilder()
                    .searchFilter(search, frontendLanguages)
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

            var addTaxa = function (taxonomyId, provider, itemType, taxonTitles) {
                dataItemPromise = putResource(taxonomyId).put(taxonTitles).$promise;

                return dataItemPromise;
            };

            return {
                /* Returns the data items. */
                getTaxons: getTaxons,
                getSpecificItems: getSpecificItems,
                getTaxon: getTaxon,
                addTaxa: addTaxa
            };
        }]);
})();