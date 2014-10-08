(function ($) {
    angular.module('selectors')
        .directive('listSelector', ['$timeout', function ($timeout) {
            return {
                restrict: "E",
                transclude: true,
                scope: {
                    selectedItemId: '=?',
                    selectedItem: '=?',
                    provider: '=?', /* content-selector */
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
                            if (mainField) {
                                return mainField;
                            }
                            else {
                                return item.Id;
                            }
                        }
                    };

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

                    this.getItemType = function () {
                        return $scope.itemType;
                    };

                    this.updateSelectedItem = function (selectedItem) {
                        $scope.selectedItem = selectedItem;
                    };

                    this.updateSelectedItemId = function (selectedItemId) {
                        $scope.selectedItemId = selectedItemId;
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

                            var i;
                            var id;

                            if (data && data.Items) {
                                //new filter
                                if (isFilterApplied() && scope.filter.paging.totalItems === 0) {
                                    scope.items = data.Items;

                                    for (i = 0; i < data.Items.length; i++) {
                                        id = data.Items[i].Id;
                                        if (isCurrentItemSelected(id)) {
                                            scope.selectedItemInTheDialog = data.Items[i];
                                        }
                                    }
                                }
                                    //load more items for already applied filter (endless scroll)
                                else if (isFilterApplied() && scope.filter.paging.totalItems !== 0) {
                                    for (i = 0; i < data.Items.length; i++) {
                                        id = data.Items[i].Id;

                                        if (isCurrentItemSelected(id)) {
                                            scope.selectedItemInTheDialog = data.Items[i];
                                        }
                                        scope.items.push(data.Items[i]);
                                    }
                                }
                                else if (!isFilterApplied()) {
                                    //add the selected item on the top
                                    if (scope.items.length === 0 && scope.selectedItem) {
                                        scope.items.push(scope.selectedItem);
                                        scope.selectedItemInTheDialog = scope.items[0];
                                    }

                                    var selectedItemId = scope.selectedItem && scope.selectedItem.Id || scope.selectedItemId;

                                    for (i = 0; i < data.Items.length; i++) {
                                        id = data.Items[i].Id;

                                        if (id !== selectedItemId) {
                                            scope.items.push(data.Items[i]);
                                        }
                                    }
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

                        var clearSelection = function () {
                            scope.selectedItem = null;
                            scope.selectedItemId = null;
                        };

                        // ------------------------------------------------------------------------
                        // helper methods
                        // ------------------------------------------------------------------------

                        var emptyGuid = '00000000-0000-0000-0000-000000000000';

                        var selectedId = function () {
                            return (scope.selectedItem && scope.selectedItem.Id) || scope.selectedItemId;
                        };

                        var loadItems = function () {
                            var skip = scope.filter.paging.totalItems;
                            var take = scope.filter.paging.itemsPerPage;
                            var filter = scope.filter.search;
                            return ctrl.getItems(skip, take, filter)
                                .then(onItemsLoadedSuccess, onError);
                        };

                        var getSelectedItem = function () {
                            var id = selectedId();

                            if (id && id !== emptyGuid) {
                                ctrl.getItem(id)
                                    .then(ctrl.onSelectedItemLoadedSuccess, clearSelection);
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

                        var isCurrentItemSelected = function (id) {
                            return id === selectedId();
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
                            if (scope.selectedItemInTheDialog && scope.selectedItemInTheDialog.Id === item.Id) {
                                scope.selectedItemInTheDialog = null;
                            }
                            else {
                                scope.selectedItemInTheDialog = item;
                            }
                        };

                        scope.selectItem = function () {
                            if (scope.selectedItemInTheDialog) {
                                //set the selected item and its id to the mapped isolated scope properties
                                scope.selectedItem = scope.selectedItemInTheDialog;
                                scope.selectedItemId = scope.selectedItemInTheDialog.Id;
                            }
                            else {
                                scope.selectedItem = null;
                                scope.selectedItemId = null;
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
                            var id = selectedId();
                            return id && id !== emptyGuid;
                        };

                        scope.showLoadingIndicator = true;

                        scope.hideEndlessScrollLoadingIndicator = false;

                        scope.bindIdentifierField = function (item) {
                            return ctrl.bindIdentifierField(item);
                        };

                        getSelectedItem();

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