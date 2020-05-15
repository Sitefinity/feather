using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Frontend.Mvc.Helpers;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Personalization.Impl.Configuration;
using Telerik.Sitefinity.Personalization.Impl.Web.UI;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Personalization
{
    /// <summary>
    /// Web forms control that is used as a proxy for personalized MVC controllers and has information about the specific widget it is used for.
    /// </summary>
    public class PersonalizedWidgetProxy : MvcWidgetProxy
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonalizedWidgetProxy"/> class.
        /// </summary>
        public PersonalizedWidgetProxy()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonalizedWidgetProxy"/> class.
        /// </summary>
        /// <param name="controlDataId">The control data ID.</param>
        /// <param name="pageDataId">The page data ID.</param>
        /// <param name="controlTypeName">Full name of the control type.</param>
        public PersonalizedWidgetProxy(Guid controlDataId, Guid pageDataId, string controlTypeName)
        {
            this.ControlDataId = controlDataId;
            this.PageDataId = pageDataId;
            this.ControlTypeName = controlTypeName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the control data ID.
        /// </summary>
        /// <value>
        /// The control data ID.
        /// </value>
        public Guid ControlDataId { get; set; }

        /// <summary>
        /// Gets or sets the page data ID.
        /// </summary>
        /// <value>
        /// The page data ID.
        /// </value>
        public Guid PageDataId { get; set; }

        /// <summary>
        /// Gets or sets the name of the control type.
        /// </summary>
        /// <value>
        /// The name of the control type.
        /// </value>
        public string ControlTypeName { get; set; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            this.personalizedViewWrapper = new PersonalizedViewWrapper(this.ControlDataId, this.PageDataId, this.ControlTypeName);
            this.Controls.Add(this.personalizedViewWrapper);
        }

        /// <inheritdoc />
        protected override void Render(HtmlTextWriter writer)
        {
            var isMvcDetailsView = (string)(this.Controller.RouteData.Values["action"]) == "Details";
            this.personalizedViewWrapper.RaiseEvents = !isMvcDetailsView;

            if (this.Page.Items["ScriptSourcesLoaded"] == null)
            {
                var currentNode = SiteMapBase.GetActualCurrentNode();
                if (currentNode != null && currentNode.Framework == PageTemplateFramework.Mvc)
                {
                    var registeredScripts = SystemManager.CurrentHttpContext.Items[ResourceHelper.JsRegisterName] as Dictionary<string, List<string>>;
                    if (registeredScripts != null)
                    {
                        this.personalizedViewWrapper.LoadedScripts = registeredScripts.SelectMany(p => p.Value);
                    }
                }
                else
                {
                    var scriptManager = ScriptManager.GetCurrent(this.Page);
                    if (scriptManager != null && scriptManager.Scripts != null && scriptManager.Scripts.Count > 0)
                    {
                        var scriptRef = scriptManager.Scripts.Select(r => new ResourceHelper.MvcScriptReference(r));
                        this.personalizedViewWrapper.LoadedScripts = scriptRef.Select(r => r.GetResourceUrl());
                    }
                }

                this.Page.Items["ScriptSourcesLoaded"] = true;
            }

            this.personalizedViewWrapper.RenderControl(writer);
        }

        #endregion

        #region Private fields

        private PersonalizedViewWrapper personalizedViewWrapper;

        #endregion
    }
}