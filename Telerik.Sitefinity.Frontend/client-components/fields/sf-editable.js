(function ($) {
    var module = angular.module('sfFields', ['kendo.directives', 'sfServices']);

    module.directive('sfEditable', function () {
        return {
            restrict: 'E',
            link: function (scope, element, attrs) {
                element.bind("keydown keypress", function (event) {
                    if (event.which === 13 || event.which === 27 || event.which === 9) {
                        scope.$apply(function () {
                            scope.$eval(attrs.sfExitEdit);
                        });
                    }
                });
            }
        };
    });
})(jQuery);
