using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Telerik.Sitefinity.Clients.JS;
using Telerik.Sitefinity.Configuration;
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
    internal class MasterPageBuilder
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
            var includeFormTag = MasterPageBuilder.IsFormTagRequired();
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
        internal static bool IsFormTagRequired()
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
        internal static PageData GetRequestedPageData()
        {
            var node = MasterPageBuilder.GetRequestedPageNode();

            if (node != null)
            {
                var siteMap = (SiteMapBase)node.Provider;
                var pageManager = PageManager.GetManager(siteMap.PageProviderName);
                var pageData = pageManager.GetPageData(node.PageId);

                return pageData;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets the requested page node.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">This resolver hasn’t been invoked with the proper route handler.</exception>
        private static PageSiteNode GetRequestedPageNode()
        {
            var httpContext = SystemManager.CurrentHttpContext;
            var requestContext = httpContext.Request.RequestContext;
            var node = requestContext.RouteData.DataTokens["SiteMapNode"] as PageSiteNode;

            if (node != null)
                return RouteHelper.GetFirstPageDataNode(node, true);
            else
                return null;
        }

        /// <summary>
        /// Appends the content of the required header.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        /// <exception cref="System.InvalidOperationException">Invalid SiteMap node specified. Either the current group node doesn't have child nodes or the current user does not have rights to view any of the child nodes.</exception>
        private void AppendRequiredHeaderContent(StringBuilder stringBuilder, bool setTitle = true)
        {
            var pageData = MasterPageBuilder.GetRequestedPageData();

            if (pageData != null)
            {
                stringBuilder.Append(this.ResourceRegistrations());
                var robotsTag = this.GetRobotsMetaTag(pageData);

                if (!string.IsNullOrEmpty(robotsTag))
                    stringBuilder.Append("\r\n\t" + robotsTag);

                if (setTitle)
                    stringBuilder.Append("\r\n\t<title>" + pageData.HtmlTitle.ToString() + "\r\n\t</title>");
            }
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
        /// Generates scripts tag needed on the page. Doesn't include any resources for the default themes as opposite to the hybrid mode which always includes the default frontend theme.
        /// </summary>
        /// <returns></returns>
        private string ResourceRegistrations()
        {
            StringBuilder sb = new StringBuilder();

            string appPath = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;

            if (!appPath.EndsWith("/", StringComparison.Ordinal))
                appPath = string.Concat(appPath, "/");

            sb.Append(string.Concat("\t<script type=\"text/javascript\">var sf_appPath='", HttpUtility.HtmlEncode(appPath), "';</script>"));

            return sb.ToString();
        }

        #endregion

        #region Constants

        private const string MasterPageDirective = "<%@ Master Language=\"C#\" AutoEventWireup=\"true\" %>\r\n \r\n";
        private const string RobotsMetaTag = "<meta name=\"robots\" content=\"noindex\" />";

        #endregion
    }
}
