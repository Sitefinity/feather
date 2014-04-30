using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Pages.Model;

namespace Telerik.Sitefinity.Frontend.Test.DummyClasses
{
    /// <summary>
    /// This class presents dummy pate template which is framework specific.
    /// </summary>
    public class DummyFrameworkSpecificPageTemplate : IFrameworkSpecificPageTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DummyFrameworkSpecificPageTemplate"/> class.
        /// </summary>
        /// <param name="framework">The framework.</param>
        public DummyFrameworkSpecificPageTemplate(PageTemplateFramework framework) 
        {
            this.Framework = framework;
        }

        public PageTemplateFramework Framework { get; set; }

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
            get { return DummyFrameworkSpecificPageTemplate.CurrentId; }
        }


        public string Theme
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<PresentationData> Presentation
        {
            get { throw new NotImplementedException(); }
        }

        public static Guid CurrentId = Guid.Parse("18c314b8-e38f-4119-9191-485538955f02");
    }
}
