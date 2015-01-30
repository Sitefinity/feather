(function () {
    angular.module('sfMediaFlatTaxonFilter', ['sfServices'])
        .directive('sfMediaFlatTaxonFilter', ['sfMediaService', 'sfFlatTaxonService', function (mediaService, taxonService) {
            var pageSize = 10;

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
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/media/sf-media-flat-filter.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    scope.taxons = [];

                    scope.select = function (taxon) {
                        if (taxon === null)
                            return;

                        if (scope.filterObject && scope.filterObject.taxon && scope.filterObject.taxon === taxon.Id)
                            return;

                        var filter = mediaService.newFilter();
                        filter.taxon.id = taxon.Id;
                        filter.taxon.field = scope.sfField;
                        scope.filterObject = filter;
                    };

                    scope.loadMore = function () {
                        //taxonService.getTaxons(...).then(...);
                    };
                }
            };
        }]);
})();