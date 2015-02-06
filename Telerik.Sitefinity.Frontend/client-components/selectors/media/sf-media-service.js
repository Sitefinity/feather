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

            this.composeExpression = function () {
                var expression = serviceHelper.filterBuilder();

                if (this.basic !== 'AllLibraries') {
                    expression = expression.lifecycleFilter();
                }

                if (this.query) {
                    expression = expression.and().searchFilter(this.query);
                }

                if (this.basic && this.basic === 'OwnItems')
                    expression = expression.and().append('Owner == (' + serverContext.getCurrentUserId() + ')');

                if (this.date && this.date !== 'AnyTime') {
                    expression = expression.and().append('LastModified > (' + this.date.toGMTString() + ')');
                }

                if (this.taxon && this.taxon.id)
                    expression = expression.and().append(this.taxon.composeExpression());

                return expression.getFilter();
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
                    hierarchyMode: options.recursive ? null : 'true',
                    excludeNeighbours: options.excludeNeighbours
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

        return {
            newFilter: getFilter,
            images: imagesObj
        };
    }]);
})();