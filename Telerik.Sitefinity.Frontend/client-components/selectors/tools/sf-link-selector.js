(function () {
    angular.module('sfSelectors')
        .directive('sfLinkSelector',
                  ['serverContext',
                   'sfLinkService',
                   'sfLinkMode',
                   'sfMultiSiteService',
                   'sfPageService',
                   function (serverContext, linkService, sfLinkMode, siteService, pageService) {
                       return {
                           restrict: 'E',
                           scope: {
                               sfLinkHtml: '=',
                               sfSelectedItem: '=',
                               sfEditorContent: '='
                           },
                           templateUrl: function (elem, attrs) {
                               var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                               var url = attrs.sfTemplateUrl || 'client-components/selectors/tools/sf-link-selector.sf-cshtml';
                               return serverContext.getEmbeddedResourceUrl(assembly, url);
                           },
                           link: {
                               post: function (scope, element, attrs, ctrl) {

                                   var init = function () {

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
                                       scope.defaultDisplayText = selectedItem.displayText;
                                   };

                                   var isCultureDefaultForSite = function (site, culture) {
                                       if (!site || !culture ||
                                           !culture.SitesUsingCultureAsDefault || culture.SitesUsingCultureAsDefault.length === 0) {
                                           return false;
                                       }
                                       return culture.SitesUsingCultureAsDefault.indexOf(site.Name) > -1;
                                   };

                                   if (serverContext.isMultisiteEnabled()) {
                                       if (siteService.getSites().length > 0) {
                                           init();
                                       }
                                       else {
                                           siteService.addHandler(function () {
                                               init();
                                           });
                                       }
                                   } else {
                                       init();
                                   }

                                   scope.$watch('sfSelectedItem.selectedPage', function () {
                                       if (scope.sfSelectedItem &&
                                           scope.sfSelectedItem.selectedPage &&
                                           scope.sfCulture &&
                                           (!scope.defaultDisplayText || !scope.sfSelectedItem.displayText)) {
                                           scope.sfSelectedItem.displayText = pageService.getPageTitleByCulture(scope.sfSelectedItem.selectedPage, scope.sfCulture.Culture);
                                           scope.defaultDisplayText = '';
                                       }
                                   });

                                   scope.$watch('sfCulture', function () {
                                       //// Clear dispaly text only in 'Page from current site' mode and culture for selected page is different from the current selected culture.
                                       if (scope.sfCulture &&
                                           scope.sfSelectedItem &&
                                           scope.sfSelectedItem.mode === sfLinkMode.InternalPage &&
                                           scope.sfSelectedItem.language !== scope.sfCulture.Culture) {

                                           if (!isCultureDefaultForSite(scope.sfSite, scope.sfCulture) &&
                                               !scope.defaultDisplayText) {
                                               scope.sfSelectedItem.displayText = '';
                                           }

                                           scope.sfSelectedItem.language = scope.sfCulture.Culture;
                                       }
                                   });

                                   scope.isTestLinkHidden = function () {
                                       if (scope.sfSelectedItem) {
                                           return scope.sfSelectedItem.linkHasChildrenElements ||
                                               !scope.sfSelectedItem.webAddress ||
                                               scope.sfSelectedItem.webAddress === 'http://' ||
                                               !scope.sfSelectedItem.displayText;
                                       }

                                       return true;
                                   };
                               }
                           }
                       };
                   }]);
})();