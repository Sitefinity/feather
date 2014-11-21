(function ($) {
    angular.module('sfSelectors')
        .directive('sfFilterSelector', function () {
            return {
                restrict: 'EA',
                scope: {
                    taxonomyFields: '=',
                    queryData: '=',
                    provider: '=?'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.templateUrl || 'client-components/selectors/common/sf-filter-selector.html';
                    return sitefinity.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        if (scope.queryData && scope.queryData.QueryItems)
                            scope.queryData = new Telerik.Sitefinity.Web.UI.QueryData(scope.queryData);
                        else
                            scope.queryData = new Telerik.Sitefinity.Web.UI.QueryData();
                    }
                }
            };
        });
})(jQuery);