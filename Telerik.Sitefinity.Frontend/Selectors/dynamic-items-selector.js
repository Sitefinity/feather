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
                            ctrl.updateSelectedItems(data.Item);                            

                            ctrl.updateSelectedIds(data.Item.Id);
                        };

                        ctrl.bindIdentifierField = function (item) {
                            if (item) {
                                var mainField = item[ctrl.identifierField];
                                if (mainField) {
                                    return mainField.Value;
                                }
                                else {
                                    return item.Id;
                                }
                            }
                        };

                        ctrl.selectorType = 'DynamicItemsSelector';

                        ctrl.templateUrl = 'Selectors/dynamic-items-selector.html';
                        ctrl.$scope.partialTemplate = 'dynamic-items-selector-template';
                    }
                }
            };
        }]);
})(jQuery);