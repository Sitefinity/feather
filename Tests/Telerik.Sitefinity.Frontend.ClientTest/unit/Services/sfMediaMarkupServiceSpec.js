describe('Media markup service', function () {
    var mediaMarkupService = null;

    beforeEach(module('sfServices'));

    beforeEach(inject(function ($injector) {
        mediaMarkupService = $injector.get('sfMediaMarkupService');
    }));

    var testMarkup = '<img sfref="[images|DefaultProvider]eb0b99c8-2d30-4967-bf6f-ff11b3702b16" src="http://full.media.url/image.jpg" alt="Test alternative title" title="Test iamge title" displaymode="Original" style="vertical-align: middle; margin: 100px 400px 300px 200px;">';

    var TestImageProperties = function () {
        this.item = { Id: 'eb0b99c8-2d30-4967-bf6f-ff11b3702b16', MediaUrl: 'http://full.media.url/image.jpg' };
        this.provider = 'DefaultProvider';
        this.displayMode = 'Thumbnail';

        this.customSize = {  // Keep the names of those properties as they are in order to support old HTML field.
            MaxWidth: null,
            MaxHeight: null,
            Width: null,
            Height: null,
            ScaleUp: false,
            Quality: null, // High, Medium, Low
            Method: null // ResizeFitToAreaArguments, CropCropArguments
        };

        this.thumbnail = {
            url: 'http://thumbnail.url/thumb.jpg',
            name: 'Special thumbnail'
        };

        this.title = 'Test iamge title';
        this.alternativeText = 'Test alternative title';
        this.alignment = 'Center';
        this.margin = {
            top: 100,
            left: 200,
            bottom: 300,
            right: 400
        };

        this.cssClass = null;

        this.openOriginalImageOnClick = false;
    };

    it('[Boyko-Karadzhov] / should render img tag with correct attributes.', function () {
        var properties = new TestImageProperties();
        properties.displayMode = 'Original';

        var markup = mediaMarkupService.image.markup(properties);
        var jMarkup = $(markup);

        expect(jMarkup.attr('sfref')).toEqual('[images|DefaultProvider]eb0b99c8-2d30-4967-bf6f-ff11b3702b16');
        expect(jMarkup.attr('src')).toEqual('http://full.media.url/image.jpg');
        expect(jMarkup.attr('alt')).toEqual('Test alternative title');
        expect(jMarkup.attr('title')).toEqual('Test iamge title');
        expect(jMarkup.attr('displayMode')).toEqual('Original');
        expect(jMarkup.css('vertical-align')).toEqual('middle');
        expect(jMarkup[0].style.marginTop).toEqual('100px');
        expect(jMarkup[0].style.marginLeft).toEqual('200px');
        expect(jMarkup[0].style.marginBottom).toEqual('300px');
        expect(jMarkup[0].style.marginRight).toEqual('400px');
    });

    it('[Boyko-Karadzhov] / should render img tag with correct attributes with thumbnail.', function () {
        var properties = new TestImageProperties();

        var markup = mediaMarkupService.image.markup(properties);
        var jMarkup = $(markup);

        expect(jMarkup.attr('sfref')).toEqual('[images|DefaultProvider|tmb:Special thumbnail]eb0b99c8-2d30-4967-bf6f-ff11b3702b16');
        expect(jMarkup.attr('src')).toEqual('http://thumbnail.url/thumb.jpg');
        expect(jMarkup.attr('alt')).toEqual('Test alternative title');
        expect(jMarkup.attr('title')).toEqual('Test iamge title');
        expect(jMarkup.attr('displayMode')).toEqual('Thumbnail');
        expect(jMarkup.css('vertical-align')).toEqual('middle');
        expect(jMarkup[0].style.marginTop).toEqual('100px');
        expect(jMarkup[0].style.marginLeft).toEqual('200px');
        expect(jMarkup[0].style.marginBottom).toEqual('300px');
        expect(jMarkup[0].style.marginRight).toEqual('400px');
    });

    it('[Boyko-Karadzhov] / should preserve properties on render and properties extraction.', function () {
        var expectedProperties = new TestImageProperties();
        expectedProperties.displayMode = 'Original';
        expectedProperties.thumbnail = { url: null, name: null };

        var markup = mediaMarkupService.image.markup(expectedProperties);
        var resultingProperties = mediaMarkupService.image.properties(markup);

        expect(resultingProperties).toEqual(expectedProperties);
    });

    it('[Boyko-Karadzhov] / should preserve properties on render and properties extraction when wrapped.', function () {
        var expectedProperties = new TestImageProperties();
        expectedProperties.displayMode = 'Original';
        expectedProperties.thumbnail = { url: null, name: null };

        var markup = mediaMarkupService.image.markup(expectedProperties, null, true);
        var resultingProperties = mediaMarkupService.image.properties(markup);

        expect(resultingProperties).toEqual(expectedProperties);
    });
});