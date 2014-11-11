Type.registerNamespace("Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI");

Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor = function(element){
    Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor.initializeBase(this, [element]);
};

Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor.prototype = {

    initialize: function () {
        Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor.callBaseMethod(this, "initialize");
    },

    _valueChangedHandler: function (sender, args) {
        var substring = "Mvc.Controllers";

        if (!(sender.target.value.indexOf(substring) > -1)) {
            jQuery(this._otherPropertiesContainer).parent().show();
            this._dataBindPropertiesItemsList(this._commonPropertiesItemsList);
        } else {
            // hide right other properties container for MVC widgets
            jQuery(this._otherPropertiesContainer).parent().hide();
        }

        this._setModuleTitle();
        this._bindFields();
    }
}

Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor.registerClass("Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI.MvcControlTemplateEditor", Telerik.Sitefinity.Modules.ControlTemplates.Web.UI.ControlTemplateEditor);