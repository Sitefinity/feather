(function ($, selectorModule) {
    selectorModule.directive('sfMultiSiteSelector', ['sfMultiSiteService', function (multiSiteService) {
        return {
            restrict: 'E',
            templateUrl: function (elem, attrs) {
                var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                var url = attrs.sfTemplateUrl || 'client-components/selectors/multi-site/sf-multi-site-selector.html';
                return sitefinity.getEmbeddedResourceUrl(assembly, url);
            },
            link: function (scope, element, attrs, ctrl) {
                var getSitesForUserPromise = multiSiteService.getSitesForUserPromise({
                    sortExpression: 'Name'
                });

                getSitesForUserPromise.then(function (data) {
                    scope.sites = data.Items;
                });

                getSitesForUserPromise.catch(function (error) {
                    scope.showError = true;
                    scope.errorMessage = error;
                });
            }
        };
    }]);
})(jQuery, angular.module('sfSelectors'));
