/* tests for the designer module */
describe('designer tests.', function () {
    beforeEach(module('designer'));

    describe('DialogCtrl test.', function () {
        var createController, $rootScope, $scope, propertyService, $httpBackend, $modalInstance, $timeout;

        beforeEach(inject(function ($injector, _$location_, _$timeout_) {

            $rootScope = $injector.get('$rootScope');
            $scope = $rootScope.$new();
            _$location_.path('/PropertyGrid');
            $timeout = _$timeout_;

            var $controller = $injector.get('$controller');
            var $q = $injector.get('$q');
            $modalInstance = $injector.get('modalInstanceMock');

            var $route = $injector.get('$route');
            var propertyService = $injector.get('PropertyServiceMock');
            var widgetContext = sitefinity.pageEditor.widgetContext;

            $httpBackend = $injector.get('$httpBackend');
            $httpBackend.when('GET', '/Telerik.Sitefinity.Frontend/Designer/View/MockedWidget/PropertyGrid?controlId={2}')
                .respond('This is my property grid view response.');

            createController = function () {
                return $controller('DialogCtrl', {
                    '$rootScope': $rootScope,
                    '$scope': $scope,
                    '$q': $q,
                    '$modalInstance': $modalInstance,
                    '$route': $route,
                    'propertyService': propertyService,
                    'widgetContext': widgetContext
                });
            };
        }));

        it('[Boyko-Karadzhov] / Saving handler is called and dialog is closed.', function () {
            $httpBackend.expectGET('/Telerik.Sitefinity.Frontend/Designer/View/MockedWidget/PropertyGrid?controlId={2}');

            var controller = createController();

            var handlerCalled = false;
            $scope.feedback.savingHandlers.push(function () { handlerCalled = true; });
            $scope.save();
            
            $scope.$digest();

            expect(handlerCalled).toBe(true);
            expect($modalInstance.isClosed).toBe(true);
        });

        it('[Boyko-Karadzhov] / Saving handler that throws error is called and dialog is not closed.', function () {
            $httpBackend.expectGET('/Telerik.Sitefinity.Frontend/Designer/View/MockedWidget/PropertyGrid?controlId={2}');

            var expectedException = 'Expected exception';
            var controller = createController();

            var handlerCalled = false;
            $scope.feedback.savingHandlers.push(function () { handlerCalled = true; throw expectedException; });
            $scope.save();

            expect(function () { $scope.$digest(); }).toThrow(expectedException);
            $rootScope.$$phase = null;
            $timeout.flush(1);

            expect(handlerCalled).toBe(true);
            expect($scope.feedback.showError).toBe(true);
            expect($scope.feedback.errorMessage).toEqual(expectedException);
            expect($modalInstance.isClosed).not.toBe(true);
        });

        it('[Boyko-Karadzhov] / Canceling handler is called and dialog is closed.', function () {
            $httpBackend.expectGET('/Telerik.Sitefinity.Frontend/Designer/View/MockedWidget/PropertyGrid?controlId={2}');

            var controller = createController();

            var handlerCalled = false;
            $scope.feedback.cancelingHandlers.push(function () { handlerCalled = true; });
            $scope.cancel();

            $scope.$digest();

            expect(handlerCalled).toBe(true);
            expect($modalInstance.isClosed).toBe(true);
        });

        it('[Boyko-Karadzhov] / Canceling handler that throws error is called and dialog is not closed.', function () {
            $httpBackend.expectGET('/Telerik.Sitefinity.Frontend/Designer/View/MockedWidget/PropertyGrid?controlId={2}');

            var expectedException = 'Expected exception';
            var controller = createController();

            var handlerCalled = false;
            $scope.feedback.cancelingHandlers.push(function () { handlerCalled = true; throw expectedException; });
            $scope.cancel();

            expect(function () { $scope.$digest(); }).toThrow(expectedException);
            $rootScope.$$phase = null;
            $timeout.flush(1);

            expect(handlerCalled).toBe(true);
            expect($scope.feedback.showError).toBe(true);
            expect($scope.feedback.errorMessage).toEqual(expectedException);
            expect($modalInstance.isClosed).not.toBe(true);
        });

        it('[Boyko-Karadzhov] / Can save and close after one failed attempt.', function () {
            $httpBackend.expectGET('/Telerik.Sitefinity.Frontend/Designer/View/MockedWidget/PropertyGrid?controlId={2}');

            var expectedException = 'Expected exception';
            var controller = createController();

            var handlerCalled = false;
            $scope.feedback.savingHandlers.push(function () { if (!handlerCalled) { handlerCalled = true; throw expectedException; } });
            $scope.save();

            expect(function () { $scope.$digest(); }).toThrow(expectedException);
            $rootScope.$$phase = null;
            $timeout.flush(1);

            expect(handlerCalled).toBe(true);
            expect($scope.feedback.showError).toBe(true);
            expect($scope.feedback.errorMessage).toEqual(expectedException);
            expect($modalInstance.isClosed).not.toBe(true);

            $scope.save();

            $scope.$digest();

            expect(handlerCalled).toBe(true);
            expect($modalInstance.isClosed).toBe(true);
        });
    });
});
