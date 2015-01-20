describe('links service', function () {
    beforeEach(module('sfSelectors'));

    var linksService;
    var linkMode;

    beforeEach(inject(function (_sfLinkService_, _sfLinkMode_) {
        linksService = _sfLinkService_;
        linkMode = _sfLinkMode_;
    }));

    describe('generating html link from object', function () {
        it('[GeorgiMateev] / it should return link from a web address.',
        function () {
            var linkItem = {
                mode:linkMode.WebAddress,
                webAddress: 'http://somesite.com',
                displayText: 'My link'
            };

            var link = linksService.getHtmlLink(linkItem)[0].outerHTML;
            var expected = '<a href="http://somesite.com">My link</a>';
            expect(link).toBe(expected);
        });

        it('[GeorgiMateev] / it should return link with target new page.',
        function () {
            var linkItem = {
                mode:linkMode.WebAddress,
                webAddress: 'http://somesite.com',
                displayText: 'My link',
                openInNewWindow: true
            };

            var link = linksService.getHtmlLink(linkItem)[0].outerHTML;
            var expected = '<a target="_blank" href="http://somesite.com">My link</a>';
            expect(link).toBe(expected);
        });

        it('[GeorgiMateev] / it should return link from a mail address.',
        function () {
            var linkItem = {
                mode:linkMode.EmailAddress,
                emailAddress: 'someone@gmail.com',
                displayText: 'My link'
            };

            var link = linksService.getHtmlLink(linkItem)[0].outerHTML;
            var expected = '<a href="mailto:someone@gmail.com">My link</a>';
            expect(link).toBe(expected);
        });

        it('[GeorgiMateev] / it should return link from a selected page.',
        function () {
            var selectedPage = {
                FullUrl: 'http://somesite.com/home',
                Id: '4c003fb0-2a77-61ec-be54-ff00007864f',
                RootId: '4c003fb0-2a77-61ec-bbbb-ff00007864f'
            };

            var linkItem = {
                mode:linkMode.InternalPage,
                displayText: 'My link'
            };

            var link = linksService.getHtmlLink(linkItem, selectedPage)[0].outerHTML;

            var expected = '<a href="http://somesite.com/home" sfref="[{0}|lng:en]{1}">My link</a>'
                .format(selectedPage.RootId, selectedPage.Id);

            expect(link).toBe(expected);

            expect(link)
        });

        it('[GeorgiMateev] / it should return link from a selected page and given root node Id.',
        function () {
            var selectedPage = {
                FullUrl: 'http://somesite.com/home',
                Id: '4c003fb0-2a77-61ec-be54-ff00007864f'
            };

            var linkItem = {
                mode:linkMode.InternalPage,
                displayText: 'My link',
                rootNodeId: '4c003fb0-2a77-61ec-bbbb-ff00007864f'
            };

            var link = linksService.getHtmlLink(linkItem, selectedPage)[0].outerHTML;

            var expected = '<a href="http://somesite.com/home" sfref="[{0}|lng:en]{1}">My link</a>'
                .format(linkItem.rootNodeId, selectedPage.Id);

            expect(link).toBe(expected);

            expect(link)
        });
    });
});