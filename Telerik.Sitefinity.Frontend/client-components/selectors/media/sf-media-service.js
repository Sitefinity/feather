(function () {
    angular.module('sfServices').factory('sfImageService', ['serviceHelper', 'serverContext', function (serviceHelper, serverContext) {
        var albumServiceUrl = serverContext.getRootedUrl('Sitefinity/Services/Content/AlbumService.svc/folders/'),
            imageServiceUrl = serverContext.getRootedUrl('Sitefinity/Services/Content/ImageService.svc/');

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

                if (this.date) {
                    var date = new Date();
                    date.setDate(date.getDate() - this.date);
                    expression = expression.and().append('LastModified > (' + date.toGMTString() + ')');
                }

                if (this.taxon && this.taxon.id)
                    expression = expression.and().append(this.taxon.composeExpression());

                return expression.getFilter();
            };
        };

        var callImageService = function (options, excludeFolders) {
            options = options || {};

            var url = options.parent ? imageServiceUrl + 'parent/' + options.parent + "/" : imageServiceUrl;
            return serviceHelper.getResource(url).get(
                {
                    itemType: 'Telerik.Sitefinity.Libraries.Model.Image',
                    filter: options.filter,
                    provider: options.provider,
                    skip: options.skip,
                    take: options.take,
                    sortExpression: options.sort,
                    includeSubFolderItems: options.recursive ? 'true' : null,
                    excludeFolders: excludeFolders
                }).$promise;
        };

        var getImages = function (options) {
            return callImageService(options, 'true');
        };

        var getFolders = function (options) {
            options = options || {};

            var url = options.parent ? albumServiceUrl + options.parent + "/" : albumServiceUrl;
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

        var getContent = function (options) {
            return callImageService(options, null);
        };

        return {
            getImages: getImages,
            getFolders: getFolders,
            getContent: getContent,

            newFilter: function () { return new MediaFilter(); }
        };
    }]);
})();