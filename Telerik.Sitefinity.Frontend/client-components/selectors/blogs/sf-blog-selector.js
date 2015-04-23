(function ($) {
    angular.module('sfSelectors')
        .directive('sfBlogSelector', ['sfBlogService', function (blogService) {
            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        ctrl.getItems = function (skip, take, search, frontendLanguages) {
                            var provider = ctrl.$scope.sfProvider;
                            return blogService.getItems(provider, skip, take, search, frontendLanguages);
                        };

                        ctrl.getSpecificItems = function (ids) {
                            var provider = ctrl.$scope.sfProvider;
                            return blogService.getSpecificItems(ids, provider);
                        };

                        ctrl.selectorType = 'BlogSelector';

                        ctrl.dialogTemplateUrl = 'client-components/selectors/blogs/sf-blog-selector.sf-cshtml';
                        ctrl.$scope.dialogTemplateId = 'sf-blog-selector-template';

                        var closedDialogTemplate = attrs.sfMultiselect ?
                            'client-components/selectors/common/sf-list-group-selection.sf-cshtml' :
                            'client-components/selectors/common/sf-bubbles-selection.sf-cshtml';

                        ctrl.closedDialogTemplateUrl = closedDialogTemplate;
                    }
                }
            };
        }]);
})(jQuery);
