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
                    scope.sortExpression = null;
                    scope.items = [];
                    scope.filterObject = sfImageService.newFilter();

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

                    scope.$on('sf-collection-item-selected', function (event, data) {
                        if (data && data.IsFolder === true) {
                            scope.filterObject.basic = null;
                            scope.filterObject.parent = data.Id;
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
                            if (scope.filterObject.basic === 'RecentItems') {
                                callback = sfImageService.getImages;
                            }
                            else if (scope.filterObject.basic === 'OwnItems') {
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
                                }
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