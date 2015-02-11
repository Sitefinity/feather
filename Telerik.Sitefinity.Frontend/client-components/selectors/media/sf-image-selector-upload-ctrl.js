; (function () {
    var sfSelectors = angular.module('sfSelectors');
    sfSelectors.requires.push('sfImageSelector');

    angular.module('sfImageSelector', ['sfServices', 'sfInfiniteScroll', 'sfCollection', 'sfTree', 'sfSearchBox', 'sfSortBox', 'sfDragDrop'])
        .controller('sf-image-selector-upload-ctrl', ['sfMediaService', 'model', function myfunction(sfMediaService, model) {
            scope.model = model;

            scope.model.file.textSize = Math.round(scope.model.file.size / 1000) + " KB";

            // TODO dummy data, please remove after integration with other components.
            scope.uploadInfo = {};
            scope.uploadInfo.percentage = 99;
            scope.uploadInfo.fileName = scope.model.file.name;

            var successAction = function (data) {
                console.log('Uploaded!');
                console.log(data);

                scope.$modalInstance.close(data);
            };

            var progressAction = function (progress) {
                console.log(progress);
            };

            var errorAction = function (err) {
                alert('something went wrong');
                console.log(err);
                scope.$modalInstance.close(false);
            };

            scope.uploadImage = function () {
                sfMediaService.images
                              .upload(scope.model)
                              .then(successAction, errorAction, progressAction);
            };

            scope.cancelUpload = function () {
                console.log('canceled');
                // file was not uploaded
                scope.$modalInstance.close(false);
            };
        }]);
})();
