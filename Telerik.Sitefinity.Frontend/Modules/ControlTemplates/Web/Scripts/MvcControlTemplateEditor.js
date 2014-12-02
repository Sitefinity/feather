Type.registerNamespace("Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI");

Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor = function(element){
    Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor.initializeBase(this, [element]);

    this._containerToHide = {};
    this._mvcTypes = null;
    this._elementIdsToBeHiddenIfMvc = null;
};

Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor.prototype = {

    initialize: function () {
        Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor.callBaseMethod(this, "initialize");

        this._containerToHide = jQuery(this._otherPropertiesContainer).parent();
    },

    dispose: function () {
        Telerik.Sitefinity.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor.callBaseMethod(this, "dispose");
    },

    _valueChangedHandler: function (sender, args) {

        if (!this._isMvcController(sender.target.value)) {
            this._dataBindPropertiesItemsList(this._commonPropertiesItemsList);
            this._toggleVisibilityOfElements(true);
        } else {
            this._toggleVisibilityOfElements(false);
        }

        this._setModuleTitle();
        this._bindFields();
    },
    // called when data binding was successful
    _dataBindSuccess: function (sender, result) {
        Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor.callBaseMethod(this, "_dataBindSuccess", [sender, result]);

        if (sender._isMvcController(result.Item.ControlType)) {
            sender._toggleVisibilityOfElements(false);
        } else {
            sender._toggleVisibilityOfElements(true);
        }
    },

    _isMvcController: function (item) {
        var len = item.indexOf("-");

        if (len > -1) {
            var mvcControllerType = item.substring(0, len);
            var mvcControllerTypesArray = JSON.parse(this.get_mvcTypes());

            for (var i = 0; i < mvcControllerTypesArray.length; i++)
                if (mvcControllerTypesArray[i] == mvcControllerType)
                    return true;
        }

        return false;
    },

    _toggleVisibilityOfElements: function(visible){
        var elements = JSON.parse(this.get_elementIdsToBeHiddenIfMvc());

        for (var i = 0; i < elements.length; i++) {
            var uiElement = jQuery('#' + elements[i]);
            uiElement.toggle(visible);
        }

        this._containerToHide.toggle(visible);
    },

    get_mvcTypes: function () {
        return this._mvcTypes;
    },

    set_mvcTypes: function (value) {
        this._mvcTypes = value;
    },

    get_elementIdsToBeHiddenIfMvc: function () {
        return this._elementIdsToBeHiddenIfMvc;
    },

    set_elementIdsToBeHiddenIfMvc: function (value) {
        this._elementIdsToBeHiddenIfMvc = value;
    }
}

Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor.registerClass("Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor", Telerik.Sitefinity.Modules.ControlTemplates.Web.UI.ControlTemplateEditor);