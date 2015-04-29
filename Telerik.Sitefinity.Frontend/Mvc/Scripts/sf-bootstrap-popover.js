(function ($) {
    // The out-of-the-box bootstrap's popover directive is not supporting html in the popover's content.
    // This directive wraps the popover.
    // Should be removed when bootstrap release the html feature.
    angular.module('sfBootstrapPopover', ['ui.bootstrap.tooltip'])
        .directive('sfPopoverHtml', ['$compile', function ($compile) {
            return {
                scope: {
                    sfPopoverPopupDelay: '@',
                    sfPopoverPlacement: '@',
                    sfPopoverTrigger: '@',
                    sfPopoverAppendToBody: '@',
                    sfPopoverContent: '@',
                    sfPopoverTitle: '@'
                },
                link: {
                    post: function (scope, element, attrs) {
                        $(element).popover({
                            html: true,
                            delay: parseInt(scope.sfPopoverPopupDelay),
                            placement: scope.sfPopoverPlacement,
                            trigger: scope.sfPopoverTrigger,
                            container: scope.sfPopoverAppendToBody && scope.sfPopoverAppendToBody.toLowerCase() === 'true' ? 'body' : false,
                            content: $compile(scope.sfPopoverContent)(scope.$parent),
                            title: $compile('<span>' + scope.sfPopoverTitle + '</span>')(scope.$parent)
                        });
                    }
                }
            };
        }]);
})(jQuery);
