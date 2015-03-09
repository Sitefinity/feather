(function ($) {
    angular.module('sfSelectors')
        .directive('sfRoleSelector', ['sfRolesService', function (rolesService) {
            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        ctrl.getItems = function (skip, take, search) {
                            var provider = ctrl.$scope.sfProvider;
                            var rolesToHide;
                            if (attrs.sfHideRoles) {
                                rolesToHide = attrs.sfHideRoles.split(',')
                                    .map(function (item) {
                                        return  item.trim();
                                    });
                            }

                            return rolesService.getRoles(provider, skip, take, search, rolesToHide);
                        };

                        ctrl.getSpecificItems = function (ids) {
                            return rolesService.getSpecificRoles(ids);
                        };

                        ctrl.$scope.providerChanged = function (provider) {
                            ctrl.$scope.sfProvider = provider;
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
                            ctrl.$scope.sfProvider = ctrl.$scope.rolesProviders[0].RoleProviderName;
                        };

                        ctrl.canPushSelectedItemFirst = function () {
                            return !ctrl.$scope.sfProvider ||
                                    ctrl.$scope.sfProvider === ctrl.$scope.selectedItemsInTheDialog[0].ProviderName;
                        };

                        var onItemsLoadedSuccess = function (data) {
                            ctrl.$scope.paging.skip += data.Items.length;
                            ctrl.$scope.items.length = 0;

                            if (ctrl.$scope.selectedItemsInTheDialog.length > 0) {
                                ctrl.$scope.sfSelectedItems = [ctrl.$scope.selectedItemsInTheDialog[0]];
                                ctrl.$scope.sfSelectedItem = ctrl.$scope.selectedItemsInTheDialog[0];
                            }

                            if (!ctrl.$scope.multiselect && !ctrl.$scope.filter.searchString && ctrl.canPushSelectedItemFirst()) {
                                ctrl.pushSelectedItemToTheTop(data.Items);
                                ctrl.pushNotSelectedItems(data.Items);
                            }
                            else {
                                ctrl.$scope.items = data.Items;
                            }

                            return ctrl.$scope.items;
                        };

                        var loadRolesProviders = function () {
                            rolesService.getRoleProviders().then(function (data) {
                                ctrl.$scope.rolesProviders = data.Items;
                                ctrl.$scope.sfProvider = ctrl.$scope.rolesProviders[0].RoleProviderName;

                                //// Consider using this logic if we want to support backward compatibility.
                                //var appRolesProvider = {
                                //    "NumOfRoles": 10,
                                //    "RoleProviderName": "AppRoles",
                                //    "RoleProviderTitle": "AppRoles"
                                //};
                                //ctrl.$scope.rolesProviders.push(appRolesProvider);
                            });
                        };

                        ctrl.selectorType = 'RoleSelector';

                        ctrl.dialogTemplateUrl = 'client-components/selectors/identity/sf-role-selector.html';
                        ctrl.$scope.dialogTemplateId = 'sf-role-selector-template';
                        ctrl.setIdentifierField("Name");
                        ctrl.$scope.searchIdentifierField = "Name";

                        var closedDialogTemplate = 'client-components/selectors/common/sf-bubbles-selection.html';

                        ctrl.closedDialogTemplateUrl = closedDialogTemplate;

                        ctrl.$scope.sfDialogHeader = 'Select a role';

                        loadRolesProviders();
                    }
                }
            };
        }]);
})(jQuery);