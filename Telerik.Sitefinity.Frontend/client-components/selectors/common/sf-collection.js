; (function ($) {
    angular.module('sfSelectors')
        .directive('sfListSelector', ['$compile', '$q', 'serverContext', function ($compile, $q, serverContext) {
            return {
                restrict: 'A',
                link: link,
                scope: {
                    sfTemplate: '@',
                    sfTemplateUrl: '@',
                    sfMultiselect: '@',
                    sfData: '@',
                    ngModel: '=',
                    sfLoadMore: '&'
                },
                link: function (scope, element, attrs, ngModel) {
                    scope.selectItem = function (id) {
                        if (scope.selectItems) {
                            var itemIndex = scope.selectItems.indexOf(id);
                            if (itemIndex < 0) {
                                scope.selectItems.push(id);
                            }
                            else {
                                scope.selectItems.splice(itemIndex, 1);
                            }
                        }

                        ngModel.$setViewValue(scope.selectItems);
                        ngModel.$apply();
                    };

                    //$scope.$on("$destroy", function () {

                    //});

                    getHtmlTemplate().then(function (htmlTemplate) {
                        element.html(htmlTemplate);
                        $compile(element.contents())(scope);
                    });
                }
            };

            var getHtmlTemplate = function () {
                if (scope.sfTemplate && scope.sfTemplateUrl) {
                    throw { message: "You can not provide both template html and template url." }
                }
                if (!scope.sfTemplate && !scope.sfTemplateUrl) {
                    throw { message: "You must provide template html or template url." }
                }

                var deferred = $q.defer();

                if (scope.sfTemplate) {
                    deferred.resolve(scope.sfTemplate);
                }
                else {
                    var assembly = scope.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = scope.sfTemplateUrl || 'client-components/selectors/common/sf-list-selector.html';
                    var finalUrl = serverContext.getEmbeddedResourceUrl(assembly, url);

                    $.get(finalUrl).success(deferred.resolve);
                }

                return deferred.promise;
            }
        }]);
})(jQuery);
