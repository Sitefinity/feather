(function ($) {
    angular.module('selectors')
        .directive('flatSelector', function () {
            return {
                restrict: "E",
                transclude: true,
                scope: {
                    selectedItemId: '=?',
                    selectedItem: '=?',
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

                    this.updateSelectedItem = function (selectedItem) {
                        $scope.selectedItem = selectedItem;
                    };

                    this.updateSelectedItemId = function (selectedItemId) {
                        $scope.selectedItemId = selectedItemId;
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

                        //invoked when the items are loaded
                        var onItemsLoadedSuccess = function (data) {
                            if (data && data.Items) {
                                scope.items = data.Items;
                                scope.filter.paging.set_totalItems(data.TotalCount);

                                //select current item if it exists
                                for (var i = 0; i < data.Items.length; i++) {
                                    var id = data.Items[i].Id;
                                    if (id === scope.selectedItemId ||
                                        (scope.selectedItem && id === scope.selectedItem.Id)) {
                                        scope.selectedItemInTheDialog = data.Items[i];
                                    }
                                }
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

                        var loadItems = function () {
                            var skip = scope.filter.paging.get_itemsToSkip();
                            var take = scope.filter.paging.itemsPerPage;
                        
                            return ctrl.getItems(skip, take, scope.filter.search)
                                .then(onItemsLoadedSuccess, onError);
                        };

                        var getSelectedItem = function (scope) {
                            var id = (scope.selectedItem && scope.selectedItem.Id) || scope.selectedItemId;

                            if (id && id !== '00000000-0000-0000-0000-000000000000') {
                                ctrl.getItem(id)
                                    .then(ctrl.onSelectedItemLoadedSuccess)
                                    .finally(hideLoadingIndicator);
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
                            scope.selectedItemInTheDialog = item;
                        };

                        scope.selectItem = function () {
                            if (scope.selectedItemInTheDialog) {
                                //set the selected item and its id to the mapped isolated scope properties
                                scope.selectedItem = scope.selectedItemInTheDialog;
                                scope.selectedItemId = scope.selectedItemInTheDialog.Id;
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
                            if (scope.selectedItemId && scope.selectedItemId != '00000000-0000-0000-0000-000000000000') {
                                return true;
                            }
                            else {
                                return false;
                            }
                        };

                        scope.showLoadingIndicator = true;

                        getSelectedItem(scope);

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

                        //var templateUrl = scope.getTemplate();
                        //if (templateUrl) {
                        //    $http.get(templateUrl, { cache: $templateCache })
                        //        .then(function (response) {
                        //            element.html(response.data);
                        //            element.children().data('$ngControllerController', ctrl);
                        //            $compile(element.contents())(scope);
                        //        });
                        //}
                    }
                }
            };
        });
})(jQuery);