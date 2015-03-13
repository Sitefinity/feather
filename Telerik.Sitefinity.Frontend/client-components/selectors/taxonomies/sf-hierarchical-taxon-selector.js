(function ($) {
    angular.module('sfSelectors')
        .directive('sfHierarchicalTaxonSelector', ['serviceHelper', 'sfHierarchicalTaxonService', function (serviceHelper, hierarchicalTaxonService) {
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
                    taxonToAdd.TitlesPath = taxonToAdd.TitlesPath || taxonPathTitle;

                    taxonToAdd.Breadcrumb = taxonToAdd.TitlesPath;
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
                            var taxonomyId = attrs.sfTaxonomyId;

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
                                return hierarchicalTaxonService.getSpecificItems(taxonomyId, ids);
                            };

                            ctrl.onSelectedItemsLoadedSuccess = function (data) {
                                var items = [];

                                angular.forEach(data.Items, function (result) {
                                    items.push(_applyBreadcrumbPath({ Items: result }));
                                });

                                ctrl.updateSelection(items);
                            };

                            ctrl.onItemSelected = function (item) {
                                item.Breadcrumb = item.TitlesPath ? item.TitlesPath + " > " + item.Title : item.Title;
                            };

                            ctrl.onFilterItemSucceeded = function (items) {
                                angular.forEach(items, function (item) {
                                    item.RootPath = item.TitlesPath ? "Under " + item.TitlesPath : 'On Top Level';
                                });
                            };

                            ctrl.selectorType = 'HierarchicalTaxonSelector';
                            ctrl.dialogTemplateUrl = 'client-components/selectors/taxonomies/sf-hierarchical-taxon-selector.html';
                            ctrl.$scope.dialogTemplateId = 'sf-hierarchical-taxon-selector';
                            ctrl.closedDialogTemplateUrl = attrs.sfMultiselect ? 'client-components/selectors/common/sf-list-group-selection.html' :
                                'client-components/selectors/common/sf-bubbles-selection.html';

                            ctrl.$scope.hierarchical = true;
                            ctrl.$scope.sfIdentifierField = "Breadcrumb";
                            ctrl.$scope.searchIdentifierField = "Title";
                        }
                    }
                };
        }]);
        })(jQuery);