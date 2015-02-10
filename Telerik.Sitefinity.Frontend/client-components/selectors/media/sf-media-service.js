(function () {
    angular.module('sfServices').factory('sfMediaService', ['serviceHelper', 'serverContext', '$q', function (serviceHelper, serverContext, $q) {
        var constants = {
            images: {
                itemType: 'Telerik.Sitefinity.Libraries.Model.Image',
                albumsServiceUrl: serverContext.getRootedUrl('Sitefinity/Services/Content/AlbumService.svc/folders/'),
                imagesServiceUrl: serverContext.getRootedUrl('Sitefinity/Services/Content/ImageService.svc/')
            },
            librarySettingsServiceUrl: serverContext.getRootedUrl('Sitefinity/Services/Configuration/ConfigSectionItems.svc/')
        };

        var getItems = function (options, excludeFolders, serviceUrl, itemType) {
            options = options || {};

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

        var imagesObj = {
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

                var callbackWrap = function () {
                    var allLanguageSearch = enableAllLanguagesSearch.toLowerCase() === "true";
                    options.filter = filterObject.composeExpression(allLanguageSearch);
                    return callback(options);
                };

                if (enableAllLanguagesSearch === null) {
                    return getEnableAllLanguagesSearch()
                        .then(callbackWrap);
                }
                else {
                    return callbackWrap();
                }
            },
            getPredecessorsFolders: function (id) {
                if (!id) {
                    return;
                }
                var options = {
                    parent: 'predecessors/' + id,
                    excludeNeighbours: true
                };
                return getFolders(options, constants.images.albumsServiceUrl)
                          .then(function (data) {
                              return data.Items;
                          });
            }
        };

        var enableAllLanguagesSearch = null;

        var getLibrarySettigns = function () {
            var url = constants.librarySettingsServiceUrl;
            return serviceHelper.getResource(url).get(
                {
                    nodeName: 'librariesConfig_0',
                    mode: 'Form'
                }).$promise;
        };

        var getEnableAllLanguagesSearch = function () {
            if (enableAllLanguagesSearch === null) {
                return getLibrarySettigns().then(function (data) {
                    if (data && data.Items) {
                        enableAllLanguagesSearch = data.Items.filter(function (item) { return item.Key == "EnableAllLanguagesSearch"; })[0].Value;
                    }
                    else {
                        enableAllLanguagesSearch = false;
                    }

                    return enableAllLanguagesSearch;
                });
            } else {
                var deferred = $q.defer();
                deferred.resolve(enableAllLanguagesSearch);
                return deferred.promise;
            }
        };

        return {
            images: imagesObj
        };
    }]);
})();