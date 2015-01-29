; (function ($) {
    angular.module('feather')
        .directive('sfTree', ['serverContext', function (serverContext) {
            return {
                restrict: 'AE',
                scope: {
                    selectedId: '=?ngModel',
                    sfIdentifier: '@',
                    sfParentIdentifier: '@',
                    sfRequestChildren: '^&'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfItemTemplateUrl || 'client-components/collections/sf-tree-item.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, attrs.sfItemTemplateUrl);
                },
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        scope.sfIdentifier = scope.sfIdentifier || 'Id';
                        scope.sfParentIdentifier = scope.sfParentIdentifier || 'ParentId';

                        scope.hierarchy = {};

                        scope.expandTree = function (parentId) {
                            scope.sfRequestChildren(parentId).then(function (data) {
                                // populate hierhy
                            });
                        }

                        // Initial load of root elements
                        scope.expandTree(null);
                    }
                }
            };
        }]);
})(jQuery);