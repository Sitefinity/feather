(function ($) {
    angular.module('selectors')
        .directive('listSelector', ['$timeout', function ($timeout) {
            return {
                restrict: "E",
                transclude: true,
                scope: {
                    //For single selection
                    selectedItemId: '=?',
                    selectedItem: '=?',

                    //For multiple selection
                    selectedItems: '=?',
                    selectedIds: '=?',

                    provider: '=?',
                    taxonomyId: '=?', /* taxon-selector */
                    itemType: '=?', /* dynamic-items-selector */
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

                    this.updateSelection = function (selectedItem) {
                        updateSelectedItems(selectedItem);
                        updateSelectedIds(selectedItem.Id);
                    };

                    var updateSelectedItems = function (selectedItem) {
                        if (!$scope.multiselect && !$scope.selectedItem) {
                            $scope.selectedItem = selectedItem;
                        }

                        if (!$scope.selectedItems) {
                            $scope.selectedItems = [];
                        }

                        var selectedIds = $scope.selectedItems.map(function (item) {
                            return item.Id;
                        });

                        if (selectedIds.indexOf(selectedItem.Id) < 0) {
                            $scope.selectedItems.push(selectedItem);
                        }
                    };

                    var updateSelectedIds = function (selectedItemId) {
                        if (!$scope.multiselect && !$scope.selectedItemId) {
                            $scope.selectedItemId = selectedItemId;
                        }

                        if (!$scope.selectedIds) {
                            $scope.selectedIds = [];
                        }

                        if ($scope.selectedIds.indexOf(selectedItemId) < 0) {
                            $scope.selectedIds.push(selectedItemId);
                        }
                    };
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.templateUrl || 'Selectors/list-selector.html';
                    return sitefinity.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    post: function (scope, element, attrs, ctrl, transclude) {
                        // ------------------------------------------------------------------------
                        // Event handlers
                        // ------------------------------------------------------------------------

                        //invoked when the items are loaded
                        var onItemsLoadedSuccess = function (data) {
                            if (data.Items.length < scope.filter.paging.itemsPerPage) {
                                scope.hideEndlessScrollLoadingIndicator = true;
                            }

                            if (data && data.Items) {
                                //new filter
                                if (isFilterApplied() && scope.filter.paging.totalItems === 0) {
                                    selectItemsInDialog(data.Items);

                                    scope.items = data.Items;
                                }
                                //load more items for already applied filter (endless scroll)
                                else if (isFilterApplied() && scope.filter.paging.totalItems !== 0) {
                                    selectItemsInDialog(data.Items);

                                    Array.prototype.push.apply(scope.items, data.Items);
                                }
                                else if (!isFilterApplied()) {
                                    //add the selected items on the top
                                    if (scope.items.length === 0 && scope.selectedItems && scope.selectedItems.length > 0) {
                                        Array.prototype.push.apply(scope.items, scope.selectedItems);
                                        scope.selectedItemsInTheDialog = scope.selectedItems;
                                    }

                                    Array.prototype.push.apply(scope.items, data.Items.filter(function (item) {
                                        if (!scope.selectedIds) return true;

                                        return scope.selectedIds.indexOf(item.Id) < 0;
                                    }));
                                }

                                scope.filter.paging.totalItems += data.Items.length;
                            }

                            scope.isListEmpty = scope.items.length === 0 && !scope.filter.search;
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

                        var getSelectedIds = function () {
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

                        var emptyGuid = '00000000-0000-0000-0000-000000000000';

                        var loadItems = function () {
                            var skip = scope.filter.paging.totalItems;
                            var take = scope.filter.paging.itemsPerPage;
                            var filter = scope.filter.search;
                            return ctrl.getItems(skip, take, filter)
                                .then(onItemsLoadedSuccess, onError);
                        };

                        var getSelectedItems = function () {
                            var ids = getSelectedIds();
                            for (var i = 0; i < ids.length; i++) {
                                if (ids[i] !== emptyGuid) {
                                    ctrl.getItem(ids[i])
                                        .then(ctrl.onSelectedItemLoadedSuccess, onError)
                                        .finally(hideLoadingIndicator);//TODO: call it only when the last item is retrieved
                                }
                            }                            
                        };

                        var showLoadingIndicator = function () {
                            scope.showLoadingIndicator = true;
                        };

                        var hideLoadingIndicator = function () {
                            scope.showLoadingIndicator = false;
                        };

                        var resetItems = function () {
                            scope.filter.paging.totalItems = 0;
                            scope.hideEndlessScrollLoadingIndicator = false;
                            scope.items = [];
                        };

                        var isFilterApplied = function () {
                            return scope.filter.search && scope.filter.search !== '';
                        };

                        var selectItemsInDialog = function (items) {
                            var selectedIds = getSelectedIds();

                            var selectedItems = items.filter(function (item) {
                                return selectedIds.indexOf(item.Id) >= 0;
                            });

                            scope.selectedItemsInTheDialog = selectedItems;                        
                        };

                        // ------------------------------------------------------------------------
                        // Scope variables and setup
                        // ------------------------------------------------------------------------

                        scope.$watch('provider', function (newProvider, oldProvider) {
                            if (newProvider !== oldProvider) {
                                if (ctrl.selectorType === 'NewsSelector') {
                                    scope.selectedItem = null;
                                }
                            }
                        });

                        var timeoutPromise = false;

                        scope.showError = false;
                        scope.isListEmpty = false;
                        scope.items = [];
                        scope.filter = {
                            search: null,
                            paging: {
                                totalItems: 0,
                                itemsPerPage: 20
                            }
                        };

                        scope.itemClicked = function (index, item) {
                            var alreadySelected;
                            var selectedItemindex;
                            for (var i = 0; i < scope.selectedItemsInTheDialog.length; i++) {
                                if (scope.selectedItemsInTheDialog[i].Id === item.Id) {
                                    alreadySelected = true;
                                    selectedItemindex = i;
                                    break;
                                }
                            }

                            if (alreadySelected) {
                                scope.selectedItemsInTheDialog.splice(selectedItemindex, 1);
                            }
                            else {
                                if (scope.multiselect) {
                                    scope.selectedItemsInTheDialog.push(item);
                                }
                                else {
                                    scope.selectedItemsInTheDialog.splice(0, 1, item);
                                }
                            }
                        };

                        scope.doneSelecting = function () {
                            if (scope.selectedItemsInTheDialog.length > 0) {
                                //set the selected item and its id to the mapped isolated scope properties
                                scope.selectedItem = scope.selectedItemsInTheDialog[0];
                                scope.selectedItemId = scope.selectedItemsInTheDialog[0].Id;

                                if (scope.selectedItems) {
                                    //Clean the array and keep all references.
                                    scope.selectedItems.length = 0;
                                }
                                else {
                                    scope.selectedItems = [];
                                }

                                Array.prototype.push.apply(scope.selectedItems, scope.selectedItemsInTheDialog);

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
                            scope.filter.search = null;

                            scope.$modalInstance.close();
                        };

                        scope.HideError = function () {
                            scope.Feedback.showError = false;
                            scope.Feedback.errorMessage = null;
                        };

                        scope.cancel = function () {
                            try {
                                resetItems();
                                scope.filter.search = null;

                                scope.$modalInstance.close();
                            } catch (e) { }
                        };

                        scope.open = function () {
                            scope.$openModalDialog();

                            showLoadingIndicator();
                            ctrl.getItems(scope.filter.paging.totalItems, scope.filter.paging.itemsPerPage, scope.filter.search)
                            .then(onItemsLoadedSuccess, onError)
                            .then(function () {
                                jQuery(".endlessScroll").scroll(function () {
                                    var raw = jQuery(".endlessScroll")[0];
                                    if (raw.scrollTop !== 0 && raw.scrollTop + raw.offsetHeight >= raw.scrollHeight) {
                                        loadItems();
                                    }
                                });
                            })
                            .catch(onError)
                            .finally(hideLoadingIndicator);
                        };

                        scope.getTemplate = function () {
                            var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                            var url = ctrl.templateUrl;
                            return sitefinity.getEmbeddedResourceUrl(assembly, url);
                        };

                        scope.reloadItems = function (value) {
                            if (timeoutPromise) {
                                $timeout.cancel(timeoutPromise);
                            }

                            timeoutPromise = $timeout(function () {
                                resetItems();
                                loadItems();
                            }, 500);
                        };

                        scope.isItemSelected = function () {
                            var ids = getSelectedIds().filter(function (id) {
                                return id !== emptyGuid;
                            });

                            return ids.length > 0;
                        };

                        scope.isItemSelectedInDialog = function (item) {
                            for (var i = 0; i < scope.selectedItemsInTheDialog.length; i++) {
                                if (scope.selectedItemsInTheDialog[i].Id === item.Id) {
                                    return true;
                                }
                            }
                        };

                        scope.multiselect = !!attrs.multiselect;

                        scope.selectedItemsInTheDialog = [];

                        scope.showLoadingIndicator = true;

                        scope.hideEndlessScrollLoadingIndicator = false;

                        scope.bindIdentifierField = function (item) {
                            return ctrl.bindIdentifierField(item);
                        };

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