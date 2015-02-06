(function ($) {
    var module = angular.module('sfFields', ['kendo.directives', 'sfServices', 'ngTagsInput']);

    module.directive('sfFlatTaxonomyField', ['serverContext', 'sfFlatTaxonService', function (serverContext, taxonService) {
        return {
            restrict: "E",
            scope: {
                sfModel: '=',
                sfTaxonomyId: '@',
                sfEnableAutoComplete: '@'
            },
            templateUrl: function (elem, attrs) {
                var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                var url = attrs.sfTemplateUrl || 'client-components/fields/flat-taxonomy-field/sf-flat-taxonomy-field.html';
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
                    if (existingTaxons.indexOf(newTaxon) === -1) {
                        taxonService.addTaxa(scope.sfTaxonomyId, null, null, [newTaxon.Title]);
                    }
                }

                //scope.$watch('sfModel', function (newValue, oldValue) {
                //    if (oldValue && newValue.length > oldValue.length) {
                //        var newTaxon = newValue[newValue.length - 1];
                //        if (existingTags.indexOf(newTaxon) === -1) {
                //            taxonService.addTaxa(scope.sfTaxonomyId, null, null, [newTaxon.Title]);
                //        }

                //    }
                //}, true);
            }
        };
    }]);
})(jQuery);
