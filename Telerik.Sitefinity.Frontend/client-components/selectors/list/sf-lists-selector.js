(function () {
    angular.module('sfSelectors')
        .directive('sfListsSelector', ['sfListsService', function (sfListsService) {
            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        ctrl.getItems = function (skip, take, search, frontendLanguages) {
                            var provider = ctrl.$scope.sfProvider;
                            return sfListsService.getItems(provider, skip, take, search, frontendLanguages);
                        };

                        ctrl.getSpecificItems = function (ids) {
                            var provider = ctrl.$scope.sfProvider;
                            return sfListsService.getSpecificItems(ids, provider);
                        };

                        ctrl.selectorType = 'ListsSelector';

                        ctrl.dialogTemplateUrl = 'client-components/selectors/list/sf-lists-selector.html';

                        var closedDialogTemplate = attrs.sfMultiselect ?
                            'client-components/selectors/common/sf-list-group-selection.html' :
                            'client-components/selectors/common/sf-bubbles-selection.html';

                        ctrl.closedDialogTemplateUrl = closedDialogTemplate;

                        ctrl.$scope.sfDialogHeader = 'Select lists';
                    }
                }
            };
        }]);
})();