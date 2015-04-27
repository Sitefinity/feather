(function ($) {
    var sfFields = angular.module('sfFields');
    sfFields.requires.push('sfCodeArea');
    var module = angular.module('sfCodeArea', []);

    module.directive('sfCodeArea', function () {
        return {
            restrict: 'E',
            scope: {
                sfType: '@',
                sfLineNumbers: '@',
                sfMatchBrackets: '@',
                sfTabMode: '@'
            },
            link: function (scope, element, attrs) {
                CodeMirror.fromTextArea(element, {
                    mode: scope.sfType,
                    lineNumbers: scope.sfLineNumbers,
                    matchBrackets: scope.sfMatchBrackets,
                    tabMode: scope.sfTabMode
                });
            }
        };
    });
})(jQuery);
