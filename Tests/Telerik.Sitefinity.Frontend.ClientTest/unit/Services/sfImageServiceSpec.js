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
        imageService.getImages(params).then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualData(dataItems);
    };

    var assertFolders = function (params) {
        var data;
        imageService.getFolders(params).then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualData(dataItems);
    };

    var assertContent = function (params) {
        var data;
        imageService.getContent(params).then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualData(dataItems);
    };

    var assertError = function (params, methodName) {
        var data;
        imageService[methodName](params).then(function (res) {
            data = res;
        }, function (err) {
            data = err;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data.data).toEqualData(errorResponse);
    };

    /* Tests */

    /* Common */
    (function () {
        it('[dzhenko] / passing no options object to folders should return all objects', function () {
            var subpath = '?hierarchyMode=true'

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders();
        });

        it('[dzhenko] / passing no options object to images should return all objects', function () {
            var subpath = '?excludeFolders=true&itemType=Telerik.Sitefinity.Libraries.Model.Image'

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertImages();
        });

        it('[dzhenko] / passing no options object to content should return all objects', function () {
            var subpath = '?itemType=Telerik.Sitefinity.Libraries.Model.Image'

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertContent();
        });
    }());

    /* Errors */
    (function () {
        it('[dzhenko] / should return error on folders', function () {
            var subpath = '?hierarchyMode=true'

            $httpBackend.expectGET(albumServicePath + subpath).respond(500, errorResponse);

            assertError(null, 'getFolders');
        });

        it('[dzhenko] / should return error on images', function () {
            var subpath = '?excludeFolders=true&itemType=Telerik.Sitefinity.Libraries.Model.Image'

            $httpBackend.expectGET(imageServicePath + subpath).respond(500, errorResponse);

            assertError(null, 'getImages');
        });

        it('[dzhenko] / should return error on content', function () {
            var subpath = '?itemType=Telerik.Sitefinity.Libraries.Model.Image'

            $httpBackend.expectGET(imageServicePath + subpath).respond(500, errorResponse);

            assertError(null, 'getContent');
        });
    }());

    /* Folders */
    (function () {
        // Root folders
        it('[dzhenko] / should return only root folders', function () {
            var subpath = '?hierarchyMode=true';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders();
        });

        it('[dzhenko] / should return only 1 root folder', function () {
            var subpath = '?hierarchyMode=true&take=1';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: null, recursive: false, take: 1 });
        });

        it('[dzhenko] / should skip 1 root folder', function () {
            var subpath = '?hierarchyMode=true&skip=1';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: null, recursive: false, skip: 1 });
        });

        it('[dzhenko] / should skip 1 and return only 1 root folder', function () {
            var subpath = '?hierarchyMode=true&skip=1&take=1';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: null, recursive: false, skip: 1, take: 1 });
        });

        it('[dzhenko] / should use custom provider with root folders', function () {
            var subpath = '?hierarchyMode=true&provider=FakeDataProvider';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: null, recursive: false, provider: 'FakeDataProvider' });
        });

        it('[dzhenko] / should use filter with root folders', function () {
            var subpath = '?filter=FakeFilterExpression&hierarchyMode=true';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: null, recursive: false, filter: 'FakeFilterExpression' });
        });

        it('[dzhenko] / should use sort expression with root folders', function () {
            var subpath = '?hierarchyMode=true&sortExpression=FakeSortExpression';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: null, recursive: false, sort: 'FakeSortExpression' });
        });

        it('[dzhenko] / should use sort and filter expression with root folders', function () {
            var subpath = '?filter=FakeFilterExpression&hierarchyMode=true&sortExpression=FakeSortExpression';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: null, recursive: false, sort: 'FakeSortExpression', filter: 'FakeFilterExpression' });
        });

        it('[dzhenko] / should use skip, take and provider with root folders', function () {
            var subpath = '?hierarchyMode=true&provider=FakeDataProvider&skip=1&take=1';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: null, recursive: false, provider: 'FakeDataProvider', skip: 1, take: 1 });
        });

        it('[dzhenko] / should use all options with root folders', function () {
            var subpath = '?filter=FakeFilterExpression&hierarchyMode=true&provider=FakeDataProvider&skip=1&sortExpression=FakeSortExpression&take=1';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: null, recursive: false, filter: 'FakeFilterExpression', sort: 'FakeSortExpression', provider: 'FakeDataProvider', skip: 1, take: 1 });
        });

        // All folders
        it('[dzhenko] / should return all existing folders', function () {
            $httpBackend.expectGET(albumServicePath).respond(dataItems);

            assertFolders({ parent: null, recursive: true });
        });

        it('[dzhenko] / should return only 1 folder', function () {
            var subpath = '?take=1';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: null, recursive: true, take: 1 });
        });

        it('[dzhenko] / should skip 1 folder', function () {
            var subpath = '?skip=1';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: null, recursive: true, skip: 1 });
        });

        it('[dzhenko] / should skip 1 and return only 1  folder', function () {
            var subpath = '?skip=1&take=1';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: null, recursive: true, skip: 1, take: 1 });
        });

        it('[dzhenko] / should use custom provider with all folders', function () {
            var subpath = '?provider=FakeDataProvider';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: null, recursive: true, provider: 'FakeDataProvider' });
        });

        it('[dzhenko] / should use filter with all folders', function () {
            var subpath = '?filter=FakeFilterExpression';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: null, recursive: true, filter: 'FakeFilterExpression' });
        });

        it('[dzhenko] / should use sort expression with all folders', function () {
            var subpath = '?sortExpression=FakeSortExpression';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: null, recursive: true, sort: 'FakeSortExpression' });
        });

        it('[dzhenko] / should use sort and filter expression with all folders', function () {
            var subpath = '?filter=FakeFilterExpression&sortExpression=FakeSortExpression';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: null, recursive: true, sort: 'FakeSortExpression', filter: 'FakeFilterExpression' });
        });

        it('[dzhenko] / should use skip, take and provider with all folders', function () {
            var subpath = '?provider=FakeDataProvider&skip=1&take=1';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: null, recursive: true, provider: 'FakeDataProvider', skip: 1, take: 1 });
        });

        it('[dzhenko] / should use all options with all folders', function () {
            var subpath = '?filter=FakeFilterExpression&provider=FakeDataProvider&skip=1&sortExpression=FakeSortExpression&take=1';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: null, recursive: true, filter: 'FakeFilterExpression', sort: 'FakeSortExpression', provider: 'FakeDataProvider', skip: 1, take: 1 });
        });

        // Child folders
        it('[dzhenko] / should return only child folders', function () {
            var subpath = sampleGuid + '/?hierarchyMode=true';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: sampleGuid, recursive: false });
        });

        it('[dzhenko] / should return only 1 child folder', function () {
            var subpath = sampleGuid + '/?hierarchyMode=true&take=1';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: sampleGuid, recursive: false, take: 1 });
        });

        it('[dzhenko] / should skip 1 child folder', function () {
            var subpath = sampleGuid + '/?hierarchyMode=true&skip=1';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: sampleGuid, recursive: false, skip: 1 });
        });

        it('[dzhenko] / should skip 1 and return only 1 child folder', function () {
            var subpath = sampleGuid + '/?hierarchyMode=true&skip=1&take=1';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: sampleGuid, recursive: false, skip: 1, take: 1 });
        });

        it('[dzhenko] / should use custom provider with child folders', function () {
            var subpath = sampleGuid + '/?hierarchyMode=true&provider=FakeDataProvider';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: sampleGuid, recursive: false, provider: 'FakeDataProvider' });
        });

        it('[dzhenko] / should use filter with child folders', function () {
            var subpath = sampleGuid + '/?filter=FakeFilterExpression&hierarchyMode=true';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: sampleGuid, recursive: false, filter: 'FakeFilterExpression' });
        });

        it('[dzhenko] / should use sort expression with child folders', function () {
            var subpath = sampleGuid + '/?hierarchyMode=true&sortExpression=FakeSortExpression';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: sampleGuid, recursive: false, sort: 'FakeSortExpression' });
        });

        it('[dzhenko] / should use sort and filter expression with child folders', function () {
            var subpath = sampleGuid + '/?filter=FakeFilterExpression&hierarchyMode=true&sortExpression=FakeSortExpression';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: sampleGuid, recursive: false, sort: 'FakeSortExpression', filter: 'FakeFilterExpression' });
        });

        it('[dzhenko] / should use skip, take and provider with child folders', function () {
            var subpath = sampleGuid + '/?hierarchyMode=true&provider=FakeDataProvider&skip=1&take=1';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: sampleGuid, recursive: false, provider: 'FakeDataProvider', skip: 1, take: 1 });
        });

        it('[dzhenko] / should use all options with child folders', function () {
            var subpath = sampleGuid + '/?filter=FakeFilterExpression&hierarchyMode=true&provider=FakeDataProvider&skip=1&sortExpression=FakeSortExpression&take=1';

            $httpBackend.expectGET(albumServicePath + subpath).respond(dataItems);

            assertFolders({ parent: sampleGuid, recursive: false, filter: 'FakeFilterExpression', sort: 'FakeSortExpression', provider: 'FakeDataProvider', skip: 1, take: 1 });
        });
    }());

    /* Images */
    (function () {
        it('[dzhenko] / should return all images', function () {
            var subpath = '?excludeFolders=true&itemType=Telerik.Sitefinity.Libraries.Model.Image';

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertImages();
        });

        it('[dzhenko] / should return recent images', function () {
            var subpath = '?excludeFolders=true&filter=(LastModified%3E(Sun,+25+Jan+2015+14:09:21+GMT))&itemType=Telerik.Sitefinity.Libraries.Model.Image';

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertImages({ filter: '(LastModified>(Sun, 25 Jan 2015 14:09:21 GMT))' });
        });

        it('[dzhenko] / should return own images', function () {
            var subpath = '?excludeFolders=true&filter=Owner+%3D%3D+(67152310-c838-6bcd-855b-ff0000c292fc)&itemType=Telerik.Sitefinity.Libraries.Model.Image';

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertImages({ filter: 'Owner == (67152310-c838-6bcd-855b-ff0000c292fc)' });
        });

        it('[dzhenko] / should return images from folder and owner', function () {
            var subpath = sampleGuid + '/?excludeFolders=true&filter=Owner+%3D%3D+(67152310-c838-6bcd-855b-ff0000c292fc)&itemType=Telerik.Sitefinity.Libraries.Model.Image';

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertImages({ parent : sampleGuid, filter: 'Owner == (67152310-c838-6bcd-855b-ff0000c292fc)' });
        });

        it('[dzhenko] / should return only 1 image', function () {
            var subpath = '?excludeFolders=true&itemType=Telerik.Sitefinity.Libraries.Model.Image&take=1';

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertImages({ take: 1 });
        });

        it('[dzhenko] / should skip 1 image', function () {
            var subpath = '?excludeFolders=true&itemType=Telerik.Sitefinity.Libraries.Model.Image&skip=1';

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertImages({ skip: 1 });
        });

        it('[dzhenko] / should skip 1 and return only 1 image', function () {
            var subpath = '?excludeFolders=true&itemType=Telerik.Sitefinity.Libraries.Model.Image&skip=1&take=1';

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertImages({ skip : 1, take: 1 });
        });

        it('[dzhenko] / should use custom provider with all images', function () {
            var subpath = '?excludeFolders=true&itemType=Telerik.Sitefinity.Libraries.Model.Image&provider=FakeDataProvider';

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertImages({ provider: 'FakeDataProvider' });
        });

        it('[dzhenko] / should use sort expression with all images', function () {
            var subpath = '?excludeFolders=true&itemType=Telerik.Sitefinity.Libraries.Model.Image&sortExpression=FakeSortExpression';

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertImages({ sort: 'FakeSortExpression' });
        });

        it('[dzhenko] / should use sort and filter expression with images', function () {
            var subpath = '?excludeFolders=true&filter=FakeFilterExpression&itemType=Telerik.Sitefinity.Libraries.Model.Image&sortExpression=FakeSortExpression';

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertImages({ sort: 'FakeSortExpression', filter: 'FakeFilterExpression' });
        });

        it('[dzhenko] / should use skip, take and provider with images', function () {
            var subpath = '?excludeFolders=true&itemType=Telerik.Sitefinity.Libraries.Model.Image&provider=FakeDataProvider&skip=1&take=1';

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertImages({ provider: 'FakeDataProvider', skip: 1, take: 1 });
        });

        it('[dzhenko] / should use all options with images', function () {
            var subpath = '?excludeFolders=true&filter=FakeFilterExpression&itemType=Telerik.Sitefinity.Libraries.Model.Image&provider=FakeDataProvider&skip=1&sortExpression=FakeSortExpression&take=1';

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertImages({ filter: 'FakeFilterExpression', sort: 'FakeSortExpression', provider: 'FakeDataProvider', skip: 1, take: 1 });
        });
    }());

    /* Content */
    (function () {
        it('[dzhenko] / should return all content', function () {
            var subpath = '?itemType=Telerik.Sitefinity.Libraries.Model.Image';

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertContent();
        });

        it('[dzhenko] / should return recent content', function () {
            var subpath = '?filter=(LastModified%3E(Sun,+25+Jan+2015+14:09:21+GMT))&itemType=Telerik.Sitefinity.Libraries.Model.Image';

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertContent({ filter: '(LastModified>(Sun, 25 Jan 2015 14:09:21 GMT))' });
        });

        it('[dzhenko] / should return own content', function () {
            var subpath = '?filter=Owner+%3D%3D+(67152310-c838-6bcd-855b-ff0000c292fc)&itemType=Telerik.Sitefinity.Libraries.Model.Image';

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertContent({ filter: 'Owner == (67152310-c838-6bcd-855b-ff0000c292fc)' });
        });

        it('[dzhenko] / should return content from folder and owner', function () {
            var subpath = sampleGuid + '/?filter=Owner+%3D%3D+(67152310-c838-6bcd-855b-ff0000c292fc)&itemType=Telerik.Sitefinity.Libraries.Model.Image';

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertContent({ parent: sampleGuid, filter: 'Owner == (67152310-c838-6bcd-855b-ff0000c292fc)' });
        });

        it('[dzhenko] / should return only 1 content', function () {
            var subpath = '?itemType=Telerik.Sitefinity.Libraries.Model.Image&take=1';

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertContent({ take: 1 });
        });

        it('[dzhenko] / should skip 1 content', function () {
            var subpath = '?itemType=Telerik.Sitefinity.Libraries.Model.Image&skip=1';

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertContent({ skip: 1 });
        });

        it('[dzhenko] / should skip 1 and return only 1 content', function () {
            var subpath = '?itemType=Telerik.Sitefinity.Libraries.Model.Image&skip=1&take=1';

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertContent({ skip: 1, take: 1 });
        });

        it('[dzhenko] / should use custom provider with all content', function () {
            var subpath = '?itemType=Telerik.Sitefinity.Libraries.Model.Image&provider=FakeDataProvider';

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertContent({ provider: 'FakeDataProvider' });
        });

        it('[dzhenko] / should use sort expression with all content', function () {
            var subpath = '?itemType=Telerik.Sitefinity.Libraries.Model.Image&sortExpression=FakeSortExpression';

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertContent({ sort: 'FakeSortExpression' });
        });

        it('[dzhenko] / should use sort and filter expression with content', function () {
            var subpath = '?filter=FakeFilterExpression&itemType=Telerik.Sitefinity.Libraries.Model.Image&sortExpression=FakeSortExpression';

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertContent({ sort: 'FakeSortExpression', filter: 'FakeFilterExpression' });
        });

        it('[dzhenko] / should use skip, take and provider with content', function () {
            var subpath = '?itemType=Telerik.Sitefinity.Libraries.Model.Image&provider=FakeDataProvider&skip=1&take=1';

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertContent({ provider: 'FakeDataProvider', skip: 1, take: 1 });
        });

        it('[dzhenko] / should use all options with content', function () {
            var subpath = '?filter=FakeFilterExpression&itemType=Telerik.Sitefinity.Libraries.Model.Image&provider=FakeDataProvider&skip=1&sortExpression=FakeSortExpression&take=1';

            $httpBackend.expectGET(imageServicePath + subpath).respond(dataItems);

            assertContent({ filter: 'FakeFilterExpression', sort: 'FakeSortExpression', provider: 'FakeDataProvider', skip: 1, take: 1 });
        });
    }());
});