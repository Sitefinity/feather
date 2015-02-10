; (function () {
    var sfSelectors = angular.module('sfSelectors');
    sfSelectors.requires.push('sfUploadImageProperties');

    angular.module('sfUploadImageProperties', ['sfServices', 'sfSelectors', 'sfFlatTaxonField'])
        .directive('sfUploadImageProperties', ['serverContext', 'serviceHelper', function (serverContext, serviceHelper) {
            return {
                restrict: 'AE',
                scope: {
                    model: '=sfModel',
                    sfCancelUploadCallback: '&'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/media/sf-upload-image-properties.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    scope.model.file.textSize = Math.round(scope.model.file.size / 1000) + " KB";

                    // TODO dummy data, please remove after integration with other components.
                    scope.uploadInfo = {};
                    scope.uploadInfo.percentage = 99;
                    scope.uploadInfo.fileName = scope.model.file.name;
                }
            };
        }]);
})();