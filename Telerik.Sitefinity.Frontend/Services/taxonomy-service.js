(function () {
    angular.module('services')
        .factory('taxonomyService', ['$resource', 'serverContext', function ($resource, serverContext) {
            /* Private methods and variables */
            var url = serverContext.getRootedUrl('Sitefinity/Services/Taxonomies/Taxonomy.svc/'),
                dataItemPromise;

            var getResource = function () {
                return $resource(url);
            };

            var getTaxonomies = function (skip, take, filter, taxonomyType) {
                var generatedFilter;

                if (filter) {
                    generatedFilter = '(Title.ToUpper().Contains("' + filter + '".ToUpper()))';
                }

                dataItemPromise = getResource().get(
                    {
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