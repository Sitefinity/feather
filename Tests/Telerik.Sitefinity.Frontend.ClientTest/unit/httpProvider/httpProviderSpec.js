/* Tests for httpProvider */
describe("httpProvider", function () {
    var httpProvider,
        q,
        _serverDataProvider;

    beforeEach(function () {
        module('designer');

        module(function ($httpProvider) {
            httpProvider = $httpProvider;
        });
    });

    beforeEach(inject(function ($q) {
        q = $q;
    }));

    function testErrorResponseInterceptor(code) {
        $window = { location: { reload: function () { } } };

        $window.onbeforeunload = function () { };

        var respondInterceptor = httpProvider.interceptors[0](q, $window);

        $window.onbeforeunload = function () { };

        var reloadSpy = spyOn($window.location, 'reload');

        var rejection = { status: code, statusText: 'Unauthorize' };
        var rejectPromise = respondInterceptor.responseError(rejection);

        expect($window.onbeforeunload).toEqual(null);
        expect(reloadSpy).toHaveBeenCalled();
        expect(rejection.data).toEqual('Unauthorize');
        expect(rejection.statusText).toEqual('Unauthorize');
    }

    describe('test provider interceptor responseError when sitefinity session is expired', function () {
        it('[manev] / should show "Unauthorize message in case of 401 response status".', function () {
            testErrorResponseInterceptor(401);
        });

        it('[manev] / should show "Unauthorize message in case of 403 response status".', function () {
            testErrorResponseInterceptor(403);
        });
    });

    describe('test provider interceptor request', function () {
        it('[manev] / should set SF_UI_CULTURE in to request header.', function () {
            var config = {
                method: 'GET',
                headers: {},
                url: 'widget.sf-cshtml'
            };
            var respondInterceptor = httpProvider.interceptors[0]();
            respondInterceptor.request(config);

            expect(config.headers.SF_UI_CULTURE).toEqual('en-EN');
        });
    });
});