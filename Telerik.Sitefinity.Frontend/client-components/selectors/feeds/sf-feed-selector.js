(function ($) {
    angular.module('sfSelectors')
        .directive('sfFeedSelector', ['sfFeedsService', function (feedsService) {
            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        ctrl.getItems = function (skip, take, search) {
                            var provider = ctrl.$scope.sfProvider;
                            return feedsService.getItems(provider, skip, take, search);
                        };

                        ctrl.getSpecificItems = function (ids) {
                            var provider = ctrl.$scope.sfProvider;
                            return feedsService.getSpecificItems(ids, provider);
                        };

                        ctrl.selectorType = 'FeedSelector';

                        ctrl.dialogTemplateUrl = 'client-components/selectors/feeds/sf-feed-selector.sf-cshtml';
                        ctrl.$scope.dialogTemplateId = 'sf-feed-selector-template';

                        var closedDialogTemplate = attrs.sfMultiselect ?
                            'client-components/selectors/common/sf-list-group-selection.sf-cshtml' :
                            'client-components/selectors/common/sf-bubbles-selection.sf-cshtml';

                        ctrl.closedDialogTemplateUrl = closedDialogTemplate;
                    }
                }
            };
        }]);
})(jQuery);