describe('serverContextProvider', function () {
    var widgetContext = {
        culture: 'fromWidgetContext'
    };

    var appPath = 'http://mysite.com:9999/myapp';

    //Helper methods
    var assertGetRoutedUrlCalled = function (context, expectedUrl) {
        var mostRecent = context.getRootedUrl.mostRecentCall;
        expect(mostRecent).toBeDefined();
        expect(mostRecent.args).toBeDefined();

        expect(mostRecent.args[0]).toBe(expectedUrl);
    };

    var assertGetEmbeddedResourceUrlCalled = function (context, expectedAssembly, expectedUrl) {
        var mostRecent = context.getEmbeddedResourceUrl.mostRecentCall;
        expect(mostRecent).toBeDefined();
        expect(mostRecent.args).toBeDefined();

        expect(mostRecent.args[0]).toBe(expectedAssembly, expectedUrl);
    };
    
    describe('default context - sitefinity', function () {
        var serverContext;

        beforeEach(module('services'));

        //Mock sitefinity global variable
        beforeEach(function () {
            sitefinity.getRootedUrl = jasmine.createSpy()
                .andCallFake(function (path) {
                    return appPath + '/' + path;
                });

            sitefinity.getEmbeddedResourceUrl = jasmine.createSpy()
                .andCallFake(function (assembly, path) {
                    return appPath + '/EmbeddedResource/' + path;
                });
        });

        beforeEach(module(function ($provide) {
            $provide.value('widgetContext', widgetContext);
        }));

        beforeEach(inject(function (_serverContext_) {
            serverContext = _serverContext_;
        }));

        it('[GMateev] / should call getRootedUrl from "sitefintiy" global variable.', function () {
            var path = 'path';
            serverContext.getRootedUrl(path);

            assertGetRoutedUrlCalled(sitefinity, path);
        });

        it('[GMateev] / should call getEmbeddedResourceUrl from "sitefintiy" global variable.', function () {
            var assembly = 'Assembly';
            var path = 'path';
            serverContext.getEmbeddedResourceUrl(assembly, path);

            assertGetEmbeddedResourceUrlCalled(sitefinity, assembly, path);
        });

        it('[GMateev] / should use "widgetContext" to retrieve culture.', function () {
            var culture = serverContext.getUICulture();

            expect(culture).toBe(widgetContext.culture);
        });
    });

    describe('custom context provided when "serverContextProvider" is configured.', function () {
        var context = {
            getRootedUrl: jasmine.createSpy()
                .andCallFake(function (path) {
                    return appPath + '/' + path;
                }),

            getEmbeddedResourceUrl: jasmine.createSpy()
                .andCallFake(function (assembly, path) {
                    return appPath + '/EmbeddedResource/' + path;
                }),

            uiCulture: 'fromCustomContext'
        },
            serverContext;

        beforeEach(module('services', function (serverContextProvider) {
            serverContextProvider.setServerContext(context);
        }));

        beforeEach(inject(function (_serverContext_) {
            serverContext = _serverContext_;
        }));

        it('[GMateev] / should call getRootedUrl from the custom context.', function () {
            var path = 'path';
            serverContext.getRootedUrl(path);

            assertGetRoutedUrlCalled(context, path);
        });

        it('[GMateev] / should call getEmbeddedResourceUrl from the custom context.', function () {
            var assembly = 'Assembly';
            var path = 'path';
            serverContext.getEmbeddedResourceUrl(assembly, path);

            assertGetEmbeddedResourceUrlCalled(context, assembly, path);
        });

        it('[GMateev] / should use the custom context to retrieve culture.', function () {
            var culture = serverContext.getUICulture();

            expect(culture).toBe(context.uiCulture);
        });
    });
});