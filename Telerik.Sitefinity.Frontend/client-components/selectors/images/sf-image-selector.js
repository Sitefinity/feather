; (function () {
    var sfSelectors = angular.module('sfSelectors');
    sfSelectors.requires.push('sfInfiniteScroll');

    sfSelectors
        .directive('sfImageSelector', ['serverContext', 'sfImageService', 'serviceHelper', function (serverContext, sfImageService, serviceHelper) {
            var constants = {
                initialLoadedItemsCount: 50,
                infiniteScrollLoadedItemsCount: 20,
                recentImagesLastDaysCount: 7
            };

            var TaxonFilterObject = function () {
                this.id = null;
                this.field = null;

                this.composeExpression = function () {
                    return this.field + '.Contains({' + this.id + '})';
                };
            };

            var FilterObject = function () {
                // Query that is typed by a user in a text box.
                this.query = null;

                // RecentImages, OwnImages or AllLibraries
                this.basic = null;

                // Parent id
                this.parent = null;

                // Number of days since modified
                this.date = null;

                // Filter by any taxon
                this.taxon = new TaxonFilterObject();

                this.composeExpression = function () {
                    var expression = serviceHelper.filterBuilder();

                    if (this.basic !== 'AllLibraries') {
                        expression = expression.lifecycleFilter();
                    }
                    
                    if (this.query) {
                        expression = expression.and().searchFilter(this.query);
                    }

                    if (this.basic && this.basic === 'OwnImages')
                        expression = expression.and().custom('Owner == (' + serverContext.getCurrentUserId() + ')');

                    if (this.date) {
                        var date = new Date();
                        date.setDate(date.getDate() - this.date);
                        expression = expression.and().custom('LastModified > (' + date.toGMTString() + ')');
                    }

                    if (this.taxon && this.taxon.id)
                        expression = expression.and().custom(this.taxon.composeExpression());

                    return expression.getFilter();
                };
            };

            return {
                restrict: 'E',
                scope: {
                    selectedItem: '=ngModel'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/selectors/images/sf-image-selector.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    scope.filterObject = new FilterObject();
                    scope.sortExpression = null;
                    scope.items = [];

                    scope.loadMore = function () {
                        refresh(true);
                    };

                    scope.$watch('filterObject', function (newVal, oldVal) {
                        if (JSON.stringify(newVal) !== JSON.stringify(oldVal)) {
                            refresh();
                        }
                    }, true);

                    scope.$watch('sortExpression', function (newVal, oldVal) {
                        if (newVal !== oldVal) {
                            refresh();
                        }
                    });

                    var refresh = function (appendItems) {
                        var options = {
                            filter: scope.filterObject.composeExpression()
                        };

                        options.parent = scope.filterObject.parent;
                        options.sort = scope.sortExpression;

                        if (appendItems) {
                            options.skip = scope.items.length;
                            options.take = constants.infiniteScrollLoadedItemsCount;
                        }
                        else {
                            options.take = constants.initialLoadedItemsCount;
                        }

                        var callback;
                        if (scope.filterObject.basic) {
                            // Defaul filter is used (Recent / My / All)
                            if (scope.filterObject.basic === 'RecentImages') {
                                callback = sfImageService.getImages;
                            }
                            else if (scope.filterObject.basic === 'OwnImages') {
                                callback = sfImageService.getImages;
                            }
                            else if (scope.filterObject.basic === 'AllLibraries') {
                                callback = sfImageService.getFolders;
                            }
                            else {
                                throw { message: 'Unknown basic filter object option.' };
                            }
                        }
                        else {
                            // custom filter is used (Libraries / Taxons / Dates)
                            callback = sfImageService.getContent;
                        }

                        callback(options).then(function (response) {
                            if (response && response.Items) {
                                if (appendItems) {
                                    scope.items = scope.items.concat(response.Items);
                                }
                                else {
                                    scope.items = response.Items;

                                    // TODO: Remove
                                    if (scope.filterObject.basic && scope.filterObject.basic === 'AllLibraries') {
                                        scope.items.push({ Id: 1 }, { Title: 'Default Library', IsFolder: true });
                                        scope.items.push({ Id: 2 }, { Title: 'Second Library', IsFolder: true });
                                        scope.items.push({ Id: 3 }, { Title: 'Third Library', IsFolder: true });
                                    }
                                }

                                // TODO: Remove
                                console.log(response);
                            }
                        }, function (error) {

                        });
                    };

                    // initial open populates dialog with all root libraries
                    scope.filterObject.basic = 'AllLibraries';
                    refresh();
                }
            };
        }]);
})();