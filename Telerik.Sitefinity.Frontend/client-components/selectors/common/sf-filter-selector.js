(function ($) {
    angular.module('sfSelectors')
        .directive('sfFilterSelector', function () {
            return {
                restrict: 'EA',
                scope: {
                    sfTaxonomyFields: '=',
                    sfQueryData: '=',
                    sfProvider: '=?',
                    sfDateGroups: '=?',
                    sfGroupLogicalOperator: '=?'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/common/sf-filter-selector.sf-cshtml';
                    return sitefinity.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        if (!scope.sfGroupLogicalOperator)
                            scope.sfGroupLogicalOperator = 'AND';

                        if (scope.sfQueryData && scope.sfQueryData.QueryItems)
                            scope.sfQueryData = new Telerik.Sitefinity.Web.UI.QueryData(scope.sfQueryData);
                        else
                            scope.sfQueryData = new Telerik.Sitefinity.Web.UI.QueryData();

                        refreshQueryGroupLogicalOperator = function () {
                            var itemCount = scope.sfQueryData.QueryItems.length;
                            for (var i = 0; i < itemCount; i++) {
                                var itemPath = scope.sfQueryData.QueryItems[i].get_ItemPath();
                                var itemLevel = scope.sfQueryData.getItemLevel(itemPath);

                                if (itemLevel === 0) {
                                    if (scope.sfQueryData.QueryItems[i].get_Join() === scope.sfGroupLogicalOperator.toUpperCase())
                                        return;

                                    scope.sfQueryData.QueryItems[i].set_Join(scope.sfGroupLogicalOperator.toUpperCase());
                                }
                            }
                        };

                        scope.$watch(
                               'sfGroupLogicalOperator',
                                function (newLogicalOperator, oldLogicalOperator) {
                                    if (newLogicalOperator.toUpperCase() === 'OR' || newLogicalOperator.toUpperCase() === 'AND') {
                                        this.refreshQueryGroupLogicalOperator();
                                    }
                                },
                                true
                              );
                    }
                }
            };
        });
})(jQuery);