; (function ($) {
    angular.module('sfSearchBox', ['sfServices'])
        .directive('sfSearchBox', ['serverContext', '$timeout', function (serverContext, $timeout) {
            return {
                restrict: 'AE',
                scope: {
                    sfAction: '&',
                    sfMinTextLength: '@?',
                    sfPlaceholder: '@?',
                    sfEnableSuggestions: '@',
                    sfGetSuggestions: '&?',
                    sfClearSearchString: '=',
                    sfTimeoutMs: '@'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/search/sf-search-box.sf-cshtml';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    var timeoutPromise = false;
                    var timeoutMs = parseInt(scope.sfTimeoutMs) ? parseInt(scope.sfTimeoutMs) : 0;
                    scope.showError = false;
                    scope.showSuggestions = false;
                    scope.suggestions = [];

                    if (!attrs.sfAction)
                        scope.showError = true;

                    if (!scope.sfMinTextLength)
                        scope.sfMinTextLength = 0;

                    scope.$watch('sfClearSearchString', function (newVal, oldVal) {
                        if (newVal !== oldVal && scope.sfModel) {
                            scope.sfModel = "";
                        }
                    });

                    scope.sfSearchCallback = function () {
                        if (timeoutPromise) {
                            $timeout.cancel(timeoutPromise);
                        }

                        timeoutPromise = $timeout(function () {
                            if (!scope.sfModel || scope.sfModel.length >= scope.sfMinTextLength) {
                                scope.sfAction({ query: scope.sfModel });

                                if (scope.sfModel && scope.sfEnableSuggestions && attrs.sfGetSuggestions) {
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
                        }, timeoutMs);

                        return timeoutPromise;
                    };
                }
            };
        }]);
})(jQuery);