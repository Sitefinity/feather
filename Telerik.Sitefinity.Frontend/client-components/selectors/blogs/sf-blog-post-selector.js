(function ($) {
    angular.module('sfSelectors')
        .directive('sfBlogPostSelector', ['sfBlogPostService', function (blogPostService) {
            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        ctrl.getItems = function (skip, take, search, frontendLanguages) {
                            var provider = ctrl.$scope.sfProvider;
                            return blogPostService.getItems(provider, skip, take, search, frontendLanguages);
                        };

                        ctrl.getSpecificItems = function (ids) {
                            var provider = ctrl.$scope.sfProvider;
                            return blogPostService.getSpecificItems(ids, provider);
                        };

                        ctrl.selectorType = 'BlogPostSelector';

                        ctrl.dialogTemplateUrl = 'client-components/selectors/blogs/sf-blog-post-selector.sf-cshtml';
                        ctrl.$scope.dialogTemplateId = 'sf-blog-post-selector-template';

                        var closedDialogTemplate = attrs.sfMultiselect ?
                            'client-components/selectors/common/sf-list-group-selection.sf-cshtml' :
                            'client-components/selectors/common/sf-bubbles-selection.sf-cshtml';

                        ctrl.closedDialogTemplateUrl = closedDialogTemplate;
                    }
                }
            };
        }]);
})(jQuery);
