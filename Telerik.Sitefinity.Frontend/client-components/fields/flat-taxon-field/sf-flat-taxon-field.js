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
                var existingTaxons = [];
                scope.loadTaxons = function (query) {
                    return taxonService.getTaxons(scope.sfTaxonomyId, null, null, query)
                        .then(function (data) {
                            if (data && data.Items) {
                                existingTaxons = jQuery.map(data.Items, function (item, index) {
                                    return item.Title;
                                });
                                return data.Items;
                            }
                            else return [];
                        })
                        .finally(function () {
                            scope.isLoading = false;
                        });
                };

                scope.taxonAdded = function (newTaxon) {
                    if (newTaxon && existingTaxons.indexOf(newTaxon) === -1) {
                        taxonService.addTaxa(scope.sfTaxonomyId, null, null, [newTaxon.Title])
                            .then(function (data) {
                                if (data && data.length > 0) {
                                    newTaxon.Id = data[0].Id;
                                }
                            });
                    }
                };
            }
        };
    }]);
})(jQuery);
