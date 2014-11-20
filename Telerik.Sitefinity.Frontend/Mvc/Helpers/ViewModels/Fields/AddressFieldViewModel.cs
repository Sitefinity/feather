using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.GeoLocations.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers.ViewModels.Fields
{
    /// <summary>
    /// This class represents view model for address fields.
    /// </summary>
    public class AddressFieldViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateFieldViewModel" /> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="addressFormat">The address format.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldTitle">The field title.</param>
        public AddressFieldViewModel(Address address, string addressFormat, string fieldName, string fieldTitle)
        {
            this.Address = address;
            this.AddressFormat = addressFormat;
            this.FieldName = fieldName;
            this.FieldTitle = fieldTitle;
        }

        /// <summary>
        /// Gets or sets the address format.
        /// </summary>
        /// <value>
        /// The address format.
        /// </value>
        public string AddressFormat { get; set; }

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
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public Address Address { get; set; }
    }
}
