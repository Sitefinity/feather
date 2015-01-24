(function (serviceModule) {

    serviceModule.factory('sfMultiSiteService',
    ['serviceHelper',
     'serverContext',
      function (serviceHelper, serverContext) {
          /* Private methods and variables */
          var serviceUrl = serverContext.getRootedUrl('Sitefinity/Services/Multisite/Multisite.svc/');

          var handlers = [];

          var sfSites = [];

          var getResource = function (urlParams) {
              var url = serviceUrl;

              if (urlParams) {
                  url = url + urlParams;
              }

              return serviceHelper.getResource(url);
          };

          var addHandler = function (callback) {
              handlers.push(callback);
          };

          var getSiteByRootNoteId = function (rootNodeId) {
              if (sfSites) {
                  for (var i = 0; i < sfSites.length; i++) {
                      var site = sfSites[i];
                      if (site.SiteMapRootNodeId === rootNodeId) {
                          return site;
                      }
                  }
              }
          };

          var getSitesForUserPromise = function (defaultParams) {
              var urlTemplate = 'user/:userId/sites/';

              var promise = getResource(urlTemplate).get(
                  {
                      userId: serverContext.getCurrentUserId(),
                      sortExpression: defaultParams.sortExpression,
                      skip: defaultParams.skip,
                      take: defaultParams.take,
                      filter: defaultParams.filter
                  })
                  .$promise;

              promise
                  .then(function (data) {
                      sfSites = data.Items;
                  })
                  .then(function (data) {
                      angular.forEach(handlers, function (promise) {
                          if (typeof promise === 'function') {
                              promise();
                          }
                      });
                  });

              return promise;
          };

          var getSites = function () {
              return sfSites;
          };

          return {
              /* Returns the data items. */
              getSitesForUserPromise: getSitesForUserPromise,
              addHandler: addHandler,
              getSiteByRootNoteId: getSiteByRootNoteId,
              getSites: getSites
          };
      }]);
})(angular.module('sfServices'));