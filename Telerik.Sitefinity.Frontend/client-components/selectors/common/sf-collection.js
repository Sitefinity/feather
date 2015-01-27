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
                    sfIdentifier: '@',
                    selectedItems: '=ngModel',
                    sfLoadMore: '&'
                },
                templateUrl: function (elem, attrs) {
                    if (!attrs.sfTemplateUrl) {
                        throw { message: "You must provide template url." };
                    }

                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    return serverContext.getEmbeddedResourceUrl(assembly, attrs.sfTemplateUrl);
                },
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        scope.isSelected = function (item) {
                            if (scope.selectedItems === undefined) {
                                return false;
                            }

                            return getItemIndex(item) >= 0;
                        };
                        scope.selectItem = function (item) {
                            if (scope.selectedItems === undefined) {
                                return;
                            }

                            var itemIndex = getItemIndex(item);

                            if (scope.sfMultiselect === undefined) {
                                if (itemIndex < 0) {
                                    scope.selectedItems = [item];
                                }
                                else {
                                    scope.selectedItems = [];
                                }
                            }
                            else {
                                if (itemIndex < 0) {
                                    scope.selectedItems.push(item);
                                }
                                else {
                                    scope.selectedItems.splice(itemIndex, 1);
                                }
                            }
                        };

                        var getItemIndex = function (item) {
                            if (scope.selectedItems === undefined) {
                                return;
                            }

                            var prop = scope.sfIdentifier || 'Id';

                            for (var i = 0; i < scope.selectedItems.length; i++) {
                                if (scope.selectedItems[i][prop] === item[prop]) {
                                    return i;
                                }
                            }

                            return -1;
                        };
                    }
                }
            };
        }]);
})(jQuery);
