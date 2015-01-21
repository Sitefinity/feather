(function ($, selectorModule) {
    selectorModule.directive('sfSiteSelector', ['sfMultiSiteService', function (multiSiteService) {
        return {
            restrict: 'E',
            scope: {
                sfSite: '='
            },
            templateUrl: function (elem, attrs) {
                var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                var url = attrs.sfTemplateUrl || 'client-components/selectors/multi-site/sf-site-selector.html';
                return sitefinity.getEmbeddedResourceUrl(assembly, url);
            },
            link: function (scope, element, attrs) {
                var getSitesForUserPromise = multiSiteService.getSitesForUserPromise({
                    sortExpression: 'Name'
                });

                getSitesForUserPromise.then(function (data) {
                    scope.sfSites = data.Items;
                    
                    multiSiteService.currentSites = scope.sfSites;

                    if (scope.sfSites.length > 0 && !scope.sfSite) {
                        scope.sfSite = scope.sfSites[0];
                    }
                });

                getSitesForUserPromise.catch(function (error) {
                    scope.showError = true;
                    scope.errorMessage = error;
                });
            }
        };
    }]);
})(jQuery, angular.module('sfSelectors'));
