(function ($) {

    angular.module('expander', [])
        .directive('expander', function () {
            return {
                restrict: 'EA',
                replace: true,
                transclude: true,
                scope: { title: '@expanderTitle' },
                template: '<div>' +
                        '<div class="title" ng-click="toggle()">{{title}}</div>' +
                        '<div class="body" ng-show="isExpanded" ng-transclude></div>' +
                    '</div>',
                link: function (scope, element, attrs) {
                    scope.isExpanded = false;

                    scope.toggle = function toggle() {
                        scope.isExpanded = !scope.isExpanded;
                    };
                }
            };
        });

})(jQuery);