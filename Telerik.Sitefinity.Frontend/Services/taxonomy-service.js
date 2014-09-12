(function () {
    angular.module('services')
        .factory('taxonomyService', ['$resource', function ($resource) {
            /* Private methods and variables */
            var getResource = function () {
                var url = sitefinity.services.getTaxonomyServiceUrl();

                return $resource(url);
            };

            var dataItemPromise;

            var getTaxonomies = function (provider, skip, take, filter, taxonomyType) {
                var generatedFilter;

                if (filter) {
                    generatedFilter = '(Title.ToUpper().Contains("' + filter + '".ToUpper()))';
                }

                dataItemPromise = getResource().get(
                    {
                        provider: provider,
                        sortExpression: 'Title ASC',
                        skip: skip,
                        take: take,
                        filter: generatedFilter,
                        taxonomyType: taxonomyType
                    })
                    .$promise;

                return dataItemPromise;
            };

            return {
                /* Returns the taxonomy items. */
                getTaxonomies: getTaxonomies
            };
        }]);
})();