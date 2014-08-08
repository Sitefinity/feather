using System;
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
        /// Constructs a new instances of the <see cref="ResourceRegister"/> class.
        /// </summary>
        /// <param name="name">The name of the register.</param>
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
        protected internal virtual HashSet<string> Container
        {
            get
            {
                if (this.Context.Items.Contains(this.name))
                {
                    this.container = (HashSet<string>)this.Context.Items[this.name];
                }
                else
                {
                    this.container = new HashSet<string>();
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
        /// Registers a client resource.
        /// </summary>
        /// <param name="resourceKey">The attributes associated with the resource.</param>
        public void RegisterResource(string resourceKey)
        {
            if (this.IsRegistered(resourceKey))
                throw new ArgumentException(string.Format("{0} is already registered!", resourceKey));

            this.Register(resourceKey);
        }

        /// <summary>
        /// Registers a client resource. A return value indicates whether the registration succeeded.
        /// </summary>
        /// <returns><value>true</value> if s was registered successfully; otherwise, <value>false</value>.</returns>
        public bool TryRegisterResource(string resourceKey)
        {
            bool result;

            if (!this.IsRegistered(resourceKey))
            {
                this.Register(resourceKey);
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        #endregion

        #region Private Methods

        private bool IsRegistered(string resourceKey)
        {
            return this.Container.Contains(resourceKey);
        }

        private void Register(string resourceKey)
        {
            this.Container.Add(resourceKey);
        }

        #endregion

        #region Fields

        private HttpContextBase context;
        private HashSet<string> container;
        private string name;

        #endregion
    }
}