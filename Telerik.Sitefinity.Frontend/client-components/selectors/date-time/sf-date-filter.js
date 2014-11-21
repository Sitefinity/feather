(function ($) {
    angular.module('sfSelectors')
        .directive('sfDateFilter', function () {

            var timeSpanItem = function () {
                this.periodType = 'anyTime';
                this.fromDate = null;
                this.toDate = null;
                this.timeSpanValue = 1;
                this.timeSpanInterval = 'weeks';
                this.displayText = '';
            };

            return {
                restrict: 'EA',
                scope: {
                    queryData: '=',
                    groupLogicalOperator: '@',
                    itemLogicalOperator: '@',
                    queryFieldName: '@'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.templateUrl || 'client-components/selectors/date-time/sf-date-filter.html';
                    return sitefinity.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        // ------------------------------------------------------------------------
                        // helper methods
                        // ------------------------------------------------------------------------
                        var constructDateFilterExpressionValue = function (timeSpanValue, timeSpanInterval) {
                            var value;
                            if (timeSpanInterval == 'days')
                                value = 'DateTime.UtcNow.AddDays(-' + timeSpanValue.toFixed(1) + ')';
                            else if (timeSpanInterval == 'weeks')
                                value = 'DateTime.UtcNow.AddDays(-' + (timeSpanValue * 7).toFixed(1) + ')';
                            else if (timeSpanInterval == 'months')
                                value = 'DateTime.UtcNow.AddMonths(-' + timeSpanValue + ')';
                            else if (timeSpanInterval == 'years')
                                value = 'DateTime.UtcNow.AddYears(-' + timeSpanValue + ')';

                            return value;
                        };

                        var translateDateFilterToTimeSpanItem = function (filterValue, timeSpanItem) {
                            var spanValue = filterValue.match(/\(([^)]+)\)/)[1];
                            spanValue = -parseInt(spanValue);
                            timeSpanItem.timeSpanValue = spanValue;

                            if (filterValue.indexOf('AddDays') > 0) {
                                var weeksCount = Math.floor(spanValue / 7);
                                var rem = spanValue % 7;
                                if (rem === 0 && weeksCount !== 0) {
                                    timeSpanItem.timeSpanInterval = 'weeks';
                                    timeSpanItem.timeSpanValue = weeksCount;
                                }
                                else {
                                    timeSpanItem.timeSpanInterval = 'days';
                                }
                            }
                            else if (filterValue.indexOf('AddMonths') > 0) {
                                timeSpanItem.timeSpanInterval = 'months';
                            }
                            else if (filterValue.indexOf('AddYears') > 0){
                                timeSpanItem.timeSpanInterval = 'years';
                            }
                        };

                        var translateQueryItems = function (collection) {
                            var result = new timeSpanItem();

                            if (!collection || collection.length === 0 || collection.length > 2)
                                return result;
                            
                            for (var i = 0; i < collection.length; i++) {
                                var item = collection[i];
                                var operator = item.Condition.Operator;
                                if (operator == '>') {
                                    if (item.Value.indexOf('DateTime.UtcNow') == -1) {
                                        result.fromDate = new Date(item.Value);
                                        result.periodType = "customRange";
                                    }
                                    else {
                                        translateDateFilterToTimeSpanItem(item.Value, result);
                                        result.periodType = "periodToNow";
                                    }
                                }
                                else if (operator == '<') {
                                    result.toDate = new Date(item.Value);
                                }
                            }

                            return result;
                        };

                        var addChildDateQueryItem = function (dateItem, groupName) {
                            var groupItem = scope.queryData.getItemByName(groupName);

                            if (!groupItem)
                                groupItem = scope.queryData.addGroup(groupName, scope.groupLogicalOperator);

                            if (dateItem.periodType == 'periodToNow') {
                                var queryValue = constructDateFilterExpressionValue(dateItem.timeSpanValue, dateItem.timeSpanInterval);
                                var queryName = scope.queryFieldName+ '.' + queryValue;
                                scope.queryData.addChildToGroup(groupItem, queryName, scope.itemLogicalOperator, groupName, 'System.DateTime', '>', queryValue);
                            }
                            else if (dateItem.periodType == 'customRange') {
                                if (dateItem.fromDate) {
                                    var fromQueryValue = dateItem.fromDate.toUTCString();
                                    var fromQueryName = scope.queryFieldName + '.' + fromQueryValue;
                                    scope.queryData.addChildToGroup(groupItem, fromQueryName, scope.itemLogicalOperator, groupName, 'System.DateTime', '>', fromQueryValue);
                                }
                                if (dateItem.toDate) {
                                    var toQueryValue = dateItem.toDate.toUTCString();
                                    var toQueryName = scope.queryFieldName + '.' + toQueryValue;
                                    scope.queryData.addChildToGroup(groupItem, toQueryName, scope.itemLogicalOperator, groupName, 'System.DateTime', '<', toQueryValue);
                                }
                            }
                        };

                        var constructFilterItem = function (selectedDateFilterKey) {
                            var selectedDateQueryItems = scope.queryData.QueryItems.filter(function (f) {
                                return f.Condition &&
                                    f.Condition.FieldName == selectedDateFilterKey &&
                                    f.Condition.FieldType == 'System.DateTime';
                            });
                            scope.selectedDateFilters[selectedDateFilterKey] = translateQueryItems(selectedDateQueryItems);
                        };

                        var populateSelectedDateFilters = function () {
                            if (!scope.selectedDateFilters) {
                                scope.selectedDateFilters = [];
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
                            var newSelectedDateItem = changeArgs.newSelectedItem;
                            
                            var groupToRemove = scope.queryData.getItemByName(scope.queryFieldName);

                            if (groupToRemove)
                                scope.queryData.removeGroup(groupToRemove);

                            if (newSelectedDateItem.periodType != "anyTime")
                                addChildDateQueryItem(newSelectedDateItem, scope.queryFieldName);
                        };
                        
                        scope.toggleDateSelection = function (filterName) {
                            // is currently selected
                            if (filterName in scope.selectedDateFilters) {
                                delete scope.selectedDateFilters[filterName];

                                var groupToRemove = scope.queryData.getItemByName(filterName);

                                if (groupToRemove)
                                    scope.queryData.removeGroup(groupToRemove);
                            }

                            // is newly selected
                            else {
                                constructFilterItem(filterName);
                            }
                        };

                        populateSelectedDateFilters();
                    }
                }
            };
        });
})(jQuery);