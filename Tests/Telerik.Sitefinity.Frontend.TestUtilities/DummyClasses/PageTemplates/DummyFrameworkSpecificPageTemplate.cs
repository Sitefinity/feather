using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Pages.Model;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.PageTemplates
{
    /// <summary>
    /// This class presents dummy implementation of <see cref="Telerik.Sitefinity.Pages.Model.IFrameworkSpecificPageTemplate"/> which is framework specific.
    /// </summary>
    public class DummyFrameworkSpecificPageTemplate : IFrameworkSpecificPageTemplate
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyFrameworkSpecificPageTemplate"/> class.
        /// </summary>
        /// <param name="framework">The framework.</param>
        public DummyFrameworkSpecificPageTemplate(PageTemplateFramework framework) 
        {
            this.Framework = framework;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the framework.
        /// </summary>
        /// <value>
        /// The framework.
        /// </value>
        public PageTemplateFramework Framework { get; set; }

        #endregion

        #region Overrides

        /// <inheritdoc />
        public int LastControlId { get; set; }

        /// <inheritdoc />
        public string ExternalPage { get; set; }

        /// <inheritdoc />
        public bool IncludeScriptManager { get; set; }

        /// <inheritdoc />
        public bool IsPersonalized { get; set; }

        /// <inheritdoc />
        public string MasterPage { get; set; }

        /// <inheritdoc />
        public Guid PersonalizationMasterId { get; set; }

        /// <inheritdoc />
        public Guid PersonalizationSegmentId { get; set; }

        /// <inheritdoc />
        public string Key
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc />
        public IPageTemplate ParentTemplate
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<ControlData> Controls
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc />
        public Guid Id
        {
            get { return DummyFrameworkSpecificPageTemplate.currentId; }
        }

        /// <inheritdoc />
        public string Theme
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc />
        public IEnumerable<PresentationData> Presentation
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Private fields and constants

        private static Guid currentId = Guid.Parse("18c314b8-e38f-4119-9191-485538955f02");

        #endregion
    }
}
