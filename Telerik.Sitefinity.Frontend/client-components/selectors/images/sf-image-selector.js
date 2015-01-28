(function () {
    var sfSelectors = angular.module('sfSelectors');
    sfSelectors.requires.push('sfInfiniteScroll');

    sfSelectors
        .directive('sfImageSelector', ['serverContext', function (serverContext) {
            return {
                restrict: 'E',
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/images/sf-image-selector.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    var loadingStep = 10;
                    scope.items = [];

                    scope.loadMore = function () {
                        for (var i = 0; i < loadingStep; i++) {
                            scope.items.push({ Id: scope.items.length + 1 });
                        }
                    };

                    scope.loadMore();
                    scope.loadMore();
                }
            };
        }]);
})();