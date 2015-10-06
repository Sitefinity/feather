/* Tests for personalization-services.js */
describe('personalizationService', function () {
    beforeEach(module('personalizationServices'));

    var appPath = 'http://mysite.com:9999/myapp';
    var personalizationServiceBaseUrl = '/RestApi/Sitefinity/personalizations/controls/';

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

    var $httpBackend;
    var personalizationService;
    var segments = [
        {
            Id: ""
        }
    ];

    var personalizedWidgetId = "1";
    var personalizedSegmentId = "111";
    var personalizedSegmentName = "Segment 1";

    beforeEach(inject(function ($injector) {
        // Set up the mock http service responses
        $httpBackend = $injector.get('$httpBackend');
        personalizationService = $injector.get('personalizationService');
    }));

    /* Helper methods */
    var assertSegments = function () {
        var data;
        personalizationService.getSegments.apply(personalizationService, null).then(function (res) {
            data = res;
        });


        $httpBackend.flush();

        expect(data).toEqualData(segments);
    };

    var assertPersonalize = function () {
        var data;

        var model = {
            segmentId: personalizedSegmentId,
            segmentName: personalizedSegmentName
        };

        var params = [model];

        personalizationService.personalize.apply(personalizationService, params).then(function (res) {
            data = res;
        });


        $httpBackend.flush();

        expect(data.Id).toEqualData(personalizedWidgetId);
        expect(data.SegmentName).toEqualData(personalizedSegmentName);
    };

    var expectGetSegmentsServiceCall = function (items) {
        var url = personalizationServiceBaseUrl + 'segments/?ControlId=' + sitefinity.pageEditor.widgetContext.Id + '&PageId=' + sitefinity.pageEditor.widgetContext.PageId;

        $httpBackend.expectGET(url).respond(items);
    };

    var expectPostPersonalizeServiceCall = function (itemId) {
        var url = personalizationServiceBaseUrl;

        $httpBackend.expectPOST(url).respond(itemId);
    };

    /* Tests */
    it('[GSashev] / should retrieve segments.', function () {
        expectGetSegmentsServiceCall.apply(this, [segments]);

        assertSegments();
    });

    it('[GSashev] / should retrieve personalized content id.', function () {
        var params = [personalizedWidgetId];

        expectPostPersonalizeServiceCall.apply(this, params);

        assertPersonalize();
    });
});