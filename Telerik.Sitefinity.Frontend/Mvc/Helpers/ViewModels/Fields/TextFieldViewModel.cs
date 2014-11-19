using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers.ViewModels.Fields
{
    public class TextFieldViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextFieldViewModel"/> class.
        /// </summary>
        public TextFieldViewModel(string fieldValue, string fieldName)
        {
            this.FieldValue = fieldValue;
            this.FieldName = fieldName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextFieldViewModel"/> class.
        /// </summary>
        /// <param name="fieldValue">The field value.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="format">The format.</param>
        public TextFieldViewModel(string fieldValue, string fieldName, string fieldTitle)
        {
            this.FieldValue = fieldValue;
            this.FieldName = fieldName;
            this.FieldTitle = fieldTitle;
        }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the field title.
        /// </summary>
        /// <value>
        /// The field title.
        /// </value>
        public string FieldTitle { get; set; }

        /// <summary>
        /// Gets or sets the field value.
        /// </summary>
        /// <value>
        /// The field value.
        /// </value>
        public string FieldValue { get; set; }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>
        /// The format.
        /// </value>
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the unit.
        /// </summary>
        /// <value>
        /// The unit.
        /// </value>
        public string Unit { get; set; }
    }
}
