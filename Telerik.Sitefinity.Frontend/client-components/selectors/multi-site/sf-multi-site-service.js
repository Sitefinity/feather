(function (serviceModule) {

    serviceModule.factory('sfMultiSiteService',
    ['serviceHelper',
     'serverContext',
      function (serviceHelper, serverContext) {
          /* Private methods and variables */
          var serviceUrl = serverContext.getRootedUrl('/Sitefinity/Services/Multisite/Multisite.svc/');

          var getResource = function (urlParams) {
              var url = serviceUrl;

              if (urlParams) {
                  url = url + urlParams;
              }

              return serviceHelper.getResource(url);
          };

          var getSitesForUserPromise = function (defaultParams) {
              var urlTemplate = 'user/:userId/sites/';
              
              dataItemPromise = getResource(urlTemplate).get(
                  {
                      userId: serverContext.getCurrentUserId(),
                      sortExpression: defaultParams.sortExpression,
                      skip: defaultParams.skip,
                      take: defaultParams.take,
                      filter: defaultParams.filter
                  })
                  .$promise;

              return dataItemPromise;
          };

          return {
              /* Returns the data items. */
              getSitesForUserPromise: getSitesForUserPromise,
          };
      }]);
})(angular.module('sfServices'));