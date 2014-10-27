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
                        ctrl.templateUrl = 'Selectors/sf-news-selector.html';
                        ctrl.$scope.partialTemplate = 'sf-news-selector-template';
                    }
                }
            };
        }]);
})(jQuery);