(function ($) {
    angular.module('selectors')
        .directive('contentSelector', ['genericDataService', function (genericDataService) {
            return {
                restrict: "EA",
                transclude: true,
                scope: {
                    itemType: '@',
                    itemProvider: '=',
                    selectedItemId: '=',
                    selectedItem: '&'
                },
                template:
'<div id="{{selectorId}}">' +
    '<div id="selectedItemsPlaceholder">' +
        '<alert type="danger" ng-show="showError">{{errorMessage}}</alert>' +
        '<div ng-hide="showError">' +
            '<span ng-bind="selectedItem.Title"></span>' +
            '<button id="openSelectorBtn">Select</button>' +
        '</div>' +
    '</div>' +
    '<div class="contentSelector" modal template-url="selector-template" open-button="#{{selectorId}} #openSelectorBtn" window-class="sf-designer-dlg" existing-scope="true">' +
        '<script type="text/ng-template" id="selector-template">' +
            '<div class="modal-header">' +
                '<h1 class="modal-title">Select content</h1>' +
            '</div>' +
            '<div class="modal-body">' +
                '<div ng-show="isListEmpty" class="alert alert-info">NoItemsHaveBeedCreatedYet</div>' +
                '<div ng-hide="isListEmpty">' +
                    '<div class="input-group m-bottom-sm">' +
                        '<span class="input-group-addon">' +
                            '<i class="glyphicon glyphicon-search"></i>' +
                        '</span>' +
                        '<input type="text" ng-model="filter.search" class="form-control" placeholder="NarrowByTyping" />' +
                    '</div>' +
                    '<div class="list-group s-items-list-wrp">' +
                        '<a ng-repeat="item in contentItems"' +
                                "ng-class=\"{'list-group-item':true, 'active': item.Id==selectedItemInTheDialog.Id }\" " +
                                'ng-click="contentItemClicked($index, item)"> ' +
                            '<span ng-bind="item.Title"></span>' +
                        '</a>' +
                    '</div>' +
                    '<pagination ng-show="filter.paging.isVisible" items-per-page="filter.paging.itemsPerPage" total-items="filter.paging.totalItems" ng-model="filter.paging.currentPage"></pagination>' +
                    '<div ng-hide="contentItems.length">NoItemsFound</div>' +
                '</div>' +
            '</div>' +
            '<div class="modal-footer">' +
                '<button type="button" ng-hide="isListEmpty" class="btn btn-primary" ng-click="selectContent()">DoneSelecting</button>' +
                '<button type="button" class="btn btn-link" ng-click="cancel()">Cancel</button>' +
            '</div>' +
        '</script>' +
    '</div>' +
'</div>',
                link: function (scope, element, attrs, ctrl, translude) {

                    // ------------------------------------------------------------------------
                    // Event handlers
                    // ------------------------------------------------------------------------

                    //invoked when the content items are loaded
                    var onLoadedSuccess = function (data) {
                        if (data && data.Items) {
                            scope.contentItems = data.Items;
                            scope.filter.paging.set_totalItems(data.TotalCount);

                            //select current item if it exists
                            for (var i = 0; i < data.Items.length; i++) {
                                var id = data.Items[i].Id;
                                if (id === scope.selectedItemId ||
                                    (scope.selectedItem && id === scope.selectedItem.Id)) {
                                    scope.selectedItemInTheDialog = data.Items[i];
                                    scope.selectedItem = data.Items[i];
                                }
                            }
                        }

                        scope.isListEmpty = scope.contentItems.length === 0 && !scope.filter.search;
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
                            .then(onLoadedSuccess, onError);
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

                    //Will be set to the id of the wrapper div of the template. This way we avoid issues when there are several selectors on one page.
                    scope.selectorId = "sf" + Date.now();

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

                    scope.showLoadingIndicator = true;

                    genericDataService.getItems(scope.itemType, scope.itemProvider, scope.filter.paging.get_itemsToSkip(),
                        scope.filter.paging.itemsPerPage, scope.filter.search)
                        .then(onLoadedSuccess, onError)
                        .then(function () {
                            scope.$watch('filter.search', reloadContentItems);
                            scope.$watch('filter.paging.currentPage', reloadContentItems);
                        })
                        .catch(onError)
                        .finally(hideLoadingIndicator);

                    translude(function (clone) {
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