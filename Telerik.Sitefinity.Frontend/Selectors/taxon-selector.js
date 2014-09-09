(function ($) {
    angular.module('selectors')
        .directive('taxonSelector', ['flatTaxonService', function (flatTaxonService) {
            return {
                require: "^flatSelector",
                restrict: "A",
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        ctrl.getItems = function (skip, take, search) {
                            return flatTaxonService.getTaxons(ctrl.taxonomyId, ctrl.provider, skip, take, search);
                        }

                        ctrl.getItem = function (id) {
                            return flatTaxonService.getTaxon(ctrl.taxonomyId, id, ctrl.provider);
                        }

                        ctrl.onSelectedItemLoadedSuccess = function (data) {
                            if (!ctrl.selectedItem) {
                                ctrl.updateSelectedItem(data);
                            }

                            if (!ctrl.selectedItemId) {
                                ctrl.updateSelectedItemId(data.Id);
                            }
                        };

                        ctrl.templateUrl = 'Selectors/taxon-selector.html';
                    }
                }
            };
        }]);
})(jQuery);