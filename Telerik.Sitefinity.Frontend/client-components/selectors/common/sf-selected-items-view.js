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
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/common/sf-selected-items-view.sf-cshtml';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                controller: function ($scope) {
                    this.isItemSelected = function (id) {
                        if ($scope.sfSelectedItems) {
                            for (var i = 0; i < $scope.sfSelectedItems.length; i++) {
                                if ($scope.sfSelectedItems[i].Id === id) {
                                    return true;
                                }
                            }
                        }

                        return false;
                    };

                    this.itemClicked = function (item) {
                        if (!$scope.sfSelectedItems) {
                            $scope.sfSelectedItems = [];
                        }

                        var selectedItemIndex;
                        var alreadySelected = false;
                        for (var i = 0; i < $scope.sfSelectedItems.length; i++) {
                            if ($scope.sfSelectedItems[i].Id === item.Id) {
                                selectedItemIndex = i;
                                alreadySelected = true;
                                break;
                            }
                        }

                        if (alreadySelected) {
                            $scope.sfSelectedItems.splice(selectedItemIndex, 1);
                        }
                        else {
                            $scope.sfSelectedItems.push(item);
                        }
                    };
                },
                link: {
                    post: function (scope, element, attrs, ctrl) {
                        var defaultIdentifierField = 'Title';
                        var identifierField = scope.sfIdentifierField || defaultIdentifierField;

                        // The view is binded to this collection
                        scope.currentItems = [];
                        scope.showLoadingIndicator = true && scope.sfSelectedItems && scope.sfSelectedItems.length > 0;

                        scope.$watch('sfItems.length', function (newValue, oldValue) {
                            if (newValue && newValue !== 0) {
                                scope.currentItems = [];
                                Array.prototype.push.apply(scope.currentItems, scope.sfItems);

                                if (scope.filter.searchString && scope.filter.searchString !== "") {
                                    scope.filter.search(scope.filter.searchString);
                                }

                                scope.showLoadingIndicator = false;
                            }
                            else {
                                scope.currentItems = [];
                            }
                        });

                        scope.isListEmpty = function () {
                            return !scope.showLoadingIndicator && scope.sfItems && scope.sfItems.length === 0;
                        };

                        scope.bindIdentifierField = function (item) {
                            return bindSearchIdentifierField(item, identifierField);
                        };

                        var bindSearchIdentifierField = function (item, filterIdentifierField, identifierField) {
                            if (item) {
                                var mainField = item[filterIdentifierField];

                                var valueProp = 'Value';

                                if (!mainField && identifierField)
                                    return item[identifierField];
                                else if (!mainField) {
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
                            var element = scope.sfItems[e.oldIndex];
                            scope.sfItems.splice(e.oldIndex, 1);
                            scope.sfItems.splice(e.newIndex, 0, element);
                        };

                        scope.isItemSelected = ctrl.isItemSelected;
                        scope.itemClicked = ctrl.itemClicked;

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
                                            if (bindSearchIdentifierField(scope.sfItems[i], scope.sfSearchIdentifierField, scope.sfIdentifierField).toLowerCase().indexOf(keyword.toLowerCase()) !== -1) {
                                                scope.currentItems.push(scope.sfItems[i]);
                                            }
                                        }
                                        else if (scope.bindIdentifierField(scope.sfItems[i]).toLowerCase().indexOf(keyword.toLowerCase()) !== -1) {
                                            scope.currentItems.push(scope.sfItems[i]);
                                        }
                                    }
                                }
                            }
                        };

                        scope.sortableOptions = {
                            hint: function (element) {
                                return $('<div class="sf-backend-wrp"><div class="list-group-item list-group-item-multiselect list-group-item-draggable list-group-item-hint">' +
                                            element.html() +
                                        '</div></div>');
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
