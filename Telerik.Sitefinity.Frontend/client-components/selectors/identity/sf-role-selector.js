(function ($) {
    angular.module('sfSelectors')
        .directive('sfRoleSelector', ['sfRolesService', function (rolesService) {
            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        var rolesRequest;
                        var specificItemsRequest;
                        var rolesProviderRequest;

                        ctrl.$scope.rolesProviders = [];

                        ctrl.getItems = function (skip, take, search) {
                            cancelRequest(rolesRequest);

                            var provider = ctrl.$scope.sfProvider;
                            var rolesToHide;
                            if (attrs.sfHideRoles) {
                                rolesToHide = attrs.sfHideRoles.split(',')
                                    .map(function (item) {
                                        return  item.trim();
                                    });
                            }

                            rolesRequest = rolesService.getRoles(provider, skip, take, search, rolesToHide);
                            return rolesRequest.promise;
                        };

                        ctrl.getSpecificItems = function (ids) {
                            cancelRequest(specificItemsRequest);

                            specificItemsRequest = rolesService.getSpecificRoles(ids);
                            return specificItemsRequest.promise;
                        };

                        ctrl.$scope.providerChanged = function (provider) {
                            cancelRequest(rolesRequest);
                            ctrl.$scope.showLoadingIndicator = true;
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
                            if (ctrl.$scope.rolesProviders && ctrl.$scope.rolesProviders.length > 0) {
                                ctrl.$scope.sfProvider = ctrl.$scope.rolesProviders[0].RoleProviderName;
                            }
                        };

                        ctrl.onCancel = function () {
                            cancelRequest(rolesRequest);
                            cancelRequest(specificItemsRequest);
                            cancelRequest(rolesProviderRequest);
                        };

                        ctrl.onDoneSelecting = function () {
                            cancelRequest(rolesRequest);
                            cancelRequest(specificItemsRequest);
                            cancelRequest(rolesProviderRequest);
                        };

                        ctrl.onOpen = function () {
                            if (!ctrl.$scope.rolesProviders || ctrl.$scope.rolesProviders.length === 0) {
                                loadRolesProviders();
                            }
                        };

                        ctrl.canPushSelectedItemFirst = function () {
                            return !ctrl.$scope.sfProvider ||
                                    (ctrl.$scope.selectedItemsInTheDialog && ctrl.$scope.selectedItemsInTheDialog.length > 0 &&
                                    ctrl.$scope.sfProvider === ctrl.$scope.selectedItemsInTheDialog[0].ProviderName);
                        };

                        var cancelRequest = function (request) {
                            if (request && request.cancel) {
                                request.cancel('canceled');
                            }
                        };

                        var onItemsLoadedSuccess = function (data) {
                            ctrl.$scope.paging.skip += data.Items.length;
                            ctrl.$scope.items.length = 0;

                            if (ctrl.$scope.selectedItemsInTheDialog && ctrl.$scope.selectedItemsInTheDialog.length > 0) {
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

                            ctrl.$scope.showLoadingIndicator = false;

                            return ctrl.$scope.items;
                        };

                        var loadRolesProviders = function () {
                            rolesProviderRequest = rolesService.getRoleProviders();
                            rolesProviderRequest.promise.then(function (data) {
                                ctrl.$scope.rolesProviders = data.Items;

                                //// Consider using this logic if we want to support backward compatibility.
                                //var appRolesProvider = {
                                //    "NumOfRoles": 10,
                                //    "RoleProviderName": "AppRoles",
                                //    "RoleProviderTitle": "AppRoles"
                                //};
                                //ctrl.$scope.rolesProviders.push(appRolesProvider);

                                if (ctrl.$scope.rolesProviders && ctrl.$scope.rolesProviders.length > 0) {
                                    ctrl.$scope.sfProvider = ctrl.$scope.rolesProviders[0].RoleProviderName;
                                }
                            });
                        };

                        ctrl.selectorType = 'RoleSelector';

                        ctrl.dialogTemplateUrl = 'client-components/selectors/identity/sf-role-selector.sf-cshtml';
                        ctrl.$scope.dialogTemplateId = 'sf-role-selector-template';
                        ctrl.setIdentifierField("Name");
                        ctrl.$scope.searchIdentifierField = "Name";

                        var closedDialogTemplate = 'client-components/selectors/common/sf-bubbles-selection.sf-cshtml';

                        ctrl.closedDialogTemplateUrl = closedDialogTemplate;
                    }
                }
            };
        }]);
})(jQuery);