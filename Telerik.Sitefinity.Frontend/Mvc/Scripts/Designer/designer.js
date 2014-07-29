
(function ($) {
    if (typeof ($telerik) != 'undefined') {
        $telerik.$(document).one('dialogRendered', function () {
            angular.bootstrap($('.designer'), ['designer']);
        });
    }

    var resolveDefaultView = function (serverData) {
        return serverData.get('defaultView');
    };

    var resolveControllerName = function (view) {
        return view + 'Ctrl';
    };

    var resolveTemplateUrl = function (view, serverData) {
        var appRoot = serverData.get('applicationRoot');
        if (appRoot.slice(-1) !== '/')
            appRoot = appRoot + '/';

        var widgetName = serverData.get('widgetName');

        return appRoot + String.format('Telerik.Sitefinity.Frontend/Designer/View/{0}/{1}', widgetName, view);
    };

    var designerModule = angular.module('designer', ['pageEditorServices', 'ngRoute', 'modalDialog', 'serverDataModule']);

    designerModule.config(['$routeProvider', 'serverDataProvider', function ($routeProvider, serverDataProvider) {
        var serverData = serverDataProvider.$get();

        $routeProvider
            .when('/:view', {
                templateUrl: function (params) {
                    return resolveTemplateUrl(params.view, serverData);
                },
                controller: 'RoutingCtrl'
            })
            .otherwise({
                redirectTo: '/' + resolveDefaultView(serverData)
            });
    }]);

    designerModule.run(['$rootScope', 'dialogFeedbackService', function ($rootScope, dialogFeedbackService) {
        $rootScope.feedback = dialogFeedbackService;

        $rootScope.$on('$routeChangeStart', function () {
            $rootScope.feedback.showLoadingIndicator = true;
        });

        $rootScope.$on('$routeChangeSuccess', function () {
            $rootScope.feedback.showLoadingIndicator = false;
        });

        $rootScope.$on('$routeChangeError', function () {
            $rootScope.feedback.showLoadingIndicator = false;
            $rootScope.feedback.errorMessage = 'Could not load designer view!';
            $rootScope.feedback.showError = true;
        });
    }]);

    designerModule.factory('dialogFeedbackService', function () {
        return {
            showLoadingIndicator: false,
            showError: false,
            errorMessage: null,

            savingPromise: null,
            cancelingPromise: null
        };
    });

    designerModule.directive('section', ['$compile', function ($compile) {
        return {
            restrict: 'AC',
            link: function (scope, element, attr) {
                var placeholder = $('[placeholder="' + attr.section + '"]');
                if (placeholder.length > 0) {
                    placeholder.html($compile(element.html())(scope));
                }
            }
        };
    }]);

    designerModule.controller('RoutingCtrl', ['$scope', '$routeParams', '$controller',
        function ($scope, $routeParams, $controller) {
            try {
                $controller(resolveControllerName($routeParams.view), { $scope: $scope });
            }
            catch (err) {
                $controller('DefaultCtrl', { $scope: $scope });
            }
        }
    ]);

    designerModule.controller('DefaultCtrl', ['$scope', 'propertyService', 'dialogFeedbackService', function ($scope, propertyService, dialogFeedbackService) {
        $scope.feedback.showLoadingIndicator = true;

        propertyService.get()
            .then(function (data) {
                if (data) {
                    $scope.properties = propertyService.toAssociativeArray(data.Items);
                }
            }, 
            function (data) {
                $scope.feedback.showError = true;
                if (data)
                    $scope.feedback.errorMessage = data.Detail;
            })
            .finally(function () {
                $scope.feedback.showLoadingIndicator = false;
            });
    }]);

    designerModule.controller('DialogCtrl', ['$rootScope', '$scope', '$q', '$modalInstance', '$route', '$timeout', 'propertyService', 'widgetContext', 'dialogFeedbackService',
        function ($rootScope, $scope, $q, $modalInstance, $route, $timeout, propertyService, widgetContext, dialogFeedbackService) {
            var isSaveToAllTranslations = true,
                futureSave = $q.defer(),
                futureCancel = $q.defer();

            // ------------------------------------------------------------------------
            // Event handlers
            // ------------------------------------------------------------------------

            var onError = function (data) {
                $scope.feedback.showError = true;
                if (data)
                    $scope.feedback.errorMessage = data.Detail;
            };

            // ------------------------------------------------------------------------
            // helper methods
            // ------------------------------------------------------------------------

            var saveProperties = function () {
                var saveMode = {
                    Default: 0,
                    AllTranslations: 1,
                    CurrentTranslationOnly: 2
                };

                if (widgetContext.culture) {
                    currentSaveMode = isSaveToAllTranslations ? saveMode.AllTranslations : saveMode.CurrentTranslationOnly;
                }
                else {
                    currentSaveMode = saveMode.Default;
                }

                return propertyService.save(currentSaveMode);
            };

            // ------------------------------------------------------------------------
            // Scope variables and setup
            // ------------------------------------------------------------------------

            $scope.feedback.showLoadingIndicator = true;
            $scope.feedback.showError = false;

            $scope.feedback.savingPromise = futureSave.promise;
            $scope.feedback.cancelingPromise = futureCancel.promise;

            //the save action - it will check which properties are changed and send only them to the server 
            $scope.save = function (saveToAllTranslations) {
                isSaveToAllTranslations = saveToAllTranslations;

                $scope.feedback.savingPromise
                    .then(saveProperties)
                    .then($scope.close)
                    .catch(onError)
                    .finally(function () {
                        $scope.feedback.showLoadingIndicator = false;
                    });

                $scope.feedback.showLoadingIndicator = true;
                futureSave.resolve();
            };

            $scope.cancel = function () {
                $scope.feedback.cancelingPromise
                    .then(function () {
                        propertyService.reset();
                        $scope.close();
                    })
                    .catch(onError)
                    .finally(function () {
                        $scope.feedback.showLoadingIndicator = false;
                    });

                $scope.feedback.showLoadingIndicator = true;
                futureCancel.resolve();
            };

            $scope.close = function () {
                try {
                    $modalInstance.close();
                } catch (e) { }

                if (typeof ($telerik) != 'undefined')
                    $telerik.$(document).trigger('modalDialogClosed');
            };

            $scope.hideSaveAllTranslations = widgetContext.hideSaveAllTranslations;

            $scope.isCurrentView = function (view) {
                return $route.current && $route.current.params.view === view;
            };
            
            $scope.hideError = function () {
                $scope.feedback.showError = false;
                $scope.feedback.errorMessage = null;
            };

            propertyService.get().catch(onError);
        }
    ]);
})(jQuery);