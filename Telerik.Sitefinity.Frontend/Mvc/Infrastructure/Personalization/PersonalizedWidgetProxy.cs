using System;
using System.Web.UI;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Personalization.Impl.Web.UI;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Personalization
{
    /// <summary>
    /// Web forms control that is used as a proxy for personalized MVC controllers and has information about the specific widget it is used for.
    /// </summary>
    public class PersonalizedWidgetProxy : MvcWidgetProxy
    {
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
            this.personalizedViewWrapper.RenderControl(writer);
        }

        #endregion

        #region Private fields

        private PersonalizedViewWrapper personalizedViewWrapper;

        #endregion
    }
}
