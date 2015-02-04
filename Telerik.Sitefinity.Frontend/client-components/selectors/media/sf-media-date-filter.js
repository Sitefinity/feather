; (function ($) {
    angular.module('sfMediaDateFilter', ['sfServices', 'sfCollection'])
        .directive('sfMediaDateFilter', ['serverContext', 'sfMediaService', function (serverContext, sfMediaService) {
            var getDate = function (daysToSubstract, monthsToSubstract, yearsToSubstract) {
                var now = new Date();

                if (daysToSubstract) {
                    now.setDate(now.getDate() - daysToSubstract);
                }

                if (monthsToSubstract) {
                    now.setMonth(now.getMonth() - monthsToSubstract);
                }

                if (yearsToSubstract) {
                    now.setYear(now.getYear() - yearsToSubstract);
                }

                return now;
            };

            var constants = {
                anyTimeDate: { text: 'Any time', dateValue: 'AnyTime' },
                defaultDates: [
                    { text: 'Last 1 day', dateValue: getDate(1) },
                    { text: 'Last 3 days', dateValue: getDate(3) },
                    { text: 'Last 1 week', dateValue: getDate(7) },
                    { text: 'Last 1 month', dateValue: getDate(0, 1) },
                    { text: 'Last 6 months', dateValue: getDate(0, 6) },
                    { text: 'Last 1 year', dateValue: getDate(0, 0, 1) },
                    { text: 'Last 2 years', dateValue: getDate(0, 0, 2) },
                    { text: 'Last 5 years', dateValue: getDate(0, 0, 5) }
                ]
            };
            return {
                restrict: 'AE',
                scope: {
                    filterObject: '=sfModel',
                    sfDates: '=',
                    sfShowAnyTime: '@'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/media/sf-media-date-filter.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    if (scope.filterObject && sfMediaService.newFilter().constructor.prototype !== scope.filterObject.constructor.prototype) {
                        throw { Message: 'sf-model must be of type MediaFilter.' };
                    }

                    // Unable to access scope properties bound by @
                    scope.sfShowAnyTime = attrs.sfShowAnyTime;

                    scope.dates = scope.sfDates || constants.defaultDates;

                    if (scope.sfShowAnyTime !== undefined && scope.sfShowAnyTime.toLowerCase() !== 'false') {
                        scope.dates.unshift(constants.anyTimeDate);
                    }

                    scope.selectedDate = [];

                    scope.$watch('selectedDate', function (newVal, oldVal) {
                        // removes all taxons, so only the parent is set.
                        var filter = sfMediaService.newFilter();

                        // if deselected (undefined) the value must remain the original null
                        if (newVal !== oldVal && newVal[0] !== undefined) {
                            // sf collection always binds to array of items.
                            filter.date = newVal[0];
                        }

                        // media selector watches this and reacts to changes.
                        scope.filterObject = filter;
                    }, true);
                }
            };
        }]);
})(jQuery);