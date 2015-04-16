(function () {
    angular.module('sfAspectRatioSelection', ['sfServices'])
        .directive('sfAspectRatioSelection', ['serverContext', function (serverContext) {
            return {
                restrict: 'AE',
                scope: {
                    sfRatio: '=',
                    sfHeight: '=',
                    sfWidth: '=',
                    sfItem: '='
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/media/sf-aspect-ratio-selection.sf-cshtml';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs) {

                    var recalculateSize = function (ratio) {
                        if (ratio === '4x3') {
                            scope.sfWidth = 600;
                            scope.sfHeight = 450;
                            aspectRatioCoefficient = 4 / 3;
                        }
                        else if (ratio === '16x9') {
                            scope.sfWidth = 600;
                            scope.sfHeight = 338;
                            aspectRatioCoefficient = 16 / 9;
                        }
                        else if (ratio === 'auto') {
                            if (!scope.sfItem) return;
                            scope.sfWidth = parseInt(scope.sfItem.Width, 10);
                            scope.sfHeight = parseInt(scope.sfItem.Height, 10);
                        }
                        else if (ratio === 'custom') {
                            scope.sfWidth = parseInt(scope.sfWidth, 10);
                            scope.sfHeight = parseInt(scope.sfHeight, 10);
                        }
                    };

                    scope.sfRatio = scope.sfRatio || 'auto';
                    var aspectRatioCoefficient = 1;
                    recalculateSize(scope.sfRatio);

                    scope.$watch('sfRatio', function (newVal, oldVal) {

                        if (!newVal || newVal === oldVal) {
                            return;
                        }

                        recalculateSize(newVal);
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