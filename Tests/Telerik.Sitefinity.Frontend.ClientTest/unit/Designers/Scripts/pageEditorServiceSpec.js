/* tests for the pageEditorServices */
describe('pageEditorServices tests.', function () {

    beforeEach(module('pageEditorServices'));
    var $httpBackend;

    beforeEach(inject(function ($injector) {
        // Set up the mock http service responses
        $httpBackend = $injector.get('$httpBackend');

    }));

    it('[EGaneva] / Ensure the properties are initially requested.', inject(function (propertyService) {
        $httpBackend.expectGET('/Sitefinity/Services/Pages/ControlPropertyService.svc/902033f3-aceb-6a2e-a9a3-ff0000ffeb85/');
        propertyService.get();
    }));

    it('[EGaneva] / Ensure put call is executed.', inject(function (propertyService) {
        $httpBackend.expectPUT('/Sitefinity/Services/Pages/ControlPropertyService.svc/batch/902033f3-aceb-6a2e-a9a3-ff0000ffeb85/?pageId=111033f3-aceb-6a2e-a9a3-ff0000ffeb85&mediaType=Page');
        propertyService.save();
    }));

});