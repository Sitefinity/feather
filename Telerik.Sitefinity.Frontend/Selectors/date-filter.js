(function ($) {
    angular.module('selectors')
        .directive('dateFilter', function () {       

            return {
                restrict: 'EA',
                scope: {
                    additionalFilters: '='
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.templateUrl || 'Selectors/date-filter.html';
                    return sitefinity.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    post: function (scope, element, attrs, ctrl) {

                        var findSiblingsCount = function (groupItem) {
                            var siblingItemsCount = 0;

                            if (groupItem) {
                                siblingItemsCount = scope.additionalFilters.QueryItems.filter(function (f) {
                                    return (f.ItemPath.indexOf(groupItem.ItemPath) == 0
                                        && f.ItemPath.length > groupItem.ItemPath.length
                                        && f.ItemPath.substring(groupItem.ItemPath.length).indexOf(f._itemPathSeparator) < 0);
                                }).length;
                            }

                            return siblingItemsCount;
                        };

                        var findGroupItem = function (groupName) {
                            var groupItem;

                            if (scope.additionalFilters.QueryItems) {
                                groupItem = scope.additionalFilters.QueryItems.filter(function (f) {
                                    return (f.Name === groupName && f.IsGroup);
                                })[0];
                            }

                            return groupItem;
                        };

                        var dateFilteringCondition = function(fieldName, operator){
                            this.FiledName = fieldName;
                            this.Operator = operator;
                            this.FieldType = 'System.DateTime';
                        };

                        var constructDateFilterExpressionValue = function(timeSpanMeasure, timeSpanInterval){
                            var value;
                            if (timeSpanInterval == 'days')
                                value = 'DateTime.UtcNow.AddDays(-' + timeSpanMeasure + ')';
                            else if (timeSpanInterval == 'weeks')
                                value = 'DateTime.UtcNow.AddDays(-' + timeSpanMeasure * 7 + ')';
                            else if (timeSpanInterval == 'months')
                                value = 'DateTime.UtcNow.AddMonths(-' + timeSpanMeasure + ')';
                            else if(timeSpanInterval == 'years')
                                value = 'DateTime.UtcNow.AddYears(-' + timeSpanMeasure + ')';

                            return value;
                        }

                        var addGroup = function (groupName) {
                            if (!scope.additionalFilters.QueryItems ||
                                    scope.additionalFilters.QueryItems.filter(function (f) {
                                        return f.Name === groupName;
                            }).length !== 1) {
                                scope.additionalFilters.addGroupQueryDataItem(groupName, 'AND');
                            }
                        };

                        var addChildDateQueryItem = function (dateItem, groupName) {
                            addGroup(groupName);
                            var groupItem = findGroupItem(groupName);
                            var siblingItemsCount = findSiblingsCount(groupItem);

                            if (dateItem.periodType == 'periodToNow') {
                                var condition = new dateFilteringCondition(groupName, '>');
                                var queryValue = constructDateFilterExpressionValue(dateItem.timeSpanMeasure, dateItem.timeSpanInterval);
                                var queryName = 'Dates.' + queryValue;
                                scope.additionalFilters.addChildQueryDateItem(queryName, 'AND', groupItem, siblingItemsCount, queryValue, condition);
                            }
                            else if (dateItem.periodType == 'customRange') {
                                var fromCondition = new dateFilteringCondition(groupName, '>');
                                var fromQueryValue = dateItem.fromDate.toUTCString();
                                var fromQueryName = 'Dates.' + queryValue;
                                scope.additionalFilters.addChildQueryDateItem(fromQueryName, 'AND', groupItem, siblingItemsCount, fromQueryValue, fromCondition);

                                var toCondition = new dateFilteringCondition(groupName, '<');
                                var toQueryValue = dateItem.toDate.toUTCString();
                                var toQueryName = 'Dates.' + queryValue;
                                scope.additionalFilters.addChildQueryDateItem(toQueryName, 'AND', groupItem, siblingItemsCount + 1, toQueryValue, toCondition);
                            }
                        };

                        var removeGroup = function (groupName) {
                            var groupItem = scope.additionalFilters.QueryItems.filter(function (f) {
                                return f.Name === groupName;
                            })[0];

                            if (groupItem) {
                                scope.additionalFilters.QueryItems = scope.additionalFilters.QueryItems.filter(function (f) {
                                    return f.ItemPath.indexOf(groupItem.ItemPath) !== 0;
                                });
                            }

                            if (!scope.additionalFilters.QueryItems || scope.additionalFilters.QueryItems.length == 0) {
                                scope.additionalFilters = {};
                            }
                        };

                        var populateSelectedDateFilters = function () {
                            if (!scope.selectedDateFilters) {
                                scope.selectedDateFilters = [];
                            }

                            if (scope.additionalFilters.QueryItems) {
                                scope.additionalFilters.QueryItems.forEach(function (queryItem) {
                                    {
                                        if (queryItem.IsGroup)
                                            scope.selectedDateFilters.push(queryItem.Name);
                                    }
                                });
                            }
                        };

                        scope.itemSelected = function (itemSelectedArgs) {
                            var newSelectedDateItem = itemSelectedArgs.newSelectedItem;
                            var oldSelectedDateItem = itemSelectedArgs.oldSelectedItem;
                            
                            removeGroup('PublicationDate');
                            addChildDateQueryItem(newSelectedDateItem, 'PublicationDate');
                        };
                        
                        scope.toggleDateSelection = function (filterName) {
                            if (!scope.additionalFilters.QueryItems)
                                scope.additionalFilters.QueryItems = [];

                            var idx = scope.selectedDateFilters.indexOf(filterName);

                            // is currently selected
                            if (idx > -1) {
                                scope.selectedDateFilters.splice(idx, 1);

                                removeGroup(filterName);
                            }

                            // is newly selected
                            else {
                                scope.selectedDateFilters.push(filterName);
                            }
                        };

                        populateSelectedDateFilters();
                    }
                }
            };
        });
})(jQuery);