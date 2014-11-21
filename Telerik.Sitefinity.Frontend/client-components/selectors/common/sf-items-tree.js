(function () {
    var module = angular.module('sfSelectors');

    module.factory('sfTreeHelper', function () {
        var constructPredecessorsTree = function (predecessors, selectedItemId) {
            // The predecessors collection contains all children of if the items that are in the path to the selected one.            

            var parentsIds = [],
                lastIndex = predecessors.length - 1,
                currentParentId = predecessors[lastIndex].ParentId,
                previousParentId,
                currentLevel = [],                
                previousLevel,
                previousLevelParent,
                isFirstLevel = true,
                selectedItem;

            // We construct the tree from the bottom to the top.
            for (var i = lastIndex; i >= 0 ; i--) {
                var currentItem = predecessors[i];                
                
                if (currentItem.ParentId !== currentParentId) {
                    // We are in a upper level in the hierarchy.

                    if (previousLevelParent) {
                        // We push the parent of the previous level here so it can appear first in the list of items.
                        currentLevel.push(previousLevelParent);
                    }

                    if (selectedItem && isFirstLevel) {
                        // The selected item should be first in the list of items.
                        currentLevel.push(selectedItem);
                    }

                    // Continue with the next level.
                    previousParentId = currentParentId;
                    currentParentId = currentItem.ParentId;

                    previousLevel = currentLevel.reverse();
                    currentLevel = [];

                    isFirstLevel = false;
                }

                // The selected item can only be in the bottom level.
                if (currentItem.Id === previousParentId) {
                    // This item is parent of the previous level of items.
                    // Append the previous level as its children.
                    parentsIds.push(currentItem.Id);
                    currentItem.items = previousLevel;
                    previousLevelParent = currentItem;
                }
                else if (currentItem.Id === selectedItemId) {
                    selectedItem = currentItem;
                }
                else {
                    currentLevel.push(currentItem);
                }                
            }

            if (selectedItem && isFirstLevel) {
                // The selected item is on the root level.
                currentLevel.push(selectedItem);
            }

            if (previousLevelParent) {
                // If we have parent of a previous level this means that we are on root level and we have to add the item to it.
                currentLevel.push(previousLevelParent);
            }

            // The last level is the root level.
            return {
                items: currentLevel.reverse(),
                parentsIds: parentsIds.reverse()
            };
        };

        return {
            constructPredecessorsTree: constructPredecessorsTree
        };
    });

    module.directive('sfItemsTree', ['serverContext', 'sfTreeHelper', '$q', function (serverContext, sfTreeHelper, $q) {
        return {
            restrict: 'E',
            scope: {
                sfMultiselect: '=',
                sfExpandSelection: '=',
                sfItemsPromise: '=',
                sfSelectedIds: '=',
                sfSelectItem: '&',
                sfItemSelected: '&',
                sfItemDisabled: '&?',
                sfGetChildren: '&',
                sfGetPredecessors: '&',
                sfIdentifierFieldValue: '&'
            },
            templateUrl: function (elem, attrs) {
                var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                var url = attrs.sfTemplateUrl || 'client-components/selectors/common/sf-items-tree.html';
                return serverContext.getEmbeddedResourceUrl(assembly, url);
            },
            link: function (scope, element, attrs) {
                /* Helper methods */
                var predecessorsTree;
                var predecessorsTreePromise = $q.defer();

                var constructPredecessorsTree = function (selectedIds) {
                    var selectedId = selectedIds[0];
                    return scope.sfGetPredecessors({ itemId: selectedId })
                        .then(function (predecessors) {
                            return sfTreeHelper.constructPredecessorsTree(predecessors.Items, selectedId);                                                        
                        });
                };

                var getFromAvailableItems = function (data, id) {
                    if (!id) {
                        return data;
                    }
                    else if(data) {
                        for (var i = 0; i < data.length; i++) {
                            if (data[i].Id === id) {
                                return data[i].items;
                            }
                            else if (data[i].items) {
                                var result = getFromAvailableItems(data[i].items, id);
                                if (result) return result;
                            }
                        }
                    }
                };

                var shouldExpandTree = function (scope) {
                    return scope.sfSelectedIds &&
                        scope.sfSelectedIds.length > 0 &&
                        !scope.sfMultiselect &&
                        scope.sfExpandSelection
                };

                /* Scope properties */
                scope.itemsDataSource = new kendo.data.HierarchicalDataSource({
                    schema: {
                        model: {
                            id: 'Id',
                            hasChildren: 'HasChildren'
                        }
                    },
                    transport: {
                        read: function (options) {
                            var id = options.data.Id;
                            var availableItems = getFromAvailableItems(predecessorsTree && predecessorsTree.items, id);
                            if (availableItems) {
                                options.success(availableItems);
                            }
                            else if(id) {
                                scope.sfGetChildren({ parentId: id })
                                    .then(function (children) {
                                        options.success(children);
                                    });
                            }
                        }
                    }
                });

                scope.sfItemsPromise.then(function (newValue) {
                    if (shouldExpandTree(scope)) {
                            // We have to expand only in single selection mode and when there is selected item.
                            // The user of the directive can also disable this behaviour.
                            constructPredecessorsTree(scope.sfSelectedIds)
                                .then(function  (constructedTree) {
                                    predecessorsTree = constructedTree;

                                    scope.itemsDataSource.data(predecessorsTree.items);

                                    predecessorsTreePromise.resolve();
                                });
                    }
                    else {
                        scope.itemsDataSource.data(newValue);
                    }
                });

                scope.$on("kendoWidgetCreated", function(event, widget){
                    // the event is emitted for every widget
                    // if we have multiple widgets, we need to check that the event
                    // is for the one we're interested in.
                    if (widget === scope.treeView && shouldExpandTree(scope)) {
                        predecessorsTreePromise.promise.then(function () {                           
                            scope.treeView.expandPath(predecessorsTree.parentsIds); 
                        });
                    }
                });

                scope.checkboxes = {
                    template: '<input type="checkbox" ng-click="sfSelectItem({ dataItem: dataItem })" ng-checked="sfItemSelected({dataItem: dataItem})" ng-hide="sfItemDisabled({dataItem: dataItem})">'
                };

                scope.itemTemplate = "<a ng-click=\"sfSelectItem({ dataItem: dataItem })\" ng-class=\"{'disabled': sfItemDisabled({dataItem: dataItem}),'active': sfItemSelected({dataItem: dataItem})}\" >" +
                        "<span ng-class=\"{'text-muted': sfItemDisabled({dataItem: dataItem})}\">{{ sfIdentifierFieldValue({dataItem: dataItem}) }}</span> <em ng-show='sfItemDisabled({dataItem: dataItem})' class=\" m-left-md \">(not translated)</em>" +
                    "</a>";
            }
        };
    }]);
})();