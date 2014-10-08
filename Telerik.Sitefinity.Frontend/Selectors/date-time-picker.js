(function ($) {
    angular.module('selectors')
        .directive('dateTimePicker', ['$timeout', function ($timeout) {

            return {
                restrict: "E",
                transclude: true,
                scope: {
                    ngModel: '=?'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.templateUrl || 'Selectors/date-time-picker.html';
                    return sitefinity.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    pre: function (scope, element, attrs, ctrl, transclude) {
                       
                        scope.openDatePicker = function ($event) {
                            $event.preventDefault();
                            $event.stopPropagation();

                            scope.isOpen = true;
                        };

                        scope.updateHours = function (hstep) {
                            if (!scope.ngModel)
                                scope.ngModel = new Date();

                            scope.ngModel.setHours(hstep);
                        };

                        scope.updateMinutes = function (mstep) {
                            if (!scope.ngModel)
                                scope.ngModel = new Date();

                            scope.ngModel.setMinutes(mstep);
                        };

                        scope.hsteps = [];
                        scope.msteps = [];

                        for (var h = 0; h < 24; h++)
                            scope.hsteps.push(h);

                        for (var m = 0; m < 60; m += 10)
                            scope.msteps.push(m)
                        
                        if (scope.ngModel) {
                            scope.hstep = scope.ngModel.getHours();
                            scope.mstep = scope.ngModel.getMinutes();
                        }
                    }
                }
            };
        }]);
})(jQuery);