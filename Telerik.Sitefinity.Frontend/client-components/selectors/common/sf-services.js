(function () {
    var module = angular.module('sfServices', ['ngResource', 'serverDataModule']);

    module.config(['$httpProvider', function ($httpProvider) {
        if (!$httpProvider.defaults.headers.get) {
            $httpProvider.defaults.headers.get = {};
        }

        var getHeaders = $httpProvider.defaults.headers.get;

        //disable IE ajax request caching
        //NOTE: This breaks angular logic for loading templates through XHR request leading to 400 - bad request.
        //Only specific format is accepted for this header by the server.  
        //getHeaders['If-Modified-Since'] = 'Thu, 01 Feb 1900 00:00:00';
        getHeaders['Cache-Control'] = 'no-cache';
        getHeaders.Pragma = 'no-cache';
    }]);

    module.factory('serviceHelper', ['$resource', 'serverContext', function ($resource, serverContext) {
        /* Private methods and variables */
        var emptyGuid = '00000000-0000-0000-0000-000000000000';

        function endsWith(str, suffix) {
            return str.indexOf(suffix, str.length - suffix.length) >= 0;
        }

        function trimRight(str, suffix) {
            return str.substr(0, str.length - suffix.length);
        }

        var getResource = function (url, options, headers, isArray) {
            var headerData = headers || {};

            var resourceOption = options || { stripTrailingSlashes: false };

            var culture = serverContext.getUICulture();

            if (culture && !headerData.SF_UI_CULTURE) {
                headerData.SF_UI_CULTURE = culture;
            }

            //headerData['Cache-Control'] = 'no-cache';
            //headerData.Pragma = 'no-cache';

            return $resource(url, {}, {
                get: {
                    method: 'GET',
                    isArray: isArray,
                    headers: headerData
                },
                put: {
                    method: 'PUT',
                    isArray: isArray,
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
                    for (var i = 0; i < frontendLanguages.length; i++) {
                        var localizedField = String.format("{0}[\"{1}\"]", field, frontendLanguages[i]);
                        searchFilter += String.format("OR {0}.ToUpper().Contains(\"{1}\".ToUpper())", localizedField, search);
                    }
                    searchFilter = '(' + searchFilter + ')';
                }

                this.filter += searchFilter;

                return this;
            },
            differFilter: function (items, identifier) {
                var itemsFilterArray = [];

                if (!items || items.length === 0) return this.trimOperator();

                itemsFilterArray.push(identifier + '!="' + items[0] + '"');

                if (items.length > 1) {
                    for (var i = 1; i < items.length; i++) {
                        itemsFilterArray.push(' AND ' + identifier + '!="' + items[i] + '"');
                    }
                }

                this.filter += '(' + itemsFilterArray.join('') + ')';

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
            append: function (filter) {
                if (filter) {
                    this.filter += '(' + filter + ')';
                }

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
                getCurrentUserId: customContext.getCurrentUserId || sitefinity.getCurrentUserId,
                getUICulture: function () {
                    if ($injector.has('widgetContext')) {
                        return $injector.get('widgetContext').culture;
                    }

                    return customContext && customContext.uiCulture;
                },
                isMultisiteEnabled: customContext.isMultisiteEnabled || sitefinity.isMultisiteEnabled
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