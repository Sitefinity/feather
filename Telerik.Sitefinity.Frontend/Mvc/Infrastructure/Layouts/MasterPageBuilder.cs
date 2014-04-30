using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Telerik.Sitefinity.Clients.JS;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Mvc.Rendering;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Utilities.HtmlParsing;
using Telerik.Sitefinity.Utilities.TypeConverters;
using Telerik.Sitefinity.Web;
using Telerik.Sitefinity.Web.UI;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// This class is appends all additional data to the html of the layout template. This required header elements, scripts, stylesheets, form tags, etc.
    /// </summary>
    public class MasterPageBuilder
    {
        #region Public methods

        /// <summary>
        /// Processes the layout string adding the required attributes to the head tag
        /// and also adding the required form tag.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns></returns>
        public virtual string ProcessLayoutString(string template)
        {
            var includeFormTag = this.IsFormTagRequired();
            StringBuilder outPut = new StringBuilder();
            HtmlChunk chunk = null;

            using (HtmlParser parser = new HtmlParser(template))
            {
                parser.SetChunkHashMode(false);
                parser.AutoExtractBetweenTagsOnly = false;
                parser.CompressWhiteSpaceBeforeTag = false;
                parser.KeepRawHTML = true;
                bool setTitle = true;
                bool modified;
                bool isOpenBodyTag;
                bool isCloseBodyTag;
                bool isClosedHeadTag;

                while ((chunk = parser.ParseNext()) != null)
                {
                    modified = false;
                    isOpenBodyTag = false;
                    isCloseBodyTag = false;
                    isClosedHeadTag = false;

                    if (chunk.Type == HtmlChunkType.OpenTag)
                    {
                        if (chunk.TagName.Equals("head", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!chunk.HasAttribute("runat"))
                            {
                                chunk.SetAttribute("runat", "server");
                                modified = true;
                            }
                        }
                        else if (chunk.TagName.Equals("body", StringComparison.OrdinalIgnoreCase))
                            isOpenBodyTag = true;
                        else if (chunk.TagName.Equals("title", StringComparison.OrdinalIgnoreCase))
                            setTitle = false;

                    }
                    else if (chunk.Type == HtmlChunkType.CloseTag)
                    {
                        if (chunk.TagName.Equals("body", StringComparison.OrdinalIgnoreCase))
                            isCloseBodyTag = true;

                        if (chunk.TagName.Equals("head", StringComparison.OrdinalIgnoreCase))
                            isClosedHeadTag = true;
                    }

                    if (includeFormTag && isCloseBodyTag)
                        outPut.Append("</form>");
                    else if (!includeFormTag && isClosedHeadTag)
                        this.AppendRequiredHeaderContent(outPut, setTitle);

                    if (modified)
                        outPut.Append(chunk.GenerateHtml());
                    else
                        outPut.Append(chunk.Html);

                    if (includeFormTag && isOpenBodyTag)
                        outPut.Append("<form runat='server'>");
                }
            }

            return outPut.ToString();
        }

        /// <summary>
        /// Adds the master page directives.
        /// </summary>
        /// <param name="layoutHtmlString">The layout HTML string.</param>
        /// <returns></returns>
        public virtual string AddMasterPageDirectives(string layoutHtmlString)
        {
            layoutHtmlString = string.Format("{0}{1}", MasterPageBuilder.masterPageDirective, layoutHtmlString);

            return layoutHtmlString;
        }

        #endregion 

        #region Protected methods

        /// <summary>
        /// Determines whether the form tag is required.
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsFormTagRequired()
        {
            bool insertFormTag = true;
            var isBackendRequest = true;
            var currentContext = SystemManager.CurrentHttpContext;

            if (currentContext.Items.Contains("IsBackendRequest"))
                isBackendRequest = (bool)currentContext.Items["IsBackendRequest"];

            if (!isBackendRequest && currentContext.Items.Contains("ServedPageNode") && currentContext.Items["ServedPageNode"] is PageSiteNode)
            {
                var servedPageNode = currentContext.Items["ServedPageNode"] as PageSiteNode;

                if (servedPageNode.Framework == Pages.Model.PageTemplateFramework.Mvc)
                    insertFormTag = false;
            }

            return insertFormTag;
        }

        /// <summary>
        /// Gets the requested page data.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Invalid SiteMap node specified. Either the current group node doesn't have child nodes or the current user does not have rights to view any of the child nodes.</exception>
        protected virtual PageData GetRequestedPageData()
        {
            var node = this.GetRequestedPageNode();

            if (node == null)
                throw new InvalidOperationException("Invalid SiteMap node specified. Either the current group node doesn't have child nodes or the current user does not have rights to view any of the child nodes.");

            var siteMap = (SiteMapBase)node.Provider;
            var pageManager = PageManager.GetManager(siteMap.PageProviderName);
            var pageData = pageManager.GetPageData(node.PageId);

            return pageData;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Appends the content of the required header.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        /// <exception cref="System.InvalidOperationException">Invalid SiteMap node specified. Either the current group node doesn't have child nodes or the current user does not have rights to view any of the child nodes.</exception>
        private void AppendRequiredHeaderContent(StringBuilder stringBuilder, bool setTitle = true)
        {
            var pageData = this.GetRequestedPageData();
            stringBuilder.Append(this.ResourceRegistrations(string.Empty));
            var robotsTag = this.GetRobotsMetaTag(pageData);

            if (!string.IsNullOrEmpty(robotsTag))
                stringBuilder.Append("\r\n\t" + robotsTag);

            if (setTitle)
                stringBuilder.Append("\r\n\t<title>" + pageData.HtmlTitle.ToString() + "\r\n\t</title>");

            var descriptionTag = this.GetDescriptionTag(pageData);

            if (!string.IsNullOrEmpty(descriptionTag))
                stringBuilder.Append("\r\n\t" + descriptionTag);

            var keywordsTag = this.GetKeywordsTag(pageData);

            if (!string.IsNullOrEmpty(keywordsTag))
                stringBuilder.Append("\r\n\t" + keywordsTag);
        }

        /// <summary>
        /// Generates the robots meta tag.
        /// </summary>
        /// <param name="pageData">
        /// The information about the page.
        /// </param>
        /// <returns>
        /// Robots meta tag if page is not crawlable, otherwise empty string.
        /// </returns>
        private string GetRobotsMetaTag(PageData pageData)
        {
            if (pageData == null)
                throw new ArgumentNullException("pageData");

            if (pageData.NavigationNode.Crawlable)
                return null;

            return MasterPageBuilder.robotsMetaTag;
        }

        /// <summary>
        /// Generates the page keywords meta tag from page data
        /// </summary>
        /// <param name="pageData">The information about the page</param>
        /// <returns>Keywords meta tag if page has set keywords, otherwise null</returns>
        private string GetKeywordsTag(PageData pageData)
        {
            if (pageData == null)
                throw new ArgumentNullException("pageData");

            if (string.IsNullOrEmpty(pageData.Keywords))
                return null;

            return string.Format(MasterPageBuilder.keywordsMetaTag, pageData.Keywords);
        }

        /// <summary>
        /// Generates the page description meta tag from page data
        /// </summary>
        /// <param name="pageData">The information about the page.</param>
        /// <returns>Description meta tag if page has set description, otherwise null</returns>
        private string GetDescriptionTag(PageData pageData)
        {
            if (pageData == null)
                throw new ArgumentNullException("pageData");

            if (string.IsNullOrEmpty(pageData.Description))
                return null;

            return string.Format(MasterPageBuilder.pageDescriptionMetaTag, pageData.Description);
        }

        /// <summary>
        /// Generates HTML link tags to include CSS for all stylesheets needed on the page (coming from the theme).
        /// Provides an alternative for MVC where resources are inserted as pure HTML link tags instead of a ResourceLinks control, which is the case in the
        /// WebForms and hybrid modes.
        /// </summary>
        /// <param name="theme">The name of the theme</param>
        /// <returns></returns>
        private string ResourceRegistrations(string theme)
        {
            var pageProxy = new PageProxy(theme);
            var resources = ThemeController.GetGlobalStyles(pageProxy);

            if (resources == null)
                return null;

            StringBuilder sb = new StringBuilder();
            this.AppendStylesheetResourceTag(pageProxy, ref sb, "Telerik.Sitefinity.Resources.Reference", "Telerik.Sitefinity.Resources.Themes.LayoutsBasics.css");

            foreach (var link in resources.Links)
            {
                this.AppendStylesheetResourceTag(pageProxy, ref sb, link.AssemblyInfo, link.Name);
            }

            string appPath = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;

            if (!appPath.EndsWith("/"))
                appPath = string.Concat(appPath, "/");

            sb.Append(String.Concat("\t<script type=\"text/javascript\">var sf_appPath='", appPath, "';</script>"));

            // add the scripts for personalization in the page
            sb.Append("\t<script src=\"");
            sb.Append(pageProxy.ClientScript.GetWebResourceUrl(typeof(PageStatisticsJSClient), "Telerik.Sitefinity.Clients.JS.StatsClient.min.js"));
            sb.Append("\" type=\"text/javascript\"></script>");

            return sb.ToString();
        }

        /// <summary>
        /// Appends the CSS resource tag.
        /// </summary>
        /// <param name="pageProxy">The page proxy.</param>
        /// <param name="sb">The sb.</param>
        /// <param name="resourceAssemblyInfo">The resource assembly information.</param>
        /// <param name="resourceName">Name of the resource.</param>
        private void AppendStylesheetResourceTag(PageProxy pageProxy, ref StringBuilder sb, string resourceAssemblyInfo, string resourceName)
        {
            sb.AppendLine();
            sb.Append("\t<link rel=\"stylesheet\" href=\"");

            if (resourceName.StartsWith("~/"))
                sb.Append(VirtualPathUtility.ToAbsolute(resourceName));
            else
                sb.Append(pageProxy.ClientScript.GetWebResourceUrl(TypeResolutionService.ResolveType(resourceAssemblyInfo), resourceName));

            sb.Append("\" type=\"text/css\" />");
        }

        /// <summary>
        /// Gets the requested page node.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">This resolver hasn’t been invoked with the proper route handler.</exception>
        private PageSiteNode GetRequestedPageNode()
        {
            var httpContext = SystemManager.CurrentHttpContext;
            var requestContext = httpContext.Request.RequestContext;
            var node = (PageSiteNode)requestContext.RouteData.DataTokens["SiteMapNode"];

            if (node == null)
                throw new ArgumentException("This resolver hasn’t been invoked with the proper route handler.");

            return RouteHelper.GetFirstPageDataNode(node, true);
        }
//<<<<<<< HEAD:Telerik.Sitefinity.Frontend/Mvc/Infrastructure/Layouts/LayoutTemplateBuilder.cs
//        }

//        private IList<Func<string, string>> GetPathTransformations(Controller controller)
//        {
//            var packagesManager = new PackagesManager();
//            var currentPackage = packagesManager.GetCurrentPackage();
//            var pathTransformations = new List<Func<string, string>>(1);
//            var baseVirtualPath = MvcIntegrationManager.GetVirtualPathBuilder().GetVirtualPath(this.GetType().Assembly);

//            pathTransformations.Add(path =>
//                {
//                    //{1} is the ControllerName argument in VirtualPathProviderViewEngines
//                    var result = path
//                                    .Replace("{1}", "Layouts")
//                                    .Replace("~/", "~/{0}Mvc/".Arrange(baseVirtualPath));

//                    if (currentPackage.IsNullOrEmpty())
//                        result += "#" + currentPackage + Path.GetExtension(path);

//                    return result;
//                });

//            return pathTransformations;
//        }
//=======
//        } 
//>>>>>>> 7d900388018c61332bf547cb8cdf9ce5812b3377:Telerik.Sitefinity.Frontend/Mvc/Infrastructure/Layouts/MasterPageBuilder.cs

        #endregion

        #region Constants

        private const string masterPageDirective = "<%@ Master Language=\"C#\" AutoEventWireup=\"true\" %>\r\n \r\n";
        private const string pageDescriptionMetaTag = "<meta name=\"description\" content=\"{0}\" />";
        private const string robotsMetaTag = "<meta name=\"robots\" content=\"noindex\" />";
        private const string keywordsMetaTag = "<meta name=\"keywords\" content=\"{0}\" />"; 

        #endregion
    }
}
