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
        /// <param name="targetTemplate">The template.</param>
        /// <returns></returns>
        public virtual string ProcessLayoutString(string targetTemplate)
        {
            var includeFormTag = this.IsFormTagRequired();
            StringBuilder outPut = new StringBuilder();
            HtmlChunk chunk = null;

            using (HtmlParser parser = new HtmlParser(targetTemplate))
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
        /// <param name="layoutHtmlValue">The layout HTML string.</param>
        /// <returns></returns>
        public virtual string AddMasterPageDirectives(string layoutHtmlValue)
        {
            layoutHtmlValue = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}{1}", MasterPageBuilder.MasterPageDirective, layoutHtmlValue);

            return layoutHtmlValue;
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
            stringBuilder.Append(this.ResourceRegistrations());
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

            return MasterPageBuilder.RobotsMetaTag;
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

            return string.Format(System.Globalization.CultureInfo.InvariantCulture, MasterPageBuilder.KeywordsMetaTag, pageData.Keywords);
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

            return string.Format(System.Globalization.CultureInfo.InvariantCulture, MasterPageBuilder.PageDescriptionMetaTag, pageData.Description);
        }

        /// <summary>
        /// Generates scripts tag needed on the page. Doesn't include any resources for the default themes as opposite to the hybrid mode which always includes the default frontend theme.
        /// </summary>
        /// <returns></returns>
        private string ResourceRegistrations()
        {
            var pageProxy = new PageProxy(string.Empty);
            
            StringBuilder sb = new StringBuilder();

            string appPath = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;

            if (!appPath.EndsWith("/", StringComparison.Ordinal))
                appPath = string.Concat(appPath, "/");

            sb.Append(string.Concat("\t<script type=\"text/javascript\">var sf_appPath='", appPath, "';</script>"));

            // add the scripts for personalization in the page
            sb.Append("\t<script src=\"");
            sb.Append(pageProxy.ClientScript.GetWebResourceUrl(typeof(PageStatisticsJSClient), "Telerik.Sitefinity.Clients.JS.StatsClient.min.js"));
            sb.Append("\" type=\"text/javascript\"></script>");

            return sb.ToString();
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

        #endregion

        #region Constants

        private const string MasterPageDirective = "<%@ Master Language=\"C#\" AutoEventWireup=\"true\" %>\r\n \r\n";
        private const string PageDescriptionMetaTag = "<meta name=\"description\" content=\"{0}\" />";
        private const string RobotsMetaTag = "<meta name=\"robots\" content=\"noindex\" />";
        private const string KeywordsMetaTag = "<meta name=\"keywords\" content=\"{0}\" />"; 

        #endregion
    }
}
