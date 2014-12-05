(function ($) {
    angular.module('sfSelectors')
        .directive('sfListSelector', ['serverContext', '$q', function (serverContext, $q) {
            return {
                restrict: 'E',
                transclude: true,
                scope: {
                    //For single selection
                    sfSelectedItemId: '=?',
                    sfSelectedItem: '=?',

                    //For multiple selection
                    sfSelectedItems: '=?',
                    sfSelectedIds: '=?',

                    sfProvider: '=?',
                    sfChange: '=',
                    sfSortable: '=?',
                    sfItemType: '=?', /* sf-dynamic-items-selector */
                    sfIdentifierField: '@?',
                    sfDialogHeader: '@?'
                },
                controller: function ($scope) {
                    this.defaultIdentifierField = 'Title';

                    this.identifierField = $scope.sfIdentifierField || this.defaultIdentifierField;

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

                    this.onSelectedItemsLoadedSuccess = function (data) {
                        this.updateSelection(data.Items);
                    };

                    this.updateSelection = function (selectedItems) {
                        selectedItems.sort(compareFunction);

                        var firstItem = selectedItems[0];
                        $scope.sfSelectedItem = firstItem;
                        $scope.sfSelectedItemId = firstItem && firstItem.Id;

                        $scope.sfSelectedItems = selectedItems;
                        $scope.sfSelectedIds = selectedItems.map(function (item) {
                            return item.Id;
                        });
                    };

                    this.onFilterItemSucceeded = function (items) {
                    };

                    this.onItemSelected = function (item) {
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
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/common/sf-list-selector.html';
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
                            return scope.items;
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

                            if (ctrl.onFilterItemSucceeded) {
                                ctrl.onFilterItemSucceeded(scope.items);
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

                        var currentSelectedIds;

                        var pushSelectedItemToTheTop = function () {
                            if (scope.items.length === 0 && scope.sfSelectedItems && scope.sfSelectedItems.length > 0) {
                                scope.items.push(scope.sfSelectedItems[0]);
                            }
                        };

                        var pushNotSelectedItems = function (items) {
                            var ids = scope.getSelectedIds();

                            Array.prototype.push.apply(scope.items,
                                items.filter(function (item) {
                                    return ids.indexOf(item.Id) < 0;
                                }));
                        };

                        var fetchSelectedItems = function () {
                            var ids = scope.getSelectedIds();
                            currentSelectedIds = ids;

                            if (ids.length === 0) {
                                return;
                            }

                            return ctrl.getSpecificItems(ids)
                                .then(function (data) {
                                    ////ctrl.updateSelection(data.Items);
                                    ctrl.onSelectedItemsLoadedSuccess(data);
                                }, onError)
                                .finally(function () {
                                    scope.showLoadingIndicator = false;
                                });
                        };

                        var ensureSelectionIsUpToDate = function () {
                            $q.when(fetchSelectedItems()).then(function () {
                                updateSelectionInTheDialog();

                                scope.collectSelectedItems();
                            });
                        };

                        var updateSelectionInTheDialog = function () {
                            if (scope.sfSelectedItems) {
                                Array.prototype.push.apply(scope.selectedItemsInTheDialog,
                                    scope.sfSelectedItems.map(function (item) {
                                        return {
                                            item: item,
                                            isChecked: true
                                        };
                                    }));
                            }
                        };

                        var resetItems = function () {
                            scope.paging.skip = 0;
                            scope.paging.areAllItemsLoaded = false;
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

                        scope.$watch('sfProvider', function (newProvider, oldProvider) {
                            if (newProvider !== oldProvider) {
                                if (ctrl.selectorType === 'NewsSelector') {
                                    scope.sfSelectedItems = null;
                                    scope.sfSelectedIds = null;
                                }
                            }
                        });

                        scope.$watchCollection('sfSelectedIds', function (newIds, oldIds) {
                            if (newIds && newIds.length > 0 && !areArrayEquals(newIds, currentSelectedIds)) {
                                fetchSelectedItems();
                            }
                        });

                        scope.showError = false;
                        scope.selectedItemsViewData = [];
                        scope.items = [];
                        scope.filter = {
                            placeholder: 'Narrow by typing',
                            timeoutMs: 500,
                            search: function (keyword) {
                                scope.paging.areAllItemsLoaded = false;
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
                            areAllItemsLoaded: false,
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

                            if (ctrl.onItemSelected) {
                                ctrl.onItemSelected(item);
                            }

                            if (scope.itemDisabled(item)) {
                                return;
                            }

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

                            if (scope.sfChange) {
                                var oldSelectedItems = [];
                                Array.prototype.push.apply(oldSelectedItems, scope.sfSelectedItems);
                                var changeArgs = {
                                    "newSelectedItems": scope.selectedItemsInTheDialog.map(function (item) {
                                        return item.item;
                                    }),
                                    "oldSelectedItems": oldSelectedItems
                                };
                                scope.sfChange.call(scope.$parent, changeArgs);
                            }

                            if (scope.selectedItemsInTheDialog.length > 0) {
                                //set the selected item and its id to the mapped isolated scope properties
                                scope.sfSelectedItem = scope.selectedItemsInTheDialog[0].item;
                                scope.sfSelectedItemId = scope.selectedItemsInTheDialog[0].item.Id;

                                if (scope.sfSelectedItems) {
                                    //Clean the array and keep all references.
                                    scope.sfSelectedItems.length = 0;
                                }
                                else {
                                    scope.sfSelectedItems = [];
                                }

                                Array.prototype.push.apply(scope.sfSelectedItems, scope.selectedItemsInTheDialog.map(function (item) {
                                    return item.item;
                                }));

                                scope.sfSelectedIds = scope.sfSelectedItems.map(function (item) {
                                    return item.Id;
                                });
                            }
                            else {
                                scope.sfSelectedItem = null;
                                scope.sfSelectedItemId = null;
                                scope.sfSelectedItems = [];
                                scope.sfSelectedIds = [];
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

                            scope.itemsPromise = ctrl.getItems(scope.paging.skip, scope.paging.take)
                            .then(onFirstPageLoadedSuccess, onError);

                            scope.itemsPromise.finally(function () {
                                scope.showLoadingIndicator = false;
                            });

                            ensureSelectionIsUpToDate();
                        };

                        scope.getDialogTemplate = function () {
                            var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                            var url = attrs.sfDialogTemplate || ctrl.dialogTemplateUrl;
                            return serverContext.getEmbeddedResourceUrl(assembly, url);
                        };

                        scope.getClosedDialogTemplate = function () {
                            var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
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

                        scope.multiselect = (attrs.sfMultiselect && attrs.sfMultiselect.toLowerCase() == "true") ? true : false;

                        scope.selectedItemsInTheDialog = [];

                        scope.showLoadingIndicator = true;

                        scope.bindIdentifierField = function (item) {
                            return ctrl.bindIdentifierField(item);
                        };

                        scope.getChildren = function (parentId) {
                            return ctrl.getChildren(parentId, scope.filter.searchString);
                        };

                        scope.getPredecessors = function (itemId) {
                            return ctrl.getPredecessors(itemId);
                        };

                        scope.itemDisabled = function (item) {
                            if (ctrl.itemDisabled) {
                                return ctrl.itemDisabled(item);
                            }

                            return false;
                        };

                        scope.getSelectedIds = function () {
                            if (scope.multiselect) {
                                if (scope.sfSelectedIds && scope.sfSelectedIds.length > 0) {
                                    return scope.sfSelectedIds.filter(function (id) {
                                        return id;
                                    });
                                }
                                else if (scope.sfSelectedItems && scope.sfSelectedItems.length > 0) {
                                    return scope.sfSelectedItems.map(function (item) {
                                        return item.Id;
                                    });
                                }
                            }
                            else {
                                var id = (scope.sfSelectedItem && scope.sfSelectedItem.Id) || scope.sfSelectedItemId;
                                if (id && id !== emptyGuid) {
                                    return [id];
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

                        if (scope.sfSelectedIds && scope.sfSelectedIds.length !== 0) {
                            scope.sfSelectedIds = scope.sfSelectedIds.filter(function (value) {
                                return value && value !== "";
                            });
                        }

                        fetchSelectedItems();

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
