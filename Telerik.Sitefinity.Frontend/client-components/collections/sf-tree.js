; (function ($) {
    angular.module('sfTree', ['sfServices'])
        .directive('sfTree', ['serverContext', '$q', function (serverContext, $q) {
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

            var TreeNode = function (item) {
                this.item = item;
                this.children = null;
                this.collapsed = true;
            };

            return {
                restrict: 'AE',
                scope: {
                    selectedItems: '=?sfModel',
                    sfMultiselect: '@',
                    sfIdentifier: '@',
                    sfHasChildrenField: '@',
                    sfExpandOnSelect: '@',
                    sfItemTemplateUrl: '@',
                    sfItemTemplateAssembly: '@',
                    sfDeselectable: '@',
                    sfRequestChildren: '&'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/collections/sf-tree.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    var itemAssembly = scope.sfItemTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var itemUrl = scope.sfItemTemplateUrl || 'client-components/collections/sf-tree-item.html';
                    scope.itemTemplateUrl = serverContext.getEmbeddedResourceUrl(itemAssembly, itemUrl);

                    scope.selectedItems = scope.selectedItems || [];

                    // In case no function for getting children is provided, a default one returning empty array is provided.
                    scope.sfRequestChildren = scope.sfRequestChildren || function () { var r = $q.defer(); r.resolve([]); return r.$promise; };

                    scope.hasChildren = function (node) {
                        node = node || {};

                        var result = false;

                        if (scope.sfHasChildrenField === undefined) {
                            result = JSON.stringify({}) !== JSON.stringify(node.children);
                        }
                        else {
                            result = node.item[scope.sfHasChildrenField] === true;
                        }

                        return result;
                    };

                    scope.isSelected = function (node) {
                        return getItemIndex(scope.selectedItems, node.item, scope.sfIdentifier) >= 0;
                    };

                    scope.select = function (node) {
                        var event = { item: node.item, cancel: false };
                        scope.$emit('sf-tree-item-selected', event);

                        var isMultiselect = scope.sfMultiselect !== undefined && scope.sfMultiselect.toLowerCase() !== 'false';
                        var isDeselectable = scope.sfDeselectable !== undefined && scope.sfDeselectable.toLowerCase() !== 'false';
                        var isExpandOnSelect = scope.sfExpandOnSelect !== undefined && scope.sfExpandOnSelect.toLowerCase() !== 'false';

                        var itemIndex = getItemIndex(scope.selectedItems, node.item, scope.sfIdentifier);

                        if (!isMultiselect) {
                            if (itemIndex < 0) {
                                if (scope.sfIdentifier) {
                                    scope.selectedItems = [node.item[scope.sfIdentifier]];
                                }
                                else {
                                    scope.selectedItems = [node.item];
                                }
                            }
                            else if (isDeselectable) {
                                scope.selectedItems = [];
                            }
                        }
                        else {
                            if (itemIndex < 0) {
                                if (scope.sfIdentifier) {
                                    scope.selectedItems.push(node.item[scope.sfIdentifier]);
                                }
                                else {
                                    scope.selectedItems.push(node.item);
                                }
                            }
                            else if (isDeselectable) {
                                scope.selectedItems.splice(itemIndex, 1);
                            }
                        }

                        if (isExpandOnSelect) {
                            scope.toggle(node);
                        }
                    };

                    scope.toggle = function (parentNode) {
                        if (!parentNode || JSON.stringify(parentNode.children) === JSON.stringify({})) {
                            return;
                        }

                        // no requests yet
                        if (parentNode.children === null) {
                            scope.sfRequestChildren({ parent: parentNode.item }).then(function (items) {
                                if (items && items instanceof Array) {
                                    parentNode.children = parentNode.children || {};

                                    // Item must remain expanded if it has no children
                                    if (items.length === 0) {
                                        parentNode.collapsed = false;
                                    }
                                    else {
                                        items.forEach(function (item) {
                                            if (scope.sfIdentifier) {
                                                parentNode.children[item[scope.sfIdentifier]] = new TreeNode(item);
                                            }
                                            else if (item.Id) {
                                                parentNode.children[item.Id] = new TreeNode(item);
                                            }
                                            else {
                                                parentNode.children[item] = new TreeNode(item);
                                            }
                                        });
                                    }
                                }
                            });
                        }

                        parentNode.collapsed = !parentNode.collapsed;
                    };

                    scope.bind = function () {
                        scope.hierarchy = [];
                        // Initial load of root elements
                        scope.sfRequestChildren({ parent: null }).then(function (items) {
                            if (items && items instanceof Array) {
                                items.forEach(function (item) {
                                    scope.hierarchy.push(new TreeNode(item));
                                });
                            }
                        });
                    };

                    scope.bind();
                }
            };
        }]);
})(jQuery);