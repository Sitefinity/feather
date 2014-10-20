(function () {
    angular.module('selectors')
        .directive('sfPageSelector', ['sfPageService', function (pageService) {
            return {
                require: '^listSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        ctrl.getChildItems = function (parentId, search) {
                            var siteId = ctrl.$scope.siteId;
                            var provider = ctrl.$scope.provider;
                            return pageService.getItems(parentId, siteId, provider, search);
                        };

                        ctrl.getSpecificItems = function (ids) {
                            //var provider = ctrl.$scope.provider;
                            //return pageService.getSpecificItems(ids, provider);
                        };

                        ctrl.onSelectedItemsLoadedSuccess = function (data) {
                            ctrl.updateSelection(data.Items);
                        };

                        ctrl.selectorType = 'PageSelector';
                        ctrl.templateUrl = 'Selectors/page-selector.html';
                        ctrl.$scope.partialTemplate = 'page-selector-template';
                    }
                }
            };
        }]);
})();