(function ($) {
    angular.module('designer').directive('sfChangeObserver', function () {
        return {
            restrict: 'A',
            link: function ($scope, $el) {

                function setCustomClass(target) {
                    var customClass = "sf-custom";
                    target.classList.add(customClass);

                    var targetChildren = target.querySelectorAll("*");

                    targetChildren.forEach(function (child) {
                        child.classList.add(customClass);
                    });
                }

                setCustomClass($el[0]);

                var observer = new MutationObserver(function (mutations) {
                    mutations.forEach(function (mutation) {
                        var target = mutation.target;

                        setCustomClass(target);
                    });
                });
                observer.observe($el[0], {
                    childList: true,
                    subtree: true
                });
            }
        };
    });
})(jQuery);
