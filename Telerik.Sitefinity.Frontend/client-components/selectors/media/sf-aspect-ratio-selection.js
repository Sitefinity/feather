(function () {
    angular.module('sfAspectRatioSelection', ['sfServices'])
        .directive('sfAspectRatioSelection', ['serverContext', function (serverContext) {
            return {
                restrict: 'AE',
                scope: {
                    model: '=sfModel'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/media/sf-aspect-ratio-selection.sf-cshtml';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs) {

                    scope.model.aspectRatio = 'auto';

                    scope.$watch('model.aspectRatio', function (newVal, oldVal) {

                        if (!newVal || newVal === oldVal) {
                            return;
                        }

                        if (newVal === '4x3') {
                            scope.model.width = 600;
                            scope.model.height = 450;
                            scope.model.aspectRatioCoefficient = 4 / 3;
                        }
                        else if (newVal === '16x9') {
                            scope.model.width = 600;
                            scope.model.height = 338;
                            scope.model.aspectRatioCoefficient = 16 / 9;
                        }
                        else if (newVal === 'auto') {
                            if (!scope.item) return;
                            scope.model.width = scope.item.Width;
                            scope.model.height = scope.item.Height;
                        }
                        else if (newVal === 'custom') {
                            scope.model.width = "";
                            scope.model.height = "";
                        }
                    });

                    scope.updateWidth = function () {
                        if (scope.model.aspectRatio != '16x9' && scope.model.aspectRatio != '4x3') return;

                        scope.model.width = Math.round(scope.model.height * scope.model.aspectRatioCoefficient);
                    };

                    scope.updateHeight = function () {
                        if (scope.model.aspectRatio != '16x9' && scope.model.aspectRatio != '4x3') return;

                        scope.model.height = Math.round(scope.model.width / scope.model.aspectRatioCoefficient);
                    };
                }
            };
        }])
})();