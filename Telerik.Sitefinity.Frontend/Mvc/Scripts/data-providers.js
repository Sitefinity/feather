(function ($) {
    var dataProvidersModule = angular.module('dataProviders', ['serverDataModule']);

    dataProvidersModule.factory('providerService', ['$http', '$q', 'serverData',
        function ($http, $q, serverData) {
            var defaultProviderName = serverData.get('defaultProviderName'),
                defaulltProvider;

            var getCookie = function (cname) {
                var name = cname + '=';
                var ca = document.cookie.split(';');
                for (var i = 0; i < ca.length; i++) {
                    var c = ca[i].trim();
                    if (c.indexOf(name) === 0)
                        return c.substring(name.length, c.length);
                }
                return '';
            };

            //returns an array of available providers
            var getAll = function (managerName) {
                var getUrl = sitefinity.services.getProviderServiceUrl() +
                    'providers/?sortExpression=Title' +
                    '&dataSourceName=' + managerName +
                    '&siteId=' + getCookie('sf_site') +
                    '&itemType=Telerik.Sitefinity.Multisite.Web.Services.ViewModel.SiteDataSourceLinkViewModel' +
                    '&itemSurrogateType=Telerik.Sitefinity.Multisite.Web.Services.ViewModel.SiteDataSourceLinkViewModel' +
                    '&allProviders=true' +
                    '&skip=0' +
                    '&take=50';

                var deferred = $q.defer();
                $http.get(getUrl, { cache: false })
                    .success(function (data) {
                        deferred.resolve(data);
                    })
                    .error(function (data) {
                        deferred.reject(data);
                    });

                return deferred.promise;
            };

            //return the default provider
            var getDefault = function (providerList) {
                if (providerList && providerList.length > 0) {
                    for (var i = 0; i < providerList.length; i++) {
                        if (providerList[i].Name === defaultProviderName) {
                            return providerList[i];
                        }
                    }
                    return providerList[0];
                }

                return null;
            };

            //sets the default provider name
            var setDefaultProviderName = function (providerName) {
                defaultProviderName = providerName;
            };

            //gets the default provider name
            var getDefaultProviderName = function () {
                return defaultProviderName;
            };

            //the public interface of the service
            return {
                getAll: getAll,
                getDefault: getDefault,
                setDefaultProviderName: setDefaultProviderName,
                getDefaultProviderName: getDefaultProviderName
            };
        }
    ]);

    dataProvidersModule.directive('providerSelector', ['providerService', function (providerService) {
        return {
            restrict: 'E',
            templateUrl: sitefinity.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'Mvc/Scripts/Templates/provider-selector.html'),
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
                scope.providerLabel = tAttrs.providerLabel ? tAttrs.providerLabel : 'Provider';
                scope.managerType = tAttrs.managerType;
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
