(function () {
    var GUID_REGEX = /([a-zA-Z0-9]{8}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{12})/g;
    var LANG_REGEX = /\|lng:([^\]]+)/;

    angular.module('sfSelectors')
        .factory('sfLinkService', ['serverContext', function (serverContext) {
            var linkItem = function (linkHtml) {
                this.mode = '1';
                this.openInNewWindow = false;
                this.webAddress = null;
                this.site = null;
                this.language = null;
                this.rootNodeId = null;
                this.pageId = null;
                this.emailAddress = null;
                this.displayText = '';

                this.setMode = function () {
                    if (linkHtml.attr('sfref') && linkHtml.attr('sfref').startsWith('[')) {
                        this.mode = '2';
                    }
                    else if (linkHtml.attr('href') && linkHtml.attr('href').indexOf('mailto:') > -1) {
                        this.mode = '3';
                    }
                    else {
                        this.mode = '1';
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
                    if (cultures.length > 1) {
                        //NOTE: For a non-global regexp - it finds the first match and returns an array: the full match becomes array item at index 0, the first group - at index 1, and so on.
                        this.language = cultures[1];
                    }

                    //var langDelimiter = sfref.indexOf('|');
                    //idx = langDelimiter > -1 ? langDelimiter : sfref.indexOf(']');

                    //if (idx > -1) {
                    //    this.rootNodeId = sfref.substr(1, idx - 1);
                    //    this.pageId = sfref.substring(idx + 1);

                    //    var langDelimiter = sfref.indexOf("|lng:");

                    //    var selectedCulture = langDelimiter == -1 ? serverContext.getUICulture() :
                    //                                sfref.substring(langDelimiter, sfref.indexOf(']'));
                    //}
                };

                this.setMode();
                this.setOpenInNewWindow();

                if (this.mode == '1') {
                    this.setWebAddress();
                }
                else if (this.mode == '2') {
                    this.setInternalPage();
                }
                else if (this.mode == '3') {
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

                if (linkItem.mode == '1') {
                    resultLink.attr('href', linkItem.webAddress);
                }
                else if (linkItem.mode == '2') {
                    if (selectedPage) {
                        var href = selectedPage.FullUrl;
                        resultLink.attr('href', href);

                        var selectedPageId = selectedPage.Id;
                        var selectedCulture = 'en';
                        //var selectedCulture = (this.get_uiCulture() !== this.get_pageSelector().get_languageSelectorSelectedCulture()) ?
                        //	this.get_pageSelector().get_languageSelectorSelectedCulture() : null;
                        if (selectedPageId) {
                            var key;
                            if (linkItem.rootNodeId && linkItem.rootNodeId != Telerik.Sitefinity.getEmptyGuid()) {
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
                else if (linkItem.mode == '3') {
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