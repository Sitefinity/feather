
//this is the service responsible for managing the properties data for all interested parties
angular.module("providerSelectorModule", []).
    factory('ProvidersDataService', function ($http, UrlHelperService) {
        var defaultProviderName = "";
        var defaulltProvider;

        getCookie = function (cname) {
            var name = cname + "=";
            var ca = document.cookie.split(';');
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i].trim();
                if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
            }
            return "";
        };

        //returns the array of providers
        getProviders = function (managerName, onsuccess, onerror) {

            var getUrl = UrlHelperService.providersDataServiceUrl;

            var getUrl = getUrl + "providers/?sortExpression=Title"
                +"&dataSourceName="+managerName
                +"&siteId="+ getCookie("sf_site")
                +"&itemType=Telerik.Sitefinity.Multisite.Web.Services.ViewModel.SiteDataSourceLinkViewModel"
                +"&itemSurrogateType=Telerik.Sitefinity.Multisite.Web.Services.ViewModel.SiteDataSourceLinkViewModel"
                +"&allProviders=true"
                +"&skip=0"
                +"&take=50";

            return $http.get(getUrl, { cache: false })
                .success(onsuccess)
                .error(onerror);
        };

        //return the default provider
        getDefaultProvider = function (providersList) {
            if (!defaultProviderName)
                defaultProviderName = UrlHelperService.defaultProviderName;
            if (providersList && providersList.length > 0)
            {
                for (var i = 0; i < providersList.length; i++) {
                    if (providersList[i].Name === defaultProviderName) {
                        return providersList[i];
                    }
                }
                return providersList[0];
            }

            return null;
        };

        //sets the default provider name
        setDefaultProviderName = function (providerName) {
            defaultProviderName = providerName;
        };

        //gets the default provider name
        getDefaultProviderName = function () {
            if (!defaultProviderName)
                defaultProviderName = UrlHelperService.defaultProviderName;
            return defaultProviderName;
        };

        //the public interface of the service
        return {
            getProviders: getProviders,
            getDefaultProvider: getDefaultProvider,
            setDefaultProviderName: setDefaultProviderName,
            getDefaultProviderName: getDefaultProviderName
        };
    }).
    directive('providerSelector', function (ProvidersDataService) {
        return {
            restrict: 'E',
            template: '<div class="dropdown s-bg-source-wrp" ng-show="IsProviderSelectorVisible">'
                        +'<a class="btn btn-default dropdown-toggle" >'
                            + '{{SelectedProvider.Title}} <span class="caret"></span>'
                        + '</a>' 
                        + '<ul class="dropdown-menu" >'
                            +'<li>'
                                +'<a href="#">-Provider-</a>'
                            +'</li>'
                            +'<li ng-repeat="provider in Providers">'
                                +'<a href="#" ng-click="SelectedProviderChanged(provider)">{{provider.Title}}</a>'
                            +'</li>'
                        +'</ul>'
                     +'</div>',
            replace: true,
            link: function (scope, tElement, tAttrs) {       

                var onGetProvidersSuccess = function (data) {
                    scope.Providers = data.Items;
                    scope.SelectedProvider = ProvidersDataService.getDefaultProvider(data.Items);
                    scope.SelectedProviderName = ProvidersDataService.getDefaultProviderName();
                    var currentSelectedProviderName;

                    if (data && data.Items && data.Items.length >= 2) {
                        scope.IsProviderSelectorVisible = true;
                        if (scope.SelectedProvider)
                            currentSelectedProviderName = scope.SelectedProvider.Name;
                    }
                    else
                        scope.IsProviderSelectorVisible = false;
                };

                var onGetProvidersError = function () {
                    scope.$emit('errorOccurred', { message: "Error occurred while populating providers list!" });
                };

                scope.SelectedProvider = null;

                //if provider name is set explicitly the dropdown will be updated
                scope.$watch('SelectedProviderName', function () {
                    scope.SelectedProvider = ProvidersDataService.getDefaultProvider(scope.Providers);
                    if (scope.SelectedProvider)
                        scope.$emit('providerSelectionChanged', { providerName: scope.SelectedProvider.Name });
                });

                //if selection is changed manually from the dropdown
                scope.SelectedProviderChanged = function (provider) {
                    scope.SelectedProvider = provider;
                    if (provider) {
                        var currentSelectedProviderName = provider.Name;
                        scope.$emit('providerSelectionChanged', { providerName: currentSelectedProviderName });
                    }
                };

                scope.IsProviderSelectorVisible = false;

                if (tAttrs && tAttrs.managerType)
                    ProvidersDataService.getProviders(tAttrs.managerType, onGetProvidersSuccess, onGetProvidersError);
                else
                    scope.$emit('errorOccurred', { message: "Error occurred while populating providers list! Please provide value to the managerType attribute!" });

            }
        };

    });
