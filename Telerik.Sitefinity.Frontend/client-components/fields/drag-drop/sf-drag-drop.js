; (function () {
    var sfSelectors = angular.module('sfSelectors');
    sfSelectors.requires.push('sfDragDrop');

    angular.module('sfDragDrop', ['sfServices'])
        .directive('sfDragDrop', ['serverContext', 'serviceHelper', function (serverContext, serviceHelper) {
            return {
                restrict: 'AE',
                scope: {
                    sfFileDroppedCallback: '&'
                },
                templateUrl: function (elem, attrs) {
                    var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
                    var url = attrs.sfTemplateUrl || 'client-components/fields/drag-drop/sf-drag-drop.html';
                    return serverContext.getEmbeddedResourceUrl(assembly, url);
                },
                link: function (scope, element, attrs, ctrl) {
                    var constatns = {
                        dragStartClass: 'sf-css-drag-start',
                        dragOverClass: 'sf-css-drag-over'
                    };

                    $(document).on('dragover', function () {
                        element.addClass(constatns.dragStartClass);
                    });

                    $(document).on('dragleave', function () {
                        element.removeClass(constatns.dragStartClass);
                    });

                    element.on('dragover', function (e) {
                        e.preventDefault();
                        e.stopPropagation();

                        element.addClass(constatns.dragOverClass);
                    });

                    element.on('dragleave', function (e) {
                        e.preventDefault();
                        e.stopPropagation();

                        element.removeClass(constatns.dragOverClass);
                    });

                    element.on('drop', function (e) {
                        element.removeClass(constatns.dragStartClass);
                        element.removeClass(constatns.dragOverClass);

                        if (e.originalEvent.dataTransfer) {
                            if (e.originalEvent.dataTransfer.files[0]) {
                                e.preventDefault();
                                e.stopPropagation();

                                scope.sfFileDroppedCallback({ file: e.originalEvent.dataTransfer.files[0] });
                            }
                        }
                    });
                }
            };
        }]);
})();