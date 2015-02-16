(function () {
    angular.module('sfServices').factory('sfMediaFilter', ['serviceHelper', 'serverContext', 'sfMediaService', function (serviceHelper, serverContext, mediaService) {
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

            this.composeExpression = function (allLanguageSearch) {
                var expression = serviceHelper.filterBuilder();

                if (this.basic !== this.constants.basic.allLibraries) {
                    expression = expression.lifecycleFilter();
                }

                if (this.query) {
                    var languages;
                    if (allLanguageSearch)
                        languages = serverContext.getFrontendLanguages();

                    expression = expression.and().searchFilter(this.query, languages);
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
                    to: function (parentId, forceRebind) {
                        if (self.parent !== parentId || forceRebind) {
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
                        var currentParent = self.parent;
                        reset();
                        self.parent = currentParent;
                        self.query = query;
                        changed();
                    }
                }

            };

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

        var getFilter = function () { return new MediaFilter(); };

        return {
            newFilter: getFilter
        };
    }]);
})();