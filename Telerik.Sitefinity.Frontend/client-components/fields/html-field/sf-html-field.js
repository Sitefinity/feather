﻿(function ($) {
	var module = angular.module('sfFields', ['kendo.directives', 'sfServices']);

	module.directive('sfHtmlField', ['serverContext', function (serverContext) {
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
					widget.focus();
					editor = widget;
					content = editor.wrapper.find('iframe.k-content').first();
				});

				scope.toggleHtmlView = function () {
					if (editor == null)
						return;

					if (isInHtmlView == false) {
						scope.htmlViewLabel = 'Design';

						$('<textarea class="html k-content" style="resize: none">')
							.val(editor.value())
							.insertAfter(content);

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

						editor.value(html.val());

						editor.trigger('change');

						html.remove();

						content.show();
					}

					isInHtmlView = !isInHtmlView;
				};
			}
		};
	}]);
})(jQuery);