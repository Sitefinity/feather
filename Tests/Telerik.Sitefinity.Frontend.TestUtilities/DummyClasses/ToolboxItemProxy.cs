using System.Collections.Generic;
using System.Collections.Specialized;
using Telerik.Sitefinity.DesignerToolbox;
using Telerik.Sitefinity.Modules.Pages.Configuration;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses
{
    /// <summary>
    /// Implementation of IToolboxItem for testing purposes.
    /// </summary>
    public class ToolboxItemProxy : IToolboxItem
    {
        /// <summary>
        /// Gets or sets the CLR type of the custom control or user control which is represented by the
        /// toolbox item.
        /// </summary>
        /// <remarks>
        /// If your tool represents an MVC based widget, leave this property empty and specify 
        /// ControllerType property instead.
        /// </remarks>
        /// <value>The CLR type of the custom control or user control.</value>
        public string ControlType { get; set; }

        /// <summary>
        /// Gets or sets the CLR type of the MVC controller which is to be represented by the toolbox item.
        /// </summary>
        /// <remarks>
        /// If your tool representes WebForms based widget, leave this property empty and specify
        /// ControlType property instead.
        /// </remarks>
        /// <value>The CLR type of the MVC controller.</value>
        public string ControllerType { get; set; }

        /// <summary>
        /// Gets or sets the CSS class that will be applied to the element representing the toolbox item.
        /// You can use this property to style the toolbox item. For example, you could specify the icon
        /// of the toolbox item like that.
        /// </summary>
        /// <value>The css class of the toolbox item.</value>
        public string CssClass { get; set; }

        /// <summary>
        /// Gets or sets the description of the tool; let's you specify the purpose and usage of the tool.
        /// </summary>
        /// <value>The description of the toolbox item.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value which determines weather the tool will be rendered in the toolbox.
        /// If true, the tool will be rendered; otherwise false.
        /// </summary>
        /// <value>
        /// The value which determines weather the tool should be rendered in the toolbox.
        /// </value>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the name of the embedded resource or path to external template (.ascx) which is
        /// to be represented by the toolbox item (use only for layout widgets).
        /// </summary>
        /// <remarks>
        /// Set this property only if your toolbox item represents a layout element.
        /// </remarks>
        /// <value>The layout template of the toolbox item.</value>
        public string LayoutTemplate { get; set; }

        /// <summary>
        /// Gets or sets the name of the module to which the toolbox item is associated.
        /// </summary>
        /// <remarks>
        /// This property allows Sitefinity to automatically remove widgets from toolbox, when
        /// for example certain module is uninstalled; the property is used to map toolbox items
        /// and modules. If no such connection exists, property can be left empty.
        /// </remarks>
        /// <value>The name of the module to which the toolbox item is associated.</value>
        public string ModuleName { get; set; }

        /// <summary>
        /// Gets or sets the programmatic name of the tool. This is how tool is identified
        /// inside of a toolbox section.
        /// </summary>
        /// <value>The name of the toolbox item.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the collection of default values with which the widget represented by the toolbox
        /// item will be initialized, the first time it is used.
        /// </summary>
        /// <remarks>
        /// This property can be used to initialize properties of the widget being represented
        /// by the toolbox item, when the toolbox item is first time materialized into an actual
        /// widget.
        /// </remarks>
        /// <value>
        /// The default parameters with which the widget will be initialized.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public NameValueCollection Parameters { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the global resource class used to localize the strings 
        /// of the toolbox item.
        /// </summary>
        /// <value>
        /// The id of the resource class used for localization.
        /// </value>
        public string ResourceClassId { get; set; }

        /// <summary>
        /// A set of tags that describe the current section.
        /// One example of their use would be the toolbox section filtering.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ISet<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the title of the tool. This value will be used in the user interface
        /// when representing the tool and it can be localized.
        /// </summary>
        /// <value>The title of the toolbox item.</value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the value which indicates the visibility to the toolbox item. The available
        /// values are none, only in pages and only in templates.
        /// </summary>
        /// <value>The visiblity mode of the toolbox item.</value>
        public ToolboxItemVisibilityMode VisibilityMode { get; set; }
    }
}
