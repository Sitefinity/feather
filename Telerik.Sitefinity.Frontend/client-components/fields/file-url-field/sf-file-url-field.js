; (function () {
    var sfFields = angular.module('sfFields');
    sfFields.requires.push('sfFileUrlField');

    angular.module('sfFileUrlField', ['sfServices', 'sfFileUrlSelector'])
        .directive('sfFileUrlField', ['serverContext', function (serverContext) {
            return {
                restrict: "AE",
                scope: {
                    sfModel: '=',
                    sfExtension: '@',
                    sfTitle: '@'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/fields/file-url-field/sf-file-url-field.sf-cshtml';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    scope.open = function () {
                        element.find('[data-sf-role="modal-container"]')
                                    .scope()
                                    .$openModalDialog({ title: function () { return scope.sfTitle; }, extension: function () { return scope.sfExtension; }, sfModel: function () { return scope.sfModel; } })
                                        .then(function (data) {
                                            scope.sfModel = data;
                                        });
                    };
                }
            };
        }])
		.directive('hasExtension', function ($parse) {
		    return {
		        require: 'ngModel',
		        link: function (scope, elm, attrs, ctrl) {
		            ctrl.$validators.hasExtension = function (modelValue, viewValue) {
		                if (ctrl.$isEmpty(modelValue)) {
		                    return true;
		                }

		                var extension = $parse(attrs.extension)(scope);
		                if (ctrl.$isEmpty(extension)) {
		                    return true;
		                }

		                if (viewValue.slice(-(extension.length + 1)) === '.' + extension) {
		                    return true;
		                }

		                return false;
		            };
		        }
		    };
		})
        .controller('sfFileUrlFieldDialogController', ['$scope', '$modalInstance', 'title', 'extension', 'sfModel', function ($scope, $modalInstance, title, extension, sfModel) {
            $scope.selectedUrl = sfModel;
            $scope.title = title;
            $scope.extension = extension;

            $scope.done = function () {
                $modalInstance.close($scope.selectedUrl);
            };

            $scope.cancel = function () {
                $modalInstance.dismiss();
            };
        }]);
})();
