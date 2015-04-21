(function () {
    angular.module('sfAspectRatioSelection', ['sfServices'])
        .directive('sfAspectRatioSelection', ['serverContext', function (serverContext) {
            var constants = {
                '4x3': { value: '4x3', ratio: 4 / 3, width: 600, height: 450 },
                '16x9': { value: '16x9', ratio: 16 / 9, width: 600, height: 338 },
                'custom': { value: 'custom' },
                'auto': { value: 'auto' }
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
                    scope.model.aspectRatio = scope.model.aspectRatio || constants.auto.value;
                    scope.constants = constants;
                    scope.selectedRatio = getConstantsKeyByVale(scope.model.aspectRatio);

                    scope.changeRatio = function (selectedRatio) {
                        scope.model.aspectRatio = constants[selectedRatio].value;

                        // Change width and height if selected ratio has ratio, width and height (check only for ratio)
                        if (constants[selectedRatio].ratio) {
                            scope.model.width = constants[selectedRatio].width;
                            scope.model.height = constants[selectedRatio].height;
                        }
                    };

                    // Change width if selected ratio has ratio (4x3 or 16x9)
                    scope.changeWidth = function (selectedWidth) {
                        if (constants[scope.selectedRatio].ratio) {
                            scope.model.height = Math.round(scope.model.width / constants[scope.selectedRatio].ratio);
                        }
                    };

                    // Change height if selected ratio has ratio (4x3 or 16x9)
                    scope.changeHeight = function (selectedHeight) {
                        if (constants[scope.selectedRatio].ratio) {
                            scope.model.width = Math.round(scope.model.height * constants[scope.selectedRatio].ratio);
                        }
                    };
                }
            };
        }]);
})();