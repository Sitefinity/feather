(function ($) {
    angular.module('sfFields').requires.push('sfFormField');
    angular.module('sfFormField', ['sfServices', 'sfFormSelector'])
        .directive('sfFormField', ['serverContext', function (serverContext) {
            return {
                restrict: "AE",
                scope: {
                    sfModel: '=',
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/fields/form-field/sf-form-field.sf-cshtml';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    scope.selectedItems = [{ Id: scope.sfModel }];
                    scope.clearSearch = false;
                    scope.filterObj = {};

                    scope.narrowResults = function (query) {
                        scope.filterObj.search = query;
                    };

                    scope.$watch('selectedItems', function (newVal) {
                        if (newVal && newVal.length) {
                            newVal = newVal[0] || newVal;
                            scope.sfModel = newVal.Id;
                        }
                    });
                }
            };
        }]);
})(jQuery);
