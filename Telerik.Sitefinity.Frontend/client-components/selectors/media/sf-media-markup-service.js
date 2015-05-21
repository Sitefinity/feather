; (function ($) {
    var parseMargin = function (val) {
        if (val !== null && val !== undefined && val !== 'auto') {
            return parseInt(val);
        }
        else {
            return null;
        }
    };

    angular.module('sfServices')
        .factory('sfMediaMarkupService', [function () {
            var ImageProperties = function () {
                this.item = { Id: null }; //MediaItem view model
                this.provider = null; //Name of the data provider
                this.displayMode = null; //Original, Thumbnail, Custom

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

                this.cssClass = null;

                this.openOriginalImageOnClick = false;
            };

            var DocumentProperties = function () {
                this.item = { Id: null }; //MediaItem view model
                this.provider = null; //Name of the data provider
                this.title = null;
                this.cssClass = null;
            };

            var VideoProperties = function () {
                this.item = { Id: null }; //MediaItem view model
                this.provider = null; //Name of the data provider
                this.width = null;
                this.height = null;
                this.cssClass = null;
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
                librarySettings = librarySettings || { ThumbnailExtensionPrefix: 'tmb-' };

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

            var escapeDoubleQuote = function (str) {
                if (str)
                    return str.replace(/"/g, "\'");

                return str;
            };

            var unescapeDoubleQuote = function (str) {
                if (str)
                    return str.replace(/'/g, '"');

                return str;
            };

            var image = {
                markup: function (properties, librarySettings, wrapIt) {
                    var sfref = '';
                    var src = '';
                    if (properties.displayMode === 'Thumbnail') {
                        sfref = getSfrefAttribute('images', properties.item.Id, properties.provider, properties.thumbnail.name);
                        src = resolveThumbnailUrl(properties.thumbnail.url || properties.item.ThumbnailUrl, properties.thumbnail.name, librarySettings);
                    } else {
                        sfref = getSfrefAttribute('images', properties.item.Id, properties.provider);
                        src = properties.item.MediaUrl;
                    }

                    var jElementToInsert = $('<img />');
                    jElementToInsert.attr('sfref', sfref);

                    if (properties.displayMode === 'Custom') {
                        src = properties.thumbnail.url;
                        jElementToInsert.attr('data-method', properties.customSize.Method);
                        jElementToInsert.attr('data-customsizemethodproperties', escapeDoubleQuote(JSON.stringify(properties.customSize)));
                    }

                    jElementToInsert.attr('src', src);

                    jElementToInsert.attr('alt', properties.alternativeText);
                    jElementToInsert.attr('class', properties.cssClass);
                    if (properties.title) {
                        jElementToInsert.attr('title', properties.title);
                    }

                    if (properties.displayMode)
                        jElementToInsert.attr('data-displayMode', properties.displayMode);

                    properties.margin = properties.margin || {};

                    jElementToInsert[0].style.margin = '';
                    if (properties.margin.top !== null)
                        jElementToInsert[0].style.marginTop = properties.margin.top + 'px';
                    if (properties.margin.bottom !== null)
                        jElementToInsert[0].style.marginBottom = properties.margin.bottom + 'px';
                    if (properties.margin.left !== null)
                        jElementToInsert[0].style.marginLeft = properties.margin.left + 'px';
                    if (properties.margin.right !== null)
                        jElementToInsert[0].style.marginRight = properties.margin.right + 'px';

                    switch (properties.alignment) {
                        case 'Left':
                            jElementToInsert.css('float', 'left');
                            break;
                        case 'Right':
                            jElementToInsert.css('float', 'right');
                            break;
                        case 'Center':
                            jElementToInsert.css({ 'display': 'block', 'margin-left': 'auto', 'margin-right': 'auto' });

                            break;
                        default:
                            break;
                    }

                    if (properties.openOriginalImageOnClick) {
                        jElementToInsert.attr('data-openOriginalImageOnClick', 'true');
                        jElementToInsert.wrap('<a></a>');
                        jElementToInsert = jElementToInsert.parent();
                        jElementToInsert.attr('sfref', sfref).attr('href', properties.item.MediaUrl);
                    }

                    if (wrapIt) {
                        var jSpanWrapper = $('<span />').attr('data-sfref', sfref).addClass('sf-Image-wrapper');
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
                    result.displayMode = jMarkup.attr('data-displayMode') || jMarkup.attr('displayMode');

                    if (result.displayMode === 'Thumbnail') {
                        result.thumbnail.url = jMarkup.attr('src');
                    }
                    else {
                        result.item.MediaUrl = jMarkup.attr('src');
                    }

                    if (result.displayMode === 'Custom') {
                        result.thumbnail.url = jMarkup.attr('src');
                        var customSizeProperties = jMarkup.attr('data-customsizemethodproperties') || jMarkup.attr('customsizemethodproperties');
                        result.customSize = JSON.parse(unescapeDoubleQuote(customSizeProperties));
                        result.customSize.Method = jMarkup.attr('data-method') || jMarkup.attr('method');
                    }

                    result.title = jMarkup.attr('title');
                    result.alternativeText = jMarkup.attr('alt');
                    result.cssClass = jMarkup.attr('class') || null;

                    if (jMarkup.css('display') === 'block' && jMarkup[0].style.marginLeft === 'auto' && jMarkup[0].style.marginRight === 'auto') {
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

                    if (jMarkup && jMarkup[0] && jMarkup[0].style) {
                        result.margin.top = parseMargin(stripPxFromStyle(jMarkup[0].style.marginTop));
                        result.margin.left = parseMargin(stripPxFromStyle(jMarkup[0].style.marginLeft));
                        result.margin.bottom = parseMargin(stripPxFromStyle(jMarkup[0].style.marginBottom));
                        result.margin.right = parseMargin(stripPxFromStyle(jMarkup[0].style.marginRight));
                    }

                    var openOriginal = jMarkup.attr('data-openOriginalImageOnClick') || jMarkup.attr('openOriginalImageOnClick');
                    result.openOriginalImageOnClick = openOriginal === 'true';

                    return result;
                }
            };

            var document = {
                markup: function (properties, librarySettings) {
                    var sfref = '';
                    var href = '';
                    sfref = getSfrefAttribute('documents', properties.item.Id, properties.provider);
                    href = properties.item.MediaUrl;

                    var jElementToInsert = jQuery('<a />');
                    jElementToInsert.attr('sfref', sfref);
                    jElementToInsert.attr('href', href);
                    jElementToInsert.attr('class', properties.cssClass);

                    if (properties.title) {
                        jElementToInsert.attr('title', properties.title);
                        jElementToInsert.text(properties.title);
                    }

                    return jElementToInsert[0].outerHTML;
                },

                properties: function (markup) {
                    var jMarkup = jQuery(markup);
                    var sfref = jMarkup.attr('sfref') ? jMarkup.attr('sfref') : jMarkup.children().attr('sfref');

                    if (!jMarkup.is('a'))
                        jMarkup = jMarkup.find('a');

                    var result = new DocumentProperties();

                    result.item.Id = getIdFromSfrefAttr(sfref);
                    result.provider = getProviderFromSfrefAttr(sfref);
                    result.item.MediaUrl = jMarkup.attr('href');
                    result.title = jMarkup.attr('title');
                    result.cssClass = jMarkup.attr('class') || null;

                    return result;
                }
            };

            var video = {
                markup: function (properties, librarySettings) {
                    var sfref = '';
                    var src = '';
                    sfref = getSfrefAttribute('videos', properties.item.Id, properties.provider);
                    src = properties.item.MediaUrl;

                    var jElementToInsert = jQuery('<video />');
                    jElementToInsert.attr('sfref', sfref);
                    jElementToInsert.attr('src', src);
                    jElementToInsert.attr('class', properties.cssClass);
                    jElementToInsert.attr('width', properties.width);
                    jElementToInsert.attr('height', properties.height);

                    if (properties.margin) {
                        if (properties.margin.left)
                            jElementToInsert.css('margin-left', properties.margin.left + 'px');

                        if (properties.margin.right)
                            jElementToInsert.css('margin-right', properties.margin.right + 'px');

                        if (properties.margin.top)
                            jElementToInsert.css('margin-top', properties.margin.top + 'px');

                        if (properties.margin.bottom)
                            jElementToInsert.css('margin-bottom', properties.margin.bottom + 'px');
                    }

                    jElementToInsert.attr('controls', true);

                    return jElementToInsert[0].outerHTML;
                },

                properties: function (markup) {
                    var jMarkup = jQuery(markup);
                    var sfref = jMarkup.attr('sfref') ? jMarkup.attr('sfref') : jMarkup.children().attr('sfref');

                    var result = new VideoProperties();

                    result.item.Id = getIdFromSfrefAttr(sfref);
                    result.provider = getProviderFromSfrefAttr(sfref);
                    result.item.MediaUrl = jMarkup.attr('href');
                    result.title = jMarkup.attr('title');
                    result.cssClass = jMarkup.attr('class') || null;

                    result.margin = {
                        left: jMarkup.css('margin-left'),
                        right: jMarkup.css('margin-right'),
                        top: jMarkup.css('margin-top'),
                        bottom: jMarkup.css('margin-bottom')
                    };

                    return result;
                }
            };

            return {
                image: image,
                document: document,
                video: video
            };
        }]);
})(jQuery);
