(function ($) {
    angular.module('selectors')
        .directive('taxonSelector', ['flatTaxonService', function (flatTaxonService) {
            // Tags Id
            var defaultTaxonomyId = "cb0f3a19-a211-48a7-88ec-77495c0f5374";

            return {
                require: "^flatSelector",
                restrict: "A",
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        var taxonomyId;
                        if (ctrl.taxonomyId && ctrl.taxonomyId !== "00000000-0000-0000-0000-000000000000") {
                            taxonomyId = ctrl.taxonomyId;
                        }
                        else {
                            taxonomyId = defaultTaxonomyId;
                        }

                        ctrl.getItems = function (skip, take, search) {
                            return flatTaxonService.getTaxons(taxonomyId, ctrl.provider, skip, take, search);
                        };

                        ctrl.getItem = function (id) {
                            return flatTaxonService.getTaxon(taxonomyId, id, ctrl.provider);
                        };

                        ctrl.onSelectedItemLoadedSuccess = function (data) {
                            if (!ctrl.selectedItem) {
                                ctrl.updateSelectedItem(data);
                            }

                            if (!ctrl.selectedItemId) {
                                ctrl.updateSelectedItemId(data.Id);
                            }
                        };

                        ctrl.templateUrl = 'Selectors/taxon-selector.html';
                        ctrl.setPartialTemplate('taxon-selector-template');
                    }
                }
            };
        }]);
})(jQuery);