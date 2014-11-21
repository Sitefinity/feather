(function () {
    var module = angular.module('sfSelectors', ['services', 'kendo.directives']);

    module.directive('sfItemsFilter', ['$timeout', function ($timeout) {
        return {
            restrict: 'E',
            scope: {
                filter: '='
            },
            template: '<input type="text" ng-model="filter.searchString" ng-change="applyFilter()" class="form-control" placeholder="{{filter.placeholder}}" />',
            link: function (scope, element, attrs) {
                var timeoutPromise = false;

                var setFilterIsEmpty = function () {
                    if (scope.filter) {
                        scope.filter.isEmpty = !scope.filter.searchString;
                    }
                };

                setFilterIsEmpty();

                scope.applyFilter = function (value) {
                    if (timeoutPromise) {
                        $timeout.cancel(timeoutPromise);
                    }

                    timeoutPromise = $timeout(function () {
                        scope.filter.search(scope.filter.searchString);

                        setFilterIsEmpty();
                    }, scope.filter.timeoutMs);
                };
            }
        };
    }]);

    module.directive('sfEndlessScroll', function () {
        return {
            restrict: 'A',
            scope: {
                paging: '=',
            },
            transclude: true,
            template: '<div ng-transclude></div>' +
                      '<div ng-show="isLoadingData" class="list-group-item text-center">' +
                        '<div class="sf-loading" style="display: inline-block; width: 30px;">' +
                            '<div></div><div></div><div></div>' +
                        '</div>' +
                      '</div>',
            link: function (scope, element, attrs) {
                scope.isLoadingData = false;

                element.off('scroll');
                element.on('scroll', function () {
                    var raw = jQuery(this)[0];
                    if (scrolledToBottom(raw) && !scope.isLoadingData && !scope.paging.areAllItemsLoaded) {
                        loadItems();
                    }
                });

                function loadItems() {
                    scope.isLoadingData = true;
                    scope.paging.getPage(scope.paging.skip, scope.paging.take)
                        .then(function (data) {
                            if (data.Items.length < scope.paging.take) {
                                scope.paging.areAllItemsLoaded = true;
                            }

                            scope.paging.skip += data.Items.length;
                            scope.paging.pageLoaded(data.Items);
                        }).finally(function () {
                            scope.isLoadingData = false;
                        });
                }

                function scrolledToBottom(element) {
                    return element.scrollTop !== 0 && element.scrollTop + element.offsetHeight >= element.scrollHeight;
                }
            }
        };
    });

    module.directive('sfShrinkedBreadcrumb', function () {
        /**
         * Drop a part from the beginning of the array.
         */
        function dropPartFromTheBeginning(parts, dropFirst) {
            var startIndex = dropFirst ? 0 : 1;

            var part = parts.splice(startIndex, 1)[0];
            return {
                part: part,
                index: startIndex
            };
        }

        /**
         * Removes parts from the beginning of the breadcrumb in order to fit in the provided max length.
         */
        function dropExtraParts (parts, maxLength, valueLength, skipSymbol) {
            var combinedWidth = valueLength,
                lastDroppedIndex;

            do {
                var dropFirst = parts.length == 2;
                var dropped = dropPartFromTheBeginning(parts, dropFirst);
                combinedWidth -= dropped.part.length;
                lastDroppedIndex = dropped.index;
            }
            while(combinedWidth + skipSymbol.length > maxLength && parts.length >= 2);

            return lastDroppedIndex;
        }

        /**
         * Splits the given text into parts.
         */
        function splitIntoParts (value, splitSymbol) {
            var parts = value.split(splitSymbol),
                lastIndex = parts.length - 1;

            return parts.map(function (part, index) {
                return index == lastIndex ? part : part + splitSymbol;
            });
        }

        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                attrs.$observe('sfShrinkedBreadcrumb', function (value) {
                    // Do nothing if there is no value or it is empty string.
                    if (!value) return;

                    var maxLength = attrs.sfMaxLength;

                    if (value.length <= maxLength) {
                        element.text(value);
                        return;
                    }

                    var splitSymbol = ' > ',
                        skipSymbol = '... > ';
                        
                    var parts = splitIntoParts(value, splitSymbol);

                    if(parts.length === 1) {
                        // If very long item on root level is selected.
                        element.text(parts[0]);
                        return;
                    }

                    var lastDroppedIndex = dropExtraParts(parts, maxLength, value.length, skipSymbol);                        
                    
                    // Insert the skip symbol.
                    parts.splice(lastDroppedIndex, 0, skipSymbol);

                    var text = parts.join('');
                    element.text(text);
                });
            }
        };
    });
})();
