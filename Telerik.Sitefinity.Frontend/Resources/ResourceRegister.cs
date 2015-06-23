using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
        protected virtual Dictionary<string, List<string>> Container
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
        public bool Register(string resourceKey, string sectionName = null, bool throwException = false)
        {
            if (sectionName != null)
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

            if (this.Container.ContainsKey(sectionName))
            {
                if (this.Container[sectionName].Contains(resourceKey))
                    successfullyRegistered = false;
                else
                    this.Container[sectionName].Add(resourceKey);
            }
            else
                this.Container.Add(sectionName, new List<string>() { resourceKey });

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

            return this.Container.ContainsKey(sectionName) && this.Container[sectionName].Contains(resourceKey);
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
        /// Get all resources for a section.
        /// </summary>
        /// <param name="sectionName">The name of the section key.</param>
        /// <returns>
        /// A collection of all resources for a section.
        /// </returns>
        public IEnumerable<string> GetResourcesForSection(string sectionName)
        {
            if (this.Container.ContainsKey(sectionName))
            {
                return this.Container[sectionName];
            }

            return new List<string>();
        }

        /// <summary>
        /// Get all inline resources.
        /// </summary>
        /// <returns>
        /// A collection of all inline resources.
        /// </returns>
        public IEnumerable<string> GetInlineResources()
        {
            return this.GetResourcesForSection(ResourceRegister.DefaultSectionNameKey);
        }

        #endregion

        #region Fields

        private HttpContextBase context;
        private Dictionary<string, List<string>> container;
        private HashSet<string> renderedRes;
        private string name;

        private const string DefaultSectionNameKey = "ResourceRegisterInlineResourceSectionName";
        private const string RenderedResourcesKeySuffix = "-rendered";

        #endregion
    }
}