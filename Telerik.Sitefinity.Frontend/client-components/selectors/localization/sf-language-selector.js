(function ($, selectorModule) {
    selectorModule.directive('sfLanguageSelector',
        ['sfLanguageService',
          function (languageService) {
              return {
                  restrict: 'E',
                  scope: {
                      sfSite: '=',
                      sfCulture: '=?'
                  },
                  templateUrl: function (elem, attrs) {
                      var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                      var url = attrs.sfTemplateUrl || 'client-components/selectors/localization/sf-language-selector.html';
                      return sitefinity.getEmbeddedResourceUrl(assembly, url);
                  },
                  link: function (scope, element, attrs) {

                      var beginLoadingLanguages = function () {
                          var localizationPromise = languageService.getLocalizationSettings();

                          localizationPromise.then(function (data) {
                              var allCultures = data.Cultures;
                              var siteCultures = [];

                              for (var i = 0, length = allCultures.length; i < length; i++) {
                                  var culture = allCultures[i];

                                  for (var j = 0, sitesLength = culture.SitesNames.length; j < sitesLength; j++) {
                                      var siteName = culture.SitesNames[j];
                                      if (scope.sfSite.Name === siteName) {
                                          siteCultures.push(culture);
                                      }
                                  }
                              }
                              scope.sfCultures = siteCultures;
                          });

                          localizationPromise.catch(function (error) {
                              scope.showError = true;
                              scope.errorMessage = error;
                          });

                          return localizationPromise;
                      };
                      
                      if (scope.sfSite) {
                          beginLoadingLanguages().then(function () {
                              if (!scope.sfCulture && scope.sfCultures.length > 0) {
                                  scope.sfCulture = scope.sfCultures[0];
                              }
                          });
                      }

                      scope.$watch('sfSite', function (newSite, oldSite) {
                          if (scope.sfSite) {
                              beginLoadingLanguages().then(function () {
                                  if (scope.sfCultures.length > 0) {
                                      scope.sfCulture = scope.sfCultures[0];
                                  }
                              });
                          }
                      });
                  }
              };
          }]);
})(jQuery, angular.module('sfSelectors'));
