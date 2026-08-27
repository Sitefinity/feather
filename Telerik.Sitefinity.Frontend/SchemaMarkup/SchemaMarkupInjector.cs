using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.SchemaMarkup;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;
using Telerik.Sitefinity.Web.Events;

namespace Telerik.Sitefinity.Frontend.SchemaMarkup
{
    internal class SchemaMarkupInjector
    {
        public static void OnPagePreRenderCompleteEventHandler(IPagePreRenderCompleteEvent evt)
        {
            try
            {
                if (evt != null &&
                    evt.Page != null &&
                    evt.PageSiteNode != null &&
                    !evt.PageSiteNode.IsBackend)
                {
                    var schemaMarkupResolver = ObjectFactory.Resolve<ISchemaMarkupResolver>();
                    var schemaMarkups = new List<string>();

                    IDynamicFieldsContainer detailItem = SystemManager.CurrentHttpContext.Items["detailItem"] as IDynamicFieldsContainer;
                    if (detailItem != null)
                    {
                        var detailDependencies = schemaMarkupResolver.GetCacheDependencyKeys(detailItem);
                        if (detailDependencies != null && detailDependencies.Any())
                        {
                            SystemManager.CurrentHttpContext.AddCacheDependencies(detailDependencies);
                        }

                        var detailMarkups = schemaMarkupResolver.Resolve(detailItem);
                        if (detailMarkups != null && detailMarkups.Any())
                        {
                            schemaMarkups.AddRange(detailMarkups);
                        }
                    }

                    var pageDependencies = schemaMarkupResolver.GetCacheDependencyKeys(evt.PageSiteNode);
                    if (pageDependencies != null && pageDependencies.Any())
                    {
                        SystemManager.CurrentHttpContext.AddCacheDependencies(pageDependencies);
                    }

                    var pageMarkups = schemaMarkupResolver.Resolve(evt.PageSiteNode);
                    if (pageMarkups != null && pageMarkups.Any())
                    {
                        schemaMarkups.AddRange(pageMarkups);
                    }

                    if (schemaMarkups != null && schemaMarkups.Any())
                    {
                        foreach (var schemaMarkup in schemaMarkups.Where(markup => !string.IsNullOrWhiteSpace(markup)))
                        {
                            HtmlGenericControl scriptControl = new HtmlGenericControl("script");
                            scriptControl.Attributes.Add("type", "application/ld+json");
                            scriptControl.InnerHtml = schemaMarkup;
                            evt.Page.Header.Controls.Add(scriptControl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write($"SchemaOrgMarkupInjector: Failed to resolve schema markup with exception: {ex}", ConfigurationPolicy.ErrorLog);
            }
        }
    }
}
