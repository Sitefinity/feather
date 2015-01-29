(function ($, selectorModule) {
    selectorModule.directive('sfMultisitePageSelector', ['serverContext', 'sfMultiSiteService', function (serverContext, siteService) {
        return {
            restrict: 'E',
            scope: {
                sfSite: '=?',
                sfCulture: '=?',
                sfSelectedItemId: '=?',
                sfSelectedItem: '=?',
                sfBindOnLoad: '@?'
            },
            templateUrl: function (elem, attrs) {
                var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                var url = attrs.sfTemplateUrl || 'client-components/selectors/multi-site/sf-multisite-page-selector.html';
                return serverContext.getEmbeddedResourceUrl(assembly, url);
            },
            link: function (scope) {
                var hasMultipleSites = function () {
                    scope.hasMultipleSites = siteService.getSites().length > 1;
                };

                if (siteService.getSites().length > 0) {
                    hasMultipleSites();
                }
                else {
                    siteService.addHandler(function () {
                        hasMultipleSites();
                    });
                }
            }
        };
    }]);
})(jQuery, angular.module('sfSelectors'));
