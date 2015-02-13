(function ($) {
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
                    else {
                        scope.changeImage();
                    }

                    scope.selectedItemIds = [];

                    scope.changeImage = function () {
                        angular.element('.uploadPropertiesModal').scope().$openModalDialog();
                    };

                    scope.editAllProperties = function () {
                        alert('TODO');
                    };

                    scope.done = function () {
                        angular.element('.uploadPropertiesModal').scope().$modalInstance.close();

                        if (scope.selectedItemIds.length) {
                            scope.sfModel = scope.selectedItemIds[0];
                            getImage();
                        }
                    };

                    scope.cancel = function () {
                        scope.selectedItemIds = [];
                    };
                }
            };
        }]);
})(jQuery);
