(function ($) {
    angular.module('selectors')
        .directive('taxonFilter', ['taxonomyService', function (taxonomyService) {
            return {
                restrict: 'EA',
                scope: {
                    taxonFilters: '=',
                    selectedTaxonomies: '=',
                    provider: '=?'
                },
                controller: function ($scope) {
                    this.taxonFilters = $scope.taxonFilters;
                    this.selectedTaxonomies = $scope.selectedTaxonomies;
                    this.provider = $scope.provider;
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.templateUrl || 'Selectors/taxon-filter.html';
                    return sitefinity.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    post: function (scope, element, attrs, ctrl) {

                        scope.toggleTaxonomySelection = function (taxonomyName) {
                            if (!scope.selectedTaxonomies)
                                scope.selectedTaxonomies = [];

                            var idx = scope.selectedTaxonomies.indexOf(taxonomyName);

                            // is currently selected
                            if (idx > -1) {
                                scope.selectedTaxonomies.splice(idx, 1);

                                delete scope.taxonFilters[taxonomyName];
                            }

                                // is newly selected
                            else {
                                scope.selectedTaxonomies.push(taxonomyName);
                                
                                if (!scope.taxonFilters[taxonomyName])
                                    scope.taxonFilters[taxonomyName] = [];
                            }
                        };

                        onGetTaxonomiesSuccess = function (data) {
                            scope.allTaxonomies = data.Items;
                        };

                        taxonomyService.getTaxonomies(scope.provider, null, null, null, 'FlatTaxonomy')
                            .then(onGetTaxonomiesSuccess);
                    }
                }
            };
        }]);
})(jQuery);