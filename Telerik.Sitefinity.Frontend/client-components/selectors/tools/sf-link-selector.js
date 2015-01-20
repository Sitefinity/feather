(function () {
    angular.module('sfSelectors')
        .directive('sfLinkSelector', ['serverContext', 'sfLinkService', function (serverContext, linkService) {
            return {
                restrict: 'E',
                scope: {
                    sfLinkHtml: '@'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/tools/sf-link-selector.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    post: function (scope, element, attrs, ctrl) {

                        scope.insertLink = function () {
                            var htmlLinkObj = linkService.getHtmlLink(scope.selectedItem);
                            scope.sfLinkHtml = htmlLinkObj[0];
                            scope.$modalInstance.close();
                        };

                        scope.cancel = function () {
                            scope.$modalInstance.close();
                        };

                        scope.selectedItem = linkService.constructLinkItem(jQuery(scope.sfLinkHtml));
                    }
                }
            };
        }]);
})();