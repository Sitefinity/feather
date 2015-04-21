(function () {
    angular.module('sfAspectRatioSelection', ['sfServices'])
        .directive('sfAspectRatioSelection', ['serverContext', function (serverContext) {
            var constants = {
                '4x3': { ratio: 4 / 3, width: 600, height: 450 },
                '16x9': { ratio: 16 / 9, width: 600, height: 338 },
                'Custom': {},
                'Auto': {}
            };

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
                    scope.model = scope.model || {};
                    scope.model.aspectRatio = scope.model.aspectRatio || 'Auto';
                    scope.constants = constants;

                    scope.changeRatio = function () {
                        // Change width and height if selected ratio has ratio, width and height (check only for ratio)
                        if (constants[scope.model.aspectRatio].ratio) {
                            scope.model.width = constants[scope.model.aspectRatio].width;
                            scope.model.height = constants[scope.model.aspectRatio].height;
                        }
                    };

                    // Change width if selected ratio has ratio (4x3 or 16x9)
                    scope.changeWidth = function (selectedWidth) {
                        if (constants[scope.model.aspectRatio].ratio) {
                            scope.model.height = Math.round(scope.model.width / constants[scope.model.aspectRatio].ratio);
                        }
                    };

                    // Change height if selected ratio has ratio (4x3 or 16x9)
                    scope.changeHeight = function (selectedHeight) {
                        if (constants[scope.model.aspectRatio].ratio) {
                            scope.model.width = Math.round(scope.model.height * constants[scope.model.aspectRatio].ratio);
                        }
                    };
                }
            };
        }]);
})();