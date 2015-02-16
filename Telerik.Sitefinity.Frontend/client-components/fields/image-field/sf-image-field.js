(function ($) {
    var sfFields = angular.module('sfFields');
    sfFields.requires.push('sfImageField');

    angular.module('sfImageField', ['sfServices', 'sfImageSelector'])
        .directive('sfImageField', ['serverContext', 'sfMediaService', 'sfMediaFilter', function (serverContext, sfMediaService, sfMediaFilter) {
            return {
                restrict: "AE",
                scope: {
                    sfModel: '=',
                    sfImage: '=',
                    sfProvider: '@'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/fields/image-field/sf-image-field.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    var getDateFromString = function (dateStr) {
                        return (new Date(parseInt(dateStr.substring(dateStr.indexOf('Date(') + 'Date('.length, dateStr.indexOf(')'))))).toGMTString();
                    };

                    var getImage = function () {
                        sfMediaService.images.getById(scope.sfModel, scope.sfProvider).then(function (data) {
                            if (data && data.Item) {
                                scope.sfModel = data.Item.Id;
                                scope.sfImage = data.Item;
                                scope.info = {
                                    url: data.Item.ThumbnailUrl,
                                    title: data.Item.Title.Value,
                                    type: data.Item.Extension,
                                    size: Math.ceil(data.Item.TotalSize / 1000) + " KB",
                                    uploaded: getDateFromString(data.Item.DateCreated)
                                };
                            }
                        });
                    };

                    scope.model = {
                        selectedItemIds: null,
                        filterObject: null
                    };

                    scope.editAllProperties = function () {
                    };

                    scope.done = function () {
                        scope.$modalInstance.close();

                        if (scope.model.selectedItemIds && scope.model.selectedItemIds.length) {
                            scope.sfModel = scope.model.selectedItemIds[0];
                            getImage();
                        }
                    };

                    scope.cancel = function () {
                        // cancels the image properties if no image is selected
                        if (scope.sfModel === undefined) {
                            scope.sfModel = null;
                        }

                        scope.$modalInstance.dismiss();
                    };

                    scope.changeImage = function () {
                        if (scope.sfImage) {
                            scope.model.filterObject = sfMediaFilter.newFilter();
                            scope.model.filterObject.set.parent.to(scope.sfImage.FolderId || scope.sfImage.Album.Id);
                        }

                        scope.$openModalDialog();
                    };

                    // Initialize
                    if (scope.sfModel) {
                        getImage();
                    }
                    else {
                        scope.changeImage();
                    }

                    scope.$on('sf-image-selector-image-uploaded', function (event, uploadedImageId) {
                        scope.sfModel = uploadedImageId;
                        getImage();
                        scope.$modalInstance.dismiss();
                    });
                }
            };
        }]);
})(jQuery);
