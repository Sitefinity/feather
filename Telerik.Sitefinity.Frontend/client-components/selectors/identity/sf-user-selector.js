(function ($) {
    angular.module('sfSelectors')
        .directive('sfUserSelector', ['sfUsersService', function (usersService) {
            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        ctrl.getItems = function (skip, take, search, frontendLanguages) {
                            var provider = ctrl.$scope.sfProvider;
                            var ignoreAdminUsers = false;
                            var allProviders = true;
                            var result = usersService.getUsers(ignoreAdminUsers, provider, allProviders, skip, take, search);

                            result.then(function (data) {
                                data.Items.map(function (item) {
                                    item.Id = item.UserID;
                                });
                            });

                            return result;
                        };

                        ctrl.getSpecificItems = function (ids) {
                            var provider = ctrl.$scope.sfProvider;
                            var ignoreAdminUsers = false;
                            var allProviders = true;
                            var result = usersService.getSpecificUsers(ids, ignoreAdminUsers, provider, allProviders);

                            result.then(function (data) {
                                data.Items.map(function (item) {
                                    item.Id = item.UserID;
                                });
                            });

                            return result;
                        };

                        ctrl.selectorType = 'UserSelector';
                        ctrl.setIdentifierField("UserName");
                        ctrl.$scope.searchIdentifierField = "UserName";

                        ctrl.dialogTemplateUrl = 'client-components/selectors/identity/sf-user-selector.sf-cshtml';
                        ctrl.$scope.dialogTemplateId = 'sf-user-selector-template';

                        var closedDialogTemplate = 'client-components/selectors/common/sf-bubbles-selection.sf-cshtml';

                        ctrl.closedDialogTemplateUrl = closedDialogTemplate;
                    }
                }
            };
        }]);
})(jQuery);