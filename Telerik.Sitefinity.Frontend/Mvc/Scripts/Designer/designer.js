
(function ($) {
    if (typeof ($telerik) != 'undefined') {
        $telerik.$(document).one('dialogRendered', function () {
            angular.bootstrap($('.designer'), ['designer']);
        });
    }

    var resolveDefaultView = function (serverDataProvider) {
        return serverDataProvider.get('defaultView');
    };

    var resolveControllerName = function (view) {
        return view + 'Ctrl';
    };

    var resolveTemplateUrl = function (view, serverDataProvider) {
        var appRoot = serverDataProvider.get('applicationRoot');
        if (appRoot.slice(-1) !== '/')
            appRoot = appRoot + '/';

        var widgetName = serverDataProvider.get('widgetName');

        return appRoot + String.format('Telerik.Sitefinity.Frontend/Designer/View/{0}/{1}', widgetName, view);
    };

    var designerModule = angular.module('designer', ['pageEditorServices', 'ngRoute', 'modalDialog', 'serverDataModule']);

    designerModule.config(['$routeProvider', 'serverDataProvider', function ($routeProvider, serverDataProvider) {
        serverDataProvider.update();

        $routeProvider
            .when('/:view', {
                templateUrl: function (params) {
                    return resolveTemplateUrl(params.view, serverDataProvider);
                },
                controller: 'RoutingCtrl'
            })
            .otherwise({
                redirectTo: '/' + resolveDefaultView(serverDataProvider)
            });
    }]);

    designerModule.factory('dialogFeedbackService', function () {
        return {
            showLoadingIndicator: false,
            showError: false,
            errorMessage: null,
            showView: true,

            savingPromise: null,
            cancelingPromise: null
        };
    });

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
        $scope.feedback = dialogFeedbackService;
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

    designerModule.controller('DialogCtrl', ['$rootScope', '$scope', '$q', '$modalInstance', '$routeParams', 'propertyService', 'widgetContext', 'dialogFeedbackService',
        function ($rootScope, $scope, $q, $modalInstance, $routeParams, propertyService, widgetContext, dialogFeedbackService) {
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

            var dialogClose = function () {
                try {
                    $modalInstance.close();
                } catch (e) { }

                if (typeof ($telerik) != 'undefined')
                    $telerik.$(document).trigger('modalDialogClosed');
            };

            $rootScope.$on('$routeChangeStart', function () {
                $scope.feedback.showLoadingIndicator = true;
                $scope.feedback.showView = false;
            });

            $rootScope.$on('$routeChangeSuccess', function () {
                $scope.feedback.showLoadingIndicator = false;
                $scope.feedback.showView = true;
            });

            $rootScope.$on('$routeChangeError', function () {
                $scope.feedback.showLoadingIndicator = false;
                $scope.feedback.errorMessage = 'Could not load designer view!';
                $scope.feedback.showError = true;
            });

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

            $scope.feedback = dialogFeedbackService;
            $scope.feedback.showLoadingIndicator = true;
            $scope.feedback.showError = false;

            $scope.feedback.savingPromise = futureSave.promise;
            $scope.feedback.cancelingPromise = futureCancel.promise;

            //the save action - it will check which properties are changed and send only them to the server 
            $scope.save = function (saveToAllTranslations) {
                isSaveToAllTranslations = saveToAllTranslations;

                $scope.feedback.savingPromise
                    .then(saveProperties)
                    .then(dialogClose)
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
                        dialogClose();
                    })
                    .catch(onError)
                    .finally(function () {
                        $scope.feedback.showLoadingIndicator = false;
                    });

                $scope.feedback.showLoadingIndicator = true;
                futureCancel.resolve();
            };

            $scope.hideSaveAllTranslations = widgetContext.hideSaveAllTranslations;

            $scope.isCurrentView = function (view) {
                return $routeParams.view === view;
            };
            
            $scope.hideError = function () {
                $scope.feedback.showError = false;
                $scope.feedback.errorMessage = null;
            };

            propertyService.get().catch(onError);
        }
    ]);
})(jQuery);