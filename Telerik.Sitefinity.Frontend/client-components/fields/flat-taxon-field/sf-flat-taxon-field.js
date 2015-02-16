(function ($) {
    var sfFields = angular.module('sfFields');
    sfFields.requires.push('sfFlatTaxonField');
    var module = angular.module('sfFlatTaxonField', ['sfServices', 'ngTagsInput']);

    module.directive('sfFlatTaxonField', ['serverContext', 'sfFlatTaxonService', function (serverContext, taxonService) {
        return {
            restrict: "E",
            scope: {
                sfModel: '=',
                sfTaxonomyId: '@',
                sfEnableAutoComplete: '@'
            },
            templateUrl: function (elem, attrs) {
                var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                var url = attrs.sfTemplateUrl || 'client-components/fields/flat-taxon-field/sf-flat-taxon-field.html';
                return serverContext.getEmbeddedResourceUrl(assembly, url);
            },
            link: function (scope, element) {
                scope.selectedTaxons = [];

                var retrieveInitialTaxons = function () {
                    if (scope.sfModel && scope.sfModel.length > 0) {
                        scope.isLoading = true;
                        taxonService.getSpecificItems(scope.sfTaxonomyId, scope.sfModel)
                            .then(function (data) {
                                if (data && data.Items) {
                                    scope.selectedTaxons = data.Items;
                                }
                            })
                            .finally(function () {
                                scope.isLoading = false;
                            });
                    }
                };

                var mapModel = function () {
                    scope.sfModel = jQuery.map(scope.selectedTaxons, function (item, index) {
                        return item.Id;
                    });
                };

                retrieveInitialTaxons();

                scope.loadTaxons = function (query) {
                    return taxonService.getTaxons(scope.sfTaxonomyId, null, null, query)
                        .then(function (data) {
                            if (data && data.Items) {
                                return data.Items;
                            }
                            else return [];
                        })
                        .finally(function () {
                            scope.isLoading = false;
                        });
                };

                scope.taxonAdded = function (newTaxon) {
                    if (newTaxon.Id) {
                        mapModel();
                    }
                    else if (newTaxon.Title) {
                        taxonService.addTaxa(scope.sfTaxonomyId, null, null, [newTaxon.Title])
                            .then(function (data) {
                                if (data && data.length > 0) {
                                    newTaxon.Id = data[0].Id;
                                    mapModel();
                                }
                            });
                    }
                };

                scope.taxonRemoved = function (removedTaxon) {
                    mapModel();
                };
            }
        };
    }]);
})(jQuery);
