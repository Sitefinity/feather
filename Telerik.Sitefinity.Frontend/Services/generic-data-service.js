(function () {
    angular.module('services')
        .factory('genericDataService', ['$resource', function ($resource) {
            /* Private methods and variables */
            var getResource = function () {
                var appPath = sf.pageEditor.widgetContext.AppPath;
                return $resource(appPath + "restapi/sitefinity/generic-data/:items");
            };

            var DataItem = getResource();
            var dataItemPromise;

            var getItems = function (itemType, itemProvider, skip, take, filter) {

                var generatedFilter = 'STATUS = MASTER';
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