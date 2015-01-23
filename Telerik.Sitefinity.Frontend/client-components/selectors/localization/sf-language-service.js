(function (serviceModule) {

    serviceModule.factory('sfLanguageService',
    ['serviceHelper',
     'serverContext',
      function (serviceHelper, serverContext) {
          /* Private methods and variables */
          var serviceUrl = serverContext.getRootedUrl('Sitefinity/Services/Configuration/ConfigSectionItems.svc/');

          var getResource = function (urlParams) {
              var url = serviceUrl;

              if (urlParams) {
                  url = url + urlParams;
              }

              return serviceHelper.getResource(url);
          };

          var getLocalizationSettings = function (defaultParams) {
              var urlTemplate = 'localization/';

              dataItemPromise = getResource(urlTemplate).get(
                  {
                      includeSitesNames: true,
                  })
                  .$promise;

              return dataItemPromise;
          };

          return {
              /* Returns the data items. */
              getLocalizationSettings: getLocalizationSettings
          };
      }]);
})(angular.module('sfServices'));