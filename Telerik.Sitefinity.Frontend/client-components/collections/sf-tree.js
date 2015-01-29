; (function ($) {
    angular.module('feather')
        .directive('sfTree', ['serverContext', function (serverContext) {
            return {
                restrict: 'A',
                scope: {

                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfItemTemplateUrl || 'client-components/collections/sf-tree-item.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, attrs.sfItemTemplateUrl);
                },
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        
                    }
                }
            };
        }]);
})(jQuery);
