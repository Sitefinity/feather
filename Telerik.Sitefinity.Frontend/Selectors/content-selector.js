(function ($) {
    angular.module('selectors')
        .directive('contentSelector', ['genericDataService', function (genericDataService) {
            return {
                require: "^listSelector",
                restrict: "A",
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        ctrl.getItems = function (skip, take, search) {
                            var itemType = ctrl.getItemType();
                            var provider = ctrl.getProvider();
                            return genericDataService.getItems(itemType, provider, skip, take, search);
                        };

                        ctrl.getItem = function (id) {
                            var itemType = ctrl.getItemType();
                            var provider = ctrl.getProvider();
                            return genericDataService.getItem(id, itemType, provider);
                        };

                        ctrl.onSelectedItemLoadedSuccess = function (data) {
                            if (!ctrl.getSelectedItem()) {
                                ctrl.updateSelectedItem(data.Items[0]);
                            }

                            if (!ctrl.getDelectedItemId()) {
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