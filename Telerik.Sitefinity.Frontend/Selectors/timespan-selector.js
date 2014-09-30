(function ($) {
    angular.module('selectors').requires.push('kendo.directives');
    angular.module('selectors')
        .directive('timespanSelector', ['$timeout', function ($timeout) {
            var timeSpanItem = function () {
                this.periodType = "anyTime";
                this.fromDate = null;
                this.toDate = null;
                this.timeSpanMeasure = null;
                this.timeSpanInterval = "days";
                this.formattedText = "";
            }

            return {
                restrict: "E",
                transclude: true,
                scope: {
                    selectedItem: '=?',                   
                    itemSelected: '='
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.templateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.templateUrl || 'Selectors/timespan-selector.html';
                    return sitefinity.getEmbeddedResourceUrl(assembly, url);
                },
                link: {
                    pre: function (scope, element, attrs, ctrl, transclude) {
                        // ------------------------------------------------------------------------
                        // helper methods
                        // ------------------------------------------------------------------------
                        formatTimeSpanItem = function (item) {
                            if (item.periodType == "periodToNow")
                                item.formattedText = "Last " + item.timeSpanMeasure + " " + item.timeSpanInterval;
                            else if (item.periodType == "customRange")
                                item.formattedText = item.fromDate.toLocaleString() + "-" + item.toDate.toLocaleString();
                            else
                                item.formattedText = "";
                        };

                        validate = function (item) {
                            if (item.periodType == "customRange" && item.fromDate && item.toDate) {
                                scope.errorMessage = "Invalid date range! The expiration date must be after the publication date."
                                scope.showError = true;

                                return item.fromDate < item.toDate;
                            }

                            return true;

                        };

                        // ------------------------------------------------------------------------
                        // Scope variables and setup
                        // ------------------------------------------------------------------------

                        var timeoutPromise = false;
                        var selectorId;
                        if (attrs.id) {
                            selectorId = attrs.id;
                        }
                        else {
                            //selectorId will be set to the id of the wrapper div of the template. This way we avoid issues when there are several selectors on one page.
                            selectorId = 'sf' + Math.floor((Math.random() * 1000) + 1);
                            scope.selectorId = selectorId;
                        }

                        // This id is used by the modal dialog to know which button will open him.
                        scope.openSelectorButtonId = '#' + selectorId + ' .openSelectorBtn';

                        scope.selectItem = function () {
                            if (validate(scope.selectedItemInTheDialog)) {
                                formatTimeSpanItem(scope.selectedItemInTheDialog);

                                var itemSelectedArgs = {
                                    "newSelectedItem": scope.selectedItemInTheDialog,
                                    "oldSelectedItem": jQuery.extend(true, {}, scope.selectedItem)
                                }
                                scope.itemSelected.call(scope.$parent, itemSelectedArgs);

                                scope.selectedItem = scope.selectedItemInTheDialog;

                                scope.$modalInstance.close();
                            }
                        };

                        scope.cancel = function () {
                            try {
                                scope.$modalInstance.close();
                            } catch (e) { }
                        };

                        scope.open = function () {
                            scope.showError = false;
                            scope.errorMessage = "";

                            if (!scope.selectedItem)
                                scope.selectedItemInTheDialog = new timeSpanItem();
                            else
                                scope.selectedItemInTheDialog = jQuery.extend(true, {}, scope.selectedItem);
                        };
                    }
                }
            };
        }]);
})(jQuery);