; (function () {
    var sfSelectors = angular.module('sfSelectors');
    sfSelectors.requires.push('sfImageSelector');

    angular.module('sfImageSelector', ['sfServices', 'sfInfiniteScroll', 'sfCollection', 'sfTree', 'sfSearchBox', 'sfSortBox', 'sfUploadImageProperties', 'sfDragDrop'])
        .controller('sf-image-selector-upload-ctrl', [function myfunction() {
            scope.model.file.textSize = Math.round(scope.model.file.size / 1000) + " KB";

            // TODO dummy data, please remove after integration with other components.
            scope.uploadInfo = {};
            scope.uploadInfo.percentage = 99;
            scope.uploadInfo.fileName = scope.model.file.name;


            var uploadFile = function () {

                var successAction = function (data) {
                };
                var progressAction = function (data) {
                };
                var errorAction = function (data) {
                };

                sfMediaService.images
                              .upload(scope.model)
                              .then(successAction, errorAction, progressAction);
            };
        }]);
})();
