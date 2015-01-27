/* Tests for sf-image-service.js */
describe('sfImageService', function () {
    var $httpBackend;
    var imageService;
    var sampleGuid = '1ac3b615-0ce5-46dc-a0af-5c5d1f146df9';

    var dataItems = {
        Items: [{
            Id: '4a003fb0-2a77-61ec-be54-ff00007864f4'
        }],
        TotalCount: 1
    };

    var errorResponse = {
        Detail: 'Error'
    };

    var appPath = 'http://mysite.com:9999/myapp';
    var imageServicePath = appPath + '/Sitefinity/Services/Content/ImageService.svc/';
    var albumServicePath = appPath + '/Sitefinity/Services/Content/AlbumService.svc/folders/';

    beforeEach(module('sfServices'));

    beforeEach(module(function ($provide) {
        var serverContext = {
            getRootedUrl: function (path) {
                return appPath + '/' + path;
            },
            getUICulture: function () {
                return null;
            }
        };
        $provide.value('serverContext', serverContext);
    }));

    beforeEach(inject(function ($injector) {
        // Set up the mock http service responses
        $httpBackend = $injector.get('$httpBackend');
        imageService = $injector.get('sfImageService');
    }));

    /* Helper methods */
    var assertImages = function (params) {
        var data;
        imageService.getImages.apply(imageService, params).then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualData(dataItems);
    }

    var assertFolders = function (params) {
        var data;
        imageService.getFolders.apply(imageService, params).then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualData(dataItems);
    }

    var assertContent = function (params) {
        var data;
        imageService.getContent.apply(imageService, params).then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualData(dataItems);
    }

    var assertError = function (params) {
        var data;
        imageService.getItem.apply(imageService, params).then(function (res) {
            data = res;
        }, function (err) {
            data = err;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data.data).toEqualData(errorResponse);
    };

    /* Tests */

    // Folders
    it('[dzhenko] / should return only root folders', function () {
        $httpBackend.expectGET(albumServicePath).respond(dataItems);

        assertFolders({parent : null, recursive : false});
    });

    it('[dzhenko] / should return all existing folders', function () {
        $httpBackend.expectGET(albumServicePath).respond(dataItems);

        assertFolders({ parent: null, recursive: true });
    });

    it('[dzhenko] / should return only child folders', function () {
        var subpath = sampleGuid + '/?hierarchyMode=true';

        $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

        assertFolders({ parent: sampleGuid });
    });
});