(function () {
    angular.module('sfServices').factory('sfMediaService', ['serviceHelper', 'serverContext', function (serviceHelper, serverContext) {
        var constants = {
            images: {
                itemType: 'Telerik.Sitefinity.Libraries.Model.Image',
                albumsServiceUrl: serverContext.getRootedUrl('Sitefinity/Services/Content/AlbumService.svc/folders/'),
                imagesServiceUrl: serverContext.getRootedUrl('Sitefinity/Services/Content/ImageService.svc/')
            }
        };

        var TaxonFilter = function () {
            this.id = null;
            this.field = null;

            this.composeExpression = function () {
                return this.field + '.Contains({' + this.id + '})';
            };
        };

        var MediaFilter = function () {
            var self = this;
            var events = [];

            this.constants = {
                basic: {
                    allLibraries: 'AllLibraries',
                    ownItems: 'OwnItems',
                    recentItems: 'RecentItems'
                },
                dateCreatedDescending: 'DateCreated DESC',
                anyTimeValue: 'AnyTime'
            };

            // Query that is typed by a user in a text box.
            this.query = null;

            // RecentItems, OwnItems or AllLibraries
            this.basic = null;

            // Parent id
            this.parent = null;

            // Number of days since modified
            this.date = null;

            // Filter by any taxon
            this.taxon = new TaxonFilter();

            this.attachEvent = function (callback) {
                if (typeof callback === 'function') {
                    events.push(callback);
                }
            };

            this.composeExpression = function () {
                var expression = serviceHelper.filterBuilder();

                if (this.basic !== this.constants.basic.allLibraries) {
                    expression = expression.lifecycleFilter();
                }

                if (this.query) {
                    expression = expression.and().searchFilter(this.query);
                }

                if (this.basic === this.constants.basic.ownItems)
                    expression = expression.and().append('Owner == (' + serverContext.getCurrentUserId() + ')');

                if (this.date && this.date !== this.constants.anyTimeValue) {
                    expression = expression.and().append('LastModified > (' + this.date.toGMTString() + ')');
                }

                if (this.taxon && this.taxon.id)
                    expression = expression.and().append(this.taxon.composeExpression());

                return expression.getFilter();
            };

            this.set = {
                basic: {
                    none: function () {
                        if (self.basic) {
                            self.basic = null;
                            changed();
                        }
                    },
                    allLibraries: function () {
                        if (self.basic !== self.constants.basic.allLibraries) {
                            reset();
                            self.basic = self.constants.basic.allLibraries;
                            changed();
                        }
                    },
                    ownItems: function () {
                        if (self.basic !== self.constants.basic.ownItems) {
                            reset();
                            self.basic = self.constants.basic.ownItems;
                            changed();
                        }
                    },
                    recentItems: function () {
                        if (self.basic !== self.constants.basic.recentItems) {
                            reset();
                            self.basic = self.constants.basic.recentItems;
                            changed();
                        }
                    }
                },
                date: {
                    all: function () {
                        if (self.date !== self.constants.anyTimeValue) {
                            reset();
                            self.date = self.constants.anyTimeValue;
                            changed();
                        }
                    },
                    to: function (date) {
                        if (self.date !== date) {
                            reset();
                            self.date = date;
                            changed();
                        }
                    }
                },
                parent: {
                    to: function (parentId) {
                        if (self.parent !== parentId) {
                            reset();
                            self.parent = parentId;
                            changed();
                        }
                    }
                },
                taxon: {
                    to: function (id, field) {
                        if (!self.taxon || self.taxon.id !== id || self.taxon.field !== field) {
                            reset();
                            self.taxon.id = id;
                            self.taxon.field = field;
                            changed();
                        }
                    }
                },
                query: {
                    to: function (query) {
                        if (query && self.basic === self.constants.basic.allLibraries) {
                            self.basic = null;
                        }

                        self.query = query;
                        changed();
                    }
                }

            }

            var reset = function () {
                self.query = null;
                self.basic = null;
                self.parent = null;
                self.date = null;
                self.taxon = new TaxonFilter();
            };

            var changed = function () {
                events.forEach(function (callback) {
                    callback();
                });
            };
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
                    hierarchyMode: options.recursive ? null : 'true'
                }).$promise.then(function (data) {
                    data.Items.map(function (obj) {
                        obj.IsFolder = true;
                    });
                    return data;
                });
        };

        var getFilter = function () { return new MediaFilter(); };

        var imagesObj = {
            getFolders: function (options) {
                return getFolders(options, constants.images.albumsServiceUrl);
            },
            getImages: function (options) {
                return getItems(options, 'true', constants.images.imagesServiceUrl, constants.images.itemType);
            },
            getContent: function (options) {
                return getItems(options, null, constants.images.imagesServiceUrl, constants.images.itemType);
            }
        };

        return {
            newFilter: getFilter,
            images: imagesObj
        };
    }]);
})();