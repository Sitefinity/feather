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
                               sfSelectedItem: '=',
                               sfEditorContent: '='
                           },
                           templateUrl: function (elem, attrs) {
                               var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                               var url = attrs.sfTemplateUrl || 'client-components/selectors/tools/sf-link-selector.html';
                               return serverContext.getEmbeddedResourceUrl(assembly, url);
                           },
                           link: {
                               post: function (scope, element, attrs, ctrl) {
                                   var init = function () {

                                       debugger;

                                       scope.anchors = linkService.populateAnchorIds(scope.sfEditorContent);
                                       scope.sfLinkMode = sfLinkMode;

                                       if (typeof (scope.sfLinkHtml) === "string") {
                                           var resultLink = jQuery('<a></a>');
                                           resultLink.html(scope.sfLinkHtml);
                                           scope.sfLinkHtml = resultLink;
                                       }

                                       var selectedItem = linkService.constructLinkItem(jQuery(scope.sfLinkHtml));

                                       scope.sfSite = siteService.getSiteByRootNoteId(selectedItem.rootNodeId);
                                       scope.sfCulture = { Culture: selectedItem.language };

                                       scope.sfSelectedItem = selectedItem;
                                   };

                                   if (siteService.getSites().length > 0) {
                                       init();
                                   }
                                   else {
                                       siteService.addHandler(function () {
                                           init();
                                       });
                                   }
                               }
                           }
                       };
                   }]);
})();