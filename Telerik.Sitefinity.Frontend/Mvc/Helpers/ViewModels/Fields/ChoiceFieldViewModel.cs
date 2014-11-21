using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers.ViewModels.Fields
{
    /// <summary>
    /// This class represents view model for choice fields.
    /// </summary>
    public class ChoiceFieldViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChoiceFieldViewModel"/> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldTitle">The field title.</param>
        public ChoiceFieldViewModel(string fieldName, string fieldTitle)
        {
            this.FieldTitle = fieldTitle;
            this.FieldName = fieldName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChoiceFieldViewModel"/> class.
        /// </summary>
        /// <param name="fieldValue">if set to <c>true</c> the value is yes.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldTitle">The field title.</param>
        public ChoiceFieldViewModel(bool yesNoValue, string fieldName, string fieldTitle)
        {
            this.YesNoValue = yesNoValue;
            this.FieldName = fieldName;
            this.FieldTitle = fieldTitle;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChoiceFieldViewModel"/> class.
        /// </summary>
        /// <param name="singleChoiceValue">The single choice value.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldTitle">The field title.</param>
        public ChoiceFieldViewModel(string singleChoiceValue, string fieldName, string fieldTitle)
        {
            this.ChoiceValueText = singleChoiceValue;
            this.FieldName = fieldName;
            this.FieldTitle = fieldTitle;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChoiceFieldViewModel"/> class.
        /// </summary>
        /// <param name="multipleChoiceValues">The multiple choice values.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldTitle">The field title.</param>
        public ChoiceFieldViewModel(IEnumerable multipleChoiceValues, string fieldName, string fieldTitle)
        {
            this.MultiChoiceValues = multipleChoiceValues;
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
        /// Gets or sets a value indicating whether the value is true or false.
        /// </summary>
        /// <value>
        ///   <c>true</c> if yes ; otherwise, <c>no</c>.
        /// </value>
        public bool YesNoValue { get; set; }

        /// <summary>
        /// Gets or sets the choice value as text.
        /// </summary>
        /// <value>
        /// The choice value as text.
        /// </value>
        public string ChoiceValueText { get; set; }

        /// <summary>
        /// Gets or sets the multi choice values.
        /// </summary>
        /// <value>
        /// The multi choice values.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi")]
        public IEnumerable MultiChoiceValues { get; set; }
    }
}
