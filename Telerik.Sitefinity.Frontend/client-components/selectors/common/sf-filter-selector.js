(function ($) {
    angular.module('sfSelectors')
        .directive('sfFilterSelector', function () {
            return {
                restrict: 'EA',
                scope: {
                    sfTaxonomyFields: '=',
                    sfQueryData: '=',
                    sfProvider: '=?'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/common/sf-filter-selector.html';
                    return sitefinity.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        if (scope.sfQueryData && scope.sfQueryData.QueryItems)
                            scope.sfQueryData = new Telerik.Sitefinity.Web.UI.QueryData(scope.sfQueryData);
                        else
                            scope.sfQueryData = new Telerik.Sitefinity.Web.UI.QueryData();
                    }
                }
            };
        });
})(jQuery);