(function () {
    angular.module('sfSelectors')
        .directive('sfLinkSelector', ['serverContext', 'sfLinkService', 'sfLinkMode', function (serverContext, linkService, sfLinkMode) {
            return {
                restrict: 'E',
                scope: {
                    sfLinkHtml: '=',
                    sfSelectedItem: '='
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/tools/sf-link-selector.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    post: function (scope, element, attrs, ctrl) {
                        scope.sfLinkMode = sfLinkMode;
                        scope.sfSelectedItem = linkService.constructLinkItem(jQuery(scope.sfLinkHtml));
                    }
                }
            };
        }]);
})();