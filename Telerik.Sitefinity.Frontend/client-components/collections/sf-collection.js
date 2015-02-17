; (function ($) {
    angular.module('sfCollection', ['sfServices'])
        .directive('sfCollection', ['serverContext', function (serverContext) {
            var getItemIndex = function (itemsArray, item, propName) {
                if (!itemsArray || !itemsArray.length) {
                    return -1;
                }

                var i;
                if (propName) {
                    for (i = 0; i < itemsArray.length; i++) {
                        if (itemsArray[i] === item[propName]) {
                            return i;
                        }
                    }
                }
                else if (item.Id) {
                    for (i = 0; i < itemsArray.length; i++) {
                        if (itemsArray[i].Id === item.Id) {
                            return i;
                        }
                    }
                }
                else {
                    for (i = 0; i < itemsArray.length; i++) {
                        if (itemsArray[i] === item) {
                            return i;
                        }
                    }
                }

                return -1;
            };

            return {
                restrict: 'AE',
                scope: {
                    items: '=sfData',
                    selectedItems: '=?sfModel',
                    sfMultiselect: '@',
                    sfDeselectable: '@',
                    sfIdentifier: '@'
                },

                templateUrl: function (elem, attrs) {
                    if (!attrs.sfTemplateUrl) {
                        throw { message: "You must provide template url." };
                    }

                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    return serverContext.getEmbeddedResourceUrl(assembly, attrs.sfTemplateUrl);
                },
                link: function (scope, element, attrs, ctrl) {
                    scope.selectedItems = scope.selectedItems || [];

                    scope.isSelected = function (item) {
                        return getItemIndex(scope.selectedItems, item, scope.sfIdentifier) >= 0;
                    };

                    scope.select = function (item) {
                        var event = { item: item, cancel: false };
                        scope.$emit('sf-collection-item-selected', event);

                        if (event.cancel === false) {
                            var isMultiselect = scope.sfMultiselect !== undefined && scope.sfMultiselect.toLowerCase() !== 'false';
                            var isDeselectable = scope.sfDeselectable !== undefined && scope.sfDeselectable.toLowerCase() !== 'false';

                            var itemIndex = getItemIndex(scope.selectedItems, item, scope.sfIdentifier);

                            if (!isMultiselect) {
                                if (itemIndex < 0) {
                                    if (scope.sfIdentifier) {
                                        scope.selectedItems = [item[scope.sfIdentifier]];
                                    }
                                    else {
                                        scope.selectedItems = [item];
                                    }
                                }
                                else if (isDeselectable) {
                                    scope.selectedItems = [];
                                }
                            }
                            else {
                                if (itemIndex < 0) {
                                    if (scope.sfIdentifier) {
                                        scope.selectedItems.push(item[scope.sfIdentifier]);
                                    }
                                    else {
                                        scope.selectedItems.push(item);
                                    }
                                }
                                else if (isDeselectable) {
                                    scope.selectedItems.splice(itemIndex, 1);
                                }
                            }
                        }
                    };
                }
            };
        }]);
})(jQuery);
