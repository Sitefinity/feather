Type.registerNamespace("Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI");

Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor = function(element){
    Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor.initializeBase(this, [element]);

    this._containerToHide = {};
    this._mvcTypes = null;
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
            jQuery(this._otherPropertiesContainer).parent().show();
            this._dataBindPropertiesItemsList(this._commonPropertiesItemsList);
        } else {
            // hide "other properties" container (right section) for MVC widgets
            jQuery(this._otherPropertiesContainer).parent().hide();
        }

        this._setModuleTitle();
        this._bindFields();
    },
    // called when data binding was successful
    _dataBindSuccess: function (sender, result) {
        Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor.callBaseMethod(this, "_dataBindSuccess", [sender, result]);

        if (sender._isMvcController(result.Item.ControlType)) {
            sender._containerToHide.hide();
        } else {
            sender._containerToHide.show();
        }
    },

    _isMvcController: function (item) {
        var len = item.indexOf("-");

        if (len > -1) {
            var mvcControllerType = item.substring(0, len);
            var mvcControllerTypesArray = JSON.parse(this.get_mvcTypes());

            for (var i = 0; i < mvcControllerTypesArray.length; i++) {
                if (mvcControllerTypesArray[i] == mvcControllerType) {
                    return true;
                }
            }
        }

        return false;
    },

    get_mvcTypes: function () {
        return this._mvcTypes;
    },

    set_mvcTypes: function (value) {
        this._mvcTypes = value;
    }
}

Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor.registerClass("Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor", Telerik.Sitefinity.Modules.ControlTemplates.Web.UI.ControlTemplateEditor);