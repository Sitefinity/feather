using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Telerik.Sitefinity.Frontend.Resources
{
    /// <summary>
    /// This class provides functionality for registering client resources.
    /// </summary>
    internal class ClientResourceRegister
    {
        #region Constructors
        
        /// <summary>
        /// Constructs a new instances of the <see cref="ClientResourceRegister"/> class.
        /// </summary>
        /// <param name="name">The name of the register.</param>
        /// <param name="resourceTag">The type of the HTML tag that would be generated for every registered resource.</param>
        /// <param name="keyAttribute">The attribute that will be used as identifier in order to ensure that there aren't duplication of resources.</param>
        public ClientResourceRegister(string name, string resourceTag, string keyAttribute)
        {
            this.name = name;
            this.resourceTag = resourceTag;
            this.keyAttribute = keyAttribute;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current <see cref="HttpContextBase"/> instance.
        /// </summary>
        protected virtual HttpContextBase Context
        {
            get
            {
                if (this.context == null)
                    this.context = new HttpContextWrapper(HttpContext.Current);
                return this.context;
            }
        }

        /// <summary>
        /// Gets the current container that contains the registered resources.
        /// </summary>
        protected virtual HashSet<string> Container
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

        #endregion

        #region Public Methods

        /// <summary>
        /// Registers a client resource.
        /// </summary>
        /// <param name="attributes">The attributes associated with the resource.</param>
        /// <returns>An HTML tag that contains the resource declaration.</returns>
        public string RegisterResource(params KeyValuePair<string, string>[] attributes)
        {
            var resourceKey = attributes.Single(a => a.Key == this.keyAttribute).Value;
           
            if (this.Container.Contains(resourceKey))
                throw new Exception(string.Format("{0} is already registered!", resourceKey));

            var output = this.RegisterResourceInternal(attributes);
            return output;
        }

        /// <summary>
        /// Registers a client resource. A return value indicates whether the registration succeeded.
        /// </summary>
        /// <param name="output">When this method returns, contains an HTML tag that contains the resource declaration,
        /// if the registration succeeded, or null if the registration failed.</param>
        /// <param name="attributes">The attributes associated with the resource.</param>
        /// <returns><value>true</value> if s was converted successfully; otherwise, <value>false</value>.</returns>
        public bool TryRegisterResource(out string output, params KeyValuePair<string, string>[] attributes)
        {
            bool result;

            if (!this.Container.Contains(attributes.Single(a => a.Key == this.keyAttribute).Value))
            {
                output = this.RegisterResourceInternal(attributes);
                result = true;
            }
            else
            {
                output = null;
                result = false;
            }

            return result;
        }

        #endregion

        #region Private Methods

        private string CreateTag(params KeyValuePair<string, string>[] attribbutes)
        {
            var tag = new TagBuilder(this.resourceTag);

            foreach (var attr in attribbutes)
                tag.Attributes[attr.Key] = attr.Value;

            return tag.ToString();
        }

        private string RegisterResourceInternal(params KeyValuePair<string, string>[] attribbutes)
        {
            string output = this.CreateTag(attribbutes);
            this.Container.Add(attribbutes.Single(a=>a.Key == this.keyAttribute).Value);
            return output;
        }
        
        #endregion
        
        #region Fields

        private HttpContextBase context;
        private HashSet<string> container;
        private string name;
        private string resourceTag;
        private string keyAttribute;
            
        #endregion
    }
}