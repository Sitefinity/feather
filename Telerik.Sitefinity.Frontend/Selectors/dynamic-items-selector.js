(function ($) {
    angular.module('selectors')
        .directive('dynamicItemsSelector', ['dataService', function (dataService) {
            return {
                require: "^listSelector",
                restrict: "A",
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        ctrl.getItems = function (skip, take, search) {
                            var provider = ctrl.$scope.provider;
                            return dataService.getItems(ctrl.$scope.itemType, provider, skip, take, search, ctrl.identifierField);
                        };

                        ctrl.getItem = function (id) {
                            var provider = ctrl.$scope.provider;
                            return dataService.getItem(id, ctrl.$scope.itemType, provider);
                        };

                        ctrl.onSelectedItemLoadedSuccess = function (data) {
                            ctrl.updateSelection(data.Item);
                        };

                        ctrl.selectorType = 'DynamicItemsSelector';

                        ctrl.templateUrl = 'Selectors/dynamic-items-selector.html';
                        ctrl.$scope.partialTemplate = 'dynamic-items-selector-template';
                    }
                }
            };
        }]);
})(jQuery);