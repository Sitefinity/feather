; (function () {
    var sfSelectors = angular.module('sfSelectors');
    sfSelectors.requires.push('sfImageSelector');

    angular.module('sfImageSelector', ['sfServices', 'sfInfiniteScroll', 'sfCollection', 'sfTree', 'sfSearchBox', 'sfSortBox'])
        .directive('sfImageSelector', ['serverContext', 'sfMediaService', 'serviceHelper', 'sfFlatTaxonService', 'sfHierarchicalTaxonService',
        function (serverContext, sfMediaService, serviceHelper, sfFlatTaxonService, sfHierarchicalTaxonService) {
            var helpers = {
                getDate: function (daysToSubstract, monthsToSubstract, yearsToSubstract) {
                    var now = new Date();

                    if (daysToSubstract) {
                        now.setDate(now.getDate() - daysToSubstract);
                    }

                    if (monthsToSubstract) {
                        now.setMonth(now.getMonth() - monthsToSubstract);
                    }

                    if (yearsToSubstract) {
                        now.setYear(now.getFullYear() - yearsToSubstract);
                    }

                    return now;
                }
            };

            var constants = {
                initialLoadedItemsCount: 50,
                infiniteScrollLoadedItemsCount: 20,
                recentImagesLastDaysCount: 7,
                filters: {
                    dates: [
                        { text: 'Any time', dateValue: null },
                        { text: 'Last 1 day', dateValue: helpers.getDate(1) },
                        { text: 'Last 3 days', dateValue: helpers.getDate(3) },
                        { text: 'Last 1 week', dateValue: helpers.getDate(7) },
                        { text: 'Last 1 month', dateValue: helpers.getDate(0, 1) },
                        { text: 'Last 6 months', dateValue: helpers.getDate(0, 6) },
                        { text: 'Last 1 year', dateValue: helpers.getDate(0, 0, 1) },
                        { text: 'Last 2 years', dateValue: helpers.getDate(0, 0, 2) },
                        { text: 'Last 5 years', dateValue: helpers.getDate(0, 0, 5) }
                    ],
                    tags: {
                        pageSize: 10,
                        field: 'Tags',
                        taxonomyId: 'CB0F3A19-A211-48a7-88EC-77495C0F5374'
                    },
                    categories: {
                        pageSize: 10, // not used by the service
                        field: 'Category',
                        taxonomyId: 'E5CD6D69-1543-427B-AD62-688A99F5E7D4'
                    }
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
                    var filtersLogic = {
                        // Library filter
                        loadLibraryChildren: function (parent) {
                            parent = parent || {};
                            return sfMediaService.images.getFolders({ parent: parent.Id }).then(function (response) {
                                if (response) {
                                    return response.Items;
                                }
                            });
                        },
                        // Category filter
                        getCategoryChildTaxons: function (parentId) {
                            return sfHierarchicalTaxonService.getChildTaxons(parentId, scope.query)
                                .then(function (data) {
                                    return data.Items;
                                });
                        },
                        getCategoryTaxons: function () {
                            var skip = 0;
                            return sfHierarchicalTaxonService.getTaxons(constants.filters.categories.taxonomyId, skip, constants.filters.categories.pageSize, scope.filters.category.query)
                                .then(function (data) {
                                    return data.Items;
                                });
                        },
                        loadCategoryChildren: function (parent) {
                            if (parent) {
                                return filtersLogic.getCategoryChildTaxons(parent.Id);
                            }
                            else {
                                return filtersLogic.getCategoryTaxons();
                            }
                        },
                        // Tag filter
                        loadTagTaxons: function (append) {
                            if (scope.filters.tag.isLoading) {
                                return;
                            }

                            scope.filters.tag.isLoading = true;
                            var skip = append ? scope.filters.tag.all.length : 0;
                            sfFlatTaxonService.getTaxons(constants.filters.tags.taxonomyId, skip, constants.filters.tags.pageSize, scope.filters.tag.query)
                                .then(function (data) {
                                    if (data && data.Items) {
                                        if (append) {
                                            if (scope.filters.tag.all && scope.filters.tag.all.length === skip) {
                                                scope.filters.tag.all = scope.filters.tag.all.concat(data.Items);
                                            }
                                        }
                                        else {
                                            scope.filters.tag.all = data.Items;
                                        }
                                    }
                                })
                                .finally(function () {
                                    scope.filters.tag.isLoading = false;
                                });
                        },
                        loadMoreTags: function () {
                            filtersLogic.loadTagTaxons(true);
                        }
                    };

                    var refresh = function (appendItems) {
                        if (scope.isLoading) {
                            return;
                        }

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
                            if (scope.filterObject.basic === scope.filterObject.constants.basic.recentItems) {

                                // When the filter is Recent items, the number of displayed items is fixed and we should not append more.
                                if (appendItems) return;

                                callback = sfMediaService.images.getImages;
                            }
                            else if (scope.filterObject.basic === scope.filterObject.constants.basic.ownItems) {
                                callback = sfMediaService.images.getImages;
                            }
                            else if (scope.filterObject.basic === scope.filterObject.constants.basic.allLibraries) {
                                callback = sfMediaService.images.getFolders;
                            }
                            else {
                                throw { message: 'Unknown basic filter object option.' };
                            }
                        }
                        else {
                            // custom filter is used (Libraries / Taxons / Dates)
                            callback = sfMediaService.images.getContent;
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

                    scope.sortExpression = null;
                    scope.items = [];
                    scope.filterObject = sfMediaService.newFilter();
                    scope.isLoading = false;
                    scope.showSortingAndView = false;

                    scope.filters = {
                        library: {
                            selected: null,
                            getChildren: filtersLogic.loadLibraryChildren
                        },
                        date: {
                            all: constants.filters.dates,
                            selected: constants.filters.dates[0].dateValue
                        },
                        tag: {
                            all: [],
                            selected: null,
                            query: null,
                            isLoading: false,
                            loadMore: filtersLogic.loadMoreTags
                        },
                        category: {
                            filtered: [],
                            selected: null,
                            query: null,
                            getChildren: filtersLogic.loadCategoryChildren
                        }
                    };

                    scope.narrowResults = scope.filterObject.set.query.to;

                    // load more images
                    scope.loadMore = function () {
                        refresh(true);
                    };

                    /*
                    * Watches.
                    */

                    scope.$watch('sortExpression', function (newVal, oldVal) {
                        if (newVal !== oldVal) {
                            if (newVal !== scope.filterObject.constants.dateCreatedDescending && scope.filterObject.basic === scope.filterObject.constants.basic.recentItems) {
                                scope.filterObjects.set.basic.none();
                            }
                            else {
                                refresh();
                            }
                        }
                    });

                    scope.$watch('filters.library.selected', function (newVal, oldVal) {
                        if (newVal && newVal !== oldVal) {
                            scope.filterObject.set.parent.to(newVal);
                        }
                    }, true);

                    scope.$watch('filters.tag.selected', function (newVal, oldVal) {
                        if (newVal && newVal !== oldVal) {
                            if (newVal && newVal[0]) {
                                scope.filterObject.set.taxon.to(newVal[0], constants.filters.tags.field);
                            }
                        }
                    }, true);

                    scope.$watch('filters.tag.query', function (newVal, oldVal) {
                        if (newVal !== oldVal) {
                            filtersLogic.loadTagTaxons(false);
                        }
                    });

                    scope.$watch('filters.category.selected', function (newVal, oldVal) {
                        if (newVal && newVal !== oldVal) {
                            var selectedTaxonId;
                            if (angular.isArray(newVal) && newVal.length > 0) {
                                selectedTaxonId = newVal[0];
                            }
                            else {
                                selectedTaxonId = newVal;
                            }
                            
                            scope.filterObject.set.taxon.to(selectedTaxonId, constants.filters.categories.field);
                        }
                    }, true);

                    scope.$watch('filters.category.query', function (newVal, oldVal) {
                        if (newVal !== oldVal) {
                            filtersLogic.getCategoryTaxons().then(function (items) {
                                scope.filters.category.filtered = items;
                            });
                        }
                    });

                    scope.$watch('filters.date.selected', function (newVal, oldVal) {
                        if (newVal !== oldVal) {
                            if (newVal && newVal[0]) {
                                scope.filterObject.set.date.to(newVal[0]);
                            }
                            else {
                                scope.filterObject.set.date.all();
                            }
                        }
                    }, true);

                    // Reacts when a folder is clicked.
                    scope.$on('sf-collection-item-selected', function (event, data) {
                        if (data && data.IsFolder === true) {
                            scope.filterObject.set.parent.to(data.Id)
                        }
                    });

                    /*
                    * Initialization.
                    */

                    (function initializeWindow() {
                        // initial open populates dialog with all root libraries
                        scope.filterObject.set.basic.allLibraries();

                        // initial filter dropdown option
                        scope.selectedFilterOption = 1;

                        filtersLogic.loadTagTaxons(false);

                        scope.filterObject.attachEvent(refresh);
                    }());
                }
            };
        }]);
})();