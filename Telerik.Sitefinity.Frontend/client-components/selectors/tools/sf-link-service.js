(function () {
    var GUID_REGEX = /([a-zA-Z0-9]{8}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{12})/g;
    var LANG_REGEX = /\|lng:([^\]]+)/;

    angular.module('sfSelectors')
        .factory('sfLinkMode', [function () {

            var service = {
                WebAddress: 1,
                InternalPage: 2,
                EmailAddress: 3
            };

            return service;

        }])
        .factory('sfLinkService', ['serverContext', 'sfLinkMode', function (serverContext, linkMode) {

            var emptyGuid = '00000000-0000-0000-0000-000000000000';

            var linkItem = function (linkHtml) {
                this.mode = linkMode.WebAddress;
                this.openInNewWindow = false;
                this.webAddress = null;
                this.site = null;
                this.language = null;
                this.rootNodeId = null;
                this.pageId = null;
                this.emailAddress = null;
                this.displayText = '';

                function startsWith (str, subStr) {
                    return str.slice(0, subStr.length) === subStr;
                }

                this.setMode = function () {
                    var sfref = linkHtml.attr('sfref');
                    var href = linkHtml.attr('href');

                    if (sfref && startsWith(sfref, '[')) {
                        this.mode = linkMode.InternalPage;
                    }
                    else if (href && href.indexOf('mailto:') > -1) {
                        this.mode = linkMode.EmailAddress;
                    }
                    else {
                        this.mode = linkMode.WebAddress;
                    }
                };

                this.setOpenInNewWindow = function () {
                    var targetValue = linkHtml.attr('target');
                    if (targetValue && targetValue === '_blank') {
                        this.openInNewWindow = true;
                    }
                    else {
                        this.openInNewWindow = false;
                    }
                };

                this.setDisplayText = function () {
                    this.displayText = linkHtml.html();
                };

                this.setWebAddress = function () {
                    this.webAddress = linkHtml.attr('href') ? linkHtml.attr('href') : 'http://';
                };

                this.setEmailAddress = function () {
                    var idx = linkHtml.attr('href').indexOf(':');
                    if (idx > -1) {
                        this.emailAddress = linkHtml.attr('href').substring(idx + 1);
                    }
                };

                this.setInternalPage = function () {
                    var sfref = linkHtml.attr('sfref');
                    
                    var guids = sfref.match(GUID_REGEX);
                    var cultures = sfref.match(LANG_REGEX);

                    if (guids.length > 0) {
                        this.rootNodeId = guids[0];

                        if (guids.length > 1) {
                            this.pageId = guids[1];
                        }
                    }
                    if (cultures && cultures.length > 1) {
                        //NOTE: For a non-global regexp - it finds the first match and returns an array: the full match becomes array item at index 0, the first group - at index 1, and so on.
                        this.language = cultures[1];
                    }
                    else {
                        this.language = serverContext.getUICulture();
                    }
                };

                this.setMode();
                this.setOpenInNewWindow();

                if (this.mode == linkMode.WebAddress) {
                    this.setWebAddress();
                }
                else if (this.mode == linkMode.InternalPage) {
                    this.setInternalPage();
                }
                else if (this.mode == linkMode.EmailAddress) {
                    this.setEmailAddress();
                }
            };

            var constructLinkItem = function (linkHtml) {
                var item = new linkItem(linkHtml);

                return item;
            };

            var getHtmlLink = function (linkItem, selectedPage) {
                var resultLink = jQuery('<a></a>');
                if (!linkItem)
                    return resultLink;

                resultLink.html(linkItem.displayText);
                if (linkItem.openInNewWindow)
                    resultLink.attr('target', '_blank');

                if (linkItem.mode == linkMode.WebAddress) {
                    resultLink.attr('href', linkItem.webAddress);
                }
                else if (linkItem.mode == linkMode.InternalPage) {
                    if (selectedPage) {
                        var href = selectedPage.FullUrl;
                        resultLink.attr('href', href);

                        var selectedPageId = selectedPage.Id;
                        var selectedCulture = 'en';
                        //var selectedCulture = (this.get_uiCulture() !== this.get_pageSelector().get_languageSelectorSelectedCulture()) ?
                        //	this.get_pageSelector().get_languageSelectorSelectedCulture() : null;
                        if (selectedPageId) {
                            var key;
                            if (linkItem.rootNodeId && linkItem.rootNodeId != emptyGuid) {
                                key = linkItem.rootNodeId;
                            }
                            else if (selectedPage) {
                                key = selectedPage.RootId;
                            }
                            else {
                                key = '';
                            }
                            var sfref = '[' + key;
                            if (selectedCulture) {
                                sfref += '|lng:' + selectedCulture;
                            }
                            sfref += ']' + selectedPageId;

                            resultLink.attr('sfref', sfref);
                        }
                    }
                }
                else if (linkItem.mode == linkMode.EmailAddress) {
                    resultLink.attr('href', 'mailto:' + linkItem.emailAddress);
                }

                return resultLink;
            };
            return {
                constructLinkItem: constructLinkItem,
                getHtmlLink: getHtmlLink
            };
        }]);
})();