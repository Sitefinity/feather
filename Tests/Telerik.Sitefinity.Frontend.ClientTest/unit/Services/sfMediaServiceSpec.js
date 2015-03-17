/* Tests for sf-media-service.js */
describe('sfMediaService', function () {
    var $httpBackend;
    var mediaService;
    var $interpolate;

    var dataItems = {
        Items: [{
            Id: '4a003fb0-2a77-61ec-be54-ff00007864f4',
            IsFolder: true
        }],
        TotalCount: 1
    };

    var mediaSectionSettings = {"Items": [
        {
            Key: "UrlRoot",
            Value: "images"
        },
        {
            Key: "AllowedExensionsSettings",
            Value: ".gif, .jpg, .jpeg, .png, .bmp"
        },
        {
            Key: "AllowDynamicResizing",
            Value: "True"
        },
        {
            Key: "StoreDynamicResizedImagesAsThumbnails",
            Value: "True"
        },
        {
            Key: "EnableImageUrlSignature",
            Value: "True"
        },
        {
            Key: "ImageUrlSignatureHashAlgorithm",
            Value: "SHA1"
        },
        {
            Key: "DynamicResizingThreadsCount",
            Value: "16"
        }],
        TotalCount: 7
    };

    var errorResponse = {
        Detail: 'Error'
    };

    var appPath = 'http://mysite.com:9999/myapp';
    var sampleGuid = '1ac3b615-0ce5-46dc-a0af-5c5d1f146df9';
    var uploadUrl = appPath + '/Telerik.Sitefinity.Html5UploadHandler.ashx';

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
        mediaService = $injector.get('sfMediaService');
        $interpolate = $injector.get('$interpolate');
    }));

    /* Helper methods */
    var baseAssertItems = function (params, callback) {
        var data;
        callback(params).then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualData(dataItems);
    };

    var baseAssertFolders = function (params, callback) {
        var data;
        callback(params).then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualData(dataItems);
    };

    var baseAssertContent = function (params, callback) {
        var data;
        callback(params).then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data).toEqualData(dataItems);
    };

    var baseAssertError = function (params, callback) {
        var data;
        callback(params).then(function (res) {
            data = res;
        }, function (err) {
            data = err;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        expect(data.data).toEqualData(errorResponse);
    };

    var baseAssertSettings = function (mediaType, callback) {
        var data;
        callback().then(function (res) {
            data = res;
        });

        expect(data).toBeUndefined();

        $httpBackend.flush();

        var expectedRegex = new RegExp('^' + mediaType + '\/(gif|jpg|jpeg|png|bmp)$', 'i');
        expect(data.AllowedExensionsRegex).toEqual(expectedRegex);

        delete data.AllowedExensionsRegex;

        expect(data).toEqualArrayOfObjects(mediaSectionSettings, ['Key', 'Value']);
    };

    /* Tested Services */
    var allTestObjectsSettings = [
        {
            itemType: 'Telerik.Sitefinity.Libraries.Model.Image',
            parentType: 'Telerik.Sitefinity.Libraries.Model.Album',
            itemsServicePath: appPath + '/Sitefinity/Services/Content/ImageService.svc/',
            albumsServicePath: appPath + '/Sitefinity/Services/Content/AlbumService.svc/',
            settingsNodeName: 'Images_0,librariesConfig_0',
            extensionsRegExPrefix: 'image',
            callbacks: {
                testedObject: 'images',
                folders: 'getFolders',
                items: 'getMedia',
                content: 'getContent',
                upload: 'upload',
                settings: 'getSettings'
            }
        },
        {
            itemType: 'Telerik.Sitefinity.Libraries.Model.Document',
            parentType: 'Telerik.Sitefinity.Libraries.Model.DocumentLibrary',
            itemsServicePath: appPath + '/Sitefinity/Services/Content/DocumentService.svc/',
            albumsServicePath: appPath + '/Sitefinity/Services/Content/DocumentLibraryService.svc/',
            settingsNodeName: 'Documents_0,librariesConfig_0',
            extensionsRegExPrefix: 'document',
            callbacks: {
                testedObject: 'documents',
                folders: 'getFolders',
                items: 'getMedia',
                content: 'getContent',
                upload: 'upload',
                settings: 'getSettings'
            }
        }
    ];

    /* Generic Tests */
    var runTest = function (testObjSettings) {
        /* Setup */
        var albumsServicePath = testObjSettings.albumsServicePath;
        var itemsServicePath = testObjSettings.itemsServicePath;
        var itemType = testObjSettings.itemType;
        var parentType = testObjSettings.parentType;
        var settingsNodeName = testObjSettings.settingsNodeName;
        var assertFolders,
            assertItems,
            assertContent,
            assertError,
            $window,
            $rootScope;

        beforeEach(inject(function ($injector, _$window_, _$rootScope_) {
            mediaService = $injector.get('sfMediaService');

            assertFolders = function (params) {
                baseAssertFolders(params, mediaService[testObjSettings.callbacks.testedObject][testObjSettings.callbacks.folders]);
            };

            assertItems = function (params) {
                baseAssertItems(params, mediaService[testObjSettings.callbacks.testedObject][testObjSettings.callbacks.items]);
            };

            assertContent = function (params) {
                baseAssertContent(params, mediaService[testObjSettings.callbacks.testedObject][testObjSettings.callbacks.content]);
            };

            assertError = function (params, methodName) {
                baseAssertError(params, mediaService[testObjSettings.callbacks.testedObject][methodName]);
            };

            assertSettings = function (testObjSettings) {
                baseAssertSettings(testObjSettings.extensionsRegExPrefix,
                    mediaService[testObjSettings.callbacks.testedObject][testObjSettings.callbacks.settings]);
            };

            $window = _$window_;
            $rootScope = _$rootScope_;
        }));

        /* Tests */

        /* Common */
        describe('common', function () {
            it('[dzhenko] / passing no options object to folders should return all objects', function () {
                var subpath = 'folders/?hierarchyMode=true';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders();
            });

            it('[dzhenko] / passing no options object to images should return all objects', function () {
                var subpath = '?excludeFolders=true&itemType=' + itemType;

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertItems();
            });

            it('[dzhenko] / passing no options object to content should return all objects', function () {
                var subpath = '?itemType=' + itemType;

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertContent();
            });
        });

        /* Errors */
        describe('errors', function () {
            it('[dzhenko] / should return error on folders', function () {
                var subpath = 'folders/?hierarchyMode=true';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(500, errorResponse);

                assertError(null, 'getFolders');
            });

            it('[dzhenko] / should return error on images', function () {
                var subpath = '?excludeFolders=true&itemType=' + itemType;

                $httpBackend.expectGET(itemsServicePath + subpath).respond(500, errorResponse);

                assertError(null, 'getMedia');
            });

            it('[dzhenko] / should return error on content', function () {
                var subpath = '?itemType=' + itemType;

                $httpBackend.expectGET(itemsServicePath + subpath).respond(500, errorResponse);

                assertError(null, 'getContent');
            });
        });

        /* Folders */
        describe('folders', function () {
            // Root folders
            it('[dzhenko] / should return only root folders', function () {
                var subpath = 'folders/?hierarchyMode=true';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders();
            });

            it('[dzhenko] / should return only 1 root folder', function () {
                var subpath = 'folders/?hierarchyMode=true&take=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: false, take: 1 });
            });

            it('[dzhenko] / should skip 1 root folder', function () {
                var subpath = 'folders/?hierarchyMode=true&skip=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: false, skip: 1 });
            });

            it('[dzhenko] / should skip 1 and return only 1 root folder', function () {
                var subpath = 'folders/?hierarchyMode=true&skip=1&take=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: false, skip: 1, take: 1 });
            });

            it('[dzhenko] / should use custom provider with root folders', function () {
                var subpath = 'folders/?hierarchyMode=true&provider=FakeDataProvider';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: false, provider: 'FakeDataProvider' });
            });

            it('[dzhenko] / should use filter with root folders', function () {
                var subpath = 'folders/?filter=FakeFilterExpression&hierarchyMode=true';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: false, filter: 'FakeFilterExpression' });
            });

            it('[dzhenko] / should use sort expression with root folders', function () {
                var subpath = 'folders/?hierarchyMode=true&sortExpression=FakeSortExpression';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: false, sort: 'FakeSortExpression' });
            });

            it('[dzhenko] / should use sort and filter expression with root folders', function () {
                var subpath = 'folders/?filter=FakeFilterExpression&hierarchyMode=true&sortExpression=FakeSortExpression';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: false, sort: 'FakeSortExpression', filter: 'FakeFilterExpression' });
            });

            it('[dzhenko] / should use skip, take and provider with root folders', function () {
                var subpath = 'folders/?hierarchyMode=true&provider=FakeDataProvider&skip=1&take=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: false, provider: 'FakeDataProvider', skip: 1, take: 1 });
            });

            it('[dzhenko] / should use all options with root folders', function () {
                var subpath = 'folders/?filter=FakeFilterExpression&hierarchyMode=true&provider=FakeDataProvider&skip=1&sortExpression=FakeSortExpression&take=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: false, filter: 'FakeFilterExpression', sort: 'FakeSortExpression', provider: 'FakeDataProvider', skip: 1, take: 1 });
            });

            // All folders
            it('[dzhenko] / should return all existing folders', function () {
                $httpBackend.expectGET(albumsServicePath + 'folders/').respond(dataItems);

                assertFolders({ parent: null, recursive: true });
            });

            it('[dzhenko] / should return only 1 folder', function () {
                var subpath = 'folders/?take=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: true, take: 1 });
            });

            it('[dzhenko] / should skip 1 folder', function () {
                var subpath = 'folders/?skip=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: true, skip: 1 });
            });

            it('[dzhenko] / should skip 1 and return only 1  folder', function () {
                var subpath = 'folders/?skip=1&take=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: true, skip: 1, take: 1 });
            });

            it('[dzhenko] / should use custom provider with all folders', function () {
                var subpath = 'folders/?provider=FakeDataProvider';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: true, provider: 'FakeDataProvider' });
            });

            it('[dzhenko] / should use filter with all folders', function () {
                var subpath = 'folders/?filter=FakeFilterExpression';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: true, filter: 'FakeFilterExpression' });
            });

            it('[dzhenko] / should use sort expression with all folders', function () {
                var subpath = 'folders/?sortExpression=FakeSortExpression';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: true, sort: 'FakeSortExpression' });
            });

            it('[dzhenko] / should use sort and filter expression with all folders', function () {
                var subpath = 'folders/?filter=FakeFilterExpression&sortExpression=FakeSortExpression';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: true, sort: 'FakeSortExpression', filter: 'FakeFilterExpression' });
            });

            it('[dzhenko] / should use skip, take and provider with all folders', function () {
                var subpath = 'folders/?provider=FakeDataProvider&skip=1&take=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: true, provider: 'FakeDataProvider', skip: 1, take: 1 });
            });

            it('[dzhenko] / should use all options with all folders', function () {
                var subpath = 'folders/?filter=FakeFilterExpression&provider=FakeDataProvider&skip=1&sortExpression=FakeSortExpression&take=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: true, filter: 'FakeFilterExpression', sort: 'FakeSortExpression', provider: 'FakeDataProvider', skip: 1, take: 1 });
            });

            // Child folders
            it('[dzhenko] / should return only child folders', function () {
                var subpath = 'folders/' + sampleGuid + '/?hierarchyMode=true';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: sampleGuid, recursive: false });
            });

            it('[dzhenko] / should return only 1 child folder', function () {
                var subpath = 'folders/' + sampleGuid + '/?hierarchyMode=true&take=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: sampleGuid, recursive: false, take: 1 });
            });

            it('[dzhenko] / should skip 1 child folder', function () {
                var subpath = 'folders/' + sampleGuid + '/?hierarchyMode=true&skip=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: sampleGuid, recursive: false, skip: 1 });
            });

            it('[dzhenko] / should skip 1 and return only 1 child folder', function () {
                var subpath = 'folders/' + sampleGuid + '/?hierarchyMode=true&skip=1&take=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: sampleGuid, recursive: false, skip: 1, take: 1 });
            });

            it('[dzhenko] / should use custom provider with child folders', function () {
                var subpath = 'folders/' + sampleGuid + '/?hierarchyMode=true&provider=FakeDataProvider';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: sampleGuid, recursive: false, provider: 'FakeDataProvider' });
            });

            it('[dzhenko] / should use filter with child folders', function () {
                var subpath = 'folders/' + sampleGuid + '/?filter=FakeFilterExpression&hierarchyMode=true';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: sampleGuid, recursive: false, filter: 'FakeFilterExpression' });
            });

            it('[dzhenko] / should use sort expression with child folders', function () {
                var subpath = 'folders/' + sampleGuid + '/?hierarchyMode=true&sortExpression=FakeSortExpression';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: sampleGuid, recursive: false, sort: 'FakeSortExpression' });
            });

            it('[dzhenko] / should use sort and filter expression with child folders', function () {
                var subpath = 'folders/' + sampleGuid + '/?filter=FakeFilterExpression&hierarchyMode=true&sortExpression=FakeSortExpression';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: sampleGuid, recursive: false, sort: 'FakeSortExpression', filter: 'FakeFilterExpression' });
            });

            it('[dzhenko] / should use skip, take and provider with child folders', function () {
                var subpath = 'folders/' + sampleGuid + '/?hierarchyMode=true&provider=FakeDataProvider&skip=1&take=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: sampleGuid, recursive: false, provider: 'FakeDataProvider', skip: 1, take: 1 });
            });

            it('[dzhenko] / should use all options with child folders', function () {
                var subpath = 'folders/' + sampleGuid + '/?filter=FakeFilterExpression&hierarchyMode=true&provider=FakeDataProvider&skip=1&sortExpression=FakeSortExpression&take=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: sampleGuid, recursive: false, filter: 'FakeFilterExpression', sort: 'FakeSortExpression', provider: 'FakeDataProvider', skip: 1, take: 1 });
            });
        });

        /* Media items */
        describe('media items', function () {

            var getXmlHttpRequestMock = function (window) {
                window.XMLHttpRequest = angular.noop;

                var onprogressSpy = jasmine.createSpy("onprogress");
                var openSpy = jasmine.createSpy("open");
                var sendSpy = jasmine.createSpy("send");

                var xhrObj = {
                    upload: {
                        onprogress: onprogressSpy
                    },
                    open: openSpy,
                    send: sendSpy,
                };

                spyOn(window, "XMLHttpRequest").andReturn(xhrObj);

                return xhrObj;
            };

            it('[dzhenko] / should return all images', function () {
                var subpath = '?excludeFolders=true&itemType=' + itemType;

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertItems();
            });

            it('[dzhenko] / should return recent images', function () {
                var subpath = '?excludeFolders=true&filter=(LastModified%3E(Sun,+25+Jan+2015+14:09:21+GMT))&itemType=' + itemType;

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertItems({ filter: '(LastModified>(Sun, 25 Jan 2015 14:09:21 GMT))' });
            });

            it('[dzhenko] / should return own images', function () {
                var subpath = '?excludeFolders=true&filter=Owner+%3D%3D+(67152310-c838-6bcd-855b-ff0000c292fc)&itemType=' + itemType;

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertItems({ filter: 'Owner == (67152310-c838-6bcd-855b-ff0000c292fc)' });
            });

            it('[dzhenko] / should return images from folder and owner', function () {
                var subpath = 'parent/' + sampleGuid + '/?excludeFolders=true&filter=Owner+%3D%3D+(67152310-c838-6bcd-855b-ff0000c292fc)&itemType=' + itemType;

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertItems({ parent: sampleGuid, filter: 'Owner == (67152310-c838-6bcd-855b-ff0000c292fc)' });
            });

            it('[dzhenko] / should return only 1 image', function () {
                var subpath = '?excludeFolders=true&itemType=' + itemType + '&take=1';

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertItems({ take: 1 });
            });

            it('[dzhenko] / should skip 1 image', function () {
                var subpath = '?excludeFolders=true&itemType=' + itemType + '&skip=1';

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertItems({ skip: 1 });
            });

            it('[dzhenko] / should skip 1 and return only 1 image', function () {
                var subpath = '?excludeFolders=true&itemType=' + itemType + '&skip=1&take=1';

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertItems({ skip: 1, take: 1 });
            });

            it('[dzhenko] / should use custom provider with all images', function () {
                var subpath = '?excludeFolders=true&itemType=' + itemType + '&provider=FakeDataProvider';

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertItems({ provider: 'FakeDataProvider' });
            });

            it('[dzhenko] / should use sort expression with all images', function () {
                var subpath = '?excludeFolders=true&itemType=' + itemType + '&sortExpression=FakeSortExpression';

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertItems({ sort: 'FakeSortExpression' });
            });

            it('[dzhenko] / should use sort and filter expression with images', function () {
                var subpath = '?excludeFolders=true&filter=FakeFilterExpression&itemType=' + itemType + '&sortExpression=FakeSortExpression';

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertItems({ sort: 'FakeSortExpression', filter: 'FakeFilterExpression' });
            });

            it('[dzhenko] / should use skip, take and provider with images', function () {
                var subpath = '?excludeFolders=true&itemType=' + itemType + '&provider=FakeDataProvider&skip=1&take=1';

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertItems({ provider: 'FakeDataProvider', skip: 1, take: 1 });
            });

            it('[dzhenko] / should use all options with images', function () {
                var subpath = '?excludeFolders=true&filter=FakeFilterExpression&itemType=' + itemType + '&provider=FakeDataProvider&skip=1&sortExpression=FakeSortExpression&take=1';

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertItems({ filter: 'FakeFilterExpression', sort: 'FakeSortExpression', provider: 'FakeDataProvider', skip: 1, take: 1 });
            });

            it('[Boyko-Karadzhov] / should issue a PUT request to create a media item on upload and upload the file to it.', function () {
                var settings = {
                    parentId: 'myLibraryId',
                    itemId: '00000000-0000-0000-0000-000000000000',
                    itemType: itemType,
                    provider: '',
                    parentItemType: parentType,
                    file: { }
                };

                var createImageUrl = itemsServicePath + 'parent/{{parentId}}/{{itemId}}/?itemType={{itemType}}&provider={{provider}}&parentItemType={{parentItemType}}&newParentId={{parentId}}';
                var expectedUrl = $interpolate(createImageUrl)(settings);

                var item = {
                    Item: {
                        Id: 'resultingItemId'
                    }
                };
                $httpBackend.expectPUT(expectedUrl).respond(item);

                var data;
                mediaService[testObjSettings.callbacks.testedObject]
                    .upload(settings)
                    .then(function (res) {
                        data = res;
                    });

                expect(data).toBeUndefined();

                var xhrObj = getXmlHttpRequestMock($window);

                $httpBackend.flush();
                
                xhrObj.readyState = 4;
                xhrObj.status = 200;
                xhrObj.responseText = JSON.stringify(item);

                $rootScope.$apply(xhrObj.onload);

                expect(xhrObj.onload).toBeDefined();
                expect(xhrObj.open).toHaveBeenCalled();
                expect(xhrObj.send).toHaveBeenCalled();
                expect(data).toEqualData(item);
            });
        });

        /* Content */
        describe('content', function () {
            it('[dzhenko] / should return all content', function () {
                var subpath = '?itemType=' + itemType;

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertContent();
            });

            it('[dzhenko] / should return recent content', function () {
                var subpath = '?filter=(LastModified%3E(Sun,+25+Jan+2015+14:09:21+GMT))&itemType=' + itemType;

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertContent({ filter: '(LastModified>(Sun, 25 Jan 2015 14:09:21 GMT))' });
            });

            it('[dzhenko] / should return own content', function () {
                var subpath = '?filter=Owner+%3D%3D+(67152310-c838-6bcd-855b-ff0000c292fc)&itemType=' + itemType;

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertContent({ filter: 'Owner == (67152310-c838-6bcd-855b-ff0000c292fc)' });
            });

            it('[dzhenko] / should return content from folder and owner', function () {
                var subpath = 'parent/' + sampleGuid + '/?filter=Owner+%3D%3D+(67152310-c838-6bcd-855b-ff0000c292fc)&itemType=' + itemType;

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertContent({ parent: sampleGuid, filter: 'Owner == (67152310-c838-6bcd-855b-ff0000c292fc)' });
            });

            it('[dzhenko] / should return only 1 content', function () {
                var subpath = '?itemType=' + itemType + '&take=1';

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertContent({ take: 1 });
            });

            it('[dzhenko] / should skip 1 content', function () {
                var subpath = '?itemType=' + itemType + '&skip=1';

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertContent({ skip: 1 });
            });

            it('[dzhenko] / should skip 1 and return only 1 content', function () {
                var subpath = '?itemType=' + itemType + '&skip=1&take=1';

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertContent({ skip: 1, take: 1 });
            });

            it('[dzhenko] / should use custom provider with all content', function () {
                var subpath = '?itemType=' + itemType + '&provider=FakeDataProvider';

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertContent({ provider: 'FakeDataProvider' });
            });

            it('[dzhenko] / should use sort expression with all content', function () {
                var subpath = '?itemType=' + itemType + '&sortExpression=FakeSortExpression';

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertContent({ sort: 'FakeSortExpression' });
            });

            it('[dzhenko] / should use sort and filter expression with content', function () {
                var subpath = '?filter=FakeFilterExpression&itemType=' + itemType + '&sortExpression=FakeSortExpression';

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertContent({ sort: 'FakeSortExpression', filter: 'FakeFilterExpression' });
            });

            it('[dzhenko] / should use skip, take and provider with content', function () {
                var subpath = '?itemType=' + itemType + '&provider=FakeDataProvider&skip=1&take=1';

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertContent({ provider: 'FakeDataProvider', skip: 1, take: 1 });
            });

            it('[dzhenko] / should use all options with content', function () {
                var subpath = '?filter=FakeFilterExpression&itemType=' + itemType + '&provider=FakeDataProvider&skip=1&sortExpression=FakeSortExpression&take=1';

                $httpBackend.expectGET(itemsServicePath + subpath).respond(dataItems);

                assertContent({ filter: 'FakeFilterExpression', sort: 'FakeSortExpression', provider: 'FakeDataProvider', skip: 1, take: 1 });
            });
        });

        describe('media section settings', function () {
            var configSectionService = appPath + '/Sitefinity/Services/Configuration/ConfigSectionItems.svc/?mode=Form&nodeName={0}';
            it('[GeorgiMateev] / should construct correct regex for allowed file extensions.', function () {
                var url = configSectionService.format(settingsNodeName);
                $httpBackend.expectGET(url).respond(mediaSectionSettings);

                assertSettings(testObjSettings);
            });
        });
    };

    /* Test Running */
    allTestObjectsSettings.forEach(runTest);
});