using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Pages.Model;

namespace Telerik.Sitefinity.Frontend.Test.DummyClasses
{
    /// <summary>
    /// This class represents Dummy page template which is not framework specific. 
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

        public string Key
        {
            get { throw new NotImplementedException(); }
        }

        public IPageTemplate ParentTemplate
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<ControlData> Controls
        {
            get { throw new NotImplementedException(); }
        }

        public Guid Id
        {
            get { throw new NotImplementedException(); }
        }


        public string Theme
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<PresentationData> Presentation
        {
            get { throw new NotImplementedException(); }
        }
    }
}
