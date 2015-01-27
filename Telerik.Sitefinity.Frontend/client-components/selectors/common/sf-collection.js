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
                    sfLoadMore: '&'
                },
                templateUrl: function (elem, attrs) {
                    if (!attrs.sfTemplateUrl) {
                        throw { message: "You can not provide both template html and template url." };
                    }

                    return attrs.sfTemplateUrl;
                },
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        scope.selectItem = function (item) {
                            if (ctrl) {
                                if (scope.sfMultiselect) {
                                    var itemIndex = scope.selectItems.indexOf(id);
                                    if (itemIndex < 0) {
                                        scope.selectItems.push(id);
                                    }
                                    else {
                                        scope.selectItems.splice(itemIndex, 1);
                                    }
                                }
                                else {
                                    scope.selectItems = [id];
                                }

                                ctrl.$setViewValue(scope.selectItems);
                                ctrl.$apply();
                            }
                        };

                        //$scope.$on("$destroy", function () {

                        //});
                    }
                }
            };
        }]);
})(jQuery);
