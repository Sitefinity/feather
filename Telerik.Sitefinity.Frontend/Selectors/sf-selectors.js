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
})();
