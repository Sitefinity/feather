(function ($) {
    angular.module('selectors')
        .directive('newsSelector', ['newsItemService', function (newsItemService) {
            return {
                require: "^listSelector",
                restrict: "A",
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        ctrl.getItems = function (skip, take, search) {
                            var provider = ctrl.$scope.provider;
                            return newsItemService.getItems(provider, skip, take, search);
                        };

                        ctrl.getSpecificItems = function (ids) {
                            var provider = ctrl.$scope.provider;
                            return newsItemService.getSpecificItems(ids, provider);
                        };

                        ctrl.onSelectedItemsLoadedSuccess = function (data) {
                            ctrl.updateSelection(data.Items);
                        };

                        ctrl.selectorType = 'NewsSelector';
                        ctrl.templateUrl = 'Selectors/news-selector.html';
                        ctrl.$scope.partialTemplate = 'news-selector-template';
                    }
                }
            };
        }]);
})(jQuery);