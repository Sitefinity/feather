using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using Telerik.Sitefinity.GeoLocations.Model;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Locations.Configuration;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.RelatedData;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    public class ItemViewModel : DynamicObject
    {
        public ItemViewModel(IDataItem item)
        {
            this.OriginalItem = item;
        }

        public IDataItem OriginalItem { get; set; }

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name;
            var isFound = false;
            result = null;

            var propInfo = TypeDescriptor.GetProperties(this.OriginalItem)[name];

            if (propInfo != null)
            {
                result = propInfo.GetValue(this.OriginalItem);
                isFound = true;
            }

            return isFound;
        }

        #region Address field

        /// <summary>
        /// Gets the formatted address.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="addressFormat">The address format.</param>
        /// <returns></returns>
        public string GetFormattedAddress(string fieldName, string addressFormat)
        {
            string result = string.Empty;
            var fieldValue = this.GetMemberValue(fieldName) as Address;

            if (fieldValue != null && !addressFormat.IsNullOrEmpty())
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

        #endregion

        #region DateTime field

        /// <summary>
        /// Gets formatted date time value.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldFormat">The field format.</param>
        /// <returns></returns>
        public string GetDateTime(string fieldName, string fieldFormat)
        {
            var dateTimeValue = (DateTime)this.GetMemberValue(fieldName);

            if (dateTimeValue == default(DateTime))
                return null;

            var formattedDate = dateTimeValue.ToSitefinityUITime().ToString(fieldFormat);

            return formattedDate;
        }

        #endregion

        #region Price field

        /// <summary>
        /// Gets formatted date time value.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldFormat">The field format.</param>
        /// <returns></returns>
        public string GetPrice(string fieldName, string fieldFormat)
        {
            var fieldValue = this.GetMemberValue(fieldName);
            if (fieldValue == null)
                return null;

            var formattedValue = string.Format(fieldValue.ToString(), fieldFormat);

            return formattedValue;
        }

        #endregion

        #region Yes/No field

        /// <summary>
        /// Gets formatted date time value.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldFormat">The field format.</param>
        /// <returns></returns>
        public string GetBool(string fieldName)
        {
            var boolValue = (bool)this.GetMemberValue(fieldName);

            return boolValue ? Res.Get<Labels>().Yes : Res.Get<Labels>().No;
        }

        #endregion

        #region Choice fields

        /// <summary>
        /// Gets the multiple choice value string.
        /// </summary>
        /// <param name="multiChoiceValues">The multi choice values.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "multi")]
        public string GetMultipleChoiceValueString(string fieldName)
        {
            var multiChoiceValues = this.GetMemberValue(fieldName) as IEnumerable;
            if (multiChoiceValues == null)
                return null;

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

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="parentTypeId">The parent type identifier.</param>
        /// <returns></returns>
        public string GetChoiceLabel(string fieldName, Guid parentTypeId)
        {
            var fieldValue = this.GetMemberValue(fieldName).ToString();
            Telerik.Sitefinity.DynamicModules.Builder.ModuleBuilderManager man = new Telerik.Sitefinity.DynamicModules.Builder.ModuleBuilderManager();
            var moduleType = man.Provider.GetDynamicModuleType(parentTypeId);
            string label = fieldValue;

            if (moduleType.Fields != null)
            {
                var field = moduleType.Fields.Where(f => f.Name == fieldName).FirstOrDefault();
                if (field != null)
                {
                    System.Xml.Linq.XDocument doc = System.Xml.Linq.XDocument.Parse(field.Choices);
                    var element = doc.Elements("element").Where(e => e.Attribute("value").Value == fieldValue).FirstOrDefault();
                    label = (element != null) ? element.Attribute("text").Value : fieldValue;
                }
            }

            return label;
        }

        #endregion

        #region Taxon fields

        /// <summary>
        /// Gets the taxon names.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="classificationId">The classification identifier.</param>
        /// <returns></returns>
        public IList<string> GetFlatTaxonNames(string fieldName)
        {
            var taxonIds = this.GetMemberValue(fieldName) as IList<Guid>;
            TaxonomyManager manager = TaxonomyManager.GetManager();

            var taxonNames = manager.GetTaxa<FlatTaxon>()
                    .Where(t => taxonIds.Contains(t.Id) && t.Taxonomy.Name == fieldName)
                    .Select(t => t.Title.ToString()).ToList();

            return taxonNames;
        }

        public IList<string> GetHierarchicalTaxonNames(string fieldName)
        {
            var taxonIds = this.GetMemberValue(fieldName) as IList<Guid>;
            TaxonomyManager manager = TaxonomyManager.GetManager();
            var taxonNames = manager.GetTaxa<HierarchicalTaxon>()
                   .Where(t => taxonIds.Contains(t.Id) && t.Taxonomy.Name == fieldName)
                   .Select(t => t.Title.ToString()).ToList();

            return taxonNames;
        }

        #endregion

        #region Related data fileds

        /// <summary>
        /// Gets related items.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public IList<IDataItem> RelatedItems(string fieldName)
        {
            return this.OriginalItem.GetRelatedItems(fieldName).ToList<IDataItem>();
        }

        #endregion

        /// <summary>
        /// Gets the member value.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        protected object GetMemberValue(string fieldName)
        {
            var propInfo = TypeDescriptor.GetProperties(this.OriginalItem)[fieldName];

            if (propInfo != null)
            {
                return propInfo.GetValue(this.OriginalItem);
            }

            return null;
        }
    }
}
