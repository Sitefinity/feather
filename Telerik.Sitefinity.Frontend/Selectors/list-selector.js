(function ($) {
    angular.module('selectors')
        .directive('listSelector', function () {
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

                    this.updateSelection = function (selectedItems) {
                        if (selectedItems.length === 0) {
                            return;
                        }

                        $scope.selectedItem = selectedItems[0];
                        $scope.selectedItemId = selectedItems[0].Id;

                        $scope.selectedItems = selectedItems;
                        $scope.selectedIds = selectedItems.map(function (item) {
                            return item.Id;
                        });
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

                        var onFirstPageLoadedSuccess = function (data) {
                            scope.noItemsExist = !data.Items.length;
                            scope.paging.skip += data.Items.length;

                            pushSelectedItemsToTheTop();

                            pushNotSelectedItems(data.Items);

                            scope.collectSelectedItems();
                        };

                        var onItemsFilteredSuccess = function (data) {
                            scope.paging.skip += data.Items.length;

                            selectItemsInDialog(data.Items);
                            
                            scope.items = data.Items;
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

                        var pushSelectedItemsToTheTop = function () {
                            if (scope.items.length === 0 && scope.selectedItems && scope.selectedItems.length > 0) {
                                Array.prototype.push.apply(scope.items, scope.selectedItems);
                                Array.prototype.push.apply(scope.selectedItemsInTheDialog, scope.selectedItems);
                            }
                        };

                        var pushNotSelectedItems = function (items) {
                            var ids = getSelectedIds();

                            Array.prototype.push.apply(scope.items,
                                items.filter(function (item) {
                                    return ids.indexOf(item.Id) < 0;
                                }));
                        };

                        var pushMoreFilteredItems = function (items) {
                            selectItemsInDialog(items);

                            Array.prototype.push.apply(scope.items, items);
                        };

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

                        var getSelectedItems = function () {
                            var ids = getSelectedIds();
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

                        scope.showError = false;
                        scope.selectedItemsViewData = [];
                        scope.items = [];
                        scope.filter = {
                            placeholder: 'Narrow by typing',
                            timeoutMs: 500,
                            search: function (keyword) {
                                angular.element($("[endless-scroll]"))[0].scrollTop = 0;
                                scope.showLoadingIndicator = true;
                                scope.paging.skip = 0;
                                var skip = scope.paging.skip;
                                var take = scope.paging.take;
                                return ctrl.getItems(skip, take, keyword)
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
                                if (scope.filter.searchString) {
                                    pushMoreFilteredItems(items);
                                }
                                else {
                                    pushNotSelectedItems(items);
                                }                                
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

                        scope.getTemplate = function () {
                            var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                            var url = ctrl.templateUrl;
                            return sitefinity.getEmbeddedResourceUrl(assembly, url);
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

                        scope.bindIdentifierField = function (item) {
                            return ctrl.bindIdentifierField(item);
                        };

                        scope.collectSelectedItems = function () {
                            if (scope.multiselect) {
                                scope.selectedItemsViewData.length = 0;
                                Array.prototype.push.apply(scope.selectedItemsViewData, scope.selectedItemsInTheDialog);
                            }
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
        });
})(jQuery);