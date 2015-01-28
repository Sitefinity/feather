(function () {
    var sfSelectors = angular.module('sfSelectors');
    sfSelectors.requires.push('sfInfiniteScroll');

    sfSelectors
        .directive('sfImageSelector', ['serverContext', 'sfImageService', function (serverContext, sfImageService) {
            var constants = {
                initialLoadedItemsCount: 50,
                infiniteScrollLoadedItemsCount: 20,
            };

            return {
                restrict: 'E',
                scope: {
                    selectedItem: '=ngModel'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/images/sf-image-selector.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    scope.filterObject = {};
                    scope.sortExpression = null;
                    scope.items = [];

                    var filterExpression = null;

                    // initial open populates dialog with all root libraries
                    sfImageService.getFolders({ take: constants.initialLoadedItemsCount }).then(function (rootFolders) {
                        scope.items = rootFolders;
                    });

                    scope.loadMore = function () {

                    };

                    scope.$watch('filterObject', function (newVal, oldVal) {
                        filterExpression = composeFilter(newVal);
                        refresh();
                    }, true);

                    scope.$watch('sortExpression', function (newVal, oldVal) {
                        refresh();
                    }, true);

                    var refresh = function () {

                    };

                    var composeFilter = function (filterObject) {

                    };
                }
            };
        }]);
})();