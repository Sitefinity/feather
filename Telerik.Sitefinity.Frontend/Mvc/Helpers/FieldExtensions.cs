using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Telerik.Sitefinity.GeoLocations.Model;
using Telerik.Sitefinity.Locations.Configuration;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// This class contains helper methods for working with front-end fields.
    /// </summary>
    public static class FieldExtensions
    {
        /// <summary>
        /// Gets the formatted address.
        /// </summary>
        /// <param name="fieldValue">The field value.</param>
        /// <param name="addressFormat">The address format.</param>
        /// <returns></returns>
        public static string GetFormattedAddress(this Address fieldValue, string addressFormat)
        {
            string result = string.Empty;
            if (!addressFormat.IsNullOrEmpty())
            {
                var street = string.Empty;
                if (!string.IsNullOrEmpty(fieldValue.Street))
                {
                    street = fieldValue.Street + ",";
                }

                result = addressFormat.Replace("#=Street#", street);

                var zip = string.Empty;
                if (!string.IsNullOrEmpty(fieldValue.Zip))
                {
                    zip = fieldValue.Zip + ",";
                }

                result = result.Replace("#=Zip#", zip);

                var city = string.Empty;
                if (!string.IsNullOrEmpty(fieldValue.City))
                {
                    city = fieldValue.City + ",";
                }

                result = result.Replace("#=City#", city);

                var country = Telerik.Sitefinity.Configuration.Config.Get<LocationsConfig>().Countries[fieldValue.CountryCode];
                string countryName = string.Empty;
                var state = string.Empty;
                if (country != null)
                {
                    countryName = country.Name;
                    if (fieldValue.StateCode.IsNullOrEmpty() &&
                        (fieldValue.CountryCode == "CA" || fieldValue.CountryCode == "US"))
                    {
                        var stateData = Telerik.Sitefinity.Configuration.Config.Get<LocationsConfig>().Countries[fieldValue.CountryCode]
                            .StatesProvinces[fieldValue.StateCode];
                        if (stateData != null)
                        {
                            state = stateData.Name;
                        }
                    }
                }

                result = result.Replace("#=Country#", countryName);
                result = result.Replace("#=State#", state);
            }

            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Api")]
        public static string GetApiKey(this HtmlHelper helper)
        {
            var apiKey = Telerik.Sitefinity.Configuration.Config.Get<Telerik.Sitefinity.Services.SystemConfig>().GeoLocationSettings.GoogleMapApiV3Key;

            return apiKey;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Api")]
        public static bool IsApiKeyValid(this HtmlHelper helper)
        {
            var apiKey = Telerik.Sitefinity.Configuration.Config.Get<Telerik.Sitefinity.Services.SystemConfig>().GeoLocationSettings.GoogleMapApiV3Key;
            var isValid = !apiKey.IsNullOrEmpty();

            return isValid;
        }
    }
}
