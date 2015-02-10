; (function ($) {
    angular.module('sfServices')
        .factory('sfMediaMarkupService', ['sfMediaService', function (mediaService) {
            // TODO: This class is currently not used. For now it is here just for clarity. Remove if not needed after full integration.
            var ImageProperties = function () {
                this.item = null; //MediaItem view model
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

            var resolveThumbnailUrl = function (tmbDefaultUrl, tmbName, librarySettings) {
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

            var image = {
                markup: function (properties, librarySettings) {
                    var sfref = '';
                    var src = '';
                    if (properties.displayMode === 'Thumbnail') {
                        sfref = getSfrefAttribute('images', properties.item.Id, properties.provider, properties.thumbnail.name);
                        src = resolveThumbnailUrl(properties.thumbnail.url, properties.thumbnail.name, librarySettings);
                    } else {
                        sfref = getSfrefAttribute('images', properties.item.Id);
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

                    var jSpanWrapper = $('<span />').attr('data-sfref', sfref).addClass('sfImageWrapper');
                    jSpanWrapper.append(jElementToInsert);
                    jElementToInsert = jSpanWrapper;

                    return jElementToInsert[0].outerHTML;
                },

                properties: function (markup, librarySettings) {

                }
            };

            return {
                image: image
            };
        }]);
})(jQuery);