(function ($) {
    angular.module('sfSelectors')
        .directive('sfTaxonFilter', function () {
            return {
                restrict: 'EA',
                scope: {
                    sfTaxonomyFields: '=',
                    sfQueryData: '=',
                    sfGroupLogicalOperator: '@',
                    sfItemLogicalOperator: '@',
                    sfProvider: '=?'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/taxonomies/sf-taxon-filter.sf-cshtml';
                    return sitefinity.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    post: function (scope, element, attrs, ctrl) {
                        // ------------------------------------------------------------------------
                        // helper methods
                        // ------------------------------------------------------------------------

                        var getTaxonomyName = function (taxonItem) {
                            var taxonField = scope.sfTaxonomyFields.filter(function (item) {
                                if (taxonItem.RootTaxonomyId) {
                                    return item.Id === taxonItem.RootTaxonomyId;
                                }
                                else {
                                    return item.Id === taxonItem.TaxonomyId;
                                }
                            });

                            if (taxonField.length > 0) {
                                taxonField = taxonField[0];

                                return taxonField.Name;
                            }
                            
                            throw "No item found with this TaxonomyName: " + taxonItem.TaxonomyName;
                        };

                        var addChildTaxonQueryItem = function (taxonItem, groupName) {
                            groupName = groupName || getTaxonomyName(taxonItem);
                            var groupItem = scope.sfQueryData.getItemByName(groupName);

                            if (!groupItem) {
                                groupItem = scope.sfQueryData.addGroup(groupName, scope.sfGroupLogicalOperator);
                            }

                            scope.sfQueryData.addChildToGroup(groupItem, taxonItem.Name, scope.sfItemLogicalOperator, groupName, 'System.Guid', 'Contains', taxonItem.Id);
                        };

                        var constructFilterItem = function (selectedTaxonomyFilterKey) {
                            var selectedTaxonQueryItems = scope.sfQueryData.QueryItems.filter(function (f) {
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

                            if (scope.sfQueryData.QueryItems) {
                                scope.sfQueryData.QueryItems.forEach(function (queryItem) {
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
                                    var groupToRemove = scope.sfQueryData.getItemByName(getTaxonomyName(item));

                                    if (groupToRemove)
                                        scope.sfQueryData.removeGroup(groupToRemove);
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

                                var groupToRemove = scope.sfQueryData.getItemByName(taxonomyName);

                                if (groupToRemove) {
                                    scope.groupItemsMap[taxonomyName] = scope.sfQueryData.getDirectChildren(groupToRemove);

                                    scope.sfQueryData.removeGroup(groupToRemove);
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
                                        }, taxonomyName);
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