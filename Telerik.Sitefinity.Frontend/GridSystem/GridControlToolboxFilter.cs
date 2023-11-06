﻿using System;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.DesignerToolbox;
using Telerik.Sitefinity.Frontend.Mvc.Helpers;
using Telerik.Sitefinity.Modules.Newsletters;
using Telerik.Sitefinity.Modules.Newsletters.Configuration;
using Telerik.Sitefinity.Modules.Newsletters.Web;
using Telerik.Sitefinity.Newsletters.Model;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Utilities.TypeConverters;
using Telerik.Sitefinity.Web.UI;

namespace Telerik.Sitefinity.Frontend.GridSystem
{
    /// <summary>
    /// Determines which layout controls should be visible in the toolbox of the <see cref="ZoneEditor"/>.
    /// </summary>
    internal class GridControlToolboxFilter : IToolboxFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridControlToolboxFilter"/> class.
        /// </summary>
        /// <param name="frameworkExtractor">Function that returns the Framework for the current context.</param>
        public GridControlToolboxFilter(Func<PageTemplateFramework> frameworkExtractor)
        {
            this.frameworkExtractor = frameworkExtractor;
        }

        /// <summary>
        /// Determines whether a toolbox section should be visible in the <see cref="ZoneEditor"/>.
        /// </summary>
        /// <param name="section">The section in question.</param>
        /// <param name="context">The filter context passed externally.</param>
        /// <returns><c>true</c> if it should be visible.</returns>
        public bool IsSectionVisible(IToolboxSection section, IToolboxFilterContext context)
        {
            if (section != null && context != null && section.Name == GridWidgetRegistrator.GridSectionName && section.Title == GridWidgetRegistrator.GridSectionTitle)
                return SystemManager.GetModule("Feather") != null && context.ContainerId == "LayoutToolboxContainer";

            return true;
        }

        /// <summary>
        /// Determines whether a tool (a.k.a toolbox item) should be visible in the <see cref="ZoneEditor"/>.
        /// </summary>
        /// <param name="tool">The tool in question.</param>
        /// <returns><c>true</c> if it should be visible.</returns>
        public bool IsToolVisible(IToolboxItem tool)
        {
            if (tool == null)
                return true;

            var currentFramework = this.frameworkExtractor();

            // Pages: Everything works for Hybrid mode. No need to reflect on the specific item.
            // Newsletters: Always run in Hybrid mode. Message body type needs check.
            if (currentFramework == PageTemplateFramework.Hybrid)
            {
                var currentHttpContext = SystemManager.CurrentHttpContext;

                if (NewslettersHelper.IsMvcNewsletter())
                {
                    currentFramework = PageTemplateFramework.Mvc;
                }
                else
                {
                    return true;
                }
            }

            if (!tool.ControlType.IsNullOrEmpty())
            {
                var controlType = TypeResolutionService.ResolveType(tool.ControlType, throwOnError: false);
                if (controlType == null)
                    return true;

                var isLayoutControl = typeof(LayoutControl).IsAssignableFrom(controlType);
                var isGridControl = typeof(GridControl).IsAssignableFrom(controlType);

                if (currentFramework == PageTemplateFramework.WebForms && isGridControl)
                    return false;

                if (currentFramework == PageTemplateFramework.Mvc && isLayoutControl && !isGridControl)
                    return false;
            }

            return true;
        }

        private bool IsNewslettersModuleEnabled
        {
            get
            {
                if (this.isNewslettersModuleEnabled == null)
                {
                    try
                    {
                        Config.Get<NewslettersConfig>();
                        this.isNewslettersModuleEnabled = true;
                    }
                    catch (Exception)
                    {
                        this.isNewslettersModuleEnabled = false;
                    }
                }

                return this.isNewslettersModuleEnabled.Value;
            }
        }

        private bool? isNewslettersModuleEnabled;
        private readonly Func<PageTemplateFramework> frameworkExtractor;
    }
}
