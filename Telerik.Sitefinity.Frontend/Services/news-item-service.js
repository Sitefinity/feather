(function () {
    angular.module('services')
        .factory('newsItemService', ['$resource', 'widgetContext', function ($resource, widgetContext) {
            /* Private methods and variables */
            var getResource = function (itemId) {
                var url = sitefinity.services.getNewsItemServiceUrl();
                if (itemId && itemId !== '00000000-0000-0000-0000-000000000000') {
                    url = url + '/' + itemId + '/';
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

            var getItems = function (provider, skip, take, filter) {
                var generatedFilter = 'Visible==true AND Status==live';
                if (widgetContext.culture) {
                    generatedFilter = generatedFilter + ' AND Culture==' + widgetContext.culture;
                }

                if (filter) {
                    generatedFilter = generatedFilter + ' AND (Title.ToUpper().Contains("' + filter + '".ToUpper()))';
                }

                dataItemPromise = getResource().get(
                    {
                        itemType: 'Telerik.Sitefinity.GenericContent.Model.Content',
                        itemSurrogateType: 'Telerik.Sitefinity.GenericContent.Model.Content',
                        provider: provider,
                        skip: skip,
                        take: take,
                        filter: generatedFilter
                    })
                    .$promise;

                return dataItemPromise;
            };

            var getItem = function (itemId, provider) {
                dataItemPromise = getResource(itemId).get(
                    {
                        provider: provider,
                        published: true
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