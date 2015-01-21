(function () {
    angular.module('sfSelectors')
        .directive('sfLinkSelector',
                  ['serverContext',
                   'sfLinkService',
                   'sfLinkMode',
                   'sfMultiSiteService',
                   function (serverContext, linkService, sfLinkMode, siteService) {
                       return {
                           restrict: 'E',
                           scope: {
                               sfLinkHtml: '=',
                               sfEditorContent: '@'
                           },
                           templateUrl: function (elem, attrs) {
                               var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                               var url = attrs.sfTemplateUrl || 'client-components/selectors/tools/sf-link-selector.html';
                               return serverContext.getEmbeddedResourceUrl(assembly, url);
                           },
                           link: {
                               post: function (scope, element, attrs, ctrl) {

                                   var init = function () {
                                       
                                       var selectedItem = linkService.constructLinkItem(jQuery(scope.sfLinkHtml));

                                       scope.sfSite = siteService.getSiteByRootNoteId(selectedItem.rootNodeId);

                                       scope.sfCulture = { Culture: selectedItem.language };

                                       scope.selectedItem = selectedItem;
                                   };

                                   scope.insertLink = function () {
                                       var htmlLinkObj = linkService.getHtmlLink(scope.selectedItem);
                                       scope.sfLinkHtml = htmlLinkObj[0];
                                       scope.$modalInstance.close();
                                   };

                                   scope.cancel = function () {
                                       scope.$modalInstance.close();
                                   };

                                   scope.anchors = linkService.populateAnchorIds(scope.sfEditorContent);
                                   scope.sfLinkMode = sfLinkMode;

                                   siteService.addHandler(function () {
                                       init();
                                   });
                               }
                           }
                       };
                   }]);
})();