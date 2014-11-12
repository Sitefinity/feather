Type.registerNamespace("Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI");

Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor = function(element){
    Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor.initializeBase(this, [element]);
};

var _containerToHide = {};

function containsMvcControllersString(item) {
    return (item.indexOf("Mvc.Controllers") > -1)
}

Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor.prototype = {

    initialize: function () {
        Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor.callBaseMethod(this, "initialize");
        _containerToHide = jQuery(this._otherPropertiesContainer).parent();
    },

    _valueChangedHandler: function (sender, args) {

        if (!containsMvcControllersString(sender.target.value)) {
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

        if (containsMvcControllersString(result.Item.ControlType)) {
            _containerToHide.hide();
        }
        
    },

    dispose: function () {
        Telerik.Sitefinity.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor.callBaseMethod(this, "dispose");
    }
}

Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor.registerClass("Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor", Telerik.Sitefinity.Modules.ControlTemplates.Web.UI.ControlTemplateEditor);