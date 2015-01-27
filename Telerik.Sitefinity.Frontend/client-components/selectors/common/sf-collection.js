; (function ($) {
    angular.module('sfSelectors')
        .directive('sfCollection', ['$compile', '$q', '$http', 'serverContext', function ($compile, $q, $http, serverContext) {
            return {
                restrict: 'A',
                scope: {
                    sfTemplateUrl: '@',
                    sfMultiselect: '@',
                    items: '=sfData',
                    ngModel: '=',
                    selectedItems: '=ngModel',
                    sfLoadMore: '&'
                },
                templateUrl: function (elem, attrs) {
                    if (!attrs.sfTemplateUrl) {
                        throw { message: "You can not provide both template html and template url." };
                    }

                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    return serverContext.getEmbeddedResourceUrl(assembly, attrs.sfTemplateUrl);
                },
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        scope.selectItem = function (item) {
                            if (scope.sfMultiselect) {
                                var itemIndex = scope.selectedItems.indexOf(id);
                                if (itemIndex < 0) {
                                    scope.selectedItems.push(id);
                                }
                                else {
                                    scope.selectedItems.splice(itemIndex, 1);
                                }
                            }
                            else {
                                scope.selectedItems = [id];
                            }

                            ctrl.$setViewValue(scope.selectedItems);
                            ctrl.$apply();
                        };

                        //$scope.$on("$destroy", function () {

                        //});
                    }
                }
            };
        }]);
})(jQuery);
