; (function ($) {
    var sfSelectors = angular.module('sfSelectors');
    sfSelectors.requires.push('sfFileUrlSelector');

    var fileUrlSelector = angular.module('sfFileUrlSelector', ['sfServices', 'sfTree']);
    fileUrlSelector.directive('sfFileUrlSelector', ['sfFileUrlService', 'serverContext',
        function (sfFileUrlService, serverContext) {
            return {
                restrict: 'E',
                scope: {
                    sfModel: '=',
                    extension: '=?sfExtension'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/file-url/sf-file-url-selector.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    if (scope.sfModel) {
                        scope.selectedFile = [scope.sfModel];
                    }

                    scope.getFiles = function (parent) {
                        var path = null;
                        if (parent && !parent.isFolder) {
                            path = parent.path;
                        }
                        else if (parent && parent.isFolder) {
                            path = parent.url;

                            if (path.indexOf('~/') === 0)
                                path = path.substring(2);
                        }

                        return sfFileUrlService.get(scope.extension, path);
                    };

                    scope.$watch('selectedFile', function (newVal, oldVal) {
                        var extension = scope.extension;

                        if (newVal && newVal.length > 0 && newVal[0] && extension) {
                            var hasExtension = newVal[0].slice(-(extension.length + 1)) === '.' + extension;
                            scope.sfModel = hasExtension ? newVal[0] : '';
                        }
                        else {
                            scope.sfModel = '';
                        }
                    });
                }
            };
        }]);
})(jQuery);
