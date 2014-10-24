(function ($) {
    angular.module('sfSelectors')
        .directive('sfDynamicItemsSelector', ['dataService', function (dataService) {
            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        ctrl.getItems = function (skip, take, search) {
                            var provider = ctrl.$scope.provider;
                            return dataService.getItems(ctrl.$scope.itemType, provider, skip, take, search, ctrl.identifierField);
                        };

                        ctrl.getSpecificItems = function (ids) {
                            var provider = ctrl.$scope.provider;
                            return dataService.getSpecificItems(ids, ctrl.$scope.itemType, provider);
                        };

                        ctrl.onSelectedItemsLoadedSuccess = function (data) {
                            ctrl.updateSelection(data.Items);
                        };

                        ctrl.selectorType = 'DynamicItemsSelector';

                        ctrl.templateUrl = 'Selectors/sf-dynamic-items-selector.html';
                        ctrl.$scope.partialTemplate = 'sf-dynamic-items-selector-template';
                    }
                }
            };
        }]);
})(jQuery);
