(function ($) {
    angular.module('sfSelectors')
        .directive('sfCalendarFilter', ['serverContext', function (serverContext) {

            return {
                restrict: 'EA',
                scope: {
                    sfQueryData: '=',
                    sfGroupLogicalOperator: '@',
                    sfItemLogicalOperator: '@',
                    sfQueryFieldName: '@',
                    sfProvider: '=?'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/events/sf-calendar-filter.sf-cshtml';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        // ------------------------------------------------------------------------
                        // helper methods
                        // ------------------------------------------------------------------------

                        var constructFilterItem = function (selectedCalendarFilterKey) {
                            var selectedCalendarQueryItems = scope.sfQueryData.QueryItems.filter(function (f) {
                                return f.Condition &&
                                    f.Condition.FieldName == 'Parent.Id.ToString()' &&
                                    f.Condition.FieldType == 'System.String';
                            });

                            if (selectedCalendarQueryItems.length > 0) {
                                scope.selectedCalendars[selectedCalendarFilterKey] = [];
                                Array.prototype.push.apply(scope.selectedCalendars[selectedCalendarFilterKey], selectedCalendarQueryItems.map(function (item) {
                                    return item.Value;
                                }));
                            }
                            else {
                                scope.selectedCalendars[selectedCalendarFilterKey] = [];
                            }
                        };

                        var addChildCalendarQueryItem = function (calendarItem) {
                            var groupItem = scope.sfQueryData.getItemByName(scope.sfQueryFieldName);

                            if (!groupItem) {
                                groupItem = scope.sfQueryData.addGroup(scope.sfQueryFieldName, scope.sfGroupLogicalOperator);
                            }

                            scope.sfQueryData.addChildToGroup(groupItem, 'Parent.Id', scope.sfItemLogicalOperator, 'Parent.Id.ToString()', 'System.String', 'Contains', calendarItem.Id);
                        };

                        var populateSelectedCalendars = function () {
                            if (!scope.selectedCalendars) {
                                scope.selectedCalendars = [];
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
                            var newSelectedCalendarItems = changeArgs.newSelectedItems;
                            var oldSelectedCalendarItems = changeArgs.oldSelectedItems;

                            if (oldSelectedCalendarItems && oldSelectedCalendarItems.length > 0) {
                                oldSelectedCalendarItems.forEach(function (item) {
                                    var groupToRemove = scope.sfQueryData.getItemByName(scope.sfQueryFieldName);

                                    if (groupToRemove)
                                        scope.sfQueryData.removeGroup(groupToRemove);
                                });
                            }

                            if (newSelectedCalendarItems && newSelectedCalendarItems.length > 0) {
                                newSelectedCalendarItems.forEach(function (item) {
                                    addChildCalendarQueryItem(item);
                                });
                            }
                        };

                        scope.toggleCalendarSelection = function (filterName) {
                            // is currently selected
                            if (filterName in scope.selectedCalendars) {
                                delete scope.selectedCalendars[filterName];

                                var groupToRemove = scope.sfQueryData.getItemByName(filterName);

                                if (groupToRemove)
                                    scope.sfQueryData.removeGroup(groupToRemove);
                            }

                                // is newly selected
                            else {
                                constructFilterItem(filterName);
                            }
                        };

                        populateSelectedCalendars();
                    }
                }
            };
        }]);
})(jQuery);