(function () {
    var displayMode = {
        original: 'Original',
        thumbnail: 'Thumbnail',
        custom: 'Custom'
    };

    angular.module('sfMediaSizeSelection', ['sfServices'])
        .controller('sfMediaSizeSelectionCtrl', ['$scope', 'serverContext', function ($scope, serverContext) {
            $scope.sizeSelection = null;
            $scope.sizeOptions = [];
            $scope.customThumbnailSizeTemplateUrl = serverContext.getEmbeddedResourceUrl('Telerik.Sitefinity.Frontend', 'client-components/selectors/media/sf-custom-thumbnail-size.html');

            $scope.$watch('sizeSelection', function (newVal, oldVal) {
                if (!newVal || !$scope.model)
                    return;

                $scope.model.displayMode = newVal.type;
                $scope.model.thumbnail = newVal.thumbnail;
                $scope.model.customSize = newVal.customSize;
                openDialog();
            });

            var openDialog = function (file) {

                angular.element('.thumbnailSizeModal').scope().$openModalDialog({ model: function () { return scope.model; } });
            };
        }])
        .controller('sfCustomThumbnailSizeCtrl', ['$scope', '$modalInstance', function ($scope, $modalInstance) {
            $scope.constants = {
                generationMethod: {
                    resize: 'ResizeFitToAreaArguments',
                    crop: 'CropCropArguments'
                },
                quality: ['High', 'Medium', 'Low']
            }

            $scope.generateThumbnail = function () {
            };

            $scope.cancelResizing = function () {
                $modalInstance.close();
            };
        }]);
})();