/*
 * sfWindowResize is a directive which provides functionality for
 * detecting when an element is resized.
 */
  (function () {

    angular.module('sfWindowResize', []).directive('sfWindowResize', ["$timeout", function ($timeout) {

        /*
         * Returns true if user has scrolled to the bottom of the element,
         * otherwise returns false.
         */
        var atBottom = function (element) {
            var offset = 1;
            return ($(element).scrollTop() + $(element).innerHeight() + offset) >= $(element).get(0).scrollHeight;
        };

        var onResize = function (element, callback) {
            if (!onResize.watchedElementData) {
                // First time we are called, create a list of watched elements
                // and hook up the event listeners.
                onResize.watchedElementData = [];

                var checkForChanges = function () {
                    onResize.watchedElementData.forEach(function (data) {
                        if (data.element.checkVisibility() &&
                            (data.element.offsetWidth !== data.offsetWidth || data.element.offsetHeight !== data.offsetHeight)) {
                            data.offsetWidth = data.element.offsetWidth;
                            data.offsetHeight = data.element.offsetHeight;
                            data.callback();
                        }
                    });
                };

                // Listen to the window's size changes
                window.addEventListener('resize', checkForChanges);

                // Listen to changes on the elements in the page that affect layout 
                var observer = new MutationObserver(checkForChanges);
                observer.observe(document.body, {
                    attributes: true,
                    childList: true,
                    characterData: true,
                    subtree: true
                });
            }

            // Save the element we are watching
            onResize.watchedElementData.push({
                element: element,
                offsetWidth: element.offsetWidth,
                offsetHeight: element.offsetHeight,
                callback: callback
            });
        };

        /*
         * Provides the link for the directive implementation.
         */
        var link = function ($scope, element, attributes) {

            // set overflow of the element to scroll
            $(element).css('overflow-y', 'auto');

            var callback = function () {
                // if element is at the bottom, call the delegated needsData function
                if (atBottom(element)) {

                    if (attributes.sfWindowResize === "") {
                        $timeout(function () {
                            var scope = element.scope();

                            if (scope !== undefined) {
                                scope.$parent.loadMore();
                            }
                        });
                    }
                }
            };

            if (window !== "undefined") {
                onResize(element[0], callback);
            }
        };

        return {
            restrict: 'A',

            link: link
        };

    }]);

})();
