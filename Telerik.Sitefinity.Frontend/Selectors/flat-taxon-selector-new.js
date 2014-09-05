(function ($) {
    angular.module('selectors')
        .directive('flatTaxonSelectorNew', ['flatTaxonService', function (flatTaxonService) {
            return {
                require: "^flatSelector",
                scope: {
                    taxonomyId: '=',
                    provider: '=',
                    selectedTaxonId: '=?',
                    selectedTaxon: '=?'
                },

                link: function (scope, element, attrs, flatSelectorCtrl) {
                    debugger;
                    element.scope().getItems = function (skip, take, search) {
                        return flatTaxonService.getTaxons(scope.taxonomyId, scope.provider, skip, take, search);
                    }
                    
                    element.scope().getItem = function (id) {
                        return flatTaxonService.getTaxon(scope.taxonomyId, id, scope.provider);
                    }

                    element.scope().onSelectedItemLoadedSuccess = function (data) {
                        if (!element.scope().selectedItem) {
                            element.scope().selectedItem = data;
                        }

                        if (!element.scope().selectedItemId) {
                            element.scope().selectedItemId = data.Id;
                        }
                    };

                    element.scope().selectedItemId = scope.selectedTaxonId;
                    element.scope().selectedItem = scope.selectedTaxon;
                    element.scope().templateUrl = 'Selectors/flat-taxon-selector.html';

                }
            };
        }]);
})(jQuery);