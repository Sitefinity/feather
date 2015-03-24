using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Utilities.HtmlParsing;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts
{
    public class MvcMasterPage : MasterPage
    {
        public void ApplyToPage(Page page)
        {
            this.SetMasterInternals(page);
            this.InitializeAsUserControl(page);

            var source = this.LayoutOutput(page.MasterPageFile);
            this.InitializeControls(source);

            page.Controls.Add(this);
        }

        private string LayoutOutput(string path)
        {
            var pathDefinition = new PathDefinition()
            {
                IsWildcard = true,
                VirtualPath = UrlPath.ResolveUrl("~/" + LayoutVirtualFileResolver.ResolverPath),
                ResolverName = typeof(LayoutVirtualFileResolver).FullName
            };

            string result;
            var resolver = new LayoutVirtualFileResolver();
            using (var streamReader = new StreamReader(resolver.Open(pathDefinition, path)))
            {
                result = streamReader.ReadToEnd();
            }

            return result;
        }

        private void SetMasterInternals(Page page)
        {
            typeof(Page).GetField("_master", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(page, this);
            typeof(MasterPage).GetField("_ownerControl", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, page);

            var templateCollection = typeof(Page).GetField("_contentTemplateCollection", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(page);
            typeof(MasterPage).GetField("_contentTemplates", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, templateCollection);
        }

        private void InitializeControls(string source)
        {
            var container = new Stack<Control>();
            container.Push(this);
            HtmlChunk chunk;
            StringBuilder currentLiteralText;

            using (HtmlParser parser = new HtmlParser(source))
            {
                parser.SetChunkHashMode(true);
                parser.KeepRawHTML = true;
                parser.AutoKeepScripts = true;
                parser.AutoKeepComments = true;
                parser.AutoExtractBetweenTagsOnly = false;

                currentLiteralText = new StringBuilder();
                while ((chunk = parser.ParseNext()) != null)
                {
                    if (chunk.Type == HtmlChunkType.OpenTag)
                    {
                        this.HandleOpenTag(container, chunk, currentLiteralText);
                    }
                    else if (chunk.Type == HtmlChunkType.CloseTag)
                    {
                        this.HandleCloseTag(container, chunk, currentLiteralText);
                    }
                    else
                    {
                        currentLiteralText.Append(chunk.Html);
                    }
                }
            }

            this.AddIfNotEmpty(currentLiteralText.ToString(), container.Peek());
        }

        private void HandleOpenTag(Stack<Control> container, HtmlChunk chunk, StringBuilder currentLiteralText)
        {
            if (chunk.TagName.Equals("title", StringComparison.OrdinalIgnoreCase))
            {
                this.AddIfNotEmpty(currentLiteralText.ToString(), container.Peek());
                var title = new HtmlTitle();
                container.Peek().Controls.Add(title);
                container.Push(title);
            }
            else if (chunk.TagName.Equals("head", StringComparison.OrdinalIgnoreCase))
            {
                this.AddIfNotEmpty(currentLiteralText.ToString(), container.Peek());
                currentLiteralText.Clear();
                var head = new HtmlHead();
                container.Peek().Controls.Add(head);
                container.Push(head);
            }
            else if (chunk.TagName.Equals("asp:ContentPlaceHolder", StringComparison.OrdinalIgnoreCase))
            {
                this.AddIfNotEmpty(currentLiteralText.ToString(), container.Peek());
                currentLiteralText.Clear();

                if (chunk.HasAttribute("ID"))
                {
                    var id = chunk.AttributesMap["ID"] as string;
                    var placeHolder = new ContentPlaceHolder() { ID = id };
                    container.Peek().Controls.Add(placeHolder);
                    this.AddContentPlaceHolder(id);
                    this.InstantiateControls(placeHolder);
                }
            }
            else if (chunk.TagName.Equals("form", StringComparison.OrdinalIgnoreCase) && chunk.HasAttribute("runat"))
            {
                this.AddIfNotEmpty(currentLiteralText.ToString(), container.Peek());
                currentLiteralText.Clear();

                var form = new HtmlForm();
                container.Peek().Controls.Add(form);
                container.Push(form);
            }
            else if (chunk.TagName == "%@")
            {
                //// Ignore
            }
            else
            {
                currentLiteralText.Append(chunk.Html);
            }
        }

        private void HandleCloseTag(Stack<Control> container, HtmlChunk chunk, StringBuilder currentLiteralText)
        {
            if (chunk.TagName.Equals("title", StringComparison.OrdinalIgnoreCase))
            {
                if (container.Peek() is HtmlTitle)
                {
                    ((HtmlTitle)container.Pop()).Text = currentLiteralText.ToString();
                }
                else
                {
                    this.AddIfNotEmpty(currentLiteralText.ToString(), container.Pop());
                }

                currentLiteralText.Clear();
            }
            else if (chunk.TagName.Equals("head", StringComparison.OrdinalIgnoreCase))
            {
                this.AddIfNotEmpty(currentLiteralText.ToString(), container.Pop());
                currentLiteralText.Clear();
            }
            else if (chunk.TagName.Equals("form", StringComparison.OrdinalIgnoreCase))
            {
                if (container.Peek() is HtmlForm)
                {
                    this.AddIfNotEmpty(currentLiteralText.ToString(), container.Pop());
                    currentLiteralText.Clear();
                }
                else
                {
                    currentLiteralText.Append(chunk.Html);
                }
            }
            else if (chunk.TagName.Equals("asp:ContentPlaceHolder", StringComparison.OrdinalIgnoreCase))
            {
                //// Ignore
            }
            else
            {
                currentLiteralText.Append(chunk.Html);
            }
        }
        
        private void AddIfNotEmpty(string literal, Control container)
        {
            if (!literal.IsNullOrEmpty())
            {
                var control = new Literal();
                control.Text = literal;
                container.Controls.Add(control);
            }
        }

        private void AddContentPlaceHolder(string control)
        {
            this.ContentPlaceHolders.Add(control);
        }

        private void InstantiateControls(Control placeHolder)
        {
            if (placeHolder == null)
                throw new ArgumentNullException("placeHolder");

            if (this.ContentTemplates != null && this.ContentTemplates.Contains(placeHolder.ID))
            {
                this.InstantiateInContentPlaceHolder(placeHolder, (ITemplate)this.ContentTemplates[placeHolder.ID]);
            }
        }
    }
}
