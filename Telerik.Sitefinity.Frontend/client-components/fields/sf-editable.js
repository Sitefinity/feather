(function ($) {
    var sfFields = angular.module('sfFields');
    sfFields.requires.push('sfEditable');
    var module = angular.module('sfEditable', ['kendo.directives', 'sfServices']);

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
