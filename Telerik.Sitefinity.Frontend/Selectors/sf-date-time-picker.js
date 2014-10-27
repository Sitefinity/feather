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
                            if (!scope.ngModel)
                                scope.ngModel = new Date();

                            scope.ngModel.setHours(hstep);

                            scope.showMinutes = true;
                        };

                        scope.updateMinutes = function (mstep) {
                            if (!scope.ngModel)
                                scope.ngModel = new Date();

                            scope.ngModel.setMinutes(mstep);
                        };

                        scope.hsteps = [];
                        scope.msteps = [];
                        
                        if (!scope.hourStep)
                            scope.hourStep = 1;

                        if (!scope.minueteStep)
                            scope.minueteStep = 10;

                        var h;
                        if (scope.showMeridian) {
                            for (h = 0; h < 24; h += scope.hourStep) {
                                var hour = (h < 12) ? h : h - 12;
                                var meridian = (h < 12) ? 'AM' : 'PM';
                                scope.hsteps.push({ 'label': hour + ' ' + meridian, 'value': h });
                            }
                        }
                        else{
                            for (h = 0; h < 24; h += scope.hourStep) {
                                scope.hsteps.push({ 'label': h, 'value': h });
                            }
                        }

                        for (var m = 0; m < 60; m += scope.minueteStep)
                            scope.msteps.push(m);
                        
                        if (scope.ngModel) {
                            scope.hstep = scope.ngModel.getHours();
                            scope.mstep = scope.ngModel.getMinutes();
                        }
                    }
                }
            };
        }]);
})(jQuery);