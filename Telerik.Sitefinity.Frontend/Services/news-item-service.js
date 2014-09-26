(function () {
    angular.module('services')
        .factory('newsItemService', ['$resource', 'widgetContext', function ($resource, widgetContext) {
            /* Private methods and variables */
            var getResource = function (itemId) {
                var url = sitefinity.services.getNewsItemServiceUrl();
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

            var getItems = function (provider, skip, take, filter, frontendLanguages) {
                var generatedFilter = 'Visible==true AND Status==live';
                if (widgetContext.culture) {
                    generatedFilter = generatedFilter + ' AND Culture==' + widgetContext.culture;
                }

                if (filter) {
                    searchFilter = '(Title.ToUpper().Contains("' + filter + '".ToUpper()))';
                    if (frontendLanguages && frontendLanguages.length !== 0) {
                        searchFilter = searchFilter + ' OR (';
                        for (var i = 0; i < frontendLanguages.length; i++) {
                            searchFilter = searchFilter + 'Title["' + frontendLanguages[i] + '"]=null AND ';
                        }
                        searchFilter = searchFilter + 'Title[""]!=null AND Title[""].Contains("' + filter + '"))';
                        searchFilter = '(' + searchFilter + ')';
                    }
                    generatedFilter = generatedFilter + ' AND ' + searchFilter;
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