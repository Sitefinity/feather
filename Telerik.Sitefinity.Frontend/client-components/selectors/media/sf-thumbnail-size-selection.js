﻿(function () {
    var displayMode = {
        original: 'Original',
        thumbnail: 'Thumbnail',
        custom: 'Custom'
    };

    angular.module('sfThumbnailSizeSelection', ['sfServices'])
        .directive('sfThumbnailSizeSelection', ['serverContext', function (serverContext) {
            return {
                restrict: 'AE',
                scope: {
                    model: '=sfModel',
                    mediaType: '@sfMediaType',
                    viewType: '@viewType'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/media/sf-thumbnail-size-selection.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs) {
                    attrs.$observe('sfDisableCustomSizeSelection', function () {
                        scope.disableCustomSizeSelection = scope.$eval(attrs.sfDisableCustomSizeSelection);
                    });
                }
            };
        }])
        .controller('sfThumbnailSizeSelectionCtrl', ['$scope', 'sfMediaService', 'serverContext', function ($scope, mediaService, serverContext) {
            $scope.sizeSelection = null;
            $scope.sizeOptions = [];
            $scope.customThumbnailSizeTemplateUrl = serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'client-components/selectors/media/sf-custom-thumbnail-size.sf-cshtml');
            var isResponsiveOptionAvailable = $scope.viewType && ($scope.viewType === 'Telerik.Sitefinity.Frontend.Media.Mvc.Controllers.ImageController');

            var thumbnailProfiles = [];

            $scope.$watch('sizeSelection', function (newVal, oldVal) {
                if (!newVal || !$scope.model)
                    return;

                var selection = newVal;

                if (!selection.openDialog) {
                    $scope.model.displayMode = selection.type;
                    $scope.model.thumbnail = selection.thumbnail;
                    $scope.model.customSize = selection.customSize;

                    if (isResponsiveOptionAvailable) {
                        $scope.model.responsive = selection.responsive;
                    }
                }
                else {
                    openModalDialog().then(function (model) {
                        $scope.model.displayMode = selection.type;
                        $scope.model.thumbnail = selection.thumbnail;
                        $scope.model.customSize = model;

                        if (isResponsiveOptionAvailable) {
                            $scope.model.responsive = selection.responsive;
                        }
                        populateOptions();
                    });
                }
            });

            $scope.$watch('[model.item.Width, model.item.Height]', function (newVal, oldVal) {
                var hasNewValues = newVal[0] && newVal[1];
                if (isVectorGraphics() || hasNewValues) {
                    if (thumbnailProfiles && thumbnailProfiles.length > 0) {
                        populateOptions();
                    }
                }
            }, true);

            mediaService[$scope.mediaType].thumbnailProfiles($scope.viewType).then(function (data) {
                if (data && data.Items) {
                    thumbnailProfiles = data.Items;
                    if ($scope.model) {
                        populateOptions();
                    }
                }
            });

            var openModalDialog = function () {
                return angular.element('.thumbnailSizeModal').scope().$openModalDialog({ model: function () { return $scope.model.customSize; }, isVectorGraphics: function () { return isVectorGraphics(); } });
            };

            var isVectorGraphics = function () {
                if ($scope && $scope.model && $scope.model.item && $scope.model.item.Extension) {
                    return $scope.model.item.IsVectorGraphics === true;
                }
                return false;
            };

            var addCustomSizeSelection = function () {
                var existingCustomSizeTitle;
                if ($scope.model.customSize) {
                    if (isVectorGraphics()) {
                        $scope.model.customSize.Method = 'ResizeFitToAreaArguments';
                    }
                    if ($scope.model.customSize.Method === 'ResizeFitToAreaArguments' && $scope.model.customSize.MaxWidth && $scope.model.customSize.MaxHeight) {
                        existingCustomSizeTitle = 'Custom size: ' + $scope.model.customSize.MaxWidth + 'x' + $scope.model.customSize.MaxHeight + ' px';
                    }
                    else if ($scope.model.customSize.Method === 'CropCropArguments' && $scope.model.customSize.Width && $scope.model.customSize.Height) {
                        existingCustomSizeTitle = 'Custom size: ' + $scope.model.customSize.Width + 'x' + $scope.model.customSize.Height + ' px';
                    }
                }

                var newCustomSizeTitle = 'Custom size...';

                if (existingCustomSizeTitle) {
                    newCustomSizeTitle = 'Edit custom size...';

                    $scope.sizeOptions.push({
                        index: $scope.sizeOptions.length,
                        type: displayMode.custom,
                        title: existingCustomSizeTitle,
                        thumbnail: $scope.model.thumbnail,
                        customSize: $scope.model.customSize,
                        openDialog: false
                    });
                }

                $scope.sizeOptions.push({
                    index: $scope.sizeOptions.length,
                    type: displayMode.custom,
                    title: newCustomSizeTitle,
                    thumbnail: null,
                    customSize: $scope.model.customSize,
                    openDialog: true
                });
            };

            var populateOptions = function () {
                $scope.sizeOptions = [];
                var originalSizeTitle = 'Original size';
                var responsiveTitle = 'Responsive';

                if ($scope.model.thumbnail) {
                    $scope.model.thumbnail.url = null;
                }

                if ($scope.model.item &&
                    $scope.model.item.Width &&
                    $scope.model.item.Height) {
                    originalSizeTitle = 'Original size: ' + $scope.model.item.Width + 'x' + $scope.model.item.Height + ' px';
                }

                $scope.sizeOptions.push({
                    index: $scope.sizeOptions.length,
                    type: displayMode.original,
                    title: originalSizeTitle,
                    thumbnail: null,
                    customSize: null,
                    openDialog: false
                });

                if (isResponsiveOptionAvailable) {
                    $scope.sizeOptions.push({
                        index: $scope.sizeOptions.length,
                        type: displayMode.original,
                        title: responsiveTitle,
                        thumbnail: null,
                        customSize: null,
                        responsive: true,
                        openDialog: false
                    });
                }

                for (var i = 0; i < thumbnailProfiles.length; i++) {
                    if (!isVectorGraphics()) {
                        var profile = thumbnailProfiles[i];

                        $scope.sizeOptions.push({
                            index: $scope.sizeOptions.length,
                            type: displayMode.thumbnail,
                            title: profile.Title,
                            thumbnail: {
                                url: null,
                                name: profile.Id
                            },
                            customSize: null,
                            openDialog: false
                        });
                    }
                }

                if (!$scope.disableCustomSizeSelection) {
                    if ($scope.model && $scope.model.item && $scope.model.item.BlobStorageProvider) {
                        var service = mediaService[$scope.mediaType];
                        service.customImageSizeAllowed($scope.model.item.BlobStorageProvider)
                            .then(function (customSizeAllowed) {
                                if (customSizeAllowed) {
                                    addCustomSizeSelection();
                                }
                            }).finally(function () {
                                updateSelection();
                            });
                    } else {
                        addCustomSizeSelection();
                        updateSelection();
                    }
                } else {
                    updateSelection();
                }
            };

            var updateSelection = function () {
                if ($scope.sizeOptions.length === 0)
                    return;

                for (var i = 0; i < $scope.sizeOptions.length; i++) {
                    var option = $scope.sizeOptions[i];

                    if (option.type === $scope.model.displayMode) {
                        if (option.type === displayMode.original && option.responsive === $scope.model.responsive ||
                            (option.type === displayMode.thumbnail && option.thumbnail.name === $scope.model.thumbnail.name) ||
                            (option.type === displayMode.custom && !option.openDialog)) {
                            $scope.sizeSelection = option;
                            return;
                        }
                    }
                }

                $scope.sizeSelection = $scope.sizeOptions[0];
            };
        }])
        .controller('sfCustomThumbnailSizeCtrl', ['$scope', '$uibModalInstance', 'model', 'serverContext', 'isVectorGraphics', function ($scope, $uibModalInstance, model, serverContext, isVectorGraphics) {
            $scope.quality = ['High', 'Medium', 'Low'];
            $scope.isVectorGraphics = isVectorGraphics;

            var resizeToArea = {
                value: 'ResizeFitToAreaArguments',
                title: 'Resize to area'
            };
            var cropToArea = {
                value: 'CropCropArguments',
                title: 'Crop to area'
            };

            if (isVectorGraphics === true) {
                $scope.methodOptions = [resizeToArea];
            } else {
                $scope.methodOptions = [resizeToArea, cropToArea];
            }

            $scope.model = model || {  // Keep the names of those properties as they are in order to support old HTML field.
                MaxWidth: null,
                MaxHeight: null,
                Width: null,
                Height: null,
                ScaleUp: false,
                Quality: $scope.quality[0],
                Method: $scope.methodOptions[0].value
            };

            // set method 'ResizeFitToAreaArguments' for vector graphics
            if (isVectorGraphics === true) {
                $scope.model.Method = $scope.methodOptions[0].value;
            }

            $scope.resizeToAreaUrl = serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'assets/dist/img/resize-to-area.png');
            $scope.cropToAreaUrl = serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'assets/dist/img/crop-to-area.png');

            $scope.areCustomSizeOptionsValid = function () {
                if ($scope.model.Method === $scope.methodOptions[0].value) {
                    return $scope.model.MaxHeight && $scope.model.MaxWidth &&
                        parseInt($scope.model.MaxHeight) && parseInt($scope.model.MaxWidth) ? true : false;
                }
                else {
                    return $scope.model.Height && $scope.model.Width &&
                        parseInt($scope.model.Height) && parseInt($scope.model.Width) ? true : false;
                }
            };

            $scope.done = function () {
                $uibModalInstance.close($scope.model);
            };

            $scope.cancelResizing = function () {
                $uibModalInstance.dismiss();
            };
        }]);
})();