(function ($) {
    angular.module('sfSelectors')
        .directive('sfNewsSelector', ['newsItemService', function (newsItemService) {
            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        ctrl.getItems = function (skip, take, search, frontendLanguages) {
                            var provider = ctrl.$scope.provider;
                            return newsItemService.getItems(provider, skip, take, search, frontendLanguages);
                        };

                        ctrl.getSpecificItems = function (ids) {
                            var provider = ctrl.$scope.provider;
                            return newsItemService.getSpecificItems(ids, provider);
                        };

                        ctrl.onSelectedItemsLoadedSuccess = function (data) {
                            ctrl.updateSelection(data.Items);
                        };

                        ctrl.selectorType = 'NewsSelector';

                        ctrl.dialogTemplateUrl = 'Selectors/sf-news-selector.html';
                        ctrl.$scope.dialogTemplateId = 'sf-news-selector-template';

                        var closedDialogTemplate = attrs.multiselect ?
                            'Selectors/list-group-selection.html' :
                            'Selectors/bubbles-selection.html';

                        ctrl.closedDialogTemplateUrl = closedDialogTemplate;                        
                    }
                }
            };
        }]);
})(jQuery);