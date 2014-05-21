/* global $telerik, document, kendo */

var sf = sf || {};

(function (sf) {

	var $ = jQuery,
		loader,
		loaderMarkup = '<div style="opacity:0.5; width:100%; z-index:3100; position:absolute;' +
					   'top:0; height:100%; background: black center no-repeat ' +
					   'url(#= appPath #Frontend-Assembly/Telerik.Sitefinity.Frontend/Mvc/Styles/Images/loading.gif);">' +
					   '</div>',
		loaderTemplate = kendo.template(loaderMarkup),
		dialog;

	/**
	 * Represents the Sitefinity page editor.
	 */
	sf.pageEditor = {

		/**
		 * Shows the loading animation in the page editor.
		 *
		 * @param {String} appPath The relative path of the application used to resolve the loading image.
		 */
		showLoader: function (appPath) {
			if (loader) {
				loader.show();
			} else {
				loader = $(loaderTemplate({ appPath: appPath }));
				$('body').append(loader);
			}
		},

		/**
		 *  Hides the loading animation in the page editor.
		 */
		hideLoader: function () {
			$(loader).hide();
		},

		/**
		 * Renders the dialog within page editor with the provided markup.
		 *
		 * @param {String} markup The HTML markup to be rendered within the dialog.
		 */
		renderDialog: function (markup) {
			dialog = $('<div />');
			$('body').append(dialog);
			dialog.append(markup);
			this.hideLoader();

			$(dialog).on('hidden.bs.modal', this.destroyDialog);
		},

		/**
		 * Event handler that handles the needDialog event from the Sitefinity
		 * page editor.
		 */
		openDialog: function (ev, args) {
			this.showLoader(args.AppPath);
			this.widgetContext = args;

			$.get(this.widgetContext.url)
				.done($.proxy(this.renderDialog, this))
				.fail(function (data) {
					alert('There is a problem with loading the widget designer: ' + data);
				});
		},

		/**
		 * Destroys the currently opened dialog.
		 */
		destroyDialog: function () {
			if (dialog) {
				dialog.remove();
				this.widgetContext = null;
			}
		},

		/**
		 * Provides the context for the currently active widget.
		 */
		widgetContext: null

	};

	/**
	 * Register the global Sitefinity events with the pageEditor component.
	 */
	if ($telerik) {
		$telerik.$(document).on('needsModalDialog', $.proxy(sf.pageEditor.openDialog, sf.pageEditor));
		$telerik.$(document).on('modalDialogClosed', $.proxy(sf.pageEditor.destroyDialog, sf.pageEditor));
	}
})(sf);