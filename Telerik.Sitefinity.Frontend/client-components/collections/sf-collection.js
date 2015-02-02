; (function ($) {
    angular.module('sfCollection', ['sfServices'])
        .directive('sfCollection', ['serverContext', function (serverContext) {
            return {
                restrict: 'AE',
                scope: {
                    sfMultiselect: '@',
                    items: '=sfData',
                    sfIdentifier: '@',
                    selectedItems: '=?ngModel'
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
                    scope.selectedItems = scope.selectedItems || [];

                    element.addClass(classes.grid);
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
