(function ($) {
    angular.module('sfComponents', ['sfServices'])
        .directive('sfIcon', ['serverContext', function (serverContext) {
            return {
                restrict: 'AE',
                scope: {
                    sfItem: '=?',
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/components/icon/sf-icon.sf-cshtml';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    var status = scope.sfItem.Lifecycle.IsLocked ? "locked" : scope.sfItem.Lifecycle.WorkflowStatus === "Scheduled" ?
                                                                   "scheduled" : scope.sfItem.ApprovalWorkflowState.Value.toLowerCase();
                    scope.look = status;
                }
            };
        }]);
})(jQuery);