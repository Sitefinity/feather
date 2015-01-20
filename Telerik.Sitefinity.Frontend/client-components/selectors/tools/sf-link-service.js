(function () {
    angular.module('sfSelectors')
        .factory('sfLinkMode', [function () {

            var service = {
                WebAddress: 1,
                InternalPage: 2,
                Anchor: 3,
                EmailAddress: 4
            };

            return service;

        }])
        .factory('sfLinkService', ['serverContext', 'sfLinkMode', function (serverContext, linkMode) {

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
                this.selectedAnchor = null;

                this.setMode = function () {
                    if (linkHtml.attr('sfref') && linkHtml.attr('sfref').startsWith('[')) {
                        this.mode = linkMode.InternalPage;
                    }
                    else if (linkHtml.attr('href') && linkHtml.attr('href').indexOf('mailto:') > -1) {
                        this.mode = linkMode.EmailAddress;
                    }
                    else if (linkHtml.attr('href') && linkHtml.attr('href').indexOf('#') > -1) {
                        this.mode = linkMode.Anchor;
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
                    idx = sfref.indexOf(']');
                    if (idx > -1) {
                        this.rootNodeId = sfref.substr(1, idx - 1);
                        this.pageId = sfref.substring(idx + 1);
                    }
                };

                this.setSelectedAnchor = function () {
                    var href = linkHtml.attr('href');
                    idx = href.indexOf('#');
                    if (idx > -1) {
                        this.selectedAnchor = href.substring(idx + 1);
                    }
                };

                this.setMode();
                this.setOpenInNewWindow();
                this.setDisplayText();

                if (this.mode == linkMode.WebAddress) {
                    this.setWebAddress();
                }
                else if (this.mode == linkMode.InternalPage) {
                    this.setInternalPage();
                }
                else if (this.mode == linkMode.Anchor) {
                    this.setSelectedAnchor();
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
                else if (linkItem.mode == linkMode.Anchor) {
                    resultLink.attr('href', '#' + linkItem.selectedAnchor);
                }
                else if (linkItem.mode == linkMode.EmailAddress) {
                    resultLink.attr('href', 'mailto:' + linkItem.emailAddress);
                }

                return resultLink;
            };

            var populateAnchorIds = function (editorContent) {
                var anchors = [];

                if (editorContent) {
                    var wrapperDiv = document.createElement("div");
                    wrapperDiv.innerHTML = editorContent;
                    jQuery(wrapperDiv).find("[id]").each(function () {
                        anchors.push(this.id);
                    });
                }

                return anchors;
            };
            return {
                constructLinkItem: constructLinkItem,
                getHtmlLink: getHtmlLink,
                populateAnchorIds: populateAnchorIds
            };
        }]);
})();