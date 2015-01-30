; (function ($) {
    angular.module('sfLibraryFilter', ['sfServices'])
        .directive('sfLibraryFilter', ['serverContext', 'sfMediaService', function (serverContext, sfMediaService) {
            var constants = {
                foldersCallback: 'getFolders'
            };

            return {
                restrict: 'AE',
                scope: {
                    filterObject: '=ngModel',
                    sfMediaType: '@'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/media/sf-library-picker.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    if (!(scope.filterObject instanceof MediaFilter)) {
                        throw { Message: 'ng-model must be of type MediaFilter.' };
                    };

                    if (sfMediaService[scope.sfMediaType] === undefined) {
                        throw { Message: 'sf-media-type is not valid - not found on sfMediaService.' };
                    };

                    if (sfMediaService[scope.sfMediaType][constants.foldersCallback] === undefined) {
                        throw { Message: scope.sfMediaType + ' does not contain ' + constants.foldersCallback + ' callback.' };
                    };

                    scope.selectedItemId = null;

                    scope.requestChildrenCallback = function (parent) {
                        sfMediaService[scope.sfMediaType][constants.foldersCallback](parent.parent).then(function (response) {
                            if (response) {
                                return response.Items;
                            }
                        });
                    };

                    scope.$watch('selectedItemId', function (newVal, oldVal) {
                        if (newVal !== oldVal) {
                            var filter = sfMediaService.newFilter();
                            filter.parent = newVal;

                            // media selector watches this and reacts to changes.
                            scope.filterObject = filter;
                        }
                    });
                }
            };
        }]);
})(jQuery);