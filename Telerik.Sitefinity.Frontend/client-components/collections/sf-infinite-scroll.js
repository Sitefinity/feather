/*
 * sfInfiniteScroll is a directive which provides functionality for
 * infinite scrolling through the items, as an alternative to paging.
 * The implementation is very simple and solves only the core problem,
 * which is to invoke a delegate when more data is needed. The idea of
 * this directive is not to provide eye-candy features, such as loaders
 * etc.
 */
; (function () {

    angular.module('sfInfiniteScroll', []).directive('sfInfiniteScroll', [function () {

        /*
         * Returns true if user has scrolled to the bottom of the element,
         * otherwise returns false.
         */
        var atBottom = function (element) {
            return ($(element).scrollTop() + $(element).innerHeight()) >= $(element).get(0).scrollHeight;
        };

        /*
         * Provides the link for the directive implementation.
         */
        var link = function ($scope, element, attributes) {

            // set overflow of the element to scroll
            $(element).css('overflow-y', 'auto');

            element.off('scroll');
            element.on('scroll', function () {
                // if element is at the bottom, call the delegated needsData function
                if (atBottom(element)) {
                    $scope.$apply(function () {
                        $scope.needsData();
                    });
                }
            });
        };

        return {
            restrict: 'A',
            scope: {
                needsData: '&sfInfiniteScroll'
            },
            link: link
        };

    }]);

})();
