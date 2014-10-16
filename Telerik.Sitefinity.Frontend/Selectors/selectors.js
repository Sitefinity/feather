(function () {
    var module = angular.module('selectors', ['services', 'kendo.directives']);

    module.directive('itemsFilter', ['$timeout', function ($timeout) {
        return {
            restrict: 'E',
            scope: {
                filter: '='
            },
            template: '<input type="text" ng-model="filter.searchString" ng-change="applyFilter()" class="form-control" placeholder="{{filter.placeholder}}" />',
            link: function (scope, element, attrs) {
                var timeoutPromise = false;

                scope.applyFilter = function (value) {
                    if (timeoutPromise) {
                        $timeout.cancel(timeoutPromise);
                    }

                    timeoutPromise = $timeout(function () {
                        scope.filter.search(scope.filter.searchString);
                    }, scope.filter.timeoutMs);
                };
            }
        };
    }]);

    module.directive('endlessScroll', function () {
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
                    if (scrolledToBottom(raw) && !scope.isLoadingData) {
                        loadItems();
                    }
                });

                function loadItems() {
                    scope.isLoadingData = true;
                    scope.paging.getPage(scope.paging.skip, scope.paging.take)
                        .then(function (data) {
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
