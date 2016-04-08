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
                    sfQueryFieldName: '@',
                    sfGroupName: '@',
                    sfFilterLabel: '@',
                    sfIsUpcomingPeriod: '=?',
                    sfCustomRangeMinDate: '=?',
                    sfCustomRangeMaxDate: '=?'
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
                        var constructDateFilterExpressionValue = function (timeSpanValue, timeSpanInterval, periodType) {
                            var value;
                            var periodTypeSign = periodType === 'periodToNow' ? '-' : '';
                            if (timeSpanInterval == 'days')
                                value = 'DateTime.UtcNow.AddDays(' + periodTypeSign + timeSpanValue.toFixed(1) + ')';
                            else if (timeSpanInterval == 'weeks')
                                value = 'DateTime.UtcNow.AddDays(' + periodTypeSign + (timeSpanValue * 7).toFixed(1) + ')';
                            else if (timeSpanInterval == 'months')
                                value = 'DateTime.UtcNow.AddMonths(' + periodTypeSign + timeSpanValue + ')';
                            else if (timeSpanInterval == 'years')
                                value = 'DateTime.UtcNow.AddYears(' + periodTypeSign + timeSpanValue + ')';

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
                                    if (item.Value.indexOf('DateTime.UtcNow') == -1) {
                                        result.toDate = new Date(item.Value);
                                        result.periodType = "customRange";
                                    }
                                    else {
                                        translateDateFilterToTimeSpanItem(item.Value, result);
                                        result.periodType = "periodFromNow";
                                    }
                                }
                            }

                            return result;
                        };

                        var addChildDateQueryItem = function (dateItem, groupName) {
                            var groupItem = scope.sfQueryData.getItemByName(groupName);

                            if (!groupItem)
                                groupItem = scope.sfQueryData.addGroup(groupName, scope.sfGroupLogicalOperator);

                            if (dateItem.periodType == 'periodToNow' || dateItem.periodType == 'periodFromNow') {
                                var queryValue = constructDateFilterExpressionValue(dateItem.timeSpanValue, dateItem.timeSpanInterval, dateItem.periodType);
                                var queryName = scope.sfQueryFieldName + '.' + queryValue;
                                var operator = (dateItem.periodType == 'periodToNow') ? '>' : '<';
                                scope.sfQueryData.addChildToGroup(groupItem, queryName, scope.sfItemLogicalOperator, scope.sfQueryFieldName, 'System.DateTime', operator, queryValue);
                            }
                            else if (dateItem.periodType == 'customRange') {
                                if (dateItem.fromDate) {
                                    var fromQueryValue = dateItem.fromDate.toUTCString();
                                    var fromQueryName = scope.sfQueryFieldName + '.' + fromQueryValue;
                                    scope.sfQueryData.addChildToGroup(groupItem, fromQueryName, scope.sfItemLogicalOperator, scope.sfQueryFieldName, 'System.DateTime', '>', fromQueryValue);
                                }
                                if (dateItem.toDate) {
                                    var toQueryValue = dateItem.toDate.toUTCString();
                                    var toQueryName = scope.sfQueryFieldName + '.' + toQueryValue;
                                    scope.sfQueryData.addChildToGroup(groupItem, toQueryName, scope.sfItemLogicalOperator, scope.sfQueryFieldName, 'System.DateTime', '<', toQueryValue);
                                }
                            }
                        };

                        var constructFilterItem = function (selectedGroupFilterKey) {
                            var selectedDateQueryItems = scope.sfQueryData.QueryItems.filter(function (f) {
                                return f.Condition &&
                                    f.Condition.FieldName == scope.sfQueryFieldName &&
                                    f.Condition.FieldType == 'System.DateTime';
                            });
                            scope.selectedDateFilters[selectedGroupFilterKey] = translateQueryItems(selectedDateQueryItems);
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
                        scope.sfGroupName = scope.sfGroupName ? scope.sfGroupName : scope.sfQueryFieldName;

                        scope.change = function (changeArgs) {
                            var newSelectedDateItem = changeArgs.newSelectedItem;
                            
                            var groupToRemove = scope.sfQueryData.getItemByName(scope.sfGroupName);

                            if (groupToRemove)
                                scope.sfQueryData.removeGroup(groupToRemove);

                            if (newSelectedDateItem.periodType != "anyTime")
                                addChildDateQueryItem(newSelectedDateItem, scope.sfGroupName);
                        };
                        
                        scope.toggleDateSelection = function (groupFilterName) {
                            // is currently selected
                            if (groupFilterName in scope.selectedDateFilters) {
                                delete scope.selectedDateFilters[groupFilterName];

                                var groupToRemove = scope.sfQueryData.getItemByName(groupFilterName);

                                if (groupToRemove)
                                    scope.sfQueryData.removeGroup(groupToRemove);
                            }

                            // is newly selected
                            else {
                                constructFilterItem(groupFilterName);
                            }
                        };

                        populateSelectedDateFilters();
                    }
                }
            };
        });
})(jQuery);