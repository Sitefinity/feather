(function () {
    var module = angular.module('sfSelectors', ['sfServices', 'kendo.directives']);

    module.directive('sfItemsFilter', ['$timeout', function ($timeout) {
        return {
            restrict: 'E',
            scope: {
                sfFilter: '='
            },
            template: '<input type="text" ng-model="sfFilter.searchString" ng-change="applyFilter()" class="form-control" placeholder="{{sfFilter.placeholder}}" />',
            link: function (scope, element, attrs) {
                var timeoutPromise = false;

                // Initialize the default state.
                if(scope.sfFilter) {
                    scope.sfFilter.isEmpty = true;
                }

                var setFilterIsEmpty = function () {
                    if (scope.sfFilter) {
                        scope.sfFilter.isEmpty = !scope.sfFilter.searchString;
                    }
                };

                setFilterIsEmpty();

                scope.applyFilter = function (value) {
                    if (timeoutPromise) {
                        $timeout.cancel(timeoutPromise);
                    }

                    timeoutPromise = $timeout(function () {
                        scope.sfFilter.search(scope.sfFilter.searchString);

                        setFilterIsEmpty();
                    }, scope.sfFilter.timeoutMs);
                };
            }
        };
    }]);

    module.directive('sfEndlessScroll', function () {
        return {
            restrict: 'A',
            scope: {
                sfPaging: '=',
            },
            transclude: true,
            template: '<div ng-transclude></div>' +
                      '<div ng-show="isLoadingData" class="list-group-item text-center">' +
                            '<div></div><div><sf-loading></sf-loading></div><div></div>' +
                      '</div>',
            link: function (scope, element, attrs) {
                scope.isLoadingData = false;

                element.off('scroll');
                element.on('scroll', function () {
                    var raw = jQuery(this)[0];
                    if (scrolledToBottom(raw) && !scope.isLoadingData && !scope.sfPaging.areAllItemsLoaded) {
                        loadItems();
                    }
                });

                function loadItems() {
                    scope.isLoadingData = true;
                    scope.sfPaging.getPage(scope.sfPaging.skip, scope.sfPaging.take)
                        .then(function (data) {
                            if (data.Items.length < scope.sfPaging.take) {
                                scope.sfPaging.areAllItemsLoaded = true;
                            }

                            scope.sfPaging.skip += data.Items.length;
                            scope.sfPaging.pageLoaded(data.Items);
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
            var combinedLength = valueLength,
                lastDroppedIndex;

            do {
                var dropFirst = parts.length == 2;
                var dropped = dropPartFromTheBeginning(parts, dropFirst);
                combinedLength -= dropped.part.length;
                lastDroppedIndex = dropped.index;
            }
            while(combinedLength + skipSymbol.length > maxLength && parts.length >= 2);

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

        /**
         * If needed trims the provided value and puts '...' in the end.
         */
        function trimEnd (value, maxLength) {
            if (value.length > maxLength) {
                var trimmed = value.substr(0, maxLength);
                return trimmed + '...';
            }
            return value;
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
                        var trimmed = trimEnd(parts[0], maxLength);
                        element.text(trimmed);
                        return;
                    }

                    var lastDroppedIndex = dropExtraParts(parts, maxLength, value.length, skipSymbol);                        
                    
                    // Insert the skip symbol.
                    parts.splice(lastDroppedIndex, 0, skipSymbol);

                    var text = parts.join('');
                    element.text(trimEnd(text, maxLength));
                });
            }
        };
    });
})();
