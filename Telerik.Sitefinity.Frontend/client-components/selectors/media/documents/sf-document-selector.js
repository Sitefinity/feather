﻿(function ($) {
    var sfSelectors = angular.module('sfSelectors');
    sfSelectors.requires.push('sfDocumentSelector');

    var sfDocumentSelector = angular.module('sfDocumentSelector', ['sfServices', 'sfInfiniteScroll', 'sfCollection', 'sfTree', 'sfSearchBox', 'sfSortBox', 'sfDragDrop', 'expander', 'sfBootstrapPopover']);
    sfDocumentSelector.directive('sfDocumentSelector', ['sfMediaService', 'sfMediaFilter', 'serverContext', 'serviceHelper', 'sfFlatTaxonService', 'sfHierarchicalTaxonService',
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
                recentDocumentsLastDaysCount: 7,
                filters: {
                    basic: [
                        { title: 'Recent Documents', value: 'recentItems' },
                        { title: 'My Documents', value: 'ownItems' },
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
                    provider: '=?sfProvider',
                    sfMultiselect: '@',
                    sfDeselectable: '@',
                    sfMediaSettings: '=?'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/media/documents/sf-document-selector.sf-cshtml';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {

                    // Ensures that modal is styled correctly.

                    element.parents('.modal[role=dialog]').first().addClass('modal-fluid');

                    /*
                    * Filters inner logic
                    */

                    var filtersLogic = {
                        // Library filter
                        loadLibraryChildren: function (parent) {
                            parent = parent || {};
                            return sfMediaService.documents.getFolders({ parent: parent.Id, provider: scope.provider, sort: 'Title ASC' }).then(function (response) {
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
                        scope.filters.library.selected = parent ? [parent] : [];

                        if (!scope.filterObject.parent) {
                            scope.filterObject.set.basic.allLibraries();
                            scope.filters.basic.selected = 'allLibraries';
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
                        var totalItemsCount = 0;

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
                                var getPromise = sfMediaService.documents.getPredecessorsFolders(scope.filterObject.parent, scope.provider);
                                if (getPromise) {
                                    getPromise.then(function (items) {
                                        scope.breadcrumbs = items;
                                    });
                                }
                            }

                            sfMediaService.documents.get(options, scope.filterObject, appendItems, scope.sfMediaSettings)
                                .then(function (response) {
                                    if (response && response.Items) {
                                        totalItemsCount = response.TotalCount;

                                        var removeNonNumeric = function (item) {
                                            return item.replace(/\D/g, "");
                                        };

                                        // Remove unnecessary (non-numeric) characters from LastModified string
                                        for (var i = 0; i < response.Items.length; i++) {
                                            var item = response.Items[i];
                                            if (item.LastModified) {
                                                item.LastModified = removeNonNumeric(item.LastModified);
                                            }

                                            // We can't retrive these properties for root libraries
                                            if (item.hasOwnProperty('DocumentsCount') && item.hasOwnProperty('LibrariesCount')){
                                                item.metricsAvailable = true;
                                            }

                                            if (item.DocumentsCount) {
                                                var countStr = removeNonNumeric(item.DocumentsCount);
                                                item.DocumentsCount = countStr + (countStr === '1' ? " document" : " documents");
                                            } else {
                                                item.DocumentsCount = "No documents";
                                            }

                                            if (item.LibrariesCount) {
                                                item.LibrariesCount = removeNonNumeric(item.LibrariesCount);
                                                item.LibrariesCount = item.LibrariesCount + (item.LibrariesCount == 1 ? " folder" : " folders");
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

                                    setTimeout(function() {
                                        var shouldLoadMoreItems = totalItemsCount > scope.items.length && element.find('.Media-items-holder').height() < element.find('.Media-items').height();
                                        if (shouldLoadMoreItems) {
                                            refresh(true);
                                        }
                                    }, 0);
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
                            var file = dataTransferObject.files[0];
                            var processUpload = function (settings) {
                                if (isNotAllowedExtension(settings, file)) {
                                    scope.error = {
                                        show: true,
                                        message: 'This file type is not allowed to upload. Only files with the following extensions are allowed: ' + settings.AllowedExensionsSettings
                                    };
                                    return;
                                }
                                if (!scope.isInUploadMode) {
                                    if (scope.selectedFilterOption == 1) {
                                        // set library id or null if in default library
                                        scope.model.parentId = getLibraryId();

                                        // if other files were dropped when category/tag were selected they should be cleaned
                                        scope.model.tags = [];
                                        scope.model.categories = [];
                                    }
                                    else if (scope.selectedFilterOption == 2) {
                                        if (scope.filters.tag.selected[0]) {
                                            scope.model.tags.push(scope.filters.tag.selected[0]);

                                            // if other files were dropped when category/tag were selected they should be cleaned
                                            scope.model.parentId = null;
                                            scope.model.categories = [];
                                        }
                                    }
                                    else if (scope.selectedFilterOption == 3) {
                                        if (scope.filters.category.selected[0]) {
                                            scope.model.categories.push(scope.filters.category.selected[0]);

                                            // if other files were dropped when category/tag were selected they should be cleaned
                                            scope.model.parentId = null;
                                            scope.model.tags = [];
                                        }
                                    }
                                }
                                openUploadPropertiesDialog(file);
                            };

                            if (scope.sfMediaSettings) {
                                processUpload(scope.sfMediaSettings);
                            } else {
                                sfMediaService.documents.getSettings().then(function (settings) {
                                    processUpload(settings);
                                });
                            }
                        }
                    };

                    // input logic
                    var fileUploadInput = element.find('.file-upload-chooser-input');
                    fileUploadInput.change(function () {
                        scope.$apply(function () {
                            var fileInput = fileUploadInput.get(0);
                            if (fileInput.files && fileInput.files[0]) {
                                var file = fileInput.files[0];
                                var processUpload = function (settings) {
                                    if (isNotAllowedExtension(settings, file)) {
                                        scope.error = {
                                            show: true,
                                            message: 'This file type is not allowed to upload. Only files with the following extensions are allowed: ' + settings.AllowedExensionsSettings
                                        };
                                        return;
                                    }
                                    if (!scope.isInUploadMode) {
                                        scope.model.parentId = getLibraryId();

                                        // if other files were dropped when category/tag were selected they should be cleaned
                                        scope.model.tags = [];
                                        scope.model.categories = [];
                                    }
                                    openUploadPropertiesDialog(file);
                                };

                                if (scope.sfMediaSettings) {
                                    processUpload(scope.sfMediaSettings);
                                } else {
                                    sfMediaService.documents.getSettings().then(function (settings) {
                                        processUpload(settings);
                                    });
                                }
                            }
                        });
                    });

                    var isNotAllowedExtension = function (settings, file) {
                        var allowedExensions = settings.AllowedExensionsSettings ? settings.AllowedExensionsSettings.toLowerCase() : settings.AllowedExensionsSettings;
                        var enableExtensionFiltering = true;

                        if(settings.hasOwnProperty('AllowedExensions')) {
                            enableExtensionFiltering = scope.$eval(settings.AllowedExensions.toString().toLowerCase());
                        }

                        return enableExtensionFiltering && allowedExensions && allowedExensions.search(getExtension(file.name)) == -1;
                    };

                    var getExtension = function (file) {
                        var idx = file.lastIndexOf(".");
                        var extension = "";

                        if (idx > -1) {
                            extension = file.substring(idx + 1);
                        }
                        return extension.toLowerCase();
                    };

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

                        var fileModelResolver = function () { return scope.model; };
                        var providerResolver = function () { return scope.provider; };

                        angular.element('.uploadPropertiesModal').scope().$openModalDialog({ sfFileModel: fileModelResolver, sfProvider: providerResolver })
                            .then(function (uploadedDocumentInfo) {
                                if (uploadedDocumentInfo && !uploadedDocumentInfo.ErrorMessage) {
                                    scope.$emit('sf-document-selector-document-uploaded', uploadedDocumentInfo);
                                }
                                else if (uploadedDocumentInfo && uploadedDocumentInfo.ErrorMessage) {
                                    scope.error = {
                                        show: true,
                                        message: uploadedDocumentInfo.ErrorMessage
                                    };
                                }
                            })
                            .finally(function () {
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

                    scope.isMultiselect = scope.sfMultiselect !== undefined && scope.sfMultiselect.toLowerCase() !== 'false';
                    scope.isDeselectable = scope.sfDeselectable !== undefined && scope.sfDeselectable.toLowerCase() !== 'false';

                    scope.uploadPropertiesTemplateUrl = serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'client-components/selectors/media/documents/sf-upload-document-properties.sf-cshtml');

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
                                scope.error = null;
                                scope.clearSearch = !scope.clearSearch;
                            },
                        },
                        library: {
                            index: 1,
                            selected: scope.filterObject && scope.filterObject.parent ? [scope.filterObject.parent] : [],
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
                        var handled = false;
                        switch (scope.selectedFilterOption) {
                            case '1':
                                if (scope.filters.library.selected.length > 0) {
                                    scope.filterObject.set.parent.to(scope.filters.library.selected[0], true);
                                    handled = true;
                                }
                                break;
                            case '2':
                                if (scope.filters.tag.selected.length > 0) {
                                    scope.filterObject.set.taxon.to(scope.filters.tag.selected[0], constants.filters.tags.field);
                                    handled = true;
                                }
                                break;
                            case '3':
                                if (scope.filters.category.selected.length > 0) {
                                    scope.filterObject.set.taxon.to(scope.filters.category.selected[0], constants.filters.categories.field);
                                    handled = true;
                                }
                                break;
                            case '4':
                                if (scope.filters.date.selected.length > 0) {
                                    if (scope.filters.date.selected[0] === constants.filters.anyDateValue) {
                                        scope.filterObject.set.date.all();
                                    }
                                    else {
                                        scope.filterObject.set.date.to(scope.filters.date.selected[0]);
                                    }
                                    handled = true;
                                }
                                break;
                        }
                        if (!handled) {
                            scope.filters.basic.select(scope.filters.basic.selected || constants.filters.basicRecentItemsValue);
                        }
                    };

                    // load more documents
                    scope.loadMore = function () {
                        refresh(true);
                    };

                    scope.getDateCreated = function (item) {
                        if (!item.DateCreated) return;

                        var date = item.DateCreated;
                        var startIndex = date.indexOf('(');
                        var endIndex = date.indexOf(')');
                        var dateCreated = date.substring(startIndex + 1, endIndex);
                        return dateCreated;
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

                    var clearNonSelectedFilters = function (filterName) {
                        if (filterName !== 'library') {
                            scope.filters.library.selected = [];
                        }

                        if (filterName !== 'tag') {
                            scope.filters.tag.selected = [];
                        }

                        if (filterName !== 'category') {
                            scope.filters.category.selected = [];
                        }

                        if (filterName !== 'date') {
                            scope.filters.date.selected = [];
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
                        scope.error = null;
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
                            clearNonSelectedFilters('library');
                            scope.filterObject.set.parent.to(newVal[0]);

                            scope.clearSearch = !scope.clearSearch;
                        }
                    });

                    scope.$watch('filters.tag.selected', function (newVal, oldVal) {
                        if (newVal !== oldVal && newVal && newVal[0]) {
                            scope.filters.basic.selected = null;
                            clearNonSelectedFilters('tag');
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
                            clearNonSelectedFilters('category');
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
                            clearNonSelectedFilters('date');
                            if (newVal[0] === constants.filters.anyDateValue) {
                                scope.filterObject.set.date.all();
                            }
                            else {
                                scope.filterObject.set.date.to(newVal[0]);
                            }

                            scope.clearSearch = !scope.clearSearch;
                        }
                    });

                    scope.$watch('selectedItems', function (newVal, oldVal) {
                        if (!scope.provider || scope.provider.length === 0) {
                            if (newVal && newVal.length > 0) {
                                scope.provider = newVal[newVal.length - 1].ProviderName;
                            }
                        }
                    });

                    scope.$watch('provider', function (newVal, oldVal) {
                        if (newVal === oldVal || !oldVal)
                            return;

                        // changing the provider should clear all selected documents from the previous provider
                        scope.selectedItems = [];

                        if (scope.filterObject.parent) {
                            scope.filters.basic.select(constants.filters.basicRecentItemsValue);
                        }
                        else {
                            refresh();
                        }

                        var libraryFilterScope = element.find('div.library-filter ul').scope();
                        if (libraryFilterScope) {
                            libraryFilterScope.bind();
                        }
                    });

                    // Reacts when a folder is clicked.
                    scope.$on('sf-collection-item-selected', function (event, data) {
                        var item = data.item;
                        scope.isInUploadMode = false;
                        if (item && item.IsFolder === true) {
                            data.cancel = true;
                            scope.filters.basic.selected = null;
                            scope.filterObject.set.parent.to(item.Id);
                        }
                    });

                    scope.$on('sf-tree-item-selected', function (event, data) {
                        scope.isInUploadMode = false;
                        scope.error = null;
                    });

                    scope.$watch('selectedFilterOption', function (event, data) {
                        scope.error = null;
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
                            scope.filterObject.status = attrs.sfMaster === 'true' || attrs.sfMaster === 'True' ? 'master' : 'live';
                            scope.filterObject.attachEvent(refresh);

                            // initial open populates dialog with recent documents
                            scope.filters.basic.select(constants.filters.basicRecentItemsValue);
                        }
                        else {
                            scope.filterObject.attachEvent(refresh);
                        }
                        //sfMediaService.documents.getSettings();
                    }());
                }
            };
        }])

        /*
        * Upload properties controller
        */

        .controller('SfDocumentSelectorUploadPropertiesCtrl', ['$scope', '$uibModalInstance', 'sfMediaService', 'sfFileModel', 'sfProvider', function myfunction($scope, $uibModalInstance, sfMediaService, sfFileModel, sfProvider) {
            $scope.model = sfFileModel;
            $scope.provider = sfProvider;

            $scope.model.file.textSize = Math.ceil($scope.model.file.size / 1024) + " KB";
            

            var fileName = $scope.model.file.name;
            $scope.uploadInfo = {};
            $scope.uploadInfo.fileName = fileName;

            $scope.model.title = fileName.slice(0, fileName.lastIndexOf('.'));
            $scope.model.file.extension = fileName.split('.').pop();

            var successAction = function (data) {
                data = data || {};
                $scope.model.uploadInProgress = false;
                $uibModalInstance.close(data[0]);
            };

            var progressAction = function (progress) {
                $scope.uploadInfo.percentage = progress;
            };

            var errorAction = function (err) {
                console.log(err);
                $uibModalInstance.dismiss();
            };

            $scope.uploadDocument = function () {
                $scope.model.uploadInProgress = true;
                sfMediaService.documents.upload($scope.model, sfProvider).then(successAction, errorAction, progressAction);
            };

            $scope.cancelUpload = function () {
                $uibModalInstance.dismiss();
            };
        }])

        .directive('sfScrollIfSelected', [function () {
            return {
                link: function (scope, element) {
                    if (scope.isSelected(scope.item)) {
                        setTimeout(function () {
                            element[0].scrollIntoView();
                        }, 0);
                    }
                }
            };
        }]);

})(jQuery);