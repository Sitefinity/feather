(function ($) {
	var module = angular.module('sfFields', ['kendo.directives', 'sfServices']);

    module.directive('sfHtmlField', ['serverContext', '$compile', function (serverContext, $compile) {
		return {
			restrict: "E",
			scope: {
				ngModel: '='
			},
			templateUrl: function (elem, attrs) {
				var assembly = attrs.sfTemplateAssembly || 'Telerik.Sitefinity.Frontend';
				var url = attrs.sfTemplateUrl || 'client-components/fields/html-field/sf-html-field.html';
				return serverContext.getEmbeddedResourceUrl(assembly, url);
			},
			link: function (scope, element) {
				scope.htmlViewLabel = 'HTML';

				var isInHtmlView = false;
				var editor = null;
				var content = null;

				scope.$on('kendoWidgetCreated', function (event, widget) {
                    if (widget.focus)
                        widget.focus();

					editor = widget;
					content = editor.wrapper.find('iframe.k-content').first();
				});

				scope.openLinkSelector = function () {
				    var selection = editor.getSelection();
				    var parent = selection.extentNode.parentElement;
				    if (parent.tagName.toLowerCase() === "a") {
				        scope.selectedHtml = parent;
				    }
				    else {
				        scope.selectedHtml = editor.selectedHtml();
				    }

				    angular.element("#linkSelectorModal").scope().$openModalDialog();
				};

				scope.$on('selectedHtmlChanged', function (event, data) {
				    scope.selectedHtml = data;
				    editor.exec("insertHtml", { html: data.outerHTML, split: false });
				});

				scope.toggleHtmlView = function () {
					if (editor == null)
						return;

					if (isInHtmlView == false) {
						scope.htmlViewLabel = 'Design';

                        var htmlEditor = $('<textarea class="html k-content" ng-model="ngModel" style="resize: none">');
                        $compile(htmlEditor)(scope)
                        htmlEditor.insertAfter(content);
						content.hide();

						editor.wrapper.find('.k-tool:visible').removeClass('k-state-selected').addClass('k-state-disabled').css('display', 'inline-block');
						editor.wrapper.find('[data-role=combobox]').kendoComboBox('enable', false);
						editor.wrapper.find('[data-role=dropdownlist]').kendoDropDownList('enable', false);
						editor.wrapper.find('[data-role=selectbox]').kendoSelectBox('enable', false);
						editor.wrapper.find('[data-role=colorpicker]').kendoColorPicker('enable', false);
					} else {
						scope.htmlViewLabel = 'HTML';

						editor.wrapper.find('.k-tool:visible').removeClass('k-state-disabled');
						editor.wrapper.find('[data-role=combobox]').kendoComboBox('enable', true);
						editor.wrapper.find('[data-role=dropdownlist]').kendoDropDownList('enable', true);
						editor.wrapper.find('[data-role=selectbox]').kendoSelectBox('enable', true);
						editor.wrapper.find('[data-role=colorpicker]').kendoColorPicker('enable', true);

                        var html = editor.wrapper.find('.html');

                        html.remove();
                        content.show();
					}

					isInHtmlView = !isInHtmlView;
				};
			}
		};
	}]);
})(jQuery);