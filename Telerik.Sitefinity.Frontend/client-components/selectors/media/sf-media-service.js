(function () {
    angular.module('sfServices').factory('sfMediaService', ['$http', 'serviceHelper', 'serverContext', '$interpolate', '$q', '$window', function ($http, serviceHelper, serverContext, $interpolate, $q, $window) {
        var constants = {
            images: {
                itemType: 'Telerik.Sitefinity.Libraries.Model.Image',
                parentItemType: 'Telerik.Sitefinity.Libraries.Model.Album',
                parentServiceUrl: serverContext.getRootedUrl('Sitefinity/Services/Content/AlbumService.svc/'),
                serviceUrl: serverContext.getRootedUrl('Sitefinity/Services/Content/ImageService.svc/'),
                createItemUrl: serverContext.getRootedUrl('Sitefinity/Services/Content/ImageService.svc/parent/{{libraryId}}/{{itemId}}/?itemType={{itemType}}&provider={{provider}}&parentItemType={{parentItemType}}&newParentId={{newParentId}}'),
                settingsNodeName: 'Images_0,librariesConfig_0',
                extensionsRegExPrefix: 'image',
                fileFormField: 'ImageFile'
            },
            documents: {
                itemType: 'Telerik.Sitefinity.Libraries.Model.Document',
                parentItemType: 'Telerik.Sitefinity.Libraries.Model.DocumentLibrary',
                parentServiceUrl: serverContext.getRootedUrl('Sitefinity/Services/Content/DocumentLibraryService.svc/'),
                serviceUrl: serverContext.getRootedUrl('Sitefinity/Services/Content/DocumentService.svc/'),
                createItemUrl: serverContext.getRootedUrl('Sitefinity/Services/Content/DocumentService.svc/parent/{{libraryId}}/{{itemId}}/?itemType={{itemType}}&provider={{provider}}&parentItemType={{parentItemType}}&newParentId={{newParentId}}'),
                settingsNodeName: 'Documents_0,librariesConfig_0',
                extensionsRegExPrefix: 'document',
                fileFormField: 'DocumentFile'
            },
            videos: {
                itemType: 'Telerik.Sitefinity.Libraries.Model.Video',
                parentItemType: 'Telerik.Sitefinity.Libraries.Model.VideoLibrary',
                parentServiceUrl: serverContext.getRootedUrl('Sitefinity/Services/Content/VideoLibraryService.svc/'),
                serviceUrl: serverContext.getRootedUrl('Sitefinity/Services/Content/VideoService.svc/'),
                createItemUrl: serverContext.getRootedUrl('Sitefinity/Services/Content/VideoService.svc/parent/{{libraryId}}/{{itemId}}/?itemType={{itemType}}&provider={{provider}}&parentItemType={{parentItemType}}&newParentId={{newParentId}}'),
                settingsNodeName: 'Videos_2,librariesConfig_0',
                extensionsRegExPrefix: 'video',
                fileFormField: 'VideoFile'
            },
            uploadHandlerUrl: serverContext.getRootedUrl('Telerik.Sitefinity.Html5UploadHandler.ashx'),
            librarySettingsServiceUrl: serverContext.getRootedUrl('Sitefinity/Services/Configuration/ConfigSectionItems.svc/'),
            thumbnailServiceUrl: serverContext.getRootedUrl('Sitefinity/Services/ThumbnailService.svc/')
        };

        var getById = function (id, provider, itemType, serviceUrl) {
            var url = serviceUrl + id;

            return serviceHelper.getResource(url).get(
                {
                    provider: provider,
                    itemType: itemType,
                    published: true
                }).$promise;
        };

        var getItems = function (options, excludeFolders, serviceUrl, itemType) {
            options = options || {};
            excludeFolders = excludeFolders ? excludeFolders : options.excludeFolders;

            var url = options.parent ? serviceUrl + 'parent/' + options.parent + "/" : serviceUrl;

            return serviceHelper.getResource(url).get(
                {
                    itemType: itemType,
                    filter: options.filter,
                    provider: options.provider,
                    skip: options.skip,
                    take: options.take,
                    sortExpression: options.sort,
                    includeSubFoldersItems: options.recursive ? 'true' : null,
                    excludeFolders: excludeFolders
                }).$promise;
        };

        var getFolders = function (options, serviceUrl) {
            options = options || {};

			var foldersUrl = serviceUrl + "folders/";
            var url = options.parent ? foldersUrl + options.parent + "/" : foldersUrl;

            return serviceHelper.getResource(url).get(
                {
                    filter: options.filter,
                    provider: options.provider,
                    skip: options.skip,
                    take: options.take,
                    sortExpression: options.sort,
                    hierarchyMode: options.recursive ? null : 'true',
                    excludeNeighbours: options.excludeNeighbours
                }).$promise.then(function (data) {
                    data.Items.map(function (obj) {
                        obj.IsFolder = true;
                    });
                    return data;
                });
        };

        var getEmptyArray = function () {
            var defer = $q.defer();
            defer.resolve([]);
            return defer.promise;
        };

        var toWcfDate = function (date) {
            return '/Date(' + date.getTime() + '-0000)/';
        };

        var createItem = function (settings, mediaType) {
            var nowToWcfDate = toWcfDate(new Date()),
                url = $interpolate(constants[mediaType].createItemUrl)(settings),
                item = {
                    Item: {
                        Title: {
                            PersistedValue: settings.title,
                            Value: settings.title
                        },
                        AlternativeText: {
                            PersistedValue: settings.alternativeText,
                            Value: settings.alternativeText
                        },
                        DateCreated: nowToWcfDate,
                        PublicationDate: nowToWcfDate,
                        LastModified: nowToWcfDate,
                        Tags: settings.tags,
                        Category: settings.categories
                    },
                    ItemType: constants[mediaType].itemType
                };

            return serviceHelper.getResource(url).put(item).$promise;
        };

        var uploadFile = function (url, formData) {
            
            var deferred = $q.defer();
            var xhr = new $window.XMLHttpRequest();
            xhr.onload = function (e) {
                if (xhr.readyState === 4) {
                    if (xhr.status === 200) {
                        deferred.resolve(JSON.parse(xhr.responseText));
                    } else {
                        deferred.reject(xhr.statusText);
                    }
                }
            };
            xhr.upload.onprogress = function (e) {
                var done = e.position || e.loaded,
                    total = e.totalSize || e.total,
                    present = Math.floor(done / total * 100);
                deferred.notify(present);
            };
            xhr.onerror = function (e) {
                deferred.reject(xhr.statusText);
            };

            xhr.open('POST', url);
            xhr.send(formData);

            return deferred.promise;
        };

        var uploadItem = function (settings, mediaType) {
            return createItem(settings, mediaType)
            .then(function (data) {
                var formData = new FormData();
                formData.append('ContentType', constants[mediaType].itemType);
                formData.append('LibraryId', settings.libraryId);
                formData.append('ContentId', data.Item.Id);
                formData.append('Workflow', 'Upload');
                formData.append('ProviderName', settings.provider || '');
                formData.append('SkipWorkflow', 'true');

                if (serverContext.getUICulture()) {
                    formData.append('Culture', serverContext.getUICulture());
                }

                formData.append(constants[mediaType].fileFormField, settings.file);

                return uploadFile(constants.uploadHandlerUrl, formData);
            })
            .catch(function (error) {
                throw error;
            });
        };

        var thumbnailProfiles = function (libraryType) {
            var thumbnailProfilesServiceUrl = constants.thumbnailServiceUrl + 'thumbnail-profiles/';
            return serviceHelper.getResource(thumbnailProfilesServiceUrl).get({ libraryType: libraryType }).$promise;
        };

        var checkCustomThumbnailParams = function (methodName, params) {
            var checkThumbnailParamsServiceUrl = constants.thumbnailServiceUrl + 'custom-image-thumbnail/checkParameters';
            return serviceHelper.getResource(checkThumbnailParamsServiceUrl).get({ methodName: methodName, parameters: params }).$promise;
        };

        var getCustomThumbnailUrl = function (imageId, customUrlParams, libraryProvider) {
            params = JSON.stringify(customUrlParams);
            var customThumbnailUrlService = String.format('{0}custom-image-thumbnail/url?imageId={1}&customUrlParameters={2}&libraryProvider={3}', constants.thumbnailServiceUrl, imageId, params, libraryProvider);
            var deferred = $q.defer();
            jQuery.ajax({
                type: 'GET',
                url: customThumbnailUrlService,
                processData: false,
                contentType: "application/json",
                success: function (thumbnailUrl) {
                    deferred.resolve(thumbnailUrl);
                },
                error: function (e) {
                    deferred.reject(e);
                }
            });

            return deferred.promise;
        };

        var createMediaApi = function  (mediaType) {
            var mediaSettings = null;

            return {
                getById: function (id, provider) {
                    return getById(id, provider, constants[mediaType].itemType, constants[mediaType].serviceUrl);
                },
                getFolders: function (options) {
                    return getFolders(options, constants[mediaType].parentServiceUrl);
                },
                getMedia: function (options) {
                    return getItems(options, 'true', constants[mediaType].serviceUrl, constants[mediaType].itemType);
                },
                getContent: function (options) {
                    return getItems(options, null, constants[mediaType].serviceUrl, constants[mediaType].itemType);
                },
                get: function (options, filterObject, appendItems) {
                    var callback;
                    if (filterObject.query) {
                        callback = this.getContent;
                    }
                    else if (filterObject.basic) {
                        // Defaul filter is used (Recent / My / All)
                        if (filterObject.basic === filterObject.constants.basic.recentItems) {
                            // When the filter is Recent items, the number of displayed items is fixed and we should not append more.
                            if (appendItems) {
                                callback = getEmptyArray;
                            }
                            else {
                                callback = this.getMedia;
                            }
                        }
                        else if (filterObject.basic === filterObject.constants.basic.ownItems) {
                            callback = this.getMedia;
                        }
                        else if (filterObject.basic === filterObject.constants.basic.allLibraries) {
                            callback = this.getFolders;
                        }
                        else {
                            throw { message: 'Unknown basic filter object option.' };
                        }
                    }
                    else {
                        // custom filter is used (Libraries / Taxons / Dates)
                        callback = this.getContent;
                    }

                    return getLibrarySettings()
                        .then(function (settings) {
                            var allLanguageSearch = settings.EnableAllLanguagesSearch.toLowerCase() === 'true';
                            options.filter = filterObject.composeExpression(allLanguageSearch);

                            var selectedFolderSearch = settings.EnableSelectedFolderSearch.toLowerCase() === 'true';
                            if (filterObject.query) {
                                if (selectedFolderSearch) {
                                    options.parent = filterObject.parent;
                                    options.recursive = true;
                                    options.excludeFolders = true;
                                }
                                else {
                                    options.parent = null;
                                    options.recursive = false;
                                    options.excludeFolders = false;
                                }
                            }

                            return callback(options);
                        });
                },
                getPredecessorsFolders: function (id, provider) {
                    if (!id) {
                        return;
                    }
                    var options = {
                        parent: 'predecessors/' + id,
                        excludeNeighbours: true,
                        provider: provider
                    };
                    return getFolders(options, constants[mediaType].parentServiceUrl)
                              .then(function (data) {
                                  return data.Items;
                              });
                },
                upload: function (model, provider) {

                    var defaultLibraryId = '4ba7ad46-f29b-4e65-be17-9bf7ce5ba1fb';
                    var libraryId = model.parentId || defaultLibraryId;

                    var settings = {
                        libraryId: libraryId,
                        newParentId: libraryId,
                        itemId: serviceHelper.emptyGuid(),
                        itemType: constants[mediaType].itemType,
                        provider: provider,
                        parentItemType: constants[mediaType].parentItemType,
                        title: model.title || model.file.name,
                        alternativeText: model.alternativeText,
                        categories: model.categories,
                        tags: model.tags,
                        file: model.file
                    };
                    return uploadItem(settings, mediaType);
                },
                thumbnailProfiles: function () {
                    return thumbnailProfiles(constants[mediaType].parentItemType);
                },
                getSettings: function () {
                    if (mediaSettings === null) {
                        var url = constants.librarySettingsServiceUrl;
                        return serviceHelper.getResource(url).get(
                            {
                                nodeName: constants[mediaType].settingsNodeName,
                                mode: 'Form'
                            })
                            .$promise
                            .then(function (data) {
                                mediaSettings = {};
                                for (var i = 0; i < data.Items.length; i++) {
                                    mediaSettings[data.Items[i].Key] = data.Items[i].Value;
                                }
                                if (mediaSettings.AllowedExensionsSettings) {
                                    var mediaExt = mediaSettings.AllowedExensionsSettings.replace(/,/g, '|').replace(/ |\./g, '');
                                    var regExp = '^' + constants[mediaType].extensionsRegExPrefix + '\/(' + mediaExt + ')$';
                                    mediaSettings.AllowedExensionsRegex = new RegExp(regExp, 'i');
                                }
                                return mediaSettings;
                            });
                    }
                    else {
                        var deferred = $q.defer();
                        deferred.resolve(mediaSettings);
                        return deferred.promise;
                    }
                }
            };
        };

        var librarySettings = null;
        var getLibrarySettings = function () {
            if (librarySettings === null) {
                var url = constants.librarySettingsServiceUrl;
                return serviceHelper.getResource(url).get(
                    {
                        nodeName: 'librariesConfig_0',
                        mode: 'Form'
                    })
                    .$promise
                    .then(function (data) {
                        librarySettings = {};
                        for (var i = 0; i < data.Items.length; i++) {
                            librarySettings[data.Items[i].Key] = data.Items[i].Value;
                        }

                        return librarySettings;
                    });
            }
            else {
                var deferred = $q.defer();
                deferred.resolve(librarySettings);
                return deferred.promise;
            }
        };

        return {
            images: createMediaApi('images'),
            documents: createMediaApi('documents'),
            videos: createMediaApi('videos'),
            getLibrarySettings: getLibrarySettings,
            checkCustomThumbnailParams: checkCustomThumbnailParams,
            getCustomThumbnailUrl: getCustomThumbnailUrl
        };
    }]);
})();