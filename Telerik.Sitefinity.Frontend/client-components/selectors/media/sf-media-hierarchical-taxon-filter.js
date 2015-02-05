(function ($) {
    angular.module('sfMediaHierarchicalTaxonFilter', ['sfServices', 'sfTree', 'sfCollection'])
    .directive('sfMediaHierarchicalTaxonFilter', ['serverContext', 'sfMediaService', 'sfHierarchicalTaxonService',
    function (serverContext, mediaService, taxonService) {
        return {
            restrict: 'AE',
            scope: {
                filterObject: '=sfModel',
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
                scope.selectedTaxonId = null;
                scope.filteredTaxons = [];

                scope.requestChildrenCallback = function (taxon) {
                    if (taxon) {
                        return getChildTaxons(taxon.Id);
                    }
                    else {
                        return getTaxons();
                    }
                };

                scope.$watch('selectedTaxonId', function (newVal, oldVal) {
                    if (!newVal || newVal === oldVal)
                        return;

                    if (scope.filterObject &&
                        scope.filterObject.taxon &&
                        scope.filterObject.taxon.id === newVal)
                        return;

                    var selectedTaxonId;
                    if (angular.isArray(newVal) && newVal.length > 0) {
                        selectedTaxonId = newVal[0];
                    }
                    else {
                        return;
                    }

                    var filter = mediaService.newFilter();
                    filter.taxon.id = selectedTaxonId;
                    filter.taxon.field = scope.sfField;

                    scope.filterObject = filter;
                });

                scope.$watch('query', function (newVal, oldVal) {
                    if (!newVal || newVal === oldVal)
                        return;

                    getTaxons().then(function (items) {
                       scope.filteredTaxons = items;
                    });
                });

                function getTaxons() {
                    var skip = 0;
                    var pageSize = 100; // not used by the service
                    return taxonService.getTaxons(
                            scope.sfTaxonomyId,
                            skip,
                            pageSize,
                            scope.query)
                        .then(function (data) {
                            return data.Items;
                        });
                }

                function getChildTaxons(parentId) {
                    return taxonService.getChildTaxons(parentId, scope.query)
                        .then(function (data) {
                            return data.Items;
                        });
                }
            }
        };
    }]);
})(jQuery);