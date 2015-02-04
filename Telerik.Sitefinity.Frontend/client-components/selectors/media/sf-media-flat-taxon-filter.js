(function () {
    angular.module('sfMediaFlatTaxonFilter', ['sfServices', 'sfCollection', 'sfInfiniteScroll'])
        .directive('sfMediaFlatTaxonFilter', ['serverContext', 'sfMediaService', 'sfFlatTaxonService', function (serverContext, mediaService, taxonService) {
            var pageSize = 10;

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
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/media/sf-media-flat-taxon-filter.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    scope.selectedItems = [];
                    scope.taxons = [];
                    scope.query = null;
                    scope.isLoading = false;

                    scope.$watch('selectedItems', function (newValue, oldValue) {
                        if (newValue === oldValue)
                            return;

                        if (newValue === null)
                            return;

                        var taxon = newValue.length > 0 ? newValue[0] : null;

                        if (scope.filterObject && scope.filterObject.taxon && taxon && scope.filterObject.taxon.id === taxon)
                            return;

                        var filter = mediaService.newFilter();
                        if (taxon) {
                            filter.taxon.id = taxon;
                            filter.taxon.field = scope.sfField;
                        }

                        scope.filterObject = filter;
                    });

                    scope.$watch('query', function (newValue, oldValue) {
                        if (newValue !== oldValue) {
                            load(false);
                        }
                    });

                    scope.loadMore = function () {
                        load(true);
                    };

                    var load = function (append) {
                        if (scope.isLoading)
                            return;

                        scope.isLoading = true;
                        var skip = append ? scope.taxons.length : 0;
                        taxonService.getTaxons(scope.sfTaxonomyId, skip, pageSize, scope.query)
                            .then(function (data) {
                                if (data && data.Items) {
                                    if (append) {
                                        if (scope.taxons && scope.taxons.length === skip) {
                                            scope.taxons = scope.taxons.concat(data.Items);
                                            scope.$digest();
                                        }
                                    }
                                    else {
                                        scope.taxons = data.Items;
                                    }
                                }
                            })
                            .finally(function () {
                                scope.isLoading = false;
                            });
                    };

                    load(false);
                }
            };
        }]);
})();