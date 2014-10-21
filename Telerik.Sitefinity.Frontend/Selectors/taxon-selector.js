(function ($) {
    angular.module('selectors')
        .directive('taxonSelector', ['flatTaxonService', function (flatTaxonService) {
            // Tags Id
            var defaultTaxonomyId = 'cb0f3a19-a211-48a7-88ec-77495c0f5374';

            return {
                require: '^listSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        var taxonomyId;
                        if (ctrl.getTaxonomyId() && ctrl.getTaxonomyId() !== '00000000-0000-0000-0000-000000000000') {
                            taxonomyId = ctrl.getTaxonomyId();
                        }
                        else {
                            taxonomyId = defaultTaxonomyId;
                        }

                        ctrl.getItems = function (skip, take, search) {
                            return flatTaxonService.getTaxons(taxonomyId, skip, take, search);
                        };

                        ctrl.getItem = function (id) {
                            return flatTaxonService.getTaxon(taxonomyId, id);
                        };

                        ctrl.onSelectedItemLoadedSuccess = function (data) {
                            if (!ctrl.getSelectedItem()) {
                                ctrl.updateSelectedItem(data);
                            }

                            if (!ctrl.getSelectedItemId()) {
                                ctrl.updateSelectedItemId(data.Id);
                            }
                        };

                        ctrl.setSelectorType('TaxonSelector');

                        ctrl.templateUrl = 'Selectors/taxon-selector.html';
                        ctrl.setPartialTemplate('taxon-selector-template');
                    }
                }
            };
        }]);
})(jQuery);