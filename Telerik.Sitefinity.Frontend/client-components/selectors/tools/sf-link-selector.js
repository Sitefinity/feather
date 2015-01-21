(function () {
    angular.module('sfSelectors')
        .directive('sfLinkSelector',
                  ['serverContext',
                   'sfLinkService',
                   'sfLinkMode',
                   'sfMultiSiteService',
                   function (serverContext, linkService, sfLinkMode, multiSiteService) {
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

                                   scope.sfLinkMode = sfLinkMode;
                                   scope.selectedItem = linkService.constructLinkItem(jQuery(scope.sfLinkHtml));
                                   
                                   var sites = multiSiteService.currentSites;
                                    
                                   //scope.sfSite = { SiteMapRootNodeId: scope.selectedItem.rootNodeId };
                                   scope.sfCulture = { Culture: scope.selectedItem.language };
                               }
                           }
                       };
                   }]);
})();