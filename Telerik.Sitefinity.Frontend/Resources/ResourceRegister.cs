using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Frontend.Mvc.Helpers;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts;

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
        public ResourceRegister(string name, HttpContextBase httpContext)
        {
            this.name = name;
            this.context = httpContext;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a set of already rendered resources.
        /// </summary>
        protected virtual HashSet<string> Rendered
        {
            get
            {
                if (this.Context.Items.Contains(this.name + ResourceRegister.RenderedResourcesKeySuffix))
                {
                    this.renderedRes = (HashSet<string>)this.Context.Items[this.name + ResourceRegister.RenderedResourcesKeySuffix];
                }
                else
                {
                    this.renderedRes = new HashSet<string>();
                    this.Context.Items.Add(this.name + ResourceRegister.RenderedResourcesKeySuffix, this.renderedRes);
                }

                return this.renderedRes;
            }
        }

        /// <summary>
        /// Gets the current container that contains the registered resources.
        /// </summary>
        protected virtual Dictionary<string, List<ResourceItem>> Container
        {
            get
            {
                if (this.Context.Items.Contains(this.name))
                {
                    this.container = (Dictionary<string, List<ResourceItem>>)this.Context.Items[this.name];

                    if (this.container == null)
                    {
                        this.container = new Dictionary<string, List<ResourceItem>>();
                    }
                }
                else
                {
                    this.container = new Dictionary<string, List<ResourceItem>>();
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
        public bool Register(string resourceKey, string sectionName = null, bool throwException = false)
        {
            var successfullyRegistered = this.Register(resourceKey, sectionName, throwException, null);

            return successfullyRegistered;
        }

        /// <summary>
        /// Registers a client resource. A return value indicates whether the registration succeeded.
        /// </summary>
        /// <param name="resourceKey">The attributes associated with the resource.</param>
        /// <param name="sectionName">The section name in which the resource should be rendered.</param>
        /// <param name="throwException">The section name in which the resource should be rendered.</param>
        /// <param name="attributes">The attributes that should be added to the resource.</param>
        /// <returns>
        /// <value>true</value> if s was registered successfully; otherwise, <value>false</value>.
        /// </returns>
        public bool Register(string resourceKey, string sectionName = null, bool throwException = false, List<KeyValuePair<string, string>> attributes = null)
        {
            if (sectionName != null && !ResourceHelper.RenderScriptSection)
            {
                var page = this.Context.Handler.GetPageHandler();
                if (throwException && page != null && (page.Master is MvcMasterPage) && !SectionRenderer.IsAvailable(page, sectionName))
                {
                    throw new ArgumentException("A section with name \"{0}\" could not be found.".Arrange(sectionName), sectionName);
                }
                else if (page == null)
                {
                    sectionName = null;
                }
            }

            if (string.IsNullOrEmpty(sectionName))
                sectionName = ResourceRegister.DefaultSectionNameKey;

            bool successfullyRegistered = true;

            var resourceItem = new ResourceItem(resourceKey, attributes);
            if (this.Container.ContainsKey(sectionName))
            {
                if (this.Container[sectionName].Any(x => x.ResourceKey == resourceKey))
                    successfullyRegistered = false;
                else
                    this.Container[sectionName].Add(resourceItem);
            }
            else
                this.Container.Add(sectionName, new List<ResourceItem>() { resourceItem });


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
        public bool IsRegistered(string resourceKey, string sectionName = null)
        {
            if (string.IsNullOrEmpty(sectionName))
                sectionName = ResourceRegister.DefaultSectionNameKey;

            return this.Container.ContainsKey(sectionName) && this.Container[sectionName].Any(r => r.ResourceKey == resourceKey);
        }

        /// <summary>
        /// Checks if a client resource is registered for a page.
        /// </summary>
        /// <param name="resourceKey">The attributes associated with the resource.</param>
        /// <returns>
        /// <value>true</value> if s was registered; otherwise, <value>false</value>.
        /// </returns>
        internal bool IsRegistered(string resourceKey)
        {
            foreach (var section in this.Container.Values)
            {
                if (section.Any(r => r.ResourceKey == resourceKey))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified resource is already rendered.
        /// </summary>
        /// <param name="resourceKey">The resource key.</param>
        /// <returns>
        /// Whether the specified resource is already rendered.
        /// </returns>
        public bool IsRendered(string resourceKey)
        {
            return this.Rendered.Contains(resourceKey);
        }

        /// <summary>
        /// Marks the resource as rendered.
        /// </summary>
        /// <param name="resourceKey">The resource key.</param>
        public void MarkAsRendered(string resourceKey)
        {
            this.Rendered.Add(resourceKey);
        }

        /// <summary>
        /// Get all resource items for a section.
        /// </summary>
        /// <param name="sectionName">The name of the section key.</param>
        /// <returns>
        /// A collection of all resource items for a section.
        /// </returns>
        public IEnumerable<ResourceItem> GetResourceItemsForSection(string sectionName)
        {
            if (this.Container.ContainsKey(sectionName))
            {
                return this.Container[sectionName];
            }

            return new List<ResourceItem>();
        }

        /// <summary>
        /// Get all inline resources with their attributes.
        /// </summary>
        /// <returns>
        /// A collection of all inline resources and their attributes.
        /// </returns>
        public IEnumerable<ResourceItem> GetInlineResourceItems()
        {
            return this.GetResourceItemsForSection(ResourceRegister.DefaultSectionNameKey);
        }

        #endregion

        #region Fields

        private HttpContextBase context;
        private Dictionary<string, List<ResourceItem>> container;
        private HashSet<string> renderedRes;
        private string name;

        private const string DefaultSectionNameKey = "ResourceRegisterInlineResourceSectionName";
        private const string RenderedResourcesKeySuffix = "-rendered";

        #endregion
    }
}