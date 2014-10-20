(function ($) {
    angular.module('selectors')
        .directive('newsSelector', ['newsItemService', function (newsItemService) {
            return {
                require: '^listSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        ctrl.getItems = function (skip, take, search) {
                            var provider = ctrl.getProvider();
                            return newsItemService.getItems(provider, skip, take, search);
                        };

                        ctrl.getItem = function (id) {
                            var provider = ctrl.getProvider();
                            return newsItemService.getItem(id, provider);
                        };

                        ctrl.onSelectedItemLoadedSuccess = function (data) {
                            if (!ctrl.getSelectedItem()) {
                                data.Item.Title = data.Item.Title.Value;
                                ctrl.updateSelectedItem(data.Item);
                            }

                            if (!ctrl.getSelectedItemId()) {
                                ctrl.updateSelectedItemId(data.Item.Id);
                            }
                        };

                        ctrl.setSelectorType('NewsSelector');

                        ctrl.templateUrl = 'Selectors/news-selector.html';
                        ctrl.setPartialTemplate('news-selector-template');
                    }
                }
            };
        }]);
})(jQuery);