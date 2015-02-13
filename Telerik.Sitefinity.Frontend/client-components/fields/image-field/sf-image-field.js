(function ($) {
    var sfFields = angular.module('sfFields');
    sfFields.requires.push('sfImageField');

    angular.module('sfImageField', ['sfServices', 'sfImageSelector'])
        .directive('sfImageField', ['serverContext', 'sfMediaService', function (serverContext, sfMediaService) {
            return {
                restrict: "AE",
                scope: {
                    sfModel: '=',
                    sfProvider: '@'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/fields/image-field/sf-image-field.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    var getImage = function () {
                        sfMediaService.images.getById(scope.sfModel, scope.sfProvider).then(function (data) {
                            console.log(data);

                            if (data && data.Item) {
                                scope.sfModel = data.Item.Id;
                                scope.image = data.Item;
                            }
                        });
                    };

                    if (scope.sfModel) {
                        getImage();
                    }
                    
                    scope.changeImage = function () {
                        angular.element(".imageSelectorModal").scope().$openModalDialog()
                            .then(function (selectedImageIds) {
                                if (selectedImageIds.length) {
                                    scope.sfModel = selectedImageIds[0];
                                    getImage();
                                }
                            });
                    };

                    scope.editAllProperties = function () {
                        alert('TODO');
                    };
                }
            };
        }]);
})(jQuery);
