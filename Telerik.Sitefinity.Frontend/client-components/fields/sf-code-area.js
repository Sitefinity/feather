(function ($) {
    var sfFields = angular.module('sfFields');
    sfFields.requires.push('sfCodeArea');
    var module = angular.module('sfCodeArea', ['sfServices']);

    module.directive('sfCodeArea', ['serverContext', function (serverContext) {
        return {
            restrict: 'A',
            scope: {
                sfModel: '='
            },
            link: function (scope, element, attrs) {
                var codeArea = CodeMirror.fromTextArea(element[0], {
                    mode: attrs.sfType,
                    lineNumbers: attrs.sfLineNumbers,
                    matchBrackets: attrs.sfMatchBrackets,
                    tabMode: attrs.sfTabMode,
                    onChange: function (instance, changeObj) {
                        var value = instance.getValue();
                        if (scope.sfModel !== value) {
                            scope.sfModel = value;
                            scope.$apply();
                        }
                    }
                });

                // forces a refresh after current $digest is finished.
                setTimeout(codeArea.refresh, 0);

                scope.$watch('sfModel', function (newVal, oldVal) {
                    if (codeArea.getValue() !== newVal) {
                        // Prevent error in the codeArea if the new value is undefined.
                        if (!newVal) newVal = '';

                        // forces a refresh after current $digest is finished.
                        setTimeout(function () {
                            codeArea.setValue(newVal);
                            codeArea.refresh();
                            codeArea.focus();
                        }, 0);
                    }
                });
            }
        };
    }]);
})(jQuery);
