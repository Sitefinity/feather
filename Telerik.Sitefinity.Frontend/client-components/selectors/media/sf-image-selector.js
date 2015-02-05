; (function () {
    var sfSelectors = angular.module('sfSelectors');
    sfSelectors.requires.push('sfImageSelector');

    angular.module('sfImageSelector', ['sfServices', 'sfInfiniteScroll', 'sfCollection',
                                        'sfMediaBasicFilters', 'sfLibraryFilter', 'sfMediaFlatTaxonFilter', 'sfMediaDateFilter', 'sfSearchBox', 'sfSortBox'])
        .directive('sfImageSelector', ['serverContext', 'sfMediaService', 'serviceHelper', function (serverContext, sfMediaService, serviceHelper) {
            var constants = {
                initialLoadedItemsCount: 50,
                infiniteScrollLoadedItemsCount: 20,
                recentImagesLastDaysCount: 7,
                filterOptions: {
                    basic: {
                        allLibraries: 'AllLibraries',
                        ownItems: 'OwnItems',
                        recentItems: 'RecentItems'
                    },
                    dateCreatedDescending: 'DateCreated DESC'
                }
            };

            return {
                restrict: 'E',
                scope: {
                    selectedItem: '=?sfModel'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/media/sf-image-selector.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    scope.sortExpression = null;
                    scope.items = [];
                    scope.filterObject = sfMediaService.newFilter();
                    scope.isLoading = false;
                    scope.showSortingAndView = false;

                    scope.loadMore = function () {
                        refresh(true);
                    };

                    scope.narrowResults = function (query) {
                        if (query && scope.filterObject.basic === constants.filterOptions.basic.allLibraries) {
                            scope.filterObject.basic = null;
                        }

                        scope.filterObject.query = query || null;
                    };

                    scope.$watch('filterObject', function (newVal, oldVal) {
                        if (newVal) {
                            var newValSerialized = JSON.stringify(newVal);
                            if (newValSerialized === JSON.stringify(sfMediaService.newFilter())) {
                                scope.filterObject.basic = constants.filterOptions.basic.allLibraries;
                                return;
                            }

                            if (newValSerialized !== JSON.stringify(oldVal)) {
                                if (newVal.basic === constants.filterOptions.basic.recentItems && scope.sortExpression !== constants.filterOptions.dateCreatedDescending) {
                                    scope.sortExpression = constants.filterOptions.dateCreatedDescending;
                                }
                                else {
                                    refresh();
                                }
                            }
                        }
                    }, true);

                    scope.$watch('sortExpression', function (newVal, oldVal) {
                        if (newVal !== oldVal) {
                            if (newVal !== constants.filterOptions.dateCreatedDescending && scope.filterObject.basic === constants.filterOptions.basic.recentItems) {
                                scope.filterObject.basic = null;
                            }
                            else {
                                refresh();
                            }
                        }
                    });

                    scope.$on('sf-collection-item-selected', function (event, data) {
                        if (data && data.IsFolder === true) {
                            scope.filterObject.basic = null;
                            scope.filterObject.parent = data.Id;
                        }
                    });

                    var loadPredecessorsFolders = function () {
                        if (!scope.filterObject.parent) {
                            return;
                        }
                        var options = {
                            parent : 'predecessors/' + scope.filterObject.parent,
                            excludeNeighbours: true
                        };
                        sfMediaService.images
                                      .getFolders(options)
                                      .then(function (data) {
                                          scope.breadcrumbs = data.Items;
                                      });
                    };

                    scope.onBreadcrumbItemClick = function (item) {
                        var filter = sfMediaService.newFilter();
                        filter.parent = item && item.Id ? item.Id : filter.parent;
                        scope.sortExpression = null;
                        scope.filterObject = filter;
                    };

                    var refresh = function (appendItems) {
                        if (scope.isLoading)
                            return;

                        scope.breadcrumbs = [];

                        var options = {
                            filter: scope.filterObject.composeExpression()
                        };

                        options.parent = scope.filterObject.parent;
                        options.sort = scope.sortExpression;

                        if (appendItems) {
                            options.skip = scope.items.length;
                            options.take = constants.infiniteScrollLoadedItemsCount;
                        }
                        else {
                            options.take = constants.initialLoadedItemsCount;
                        }

                        var callback;
                        if (scope.filterObject.query) {
                            callback = sfMediaService.images.getImages;
                        }
                        else if (scope.filterObject.basic) {
                            // Defaul filter is used (Recent / My / All)
                            if (scope.filterObject.basic === constants.filterOptions.basic.recentItems) {
                                callback = sfMediaService.images.getImages;
                            }
                            else if (scope.filterObject.basic === constants.filterOptions.basic.ownItems) {
                                callback = sfMediaService.images.getImages;
                            }
                            else if (scope.filterObject.basic === constants.filterOptions.basic.allLibraries) {
                                callback = sfMediaService.images.getFolders;
                            }
                            else {
                                throw { message: 'Unknown basic filter object option.' };
                            }
                        }
                        else {
                            // custom filter is used (Libraries / Taxons / Dates)
                            callback = sfMediaService.images.getContent;

                            loadPredecessorsFolders();
                        }

                        if (scope.isLoading === false) {
                            scope.isLoading = true;

                            var itemsLength = scope.items ? scope.items.length : 0;
                            callback(options)
                                .then(function (response) {
                                    if (response && response.Items) {
                                        if (appendItems) {
                                            if (scope.items && scope.items.length === itemsLength) {
                                                scope.items = scope.items.concat(response.Items);
                                            }
                                        }
                                        else {
                                            scope.items = response.Items;
                                        }
                                    }
                                })
                                .finally(function () {
                                    scope.isLoading = false;
                                });
                        }
                    };

                    // initial open populates dialog with all root libraries
                    scope.filterObject.basic = constants.filterOptions.basic.allLibraries;
                    refresh();

                    // initial filter dropdown option
                    scope.selectedFilterOption = 1;
                }
            };
        }]);
})();