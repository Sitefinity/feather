(function () {
    angular.module('services')
        .factory('genericDataService', ['$resource', function ($resource) {
            /* Private methods and variables */
            var getResource = function () {
                return $resource(sitefinity.getRootedUrl('restapi/sitefinity/generic-data/:items'));
            };

            var DataItem = getResource();
            var dataItemPromise;

            var getItems = function (itemType, itemProvider, skip, take, filter) {
                var generatedFilter = 'VISIBLE= true AND STATUS = LIVE';

                if (filter) {
                    generatedFilter = generatedFilter + ' AND (Title.ToUpper().Contains("' + filter + '".ToUpper()))';
                }

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

            var getItem = function (itemId, itemType, itemProvider) {
                dataItemPromise = DataItem.get(
                    {
                        items: 'data-items',
                        ItemId: itemId,
                        ItemType: itemType,
                        ItemProvider: itemProvider
                    })
                    .$promise;

                return dataItemPromise;
            };

            return {
                /* Returns the data items. */
                getItems: getItems,
                getItem: getItem
            };
        }]);
})();