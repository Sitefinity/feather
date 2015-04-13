(function ($, selectorModule) {
    selectorModule.directive('sfSiteSelector', ['sfMultiSiteService', 'serverContext', function (multiSiteService, serverContext) {
        return {
            restrict: 'E',
            scope: {
                sfSite: '='
            },
            templateUrl: function (elem, attrs) {
                var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                var url = attrs.sfTemplateUrl || 'client-components/selectors/multi-site/sf-site-selector.sf-cshtml';
                return serverContext.getEmbeddedResourceUrl(assembly, url);
            },
            link: function (scope, element, attrs) {
                if (serverContext.isMultisiteEnabled()) {
                    var getSitesForUserPromise = multiSiteService.getSitesForUserPromise({
                        sortExpression: 'Name'
                    });

                    getSitesForUserPromise.then(function (data) {
                        scope.sfSites = data.Items;

                        if (scope.sfSites.length > 0 && !scope.sfSite) {
                            var currentSiteMapRootNodeId = serverContext.getCurrentFrontendRootNodeId();
                            scope.sfSite = scope.sfSites.filter(function (site) {
                                return site.SiteMapRootNodeId === currentSiteMapRootNodeId;
                            })[0];
                        }
                    });

                    getSitesForUserPromise.catch(function (error) {
                        scope.showError = true;
                        scope.errorMessage = error;
                    });
                }
            }
        };
    }]);
})(jQuery, angular.module('sfSelectors'));
