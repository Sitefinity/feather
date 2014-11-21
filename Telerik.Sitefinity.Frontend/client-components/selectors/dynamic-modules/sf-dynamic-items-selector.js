(function ($) {
    angular.module('sfSelectors')
        .directive('sfDynamicItemsSelector', ['sfDataService', function (dataService) {
            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        var master = attrs.master === 'true' || attrs.master === 'True';

                        ctrl.getItems = function (skip, take, search) {
                            var provider = ctrl.$scope.provider;
                            if (master)
                                return dataService.getItems(ctrl.$scope.itemType, provider, skip, take, search, ctrl.identifierField);
                            else
                                return dataService.getLiveItems(ctrl.$scope.itemType, provider, skip, take, search, ctrl.identifierField);
                        };

                        ctrl.getSpecificItems = function (ids) {
                            var provider = ctrl.$scope.provider;
                            if (master)
                                return dataService.getSpecificItems(ids, ctrl.$scope.itemType, provider);
                            else
                                return dataService.getSpecificLiveItems(ids, ctrl.$scope.itemType, provider);
                        };

                        ctrl.selectorType = 'DynamicItemsSelector';

                        ctrl.dialogTemplateUrl = 'client-components/selectors/dynamic-modules/sf-dynamic-items-selector.html';
                        ctrl.$scope.dialogTemplateId = 'sf-dynamic-items-selector-template';

                        var closedDialogTemplate = attrs.multiselect ?
                            'client-components/selectors/common/sf-list-group-selection.html' :
                            'client-components/selectors/common/sf-bubbles-selection.html';

                        ctrl.closedDialogTemplateUrl = closedDialogTemplate;
                    }
                }
            };
        }]);
})(jQuery);
