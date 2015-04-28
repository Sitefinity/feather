(function ($) {
    var sfFields = angular.module('sfFields');
    sfFields.requires.push('sfCodeArea');
    var module = angular.module('sfCodeArea', ['sfServices', 'sfBootstrapPopover']);

    module.directive('sfCodeArea', ['serverContext', function (serverContext) {
        return {
            restrict: 'E',
            scope: {
                sfModel: '=',
                sfType: '@',
                sfLineNumbers: '@',
                sfMatchBrackets: '@',
                sfTabMode: '@'
            },
            templateUrl: function (elem, attrs) {
                var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                var url = attrs.sfTemplateUrl || 'client-components/fields/sf-code-area.sf-cshtml';
                return serverContext.getEmbeddedResourceUrl(assembly, url);
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
