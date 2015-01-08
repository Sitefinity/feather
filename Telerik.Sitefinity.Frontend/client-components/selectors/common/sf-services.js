(function () {
    var module = angular.module('sfServices', ['ngResource', 'serverDataModule']);

    module.factory('serviceHelper', ['$resource', 'serverContext', function ($resource, serverContext) {
        /* Private methods and variables */
        var emptyGuid = '00000000-0000-0000-0000-000000000000';

        function endsWith(str, suffix) {
            return str.indexOf(suffix, str.length - suffix.length) >= 0;
        }

        function trimRight(str, suffix) {
            return str.substr(0, str.length - suffix.length);
        }

        var getResource = function (url, options) {
            var headerData = {};

            var resourceOption = options || { stripTrailingSlashes: false };

            var culture = serverContext.getUICulture();
            if (culture) {
                headerData.SF_UI_CULTURE = culture;
            }

            headerData['Cache-Control'] = 'no-cache';
            headerData.Pragma = 'no-cache';

            return $resource(url, {}, {
                get: {
                    method: 'GET',
                    headers: headerData
                },
                put: {
                    method: 'PUT',
                    headers: headerData
                }
            }, resourceOption);
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
                var culture = serverContext.getUICulture();
                if (culture) {
                    this.filter += 'Culture==' + culture;
                    return this;
                }
                else {
                    return this.trimOperator();
                }
            },
            searchFilter: function (search, frontendLanguages, searchField) {
                if (!search) return this.trimOperator();

                var field = searchField || 'Title';

                var searchFilter = '(' + field + '.ToUpper().Contains("' + search + '".ToUpper()))';

                if (frontendLanguages && frontendLanguages.length > 1) {
                    searchFilter = searchFilter + ' OR (';
                    for (var i = 0; i < frontendLanguages.length; i++) {
                        searchFilter = searchFilter + field + '["' + frontendLanguages[i] + '"]=null AND ';
                    }
                    searchFilter = searchFilter + field + '[""]!=null AND ' + field + '[""].Contains("' + search + '"))';
                    searchFilter = '(' + searchFilter + ')';
                }

                this.filter += searchFilter;

                return this;
            },
            specificItemsFilter: function (ids) {
                var itemsFilterArray = [];

                if (ids.length === 0) return this.trimOperator();

                itemsFilterArray.push('Id=' + ids[0]);

                if (ids.length > 1) {
                    for (var i = 1; i < ids.length; i++) {
                        if (ids[i] === emptyGuid) continue;
                        itemsFilterArray.push(' OR Id=' + ids[i]);
                    }
                }

                this.filter += '(' + itemsFilterArray.join('') + ')';

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

    module.provider('serverContext', function ServerContextProvider() {
        var customContext = customContext || {};

        var constructContext = function ($injector) {
            return {
                getRootedUrl: customContext.getRootedUrl || sitefinity.getRootedUrl,
                getEmbeddedResourceUrl: customContext.getEmbeddedResourceUrl || sitefinity.getEmbeddedResourceUrl,
                getFrontendLanguages: customContext.getFrontendLanguages || sitefinity.getFrontendLanguages,
                getCurrentFrontendRootNodeId: customContext.getCurrentFrontendRootNodeId || sitefinity.getCurrentFrontendRootNodeId,
                setCurrentFrontendRootNodeId: customContext.setCurrentFrontendRootNodeId || sitefinity.setCurrentFrontendRootNodeId,
                getUICulture: function () {
                    if ($injector.has('widgetContext')) {
                        return $injector.get('widgetContext').culture;
                    }

                    return customContext && customContext.uiCulture;
                }
            };
        };

        /* The context should be object containing properties: 'appPath' and optionally 'currentPackage' and 'uiCulture'. */
        this.setServerContext = function (context) {
            customContext = context;            
        };

        this.$get = ['$injector', function ($injector) {
            return constructContext($injector);
        }];
    });
})();