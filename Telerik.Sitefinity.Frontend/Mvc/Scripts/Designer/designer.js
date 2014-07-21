
(function ($) {
    if (typeof ($telerik) != 'undefined') {
        $telerik.$(document).one('dialogRendered', function () {
            angular.bootstrap($('.designer'), ['designer']);
        });
    }

    var resolveDepdendencies = function () {
        var defaultDependencies = ['pageEditorServices', 'breadcrumb', 'ngRoute', 'modalDialog'];
        var rawAdditionalDependencies = $('input#additionalDependencies').val();

        if (rawAdditionalDependencies) {
            var additionalDependencies = $.parseJSON(rawAdditionalDependencies);
            return defaultDependencies.concat(additionalDependencies);
        }
        else {
            return defaultDependencies;
        }
    };

    var resolveDefaultView = function () {
        return $('input#defaultView').val();
    };

    var resolverControllerName = function (view) {
        return view + 'Ctrl';
    };

    var designerModule = angular.module('designer', resolveDepdendencies());

    designerModule.config(['$routeProvider', function ($routeProvider) {
        $routeProvider
            .when('/:view', {
                templateUrl: function (params) {
                    var templateId = params.view + '-template';
                    if (document.getElementById(templateId)) {
                        return templateId;
                    }
                    else {
                        return resolveDefaultView() + '-template';
                    }
                },
                controller: 'RoutingCtrl'
            })
            .otherwise({
                redirectTo: '/' + resolveDefaultView()
            });
    }]);

    designerModule.controller('RoutingCtrl', ['$scope', '$routeParams', '$controller', function ($scope, $routeParams, $controller) {
        try {
            $controller(resolverControllerName($routeParams.view), { $scope: $scope });
        }
        catch (err) {
            $controller('DefaultCtrl', { $scope: $scope });
        }
    }]);

    designerModule.controller('DefaultCtrl', ['$scope', 'propertyService', 'dialogFeedbackService', function ($scope, propertyService, dialogFeedbackService) {
        $scope.feedback = dialogFeedbackService;
        $scope.feedback.ShowLoadingIndicator = true;

        propertyService.get()
            .then(function (data) {
                if (data) {
                    $scope.Properties = propertyService.toAssociativeArray(data.Items);
                }
            }, 
            function (data) {
                $scope.feedback.ShowError = true;
                if (data)
                    $scope.feedback.ErrorMessage = data.Detail;
            })
            .finally(function () {
                $scope.feedback.ShowLoadingIndicator = false;
            });
    }]);

    designerModule.factory('dialogFeedbackService', [function () {
        return {
            ShowLoadingIndicator: false,
            ShowError: false,
            ErrorMessage: null,

            SavingPromise: null,
            CancelingPromise: null
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
                $scope.feedback.ShowError = true;
                if (data)
                    $scope.feedback.ErrorMessage = data.Detail;
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
            $scope.feedback.ShowLoadingIndicator = true;
            $scope.feedback.ShowError = false;

            $scope.feedback.SavingPromise = futureSave.promise;
            $scope.feedback.CancelingPromise = futureCancel.promise;

            //the save action - it will check which properties are changed and send only them to the server 
            $scope.Save = function (saveToAllTranslations) {
                isSaveToAllTranslations = saveToAllTranslations;

                $scope.feedback.SavingPromise
                    .then(saveProperties)
                    .then(dialogClose)
                    .catch(onError)
                    .finally(function () {
                        $scope.feedback.ShowLoadingIndicator = false;
                    });

                $scope.feedback.ShowLoadingIndicator = true;
                futureSave.resolve();
            };

            $scope.Cancel = function () {
                $scope.feedback.CancelingPromise
                    .then(function () {
                        propertyService.reset();
                        dialogClose();
                    })
                    .catch(onError)
                    .finally(function () {
                        $scope.feedback.ShowLoadingIndicator = false;
                    });

                $scope.feedback.ShowLoadingIndicator = true;
                futureCancel.resolve();
            };

            $scope.HideSaveAllTranslations = widgetContext.hideSaveAllTranslations;

            $scope.IsCurrentView = function (view) {
                return $routeParams.view === view;
            };
            
            $scope.HideError = function () {
                $scope.feedback.ShowError = false;
                $scope.feedback.ErrorMessage = null;
            };

            propertyService.get().catch(onError);
        }
    ]);
})(jQuery);