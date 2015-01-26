(function ($) {
    angular.module('sfSelectors')
        .directive('sfCollection', [function () {
            return {
                restrict: 'A',
                transclude: false,
                scope: true,
                controller: function ($scope) {

                },
                link: {
                    
                }
            };
        }]);
})(jQuery);
