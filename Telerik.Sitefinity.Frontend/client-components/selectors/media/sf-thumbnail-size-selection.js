(function () {
    var displayMode = {
        original: 'Original',
        thumbnail: 'Thumbnail',
        custom: 'Custom'
    };

    angular.module('sfThumbnailSizeSelection', ['sfServices'])
        .controller('sfThumbnailSizeSelectionCtrl', ['$scope', 'sfMediaService', 'serverContext', function ($scope, mediaService, serverContext) {
            $scope.sizeSelection = null;
            $scope.sizeOptions = [];
            $scope.customThumbnailSizeTemplateUrl = serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'client-components/selectors/media/sf-custom-thumbnail-size.html');

            var thumbnailProfiles = [];

            $scope.$watch('sizeSelection', function (newVal, oldVal) {
                if (!newVal || !$scope.model)
                    return;

                var selection = newVal;

                if (!selection.openDialog) {
                    $scope.model.displayMode = selection.type;
                    $scope.model.thumbnail = selection.thumbnail;
                    $scope.model.customSize = selection.customSize;
                }
                else {
                    openModalDialog().then(function (model) {
                        $scope.model.displayMode = selection.type;
                        $scope.model.thumbnail = selection.thumbnail;
                        $scope.model.customSize = model;
                    });
                }
            });

            $scope.$watch('[model.item.Width, model.item.Height]', function (newVal, oldVal) {
                if (newVal[0] && newVal[1]) {
                    if (thumbnailProfiles && thumbnailProfiles.length > 0) {
                        populateOptions();
                    }
                }
            }, true);

            mediaService.images.thumbnailProfiles().then(function (data) {
                if (data && data.Items) {
                    thumbnailProfiles = data.Items;
                    if ($scope.model && $scope.model.item && $scope.model.item.Width && $scope.model.item.Height) {
                        populateOptions();
                    }
                }
            });

            var openModalDialog = function () {
                return angular.element('.thumbnailSizeModal').scope().$openModalDialog({ model: function () { return $scope.model.customSize; } });
            };

            var populateOptions = function () {
                $scope.sizeOptions = [];
                $scope.sizeOptions.push({
                    index: $scope.sizeOptions.length,
                    type: displayMode.original,
                    title: 'Original size: ' + $scope.model.item.Width + 'x' + $scope.model.item.Height + ' px',
                    thumbnail: null,
                    customSize: null,
                    openDialog: false
                });

                for (var i = 0; i < thumbnailProfiles.length; i++) {
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

                var newCustomSizeTitle, existingCustomSizeTitle;
                if ($scope.model.customSize) {
                    newCustomSizeTitle = 'Edit custom size...';
                    if ($scope.model.customSize.MaxWidth && $scope.model.customSize.MaxHeight) {
                        existingCustomSizeTitle = 'Custom size: ' + $scope.model.customSize.MaxWidth + 'x' + $scope.model.customSize.MaxHeight + ' px';
                    }
                    else if ($scope.model.customSize.Width && $scope.model.customSize.Height) {
                        existingCustomSizeTitle = 'Custom size: ' + $scope.model.customSize.Width + 'x' + $scope.model.customSize.Height + ' px';
                    }
                }
                else {
                    newCustomSizeTitle = 'Custom size...';
                }

                if (existingCustomSizeTitle) {
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

                updateSelection();
            };

            var updateSelection = function () {
                if ($scope.sizeOptions.length === 0)
                    return;

                for (var i = 0; i < $scope.sizeOptions.length; i++) {
                    var option = $scope.sizeOptions[i];

                    if (option.type === $scope.model.displayMode) {
                        if (option.type === displayMode.original ||
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
        .controller('sfCustomThumbnailSizeCtrl', ['$scope', '$modalInstance', 'model', function ($scope, $modalInstance, model) {
            $scope.quality = ['High', 'Medium', 'Low'];

            $scope.methodOptions = [{
                value: 'ResizeFitToAreaArguments',
                title: 'Resize to area'
            }, {
                value: 'CropCropArguments',
                title: 'Crop to area'
            }];

            $scope.model = model || {  // Keep the names of those properties as they are in order to support old HTML field.
                MaxWidth: null,
                MaxHeight: null,
                Width: null,
                Height: null,
                ScaleUp: false,
                Quality: $scope.quality[0],
                Method: $scope.methodOptions[0].value
            };

            $scope.done = function () {
                $modalInstance.close($scope.model);
            };

            $scope.cancelResizing = function () {
                $modalInstance.dismiss();
            };
        }]);
})();