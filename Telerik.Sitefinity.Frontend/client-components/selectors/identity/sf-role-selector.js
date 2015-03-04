(function ($) {
    angular.module('sfSelectors')
        .directive('sfRoleSelector', ['sfRolesService', function (rolesService) {
            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        ctrl.getItems = function (skip, take, search) {
                            var provider = ctrl.$scope.selectedRoleProvider;
                            var rolesToHide = attrs.sfHideRoles.split(',').
                                map(function (item) {
                                    return  item.trim();
                                });

                            return rolesService.getRoles(provider, skip, take, search, rolesToHide);
                        };

                        ctrl.getSpecificItems = function (ids) {
                            return rolesService.getSpecificRoles(ids);
                        };

                        ctrl.$scope.providerChanged = function (provider) {
                            ctrl.$scope.selectedRoleProvider = provider;
                            ctrl.$scope.paging.skip = 0;
                            ctrl.$scope.paging.areAllItemsLoaded = false;

                            var endlessScroll = angular.element($("[sf-endless-scroll]"))[0];
                            if (endlessScroll) {
                                endlessScroll.scrollTop = 0;
                            }

                            ctrl.getItems(ctrl.$scope.paging.skip, ctrl.$scope.paging.take, ctrl.$scope.filter.searchString)
                                .then(onItemsLoadedSuccess, ctrl.onError);
                        };

                        ctrl.onResetItems = function () {
                            ctrl.$scope.selectedRoleProvider = ctrl.$scope.rolesProviders[0].RoleProviderName;
                        };

                        var onItemsLoadedSuccess = function (data) {
                            ctrl.$scope.paging.skip += data.Items.length;
                            ctrl.$scope.items = data.Items;
                        };

                        var loadRolesProviders = function () {
                            rolesService.getRoleProviders().then(function (data) {
                                ctrl.$scope.rolesProviders = data.Items;
                                ctrl.$scope.selectedRoleProvider = ctrl.$scope.rolesProviders[0].RoleProviderName;

                                // TODO: remove this when the new endpoint (returns all roles providers) is added
                                var appRolesProvider = {
                                    "NumOfRoles": 10,
                                    "RoleProviderName": "AppRoles",
                                    "RoleProviderTitle": "AppRoles"
                                };
                                ctrl.$scope.rolesProviders.push(appRolesProvider);
                            });
                        };

                        ctrl.selectorType = 'RoleSelector';

                        ctrl.dialogTemplateUrl = 'client-components/selectors/identity/sf-role-selector.html';
                        ctrl.$scope.dialogTemplateId = 'sf-role-selector-template';
                        ctrl.setIdentifierField("Name");
                        ctrl.$scope.searchIdentifierField = "Name";

                        var closedDialogTemplate = attrs.sfMultiselect ?
                            'client-components/selectors/common/sf-list-group-selection.html' :
                            'client-components/selectors/common/sf-bubbles-selection.html';

                        ctrl.closedDialogTemplateUrl = closedDialogTemplate;

                        ctrl.$scope.sfDialogHeader = 'Select a role';

                        loadRolesProviders();
                    }
                }
            };
        }]);
})(jQuery);