(function ($) {
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
				var isFullScreen = false;
				var editor = null;
				var content = null;
				var editorWrapperInitialStyle = null;

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

						var html = editor.wrapper.find('.html');

						editor.value(html.val());

						editor.trigger('change');

						html.remove();

						content.show();

						editor.wrapper.find('.k-tool:visible').removeClass('k-state-disabled');
						editor.wrapper.find('[data-role=combobox]').kendoComboBox('enable', true);
						editor.wrapper.find('[data-role=dropdownlist]').kendoDropDownList('enable', true);
						editor.wrapper.find('[data-role=selectbox]').kendoSelectBox('enable', true);
						editor.wrapper.find('[data-role=colorpicker]').kendoColorPicker('enable', true);
					}

					isInHtmlView = !isInHtmlView;
				};

				scope.toggleFullScreen = function () {
				    if (editor == null)
				        return;

				    var dialog = $(".modal-dialog");
				    var modalBody = $(".modal-body");

				    if (!editorWrapperInitialStyle) {
				        editorWrapperInitialStyle = {
				            dialog: {
				                margin: dialog.css('margin'),
                                width : dialog.width()
				            },
				            modalBody: {
				                height: modalBody.height()
				            }
				        };
				    }

				    if (isFullScreen === false) {
				        dialog.css({
				            margin: 0,
				            width: $("body").width()
				        });
				        modalBody.height($(document).height());

				        isFullScreen = true;
				    } else {
				        dialog.css({
				            margin: editorWrapperInitialStyle.dialog.margin,
				            width: editorWrapperInitialStyle.dialog.width
				        });
				        modalBody.height(editorWrapperInitialStyle.modalBody.height);

				        isFullScreen = false;
				    }
				};
			}
		};
	}]);
})(jQuery);