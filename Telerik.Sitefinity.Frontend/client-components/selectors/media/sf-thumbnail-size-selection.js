(function () {
    var displayMode = {
        original: 'Original',
        thumbnail: 'Thumbnail',
        custom: 'Custom'
    };

    angular.module('sfThumbnailSizeSelection', ['sfServices'])
        .controller('sfThumbnailSizeSelectionCtrl', ['$scope', 'sfMediaService', function ($scope, mediaService) {
            $scope.sizeSelection = null;
            $scope.sizeOptions = [];

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
                    // open custom size dialog and then set model
                }
            });

            $scope.$watch('[model.item.Width, model.item.Height]', function (newVal, oldVal) {
                if (thumbnailProfiles && thumbnailProfiles.length > 0) {
                    populateOptions();
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

                if ($scope.model.customSize && $scope.model.customSize.MaxWidth && $scope.model.customSize.MaxHeight) {
                    $scope.sizeOptions.push({
                        index: $scope.sizeOptions.length,
                        type: displayMode.custom,
                        title: 'Custom size: ' + $scope.model.customSize.MaxWidth + 'x' + $scope.model.customSize.MaxHeight + ' px',
                        thumbnail: null,
                        customSize: $scope.model.customSize,
                        openDialog: false
                    });

                    $scope.sizeOptions.push({
                        index: $scope.sizeOptions.length,
                        type: displayMode.custom,
                        title: 'Edit custom size...',
                        thumbnail: null,
                        customSize: $scope.model.customSize,
                        openDialog: true
                    });
                }
                else {
                    $scope.sizeOptions.push({
                        index: $scope.sizeOptions.length,
                        type: displayMode.custom,
                        title: 'Custom size...',
                        thumbnail: null,
                        customSize: null,
                        openDialog: true
                    });
                }

                updateSelection();
            };

            var updateSelection = function () {
                if ($scope.sizeOptions.length == 0)
                    return;

                for (var i = 0; i < $scope.sizeOptions.length; i++) {
                    var option = $scope.sizeOptions[i];

                    if (option.type === $scope.model.displayMode && option.type === displayMode.original) {
                        $scope.sizeSelection = option;
                        return;
                    }

                    if (option.type === $scope.model.displayMode && option.type === displayMode.thumbnail && option.thumbnail.name === $scope.model.thumbnail.name) {
                        $scope.sizeSelection = option;
                        return;
                    }

                    if (option.type === $scope.model.displayMode && option.type === displayMode.custom && !option.openDialog) {
                        $scope.sizeSelection = option;
                        return;
                    }
                }

                $scope.sizeSelection = $scope.sizeOptions[0];
            };
        }]);
})();