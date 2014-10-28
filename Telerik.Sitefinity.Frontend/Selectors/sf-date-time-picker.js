(function ($) {
    angular.module('sfSelectors')
        .directive('sfDateTimePicker', ['$timeout', function ($timeout) {

            return {
                restrict: 'E',
                transclude: true,
                scope: {
                    ngModel: '=?',
                    showMeridian: '@?',
                    hourStep: '@?',
                    minueteStep: '@?'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.templateUrl || 'Selectors/sf-date-time-picker.html';
                    return sitefinity.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    pre: function (scope, element, attrs, ctrl, transclude) {

                        var transformHours = function () {
                        };
                       
                        scope.openDatePicker = function ($event) {
                            $event.preventDefault();
                            $event.stopPropagation();

                            scope.isOpen = true;
                        };

                        scope.updateHours = function (hstep) {
                            if (hstep == 'none') {
                                scope.ngModel.setHours(0);
                                scope.showHours = false;
                                scope.showMinutes = false;
                            } else {
                                if (!scope.ngModel)
                                    scope.ngModel = new Date();

                                scope.ngModel.setHours(hstep);
                                scope.addMinutesClick();
                            }
                        };

                        scope.updateMinutes = function (mstep) {
                            if (mstep == 'none') {
                                scope.ngModel.setMinutes(0);
                                scope.showMinutes = false;
                            } else {
                                if (!scope.ngModel)
                                    scope.ngModel = new Date();

                                scope.ngModel.setMinutes(mstep);
                            }
                        };

                        scope.hsteps = [];
                        scope.msteps = [];
                        
                        if (!scope.hourStep)
                            scope.hourStep = 1;

                        if (!scope.minueteStep)
                            scope.minueteStep = 10;

                        var h;
                        if (scope.showMeridian) {
                            scope.hsteps.push({ 'label': '- Hour -', 'value': 'none' });

                            for (h = 0; h < 24; h += scope.hourStep) {
                                var hour = (h < 12) ? h : h - 12;

                                // display 0 hour as 12AM / 12PM
                                hour = (hour !== 0) ? hour : 12;

                                var meridian = (h < 12) ? 'AM' : 'PM';

                                scope.hsteps.push({ 'label': hour + ' ' + meridian, 'value': h });
                            }
                        }
                        else{
                            for (h = 0; h < 24; h += scope.hourStep) {
                                scope.hsteps.push({ 'label': h, 'value': h });
                            }
                        }

                        scope.msteps.push({ 'label': '- Minute -', 'value': 'none' });

                        scope.addHoursClick = function () {
                            scope.showHours = true;
                            scope.hstep = 0;
                        };


                        scope.addMinutesClick = function () {
                            scope.showMinutes = true;
                            scope.mstep = 0;
                        };

                        for (var m = 0; m < 60; m += scope.minueteStep) {
                            scope.msteps.push({ 'label': m, 'value': m });
                        }
                        
                        if (scope.ngModel) {
                            scope.hstep = scope.ngModel.getHours();
                            scope.mstep = scope.ngModel.getMinutes();
                        }
                    }
                }
            };
        }]);
})(jQuery);