(function ($) {
    angular.module('sfSelectors')
        .directive('sfProviderSelector', ['sfProviderService', 'serverContext', function (providerService, serverContext) {
            return {
                restrict: 'E',
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/common/sf-provider-selector.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                require: 'ngModel',
                replace: true,
                link: function (scope, tElement, tAttrs, ngModelCtrl) {
                    var onGetProvidersSuccess = function (data) {
                        scope.providers = data.Items;
                        if (ngModelCtrl.$viewValue) {
                            refreshSelectedProvider();
                        }
                        else {
                            scope.selectedProvider = providerService.getDefault(data.Items);
                            ngModelCtrl.$setViewValue(scope.selectedProvider.Name);
                        }

                        scope.isProviderSelectorVisible = data && data.Items && data.Items.length >= 2;
                    };

                    var onGetProvidersError = function () {
                        throw 'Error occurred while populating providers list!';
                    };

                    scope.isOpen = false;
                    scope.providerLabel = tAttrs.sfProviderLabel ? tAttrs.sfProviderLabel : '- Provider -';
                    scope.managerType = tAttrs.sfManagerType;
                    scope.selectedProvider = null;
                    scope.isProviderSelectorVisible = false;

                    //if selection is changed manually from the dropdown
                    scope.selectProvider = function (provider) {
                        scope.isOpen = false;
                        scope.selectedProvider = provider;
                        if (provider) {
                            ngModelCtrl.$setViewValue(provider.Name);
                        }
                    };

                    ngModelCtrl.$render = function () {
                        refreshSelectedProvider();
                    };

                    var refreshSelectedProvider = function () {
                        if (scope.providers) {
                            for (var i = 0; i < scope.providers.length; i++) {
                                if (scope.providers[i].Name == ngModelCtrl.$viewValue) {
                                    scope.selectedProvider = scope.providers[i];
                                    break;
                                }
                            }
                        }
                    };

                    if (scope.managerType)
                        providerService.getAll(scope.managerType).then(onGetProvidersSuccess, onGetProvidersError);
                    else
                        throw 'Error occurred while populating provider list! Please provide value to the managerType attribute!';
                }
            };
        }]);
})(jQuery);
