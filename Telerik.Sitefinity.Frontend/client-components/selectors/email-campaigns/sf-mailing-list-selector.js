(function ($) {
    angular.module('sfSelectors')
        .directive('sfMailingListSelector', ['sfMailingListService', function (mailingListService) {
            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        ctrl.getItems = function (skip, take, search) {
                            var provider = ctrl.$scope.sfProvider;
                            return mailingListService.getItems(provider, skip, take, search);
                        };

                        ctrl.getSpecificItems = function (ids) {
                            var provider = ctrl.$scope.sfProvider;
                            return mailingListService.getSpecificItems(ids, provider);
                        };

                        ctrl.selectorType = 'MailingListSelector';

                        ctrl.dialogTemplateUrl = 'client-components/selectors/email-campaigns/sf-mailing-list-selector.sf-cshtml';
                        ctrl.$scope.dialogTemplateId = 'sf-mailing-list-selector-template';

                        var closedDialogTemplate = attrs.sfMultiselect ?
                            'client-components/selectors/common/sf-list-group-selection.sf-cshtml' :
                            'client-components/selectors/common/sf-bubbles-selection.sf-cshtml';

                        ctrl.closedDialogTemplateUrl = closedDialogTemplate;
                    }
                }
            };
        }]);
})(jQuery);