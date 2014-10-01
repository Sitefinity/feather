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

                        var addChildTaxonQueryItem = function (taxonItem) {
                            var groupName = taxonItem.TaxonomyName;
                            var groupItem = scope.additionalFilters.getItemByName(groupName);

                            if (!groupItem) {
                                groupItem = scope.additionalFilters.addGroup(groupName, 'AND');
                            }

                            scope.additionalFilters.addChildToGroup(groupItem, taxonItem.Name, 'OR', groupName, 'System.Guid', 'Contains', taxonItem.Id);
                        };

                        var populateSelectedTaxonomies = function () {
                            if (!scope.selectedTaxonomies) {
                                scope.selectedTaxonomies = [];
                            }

                            if (scope.additionalFilters.QueryItems) {
                                scope.additionalFilters.QueryItems.forEach(function (queryItem) {
                                    {
                                        if (queryItem.IsGroup)
                                            scope.selectedTaxonomies.push(queryItem.Name);
                                    }
                                });
                            }
                        };

                        scope.itemSelected = function (itemSelectedArgs) {
                            var newSelectedTaxonItem = itemSelectedArgs.newSelectedItem;
                            var oldSelectedTaxonItem = itemSelectedArgs.oldSelectedItem;

                            if (oldSelectedTaxonItem) {
                                var groupToRemove = scope.additionalFilters.getItemByName(oldSelectedTaxonItem.TaxonomyName);
                                scope.additionalFilters.removeGroup(groupToRemove);
                            }

                            if (newSelectedTaxonItem) {
                                addChildTaxonQueryItem(newSelectedTaxonItem);
                            }
                        };
                        
                        scope.toggleTaxonomySelection = function (taxonomyName) {

                            var idx = scope.selectedTaxonomies.indexOf(taxonomyName);

                            // is currently selected
                            if (idx > -1) {
                                scope.selectedTaxonomies.splice(idx, 1);

                                var groupToRemove = scope.additionalFilters.getItemByName(taxonomyName);
                                scope.additionalFilters.removeGroup(groupToRemove);
                            }

                            // is newly selected
                            else {
                                scope.selectedTaxonomies.push(taxonomyName);
                            }
                        };

                        populateSelectedTaxonomies();
                    }
                }
            };
        });
})(jQuery);