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
                    taxonomyId: '=?', /* flat-selector */
                    itemType: '@?' /* content-selector */
                },
                controller: function ($scope) {
                    this.getSelectedItemId = function () {
                        return $scope.selectedItemId;
                    };

                    this.getSelectedItem = function () {
                        return $scope.selectedItem;
                    };

                    this.getTaxonomyId = function () {
                        return $scope.taxonomyId;
                    };

                    this.getProvider = function () {
                        return $scope.provider;
                    };

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

                    this.setPartialTemplate = function (template) {
                        $scope.partialTemplate = template;
                    };

                    this.setSelectorType = function (type) {
                        $scope.selectorType = type;
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
                            if (error && error.data && error.data.ResponseStatus)
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
                            var skip = scope.filter.paging.totalItems;
                            var take = scope.filter.paging.itemsPerPage;
                            var filter = scope.filter.search;
                            return ctrl.getItems(skip, take, filter)
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
                            if (scope.filter.search && scope.filter.search !== '') {
                                return true;
                            }
                            else {
                                return false;
                            }
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
                                if (scope.selectorType === 'NewsSelector') {
                                    scope.selectedItem = null;
                                }
                            }
                        });

                        var timeoutPromise = false;
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
                        scope.openSelectorButtonId = '#' + selectorId + ' .openSelectorBtn';

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
                            var index;
                            for (var i = 0; i < scope.selectedItemsInTheDialog.length; i++) {
                                if (scope.selectedItemsInTheDialog[i].Id === item.Id) {
                                    alreadySelected = true;
                                    index = i;
                                    break;
                                }
                            }

                            if (alreadySelected) {
                                scope.selectedItemsInTheDialog.splice(index, 1);
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

                                scope.selectedItems = scope.selectedItemsInTheDialog;
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

                        scope.selectedItemsInTheDialog = [];

                        scope.showLoadingIndicator = true;

                        getSelectedItems();
                        scope.hideEndlessScrollLoadingIndicator = false;


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