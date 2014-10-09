(function ($) {
    angular.module('selectors')
        .directive('selectedItemsView', ['$timeout', function ($timeout) {
            return {
                restrict: "E",
                scope: {
                    items: '=?',
                    selectedItems: '=?',
                    identifierField: '=?'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.templateUrl || 'Selectors/selected-items-view.html';
                    return sitefinity.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    post: function (scope, element, attrs) {
                        var defaultIdentifierField = 'Title';
                        var identifierField = scope.identifierField || defaultIdentifierField;
                        var timeoutPromise = false;
                        var originalItems = [];

                        var search = function (value) {
                            scope.items.splice(0, scope.items.length);
                            
                            for (var i = 0; i < originalItems.length; i++) {
                                if (!value) {
                                    scope.items.push(originalItems[i]);
                                }
                                else {
                                    if (scope.bindIdentifierField(originalItems[i]).indexOf(value) !== -1) {
                                        scope.items.push(originalItems[i]);
                                    }
                                }
                            }
                        };

                        scope.$watch('items.length', function () {
                            // the collection is not changed by the search
                            if (!scope.searchValue || scope.searchValue === "") {
                                originalItems = [];
                                for (var i = 0; i < scope.items.length; i++) {
                                    originalItems.push(scope.items[i]);
                                }
                            }
                        });

                        scope.searchValue = '';

                        scope.isListEmpty = function () {
                            return originalItems.length === 0;
                        };

                        scope.bindIdentifierField = function (item) {
                            if (item) {
                                var mainField = item[identifierField];
                                if (mainField) {
                                    return mainField;
                                }
                                else {
                                    return item.Id;
                                }
                            }
                        };

                        scope.isItemSelectedInDialog = function (item) {
                            for (var i = 0; i < scope.selectedItems.length; i++) {
                                if (scope.selectedItems[i].Id === item.Id) {
                                    return true;
                                }
                            }
                        };

                        scope.itemClicked = function (item) {
                            var alreadySelected;
                            var selectedItemindex;
                            for (var i = 0; i < scope.selectedItems.length; i++) {
                                if (scope.selectedItems[i].Id === item.Id) {
                                    alreadySelected = true;
                                    selectedItemindex = i;
                                    break;
                                }
                            }

                            if (alreadySelected) {
                                scope.selectedItems.splice(selectedItemindex, 1);
                            }
                            else {
                                scope.selectedItems.push(item);
                            }
                            
                        };

                        scope.searchItems = function (value) {
                            if (timeoutPromise) {
                                $timeout.cancel(timeoutPromise);
                            }

                            timeoutPromise = $timeout(function () {
                                search(value);
                            }, 500);
                        };
                    }
                }
            };
        }]);
})(jQuery);