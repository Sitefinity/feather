(function ($) {
    angular.module('selectors')
        .directive('contentSelector', ['genericDataService', function (genericDataService) {
            return {
                require: "^flatSelector",
                restrict: "A",
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        ctrl.getItems = function (skip, take, search) {
                            return genericDataService.getItems(ctrl.itemType, ctrl.provider, skip, take, search);
                        }

                        ctrl.getItem = function (id) {
                            return genericDataService.getItem(id, ctrl.itemType, ctrl.provider);
                        }

                        ctrl.onSelectedItemLoadedSuccess = function (data) {
                            if (!ctrl.selectedItem) {
                                ctrl.updateSelectedItem(data.Items[0]);
                            }

                            if (!ctrl.selectedItemId) {
                                ctrl.updateSelectedItemId(data.Items[0].Id);
                            }
                        };

                        ctrl.templateUrl = 'Selectors/content-selector.html';
                        ctrl.setPartialTemplate('content-selector-template');
                    }
                }
            };
        }]);
})(jQuery);