(function ($) {
    angular.module('sfSelectors')
        .directive('sfUserSelector', ['sfUsersService', function (usersService) {
            return {
                require: '^sfListSelector',
                restrict: 'A',
                link: {
                    pre: function (scope, element, attrs, ctrl) {
                        var usersRequest;
                        var specificItemsRequest;
                        var usersProviderRequest;

                        ctrl.$scope.usersProviders = [];

                        ctrl.getItems = function (skip, take, search) {
                            cancelRequest(usersRequest);

                            var provider = ctrl.$scope.sfProvider;
                            usersRequest = usersService.getUsers(provider, skip, take, search);

                            usersRequest.promise.then(function (data) {
                                data.Items.map(function (item) {
                                    item.Id = item.UserID;
                                });
                            });

                            return usersRequest.promise;
                        };

                        ctrl.getSpecificItems = function (ids) {
                            cancelRequest(specificItemsRequest);

                            specificItemsRequest = usersService.getSpecificUsers(ids);

                            specificItemsRequest.promise.then(function (data) {
                                data.Items.map(function (item) {
                                    item.Id = item.UserID;
                                });
                            });

                            return specificItemsRequest.promise;
                        };

                        ctrl.$scope.providerChanged = function (provider) {
                            cancelRequest(usersRequest);
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
                            if (ctrl.$scope.usersProviders && ctrl.$scope.usersProviders.length > 0) {
                                ctrl.$scope.sfProvider = ctrl.$scope.usersProviders[0].UserProviderName;
                            }
                        };

                        ctrl.onCancel = function () {
                            cancelRequest(usersRequest);
                            cancelRequest(specificItemsRequest);
                            cancelRequest(usersProviderRequest);
                        };

                        ctrl.onDoneSelecting = function () {
                            cancelRequest(usersRequest);
                            cancelRequest(specificItemsRequest);
                            cancelRequest(usersProviderRequest);
                        };

                        ctrl.onOpen = function () {
                            if (!ctrl.$scope.usersProviders || ctrl.$scope.usersProviders.length === 0) {
                                loadUsersProviders();
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

                        var loadUsersProviders = function () {
                            usersProviderRequest = usersService.getUserProviders();
                            usersProviderRequest.promise.then(function (data) {
                                ctrl.$scope.usersProviders = data.Items;

                                if (ctrl.$scope.usersProviders && ctrl.$scope.usersProviders.length > 0) {
                                    ctrl.$scope.sfProvider = ctrl.$scope.usersProviders[0].UserProviderName;
                                }
                            });
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