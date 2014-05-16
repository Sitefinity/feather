angular.module('breadcrumb', []).factory('BreadCrumbService', function ($rootScope) {
    var breadCrumbElements = [];    

    var breadCrumbElement = function (propPath, propName) {
        this.PropertyPath = propPath,
        this.PropertyName = propName
    };

    return {
        //push the element if it is not already presented
        push: function (propPath, propName) {
            var lastElement = breadCrumbElements[breadCrumbElements.length - 1];
            if (!lastElement || lastElement.PropertyPath !== propPath)
                breadCrumbElements.push(new breadCrumbElement(propPath, propName));
        },
        //gets all breadcrumb elements
        getAll:function ()
        {
            return breadCrumbElements;
        },
        //remove all breadcrumbElements
        removeAll: function ()
        {
            breadCrumbElements = [];
        }
    };
}).directive('breadcrumb', function (BreadCrumbService) {
    return {
        restrict: 'A',
        template: '<ul class="breadcrumb"><li ng-repeat=\'bc in Breadcrumbs\' ng-class="{\'active\': {{$last}} }"><a ng-click="RefreshHierarchy(bc.PropertyPath, bc.PropertyName)" ng-bind="bc.PropertyName"></a></li></ul>',
        replace: true,
        link: function (scope, tElement, tAttrs) {

            //inspect whether the current view has been modified
            scope.$watch(tAttrs.ngModel, function (v) {
                addSingleBreadCrumbElement(v);
                $("#advancedProperties").scrollTop(0);
            });

            //adds single path element at the end of the breadcrumb.
            addSingleBreadCrumbElement = function (propPath) {
                if (propPath) {
                    if (BreadCrumbService.getAll().length===0)
                        BreadCrumbService.push("", "Home");

                    var containingProperties = propPath.split('/');
                    var propName = containingProperties[containingProperties.length - 1];
                    BreadCrumbService.push(propPath, propName);

                    var breadcrumbElement = $(".breadcrumb");
                    breadcrumbElement.show();
                }
                scope.Breadcrumbs = BreadCrumbService.getAll();
            };

            //generates all elements of the breadcrumb
            generateCurrentBreadCrumbElements = function (proeprtyPath, propName) {
                BreadCrumbService.removeAll();
                BreadCrumbService.push("", "Home");
                var breadcrumbElement = $(".breadcrumb");
                if (proeprtyPath) {
                    breadcrumbElement.show();
                    var containingProperties = proeprtyPath.split('/');
                    containingProperties.forEach(function (propName) {
                        if (propName) {
                            var propPath = proeprtyPath.substr(0, proeprtyPath.indexOf(propName)) + propName;
                            BreadCrumbService.push(propPath, propName);
                        }
                    });
                }
                else {
                    breadcrumbElement.hide();
                }
                return BreadCrumbService.getAll();
            }

            //initial populate of the breadcrumb elements
            scope.Breadcrumbs = generateCurrentBreadCrumbElements();

            scope.RefreshHierarchy = function (propertyPath, propertyName) {
                scope.propertyPath = propertyPath;
                scope.propertyName = propertyName;
                scope.Breadcrumbs = generateCurrentBreadCrumbElements(propertyPath, propertyName);
            };
        }
    };

});