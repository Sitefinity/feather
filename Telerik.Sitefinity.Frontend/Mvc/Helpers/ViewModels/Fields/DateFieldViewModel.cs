using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers.ViewModels.Fields
{
    /// <summary>
    /// This class represents view model for date fields.
    /// </summary>
    public class DateFieldViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateFieldViewModel"/> class.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="dateFormat">The date format.</param>
        public DateFieldViewModel(DateTime date, string dateFormat, string fieldName, string fieldTitle)
        {
            this.Date = date;
            this.DateFormat = dateFormat;
            this.FieldName = fieldName;
            this.FieldTitle = fieldTitle;
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the date format.
        /// </summary>
        /// <value>
        /// The date format.
        /// </value>
        public string DateFormat { get; set; }

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
    }
}
