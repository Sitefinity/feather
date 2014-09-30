(function ($) {
    angular.module('selectors')
        .directive('taxonFilter', function () {       

            return {
                restrict: 'EA',
                scope: {
                    taxonomyFields: '=',
                    additionalFilters: '=',
                    provider: '=?'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.templateUrl || 'Selectors/taxon-filter.html';
                    return sitefinity.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    post: function (scope, element, attrs, ctrl) {

                        var taxonFilterCondition = function (taxonomyName) {
                            this.FieldName = taxonomyName;
                            this.FieldType = 'System.Guid';
                            this.Operator = 'Contains';
                        };

                        var findSiblingsCount = function (groupItem) {
                            var siblingItemsCount = scope.additionalFilters.QueryItems.filter(function (f) {
                                return (f.ItemPath.indexOf(groupItem.ItemPath) == 0
                                    && f.ItemPath.length > groupItem.ItemPath.length
                                    && f.ItemPath.substring(groupItem.ItemPath.length).indexOf(f._itemPathSeparator) < 0);
                            }).length;

                            return siblingItemsCount;
                        };

                        var findGroupItem = function (taxonItem) {
                            var groupName = taxonItem.TaxonomyName;
                            var groupItem = scope.additionalFilters.QueryItems.filter(function (f) {
                                return (f.Name === groupName && f.IsGroup);
                            })[0];

                            return groupItem;
                        };

                        var addChildTaxonQueryItem = function (taxonItem) {
                            var groupName = taxonItem.TaxonomyName;
                            var groupItem = findGroupItem(taxonItem);
                            var siblingItemsCount = findSiblingsCount(groupItem);
                            var condition = new taxonFilterCondition(groupName);

                            scope.additionalFilters.addChildQueryDateItem(taxonItem.Name, 'OR', groupItem, siblingItemsCount, taxonItem.Id, condition);
                        };

                        var removeTaxonQueryItem = function (taxonItem) {
                            scope.additionalFilters.QueryItems = scope.additionalFilters.QueryItems.filter(function (f) {
                                return !(f.Name === taxonItem.Name && !f.IsGroup);
                            });

                            var groupItem = findGroupItem(taxonItem);
                            if (findSiblingsCount(groupItem) == 0) {
                                scope.additionalFilters.QueryItems = scope.additionalFilters.QueryItems.filter(function (f) {
                                    return !(f.Name === taxonItem.TaxonomyName && f.IsGroup);
                                });
                            }

                            if (!scope.additionalFilters.QueryItems || scope.additionalFilters.QueryItems.length == 0) {
                                scope.additionalFilters = {};
                            }
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
                            if (newSelectedTaxonItem.Id) {
                                addChildTaxonQueryItem(newSelectedTaxonItem);
                            }
                            else {
                                removeTaxonQueryItem(oldSelectedTaxonItem);
                            }
                        };
                        
                        scope.toggleTaxonomySelection = function (taxonomyName) {
                            if (!scope.additionalFilters.QueryItems)
                                scope.additionalFilters.QueryItems = [];

                            var idx = scope.selectedTaxonomies.indexOf(taxonomyName);

                            // is currently selected
                            if (idx > -1) {
                                scope.selectedTaxonomies.splice(idx, 1);

                                var groupName = scope.additionalFilters.QueryItems.filter(function (f) {
                                    return f.Name === taxonomyName;
                                })[0];

                                scope.additionalFilters.QueryItems = scope.additionalFilters.QueryItems.filter(function (f) {
                                    return f.ItemPath.indexOf(groupName.ItemPath)!==0;
                                });

                                if (!scope.additionalFilters.QueryItems || scope.additionalFilters.QueryItems.length == 0)
                                {
                                    scope.additionalFilters = null;
                                }
                            }

                            // is newly selected
                            else {
                                scope.selectedTaxonomies.push(taxonomyName);
                                
                                if (!scope.additionalFilters.QueryItems ||
                                    scope.additionalFilters.QueryItems.filter(function (f) {
                                        return f.Name === taxonomyName;
                                }).length !== 1)
                                {
                                    scope.additionalFilters.addGroupQueryDataItem(taxonomyName, 'AND');
                                }
                            }
                        };

                        populateSelectedTaxonomies();
                    }
                }
            };
        });
})(jQuery);