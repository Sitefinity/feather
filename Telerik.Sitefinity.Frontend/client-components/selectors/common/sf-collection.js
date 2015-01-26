; (function ($) {
    angular.module('sfSelectors')
        .directive('sfListSelector', [function () {
            return {
                restrict: 'A',
                transclude: false,
                scope: {
                    sfData: '=?',
                    sfLoadMore: '=?'
                },
                link: function ($scope, element, attr, ctrl) {

                }
            };
        }]);
})(jQuery);
