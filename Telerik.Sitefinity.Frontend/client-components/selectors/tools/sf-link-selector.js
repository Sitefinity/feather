(function () {
    angular.module('sfSelectors')
        .directive('sfLinkSelector', ['serverContext', 'sfLinkService', 'sfLinkMode', function (serverContext, linkService, sfLinkMode) {
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

                        ////if (!scope.sfLinkHtml) {
                        ////    scope.sfLinkHtml = '<a href="/CodeBase/widgettests" sfref="[f669d9a7-009d-4d83-ddaa-000000000002|lng:en]28d7e74c-c789-61c4-9817-ff000095605c">LINK</a>';
                        ////}

                        scope.sfLinkMode = sfLinkMode;
                        scope.selectedItem = linkService.constructLinkItem(jQuery(scope.sfLinkHtml));

                        //scope.selectedItem.sfSite = { Id: scope.selectedItem. };
                    }
                }
            };
        }]);
})();