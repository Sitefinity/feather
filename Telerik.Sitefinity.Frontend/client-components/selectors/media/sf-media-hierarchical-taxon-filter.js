(function ($) {
    angular.module('sfMediaHierarchicalTaxonFilter', ['sfServices', 'sfTree'])
        .directive('sfMediaHierarchicalTaxonFilter', ['serverContext', 'sfMediaService', 'sfHierarchicalTaxonService', function (serverContext, mediaService, taxonService) {
            return {
                restrict: 'AE',
                scope: {
                    filterObject: '=ngModel',
                    sfField: '@',
                    sfTitle: '@',
                    sfTaxonomyId: '@'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/media/sf-media-hierarchical-taxon-filter.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {

                    scope.selectedItemId = null;

                    scope.requestChildrenCallback = function (parent) {

                    };

                    scope.$watch('selectedItemId', function (newVal, oldVal) {

                    });
                }
            };
        }]);
})(jQuery);