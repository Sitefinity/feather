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

                        ctrl.getItem = function (id) {
                            var provider = ctrl.$scope.provider;
                            return newsItemService.getItem(id, provider);
                        };

                        ctrl.onSelectedItemLoadedSuccess = function (data) {
                            data.Item.Title = data.Item.Title.Value;
                            ctrl.updateSelectedItems(data.Item);

                            ctrl.updateSelectedIds(data.Item.Id);
                        };

                        ctrl.selectorType = 'NewsSelector';
                        ctrl.templateUrl = 'Selectors/news-selector.html';
                        ctrl.$scope.partialTemplate = 'news-selector-template';
                    }
                }
            };
        }]);
})(jQuery);