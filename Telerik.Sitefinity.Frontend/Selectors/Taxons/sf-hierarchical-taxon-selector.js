(function ($) {
    angular.module('sfSelectors')
        .directive('sfHierarchicalTaxonSelector', ['serviceHelper', 'hierarchicalTaxonService', '$q', function (serviceHelper, hierarchicalTaxonService, $q) {
            var titlesPathPropertyName = "TitlesPath";
            var pathPropertyName = "Path";

            var _applyBreadcrumbPath = function (result) {
                var taxa = result.Items;
                var taxonToAdd = null;
                var taxonPathTitle = '';
                var taxaLength = taxa.length;
                if (taxaLength > 0) {
                    var taxonId = taxa[taxaLength - 1].Id;
                    var delimiter = ' > ';
                    for (var i = 0, l = taxa.length; i < l; i++) {
                        if (i == taxa.length - 1) {
                            delimiter = '';
                            taxonToAdd = taxa[i];
                        }
                        taxonPathTitle += taxa[i].Title + delimiter;
                    }
                    if (taxonId !== taxonToAdd.Id) {
                        throw 'unexpected end of the taxon path.';
                    }
                    taxonToAdd[titlesPathPropertyName] = taxonPathTitle;
                    taxonToAdd[pathPropertyName] = taxonPathTitle;
                }
                else {
                    throw "Getting the taxon path returned an empty collection.";
                }
                return taxonToAdd;
            };

            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        var taxonomyId = attrs.taxonomyId;

                        if (!taxonomyId || taxonomyId === serviceHelper.emptyGuid()) {
                            taxonomyId = sitefinity.getCategoriesTaxonomyId();
                        }

                        ctrl.getItems = function (skip, take, search, frontendLanguages) {
                            return hierarchicalTaxonService.getTaxons(taxonomyId, skip, take, search, frontendLanguages);
                        };

                        ctrl.getChildren = function (parentId, search) {
                            return hierarchicalTaxonService.getChildTaxons(parentId, search)
                                                           .then(function (data) {
                                                               return data.Items;
                                                           });
                        };

                        ctrl.getSpecificItems = function (ids) {
                            return $q.all(hierarchicalTaxonService.getSpecificItems(taxonomyId, ids));
                        };

                        ctrl.onSelectedItemsLoadedSuccess = function (data) {
                            var items = [];

                            angular.forEach(data, function (f) {
                                items.push(_applyBreadcrumbPath(f));
                            });

                            ctrl.updateSelection(items);
                        };

                        ctrl.onItemSelected = function (item) {
                            var parentsChain = [item];
                            
                            var parent = item.parentNode();

                            while (parent) {
                                parentsChain.push(parent);

                                parent = parent.parentNode();
                            }
                            parentsChain.reverse();

                            _applyBreadcrumbPath({ Items: parentsChain });
                        };

                        ctrl.selectorType = 'HierarchicalTaxonSelector';
                        ctrl.dialogTemplateUrl = 'Selectors/Taxons/sf-hierarchical-taxon-selector.html';
                        ctrl.$scope.dialogTemplateId = 'sf-hierarchical-taxon-selector';
                        ctrl.closedDialogTemplateUrl = attrs.multiselect ? 'Selectors/list-group-selection.html' : 'Selectors/bubbles-selection.html';

                        ctrl.$scope.hierarchical = true;
                        ctrl.$scope.identifierField = titlesPathPropertyName;
                        ctrl.$scope.searchIdentifierField = "Title";
                    }
                }
            };
        }]);
})(jQuery);