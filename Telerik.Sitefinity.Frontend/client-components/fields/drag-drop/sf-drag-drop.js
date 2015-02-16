; (function () {
    var sfSelectors = angular.module('sfSelectors');
    sfSelectors.requires.push('sfDragDrop');

    angular.module('sfDragDrop', [])
        .directive('sfDragDrop', [function () {
            return {
                restrict: 'AE',
                scope: {
                    sfTemplateHtml: '@',
                    sfDataTransferCallback: '&'
                },
                link: function (scope, element, attrs, ctrl) {
                    var constants = {
                        dragStartClass: 'sf-Drag-start',
                        dragOverClass: 'sf-Drag-over'
                    };

                    var addDragStartClass = function () {
                        element.addClass(constants.dragStartClass);
                    };

                    var removeDragStartClass = function () {
                        element.removeClass(constants.dragStartClass);
                    };

                    if (scope.sfTemplateHtml) {
                        element.prepend($(scope.sfTemplateHtml));
                    }
                    else {
                        element.prepend($('<div class="sf-Drag"><strong>Drop image here to upload</strong></div>'));
                    }

                    $(document).on('dragover', addDragStartClass);

                    $(document).on('dragleave', removeDragStartClass);

                    element.on('dragover', function (e) {
                        e.preventDefault();
                        e.stopPropagation();

                        element.addClass(constants.dragOverClass);
                    });

                    element.find('.sf-Drag').on('dragleave', function (e) {
                        e.preventDefault();
                        e.stopPropagation();

                        element.removeClass(constants.dragOverClass);
                    });

                    element.find('.sf-Drag').on('drop', function (e) {
                        element.removeClass(constants.dragStartClass);
                        element.removeClass(constants.dragOverClass);

                        if (e.originalEvent.dataTransfer) {
                            if (e.originalEvent.dataTransfer.files[0]) {
                                e.preventDefault();
                                e.stopPropagation();
                                scope.$apply(function () {
                                    scope.sfDataTransferCallback({ dataTransferObject: e.originalEvent.dataTransfer });
                                });
                            }
                        }
                    });

                    scope.$on("$destroy", function () {
                        $(document).off('dragover', addDragStartClass);
                        $(document).off('dragleave', removeDragStartClass);
                    });
                }
            };
        }]);
})();
