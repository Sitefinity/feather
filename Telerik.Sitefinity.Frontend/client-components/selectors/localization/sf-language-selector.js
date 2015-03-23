(function ($, selectorModule) {
    selectorModule.directive('sfLanguageSelector',
        ['sfLanguageService', 'serverContext',
    function (languageService, serverContext) {
        return {
            restrict: 'E',
            scope: {
                sfSite: '=',
                sfCulture: '=?'
            },
            templateUrl: function (elem, attrs) {
                var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                var url = attrs.sfTemplateUrl || 'client-components/selectors/localization/sf-language-selector.sf-cshtml';
                return serverContext.getEmbeddedResourceUrl(assembly, url);
            },
            link: function (scope, element, attrs) {

                var beginLoadingLanguages = function () {
                    var localizationPromise = languageService.getLocalizationSettings();

                    localizationPromise.then(function (data) {
                        var allCultures = data.Cultures;

                        if (serverContext.isMultisiteEnabled()) {
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
                        }
                        else {
                            scope.sfCultures = allCultures;
                        }
                        if ((!scope.sfCulture || !scope.sfCulture.Culture) && scope.sfCultures.length > 0) {
                            scope.sfCulture = getCulture();
                        }
                    });

                    localizationPromise.catch(function (error) {
                        scope.showError = true;
                        scope.errorMessage = error;
                    });
                };

                var getCulture = function () {
                    var currentCultureName = serverContext.getUICulture();

                    var currentCulture = scope.sfCultures.filter(function (culture) {
                        return culture.Culture === currentCultureName;
                    });

                    if (currentCulture.length > 0) {
                        // the cultures for this site contain the UI culture
                        return currentCulture[0];
                    }
                    else {
                        if (serverContext.isMultisiteEnabled()) {
                            return getDefaultCultureForSelectedSite();
                        }
                        else {
                            return scope.sfCultures[0];
                        }
                    }
                };

                var getDefaultCultureForSelectedSite = function () {
                    var defaultCultureForSelectedSite = scope.sfCultures.filter(function (culture) {
                        if (culture.SitesUsingCultureAsDefault) {
                            if (culture.SitesUsingCultureAsDefault.indexOf(scope.sfSite.Name) >= 0) {
                                return culture;
                            }
                        }
                    });

                    return defaultCultureForSelectedSite[0];
                };
                
                if (serverContext.isMultisiteEnabled()) {
                    scope.$watch('sfSite', function (newSite, oldSite) {
                        if (scope.sfSite) {
                            if (oldSite) {
                                scope.sfCulture = null;
                            }
                            beginLoadingLanguages();
                        }
                    });
                }
                else {
                    beginLoadingLanguages();
                }
            }
        };
    }]);
})(jQuery, angular.module('sfSelectors'));
