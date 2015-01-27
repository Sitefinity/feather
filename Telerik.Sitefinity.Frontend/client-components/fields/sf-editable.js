(function ($) {
    var module;
    try {
        module = angular.module('sfFields');
    } catch (e) {
        module = angular.module('sfFields', []);
    }
    

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

                element.bind("blur", function (event) {
                    scope.$apply(function () {
                        scope.$eval(attrs.sfExitEdit);
                    });
                });

                element.bind("focus click", function (event) {
                    scope.$apply(function () {
                        scope.$eval(attrs.sfEnterEdit);
                    });
                });
            }
        };
    });
})(jQuery);