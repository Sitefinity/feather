describe('sfFilesUrlService', function () {
    var $httpBackend;
    var fileUrlService;
    var $interpolate;

    beforeEach(module('sfServices'));

    var appPath = 'http://mysite.com:9999';
    var serviceBaseUrl = 'http://mysite.com:9999/RestApi/files-api';

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
        $httpBackend = $injector.get('$httpBackend');
        fileUrlService = $injector.get('sfFileUrlService');
        $interpolate = $injector.get('$interpolate');
    }));

    (function urlBuildingTests() {
        var unusedItems = [];

        var testUrlBuilding = function (expectedPathSuffix, extension, path, skip, take) {
            var response;
            fileUrlService.get(extension, path, skip, take).then(function (data) {
                response = data;
            });

            $httpBackend.expectGET(serviceBaseUrl + expectedPathSuffix).respond({ Items: unusedItems });

            expect(response).toBeUndefined();

            $httpBackend.flush();

            expect(response).toEqual(unusedItems);
        };

        describe('url building', function () {
            it('[dzhenko] / passing only extension should only include extension in request url', function () {
                ['css', 'js', 'html'].forEach(function (ext) { testUrlBuilding('?extension=' + ext, ext); });
            });
            it('[dzhenko] / passing parent path attaches it to the request', function () {
                testUrlBuilding('?extension=css&path=somePath', 'css', 'somePath');
            });
            it('[dzhenko] / passing skip attaches it to the request', function () {
                testUrlBuilding('?extension=css&skip=1', 'css', null, 1);
            });
            it('[dzhenko] / passing take attaches it to the request', function () {
                testUrlBuilding('?extension=css&take=1', 'css', null, null, 1);
            });
            it('[dzhenko] / passing skip and take attaches it to the request', function () {
                testUrlBuilding('?extension=css&skip=1&take=1', 'css', null, 1, 1);
            });
            it('[dzhenko] / passing all params builds correct url', function () {
                testUrlBuilding('?extension=css&path=somePath&skip=1&take=1', 'css', 'somePath', 1, 1);
            });
        });
    }());

    describe('items transformation', function () {
        it('[dzhenko] / getting sample items should transform them correctly', function () {
            var response;
            fileUrlService.get('css').then(function (data) {
                response = data;
            });

            $httpBackend.expectGET(serviceBaseUrl + '?extension=css').respond({
                Items: [
                    { Name: 'Folder1', IsFolder: true },
                    { Name: 'Folder2', IsFolder: true },
                    { Name: 'Folder3', IsFolder: true },
                    { Name: 'Item1.css', IsFolder: false },
                    { Name: 'Item2.css', IsFolder: false }
                ]
            });

            expect(response).toBeUndefined();

            $httpBackend.flush();

            expect(response).toEqual([
                { label: 'Folder1', path: '', url: '~/Folder1', isFolder: true, extension: null },
                { label: 'Folder2', path: '', url: '~/Folder2', isFolder: true, extension: null },
                { label: 'Folder3', path: '', url: '~/Folder3', isFolder: true, extension: null },
                { label: 'Item1.css', path: '', url: '~/Item1.css', isFolder: false, extension: 'css' },
                { label: 'Item2.css', path: '', url: '~/Item2.css', isFolder: false, extension: 'css' }
            ]);
        });
    });
});