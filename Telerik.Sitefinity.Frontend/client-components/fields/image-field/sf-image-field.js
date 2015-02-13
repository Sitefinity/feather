﻿(function ($) {
    var sfFields = angular.module('sfFields');
    sfFields.requires.push('sfImageField');

    angular.module('sfImageField', ['sfServices', 'sfImageSelector'])
        .directive('sfImageField', ['serverContext', 'sfMediaService', function (serverContext, sfMediaService) {
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
                        selectedItemIds: null
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
                        scope.$modalInstance.close();
                    };

                    scope.changeImage = function () {
                        scope.$openModalDialog();
                    };

                    // Initialize
                    if (scope.sfModel) {
                        getImage();
                    }
                    else {
                        scope.changeImage();
                    }
                }
            };
        }]);
})(jQuery);
