(function ($) {
    var sfFields = angular.module('sfFields');
    sfFields.requires.push('sfCodeArea');
    var module = angular.module('sfCodeArea', ['sfServices']);

    module.directive('sfCodeArea', ['serverContext', function (serverContext) {
        return {
            restrict: 'A',
            scope: {
                sfType: '@',
                sfLineNumbers: '@',
                sfMatchBrackets: '@',
                sfTabMode: '@'
            },
            link: function (scope, element, attrs) {
                CodeMirror.fromTextArea(element[0], {
                    mode: scope.sfType,
                    lineNumbers: scope.sfLineNumbers,
                    matchBrackets: scope.sfMatchBrackets,
                    tabMode: scope.sfTabMode
                });
            }
        };
    }]);
})(jQuery);
