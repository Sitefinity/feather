(function () {
    angular.module('services')
        .factory('flatTaxonService', ['$resource', 'widgetContext', function ($resource, widgetContext) {
            /* Private methods and variables */
            var getResource = function (taxonomyId, taxonId) {
                var url = sitefinity.services.getFlatTaxonServiceUrl();
                if (taxonomyId && taxonomyId !== "") {
                    url = url + taxonomyId + '/';

                    if (taxonId && taxonId !== "") {
                        url = url + taxonId + '/';
                    }
                }

                var headerData;
                if (widgetContext.culture) {
                    headerData = {
                        'SF_UI_CULTURE': widgetContext.culture
                    };
                }

                return $resource(url, {}, {
                    get: {
                        method: 'GET',
                        headers: headerData
                    }
                });
            };

            var dataItemPromise;

            var getTaxons = function (taxonomyId, skip, take, filter) {
                var generatedFilter;
                if (filter) {
                    generatedFilter = '(Title.ToUpper().Contains("' + filter + '".ToUpper()))';
                }

                dataItemPromise = getResource(taxonomyId).get(
                    {
                        sortExpression: 'Title ASC',
                        skip: skip,
                        take: take,
                        filter: generatedFilter
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
                getTaxon: getTaxon
            };
        }]);
})();