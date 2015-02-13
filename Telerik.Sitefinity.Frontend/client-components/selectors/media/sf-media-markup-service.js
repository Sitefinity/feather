; (function ($) {
    angular.module('sfServices')
        .factory('sfMediaMarkupService', [function () {
            // TODO: This class is currently not used. For now it is here just for clarity. Remove if not needed after full integration.
            var ImageProperties = function () {
                this.item = { Id: null }; //MediaItem view model
                this.provider = null; //Name of the data provider
                this.displayMode = null; //Original size, Thumbnail...
                this.thumbnail = {
                    url: null,
                    name: null
                };

                this.title = null;
                this.alternativeText = null;
                this.alignment = null;
                this.margin = {
                    top: null,
                    left: null,
                    bottom: null,
                    right: null
                };

                this.openOriginalImageOnClick = null;
            };

            var getSfrefAttribute = function (mediaType, id, provider, thumbnailName) {
                var sfref = '[' + mediaType;
                if (provider) {
                    sfref += '|' + provider;
                }

                if (thumbnailName && thumbnailName !== '') {
                    sfref += '|tmb:' + thumbnailName;
                }

                sfref += ']' + id;
                return sfref;
            };

            var getIdFromSfrefAttr = function (sfref) {
                if (sfref) {
                    var idx = sfref.indexOf("]");
                    if (idx > -1) {
                        return sfref.substring(idx + 1);
                    }
                }
                return null;
            };

            var getProviderFromSfrefAttr = function (sfref) {
                if (sfref) {
                    var startIdx = sfref.indexOf("[");
                    var endIdx = sfref.indexOf("]");
                    if (startIdx > -1 && endIdx > -1) {
                        var parts = sfref.substring(startIdx + 1, endIdx).split("|");
                        if (parts.length > 1) {
                            for (var i = 1; i < parts.length; i++) {
                                if (parts[i].indexOf(":") === -1)
                                    return parts[i];
                            }
                        }
                    }
                }
                return null;
            };

            var getThumbnailNameFromSfrefAttr = function (sfref) {
                if (sfref) {
                    var startIdx = sfref.indexOf("[");
                    var endIdx = sfref.indexOf("]");
                    if (startIdx > -1 && endIdx > -1) {
                        var parts = sfref.substring(startIdx + 1, endIdx).split("|");
                        if (parts.length > 1) {
                            for (var i = 1; i < parts.length; i++) {
                                var indx = parts[i].indexOf('tmb:');
                                if (indx === 0)
                                    return parts[i].substring(indx + 4);
                            }
                        }
                    }
                }
                return null;
            };

            var resolveThumbnailUrl = function (tmbDefaultUrl, tmbName, librarySettings) {
                librarySettings = librarySettings || 'tmb-';

                if (tmbName) {
                    var parts = tmbDefaultUrl.split('.');
                    if (parts.length > 1) {
                        var url = '';
                        for (var i = 0; i < parts.length; i++) {
                            if (url.length > 0)
                                url = url + '.';
                            if (parts[i].indexOf(librarySettings.ThumbnailExtensionPrefix) === 0)
                                url = url + librarySettings.ThumbnailExtensionPrefix + tmbName;
                            else
                                url = url + parts[i];
                        }
                        return url;
                    }
                }

                return tmbDefaultUrl;
            };

            var stripPxFromStyle = function (style) {
                if (!style || style.length < 2)
                    return style;

                if (style.substr(style.length - 2, 2).toLowerCase() === 'px') {
                    return style.substr(0, style.length - 2);
                }
                else {
                    return style;
                }
            };

            var image = {
                markup: function (properties, librarySettings, wrapIt) {
                    var sfref = '';
                    var src = '';
                    if (properties.displayMode === 'Thumbnail') {
                        sfref = getSfrefAttribute('images', properties.item.Id, properties.provider, properties.thumbnail.name);
                        src = resolveThumbnailUrl(properties.thumbnail.url, properties.thumbnail.name, librarySettings);
                    } else {
                        sfref = getSfrefAttribute('images', properties.item.Id, properties.provider);
                        src = properties.item.MediaUrl;
                    }

                    var jElementToInsert = $('<img />');
                    jElementToInsert.attr('sfref', sfref);
                    jElementToInsert.attr('src', src);

                    jElementToInsert.attr('alt', properties.alternativeText);
                    if (properties.title) {
                        jElementToInsert.attr('title', properties.title);
                    }
                    else if (properties.item.Title) {
                        jElementToInsert.attr('title', properties.item.Title);
                    }

                    if (properties.displayMode)
                        jElementToInsert.attr('displayMode', properties.displayMode);

                    jElementToInsert.css('float', '');
                    jElementToInsert.css('vertical-align', '');

                    switch (properties.alignment) {
                        case 'Left':
                            jElementToInsert.css('float', 'left');
                            break;
                        case 'Right':
                            jElementToInsert.css('float', 'right');
                            break;
                        case 'Center':
                            jElementToInsert.css('vertical-align', 'middle');
                            break;
                        default:
                            break;
                    }

                    jElementToInsert[0].style.margin = '';
                    if (properties.margin.top !== null)
                        jElementToInsert[0].style.marginTop = properties.margin.top + 'px';
                    if (properties.margin.bottom !== null)
                        jElementToInsert[0].style.marginBottom = properties.margin.bottom + 'px';
                    if (properties.margin.left !== null)
                        jElementToInsert[0].style.marginLeft = properties.margin.left + 'px';
                    if (properties.margin.right !== null)
                        jElementToInsert[0].style.marginRight = properties.margin.right + 'px';

                    if (properties.openOriginalImageOnClick) {
                        jElementToInsert.attr('openOriginalImageOnClick', 'true');
                        jElementToInsert.wrap('<a></a>');
                        jElementToInsert = jElementToInsert.parent();
                        jElementToInsert.attr('sfref', sfref).attr('href', properties.item.MediaUrl);
                    }

                    if (wrapIt) {
                        var jSpanWrapper = $('<span />').attr('data-sfref', sfref).addClass('sfImageWrapper');
                        jSpanWrapper.append(jElementToInsert);
                        jElementToInsert = jSpanWrapper;
                    }

                    return jElementToInsert[0].outerHTML;
                },

                properties: function (markup) {
                    var jMarkup = $(markup);
                    var sfref = jMarkup.attr('sfref') ? jMarkup.attr('sfref') : jMarkup.children().attr('sfref');
                    
                    if (!jMarkup.is('img'))
                        jMarkup = jMarkup.find('img');

                    var result = new ImageProperties();
                    result.item.Id = getIdFromSfrefAttr(sfref);
                    result.provider = getProviderFromSfrefAttr(sfref);
                    result.thumbnail.name = getThumbnailNameFromSfrefAttr(sfref);
                    result.displayMode = jMarkup.attr('displayMode');

                    if (result.displayMode === 'Thumbnail') {
                        result.thumbnail.url = jMarkup.attr('src');
                    }
                    else {
                        result.item.MediaUrl = jMarkup.attr('src');
                    }

                    result.title = jMarkup.attr('title');
                    result.alternativeText = jMarkup.attr('alt');

                    if (jMarkup.css('vertical-align') === 'middle') {
                        result.alignment = 'Center';
                    }
                    else {
                        switch (jMarkup.css('float')) {
                            case 'left':
                                result.alignment = 'Left';
                                break;
                            case 'right':
                                result.alignment = 'Right';
                                break;
                            default:
                                result.alignment = 'None';
                                break;
                        }
                    }

                    result.margin.top = stripPxFromStyle(jMarkup[0].style.marginTop);
                    result.margin.left = stripPxFromStyle(jMarkup[0].style.marginLeft);
                    result.margin.bottom = stripPxFromStyle(jMarkup[0].style.marginBottom);
                    result.margin.right = stripPxFromStyle(jMarkup[0].style.marginRight);

                    result.openOriginalImageOnClick = jMarkup.attr('openOriginalImageOnClick') == 'true';

                    return result;
                }
            };

            return {
                image: image
            };
        }]);
})(jQuery);