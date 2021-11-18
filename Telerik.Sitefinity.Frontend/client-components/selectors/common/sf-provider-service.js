﻿(function () {
    angular.module('sfServices')
       .factory('sfProviderService', ['$http', '$q', 'serverData', 'serverContext', function ($http, $q, serverData, serverContext) {
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

           var siteId = serverContext.siteId();
           if (!siteId) {
               siteId = getCookie('sf_site');
           }

           var defaultProviderName = serverData.get('defaultProviderName');
           if (serverContext.isMultisiteEnabled() && !defaultProviderName) {
               var url = 'Sitefinity/Services/Multisite/Multisite.svc/' + siteId + '/Telerik.Sitefinity.Modules.Libraries.LibrariesManager/availablelinks/';
               $http.get(sitefinity.getRootedUrl(url), { cache: false })
                   .success(function (data) {
                       if (data && data.Items) {
                           data.Items.forEach(function (item) {
                               if (item.Link && item.Link.IsDefault === true) {
                                   defaultProviderName = item.Link.ProviderName;
                                   return;
                               }
                           });
                       }
                   });
           }

            //returns an array of available providers
            var getAll = function (managerName) {
                var getUrl = sitefinity.services.getProviderServiceUrl() +
                    'providers/?sortExpression=Title' +
                    '&dataSourceName=' + managerName +
                    '&siteId=' + siteId +
                    '&itemType=Telerik.Sitefinity.Multisite.Web.Services.ViewModel.SiteDataSourceLinkViewModel' +
                    '&itemSurrogateType=Telerik.Sitefinity.Multisite.Web.Services.ViewModel.SiteDataSourceLinkViewModel' +
                    '&allProviders=true' +
                    '&skip=0';

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
        }]);
})();