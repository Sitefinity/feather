(function () {
    angular.module('services')
        .factory('genericDataService', ['$resource', 'serverData', function ($resource, serverData) {
            serverData.refresh();

            /* Private methods and variables */
            var getResource = function () {
                return $resource(sitefinity.getRootedUrl('restapi/sitefinity/generic-data/:items'));
            };

            var DataItem = getResource();
            var dataItemPromise;

            var getItems = function (itemType, itemProvider, skip, take, filter) {

                var generatedFilter = 'STATUS = LIVE';
                if (filter) {
                    generatedFilter = generatedFilter + ' AND (Title.ToUpper().Contains("' + filter + '".ToUpper()))';
                }

                //&filter=STATUS+%3D+MASTER
                dataItemPromise = DataItem.get(
                    {
                        items: 'data-items',
                        ItemType: itemType,
                        ItemProvider: itemProvider,
                        skip: skip,
                        take: take,
                        filter: generatedFilter
                    })
                    .$promise;

                return dataItemPromise;
            };

            return {
                /* Returns the data items. */
                getItems: getItems
            };
        }]);
})();