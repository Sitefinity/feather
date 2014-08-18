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
                scope: {
                    ItemType: "@itemtype",
                    ItemProvider: "@itemprovider",
                    SelectedItemId: "=selecteditemid"
                },
                //templateUrl: '/Views/template.cshtml',
                //templateUrl: sf.pageEditor.widgetContext.AppPath + 'Views/template.html',
                template:
'<button id="openSelectorBtn">Select</button>' +
'<div class="contentSelector" modal template-url="selector-template" open-button="#openSelectorBtn" window-class="sf-designer-dlg" existing-scope="true">' +
    '<script type="text/ng-template" id="selector-template">' +
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
        '<button type="button" ng-hide="isListEmpty" class="btn btn-primary" ng-click="selectSharedContent()">DoneSelecting</button>' +
        '<button type="button" class="btn btn-link" ng-click="cancel()">Cancel</button>' +
    '</script>'+
'</div>',
                link: function (scope, element, attrs) {

                    // ------------------------------------------------------------------------
                    // Event handlers
                    // ------------------------------------------------------------------------

                    //var onGetPropertiesSuccess = function (data) {
                    //    if (data) {
                    //        scope.Properties = propertyService.toAssociativeArray(data.Items);
                    //        scope.filter.providerName = scope.Properties.ProviderName.PropertyValue;

                    //        providerService.setDefaultProviderName(scope.filter.providerName);
                    //    }
                    //};

                    //invoked when the content blocks for a provider are loaded
                    var onLoadedSuccess = function (data) {
                        if (data && data.Items) {
                            scope.contentItems = data.Items;
                            scope.filter.paging.set_totalItems(data.TotalCount);

                            //select current cotnentBlock if it exists
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

                    //var saveProperties = function (data) {
                    //    scope.SelectedItemId = data.Item.Id;
                    //    //scope.Properties.SharedContentID.PropertyValue = data.Item.Id;
                    //    //scope.Properties.ProviderName.PropertyValue = scope.SelectedContentItem.ProviderName;
                    //    //scope.Properties.Content.PropertyValue = data.Item.Content.Value;

                    //    //var modifiedProperties = [scope.Properties.SharedContentID, scope.Properties.ProviderName, scope.Properties.Content];
                    //    //var currentSaveMode = widgetContext.culture ? 1 : 0;
                    //    //return propertyService.save(currentSaveMode, modifiedProperties);
                    //};

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
                            itemsPerPage: 50,
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

                    scope.selectSharedContent = function () {
                        if (scope.selectedContentItem) {
                            var selectedContentItemId = scope.selectedContentItem.Id;
                            //var providerName = scope.SelectedContentItem.ProviderName;
                            //var checkout = false;

                            scope.SelectedItemId = selectedContentItemId;

                            //scope.ShowLoadingIndicator = true;
                            //sharedContentService.get(selectedContentItemId, providerName, checkout)
                            //    .then(saveProperties)
                            //    .then(dialogClose)
                            //    .catch(onError)
                            //    .finally(hideLoadingIndicator);
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

                        //if (typeof ($telerik) != 'undefined')
                            //$telerik.$(document).trigger('modalDialogClosed');
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
                }
            };
        }]);
})(jQuery);