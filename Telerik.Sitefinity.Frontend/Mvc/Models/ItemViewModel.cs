using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
    /// <summary>
    /// This class represents view model for items.
    /// </summary>
    /// <remarks>
    /// It is used in Master/detail widgets.
    /// </remarks>
    public class ItemViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemViewModel"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public ItemViewModel(IDataItem item)
        {
            this.DataItem = item;
            this.Fields = new DynamicDataItemFieldAccessor(this);
        }

        /// <summary>
        /// Gets the data item that is represented by this view model.
        /// </summary>
        /// <value>
        /// The data item.
        /// </value>
        public IDataItem DataItem { get; private set; }

        /// <summary>
        /// Gets a property that accesses fields of the data item that is represented by this view model.
        /// </summary>
        public dynamic Fields { get; private set; }

        /// <summary>
        /// Gets the default URL.
        /// </summary>
        /// <value>
        /// The default URL.
        /// </value>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string DefaultUrl
        {
            get
            {
                if (this.DataItem != null)
                    return this.DataItem.GetDefaultUrl();
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the value of the identifier field for the item.
        /// </summary>
        /// <example><see cref="NewsItem"/> identifier is its Title.</example>
        /// <value>
        /// The identifier value.
        /// </value>
        public object Identifier
        {
            get
            {
                if (this.DataItem != null)
                {
                    var field = RelatedDataHelper.GetRelatedTypeIdentifierField(this.DataItem.GetType().FullName);
                    return this.Fields.GetMemberValue(field);
                }
                else
                {
                    return null;
                }
            }
        }

        #region Address field

        /// <summary>
        /// Gets the formatted address.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="addressFormat">The address format.</param>
        /// <returns></returns>
        public virtual string GetFormattedAddress(string fieldName, string addressFormat)
        {
            var cahcedResultKey = this.FieldCacheKey("GetFormattedAddress", fieldName);

            object cachedResult;
            if (this.cachedFieldValues.TryGetValue(cahcedResultKey, out cachedResult))
                return cachedResult as string;

            string result = string.Empty;
            var fieldValue = this.Fields.GetMemberValue(fieldName) as Address;

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

            this.cachedFieldValues[cahcedResultKey] = result;

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
        public virtual string GetDateTime(string fieldName, string fieldFormat)
        {
            var dateTimeValue = (DateTime)this.Fields.GetMemberValue(fieldName);

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
        public virtual string GetPrice(string fieldName, string fieldFormat)
        {
            var fieldValue = this.Fields.GetMemberValue(fieldName);
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
        public virtual string GetBool(string fieldName)
        {
            var boolValue = (bool)this.Fields.GetMemberValue(fieldName);

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
        public virtual string GetMultipleChoiceValueString(string fieldName)
        {
            var multiChoiceValues = this.Fields.GetMemberValue(fieldName) as IEnumerable;
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
        public virtual string GetChoiceLabel(string fieldName, Guid parentTypeId)
        {
            var cahcedResultKey = this.FieldCacheKey("GetChoiceLabel", fieldName);

            object cachedResult;
            if (this.cachedFieldValues.TryGetValue(cahcedResultKey, out cachedResult))
                return cachedResult as string;

            var fieldValue = this.Fields.GetMemberValue(fieldName).ToString();
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

            this.cachedFieldValues[cahcedResultKey] = label;

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
        public virtual IList<string> GetFlatTaxonNames(string fieldName)
        {
            var cahcedResultKey = this.FieldCacheKey("GetFlatTaxonNames", fieldName);
            object cachedResult;
            if (this.cachedFieldValues.TryGetValue(cahcedResultKey, out cachedResult))
                return cachedResult as IList<string>;

            var taxonIds = this.Fields.GetMemberValue(fieldName) as IList<Guid>;
            TaxonomyManager manager = TaxonomyManager.GetManager();

            var taxonNames = manager.GetTaxa<FlatTaxon>()
                    .Where(t => taxonIds.Contains(t.Id) && t.Taxonomy.Name == fieldName)
                    .Select(t => t.Title.ToString()).ToList();

            this.cachedFieldValues[cahcedResultKey] = taxonNames;

            return taxonNames;
        }

        /// <summary>
        /// Gets the hierarchical taxon names.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public virtual IList<string> GetHierarchicalTaxonNames(string fieldName)
        {
            var cahcedResultKey = this.FieldCacheKey("GetHierarchicalTaxonNames", fieldName);
            object cachedResult;
            if (this.cachedFieldValues.TryGetValue(cahcedResultKey, out cachedResult))
                return cachedResult as IList<string>;

            var taxonIds = this.Fields.GetMemberValue(fieldName) as IList<Guid>;
            string taxonomyName;
            TaxonomyManager manager = TaxonomyManager.GetManager();
            if (fieldName == "Category")
                taxonomyName = "Categories";
            else if (fieldName == "Department")
                taxonomyName = "Departments";
            else
                taxonomyName = fieldName;

            var taxonNames = manager.GetTaxa<HierarchicalTaxon>()
                   .Where(t => taxonIds.Contains(t.Id) && t.Taxonomy.Name == taxonomyName)
                   .Select(t => t.Title.ToString()).ToList();
            this.cachedFieldValues[cahcedResultKey] = taxonNames;

            return taxonNames;
        }

        /// <summary>
        /// Gets the name of the flat taxon.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public virtual string GetFlatTaxonName(string fieldName)
        {
            var taxonName = this.GetFlatTaxonNames(fieldName).FirstOrDefault();

            return taxonName;
        }

        /// <summary>
        /// Gets the name of the hierarchical taxon.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public virtual string GetHierarchicalTaxonName(string fieldName)
        {
            string taxonName = this.GetHierarchicalTaxonNames(fieldName).FirstOrDefault();

            return taxonName;
        }

        #endregion

        #region Related data fileds

        /// <summary>
        /// Gets related items.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public virtual IEnumerable<ItemViewModel> RelatedItems(string fieldName)
        {
            var cahcedResultKey = this.FieldCacheKey("RelatedItems", fieldName);
            object cachedResult;
            if (this.cachedFieldValues.TryGetValue(cahcedResultKey, out cachedResult))
                return cachedResult as IEnumerable<ItemViewModel>;
                
            IEnumerable<ItemViewModel> result;
            var relatedItems = this.DataItem.GetRelatedItems(fieldName);
            if (relatedItems != null)
                result = relatedItems.ToArray().Select(item => new ItemViewModel(item)).ToArray();
            else
                result = null;

            this.cachedFieldValues[cahcedResultKey] = result;

            return result;
        }

        /// <summary>
        /// Gets the related item when single.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public virtual ItemViewModel RelatedItem(string fieldName)
        {
            var cahcedResultKey = this.FieldCacheKey("RelatedItem", fieldName);
            object cachedResult;
            if (this.cachedFieldValues.TryGetValue(cahcedResultKey, out cachedResult))
                return cachedResult as ItemViewModel;

            ItemViewModel result;
            var relatedItems = this.DataItem.GetRelatedItems(fieldName);

            if (relatedItems != null)
                result = relatedItems.ToArray().Select(item => new ItemViewModel(item)).FirstOrDefault();
            else
                result = null;

            this.cachedFieldValues[cahcedResultKey] = result;

            return result;
        }

        #endregion

        /// <summary>
        /// Gets the cache key for <see cref="cachedFieldValues"/>.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        private string FieldCacheKey(string methodName, string fieldName)
        {
            return methodName + ":" + fieldName;
        }

        /// <summary>
        /// Contains the cached field values.
        /// </summary>
        private Dictionary<string, object> cachedFieldValues = new Dictionary<string, object>();
    }
}
