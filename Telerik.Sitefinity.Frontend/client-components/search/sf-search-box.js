; (function ($) {
    angular.module('sfSearchBox', [])
        .directive('sfSearchBox', ['serverContext', function (serverContext) {
            return {
                restrict: 'AE',
                scope: {
                    sfAction: '&',
                    sfMinTextLength: '@?',
                    sfPlaceholder: '@?',
                    sfEnableAutocomplete: '@',
                    sfGetSuggestions: '&?'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/search/sf-search-box.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    scope.showError = false;
                    scope.showSuggestions = false;
                    scope.suggestions = [];

                    if (!scope.sfAction)
                        scope.showError = true;

                    if (!scope.sfMinTextLength)
                        scope.sfMinTextLength = 3;

                    scope.sfSearchCallback = function () {
                        if (!scope.sfModel || scope.sfModel.length >= scope.sfMinTextLength) {
                            scope.sfAction({ query: scope.sfModel });

                            if (scope.sfModel && scope.sfEnableAutocomplete && scope.sfGetSuggestions) {
                                scope.sfGetSuggestions({ query: scope.sfModel }).then(function (response) {
                                    if (response && response.length) {
                                        scope.suggestions = response;
                                        scope.showSuggestions = true;
                                    }
                                });
                            }
                            else {
                                scope.showSuggestions = false;
                            }
                        }
                    };
                }
            };
        }]);
})(jQuery);