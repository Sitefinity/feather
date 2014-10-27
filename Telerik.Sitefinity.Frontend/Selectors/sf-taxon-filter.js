(function ($) {
    angular.module('sfSelectors')
        .directive('sfTaxonFilter', function () {       

            return {
                restrict: 'EA',
                scope: {
                    taxonomyFields: '=',
                    queryData: '=',
                    groupLogicalOperator: '@',
                    itemLogicalOperator: '@',
                    provider: '=?'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.templateUrl || 'Selectors/sf-taxon-filter.html';
                    return sitefinity.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    post: function (scope, element, attrs, ctrl) {
                        // ------------------------------------------------------------------------
                        // helper methods
                        // ------------------------------------------------------------------------
                        var addChildTaxonQueryItem = function (taxonItem) {
                            var groupName = taxonItem.TaxonomyName;
                            var groupItem = scope.queryData.getItemByName(groupName);

                            if (!groupItem) {
                                groupItem = scope.queryData.addGroup(groupName, scope.groupLogicalOperator);
                            }

                            scope.queryData.addChildToGroup(groupItem, taxonItem.Name, scope.itemLogicalOperator, groupName, 'System.Guid', 'Contains', taxonItem.Id);
                        };

                        var constructFilterItem = function (selectedTaxonomyFilterKey) {
                            var selectedTaxonQueryItems = scope.queryData.QueryItems.filter(function (f) {
                                return f.Condition && f.Condition.FieldName == selectedTaxonomyFilterKey &&
                                    f.Condition.FieldType == 'System.Guid';
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

                            if (scope.queryData.QueryItems) {
                                scope.queryData.QueryItems.forEach(function (queryItem) {
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

                        scope.change = function (changeArgs) {
                            var newSelectedTaxonItem = changeArgs.newSelectedItem;
                            var oldSelectedTaxonItem = changeArgs.oldSelectedItem;

                            if (oldSelectedTaxonItem && oldSelectedTaxonItem.Id) {
                                var groupToRemove = scope.queryData.getItemByName(oldSelectedTaxonItem.TaxonomyName);

                                if (groupToRemove)
                                    scope.queryData.removeGroup(groupToRemove);
                            }

                            if (newSelectedTaxonItem && newSelectedTaxonItem.Id) {
                                addChildTaxonQueryItem(newSelectedTaxonItem);
                            }
                        };
                        
                        scope.toggleTaxonomySelection = function (taxonomyName) {
                            // is currently selected
                            if (taxonomyName in scope.selectedTaxonomies) {
                                delete scope.selectedTaxonomies[taxonomyName];

                                var groupToRemove = scope.queryData.getItemByName(taxonomyName);

                                if (groupToRemove)
                                    scope.queryData.removeGroup(groupToRemove);
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