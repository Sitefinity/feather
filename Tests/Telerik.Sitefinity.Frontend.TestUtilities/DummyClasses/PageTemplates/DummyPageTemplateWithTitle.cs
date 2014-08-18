using System;
using System.Collections.Generic;
using System.Globalization;

using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Pages.Model;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.PageTemplates
{
    /// <summary>
    /// This class creates fake framework specific page template with title. It is used for test purposes only.
    /// </summary>
    public class DummyPageTemplateWithTitle : IFrameworkSpecificPageTemplate, IHasTitle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DummyPageTemplateWithTitle"/> class.
        /// </summary>
        /// <param name="framework">
        /// The framework.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        public DummyPageTemplateWithTitle(PageTemplateFramework framework, string title) 
        {
            this.Framework = framework;
            this.Title = title;
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetTitle(CultureInfo culture = null)
        {
            return this.Title;
        }

        /// <inheritdoc />
        public PageTemplateFramework Framework { get; set; }

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
        public int LastControlId
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc />
        public string ExternalPage
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc />
        public bool IncludeScriptManager
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc />
        public bool IsPersonalized
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc />
        public string MasterPage
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc />
        public Guid PersonalizationMasterId
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc />
        public Guid PersonalizationSegmentId
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
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

        /// <summary>
        /// Gets or sets the page template title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }
    }
}
