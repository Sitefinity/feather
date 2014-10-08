(function ($) {
    angular.module('selectors')
        .directive('dateFilter', function () {

            var timeSpanItem = function () {
                this.periodType = "anyTime";
                this.fromDate = null;
                this.toDate = null;
                this.timeSpanMeasure = null;
                this.timeSpanInterval = "days";
                this.formattedText = "";
            }

            return {
                restrict: 'EA',
                scope: {
                    additionalFilters: '=',
                    groupLogicalOperator: '@',
                    itemLogicalOperator: '@',
                    queryFieldName: '@'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.templateUrl || 'Selectors/date-filter.html';
                    return sitefinity.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    post: function (scope, element, attrs, ctrl) {
                        // ------------------------------------------------------------------------
                        // helper methods
                        // ------------------------------------------------------------------------
                        var constructDateFilterExpressionValue = function (timeSpanMeasure, timeSpanInterval) {
                            var value;
                            if (timeSpanInterval == 'days')
                                value = 'DateTime.UtcNow.AddDays(-' + timeSpanMeasure.toFixed(1) + ')';
                            else if (timeSpanInterval == 'weeks')
                                value = 'DateTime.UtcNow.AddDays(-' + (timeSpanMeasure * 7).toFixed(1) + ')';
                            else if (timeSpanInterval == 'months')
                                value = 'DateTime.UtcNow.AddMonths(-' + timeSpanMeasure + ')';
                            else if (timeSpanInterval == 'years')
                                value = 'DateTime.UtcNow.AddYears(-' + timeSpanMeasure + ')';

                            return value;
                        };

                        var translateDateFilterToTimeSpanItem = function (filterValue, timeSpanItem) {
                            var measure = filterValue.match(/\(([^)]+)\)/)[1];
                            timeSpanItem.timeSpanMeasure = - parseInt(measure);

                            if (filterValue.indexOf('AddDays') > 0) {
                                var weeksCount = Math.floor(measure / 7);
                                var rem = measure % 7;
                                if (rem == 0 && weeksCount != 0) {
                                    timeSpanItem.timeSpanInterval = 'weeks';
                                    timeSpanItem.timeSpanMeasure = weeksCount;
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

                            if (!collection || collection.length == 0 || collection.length > 2)
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
                            var groupItem = scope.additionalFilters.getItemByName(groupName);

                            if(!groupItem)
                                groupItem = scope.additionalFilters.addGroup(groupName, scope.groupLogicalOperator);

                            if (dateItem.periodType == 'periodToNow') {
                                var queryValue = constructDateFilterExpressionValue(dateItem.timeSpanMeasure, dateItem.timeSpanInterval);
                                var queryName = scope.queryFieldName+ '.' + queryValue;
                                scope.additionalFilters.addChildToGroup(groupItem, queryName, scope.itemLogicalOperator, groupName, 'System.DateTime', '>', queryValue);
                            }
                            else if (dateItem.periodType == 'customRange') {
                                if (dateItem.fromDate) {
                                    var fromQueryValue = dateItem.fromDate.toUTCString();
                                    var fromQueryName = scope.queryFieldName + '.' + queryValue;
                                    scope.additionalFilters.addChildToGroup(groupItem, fromQueryName, scope.itemLogicalOperator, groupName, 'System.DateTime', '>', fromQueryValue);
                                }
                                if (dateItem.toDate) {
                                    var toQueryValue = dateItem.toDate.toUTCString();
                                    var toQueryName = scope.queryFieldName + '.' + queryValue;
                                    scope.additionalFilters.addChildToGroup(groupItem, toQueryName, scope.itemLogicalOperator, groupName, 'System.DateTime', '<', toQueryValue);
                                }
                            }
                        };

                        var constructFilterItem = function (selectedDateFilterKey) {
                            var selectedDateQueryItems = scope.additionalFilters.QueryItems.filter(function (f) {
                                return f.Condition && f.Condition.FieldName == selectedDateFilterKey
                                    && f.Condition.FieldType == 'System.DateTime';
                            })
                            scope.selectedDateFilters[selectedDateFilterKey] = translateQueryItems(selectedDateQueryItems);
                        };

                        var populateSelectedDateFilters = function () {
                            if (!scope.selectedDateFilters) {
                                scope.selectedDateFilters = [];
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
                            var newSelectedDateItem = itemSelectedArgs.newSelectedItem;
                            
                            var groupToRemove = scope.additionalFilters.getItemByName(scope.queryFieldName);

                            if (groupToRemove)
                                scope.additionalFilters.removeGroup(groupToRemove);

                            addChildDateQueryItem(newSelectedDateItem, scope.queryFieldName);
                        };
                        
                        scope.toggleDateSelection = function (filterName) {
                            // is currently selected
                            if (filterName in scope.selectedDateFilters) {
                                delete scope.selectedDateFilters[filterName];

                                var groupToRemove = scope.additionalFilters.getItemByName(filterName);

                                if (groupToRemove)
                                    scope.additionalFilters.removeGroup(groupToRemove);
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