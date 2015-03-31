(function ($) {
    angular.module('sfSelectors')
        .directive('sfDateTimePicker', ['$timeout', function ($timeout) {

            return {
                restrict: 'E',
                transclude: true,
                scope: {
                    ngModel: '=?',
                    sfShowMeridian: '@?',
                    sfHourStep: '@?',
                    sfMinuteStep: '@?'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/date-time/sf-date-time-picker.sf-cshtml';
                    return sitefinity.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    pre: function (scope, element, attrs, ctrl, transclude) {
                        // ------------------------------------------------------------------------
                        // helper methods
                        // ------------------------------------------------------------------------

                        var populateHoursArray = function () {
                            scope.hsteps = [];

                            if (!scope.sfHourStep) {
                                scope.sfHourStep = 1;
                            }
                            else {
                                var hourStep = parseInt(scope.sfHourStep);
                                if (!isNaN(hourStep)) {
                                    scope.sfHourStep = hourStep;
                                }
                                else {
                                    scope.sfHourStep = 1;
                                }
                            }

                            var h;
                            if (!scope.sfShowMeridian || scope.sfShowMeridian.toLowerCase() !== 'true') {
                                for (h = 0; h < 24; h += scope.sfHourStep) {
                                    scope.hsteps.push({ 'label': h, 'value': h });
                                }
                            }
                            else {
                                for (h = 0; h < 24; h += scope.sfHourStep) {
                                    var hour = (h < 12) ? h : h - 12;
                                    var meridian = (h < 12) ? 'AM' : 'PM';

                                    // display 0 hour as 12AM / 12PM
                                    hour = (hour !== 0) ? hour : 12;

                                    scope.hsteps.push({ 'label': hour + ' ' + meridian, 'value': h });
                                }
                            }
                        };

                        var populateMinutesArray = function () {
                            scope.msteps = [];

                            if (!scope.sfMinuteStep) {
                                scope.sfMinuteStep = 10;
                            }
                            else {
                                var minuteStep = parseInt(scope.sfMinuteStep);
                                if (!isNaN(minuteStep)) {
                                    scope.sfMinuteStep = minuteStep;
                                }
                                else {
                                    scope.sfMinuteStep = 10;
                                }
                            }

                            for (var m = 0; m < 60; m += scope.sfMinuteStep) {
                                scope.msteps.push({ 'label': m, 'value': m });
                            }
                        };

                        var setHours = function (value, showMinutesField) {
                            scope.ngModel.setHours(value);
                            scope.hstep = value;
                            scope.showMinutesField = showMinutesField;
                        };

                        var setMinutes = function (value, showMinutesDropdown) {
                            scope.ngModel.setMinutes(value);
                            scope.mstep = value;
                            scope.showMinutesDropdown = showMinutesDropdown;
                        };

                        // ------------------------------------------------------------------------
                        // Scope variables and setup
                        // ------------------------------------------------------------------------

                        scope.openDatePicker = function ($event) {
                            $event.preventDefault();
                            $event.stopPropagation();

                            scope.isOpen = true;
                        };

                        scope.updateHours = function (hstep) {
                            if (!scope.ngModel)
                                scope.ngModel = new Date();

                            if (!hstep && hstep !== 0) {
                                setHours(0, false);
                                setMinutes(0, false);
                            }
                            else {
                                setHours(hstep, true);
                            }
                        };

                        scope.updateMinutes = function (mstep) {
                            if (!scope.ngModel)
                                scope.ngModel = new Date();

                            if (!mstep && mstep !== 0)
                                setMinutes(0, false);
                            else
                                setMinutes(mstep, true);
                        };
                        
                        populateHoursArray();
                        populateMinutesArray();
                        scope.showMinutesDropdown = false;
                        scope.showMinutesField = false;

                        if (scope.ngModel) {
                            scope.hstep = scope.ngModel.getHours();
                            scope.mstep = scope.ngModel.getMinutes();

                            if (scope.hstep !== 0 || scope.mstep !== 0)
                                scope.showMinutesField = true;

                            if (scope.mstep !== 0)
                                scope.showMinutesDropdown = true;
                        }
                    }
                }
            };
        }]);
})(jQuery);