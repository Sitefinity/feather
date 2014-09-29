(function ($) {
    angular.module('selectors')
        .directive('filterSelector', function () {
            return {
                restrict: 'EA',
                scope: {
                    taxonomyFields: '=',
                    additionalFilters: '=',
                    provider: '=?'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.templateUrl || 'Selectors/filter-selector.html';
                    return sitefinity.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    pre: function (scope, element, attrs, ctrl) {

                        taxonFilterCondition = function (taxonomyName) {
                            this.FieldName = taxonomyName;
                            this.FieldType = 'System.Guid';
                            this.Operator = 'Contains';
                        };

                        queryDataItem = function () {
                            // define variables
                            this.Name = null;
                            this.IsGroup = null;
                            this.Ordinal = null;
                            this.Join = null;
                            this.ItemPath = null;
                            this.Value = null;
                            this.Condition = null;
                            this._itemPathSeparator = '_';
                        };
                        scope.additionalFilters.prototype.addGroupQueryDateItem = function (name, join, parentGroup) {
                            var queryItem = new queryDataItem();
                            queryItem.Name = name;
                            queryItem.IsGroup = true;
                            queryItem.Ordinal = 0;
                            queryItem.Join = join;
                            var parentPath = parentGroup ? parentGroup.ItemPath : '';
                            queryItem.ItemPath = parentPath + queryItem._itemPathSeparator + queryItem.Ordinal;

                            this.QueryItems.push(queryItem);
                            return queryItem;
                        };

                        scope.additionalFilters.prototype.addChildQueryDateItem = function (name, join, parentGroup, ordinal, value, condition) {
                            var queryItem = new queryDataItem();
                            queryItem.Name = name;
                            queryItem.IsGroup = false;
                            queryItem.Ordinal = ordinal;
                            queryItem.Join = join;
                            queryItem.Condition = condition;
                            queryItem.Value = value;
                            var parentPath = parentGroup ? parentGroup.ItemPath : '';
                            queryItem.ItemPath = parentPath + queryItem._itemPathSeparator + queryItem.Ordinal;

                            this.QueryItems.push(queryItem);
                            return queryItem;
                        };
                    }
                }
            };
        });
})(jQuery);