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

                        var getTaxonomyName = function (taxonItem) {
                            if (taxonItem.TaxonomyId === sitefinity.getCategoriesTaxonomyId()) {

                                ////NOTE: We should return 'Category' instead of 'Categories' when taxon is of category type. 
                                ////This will result in correct query filtering for a category taxons -> Handling old bug in Sitefinity.
                                return "Category";
                            }
                            return taxonItem.TaxonomyName;
                        };

                        var addChildTaxonQueryItem = function (taxonItem) {
                            var groupName = getTaxonomyName(taxonItem);
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

                            if (selectedTaxonQueryItems.length > 0) {
                                scope.selectedTaxonomies[selectedTaxonomyFilterKey] = [];
                                Array.prototype.push.apply(scope.selectedTaxonomies[selectedTaxonomyFilterKey], selectedTaxonQueryItems.map(function (item) {
                                    return item.Value;
                                }));
                            }
                            else {
                                scope.selectedTaxonomies[selectedTaxonomyFilterKey] = [];
                            }
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
                            var newSelectedTaxonItems = changeArgs.newSelectedItems;
                            var oldSelectedTaxonItems = changeArgs.oldSelectedItems;

                            if (oldSelectedTaxonItems && oldSelectedTaxonItems.length > 0) {
                                oldSelectedTaxonItems.forEach(function (item) {
                                    var groupToRemove = scope.queryData.getItemByName(getTaxonomyName(item));

                                    if (groupToRemove)
                                        scope.queryData.removeGroup(groupToRemove);
                                });
                            }

                            if (newSelectedTaxonItems && newSelectedTaxonItems.length > 0) {
                                newSelectedTaxonItems.forEach(function (item) {
                                    addChildTaxonQueryItem(item);
                                });
                            }
                        };

                        scope.groupItemsMap = [];

                        scope.toggleTaxonomySelection = function (taxonomyName) {
                            // is currently selected
                            if (taxonomyName in scope.selectedTaxonomies) {

                                delete scope.selectedTaxonomies[taxonomyName];

                                var groupToRemove = scope.queryData.getItemByName(taxonomyName);

                                if (groupToRemove) {
                                    scope.groupItemsMap[taxonomyName] = scope.queryData.getDirectChildren(groupToRemove);

                                    scope.queryData.removeGroup(groupToRemove);
                                }
                            }
                            else {
                                // is newly selected
                                if (scope.groupItemsMap && scope.groupItemsMap[taxonomyName]) {
                                    var items = scope.groupItemsMap[taxonomyName];
                                    items.forEach(function (i) {
                                        addChildTaxonQueryItem({
                                            TaxonomyName: taxonomyName,
                                            Name: i.Name,
                                            Id: i.Value
                                        });
                                    });
                                }
                                constructFilterItem(taxonomyName);
                            }
                        };

                        populateSelectedTaxonomies();
                    }
                }
            };
        });
})(jQuery);