using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Pages.Model;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.PageTemplates
{
    /// <summary>
    /// This class represents dummy implementation of <see cref="Telerik.Sitefinity.Pages.Model.IPageTemplate"/> which is not framework specific. 
    /// </summary>
    public class DummyPageTemplate : IPageTemplate
    {
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

        /// <inheritdoc />
        public IEnumerable<ControlData> Controls
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc />
        public Guid Id
        {
            get { throw new NotImplementedException(); }
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
    }
}
