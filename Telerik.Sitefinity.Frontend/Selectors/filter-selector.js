(function ($) {
    angular.module('selectors')
        .directive('filterSelector', function () {
            return {
                restrict: 'EA',
                scope: {
                    taxonomyFields: '=',
                    additionalFilters: '=',
                    provider: '=?'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.templateUrl || 'Selectors/filter-selector.html';
                    return sitefinity.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        if(!scope.additionalFilters.QueryItems)
                            scope.additionalFilters = new Telerik.Sitefinity.Web.UI.QueryData();
                    }
                }
            };
        });
})(jQuery);