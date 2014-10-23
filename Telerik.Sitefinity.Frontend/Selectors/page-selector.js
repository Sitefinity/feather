(function () {
    angular.module('selectors')
        .directive('sfPageSelector', ['sfPageService', function (pageService) {
            return {
                require: '^listSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        var rootPage = 'f669d9a7-009d-4d83-ddaa-000000000002';

                        var getItems = function (parentId, search) {
                            var siteId = ctrl.$scope.siteId;
                            var provider = ctrl.$scope.provider;
                            return pageService.getItems(parentId, siteId, provider, search);
                        };

                        ctrl.getItems = function (skip, take, search) {
                            return getItems(rootPage, search);
                        };

                        ctrl.getChildren = function (parentId, search) {                            
                            return getItems(parentId, search)
                                .then(function (data) {
                                    return data.Items;
                                });
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