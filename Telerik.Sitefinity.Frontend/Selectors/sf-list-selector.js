(function ($) {
    angular.module('sfSelectors')
        .directive('sfListSelector', ['serverContext', function (serverContext) {
            return {
                restrict: 'E',
                transclude: true,
                scope: {
                    //For single selection
                    selectedItemId: '=?',
                    selectedItem: '=?',

                    //For multiple selection
                    selectedItems: '=?',
                    selectedIds: '=?',

                    provider: '=?',
                    change: '=',
                    taxonomyId: '=?', /* sf-taxon-selector */
                    itemType: '=?', /* sf-dynamic-items-selector */
                    identifierField: '=?'
                },
                controller: function ($scope) {
                    this.defaultIdentifierField = 'Title';

                    this.identifierField = $scope.identifierField || this.defaultIdentifierField;

                    this.bindIdentifierField = function (item) {
                        if (item) {
                            var mainField = item[this.identifierField];
                            var valueProp = 'Value';

                            if (!mainField) {
                                return item.Id;
                            }

                            if (typeof mainField === 'string') {
                                return mainField;
                            }
                            else if (valueProp in mainField) {
                                return mainField.Value;
                            }
                        }
                    };

                    this.$scope = $scope;

                    this.updateSelection = function (selectedItems) {
                        selectedItems.sort(compareFunction);

                        var firstItem = selectedItems[0];
                        $scope.selectedItem = firstItem;
                        $scope.selectedItemId = firstItem && firstItem.Id;

                        $scope.selectedItems = selectedItems;
                        $scope.selectedIds = selectedItems.map(function (item) {
                            return item.Id;
                        });
                    };

                    var compareFunction = function (item1, item2) {
                        var orderedIds = $scope.getSelectedIds();

                        var index1 = orderedIds.indexOf(item1.Id);
                        var index2 = orderedIds.indexOf(item2.Id);

                        if (index1 < index2) {
                            return -1;
                        }
                        if (index1 > index2) {
                            return 1;
                        }
                        return 0;
                    };
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.templateUrl || 'Selectors/sf-list-selector.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    post: function (scope, element, attrs, ctrl, transclude) {
                        // ------------------------------------------------------------------------
                        // Event handlers
                        // ------------------------------------------------------------------------

                        var onFirstPageLoadedSuccess = function (data) {
                            scope.noItemsExist = !data.Items.length;
                            scope.paging.skip += data.Items.length;

                            if (scope.multiselect) {
                                Array.prototype.push.apply(scope.items, data.Items);
                            }
                            else {
                                pushSelectedItemToTheTop();
                                pushNotSelectedItems(data.Items);
                            }

                            if (scope.selectedItems) {
                                Array.prototype.push.apply(scope.selectedItemsInTheDialog, scope.selectedItems.map(function (item) {
                                    return {
                                        item: item,
                                        isChecked: true
                                    };
                                }));
                            }

                            scope.collectSelectedItems();
                        };

                        var onItemsFilteredSuccess = function (data) {
                            scope.paging.skip += data.Items.length;

                            if (!scope.multiselect && !scope.filter.searchString) {
                                scope.items = [];
                                pushSelectedItemToTheTop();
                                pushNotSelectedItems(data.Items);
                            }
                            else {
                                scope.items = data.Items;
                            }
                        };

                        var onError = function (error) {
                            var errorMessage = '';
                            if (error && error.data && error.data.ResponseStatus) {
                                errorMessage = error.data.ResponseStatus.Message;
                            }
                            else if (error && error.statusText) {
                                errorMessage = error.statusText;
                            }

                            scope.showError = true;
                            scope.errorMessage = errorMessage;
                        };

                        // ------------------------------------------------------------------------
                        // helper methods
                        // ------------------------------------------------------------------------

                        var emptyGuid = '00000000-0000-0000-0000-000000000000';

                        var currentSelectedIds = scope.selectedIds;

                        var pushSelectedItemToTheTop = function () {
                            if (scope.items.length === 0 && scope.selectedItems && scope.selectedItems.length > 0) {
                                scope.items.push(scope.selectedItems[0]);
                            }
                        };

                        var pushNotSelectedItems = function (items) {
                            var ids = scope.getSelectedIds();

                            Array.prototype.push.apply(scope.items,
                                items.filter(function (item) {
                                    return ids.indexOf(item.Id) < 0;
                                }));
                        };

                        var getSelectedItems = function () {
                            var ids = scope.getSelectedIds();
                            if (ids.length === 0) {
                                return;
                            }

                            ctrl.getSpecificItems(ids)
                                .then(ctrl.onSelectedItemsLoadedSuccess, onError)
                                .finally(function () {
                                    scope.showLoadingIndicator = false;
                                });
                        };

                        var resetItems = function () {
                            scope.paging.skip = 0;
                            scope.filter.searchString = null;
                            scope.items = [];
                            scope.selectedItemsInTheDialog = [];
                            scope.selectedItemsViewData = [];
                        };

                        var areArrayEquals = function (arr1, arr2) {
                            if (arr1 && arr2) {
                                var clonedArr1 = [].concat(arr1);
                                var clonedArr2 = [].concat(arr2);

                                return clonedArr1.sort().toString() === clonedArr2.sort().toString();
                            }
                            return false;
                        };

                        // ------------------------------------------------------------------------
                        // Scope variables and setup
                        // ------------------------------------------------------------------------

                        scope.$watch('provider', function (newProvider, oldProvider) {
                            if (newProvider !== oldProvider) {
                                if (ctrl.selectorType === 'NewsSelector') {
                                    scope.selectedItems = null;
                                    scope.selectedIds = null;
                                }
                            }
                        });

                        scope.$watchCollection('selectedIds', function (newIds, oldIds) { 
                            if (newIds && newIds.length > 0 && !areArrayEquals(newIds, currentSelectedIds)) {
                                getSelectedItems();
                            }
                        });

                        scope.showError = false;
                        scope.selectedItemsViewData = [];
                        scope.items = [];
                        scope.filter = {
                            placeholder: 'Narrow by typing',
                            timeoutMs: 500,
                            search: function (keyword) {
                                var endlessScroll = angular.element($("[endless-scroll]"))[0];
                                if (endlessScroll) {
                                    endlessScroll.scrollTop = 0;
                                }
                                scope.showLoadingIndicator = true;
                                scope.paging.skip = 0;
                                var skip = scope.paging.skip;
                                var take = scope.paging.take;
                                var languages = serverContext.getFrontendLanguages();
                                return ctrl.getItems(skip, take, keyword, languages)
                                    .then(onItemsFilteredSuccess, onError)
                                    .finally(function () {
                                        scope.showLoadingIndicator = false;
                                    });
                            }
                        };
                        scope.paging = {
                            skip: 0,
                            take: 20,
                            getPage: function () {
                                return ctrl.getItems(this.skip, this.take, scope.filter.searchString);
                            },

                            pageLoaded: function (items) {
                                if (!scope.multiselect && !scope.filter.searchString) {
                                    pushNotSelectedItems(items);
                                }
                                else {
                                    Array.prototype.push.apply(scope.items, items);
                                }                                
                            }
                        };

                        scope.itemClicked = function (index, item) {
                            if (typeof index === 'object' && !item) item = index;

                            var alreadySelected;
                            var selectedItemIndex;
                            for (var i = 0; i < scope.selectedItemsInTheDialog.length; i++) {
                                if (scope.selectedItemsInTheDialog[i].item.Id === item.Id) {
                                    alreadySelected = true;
                                    selectedItemIndex = i;
                                    break;
                                }
                            }

                            if (alreadySelected) {
                                scope.selectedItemsInTheDialog.splice(selectedItemIndex, 1);
                            }
                            else {
                                if (scope.multiselect) {
                                    scope.selectedItemsInTheDialog.push({ item: item, isChecked: true });
                                }
                                else {
                                    scope.selectedItemsInTheDialog.splice(0, 1, { item: item, isChecked: true });
                                }
                            }
                        };

                        scope.doneSelecting = function () {
                            scope.removeUnselectedItems();

                            if (scope.selectedItemsInTheDialog.length > 0) {
                                if (scope.change) {
                                    var oldSelectedItems = [];
                                    Array.prototype.push.apply(oldSelectedItems, scope.selectedItems);
                                    var changeArgs = {
                                        "newSelectedItems": scope.selectedItemsInTheDialog.map(function (item) {
                                            return item.item;
                                        }),
                                        "oldSelectedItems": oldSelectedItems
                                    };
                                    scope.change.call(scope.$parent, changeArgs);
                                }

                                //set the selected item and its id to the mapped isolated scope properties
                                scope.selectedItem = scope.selectedItemsInTheDialog[0].item;
                                scope.selectedItemId = scope.selectedItemsInTheDialog[0].item.Id;

                                if (scope.selectedItems) {
                                    //Clean the array and keep all references.
                                    scope.selectedItems.length = 0;
                                }
                                else {
                                    scope.selectedItems = [];
                                }

                                Array.prototype.push.apply(scope.selectedItems, scope.selectedItemsInTheDialog.map(function (item) {
                                    return item.item;
                                }));

                                scope.selectedIds = scope.selectedItems.map(function (item) {
                                    return item.Id;
                                });
                            }
                            else {
                                scope.selectedItem = null;
                                scope.selectedItemId = null;
                                scope.selectedItems = null;
                                scope.selectedIds = null;
                            }

                            resetItems();
                            scope.$modalInstance.close();
                        };

                        scope.cancel = function () {
                            resetItems();
                            scope.$modalInstance.close();
                        };

                        scope.open = function () {
                            scope.$openModalDialog();

                            scope.showLoadingIndicator = true;

                            ctrl.getItems(scope.paging.skip, scope.paging.take)
                            .then(onFirstPageLoadedSuccess, onError)
                            .catch(onError)
                            .finally(function () {
                                scope.showLoadingIndicator = false;
                            });
                        };

                        scope.getDialogTemplate = function () {
                            var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                            var url = attrs.sfDialogTemplate || ctrl.dialogTemplateUrl;
                            return serverContext.getEmbeddedResourceUrl(assembly, url);
                        };

                        scope.getClosedDialogTemplate = function () {
                            var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                            var url = attrs.sfClosedDialogTemplate || ctrl.closedDialogTemplateUrl;
                            return serverContext.getEmbeddedResourceUrl(assembly, url);
                        };

                        scope.isItemSelected = function () {
                            var ids = scope.getSelectedIds().filter(function (id) {
                                return id !== emptyGuid;
                            });

                            return ids.length > 0;
                        };
                        
                        scope.isItemSelectedInDialog = function (item) {
                            for (var i = 0; i < scope.selectedItemsInTheDialog.length; i++) {
                                if (scope.selectedItemsInTheDialog[i].item.Id === item.Id) {
                                    return true;
                                }
                            }
                        };

                        scope.getSelectedItemsCount = function () {
                            return scope.selectedItemsInTheDialog.filter(function (item) {
                                return item.isChecked;
                            }).length;
                        };

                        scope.multiselect = !!attrs.multiselect;

                        scope.selectedItemsInTheDialog = [];

                        scope.showLoadingIndicator = true;

                        scope.bindIdentifierField = function (item) {
                            return ctrl.bindIdentifierField(item);
                        };

                        scope.getChildren = function (parentId) {
                            return ctrl.getChildren(parentId, scope.filter.searchString);
                        };

                        scope.getSelectedIds = function () {
                            if (attrs.multiselect) {
                                if (scope.selectedIds && scope.selectedIds.length > 0) {
                                    return scope.selectedIds;
                                }
                                else if (scope.selectedItems && scope.selectedItems.length > 0) {
                                    return scope.selectedItems.map(function (item) {
                                        return item.Id;
                                    });
                                }
                            }
                            else {
                                var id = (scope.selectedItem && scope.selectedItem.Id) || scope.selectedItemId;
                                if (id) {
                                    var selected = [];
                                    selected.push(id);
                                    return selected;
                                }
                            }

                            return [];
                        };

                        scope.collectSelectedItems = function () {
                            if (scope.multiselect) {
                                scope.selectedItemsViewData = [];
                                Array.prototype.push.apply(scope.selectedItemsViewData, scope.selectedItemsInTheDialog);
                            }
                        };

                        scope.removeUnselectedItems = function () {
                            scope.selectedItemsViewData = [];
                            scope.selectedItemsInTheDialog = scope.selectedItemsInTheDialog.filter(function (item) {
                                return item.isChecked;
                            });
                        };

                        if (scope.selectedIds && scope.selectedIds.length !== 0) {
                            scope.selectedIds = scope.selectedIds.filter(function (value) {
                                return value && value !== "";
                            });
                        }

                        getSelectedItems();

                        transclude(scope, function (clone) {
                            var hasContent;
                            for (var i = 0; i < clone.length; i++) {
                                var currentHtml = clone[i] && clone[i].outerHTML;

                                //check if the content is not empty string or white space only
                                hasContent = currentHtml && !/^\s*$/.test(currentHtml);

                                if (hasContent) {
                                    element.find('#selectedItemsPlaceholder').empty().append(clone);
                                    break;
                                }
                            }
                        });
                    }
                }
            };
        }]);
})(jQuery);
