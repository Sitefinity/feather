using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using Telerik.Sitefinity.Frontend.Mvc.Helpers;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// This control is responsible for rendering the section content.
    /// </summary>
    public class SectionRenderer : Control
    {
        /// <summary>
        /// Gets or sets the name of the section.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Determines whether the specified section is available on the page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="sectionName">Name of the section.</param>
        /// <returns>True if the section is available on the page and false otherwise.</returns>
        internal static bool IsAvailable(Page page, string sectionName)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            if (page == null)
                return false;

            return page.Items.Contains(SectionRenderer.AvailableSectionsKey) && ((ISet<string>)page.Items[SectionRenderer.AvailableSectionsKey]).Contains(sectionName);
        }

        /// <summary>
        /// Marks the specified section as availabile.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="sectionName">Name of the section.</param>
        /// <exception cref="DuplicateSectionException"></exception>
        internal static void MarkAvailability(Page page, string sectionName)
        {
            if (page != null && !sectionName.IsNullOrEmpty())
            {
                ISet<string> availableSections;
                if (page.Items.Contains(SectionRenderer.AvailableSectionsKey))
                {
                    availableSections = (ISet<string>)page.Items[SectionRenderer.AvailableSectionsKey];
                }
                else
                {
                    availableSections = new HashSet<string>();
                    page.Items.Add(SectionRenderer.AvailableSectionsKey, availableSections);
                }

                if (!availableSections.Contains(sectionName))
                {
                    availableSections.Add(sectionName);
                }
                else
                {
                    throw new DuplicateSectionException(sectionName);
                }
            }
        }

        /// <summary>
        /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter" /> object, which writes the content to be rendered on the client.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the server control content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            var context = new HttpContextWrapper(this.Context);
            writer.Write(ResourceHelper.RenderAllScripts(context, this.Name));
            writer.Write(ResourceHelper.RenderAllStylesheets(context, this.Name));
        }

        private const string AvailableSectionsKey = "available-sections";
    }
}
