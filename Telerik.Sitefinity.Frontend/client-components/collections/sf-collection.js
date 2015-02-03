; (function ($) {
    angular.module('sfCollection', ['sfServices'])
        .directive('sfCollection', ['serverContext', function (serverContext) {
            return {
                restrict: 'AE',
                scope: {
                    sfMultiselect: '@',
                    sfDeselectable: '@',
                    items: '=sfData',
                    sfIdentifier: '@',
                    selectedItemIds: '=?ngModel'
                },
                templateUrl: function (elem, attrs) {
                    if (!attrs.sfTemplateUrl) {
                        throw { message: "You must provide template url." };
                    }

                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    return serverContext.getEmbeddedResourceUrl(assembly, attrs.sfTemplateUrl);
                },
                link: function (scope, element, attrs, ctrl) {
                    var classes = {
                        grid: 'sf-collection-grid',
                        list: 'sf-collection-list'
                    };

                    scope.sfIdentifier = scope.sfIdentifier || 'Id';
                    scope.selectedItemIds = scope.selectedItemIds || [];
                    
                    element.addClass(classes.grid);
                    scope.isSelected = function (item) {
                        if (scope.selectedItemIds === undefined) {
                            return false;
                        }

                        return scope.selectedItemIds.indexOf(item[scope.sfIdentifier]) >= 0;
                    };

                    scope.select = function (item) {
                        if (scope.selectedItemIds === undefined) {
                            return;
                        }

                        var itemIndex = scope.selectedItemIds.indexOf(item[scope.sfIdentifier]);

                        if (scope.sfMultiselect === undefined || scope.sfMultiselect.toLowerCase() === 'false') {
                            if (itemIndex < 0) {
                                scope.selectedItemIds = [item[scope.sfIdentifier]];
                            }
                            else if (scope.sfDeselectable !== undefined && scope.sfDeselectable.toLowerCase() !== 'false') {
                                // item is deselected
                                scope.selectedItemIds = [];
                            }
                        }
                        else {
                            if (itemIndex < 0) {
                                scope.selectedItemIds.push(item[scope.sfIdentifier]);
                            }
                            else if (scope.sfDeselectable !== undefined && scope.sfDeselectable.toLowerCase() !== 'false') {
                                // item is deselected
                                scope.selectedItemIds.splice(itemIndex, 1);
                            }
                        }

                        scope.$emit('sf-collection-item-selected', item);
                    };

                    scope.switchToGrid = function () {
                        element.removeClass(classes.list);
                        element.addClass(classes.grid);
                    };

                    scope.switchToList = function () {
                        element.removeClass(classes.grid);
                        element.addClass(classes.list);
                    };
                }
            };
        }]);
})(jQuery);
