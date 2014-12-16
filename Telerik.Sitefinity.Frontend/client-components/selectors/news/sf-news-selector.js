(function ($) {
    angular.module('sfSelectors')
        .directive('sfNewsSelector', ['sfNewsItemService', function (newsItemService) {
            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        ctrl.getItems = function (skip, take, search, frontendLanguages) {
                            var provider = ctrl.$scope.sfProvider;
                            return newsItemService.getItems(provider, skip, take, search, frontendLanguages);
                        };

                        ctrl.getSpecificItems = function (ids) {
                            var provider = ctrl.$scope.sfProvider;
                            return newsItemService.getSpecificItems(ids, provider);
                        };

                        ctrl.selectorType = 'NewsSelector';

                        ctrl.dialogTemplateUrl = 'client-components/selectors/news/sf-news-selector.html';
                        ctrl.$scope.dialogTemplateId = 'sf-news-selector-template';

                        var closedDialogTemplate = attrs.sfMultiselect ?
                            'client-components/selectors/common/sf-list-group-selection.html' :
                            'client-components/selectors/common/sf-bubbles-selection.html';

                        ctrl.closedDialogTemplateUrl = closedDialogTemplate;                        
                    }
                }
            };
        }]);
})(jQuery);