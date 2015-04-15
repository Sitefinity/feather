(function () {
    angular.module('sfAspectRatioSelection', ['sfServices'])
        .directive('sfAspectRatioSelection', ['serverContext', function (serverContext) {
            return {
                restrict: 'AE',
                scope: {
                    sfRatio: '=',
                    sfHeight: "=",
                    sfWidth: "=",
                    sfItem: "="
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/media/sf-aspect-ratio-selection.sf-cshtml';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs) {

                    scope.sfRatio = scope.sfRatio || 'auto';
                    var aspectRatioCoefficient = 1;

                    scope.$watch('sfRatio', function (newVal, oldVal) {

                        if (!newVal || newVal === oldVal) {
                            return;
                        }

                        if (newVal === '4x3') {
                            scope.sfWidth = 600;
                            scope.sfHeight = 450;
                            aspectRatioCoefficient = 4 / 3;
                        }
                        else if (newVal === '16x9') {
                            scope.sfWidth = 600;
                            scope.sfHeight = 338;
                            aspectRatioCoefficient = 16 / 9;
                        }
                        else if (newVal === 'auto') {
                            if (!scope.sfItem) return;
                            scope.sfWidth = scope.sfItem.Width;
                            scope.sfHeight = scope.sfItem.Height;
                        }
                        else if (newVal === 'custom') {
                            scope.sfWidth = "";
                            scope.sfHeight = "";
                        }
                    });

                    scope.updateWidth = function () {
                        if (scope.sfRatio != '16x9' && scope.sfRatio != '4x3') return;

                        scope.sfWidth = Math.round(scope.sfHeight * aspectRatioCoefficient);
                    };

                    scope.updateHeight = function () {
                        if (scope.sfRatio != '16x9' && scope.sfRatio != '4x3') return;

                        scope.sfHeight = Math.round(scope.sfWidth / aspectRatioCoefficient);
                    };
                }
            };
        }]);
})();