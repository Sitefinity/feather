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
                    sfQueryData: '=',
                    sfGroupLogicalOperator: '@',
                    sfItemLogicalOperator: '@',
                    sfQueryFieldName: '@'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/date-time/sf-date-filter.sf-cshtml';
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
                            var groupItem = scope.sfQueryData.getItemByName(groupName);

                            if (!groupItem)
                                groupItem = scope.sfQueryData.addGroup(groupName, scope.sfGroupLogicalOperator);

                            if (dateItem.periodType == 'periodToNow') {
                                var queryValue = constructDateFilterExpressionValue(dateItem.timeSpanValue, dateItem.timeSpanInterval);
                                var queryName = scope.sfQueryFieldName + '.' + queryValue;
                                scope.sfQueryData.addChildToGroup(groupItem, queryName, scope.sfItemLogicalOperator, groupName, 'System.DateTime', '>', queryValue);
                            }
                            else if (dateItem.periodType == 'customRange') {
                                if (dateItem.fromDate) {
                                    var fromQueryValue = dateItem.fromDate.toUTCString();
                                    var fromQueryName = scope.sfQueryFieldName + '.' + fromQueryValue;
                                    scope.sfQueryData.addChildToGroup(groupItem, fromQueryName, scope.sfItemLogicalOperator, groupName, 'System.DateTime', '>', fromQueryValue);
                                }
                                if (dateItem.toDate) {
                                    var toQueryValue = dateItem.toDate.toUTCString();
                                    var toQueryName = scope.sfQueryFieldName + '.' + toQueryValue;
                                    scope.sfQueryData.addChildToGroup(groupItem, toQueryName, scope.sfItemLogicalOperator, groupName, 'System.DateTime', '<', toQueryValue);
                                }
                            }
                        };

                        var constructFilterItem = function (selectedDateFilterKey) {
                            var selectedDateQueryItems = scope.sfQueryData.QueryItems.filter(function (f) {
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
                            var newSelectedDateItem = changeArgs.newSelectedItem;
                            
                            var groupToRemove = scope.sfQueryData.getItemByName(scope.sfQueryFieldName);

                            if (groupToRemove)
                                scope.sfQueryData.removeGroup(groupToRemove);

                            if (newSelectedDateItem.periodType != "anyTime")
                                addChildDateQueryItem(newSelectedDateItem, scope.sfQueryFieldName);
                        };
                        
                        scope.toggleDateSelection = function (filterName) {
                            // is currently selected
                            if (filterName in scope.selectedDateFilters) {
                                delete scope.selectedDateFilters[filterName];

                                var groupToRemove = scope.sfQueryData.getItemByName(filterName);

                                if (groupToRemove)
                                    scope.sfQueryData.removeGroup(groupToRemove);
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