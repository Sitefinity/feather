(function ($) {
    angular.module('sfSelectors')
        .directive('sfDynamicItemsSelector', ['sfDataService', function (dataService) {
            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        var master = attrs.sfMaster === 'true' || attrs.sfMaster === 'True';

                        ctrl.getItems = function (skip, take, search) {
                            var provider = ctrl.$scope.sfProvider;
                            if (master)
                                return dataService.getItems(ctrl.$scope.sfItemType, provider, skip, take, search, ctrl.identifierField);
                            else
                                return dataService.getLiveItems(ctrl.$scope.sfItemType, provider, skip, take, search, ctrl.identifierField);
                        };

                        ctrl.getSpecificItems = function (ids) {
                            var provider = ctrl.$scope.sfProvider;
                            if (master)
                                return dataService.getSpecificItems(ids, ctrl.$scope.sfItemType, provider);
                            else
                                return dataService.getSpecificLiveItems(ids, ctrl.$scope.sfItemType, provider);
                        };

                        ctrl.selectorType = 'DynamicItemsSelector';

                        ctrl.dialogTemplateUrl = 'client-components/selectors/dynamic-modules/sf-dynamic-items-selector.sf-cshtml';
                        ctrl.$scope.dialogTemplateId = 'sf-dynamic-items-selector-template';

                        var closedDialogTemplate = attrs.sfMultiselect ?
                            'client-components/selectors/common/sf-list-group-selection.sf-cshtml' :
                            'client-components/selectors/common/sf-bubbles-selection.sf-cshtml';

                        ctrl.closedDialogTemplateUrl = closedDialogTemplate;
                    }
                }
            };
        }]);
})(jQuery);
