
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

    designerModule.controller('DefaultCtrl', ['$scope', 'propertyService', function ($scope, propertyService) {
        $scope.ShowLoadingIndicator = true;

        propertyService.get().then(function (data) {
            if (data) {
                $scope.Items = data.Items;
                $scope.Properties = propertyService.toAssociativeArray(data.Items);
            }
            $scope.ShowLoadingIndicator = false;
        }, 
        function (data) {
            $scope.ShowError = true;
            if (data)
                $scope.ErrorMessage = data.Detail;
            $scope.ShowLoadingIndicator = false;
        });
    }]);

    designerModule.controller('DialogCtrl', ['$scope', '$modalInstance', '$routeParams', '$location', 'propertyService', 'widgetContext',
        function ($scope, $modalInstance, $routeParams, $location, propertyService, widgetContext) {
            var isSaveToAllTranslations = true;
            // ------------------------------------------------------------------------
            // Event handlers
            // ------------------------------------------------------------------------

            var onGetPropertiesSuccess = function (data) {
                if (typeof ($telerik) != 'undefined')
                    $telerik.$(document).trigger('controlPropertiesLoad', [{ 'Items': data.Items }]);

                $scope.ShowLoadingIndicator = false;
            };

            var onError = function (data, status, headers, config) {
                showError(data.Detail);
            }

            var dialogClose = function () {
                try {
                    $modalInstance.close()
                } catch (e) { }

                $scope.ShowLoadingIndicator = false;

                if (typeof ($telerik) != 'undefined')
                    $telerik.$(document).trigger('modalDialogClosed');
            };

            // ------------------------------------------------------------------------
            // helper methods
            // ------------------------------------------------------------------------

            var saveProperties = function (modifiedProperties) {
                var saveMode = {
                    Default: 0,
                    AllTranslations: 1,
                    CurrentTranslationOnly: 2
                };

                var currentSaveMode = saveMode.Default;
                if (widgetContext.culture) {
                    currentSaveMode = isSaveToAllTranslations ? saveMode.AllTranslations : saveMode.CurrentTranslationOnly;
                }

                propertyService.save(currentSaveMode, modifiedProperties).then(dialogClose, onError);
            };

            var showError = function(message){
                $scope.ShowError = true;
                if (message)
                    $scope.ErrorMessage = message;

                $scope.ShowLoadingIndicator = false;
            };

            // ------------------------------------------------------------------------
            // Scope variables and setup
            // ------------------------------------------------------------------------

            $scope.ShowLoadingIndicator = true;
            $scope.ShowError = false;

            //the save action - it will check which properties are changed and send only them to the server 
            $scope.Save = function (saveToAllTranslations) {
                isSaveToAllTranslations = saveToAllTranslations;

                $scope.$broadcast('saveButtonPressed', null);

                $scope.ShowLoadingIndicator = true;

                var args = { Cancel: false };

                if (typeof ($telerik) != 'undefined') {
                    propertyService.get().then(function (data) {
                        $telerik.$(document).trigger('controlPropertiesUpdating', [{ 'Items': data.Items, 'args': args }]);
                    }, onError);
                }

                if (!args.Cancel)
                    saveProperties();
            };

            $scope.Cancel = function () {
                if (typeof ($telerik) != 'undefined') {
                    propertyService.get().then(function (data) {
                        $telerik.$(document).trigger('controlPropertiesUpdateCanceling', [{ 'Items': data.Items }]);
                        propertyService.reset();
                        dialogClose();
                    }, onError);
                }
            };

            $scope.HideSaveAllTranslations = widgetContext.hideSaveAllTranslations;

            $scope.IsCurrentView = function (view) {
                return $routeParams.view === view;
            };

            propertyService.get().then(onGetPropertiesSuccess, onError);

            if (typeof ($telerik) != 'undefined') {
                $telerik.$(document).one('controlPropertiesUpdate', function (e, params) {
                    if(params.Items)
                        saveProperties(params.Items);
                    if (params.error)
                        showError(params.error);
                    else
                        dialogClose();
                });
                $telerik.$(document).one('controlPropertiesLoaded', function (e, params) {
                    propertyService.set(params.Items);
                });
            }
        }
    ]);
})(jQuery);