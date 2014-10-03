(function () {
    angular.module('services')
        .factory('dataService', ['$resource', 'widgetContext', function ($resource, widgetContext) {
            /* Private methods and variables */
            var getResource = function (itemId) {
                var url = sitefinity.services.getDataServiceUrl();
                if (itemId && itemId !== '00000000-0000-0000-0000-000000000000') {
                    url = url + itemId + '/';
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

            var getItems = function (itemType, provider, skip, take, filter, searchField) {
                if (!searchField) {
                    searchField = 'Title';
                }

                var generatedFilter = "";
                var addConjunction;                

                if (filter) {
                    generatedFilter = '('+ searchField +'.ToUpper().Contains("' + filter + '".ToUpper()))';
                    addConjunction = true;
                }

                if (widgetContext.culture) {
                    if (addConjunction) {
                        generatedFilter += ' AND ';
                    }

                    generatedFilter += 'Culture==' + widgetContext.culture;                    
                }

                dataItemPromise = getResource().get(
                    {
                        itemType: itemType,
                        itemSurrogateType: itemType,
                        provider: provider,
                        skip: skip,
                        take: take,
                        filter: generatedFilter
                    })
                    .$promise;

                return dataItemPromise;
            };

            var getItem = function (itemId, itemType, provider) {
                dataItemPromise = getResource(itemId).get(
                    {
                        itemType: itemType,
                        provider: provider
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