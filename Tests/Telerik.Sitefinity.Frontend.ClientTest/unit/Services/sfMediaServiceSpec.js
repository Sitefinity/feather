/* Tests for sf-media-service.js */
describe('sfMediaService', function () {
    var $httpBackend;
    var mediaService;

    var dataItems = {
        Items: [{
            Id: '4a003fb0-2a77-61ec-be54-ff00007864f4',
            IsFolder: true
        }],
        TotalCount: 1
    };

    var errorResponse = {
        Detail: 'Error'
    };

    var appPath = 'http://mysite.com:9999/myapp';
    var sampleGuid = '1ac3b615-0ce5-46dc-a0af-5c5d1f146df9';

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

    /* Tested Services */
    var allTestObjectsSettings = [
        {
            itemType: 'Telerik.Sitefinity.Libraries.Model.Image',
            itemsServicePath: appPath + '/Sitefinity/Services/Content/ImageService.svc/',
            albumsServicePath: appPath + '/Sitefinity/Services/Content/AlbumService.svc/folders/',
            callbacks: {
                testedObject: 'images',
                folders: 'getFolders',
                items: 'getImages',
                content: 'getContent'
            }
        }
    ];

    /* Generic Tests */
    var runTest = function (testObjSettings) {
        /* Setup */
        var albumsServicePath = testObjSettings.albumsServicePath;
        var itemsServicePath = testObjSettings.itemsServicePath;
        var itemType = testObjSettings.itemType;
        var assertFolders,
            assertItems,
            assertContent,
            assertError;

        beforeEach(inject(function ($injector) {
            mediaService = $injector.get('sfMediaService');

            assertFolders = function (params) {
                baseAssertFolders(params, mediaService[testObjSettings.callbacks.testedObject][testObjSettings.callbacks.folders]);
            }

            assertItems = function (params) {
                baseAssertItems(params, mediaService[testObjSettings.callbacks.testedObject][testObjSettings.callbacks.items]);
            }

            assertContent = function (params) {
                baseAssertContent(params, mediaService[testObjSettings.callbacks.testedObject][testObjSettings.callbacks.content]);
            }

           assertError = function (params, methodName) {
               baseAssertError(params, mediaService[testObjSettings.callbacks.testedObject][methodName]);
            }
        }));

        /* Tests */

        /* Common */
        (function () {
            it('[dzhenko] / passing no options object to folders should return all objects', function () {
                var subpath = '?hierarchyMode=true'

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
        }());

        /* Errors */
        (function () {
            it('[dzhenko] / should return error on folders', function () {
                var subpath = '?hierarchyMode=true'

                $httpBackend.expectGET(albumsServicePath + subpath).respond(500, errorResponse);

                assertError(null, 'getFolders');
            });

            it('[dzhenko] / should return error on images', function () {
                var subpath = '?excludeFolders=true&itemType=' + itemType;

                $httpBackend.expectGET(itemsServicePath + subpath).respond(500, errorResponse);

                assertError(null, 'getImages');
            });

            it('[dzhenko] / should return error on content', function () {
                var subpath = '?itemType=' + itemType;

                $httpBackend.expectGET(itemsServicePath + subpath).respond(500, errorResponse);

                assertError(null, 'getContent');
            });
        }());

        /* Folders */
        (function () {
            // Root folders
            it('[dzhenko] / should return only root folders', function () {
                var subpath = '?hierarchyMode=true';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders();
            });

            it('[dzhenko] / should return only 1 root folder', function () {
                var subpath = '?hierarchyMode=true&take=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: false, take: 1 });
            });

            it('[dzhenko] / should skip 1 root folder', function () {
                var subpath = '?hierarchyMode=true&skip=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: false, skip: 1 });
            });

            it('[dzhenko] / should skip 1 and return only 1 root folder', function () {
                var subpath = '?hierarchyMode=true&skip=1&take=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: false, skip: 1, take: 1 });
            });

            it('[dzhenko] / should use custom provider with root folders', function () {
                var subpath = '?hierarchyMode=true&provider=FakeDataProvider';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: false, provider: 'FakeDataProvider' });
            });

            it('[dzhenko] / should use filter with root folders', function () {
                var subpath = '?filter=FakeFilterExpression&hierarchyMode=true';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: false, filter: 'FakeFilterExpression' });
            });

            it('[dzhenko] / should use sort expression with root folders', function () {
                var subpath = '?hierarchyMode=true&sortExpression=FakeSortExpression';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: false, sort: 'FakeSortExpression' });
            });

            it('[dzhenko] / should use sort and filter expression with root folders', function () {
                var subpath = '?filter=FakeFilterExpression&hierarchyMode=true&sortExpression=FakeSortExpression';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: false, sort: 'FakeSortExpression', filter: 'FakeFilterExpression' });
            });

            it('[dzhenko] / should use skip, take and provider with root folders', function () {
                var subpath = '?hierarchyMode=true&provider=FakeDataProvider&skip=1&take=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: false, provider: 'FakeDataProvider', skip: 1, take: 1 });
            });

            it('[dzhenko] / should use all options with root folders', function () {
                var subpath = '?filter=FakeFilterExpression&hierarchyMode=true&provider=FakeDataProvider&skip=1&sortExpression=FakeSortExpression&take=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: false, filter: 'FakeFilterExpression', sort: 'FakeSortExpression', provider: 'FakeDataProvider', skip: 1, take: 1 });
            });

            // All folders
            it('[dzhenko] / should return all existing folders', function () {
                $httpBackend.expectGET(albumsServicePath).respond(dataItems);

                assertFolders({ parent: null, recursive: true });
            });

            it('[dzhenko] / should return only 1 folder', function () {
                var subpath = '?take=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: true, take: 1 });
            });

            it('[dzhenko] / should skip 1 folder', function () {
                var subpath = '?skip=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: true, skip: 1 });
            });

            it('[dzhenko] / should skip 1 and return only 1  folder', function () {
                var subpath = '?skip=1&take=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: true, skip: 1, take: 1 });
            });

            it('[dzhenko] / should use custom provider with all folders', function () {
                var subpath = '?provider=FakeDataProvider';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: true, provider: 'FakeDataProvider' });
            });

            it('[dzhenko] / should use filter with all folders', function () {
                var subpath = '?filter=FakeFilterExpression';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: true, filter: 'FakeFilterExpression' });
            });

            it('[dzhenko] / should use sort expression with all folders', function () {
                var subpath = '?sortExpression=FakeSortExpression';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: true, sort: 'FakeSortExpression' });
            });

            it('[dzhenko] / should use sort and filter expression with all folders', function () {
                var subpath = '?filter=FakeFilterExpression&sortExpression=FakeSortExpression';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: true, sort: 'FakeSortExpression', filter: 'FakeFilterExpression' });
            });

            it('[dzhenko] / should use skip, take and provider with all folders', function () {
                var subpath = '?provider=FakeDataProvider&skip=1&take=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: true, provider: 'FakeDataProvider', skip: 1, take: 1 });
            });

            it('[dzhenko] / should use all options with all folders', function () {
                var subpath = '?filter=FakeFilterExpression&provider=FakeDataProvider&skip=1&sortExpression=FakeSortExpression&take=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: null, recursive: true, filter: 'FakeFilterExpression', sort: 'FakeSortExpression', provider: 'FakeDataProvider', skip: 1, take: 1 });
            });

            // Child folders
            it('[dzhenko] / should return only child folders', function () {
                var subpath = sampleGuid + '/?hierarchyMode=true';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: sampleGuid, recursive: false });
            });

            it('[dzhenko] / should return only 1 child folder', function () {
                var subpath = sampleGuid + '/?hierarchyMode=true&take=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: sampleGuid, recursive: false, take: 1 });
            });

            it('[dzhenko] / should skip 1 child folder', function () {
                var subpath = sampleGuid + '/?hierarchyMode=true&skip=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: sampleGuid, recursive: false, skip: 1 });
            });

            it('[dzhenko] / should skip 1 and return only 1 child folder', function () {
                var subpath = sampleGuid + '/?hierarchyMode=true&skip=1&take=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: sampleGuid, recursive: false, skip: 1, take: 1 });
            });

            it('[dzhenko] / should use custom provider with child folders', function () {
                var subpath = sampleGuid + '/?hierarchyMode=true&provider=FakeDataProvider';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: sampleGuid, recursive: false, provider: 'FakeDataProvider' });
            });

            it('[dzhenko] / should use filter with child folders', function () {
                var subpath = sampleGuid + '/?filter=FakeFilterExpression&hierarchyMode=true';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: sampleGuid, recursive: false, filter: 'FakeFilterExpression' });
            });

            it('[dzhenko] / should use sort expression with child folders', function () {
                var subpath = sampleGuid + '/?hierarchyMode=true&sortExpression=FakeSortExpression';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: sampleGuid, recursive: false, sort: 'FakeSortExpression' });
            });

            it('[dzhenko] / should use sort and filter expression with child folders', function () {
                var subpath = sampleGuid + '/?filter=FakeFilterExpression&hierarchyMode=true&sortExpression=FakeSortExpression';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: sampleGuid, recursive: false, sort: 'FakeSortExpression', filter: 'FakeFilterExpression' });
            });

            it('[dzhenko] / should use skip, take and provider with child folders', function () {
                var subpath = sampleGuid + '/?hierarchyMode=true&provider=FakeDataProvider&skip=1&take=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: sampleGuid, recursive: false, provider: 'FakeDataProvider', skip: 1, take: 1 });
            });

            it('[dzhenko] / should use all options with child folders', function () {
                var subpath = sampleGuid + '/?filter=FakeFilterExpression&hierarchyMode=true&provider=FakeDataProvider&skip=1&sortExpression=FakeSortExpression&take=1';

                $httpBackend.expectGET(albumsServicePath + subpath).respond(dataItems);

                assertFolders({ parent: sampleGuid, recursive: false, filter: 'FakeFilterExpression', sort: 'FakeSortExpression', provider: 'FakeDataProvider', skip: 1, take: 1 });
            });
        }());

        /* Images */
        (function () {
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
        }());

        /* Content */
        (function () {
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
        }());
    };

    /* Test Running */
    allTestObjectsSettings.forEach(runTest);

    /* Filter Tests */
    (function () {
        var assertFilterSetCallsFunction = function (object, callback, val1, val2) {
            var filter = mediaService.newFilter();
            var called = false;

            filter.attachEvent(function () {
                called = true;
            });

            filter.set[object][callback](val1, val2);

            expect(called).toBe(true);

            return filter;
        }

        var assertAllButOnePropertyAreNull = function (filter, propName) {
            if (propName !== 'basic') {
                expect(filter.basic).toBe(null);
            }

            if (propName !== 'parent') {
                expect(filter.parent).toBe(null);
            }

            if (propName !== 'query') {
                expect(filter.query).toBe(null);
            }

            if (propName !== 'date') {
                expect(filter.date).toBe(null);
            }

            if (propName !== 'taxon') {
                expect(filter.taxon.id).toBe(null);
                expect(filter.taxon.field).toBe(null);
            }
        }

        it('[dzhenko] / a new filter should have all its properties null', function () {
            assertAllButOnePropertyAreNull(mediaService.newFilter(), null);
        });

        // basic
        it('[dzhenko] / attaching a function should not call it when changing basic none from initial state and set properties correctly', function () {
            var filter = mediaService.newFilter();
            var called = false;

            filter.attachEvent(function () {
                called = true;
            });

            filter.set.basic.none();

            expect(called).toBe(false);
            assertAllButOnePropertyAreNull(filter, null);
        });

        it('[dzhenko] / attaching a function should call it when changing basic none not from initial state and set properties correctly', function () {
            var filter = mediaService.newFilter();
            var called = false;

            filter.set.basic.allLibraries();

            filter.attachEvent(function () {
                called = true;
            });

            filter.set.basic.none();

            expect(called).toBe(true);
            assertAllButOnePropertyAreNull(filter, null);
        });

        it('[dzhenko] / attaching a function should call it when changing basic to all libraries and set properties correctly', function () {
            var filter = assertFilterSetCallsFunction('basic', 'allLibraries');
            assertAllButOnePropertyAreNull(filter, 'basic');
            expect(filter.basic).toEqual(filter.constants.basic.allLibraries);
        });

        it('[dzhenko] / attaching a function should call it when changing basic to own items and set properties correctly', function () {
            var filter = assertFilterSetCallsFunction('basic', 'ownItems');
            assertAllButOnePropertyAreNull(filter, 'basic');
            expect(filter.basic).toEqual(filter.constants.basic.ownItems);
        });

        it('[dzhenko] / attaching a function should call it when changing basic to recent items and set properties correctly', function () {
            var filter = assertFilterSetCallsFunction('basic', 'recentItems');
            assertAllButOnePropertyAreNull(filter, 'basic');
            expect(filter.basic).toEqual(filter.constants.basic.recentItems);
        });

        // date
        it('[dzhenko] / attaching a function should call it when changing date to all time value and set properties correctly', function () {
            var filter = assertFilterSetCallsFunction('date', 'all');
            assertAllButOnePropertyAreNull(filter, 'date');
            expect(filter.date).toEqual(filter.constants.anyTimeValue);
        });

        it('[dzhenko] / attaching a function should call it when changing date to some value and set properties correctly', function () {
            var date = new Date();
            var filter = assertFilterSetCallsFunction('date', 'to', date);
            assertAllButOnePropertyAreNull(filter, 'date');
            expect(filter.date).toEqual(date);
        });

        // parent
        it('[dzhenko] / attaching a function should call it when changing parent to some value and set properties correctly', function () {
            var parent = 'someParent';
            var filter = assertFilterSetCallsFunction('parent', 'to', parent);
            assertAllButOnePropertyAreNull(filter, 'parent');
            expect(filter.parent).toEqual(parent);
        });

        // taxon
        it('[dzhenko] / attaching a function should call it when changing taxon to some value and set properties correctly', function () {
            var taxonId = 'taxonId';
            var taxonField = 'taxonField';
            var filter = assertFilterSetCallsFunction('taxon', 'to', taxonId, taxonField);
            assertAllButOnePropertyAreNull(filter, 'taxon');
            expect(filter.taxon.id).toEqual(taxonId);
            expect(filter.taxon.field).toEqual(taxonField);
        });

        // query
        it('[dzhenko] / attaching a function should call it when changing query to some value and set properties correctly', function () {
            var query = 'query';
            var filter = assertFilterSetCallsFunction('query', 'to', query);
            assertAllButOnePropertyAreNull(filter, 'query');
            expect(filter.query).toEqual(query);
        });

        // query
        it('[dzhenko] / setting query when basic was all libraries should make basic to null', function () {
            var filter = mediaService.newFilter();
            filter.set.basic.allLibraries();

            expect(filter.basic).toEqual(filter.constants.basic.allLibraries);

            filter.set.query.to('some query');
            expect(filter.basic).toEqual(null);
        });
    }());
});