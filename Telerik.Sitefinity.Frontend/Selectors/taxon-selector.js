(function ($) {
    angular.module('selectors')
        .directive('taxonSelector', ['flatTaxonService', function (flatTaxonService) {
            // Tags Id
            var defaultTaxonomyId = "cb0f3a19-a211-48a7-88ec-77495c0f5374";
            var emptyGuid = "00000000-0000-0000-0000-000000000000";

            return {
                require: "^listSelector",
                restrict: "A",
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        var taxonomyId = (ctrl.$scope.taxonomyId && ctrl.$scope.taxonomyId !== emptyGuid) || defaultTaxonomyId;

                        ctrl.getItems = function (skip, take, search) {
                            return flatTaxonService.getTaxons(taxonomyId, skip, take, search);
                        };

                        ctrl.getItem = function (id) {
                            return flatTaxonService.getTaxon(taxonomyId, id);
                        };

                        ctrl.onSelectedItemLoadedSuccess = function (data) {
                            ctrl.updateSelection(data);
                        };

                        ctrl.selectorType = 'TaxonSelector';
                        ctrl.templateUrl = 'Selectors/taxon-selector.html';

                        ctrl.$scope.partialTemplate = 'taxon-selector-template';
                    }
                }
            };
        }]);
})(jQuery);