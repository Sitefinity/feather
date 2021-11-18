using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Mvc.Proxy;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Security.Claims;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// This class contains helpers for working with controller related operations.
    /// </summary>
    public static class ControllerHelper
    {
        /// <summary>
        /// Load controller model
        /// </summary>
        /// <param name="widgetId">The widget id</param>
        /// <param name="culture">The culture</param>
        /// <returns>Controller model</returns>
        public static object LoadControllerModel(Guid widgetId, CultureInfo culture)
        {
            return LoadControllerModel(widgetId, culture, new Guid());
        }

        /// <summary>
        /// Load controller model
        /// </summary>
        /// <param name="widgetId">The widget id</param>
        /// <param name="culture">The culture</param>
        /// <param name="pageId">The page id</param>
        /// <returns>Controller model</returns>
        public static object LoadControllerModel(Guid widgetId, CultureInfo culture, Guid pageId)
        {
            if (widgetId == null || widgetId == Guid.Empty)
            {
                return null;
            }

            var pageManager = PageManager.GetManager();
            ObjectData objectData = null;

            if (pageId != Guid.Empty)
            {
                objectData = GetOverridingControlForPage(widgetId, pageId);
            }

            if (objectData == null)
            {
                objectData = pageManager.GetControls<ObjectData>().SingleOrDefault(p => p.Id == widgetId);
            }

            if (objectData is PageDraftControl && ClaimsManager.IsBackendUser() == false)
            {
                return null;
            }

            object model = null;

            if (objectData != null)
            {
                var mvcProxy = pageManager.LoadControl(objectData, culture) as MvcControllerProxy;
                if (mvcProxy != null)
                {
                    var controller = mvcProxy.Controller;
                    var controllerType = controller.GetType();
                    var modelProperty = controllerType.GetProperty(Model);
                    if (modelProperty != null)
                    {
                        model = modelProperty.GetValue(controller);
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// Get controller widget id
        /// </summary>
        /// <returns>Widget Id</returns>
        public static Guid GetWidgetId(Controller controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }

            var pageManager = PageManager.GetManager();
            var viewBagControlId = controller.ViewData[ControllerKey];
            if (viewBagControlId == null)
                return Guid.Empty;

            var widgetId = Guid.Empty;
            string controlId = (string)viewBagControlId;

            // templates
            if (controller.HttpContext.Items[IsTemplate] != null && (bool)controller.HttpContext.Items[IsTemplate] == true)
            {
                widgetId = GetTemplateWidgetId(pageManager, controller, controlId);
            }
            else
            {
                widgetId = GetPageWidgetId(pageManager, controlId);
            }

            return widgetId;
        }

        private static Guid GetTemplateWidgetId(PageManager pageManager, Controller controller, string controlId)
        {
            // check if action is after save or cancel in template
            if (controller.HttpContext.Items[FormControlId] != null)
                return (Guid)controller.HttpContext.Items[FormControlId];

            var templateId = GetTemplateIdKey(controller.ControllerContext.HttpContext);
            var versionId = GetVersionNumberKey(controller.ControllerContext.HttpContext);

            if (templateId != null && versionId != null)
            {
                var template = pageManager.GetTemplate(new Guid(templateId));
                if (template != null)
                {
                    var versionManager = Telerik.Sitefinity.Versioning.VersionManager.GetManager();
                    TemplateDraft draft = new TemplateDraft();
                    versionManager.GetSpecificVersionByChangeId(draft, new Guid(versionId));

                    var control = GetControl(draft.Controls, controlId);
                    if (control != null)
                        return control.OriginalControlId;
                }
            }

            // check if loaded in template
            var templateData = controller.HttpContext.Items[TemplateDraftProxy] as TemplateDraftProxy;
            if (templateData != null)
            {
                var template = pageManager.GetTemplate(templateData.ParentItemId);
                if (template != null)
                {
                    if (SystemManager.IsDesignMode || SystemManager.IsPreviewMode)
                    {
                        var control = GetControl(template.Drafts.FirstOrDefault(p => p.IsTempDraft).Controls, controlId);
                        if (control != null)
                            return control.Id;
                    }
                    else
                    {
                        var control = GetControl(template.Controls, controlId);
                        if (control != null)
                            return control.Id;
                    }
                }
            }

            return Guid.Empty;
        }

        private static Guid GetPageWidgetId(PageManager pageManager, string controlId)
        {
            // pages
            var pageId = SiteMapBase.GetCurrentNode().PageId;
            var page = pageManager.GetPageData(pageId);

            if (page.Template != null)
            {
                var templateControl = GetControl(page.Template.Controls, controlId);
                if (templateControl != null)
                    return templateControl.Id;
            }

            if (SystemManager.IsDesignMode || SystemManager.IsPreviewMode)
            {
                var pageDraft = page.Drafts.FirstOrDefault(p => p.IsTempDraft);

                // Draft, if page is created page template is null, only draft is available
                if (page.Template == null && pageDraft.TemplateId != Guid.Empty)
                {
                    var template = pageManager.GetTemplate(pageDraft.TemplateId);
                    if (template != null)
                    {
                        var templateControl = GetControl(template.Controls, controlId);
                        if (templateControl != null)
                            return templateControl.Id;
                    }
                }

                var control = GetControl(pageDraft.Controls, controlId);
                if (control != null)
                    return control.Id;
            }
            else
            {
                var control = GetControl(page.Controls, controlId);
                if (control != null)
                    return control.Id;
            }

            return Guid.Empty;
        }

        private static ControlData GetOverridingControlForPage(Guid controlId, Guid pageId)
        {
            var pageManager = PageManager.GetManager();
            ControlData overridingControl = pageManager.GetControls<PageDraftControl>().Where(c => c.Page.Id == pageId && (c.Id == controlId || c.BaseControlId == controlId)).FirstOrDefault();

            if (overridingControl == null)
                overridingControl = pageManager.GetControls<PageControl>().Where(c => c.Page.Id == pageId && (c.Id == controlId || c.BaseControlId == controlId)).FirstOrDefault();

            if (overridingControl == null)
            {
                var pageDraft = pageManager.GetDrafts<PageDraft>().Where(a => a.Id == pageId).FirstOrDefault();
                if (pageDraft != null)
                {
                    var iter = pageManager.GetTemplates().Where(c => c.Id == pageDraft.TemplateId).FirstOrDefault();

                    while (iter != null)
                    {
                        overridingControl = pageManager.GetControls<TemplateControl>().Where(c => c.Page.Id == iter.Id && (c.Id == controlId || c.BaseControlId == controlId)).FirstOrDefault();
                        if (overridingControl != null)
                            break;
                        iter = iter.ParentTemplate;
                    }
                }
            }

            return overridingControl;
        }

        private static ControlData GetControl(IEnumerable<ControlData> controls, string controlId)
        {
            return controls.FirstOrDefault(p => p.Properties.FirstOrDefault(t => t.Name == IDParameter && controlId.EndsWith(t.Value)) != null);
        }

        private static string GetTemplateIdKey(HttpContextBase context)
        {
            var requestContext = context.Items[RouteHandler.RequestContextKey] as RequestContext ?? context.Request.RequestContext;
            if (requestContext.RouteData.Values.ContainsKey(ItemId))
            {
                return requestContext.RouteData.Values[ItemId] as string;
            }
            else
            {
                return null;
            }
        }

        private static string GetVersionNumberKey(HttpContextBase context)
        {
            var requestContext = context.Items[RouteHandler.RequestContextKey] as RequestContext ?? context.Request.RequestContext;
            if (requestContext.RouteData.Values.ContainsKey(VersionNumber))
            {
                return requestContext.RouteData.Values[VersionNumber] as string;
            }
            else
            {
                return null;
            }
        }

        private const string TemplateDraftProxy = "TemplateDraftProxy";
        private const string IsTemplate = "IsTemplate";
        private const string FormControlId = "FormControlId";
        private const string ControllerKey = "sf_cntrl_id";
        private const string VersionNumber = "VersionNumber";
        private const string ItemId = "itemId";
        private const string IDParameter = "ID";
        private const string Model = "Model";
    }
}