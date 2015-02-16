; (function () {
    var sfSelectors = angular.module('sfSelectors');
    sfSelectors.requires.push('sfImageSelector');

    angular.module('sfImageSelector', ['sfServices', 'sfInfiniteScroll', 'sfCollection', 'sfTree', 'sfSearchBox', 'sfSortBox', 'sfDragDrop', 'expander'])
        .directive('sfImageSelector', ['sfMediaService', 'sfMediaFilter', 'serverContext', 'serviceHelper', 'sfFlatTaxonService', 'sfHierarchicalTaxonService',
        function (sfMediaService, sfMediaFilter, serverContext, serviceHelper, sfFlatTaxonService, sfHierarchicalTaxonService) {
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
                    basic: [
                        { title: 'Recent Images', value: 'recentItems' },
                        { title: 'My Images', value: 'ownItems' },
                        { title: 'All Libraries', value: 'allLibraries' }
                    ],
                    basicRecentItemsValue: 'recentItems',
                    anyDateValue: 'AnyTime',
                    dates: [
                        { text: 'Any time', dateValue: 'AnyTime' },
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
                        pageSize: 30,
                        field: 'Tags',
                        taxonomyId: 'CB0F3A19-A211-48a7-88EC-77495C0F5374'
                    },
                    categories: {
                        pageSize: 30, // not used by the service
                        field: 'Category',
                        taxonomyId: 'E5CD6D69-1543-427B-AD62-688A99F5E7D4'
                    }
                },
                sorting: {
                    defaultValue: 'DateCreated DESC',
                    dateCreatedDesc: 'DateCreated DESC',
                    lastModifiedDesc: 'LastModified DESC',
                    titleAsc: 'Title ASC',
                    titleDesc: 'Title DESC'
                }
            };

            return {
                restrict: 'E',
                scope: {
                    selectedItems: '=?sfModel',
                    filterObject: '=?sfFilter',
                    provider: '=?sfProvider'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/media/sf-image-selector.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {

                    /*
                    * Filters inner logic
                    */

                    var filtersLogic = {
                        // Library filter
                        loadLibraryChildren: function (parent) {
                            parent = parent || {};
                            return sfMediaService.images.getFolders({ parent: parent.Id, provider: scope.provider }).then(function (response) {
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

                                    if (!append) {
                                        // scrolls the collection of items to the top
                                        element.find('.sf-Tree').scrollTop(0);
                                    }
                                });
                        },
                        loadMoreTags: function () {
                            filtersLogic.loadTagTaxons(true);
                        }
                    };

                    // breadcrumb logic
                    scope.onBreadcrumbItemClick = function (item) {
                        var parent = item ? item.Id : null;

                        if (parent && parent === scope.filterObject.parent) {
                            return;
                        }

                        scope.filterObject.parent = parent;
                        if (!scope.filterObject.parent) {
                            scope.filterObject.set.basic.allLibraries();
                        }
                        refresh();
                    };

                    // view mode alternation
                    scope.isGrid = true;
                    scope.switchToGrid = function () {
                        scope.isGrid = true;
                        scope.isList = false;
                    };
                    scope.switchToList = function () {
                        scope.isGrid = false;
                        scope.isList = true;
                    };

                    /*
                    * Content collection refresh
                    */

                    var refresh = function (appendItems) {
                        if (scope.isLoading) {
                            return;
                        }

                        scope.breadcrumbs = [];

                        var options = {
                            parent: scope.filterObject.parent,
                            sort: scope.sortExpression,
                            provider: scope.provider
                        };

                        if (appendItems) {
                            options.skip = scope.items.length;
                            options.take = constants.infiniteScrollLoadedItemsCount;
                        }
                        else {
                            options.take = constants.initialLoadedItemsCount;
                        }

                        if (scope.isLoading === false) {
                            scope.isLoading = true;

                            var itemsLength = scope.items ? scope.items.length : 0;

                            if (!scope.filterObject.query && !scope.filterObject.basic) {
                                var getPromise = sfMediaService.images.getPredecessorsFolders(scope.filterObject.parent, scope.provider);
                                if (getPromise) {
                                    getPromise.then(function (items) {
                                        scope.breadcrumbs = items;
                                    });
                                }
                            }

                            sfMediaService.images.get(options, scope.filterObject, appendItems)
                                .then(function (response) {
                                    if (response && response.Items) {

                                        var removeNonNumeric = function (item) {
                                            return item.replace(/\D/g, "");
                                        };

                                        // Remove unnecessary (non-numeric) characters from LastModified string
                                        for (var key in response.Items) {
                                            var item = response.Items[key];
                                            if (item.LastModified) {
                                                item.LastModified = removeNonNumeric(item.LastModified);
                                            }
                                            if (item.ImagesCount) {
                                                item.ImagesCount = removeNonNumeric(item.ImagesCount);
                                            }
                                            if (item.LibrariesCount) {
                                                item.LibrariesCount = removeNonNumeric(item.LibrariesCount);
                                            }
                                        }

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

                                    if (!appendItems) {
                                        // scrolls the collection of items to the top
                                        element.find('.Media-items').scrollTop(0);
                                    }
                                });
                        }
                    };

                    /*
                    * File uploading
                    */

                    // fetching of library when file is dropped in library filter mode
                    var getLibraryId = function () {
                        if (scope.breadcrumbs && scope.breadcrumbs.length) {
                            return scope.breadcrumbs[scope.breadcrumbs.length - 1].Id;
                        }
                        else {
                            return null;
                        }
                    };

                    // drag-drop logic
                    scope.dataTransferDropped = function (dataTransferObject) {
                        // using only the first file
                        if (dataTransferObject.files && dataTransferObject.files[0]) {
                            if (!scope.isInUploadMode) {
                                if (scope.selectedFilterOption == 1) {
                                    // set library id or null if in default library
                                    scope.model.parentId = getLibraryId();
                                }
                                else if (scope.selectedFilterOption == 2) {
                                    if (scope.filters.tag.selected[0]) {
                                        scope.model.tags.push(scope.filters.tag.selected[0]);
                                    }
                                }
                                else if (scope.selectedFilterOption == 3) {
                                    if (scope.filters.category.selected[0]) {
                                        scope.model.categories.push(scope.filters.category.selected[0]);
                                    }
                                }
                            }

                            openUploadPropertiesDialog(dataTransferObject.files[0]);
                        }
                    };

                    // input logic
                    var fileUploadInput = element.find('.file-upload-chooser-input');
                    fileUploadInput.change(function () {
                        scope.$apply(function () {
                            var fileInput = fileUploadInput.get(0);
                            if (fileInput.files && fileInput.files[0]) {
                                if (!scope.isInUploadMode) {
                                    scope.model.parentId = getLibraryId();
                                }

                                openUploadPropertiesDialog(fileInput.files[0]);
                            }
                        });
                    });

                    // called when 'select from your computer' link is clicked
                    scope.openSelectFileDialog = function () {
                        // // call the click event in a timeout to avoid digest loop
                        setTimeout(function () {
                            fileUploadInput.click();
                        }, 0);

                        return false;
                    };

                    // Upload properties logic
                    var openUploadPropertiesDialog = function (file) {
                        scope.model.file = file;

                        angular.element('.uploadPropertiesModal').scope().$openModalDialog({ sfFileModel: function () { return scope.model; } })
                            .then(function (uploadedImageInfo) {
                                if (uploadedImageInfo) {
                                    scope.$emit('sf-image-selector-image-uploaded', uploadedImageInfo);
                                }

                                restoreFileModel();
                            });
                    };

                    // cleares both scope model and html input
                    var restoreFileModel = function () {
                        // remove the selected file - if missing change will not trigger on file select -> cancel -> same file select
                        fileUploadInput.replaceWith(fileUploadInput = fileUploadInput.clone(true));

                        scope.model = {
                            file: null,
                            parentId: null,
                            title: null,
                            alternativeText: null,
                            categories: [],
                            tags: []
                        };
                    };

                    /*
                    * Scope properties
                    */

                    scope.sortExpression = null;
                    scope.items = [];
                    scope.isLoading = false;
                    scope.showSortingAndView = false;
                    scope.clearSearch = false;

                    if (!scope.selectedItems || !angular.isArray(scope.selectedItems)) {
                        scope.selectedItems = [];
                    }

                    scope.uploadPropertiesTemplateUrl = serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'client-components/selectors/media/sf-upload-image-properties.html');

                    scope.filters = {
                        basic: {
                            all: constants.filters.basic,
                            selected: null,
                            select: function (basicFilter) {
                                scope.isInUploadMode = false;
                                scope.filters.basic.selected = basicFilter;

                                if (basicFilter === constants.filters.basic[0].value) {
                                    scope.sortExpression = constants.sorting.defaultValue;
                                }

                                scope.filterObject.set.basic[basicFilter]();

                                scope.filters.library.selected = [];
                                scope.filters.date.selected = [];
                                scope.filters.tag.selected = [];
                                scope.filters.category.selected = [];

                                scope.clearSearch = !scope.clearSearch;
                            },
                        },
                        library: {
                            index: 1,
                            selected: [],
                            getChildren: filtersLogic.loadLibraryChildren
                        },
                        tag: {
                            all: [],
                            selected: [],
                            query: null,
                            isLoading: false,
                            loadMore: filtersLogic.loadMoreTags
                        },
                        category: {
                            filtered: [],
                            selected: [],
                            query: null,
                            getChildren: filtersLogic.loadCategoryChildren
                        },
                        date: {
                            all: constants.filters.dates,
                            selected: []
                        }
                    };

                    scope.narrowResults = function (query) {
                        if (query) {
                            scope.filterObject.set.query.to(query);
                            return;
                        }
                        switch (scope.selectedFilterOption) {
                            case '1':
                                if (scope.filters.library.selected)
                                    scope.filterObject.set.parent.to(scope.filters.library.selected[0], true);
                                break;
                            case '2':
                                if (scope.filters.tag.selected)
                                    scope.filterObject.set.taxon.to(scope.filters.tag.selected[0], constants.filters.tags.field);
                                break;
                            case '3':
                                if (scope.filters.category.selected)
                                    scope.filterObject.set.taxon.to(scope.filters.category.selected[0], constants.filters.categories.field);
                                break;
                            case '4':
                                if (scope.filters.date.selected) {
                                    if (scope.filters.date.selected[0] === constants.filters.anyDateValue) {
                                        scope.filterObject.set.date.all();
                                    }
                                    else {
                                        scope.filterObject.set.date.to(scope.filters.date.selected[0]);
                                    }
                                }
                                break;

                            default:
                                scope.filters.basic.select(scope.filters.basic.selected || constants.filters.basicRecentItemsValue);
                        }
                    };

                    // load more images
                    scope.loadMore = function () {
                        refresh(true);
                    };

                    var extractDate = function (dateString) {
                        return parseInt(dateString.substring(dateString.indexOf('Date') + 'Date('.length, dateString.indexOf(')')));
                    };

                    var reorderItems = function (val) {
                        if (val === constants.sorting.dateCreatedDesc) {
                            scope.items.sort(function (a, b) {
                                return extractDate(b.DateCreated) - extractDate(a.DateCreated);
                            });
                        }
                        else if (val === constants.sorting.lastModifiedDesc) {
                            scope.items.sort(function (a, b) {
                                return extractDate(b.DateModified) - extractDate(a.DateModified);
                            });
                        }
                        else if (val === constants.sorting.titleDesc) {
                            scope.items.sort(function (a, b) {
                                return b.Title.localeCompare(a.Title);
                            });
                        }
                        else if (val === constants.sorting.titleAsc) {
                            scope.items.sort(function (a, b) {
                                return a.Title.localeCompare(b.Title);
                            });
                        }
                    };

                    scope.switchToUploadMode = function () {
                        scope.isInUploadMode = !scope.isInUploadMode;
                        // clear filter selection
                        scope.filters.basic.selected = null;
                        scope.filters.library.selected = [];
                        scope.filters.date.selected = [];
                        scope.filters.tag.selected = [];
                        scope.filters.category.selected = [];
                    };

                    /*
                    * Watches.
                    */

                    scope.$watch('sortExpression', function (newVal, oldVal) {
                        if (newVal !== oldVal) {
                            if (scope.filterObject.basic === scope.filterObject.constants.basic.recentItems) {
                                // In recent items we reorder the items on client side
                                reorderItems(newVal);
                            }
                            else {
                                refresh();
                            }
                        }
                    });

                    scope.$watch('filters.library.selected', function (newVal, oldVal) {
                        if (newVal !== oldVal && newVal && newVal[0]) {
                            scope.filters.basic.selected = null;
                            scope.filterObject.set.parent.to(newVal[0]);

                            scope.clearSearch = !scope.clearSearch;
                        }
                    });

                    scope.$watch('filters.tag.selected', function (newVal, oldVal) {
                        if (newVal !== oldVal && newVal && newVal[0]) {
                            scope.filters.basic.selected = null;
                            scope.filterObject.set.taxon.to(newVal[0], constants.filters.tags.field);

                            scope.clearSearch = !scope.clearSearch;
                        }
                    });

                    scope.$watch('filters.tag.query', function (newVal, oldVal) {
                        if (newVal !== oldVal) {
                            filtersLogic.loadTagTaxons(false);
                        }
                    });

                    scope.$watch('filters.category.selected', function (newVal, oldVal) {
                        if (newVal !== oldVal && newVal && newVal[0]) {
                            scope.filters.basic.selected = null;
                            scope.filterObject.set.taxon.to(newVal[0], constants.filters.categories.field);

                            scope.clearSearch = !scope.clearSearch;
                        }
                    });

                    scope.$watch('filters.category.query', function (newVal, oldVal) {
                        if (newVal !== oldVal) {
                            filtersLogic.getCategoryTaxons().then(function (items) {
                                scope.filters.category.filtered = items;
                            });
                        }
                    });

                    scope.$watch('filters.date.selected', function (newVal, oldVal) {
                        if (newVal !== oldVal && newVal[0]) {
                            scope.filters.basic.selected = null;
                            if (newVal[0] === constants.filters.anyDateValue) {
                                scope.filterObject.set.date.all();
                            }
                            else {
                                scope.filterObject.set.date.to(newVal[0]);
                            }

                            scope.clearSearch = !scope.clearSearch;
                        }
                    });

                    scope.$watch('provider', function (newVal, oldVal) {
                        if (newVal === oldVal || !oldVal)
                            return;

                        if (scope.filterObject.parent) {
                            scope.filters.basic.select(constants.filters.basicRecentItemsValue);
                        }
                        else {
                            refresh();
                        }

                        element.find('div.library-filter ul').scope().bind();
                    });

                    // Reacts when a folder is clicked.
                    scope.$on('sf-collection-item-selected', function (event, data) {
                        scope.isInUploadMode = false;
                        if (data && data.IsFolder === true) {
                            scope.filters.basic.selected = null;
                            scope.filterObject.set.parent.to(data.Id);
                        }
                    });

                    scope.$on('sf-tree-item-selected', function (event, data) {
                        scope.isInUploadMode = false;
                    });

                    /*
                    * Initialization.
                    */

                    (function initializeWindow() {
                        // set initial file model
                        restoreFileModel();

                        // initial filter dropdown option
                        scope.selectedFilterOption = '1';

                        filtersLogic.loadTagTaxons(false);

                        if (!scope.filterObject) {
                            scope.filterObject = sfMediaFilter.newFilter();
                            scope.filterObject.attachEvent(refresh);

                            // initial open populates dialog with recent images
                            scope.filters.basic.select(constants.filters.basicRecentItemsValue);
                        }
                        else {
                            scope.filterObject.attachEvent(refresh);
                        }
                    }());
                }
            };
        }])

        /*
        * Upload properties controller
        */

        .controller('SfImageSelectorUploadPropertiesCtrl', ['$scope', '$modalInstance', 'sfMediaService', 'sfFileModel', function myfunction($scope, $modalInstance, sfMediaService, sfFileModel) {
            $scope.model = sfFileModel;

            $scope.model.file.textSize = Math.ceil($scope.model.file.size / 1000) + " KB";

            var fileName = $scope.model.file.name;
            $scope.uploadInfo = {};
            $scope.uploadInfo.fileName = fileName;

            $scope.model.title = fileName.slice(0, fileName.lastIndexOf('.'));

            var successAction = function (data) {
                data = data || {};
                $modalInstance.close(data[0]);
            };

            var progressAction = function (progress) {
                $scope.uploadInfo.percentage = progress;
            };

            var errorAction = function (err) {
                console.log(err);
                $modalInstance.dismiss();
            };

            $scope.uploadImage = function () {
                sfMediaService.images.upload($scope.model).then(successAction, errorAction, progressAction);
            };

            $scope.cancelUpload = function () {
                $modalInstance.dismiss();
            };
        }]);
})();
