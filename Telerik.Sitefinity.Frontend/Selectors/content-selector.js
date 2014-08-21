(function ($) {
    //if (typeof ($telerik) != 'undefined') {
    //    $telerik.$(document).one('dialogRendered', function () {
    //        angular.bootstrap($('.contentSelector'), ['selectors']);
    //    });
    //}

    angular.module('selectors')
        .directive('contentSelector', ['genericDataService', function (genericDataService) {
            return {
                restrict: "EA",
                transclude: true,
                scope: {
                    ItemType: "@itemtype",
                    ItemProvider: "@itemprovider",
                    SelectedItemId: "=selecteditemid"
                },
                template:
'<div id="selectedItemsPlaceholder">' +
    '<span ng-bind="selectedContentItem.Title"></span>' +
    '<button id="openSelectorBtn">Select</button>' +
'</div>' +
'<div class="contentSelector" modal template-url="selector-template" open-button="#openSelectorBtn" window-class="sf-designer-dlg" existing-scope="true">' +
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
                            "ng-class=\"{'list-group-item':true, 'active': item.Id==selectedContentItem.Id }\" " +
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
    '</script>'+
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
                                if (data.Items[i].Id == scope.SelectedItemId) {
                                    scope.selectedContentItem = data.Items[i];
                                }
                            }
                        }

                        scope.isListEmpty = scope.contentItems.length === 0 && !scope.filter.search;
                    };

                    var onError = function () {
                        var errorMessage = '';
                        if (data)
                            errorMessage = data.Detail;

                        scope.showError = true;
                        scope.errorMessage = errorMessage;
                    };

                    // ------------------------------------------------------------------------
                    // helper methods
                    // ------------------------------------------------------------------------

                    var loadContentItems = function () {
                        var skip = scope.filter.paging.get_itemsToSkip();
                        var take = scope.filter.paging.itemsPerPage;

                        return genericDataService.getItems(scope.ItemType, scope.ItemProvider, skip, take, scope.filter.search)
                            .then(onLoadedSuccess, onError);
                    };

                    var reloadContentItems = function (newValue, oldValue) {
                        if (newValue != oldValue) {
                            loadContentItems();
                        }
                    };

                    var hideLoadingIndicator = function () {
                        scope.ShowLoadingIndicator = false;
                    };

                    // ------------------------------------------------------------------------
                    // Scope variables and setup
                    // ------------------------------------------------------------------------

                    scope.showError = false;
                    scope.isListEmpty = false;
                    scope.contentItems = [];
                    scope.filter = {
                        //providerName: null,
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
                        scope.selectedContentItem = item;
                    };

                    scope.selectContent = function () {
                        if (scope.selectedContentItem) {
                            var selectedContentItemId = scope.selectedContentItem.Id;

                            scope.SelectedItemId = selectedContentItemId;
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
                    genericDataService.getItems(scope.ItemType, scope.ItemProvider, scope.filter.paging.get_itemsToSkip(), 
                        scope.filter.paging.itemsPerPage, scope.filter.search)
                        .then(onLoadedSuccess, onError)
                        .then(function () {
                            scope.$watch('filter.search', reloadContentItems);
                            //scope.$watch('filter.providerName', reloadContentItems);
                            scope.$watch('filter.paging.currentPage', reloadContentItems);
                        })
                        .catch(onError)
                        .finally(hideLoadingIndicator);

                    translude(function (clone) {
                        if (clone.html() && clone.html().trim()) {
                            element.find("#selectedItemsPlaceholder").empty().append(clone);
                        }
                    });
                }
            };
        }]);
})(jQuery);