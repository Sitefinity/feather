; (function ($) {
    angular.module('sfSearchBox', [])
        .directive('sfSearchBox', function () {
            return {
                restrict: 'AE',
                scope: {
                    sfAction: '@',
                    sfMinTextLength: '@?',
                    sfPlaceholder: '@?',
                    sfEnableAutocomplete: '@',
                    sfGetSuggestions: '@?'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/search/sf-search-box.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    if (!scope.sfAction)
                        scope.showError = true;

                    if (!scope.sfMinTextLength)
                        scope.sfMinTextLength = 3;

                    scope.sfSearchCallback = function () {
                        if (!scope.sfModel || scope.sfModel.length >= scope.sfMinTextLength) {
                            scope.sfAction({ query: scope.sfModel });
                        }
                    };
                }
            };
        });
})(jQuery);