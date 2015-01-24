(function ($, selectorModule) {
    selectorModule.directive('sfMultisitePageSelector', ['serverContext', function (serverContext) {
        return {
            restrict: 'E',
            scope: {
                sfSite: '=?',
                sfCulture: '=?',
                sfSelectedItemId: '=?',
                sfSelectedItem: '=?'
            },
            templateUrl: function (elem, attrs) {
                var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                var url = attrs.sfTemplateUrl || 'client-components/selectors/multi-site/sf-multisite-page-selector.html';
                return serverContext.getEmbeddedResourceUrl(assembly, url);
            }
        };
    }]);
})(jQuery, angular.module('sfSelectors'));
