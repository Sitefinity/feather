using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Telerik.Sitefinity.GeoLocations.Model;
using Telerik.Sitefinity.Locations.Configuration;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    #region Address field

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

        /// <summary>
        /// Gets the API key.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Api")]
        public static string GetApiKey(this HtmlHelper helper)
        {
            var apiKey = Telerik.Sitefinity.Configuration.Config.Get<Telerik.Sitefinity.Services.SystemConfig>().GeoLocationSettings.GoogleMapApiV3Key;

            return apiKey;
        }

        /// <summary>
        /// Determines whether the API key valid.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Api")]
        public static bool IsApiKeyValid(this HtmlHelper helper)
        {
            var apiKey = Telerik.Sitefinity.Configuration.Config.Get<Telerik.Sitefinity.Services.SystemConfig>().GeoLocationSettings.GoogleMapApiV3Key;
            var isValid = !apiKey.IsNullOrEmpty();

            return isValid;
        }

        #endregion

        #region Choice fields

        /// <summary>
        /// Gets the multiple choice value string.
        /// </summary>
        /// <param name="multiChoiceValues">The multi choice values.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "multi")]
        public static string GetMultipleChoiceValueString(this IEnumerable multiChoiceValues)
        {
            StringBuilder sb = new StringBuilder();

            foreach (object val in multiChoiceValues)
            {
                sb.Append(val.ToString());
                sb.Append(", ");
            }

            if (sb.Length > 1)
                sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }

        #endregion

        #region Taxon fields

        /// <summary>
        /// Gets the taxon names.
        /// </summary>
        /// <returns></returns>
        public static IList<string> GetTaxonNames(this IList<Guid> taxonIds, Guid classificationId)
        {
            TaxonomyManager manager = TaxonomyManager.GetManager();
            IList<string> taxonNames = null;
            var taxonomyType = FieldExtensions.GetTaxonomyType(classificationId);
            if (taxonomyType == TaxonomyType.Flat)
            {
                taxonNames = manager.GetTaxa<FlatTaxon>()
                    .Where(t => taxonIds.Contains(t.Id) && t.Taxonomy.Id == classificationId)
                    .Select(t => t.Title.ToString()).ToList();
            }
            else if (taxonomyType == TaxonomyType.Hierarchical)
            {
                taxonNames = manager.GetTaxa<HierarchicalTaxon>()
                   .Where(t => taxonIds.Contains(t.Id) && t.Taxonomy.Id == classificationId)
                   .Select(t => t.Title.ToString()).ToList();
            }

            return taxonNames;
        }

        /// <summary>
        /// Gets the type of the taxonomy.
        /// </summary>
        /// <param name="classificationId">The classification identifier.</param>
        /// <returns></returns>
        /// <value>
        /// The type of the taxonomy.
        ///   </value>
        /// <exception cref="System.InvalidOperationException"></exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static TaxonomyType GetTaxonomyType(Guid classificationId)
        {
            TaxonomyManager manager = TaxonomyManager.GetManager();
            var taxonomy = manager.GetTaxonomy(classificationId);
            var type = taxonomy.GetType();

            if (type.IsAssignableFrom(typeof(FlatTaxonomy)))
                return TaxonomyType.Flat;
            else if (type.IsAssignableFrom(typeof(FacetTaxonomy)))
                return TaxonomyType.Facet;
            else if (type.IsAssignableFrom(typeof(HierarchicalTaxonomy)))
                return TaxonomyType.Hierarchical;
            else if (type.IsAssignableFrom(typeof(NetworkTaxonomy)))
                return TaxonomyType.Network;

            throw new InvalidOperationException();
        }

        #endregion
    }
}
