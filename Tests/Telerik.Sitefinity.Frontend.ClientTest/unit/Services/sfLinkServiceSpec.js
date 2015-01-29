describe('links service', function () {
    beforeEach(module('sfSelectors'));

    var linksService;
    var linkMode;

    beforeEach(inject(function (_sfLinkService_, _sfLinkMode_) {
        linksService = _sfLinkService_;
        linkMode = _sfLinkMode_;
    }));

    describe('generate html anchor tag from object', function () {
        it('[GeorgiMateev] / it should return empty link if no item is present.',
        function () {
            var link = linksService.getHtmlLink()[0].outerHTML;
            expect(link).toBe('<a></a>');
        });

        it('[GeorgiMateev] / it should return link from a web address.',
        function () {
            var linkItem = {
                mode: linkMode.WebAddress,
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
                mode: linkMode.WebAddress,
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
                mode: linkMode.EmailAddress,
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
                mode: linkMode.InternalPage,
                displayText: 'My link',
                selectedPage: selectedPage,
                language: 'en'
            };

            var link = linksService.getHtmlLink(linkItem)[0].outerHTML;

            var expected = '<a href="{0}" sfref="[{1}|lng:en]{2}">My link</a>'
                .format(selectedPage.FullUrl, selectedPage.RootId, selectedPage.Id);

            expect(link).toBe(expected);
        });

        it('[GeorgiMateev] / it should return link from a selected page and given root node Id.',
        function () {
            var selectedPage = {
                FullUrl: 'http://somesite.com/home',
                Id: '4c003fb0-2a77-61ec-be54-ff00007864f',
                RootId: '4c003fb0-2a77-61ec-bbbb-ff00007864f'
            };

            var linkItem = {
                mode: linkMode.InternalPage,
                displayText: 'My link',
                rootNodeId: '4c003fb0-2a77-61ec-bbbb-ff00007864f',
                selectedPage: selectedPage,
                language: 'en'
            };

            var link = linksService.getHtmlLink(linkItem)[0].outerHTML;

            var expected = '<a href="{0}" sfref="[{1}|lng:en]{2}">My link</a>'
                .format(selectedPage.FullUrl, selectedPage.RootId, selectedPage.Id);

            expect(link).toBe(expected);
        });

        it('[Manev] / it should return link from a selected page and given root node Id with BG language.',
           function () {
               var selectedPage = {
                   FullUrl: 'http://somesite.com/home',
                   Id: '4c003fb0-2a77-61ec-be54-ff00007864f',
                   RootId: '4c003fb0-2a77-61ec-bbbb-ff00007864f'
               };

               var linkItem = {
                   mode: linkMode.InternalPage,
                   displayText: 'My link',
                   rootNodeId: '4c003fb0-2a77-61ec-bbbb-ff00007864f',
                   selectedPage: selectedPage,
                   language: 'bg'
               };

               var link = linksService.getHtmlLink(linkItem)[0].outerHTML;

               var expected = '<a href="{0}" sfref="[{1}|lng:bg]{2}">My link</a>'
                   .format(selectedPage.FullUrl, selectedPage.RootId, selectedPage.Id);

               expect(link).toBe(expected);
           });

        it('[Manev] / it should return link from a selected page and given root node Id withoud any language set.',
          function () {
              var selectedPage = {
                  FullUrl: 'http://somesite.com/home',
                  Id: '4c003fb0-2a77-61ec-be54-ff00007864f',
                  RootId: '4c003fb0-2a77-61ec-bbbb-ff00007864f'
              };

              var linkItem = {
                  mode: linkMode.InternalPage,
                  displayText: 'My link',
                  rootNodeId: '4c003fb0-2a77-61ec-bbbb-ff00007864f',
                  selectedPage: selectedPage,
              };

              var link = linksService.getHtmlLink(linkItem)[0].outerHTML;

              var expected = '<a href="{0}" sfref="[{1}]{2}">My link</a>'
                  .format(selectedPage.FullUrl, selectedPage.RootId, selectedPage.Id);

              expect(link).toBe(expected);
          });
    });

    describe('retrieve data object from jQuery anchor element', function () {
        it('[GeorgiMateev] / should get the display text.', function () {
            var element = $('<a href="mailto:someone@gmail.com">My link</a>');

            var linkItem = linksService.constructLinkItem(element);

            expect(linkItem.displayText).toBe('My link');
        });

        it('[GeorgiMateev] / should know if the link will be open in a new window.', function () {
            var element = $('<a target="_blank">My link</a>');

            var linkItem = linksService.constructLinkItem(element);

            expect(linkItem.openInNewWindow).toBe(true);
        });

        it('[GeorgiMateev] / should get the web address.', function () {
            var element = $('<a href="http://somesite.com">My link</a>');

            var linkItem = linksService.constructLinkItem(element);

            expect(linkItem.mode).toBe(linkMode.WebAddress);
            expect(linkItem.webAddress).toBe('http://somesite.com');
        });

        it('[GeorgiMateev] / should get the mail address.', function () {
            var element = $('<a href="mailto:someone@gmail.com">My link</a>');

            var linkItem = linksService.constructLinkItem(element);

            expect(linkItem.mode).toBe(linkMode.EmailAddress);
            expect(linkItem.emailAddress).toBe('someone@gmail.com');
        });

        it('[GeorgiMateev] / should get page data from the anchor sfref attribute.', function () {
            var selectedPage = {
                FullUrl: 'http://somesite.com/home',
                Id: '4c003fb0-2a77-61ec-be54-ff00007864fF',
                RootId: '4c003fb0-2a77-61ec-bbbb-ff00007864fF'
            };

            var html = '<a sfref="[{0}|lng:en]{1}">My link</a>'
                .format(selectedPage.RootId, selectedPage.Id);

            var linkItem = linksService.constructLinkItem($(html));

            expect(linkItem.mode).toBe(linkMode.InternalPage);
            expect(linkItem.pageId).toBe(selectedPage.Id);
            expect(linkItem.rootNodeId).toBe(selectedPage.RootId);
            expect(linkItem.language).toBe('en');
        });
    });
});