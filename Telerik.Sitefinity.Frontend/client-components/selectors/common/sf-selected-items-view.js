(function ($) {
    angular.module('sfSelectors')
        .directive('sfSelectedItemsView', ['serverContext', function (serverContext) {
            return {
                restrict: "E",
                scope: {
                    sfItems: '=?',
                    sfSelectedItems: '=?',
                    sfIdentifierField: '=?',
                    sfSearchIdentifierField: '=?',
                    sfSortable: '@'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/common/sf-selected-items-view.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    post: function (scope, element, attrs) {
                        var defaultIdentifierField = 'Title';
                        var identifierField = scope.sfIdentifierField || defaultIdentifierField;

                        // The view is binded to this collection
                        scope.currentItems = [];

                        scope.$watch('sfItems.length', function (newValue, oldValue) {
                            if (newValue !== 0) {
                                scope.currentItems = [];
                                Array.prototype.push.apply(scope.currentItems, scope.sfItems);

                                if (scope.filter.searchString && scope.filter.searchString !== "") {
                                    scope.filter.search(scope.filter.searchString);
                                }
                            }
                        });

                        scope.isListEmpty = function () {
                            return scope.sfItems.length === 0;
                        };

                        scope.bindIdentifierField = function (item) {
                            return bindSearchIdentifierField(item, identifierField);
                        };

                        var bindSearchIdentifierField = function (item, filterIdentifierField) {
                            if (item) {
                                var mainField = item[filterIdentifierField];

                                var valueProp = 'Value';

                                if (!mainField) {
                                    return item.Id;
                                }

                                if (typeof mainField === 'string') {
                                    return mainField;
                                }
                                else if (valueProp in mainField) {
                                    return mainField.Value;
                                }
                            }
                        };

                        scope.sortItems = function (e) {
                            var element = scope.sfSelectedItems[e.oldIndex];
                            scope.sfSelectedItems.splice(e.oldIndex, 1);
                            scope.sfItems.splice(e.oldIndex, 1);
                            scope.sfSelectedItems.splice(e.newIndex, 0, element);
                            scope.sfItems.splice(e.newIndex, 0, element);
                        };

                        scope.itemClicked = function (item) {
                            var selectedItemIndex;
                            for (var i = 0; i < scope.sfSelectedItems.length; i++) {
                                if (scope.sfSelectedItems[i].item.Id === item.Id) {
                                    selectedItemIndex = i;
                                    break;
                                }
                            }

                            scope.sfSelectedItems[selectedItemIndex].isChecked = !scope.sfSelectedItems[selectedItemIndex].isChecked;
                        };

                        scope.filter = {
                            placeholder: 'Narrow by typing',
                            timeoutMs: 500,
                            search: function (keyword) {
                                scope.currentItems.length = 0;

                                if (!keyword) {
                                    Array.prototype.push.apply(scope.currentItems, scope.sfItems);
                                }
                                else {
                                    for (var i = 0; i < scope.sfItems.length; i++) {
                                        if (scope.sfSearchIdentifierField) {
                                            if (bindSearchIdentifierField(scope.sfItems[i].item, scope.sfSearchIdentifierField).toLowerCase().indexOf(keyword.toLowerCase()) !== -1) {
                                                scope.currentItems.push(scope.sfItems[i]);
                                            }
                                        }
                                        else if (scope.bindIdentifierField(scope.sfItems[i].item).toLowerCase().indexOf(keyword.toLowerCase()) !== -1) {
                                            scope.currentItems.push(scope.sfItems[i]);
                                        }
                                    }
                                }
                            }
                        };

                        scope.sortableOptions = {
                            hint: function (element) {
                                return $('<div class="list-group-item list-group-item-multiselect list-group-item-draggable list-group-item-hint">' +
                                            element.html() +
                                        '</div>');
                            },
                            placeholder: function (element) {
                                return $('<div class="list-group-item list-group-item-placeholder"></div>');
                            },
                            handler: ".handler",
                            axis: "y"
                        };
                    }
                }
            };
        }]);
})(jQuery);
