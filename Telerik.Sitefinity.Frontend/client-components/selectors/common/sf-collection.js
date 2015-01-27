; (function ($) {
    angular.module('sfSelectors')
        .directive('sfCollection', ['serverContext', function (serverContext) {
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
                        scope.sfIdentifier = scope.sfIdentifier || 'Id';

                        element.addClass('sf-collection-grid');
                        scope.isSelected = function (item) {
                            if (scope.selectedItems === undefined) {
                                return false;
                            }

                            return scope.selectedItems.indexOf(item[scope.sfIdentifier]) >= 0;
                        };
                        scope.select = function (item) {
                            if (scope.selectedItems === undefined) {
                                return;
                            }

                            var itemIndex = scope.selectedItems.indexOf(item[scope.sfIdentifier]);

                            if (scope.sfMultiselect === undefined) {
                                if (itemIndex < 0) {
                                    scope.selectedItems = [item[scope.sfIdentifier]];
                                }
                                else {
                                    scope.selectedItems = [];
                                }
                            }
                            else {
                                if (itemIndex < 0) {
                                    scope.selectedItems.push(item[scope.sfIdentifier]);
                                }
                                else {
                                    scope.selectedItems.splice(itemIndex, 1);
                                }
                            }
                        };

                        scope.switchToGrid = function () {
                            element.removeClass('sf-collection-list');
                            element.addClass('sf-collection-grid');
                        };

                        scope.switchToList = function () {
                            element.removeClass('sf-collection-grid');
                            element.addClass('sf-collection-list');
                        };
                    }
                }
            };
        }]);
})(jQuery);
