(function ($) {
    angular.module('selectors')
        .directive('flatSelector', function () {
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
                    taxonomyId: '=?', /* flat-selector */
                    itemType: '@?' /* content-selector */
                },
                controller: function ($scope) {
                    this.selectedItemId = $scope.selectedItemId;
                    this.selectedItem = $scope.selectedItem;
                    this.taxonomyId = $scope.taxonomyId;
                    this.provider = $scope.provider;
                    this.itemType = $scope.itemType;

                    this.updateSelectedItems = function (selectedItem) {
                        if (!$scope.multiselect && !$scope.selectedItem) {
                            $scope.selectedItem = selectedItem;
                        }
                        
                        if (!$scope.selectedItems) {
                            $scope.selectedItems = [];
                        }
                        if ($scope.selectedItems.length === 0) {
                            $scope.selectedItems.push(selectedItem);
                        }
                    };

                    this.updateSelectedIds = function (selectedItemId) {
                        if (!$scope.multiselect && !$scope.selectedItemId) {
                            $scope.selectedItemId = selectedItemId;
                        }

                        if (!$scope.selectedIds) {
                            $scope.selectedIds = [];
                        }
                        if ($scope.selectedIds.length === 0) {
                            $scope.selectedIds.push(selectedItemId);
                        }
                    };
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.templateUrl || 'Selectors/flat-selector.html';
                    return sitefinity.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    post: function (scope, element, attrs, ctrl, transclude) {
                        // ------------------------------------------------------------------------
                        // Event handlers
                        // ------------------------------------------------------------------------
                        debugger;
                        //invoked when the items are loaded
                        var onItemsLoadedSuccess = function (data) {
                            if (data && data.Items) {
                                scope.items = data.Items;
                                scope.filter.paging.set_totalItems(data.TotalCount);

                                var selectedIds = getSelectedIds();

                                var selectedItems = data.Items.filter(function (item) {
                                    return selectedIds.indexOf(item.Id) >= 0;
                                });

                                scope.selectedItemsInTheDialog = selectedItems;
                            }

                            scope.isListEmpty = scope.items.length === 0 && !scope.filter.search;
                        };

                        var onError = function (error) {
                            var errorMessage = '';
                            if (error && error.data.ResponseStatus)
                                errorMessage = error.data.ResponseStatus.Message;

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
                                        return item.id;
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

                        var loadItems = function () {
                            var skip = scope.filter.paging.get_itemsToSkip();
                            var take = scope.filter.paging.itemsPerPage;
                        
                            return ctrl.getItems(skip, take, scope.filter.search)
                                .then(onItemsLoadedSuccess, onError);
                        };

                        var getSelectedItems = function () {
                            var ids = getSelectedIds();
                            for (var i = 0; i < ids.length; i++) {
                                if (ids[i] !== '00000000-0000-0000-0000-000000000000') {
                                    ctrl.getItem(ids[i])
                                        .then(ctrl.onSelectedItemLoadedSuccess)
                                        .finally(hideLoadingIndicator);//TODO: call it only when the last item is retrieved
                                }
                            }                            
                        };

                        var reloadItems = function (newValue, oldValue) {
                            if (newValue !== oldValue) {
                                loadItems();
                            }
                        };

                        var hideLoadingIndicator = function () {
                            scope.ShowLoadingIndicator = false;
                        };

                        // ------------------------------------------------------------------------
                        // Scope variables and setup
                        // ------------------------------------------------------------------------

                        var selectorId;
                        if (attrs.id) {
                            selectorId = attrs.id;
                        }
                        else {
                            //selectorId will be set to the id of the wrapper div of the template. This way we avoid issues when there are several selectors on one page.
                            selectorId = 'sf' + Math.floor((Math.random() * 1000) + 1);
                            scope.selectorId = selectorId;
                        }

                        // This id is used by the modal dialog to know which button will open him.
                        scope.openSelectorButtonId = '#' + selectorId + ' #openSelectorBtn';

                        scope.showError = false;
                        scope.isListEmpty = false;
                        scope.items = [];
                        scope.filter = {
                            search: null,
                            paging: {
                                totalItems: 0,
                                currentPage: 1,
                                itemsPerPage: 20,
                                get_itemsToSkip: function () {
                                    return (this.currentPage - 1) * this.itemsPerPage;
                                },
                                set_totalItems: function (itemsCount) {
                                    this.totalItems = itemsCount;
                                    this.isVisible = this.totalItems > this.itemsPerPage;
                                },
                                isVisible: false
                            }
                        };

                        scope.itemClicked = function (index, item) {
                            if (attrs.multiselect) {
                                scope.selectedItemsInTheDialog.push(item);
                            }
                            else {
                                //Switch the items in single selection mode
                                scope.selectedItemsInTheDialog.splice(0, 1, item);
                            }
                        };

                        scope.doneSelecting = function () {
                            if (scope.selectedItemsInTheDialog.length > 0) {
                                //set the selected item and its id to the mapped isolated scope properties
                                scope.selectedItem = scope.selectedItemsInTheDialog[0];
                                scope.selectedItemId = scope.selectedItemsInTheDialog[0].Id;

                                scope.selectedItems = scope.selectedItemsInTheDialog;
                                scope.selectedIds = scope.selectedItems.map(function (item) {
                                    return item.Id;
                                });
                            }

                            scope.$modalInstance.close();
                        };

                        scope.HideError = function () {
                            scope.Feedback.showError = false;
                            scope.Feedback.errorMessage = null;
                        };

                        scope.cancel = function () {
                            try {
                                scope.$modalInstance.close();
                            } catch (e) { }
                        };

                        scope.open = function () {
                            ctrl.getItems(scope.filter.paging.get_itemsToSkip(), scope.filter.paging.itemsPerPage, scope.filter.search)
                            .then(onItemsLoadedSuccess, onError)
                            .then(function () {
                                scope.$watch('filter.search', reloadItems);
                                scope.$watch('filter.paging.currentPage', reloadItems);
                            })
                            .catch(onError)
                            .finally(hideLoadingIndicator);
                        };

                        scope.getTemplate = function () {
                            var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                            var url = ctrl.templateUrl;
                            return sitefinity.getEmbeddedResourceUrl(assembly, url);
                        };

                        scope.isItemSelected = function () {
                            var ids = getSelectedIds().filter(function (id) {
                                return id !== '00000000-0000-0000-0000-000000000000';
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

                        scope.showLoadingIndicator = true;

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
        });
})(jQuery);