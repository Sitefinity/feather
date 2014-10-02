(function ($) {
    angular.module('selectors')
        .directive('taxonFilter', function () {       

            return {
                restrict: 'EA',
                scope: {
                    taxonomyFields: '=',
                    additionalFilters: '=',
                    groupLogicalOperator: '@',
                    itemLogicalOperator: '@',
                    provider: '=?'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.templateUrl || 'Selectors/taxon-filter.html';
                    return sitefinity.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    post: function (scope, element, attrs, ctrl) {
                        // ------------------------------------------------------------------------
                        // helper methods
                        // ------------------------------------------------------------------------
                        var addChildTaxonQueryItem = function (taxonItem) {
                            var groupName = taxonItem.TaxonomyName;
                            var groupItem = scope.additionalFilters.getItemByName(groupName);

                            if (!groupItem) {
                                groupItem = scope.additionalFilters.addGroup(groupName, 'AND');
                            }

                            scope.additionalFilters.addChildToGroup(groupItem, taxonItem.Name, 'OR', groupName, 'System.Guid', 'Contains', taxonItem.Id);
                        };

                        var constructFilterItem = function (selectedTaxonomyFilterKey) {
                            var selectedTaxonQueryItems = scope.additionalFilters.QueryItems.filter(function (f) {
                                return f.Condition && f.Condition.FieldName == selectedTaxonomyFilterKey
                                    && f.Condition.FieldType == 'System.Guid';
                            });

                            if (selectedTaxonQueryItems.length > 0)
                                scope.selectedTaxonomies[selectedTaxonomyFilterKey] = selectedTaxonQueryItems[0].Value;
                            else
                                scope.selectedTaxonomies[selectedTaxonomyFilterKey] = null;
                        };

                        var populateSelectedTaxonomies = function () {
                            if (!scope.selectedTaxonomies) {
                                scope.selectedTaxonomies = [];
                            }

                            if (scope.additionalFilters.QueryItems) {
                                scope.additionalFilters.QueryItems.forEach(function (queryItem) {
                                    {
                                        if (queryItem.IsGroup)
                                            constructFilterItem(queryItem.Name);
                                    }
                                });
                            }
                        };

                        // ------------------------------------------------------------------------
                        // Scope variables and setup
                        // ------------------------------------------------------------------------

                        scope.itemSelected = function (itemSelectedArgs) {
                            var newSelectedTaxonItem = itemSelectedArgs.newSelectedItem;
                            var oldSelectedTaxonItem = itemSelectedArgs.oldSelectedItem;

                            if (oldSelectedTaxonItem && oldSelectedTaxonItem.Id) {
                                var groupToRemove = scope.additionalFilters.getItemByName(oldSelectedTaxonItem.TaxonomyName);
                                scope.additionalFilters.removeGroup(groupToRemove);
                            }

                            if (newSelectedTaxonItem && newSelectedTaxonItem.Id) {
                                addChildTaxonQueryItem(newSelectedTaxonItem);
                            }
                        };
                        
                        scope.toggleTaxonomySelection = function (taxonomyName) {
                            // is currently selected
                            if (taxonomyName in scope.selectedTaxonomies) {
                                delete scope.selectedTaxonomies[taxonomyName];

                                var groupToRemove = scope.additionalFilters.getItemByName(taxonomyName);
                                scope.additionalFilters.removeGroup(groupToRemove);
                            }

                            // is newly selected
                            else {
                                constructFilterItem(taxonomyName);
                            }
                        };

                        populateSelectedTaxonomies();
                    }
                }
            };
        });
})(jQuery);