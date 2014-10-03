(function ($) {
    angular.module('selectors')
        .directive('dynamicItemsSelector', ['dataService', function (dataService) {
            return {
                require: "^listSelector",
                restrict: "A",
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        ctrl.getItems = function (skip, take, search) {
                            var provider = ctrl.getProvider();
                            return dataService.getItems(ctrl.getItemType(), provider, skip, take, search, ctrl.identifierField);
                        };

                        ctrl.getItem = function (id) {
                            var provider = ctrl.getProvider();
                            return dataService.getItem(id, ctrl.getItemType(), provider);
                        };

                        ctrl.onSelectedItemLoadedSuccess = function (data) {
                            if (!ctrl.getSelectedItem()) {
                                ctrl.updateSelectedItem(data.Item);
                            }

                            if (!ctrl.getSelectedItemId()) {
                                ctrl.updateSelectedItemId(data.Item.Id);
                            }
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

                        ctrl.setSelectorType('DynamicItemsSelector');

                        ctrl.templateUrl = 'Selectors/dynamic-items-selector.html';
                        ctrl.setPartialTemplate('dynamic-items-selector-template');
                    }
                }
            };
        }]);
})(jQuery);