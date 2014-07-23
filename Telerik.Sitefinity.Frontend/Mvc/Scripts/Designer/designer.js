
(function ($) {
    if (typeof ($telerik) != 'undefined') {
        $telerik.$(document).one('dialogRendered', function () {
            angular.bootstrap($('.designer'), ['designer']);
        });
    }

    var resolveDefaultView = function () {
        return $('input#defaultView').val();
    };

    var resolveControllerName = function (view) {
        return view + 'Ctrl';
    };

    var resolveTemplateId = function (view) {
        return view + '-template';
    };

    var designerModule = angular.module('designer', ['pageEditorServices', 'ngRoute', 'modalDialog']);

    designerModule.config(['$routeProvider', function ($routeProvider) {
        $routeProvider
            .when('/:view', {
                templateUrl: function (params) {
                    var templateId = resolveTemplateId(params.view);
                    if (document.getElementById(templateId)) {
                        return templateId;
                    }
                    else {
                        return resolveTemplateId(resolveDefaultView());
                    }
                },
                controller: 'RoutingCtrl'
            })
            .otherwise({
                redirectTo: '/' + resolveDefaultView()
            });
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

    designerModule.factory('dialogFeedbackService', [function () {
        return {
            showLoadingIndicator: false,
            showError: false,
            errorMessage: null,

            savingPromise: null,
            cancelingPromise: null
        };
    }]);

    designerModule.controller('DialogCtrl', ['$scope', '$q', '$modalInstance', '$routeParams', 'propertyService', 'widgetContext', 'dialogFeedbackService',
        function ($scope, $q, $modalInstance, $routeParams, propertyService, widgetContext, dialogFeedbackService) {
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
            $scope.Save = function (saveToAllTranslations) {
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

            $scope.Cancel = function () {
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

            $scope.HideSaveAllTranslations = widgetContext.hideSaveAllTranslations;

            $scope.IsCurrentView = function (view) {
                return $routeParams.view === view;
            };
            
            $scope.HideError = function () {
                $scope.feedback.showError = false;
                $scope.feedback.errorMessage = null;
            };

            propertyService.get().catch(onError);
        }
    ]);
})(jQuery);