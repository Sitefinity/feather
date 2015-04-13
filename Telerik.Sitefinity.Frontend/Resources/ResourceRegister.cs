using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;

namespace Telerik.Sitefinity.Frontend.Resources
{
    /// <summary>
    /// This class provides functionality for registering client resources.
    /// </summary>
    internal class ResourceRegister
    {
        #region Constructors

        /// <summary>
        /// Constructs a new instances of the <see cref="ResourceRegister" /> class.
        /// </summary>
        /// <param name="name">The name of the register.</param>
        /// <param name="httpContext">The HTTP context.</param>
        public ResourceRegister(string name, System.Web.HttpContextBase httpContext)
        {
            this.name = name;
            this.context = httpContext;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current container that contains the registered resources.
        /// </summary>
        protected internal virtual Dictionary<string, List<string>> Container
        {
            get
            {
                if (this.Context.Items.Contains(this.name))
                {
                    this.container = (Dictionary<string, List<string>>)this.Context.Items[this.name];
                }
                else
                {
                    this.container = new Dictionary<string, List<string>>();
                    this.Context.Items.Add(this.name, this.container);
                }

                return this.container;
            }
        }

        /// <summary>
        /// Gets the current <see cref="HttpContextBase"/> instance.
        /// </summary>
        protected virtual HttpContextBase Context
        {
            get
            {
                return this.context;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Registers a client resource. A return value indicates whether the registration succeeded.
        /// </summary>
        /// <param name="resourceKey">The attributes associated with the resource.</param>
        /// <param name="sectionName">The section name in which the resource should be rendered.</param>
        /// <param name="throwException">The section name in which the resource should be rendered.</param>
        /// <returns>
        /// <value>true</value> if s was registered successfully; otherwise, <value>false</value>.
        /// </returns>
        public bool RegisterResource(string resourceKey, string sectionName = null, bool throwException = false)
        {
            if (string.IsNullOrEmpty(sectionName))
                sectionName = ResourceRegister.DefaultSectionNameKey;

            bool successfullyRegistered = true;

            if (this.Container.ContainsKey(sectionName))
            {
                if (this.Container[sectionName].Contains(resourceKey))
                    successfullyRegistered = false;
                else
                    this.Container[sectionName].Add(resourceKey);
            }
            else
                this.Container.Add(sectionName, new List<string>() { resourceKey });

            if (throwException && !successfullyRegistered)
            {
                throw new ArgumentException(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} is already registered {1}!", resourceKey, string.IsNullOrEmpty(sectionName) ? "inline" : string.Format("in the {0} section", sectionName)));
            }

            return successfullyRegistered;
        }

        /// <summary>
        /// Checks if a client resource is registered for a section.
        /// </summary>
        /// <param name="resourceKey">The attributes associated with the resource.</param>
        /// <param name="sectionName">The section name in which the resource should be rendered.</param>
        /// <returns>
        /// <value>true</value> if s was registered; otherwise, <value>false</value>.
        /// </returns>
        public bool IsResourceRegistered(string resourceKey, string sectionName = null)
        {
            if (string.IsNullOrEmpty(sectionName))
                sectionName = ResourceRegister.DefaultSectionNameKey;

            return this.Container.ContainsKey(sectionName) && this.Container[sectionName].Contains(resourceKey);
        }

        #endregion

        #region Fields

        private HttpContextBase context;
        private Dictionary<string, List<string>> container;
        private string name;

        internal const string DefaultSectionNameKey = "InternalResourceRegisterInlineResourceSectionName";

        #endregion
    }
}