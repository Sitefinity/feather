(function ($) {
    angular.module('sfSelectors')
        .directive('sfBlogPostSelector', ['sfBlogPostService', function (blogPostService) {
            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        var status = attrs.sfMaster === 'true' || attrs.sfMaster === 'True' ? 'master' : 'live';

                        ctrl.getItems = function (skip, take, search, frontendLanguages) {
                            var provider = ctrl.$scope.sfProvider;
                            return blogPostService.getItems(provider, skip, take, search, frontendLanguages, status);
                        };

                        ctrl.getSpecificItems = function (ids) {
                            var provider = ctrl.$scope.sfProvider;
                            return blogPostService.getSpecificItems(ids, provider, status);
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
