using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Frontend.InlineEditing.Attributes;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Models
{
    /// <summary>
    /// This class is used for a fake widget property model.
    /// </summary>
    /// <remarks>
    /// One of its properties is marked with FieldInfoAttribute and the other is not.
    /// </remarks>
    public class DummyWidgetModel
    {
        /// <summary>
        /// Gets or sets the HTML which must be wrapped into InlineEditing region.
        /// </summary>
        [FieldInfo("DummyWidget", "LongText")]
        public string EditableContent { get; set; }

        /// <summary>
        /// Gets or sets the non editable content.
        /// </summary>
        public string NonEditableContent { get; set; }
    }
}
