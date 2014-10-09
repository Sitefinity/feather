(function () {
    var module = angular.module('services', ['ngResource']);

    module.factory('serviceHelper', ['$resource', 'widgetContext', function ($resource, widgetContext) {
        /* Private methods and variables */
        var emptyGuid = '00000000-0000-0000-0000-000000000000';

        function endsWith(str, suffix) {
            return str.indexOf(suffix, str.length - suffix.length) >= 0;
        }

        function trimRight(str, suffix) {
            return str.substr(0, str.length - suffix.length);
        }

        var getResource = function (url) {
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

        function FilterBuilder(baseFilter) {
            this.filter = baseFilter || '';
            this.liveItemsFilter = 'Visible==true AND Status==live';
            this.andOperator = ' AND ';
        }
        FilterBuilder.prototype = {
            constructor: FilterBuilder,
            lifecycleFilter: function () {
                this.filter += this.liveItemsFilter;
                return this;
            },
            cultureFilter: function () {
                if (widgetContext.culture) {
                    this.filter += 'Culture==' + widgetContext.culture;
                    return this;
                }
                else {
                    return this.trimOperator();
                }
            },
            searchFilter: function (search, searchField) {
                if (!search) return this.trimOperator();

                var field = searchField || 'Title';

                this.filter += '(' + field + '.ToUpper().Contains("' + search + '".ToUpper()))';

                return this;
            },
            specificItemsFilter: function (ids) {
                var itemsFilterArray = [];

                if (ids.length === 0) return this.trimOperator();

                itemsFilterArray.push('Id=(' + ids[0] + ')');

                if (ids.length > 1) {
                    for (var i = 1; i < ids.length; i++) {
                        if (ids[i] === emptyGuid) continue;
                        itemsFilterArray.push(' OR Id=(' + ids[i] + ')');
                    }
                }

                this.filter += itemsFilterArray.join('');

                return this;
            },
            and: function () {
                if (this.filter) {
                    this.filter += this.andOperator;
                }
                
                return this;
            },
            trimOperator: function () {
                if (endsWith(this.filter, this.andOperator)) {
                    this.filter = trimRight(this.filter, this.andOperator);
                }

                return this;
            },
            getFilter: function () {
                return this.filter;
            }
        };

        /* Public interface */
        return {
            filterBuilder: function (baseFilter) {
                return new FilterBuilder(baseFilter);
            },
            emptyGuid: function () {
                return emptyGuid;
            },
            getResource: getResource
        };
    }]);
})();