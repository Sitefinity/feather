(function ($) {
    angular.module('selectors')
        .directive('contentSelectorNew', ['genericDataService', function (genericDataService) {
            return {
                require: "^flatSelector",
                scope: {
                    itemType: '@',
                    itemProvider: '=',
                    selectedItemId: '=?',
                    selectedItem: '=?'
                },

                link: function (scope, element, attrs, contentSelectorCtrl) {
                    element.scope().getItems = function (skip, take, search) {
                        return genericDataService.getItems(scope.itemType, scope.itemProvider, skip, take, search);
                    }
                    
                    element.scope().getItem = function (id) {
                        return genericDataService.getItem(id, scope.itemType, scope.itemProvider);
                    }

                    element.scope().onSelectedItemLoadedSuccess = function (data) {
                        debugger;
                        if (!element.scope().selectedItem) {
                            element.scope().selectedItem = data.Items[0];
                        }

                        if (!element.scope().selectedItemId) {
                            element.scope().selectedItemId = data.Items[0].Id;
                        }
                    };

                    element.scope().selectedItemId = scope.selectedTaxonId;
                    element.scope().selectedItem = scope.selectedTaxon;
                    element.scope().templateUrl = 'Selectors/content-selector.html';

                }
            };
        }]);
})(jQuery);