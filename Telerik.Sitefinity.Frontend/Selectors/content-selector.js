(function ($) {
    angular.module('selectors')
        .directive('contentSelector', ['genericDataService', function (genericDataService) {
            return {
                restrict: "EA",
                transclude: true,
                scope: {
                    itemType: '@',
                    itemProvider: '=',
                    selectedItemId: '=?',
                    selectedItem: '=?'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.templateUrl || 'Selectors/content-selector.html';
                    return sitefinity.getEmbeddedResourceUrl(assembly, url);
                },

                link: function (scope, element, attrs, ctrl, translude) {

                    // ------------------------------------------------------------------------
                    // Event handlers
                    // ------------------------------------------------------------------------

                    //invoked when the content items are loaded
                    var onItemsLoadedSuccess = function (data) {
                        if (data && data.Items) {
                            scope.contentItems = data.Items;
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

                        scope.isListEmpty = scope.contentItems.length === 0 && !scope.filter.search;
                    };

                    var onSelectedItemLoadedSuccess = function (data) {
                        if (!scope.selectedItem) {
                            scope.selectedItem = data.Items[0];
                        }

                        if (!scope.selectedItemId) {
                            scope.selectedItemId = data.Items[0].Id;
                        }
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

                    var loadContentItems = function () {
                        var skip = scope.filter.paging.get_itemsToSkip();
                        var take = scope.filter.paging.itemsPerPage;

                        return genericDataService.getItems(scope.itemType, scope.itemProvider, skip, take, scope.filter.search)
                            .then(onItemsLoadedSuccess, onError);
                    };

                    var getSelectedItem = function (scope) {
                        var id = (scope.selectedItem && scope.selectedItem.Id) || scope.selectedItemId;

                        if (id && id !== '00000000-0000-0000-0000-000000000000') {
                            genericDataService.getItem(id, scope.itemType, scope.itemProvider)
                                .then(onSelectedItemLoadedSuccess)
                                .finally(hideLoadingIndicator);
                        }
                    };

                    var reloadContentItems = function (newValue, oldValue) {
                        if (newValue !== oldValue) {
                            loadContentItems();
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
                    scope.contentItems = [];
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

                    scope.contentItemClicked = function (index, item) {
                        scope.selectedItemInTheDialog = item;
                    };

                    scope.selectContent = function () {
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
                        genericDataService.getItems(scope.itemType, scope.itemProvider, scope.filter.paging.get_itemsToSkip(),
                        scope.filter.paging.itemsPerPage, scope.filter.search)
                        .then(onItemsLoadedSuccess, onError)
                        .then(function () {
                            scope.$watch('filter.search', reloadContentItems);
                            scope.$watch('filter.paging.currentPage', reloadContentItems);
                        })
                        .catch(onError)
                        .finally(hideLoadingIndicator);
                    };

                    scope.showLoadingIndicator = true;

                    getSelectedItem(scope);

                    translude(scope, function (clone) {
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
            };
        }]);
})(jQuery);