(function () {
    var displayMode = {
        original: 'Original',
        thumbnail: 'Thumbnail',
        custom: 'Custom'
    };

    angular.module('sfMediaSizeSelection', [])
        .controller('sfMediaSizeSelectionCtrl', ['$scope', function ($scope) {
            $scope.sizeSelection = null;
            $scope.sizeOptions = [];

            $scope.$watch('sizeSelection', function (newVal, oldVal) {
                if (!newVal || !$scope.model)
                    return;

                $scope.model.displayMode = newVal.type;
                $scope.model.thumbnail = newVal.thumbnail;
                $scope.model.customSize = newVal.customSize;
            });
        }]);
})();