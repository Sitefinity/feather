(function () {
    angular.module('sfMediaBasicFilters', ['sfServices'])
        .directive('sfMediaBasicFilters', ['serverContext', 'sfMediaService', function (serverContext, mediaService) {
            return {
                restrict: 'A',
                scope: {
                    filterObject: '=sfModel'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/media/sf-media-basic-filters.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    scope.select = function (value) {
                        if (scope.filterObject && scope.filterObject.basic === value)
                            return;

                        var filter = mediaService.newFilter();
                        filter.basic = value;

                        scope.filterObject = filter;
                    };
                }
            };
        }]);
})();