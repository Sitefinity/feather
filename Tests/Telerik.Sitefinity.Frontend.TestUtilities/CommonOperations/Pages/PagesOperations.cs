using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.TestIntegration.Helpers;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations
{
    /// <summary>
    /// Provides common pages operations
    /// </summary>
    public class PagesOperations
    {
        /// <summary>
        /// Creates a page with template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="pageTitle">The page title.</param>
        /// <param name="pageUrlName">Name of the page URL.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#")]
        public Guid CreatePageWithTemplate(PageTemplate template, string pageTitle, string pageUrlName)
        {
            Guid pageId = Guid.Empty;
            App.WorkWith()
               .Page()
               .CreateNewStandardPage()
               .Do(p =>
               {
                   p.GetPageData().Template = template;
                   p.Title = pageTitle;
                   p.UrlName = pageUrlName;
                   pageId = p.Id;
               })
               .CheckOut()
               .Publish(CultureInfo.InvariantCulture)
               .SaveChanges();
            return pageId;
        }

        /// <summary>
        /// Adds Mvc widget to existing page
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="controllerType"></param>
        /// <param name="widgetName"></param>
        /// <param name="placeHolderId"></param>
        public void AddMvcWidgetToPage(Guid pageId, string controllerType, string widgetName, string placeHolderId)
        {
            PageManager pageManager = PageManager.GetManager();
            pageManager.Provider.SuppressSecurityChecks = true;
            var pageDataId = pageManager.GetPageNode(pageId).PageId;
            var page = pageManager.EditPage(pageDataId, CultureInfo.CurrentUICulture);

            var mvcWidget = new Telerik.Sitefinity.Mvc.Proxy.MvcControllerProxy();

            mvcWidget.ControllerName = controllerType;

            var draftControlDefault = pageManager.CreateControl<PageDraftControl>(mvcWidget, placeHolderId);
            draftControlDefault.Caption = widgetName;

            pageManager.SetControlDefaultPermissions(draftControlDefault);
            page.Controls.Add(draftControlDefault);

            pageManager.PublishPageDraft(page, CultureInfo.CurrentUICulture);
            pageManager.SaveChanges();
        }

        /// <summary>
        /// Gets the public page content.
        /// </summary>
        /// <param name="pageId">The id of the page.</param>
        /// <returns>The page content.</returns>
        public string GetPageContent(Guid pageId)
        {
            PageManager pageManager = PageManager.GetManager();

            var page = pageManager.GetPageNode(pageId);
            var pageUrl = page.GetFullUrl();
            pageUrl = RouteHelper.GetAbsoluteUrl(pageUrl);
            pageUrl = UrlTransformations.AppendParam(pageUrl, "t", Guid.NewGuid().ToString());

            string pageContent = WebRequestHelper.GetPageWebContent(pageUrl);

            return pageContent;
        }

        /// <summary>
        /// Creates a pure MVC template.
        /// </summary>
        /// <param name="templateTitle">The template title.</param>
        /// <returns>The template ID.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        public Guid CreatePureMvcTemplate(string templateTitle)
        {
            Guid templateId = Guid.Empty;
            var fluent = App.WorkWith();
            templateId = fluent.PageTemplate()
                               .CreateNew()
                               .Do(t =>
                               {
                                   t.Title = templateTitle;
                                   t.Name = templateTitle.ToLower(CultureInfo.InvariantCulture);
                                   t.Description = templateTitle + " descr";
                                   t.ShowInNavigation = true;
                                   t.Framework = PageTemplateFramework.Mvc;
                                   t.Visible = true;
                               })
                               .SaveAndContinue()
                               .Get()
                               .Id;
            var pageManager = PageManager.GetManager();
            var template = pageManager.GetTemplates().Where(t => t.Id == templateId).SingleOrDefault();
            var master = pageManager.TemplatesLifecycle.Edit(template);
            pageManager.TemplatesLifecycle.Publish(master);
            pageManager.SaveChanges();

            return template.Id;
        }

        /// <summary>
        /// Creates pure MVC template.
        /// </summary>
        /// <param name="templateTitle">The template title.</param>
        /// <param name="parentTemplateTitle">The parent template title.</param>
        /// <returns>The template.</returns>
        public PageTemplate CreatePureMvcTemplate(string templateTitle, string parentTemplateTitle)
        {
            var pageManager = PageManager.GetManager();

            var templateId = this.CreatePureMvcTemplate(templateTitle);
            var template = pageManager.GetTemplates().Where(t => t.Id == templateId).SingleOrDefault();

            var parent = pageManager.GetTemplates().Where(t => t.Title == parentTemplateTitle).FirstOrDefault();

            template.ParentTemplate = parent;

            var master = pageManager.TemplatesLifecycle.Edit(template);
            pageManager.TemplatesLifecycle.Publish(master);
            pageManager.SaveChanges();

            return template;
        }
    }
}
