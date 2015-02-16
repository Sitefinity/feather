(function () {
    angular.module('sfServices').factory('sfMediaService', ['$http', 'serviceHelper', 'serverContext', '$interpolate', '$q', '$window', function ($http, serviceHelper, serverContext, $interpolate, $q, $window) {
        var constants = {
            images: {
                itemType: 'Telerik.Sitefinity.Libraries.Model.Image',
                albumItemType: 'Telerik.Sitefinity.Libraries.Model.Album',
                albumsServiceUrl: serverContext.getRootedUrl('Sitefinity/Services/Content/AlbumService.svc/'),
                imagesServiceUrl: serverContext.getRootedUrl('Sitefinity/Services/Content/ImageService.svc/'),
                createImageUrl: serverContext.getRootedUrl('Sitefinity/Services/Content/ImageService.svc/parent/{{libraryId}}/{{itemId}}/?itemType={{itemType}}&provider={{provider}}&parentItemType={{parentItemType}}&newParentId={{newParentId}}')
            },
            uploadHandlerUrl: serverContext.getRootedUrl('Telerik.Sitefinity.Html5UploadHandler.ashx'),
            librarySettingsServiceUrl: serverContext.getRootedUrl('Sitefinity/Services/Configuration/ConfigSectionItems.svc/'),
            thumbnailService: serverContext.getRootedUrl('Sitefinity/Services/ThumbnailService.svc/thumbnail-profiles/')
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
                    includeSubFolderItems: options.recursive ? 'true' : null,
                    excludeFolders: excludeFolders
                }).$promise;
        };

        var getFolders = function (options, serviceUrl) {
            options = options || {};

            var url = options.parent ? serviceUrl + options.parent + "/" : serviceUrl;
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

        var createImage = function (settings) {
            var nowToWcfDate = toWcfDate(new Date()),
                url = $interpolate(constants.images.createImageUrl)(settings),
                image = {
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
                    ItemType: constants.images.itemType
                };

            return serviceHelper.getResource(url).put(image).$promise;
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

        var uploadImage = function (settings) {
            return createImage(settings)
            .then(function (data) {
                var formData = new FormData();
                formData.append('ContentType', constants.images.itemType);
                formData.append('LibraryId', settings.libraryId);
                formData.append('ContentId', data.Item.Id);
                formData.append('Workflow', 'Upload');
                formData.append('ProviderName', settings.provider || '');
                formData.append('SkipWorkflow', 'true');

                if (serverContext.getUICulture()) {
                    formData.append('Culture', serverContext.getUICulture());
                }

                formData.append('ImageFile', settings.file);

                return uploadFile(constants.uploadHandlerUrl, formData);
            })
            .catch(function (error) {
                throw error;
            });
        };

        var thumbnailProfiles = function (libraryType) {
            return serviceHelper.getResource(constants.thumbnailService).get({ libraryType: libraryType }).$promise;
        };

        var imagesObj = {
            getById: function (id, provider) {
                return getById(id, provider, constants.images.itemType, constants.images.imagesServiceUrl);
            },
            getFolders: function (options) {
                return getFolders(options, constants.images.albumsServiceUrl);
            },
            getMedia: function (options) {
                return getItems(options, 'true', constants.images.imagesServiceUrl, constants.images.itemType);
            },
            getContent: function (options) {
                return getItems(options, null, constants.images.imagesServiceUrl, constants.images.itemType);
            },
            get: function (options, filterObject, appendItems) {
                var callback;
                if (filterObject.query) {
                    callback = imagesObj.getContent;
                }
                else if (filterObject.basic) {
                    // Defaul filter is used (Recent / My / All)
                    if (filterObject.basic === filterObject.constants.basic.recentItems) {
                        // When the filter is Recent items, the number of displayed items is fixed and we should not append more.
                        if (appendItems) {
                            callback = getEmptyArray;
                        }
                        else {
                            callback = imagesObj.getMedia;
                        }
                    }
                    else if (filterObject.basic === filterObject.constants.basic.ownItems) {
                        callback = imagesObj.getMedia;
                    }
                    else if (filterObject.basic === filterObject.constants.basic.allLibraries) {
                        callback = imagesObj.getFolders;
                    }
                    else {
                        throw { message: 'Unknown basic filter object option.' };
                    }
                }
                else {
                    // custom filter is used (Libraries / Taxons / Dates)
                    callback = imagesObj.getContent;
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
            getPredecessorsFolders: function (id) {
                if (!id) {
                    return;
                }
                var options = {
                    parent: 'folders/predecessors/' + id,
                    excludeNeighbours: true
                };
                return getFolders(options, constants.images.albumsServiceUrl)
                          .then(function (data) {
                              return data.Items;
                          });
            },
            upload: function (model) {

                var defaultLibraryId = '4ba7ad46-f29b-4e65-be17-9bf7ce5ba1fb';
                var libraryId = model.parentId || defaultLibraryId;

                var settings = {
                    libraryId: libraryId,
                    newParentId: libraryId,
                    itemId: serviceHelper.emptyGuid(),
                    itemType: constants.images.itemType,
                    provider: model.provider,
                    parentItemType: constants.images.albumItemType,
                    title: model.title || model.file.name,
                    alternativeText: model.alternativeText,
                    categories: model.categories,
                    tags: model.tags,
                    file: model.file
                };
                return uploadImage(settings);
            },
            thumbnailProfiles: function () {
                return thumbnailProfiles(constants.images.albumItemType);
            }
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

        var imagesSettings = null;
        var getImagesSettings = function () {
            if (imagesSettings === null) {
                var url = constants.librarySettingsServiceUrl;
                return serviceHelper.getResource(url).get(
                    {
                        nodeName: 'Images_0,librariesConfig_0',
                        mode: 'Form'
                    })
                    .$promise
                    .then(function (data) {
                        imagesSettings = {};
                        for (var i = 0; i < data.Items.length; i++) {
                            imagesSettings[data.Items[i].Key] = data.Items[i].Value;
                        }
                        if (imagesSettings.AllowedExensionsSettings) {
                            var imagesExt = imagesSettings.AllowedExensionsSettings.replace(/,/g, '|').replace(/ |\./g, '');
                            var regExp = '^image\/(' + imagesExt + ')$';
                            imagesSettings.AllowedExensionsRegex = new RegExp(regExp, 'i');
                        }
                        return imagesSettings;
                    });
            }
            else {
                var deferred = $q.defer();
                deferred.resolve(imagesSettings);
                return deferred.promise;
            }
        };

        return {
            images: imagesObj,
            getLibrarySettings: getLibrarySettings,
            getImagesSettings: getImagesSettings
        };
    }]);
})();