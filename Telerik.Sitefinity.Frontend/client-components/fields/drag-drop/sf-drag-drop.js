; (function () {
    var sfSelectors = angular.module('sfSelectors');
    sfSelectors.requires.push('sfDragDrop');

    angular.module('sfDragDrop', ['sfServices'])
        .directive('sfDragDrop', ['serverContext', 'serviceHelper', function (serverContext, serviceHelper) {
            return {
                restrict: 'AE',
                scope: {
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

                    $(document).on('dragover', addDragStartClass);

                    $(document).on('dragleave', removeDragStartClass);

                    element.on('dragover', function (e) {
                        e.preventDefault();
                        e.stopPropagation();

                        element.addClass(constants.dragOverClass);
                    });

                    $('.sf-Drag').on('dragleave', function (e) {
                        e.preventDefault();
                        e.stopPropagation();

                        element.removeClass(constants.dragOverClass);
                    });

                    $('.sf-Drag').on('drop', function (e) {
                        element.removeClass(constants.dragStartClass);
                        element.removeClass(constants.dragOverClass);

                        if (e.originalEvent.dataTransfer) {
                            if (e.originalEvent.dataTransfer.files[0]) {
                                e.preventDefault();
                                e.stopPropagation();

                                scope.sfDataTransferCallback({ dataTransferObject: e.originalEvent.dataTransfer });
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
