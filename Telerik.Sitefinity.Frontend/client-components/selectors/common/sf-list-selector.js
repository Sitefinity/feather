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

                    sfMissingSelectedItems: "=?",

                    sfProvider: '=?',
                    sfChange: '=',
                    sfSortable: '=?',
                    sfItemType: '=?', /* sf-dynamic-items-selector */
                    sfIdentifierField: '@?',
                    sfDialogHeader: '@?',
                    sfKeepSelectedItemsBound: '@?',

                    sfExternalPages: '=?',
                    sfOpenExternalsInNewTab: '='
                },
                controller: function ($scope) {
                    this.defaultIdentifierField = 'Title';
                    this.identifierField = $scope.sfIdentifierField || this.defaultIdentifierField;

                    this.setIdentifierField = function (identifier) {
                        this.identifierField = identifier;
                        $scope.sfIdentifierField = identifier;
                    };

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

                    this.removeUnselectedItems = function () {
                        if ($scope.multiselect) {
                            var reoderedItems = [];
                            if ($scope.selectedItemsViewData && $scope.selectedItemsViewData.length > 0) {
                                for (var i = 0; i < $scope.selectedItemsViewData.length; i++) {
                                    for (var j = 0; j < $scope.selectedItemsInTheDialog.length; j++) {
                                        if ($scope.selectedItemsInTheDialog[j].Id === $scope.selectedItemsViewData[i].Id) {
                                            reoderedItems.push($scope.selectedItemsInTheDialog[j]);
                                            break;
                                        }
                                    }
                                }

                                $scope.selectedItemsInTheDialog = [];
                                Array.prototype.push.apply($scope.selectedItemsInTheDialog, reoderedItems);
                            }

                            $scope.selectedItemsViewData = [];
                        }
                    };

                    this.$scope = $scope;

                    this.canPushSelectedItemFirst = function () {
                        return true;
                    };

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

                    this.pushSelectedItemToTheTop = function (items) {
                        if ($scope.items.length === 0 && $scope.sfSelectedItems && $scope.sfSelectedItems.length > 0) {
                            $scope.items.push($scope.sfSelectedItems[0]);
                        }
                        else {
                            var ids = $scope.getSelectedIds();
                            if ($scope.items.length === 0 && ids && ids.length > 0) {
                                Array.prototype.push.apply($scope.items,
                                               items.filter(function (item) {
                                                   return ids.indexOf(item.Id) === 0;
                                               }));
                            }
                        }
                    };

                    this.pushNotSelectedItems = function (items) {
                        var ids = $scope.getSelectedIds();

                        Array.prototype.push.apply($scope.items,
                            items.filter(function (item) {
                                return ids.indexOf(item.Id) < 0;
                            }));
                    };

                    this.onFilterItemSucceeded = function (items) {
                    };

                    this.onItemSelected = function (item) {
                    };

                    this.onPostLinkComleted = function () {
                    };

                    this.onResetItems = function () {
                    };

                    this.onCancel = function () {
                    };

                    this.onDoneSelecting = function () {
                    };

                    this.onOpen = function () {
                    };

                    this.resetItems = function () {
                        $scope.paging.skip = 0;
                        $scope.paging.areAllItemsLoaded = false;
                        $scope.filter.isEmpty = true;
                        $scope.filter.searchString = null;
                        $scope.items = [];
                        $scope.selectedItemsInTheDialog = [];
                        $scope.selectedItemsViewData = [];

                        if (this.onResetItems) {
                            this.onResetItems();
                        }
                    };

                    this.OnItemsFiltering = function (items) {
                        return items;
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

                    this.onError = function (error) {
                        var errorMessage = '';
                        if (error && error.data && error.data.ResponseStatus) {
                            errorMessage = error.data.ResponseStatus.Message;
                        }
                        else if (error && error.statusText) {
                            if (error.statusText === 'canceled') {
                                return;
                            }
                            errorMessage = error.statusText;
                        }

                        $scope.showError = true;
                        $scope.errorMessage = errorMessage;
                    };

                    this.fetchSelectedItems = function () {
                        var ids = $scope.getSelectedIds();
                        currentSelectedIds = ids;

                        if (ids.length === 0)
                            return;

                        var that = this;
                        return this.getSpecificItems(ids)
                            .then(function (data) {
                                // Some of the items were not found.
                                $scope.sfMissingSelectedItems = data.Items.length < ids.length;

                                that.onSelectedItemsLoadedSuccess(data);
                            }, that.onError);
                    };

                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/common/sf-list-selector.sf-cshtml';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    pre: function (scope) {
                        if (!scope.sfExternalPages)
                            scope.sfExternalPages = [];


                        scope.kendoTabStrips = [];
                        scope.$on('kendoWidgetCreated', function (event, widget) {
                            if (widget.wrapper && widget.wrapper.is('.k-tabstrip')) {
                                scope.kendoTabStrips.push(widget);
                            }
                        });

                        scope.$on('kendoRendered', function (event) {
                            for (var i = 0; i < scope.kendoTabStrips.length; i++) {
                                scope.kendoTabStrips[i].activateTab('li.k-state-active');
                            }
                        });
                    },
                    post: function (scope, element, attrs, ctrl, transclude) {
                        // ------------------------------------------------------------------------
                        // Event handlers
                        // ------------------------------------------------------------------------
                        var onFirstPageLoadedSuccess = function (data) {
                            scope.noItemsExist = !data.Items.length;
                            scope.paging.skip += data.Items.length;

                            if (scope.multiselect || ctrl.selectorType === 'LibrarySelector') {
                                Array.prototype.push.apply(scope.items, data.Items);
                            }
                            else {
                                ctrl.pushSelectedItemToTheTop(data.Items);
                                ctrl.pushNotSelectedItems(data.Items);
                            }

                            return scope.items;
                        };

                        var onItemsFilteredSuccess = function (data) {
                            scope.paging.skip += data.Items.length;

                            if (!scope.multiselect && !scope.filter.searchString && ctrl.canPushSelectedItemFirst()) {
                                scope.items = [];
                                ctrl.pushSelectedItemToTheTop();
                                ctrl.pushNotSelectedItems(data.Items);
                            }
                            else {
                                scope.items = ctrl.OnItemsFiltering(data.Items);
                            }

                            if (ctrl.onFilterItemSucceeded) {
                                ctrl.onFilterItemSucceeded(scope.items);
                            }
                        };

                        // ------------------------------------------------------------------------
                        // helper methods
                        // ------------------------------------------------------------------------

                        var emptyGuid = '00000000-0000-0000-0000-000000000000';

                        var currentSelectedIds;

                        var updateSelectedItems = function () {
                            ctrl.removeUnselectedItems();

                            if (scope.sfChange) {
                                var oldSelectedItems = [];
                                Array.prototype.push.apply(oldSelectedItems, scope.sfSelectedItems);
                                var changeArgs = {
                                    "newSelectedItems": scope.selectedItemsInTheDialog,
                                    "oldSelectedItems": oldSelectedItems
                                };
                                scope.sfChange.call(scope.$parent, changeArgs);
                            }

                            if (scope.selectedItemsInTheDialog.length > 0) {
                                //set the selected item and its id to the mapped isolated scope properties
                                scope.sfSelectedItem = scope.selectedItemsInTheDialog[0];
                                scope.sfSelectedItemId = scope.selectedItemsInTheDialog[0].Id;

                                if (scope.sfSelectedItems) {
                                    //Clean the array and keep all references.
                                    scope.sfSelectedItems.length = 0;
                                }
                                else {
                                    scope.sfSelectedItems = [];
                                }

                                Array.prototype.push.apply(scope.sfSelectedItems, scope.selectedItemsInTheDialog);

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
                        };

                        ctrl.ensureSelectionIsUpToDate = function () {
                            $q.when(ctrl.fetchSelectedItems()).then(function () {
                                updateSelectionInTheDialog();

                                scope.collectSelectedItems();
                            });
                        };

                        var updateSelectionInTheDialog = function () {
                            if (scope.sfSelectedItems) {
                                scope.selectedItemsInTheDialog = [];
                                Array.prototype.push.apply(scope.selectedItemsInTheDialog, scope.sfSelectedItems);
                            }
                        };

                        var areArrayEquals = function (arr1, arr2) {
                            if (arr1 && arr2) {
                                var clonedArr1 = [].concat(arr1);
                                var clonedArr2 = [].concat(arr2);

                                return clonedArr1.sort().toString() === clonedArr2.sort().toString();
                            }
                            return false;
                        };

                        ctrl.beginLoadingItems = function () {
                            scope.showLoadingIndicator = true;

                            scope.itemsPromise = ctrl.getItems(scope.paging.skip, scope.paging.take)
                                                     .then(onFirstPageLoadedSuccess, ctrl.onError);

                            scope.itemsPromise.finally(function () {
                                scope.showLoadingIndicator = false;
                            });

                            ctrl.ensureSelectionIsUpToDate();
                        };

                        // ------------------------------------------------------------------------
                        // Scope variables and setup
                        // ------------------------------------------------------------------------

                        scope.$watch('sfProvider', function (newProvider, oldProvider) {
                            if (newProvider !== oldProvider) {
                                if (ctrl.selectorType === 'NewsSelector' ||
                                    ctrl.selectorType === 'DynamicItemsSelector' ||
                                    ctrl.selectorType === 'LibrarySelector' ||
                                    ctrl.selectorType === 'ListsSelector') {
                                    scope.sfSelectedItems = null;
                                    scope.sfSelectedIds = null;
                                }
                            }
                        });

                        scope.$watchCollection('sfSelectedIds', function (newIds, oldIds) {
                            if (newIds && newIds.length > 0 && !areArrayEquals(newIds, currentSelectedIds)) {
                                ctrl.fetchSelectedItems();
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
                                var endlessScroll = angular.element($("[sf-endless-scroll]"))[0];
                                if (endlessScroll) {
                                    endlessScroll.scrollTop = 0;
                                }
                                scope.showLoadingIndicator = true;
                                scope.paging.skip = 0;
                                var skip = scope.paging.skip;
                                var take = scope.paging.take;
                                var languages = serverContext.getFrontendLanguages();
                                return ctrl.getItems(skip, take, keyword, languages)
                                    .then(onItemsFilteredSuccess, ctrl.onError)
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
                                    ctrl.pushNotSelectedItems(items);
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
                                if (scope.selectedItemsInTheDialog[i].Id === item.Id) {
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
                                    scope.selectedItemsInTheDialog.push(item);
                                }
                                else {
                                    scope.selectedItemsInTheDialog.splice(0, 1, item);
                                }

                                if (scope.sfKeepSelectedItemsBound) {
                                    updateSelectedItems();
                                }
                            }
                        };

                        scope.doneSelecting = function () {
                            updateSelectedItems();

                            ctrl.resetItems();

                            if (ctrl.onDoneSelecting) {
                                ctrl.onDoneSelecting();
                            }

                            scope.$modalInstance.close();
                        };

                        scope.cancel = function () {
                            ctrl.resetItems();

                            if (ctrl.onCancel) {
                                ctrl.onCancel();
                            }

                            scope.$modalInstance.close();
                        };

                        scope.open = function () {
                            if (ctrl.onOpen) {
                                ctrl.onOpen();
                            }

                            if (scope.$openModalDialog) {
                                scope.$openModalDialog();
                            }

                            ctrl.beginLoadingItems();
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
                            if (!item) return false;

                            for (var i = 0; i < scope.selectedItemsInTheDialog.length; i++) {
                                if (scope.selectedItemsInTheDialog[i].Id === item.Id) {
                                    return true;
                                }
                            }
                        };

                        scope.getSelectedItemsCount = function () {
                            return scope.selectedItemsInTheDialog.length;
                        };

                        attrs.$observe('sfMultiselect', function () {
                            scope.multiselect = (attrs.sfMultiselect && attrs.sfMultiselect.toLowerCase() == 'true') ? true : false;

                            if (!scope.multiselect && scope.sfSelectedItems && scope.sfSelectedItems.length > 1) {
                                ctrl.updateSelection([scope.sfSelectedItems[0]]);
                            }
                        });

                        scope.multiselect = (attrs.sfMultiselect && attrs.sfMultiselect.toLowerCase() == 'true') ? true : false;
                        if (!scope.sfSelectedItemId && scope.sfSelectedIds && scope.sfSelectedIds.length)
                            scope.sfSelectedItemId = scope.sfSelectedIds[0];

                        scope.selectButtonText = attrs.sfSelectButtonText;
                        scope.changeButtonText = attrs.sfChangeButtonText;

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
                                    }).filter(function (id) {
                                        return id;
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

                        scope.removeUnselectedItems = ctrl.removeUnselectedItems;

                        if (scope.sfSelectedIds && scope.sfSelectedIds.length !== 0) {
                            scope.sfSelectedIds = scope.sfSelectedIds.filter(function (value) {
                                return value && value !== "";
                            });
                        }

                        ctrl.fetchSelectedItems();

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

                        ////NOTE: Emit indication when initial setup is completed so child directives can be notified.
                        if (ctrl.onPostLinkComleted) {
                            ctrl.onPostLinkComleted();
                        }
                    }
                }
            };
        }]);
})(jQuery);

