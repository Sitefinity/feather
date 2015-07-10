using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Frontend.FilesMonitoring.Data;
using Telerik.Sitefinity.Frontend.GridSystem;
using Telerik.Sitefinity.Libraries.Model;
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Multisite;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.RelatedData.Messages;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Web;
using Telerik.Sitefinity.Web.UI;

namespace Telerik.Sitefinity.Frontend.FilesMonitoring
{
    /// <summary>
    /// This class manages the behavior when a layout file is moved over the application folder structure.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Code that works with the PageManager could be moved away from this class.")]
    internal class LayoutFileManager : IFileManager
    {
        #region Properties

        /// <summary>
        /// Gets the required folder path structure. Only layout files placed inside the specified folder structure will trigger automatic creation of the templates.
        /// </summary>
        /// <value>
        /// The folder path structure.
        /// </value>
        public virtual ICollection<string> FolderPathStructure
        {
            get
            {
                if (this.folderPathStructure == null)
                    this.folderPathStructure = new List<string> { "Mvc", "Views" };

                return this.folderPathStructure;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the or create template category identifier.
        /// </summary>
        /// <param name="templateCategoryName">The template category name.</param>
        /// <param name="createIfNotExist">if set to <c>true</c> [create if not exist].</param>
        /// <returns>The id of the category.</returns>
        public virtual Guid GetOrCreateTemplateCategoryId(string templateCategoryName, bool createIfNotExist = true)
        {
            var taxonomyManager = TaxonomyManager.GetManager();

            var pageTemplatesTaxonomy = taxonomyManager.GetTaxonomy<HierarchicalTaxonomy>(SiteInitializer.PageTemplatesTaxonomyId);
            var templateCategory = pageTemplatesTaxonomy.Taxa.SingleOrDefault(t => t.Name.Equals(templateCategoryName, StringComparison.OrdinalIgnoreCase));

            if (templateCategory == null && createIfNotExist)
            {
                templateCategory = taxonomyManager.CreateTaxon<HierarchicalTaxon>();
                templateCategory.Name = templateCategoryName;
                templateCategory.UrlName = templateCategoryName;
                templateCategory.RenderAsLink = false;
                templateCategory.Title = templateCategoryName;
                templateCategory.Description = string.Format("Represents category for {0} page templates.", templateCategoryName);

                pageTemplatesTaxonomy.Taxa.Add(templateCategory);
                taxonomyManager.SaveChanges();
            }

            return templateCategory.Id;
        }

        /// <summary>
        /// Attaches image to a template (if available).
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="pageManager">The page manager.</param>
        /// <param name="imageName">Name of the image file.</param>
        public virtual void AttachImageToTemplate(PageTemplate template, PageManager pageManager, string imageName)
        {
            var templateImage = this.GetTemplateImage(template, imageName);
            if (templateImage != null)
            {
                this.AddTemplatePresentation(template, pageManager, imageName);
                this.AddTemplateRelatedData(template, templateImage, pageManager);
            }
        }

        #endregion

        #region IFileManager

        /// <summary>
        /// Process the file if such is added to the existing folder.
        /// </summary>
        /// <param name="fileName">The virtual file name.</param>
        /// <param name="filePath">The virtual file path.</param>
        /// <param name="packageName">Name of the package.</param>
        public void FileAdded(string fileName, string filePath, string packageName = "")
        {
            var fileMonitorDataManager = FileMonitorDataManager.GetManager();

            var fileData = fileMonitorDataManager.GetFilesData().Where(file => file.FilePath.Equals(filePath, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            this.CreateTemplateAndFileData(fileName, filePath, packageName, fileMonitorDataManager, fileData);
        }

        /// <summary>
        /// Called on file deletion
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="packageName">Name of the package.</param>
        public void FileDeleted(string filePath, string packageName)
        {
            var fileMonitorDataManager = FileMonitorDataManager.GetManager();

            var fileData = fileMonitorDataManager.GetFilesData().Where(file => file.FilePath.Equals(filePath, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if (fileData != null)
            {
                fileMonitorDataManager.Delete(fileData);
                fileMonitorDataManager.SaveChanges();
            }
        }

        /// <summary>
        /// Reacts on file renaming
        /// </summary>
        /// <param name="newFileName">New name of the file.</param>
        /// <param name="oldFileName">Old name of the file.</param>
        /// <param name="newFilePath"></param>
        /// <param name="oldFilePath"></param>
        /// <param name="packageName">Name of the package.</param>
        public void FileRenamed(string newFileName, string oldFileName, string newFilePath, string oldFilePath, string packageName = "")
        {
            var fileMonitorDataManager = FileMonitorDataManager.GetManager();

            var fileData = fileMonitorDataManager.GetFilesData().Where(file => file.FilePath.Equals(oldFilePath, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if (fileData != null)
                this.CreateTemplateAndFileData(newFileName, newFilePath, packageName, fileMonitorDataManager, fileData);
            else
                this.FileAdded(newFileName, newFilePath, packageName);
        }

        internal void CreateDefaultSemanticUiTemplates()
        {
            var header = new LayoutControlDescription()
            {
                Caption = LayoutFileManager.LayoutCaptionHeader,
                Description = LayoutFileManager.LayoutDescriptionHeader,
                Path = LayoutFileManager.GridTemplatePathSemanticUiContainer
            };

            var content = new LayoutControlDescription()
            {
                Caption = LayoutFileManager.LayoutCaptionContent,
                Description = LayoutFileManager.LayoutDescriptionContent,
                Path = LayoutFileManager.GridTemplatePathSemanticUiContainer
            };

            var footer = new LayoutControlDescription()
            {
                Caption = LayoutFileManager.LayoutCaptionFooter,
                Description = LayoutFileManager.LayoutDescriptionFooter,
                Path = LayoutFileManager.GridTemplatePathSemanticUiContainer
            };

            var twoColumns = new LayoutControlDescription()
            {
                Caption = LayoutFileManager.LayoutCaption2Columns,
                Description = LayoutFileManager.LayoutDescriptionContentColumns,
                Path = LayoutFileManager.GridTemplatePathSemanticUi2Columns
            };

            var threeColumns = new LayoutControlDescription()
            {
                Caption = LayoutFileManager.LayoutCaption3Columns,
                Description = LayoutFileManager.LayoutDescriptionContentColumns,
                Path = LayoutFileManager.GridTemplatePathSemanticUi3Columns
            };

            var fourColumns = new LayoutControlDescription()
            {
                Caption = LayoutFileManager.LayoutCaption4Columns,
                Description = LayoutFileManager.LayoutDescriptionContentColumns,
                Path = LayoutFileManager.GridTemplatePathSemanticUi4Columns
            };

            var leftSidebar = new LayoutControlDescription()
            {
                Caption = LayoutFileManager.LayoutCaptionContentSidebar,
                Description = LayoutFileManager.LayoutDescriptionContentSidebar,
                Path = LayoutFileManager.GridTemplatePathSemanticUiLeftSidebar
            };

            var rightSidebar = new LayoutControlDescription()
            {
                Caption = LayoutFileManager.LayoutCaptionContentSidebar,
                Description = LayoutFileManager.LayoutDescriptionContentSidebar,
                Path = LayoutFileManager.GridTemplatePathSemanticUiRightSidebar
            };

            this.CreateSemanticUiTemplate(LayoutFileManager.TemplateCaption1Column, LayoutFileManager.TemplateImage1Column, new LayoutControlDescription[] { header, content, footer });
            this.CreateSemanticUiTemplate(LayoutFileManager.TemplateCaption2Columns, LayoutFileManager.TemplateImage2Columns, new LayoutControlDescription[] { header, twoColumns, footer });
            this.CreateSemanticUiTemplate(LayoutFileManager.TemplateCaption3Columns, LayoutFileManager.TemplateImage3Columns, new LayoutControlDescription[] { header, threeColumns, footer });
            this.CreateSemanticUiTemplate(LayoutFileManager.TemplateCaption4Columns, LayoutFileManager.TemplateImage4Columns, new LayoutControlDescription[] { header, fourColumns, footer });
            this.CreateSemanticUiTemplate(LayoutFileManager.TemplateCaptionLeftSidebar, LayoutFileManager.TemplateImageLeftSidebar, new LayoutControlDescription[] { header, leftSidebar, footer });
            this.CreateSemanticUiTemplate(LayoutFileManager.TemplateCaptionRightSidebar, LayoutFileManager.TemplateImageRightSidebar, new LayoutControlDescription[] { header, rightSidebar, footer });
        }

        internal void CreateDefaultFoundationTemplates()
        {
            var header = new LayoutControlDescription()
            {
                Caption = LayoutFileManager.LayoutCaptionHeader,
                Description = LayoutFileManager.LayoutDescriptionHeader,
                Path = LayoutFileManager.GridTemplatePathFoundationContainer
            };

            var content = new LayoutControlDescription()
            {
                Caption = LayoutFileManager.LayoutCaptionContent,
                Description = LayoutFileManager.LayoutDescriptionContent,
                Path = LayoutFileManager.GridTemplatePathFoundationContainer
            };

            var footer = new LayoutControlDescription()
            {
                Caption = LayoutFileManager.LayoutCaptionFooter,
                Description = LayoutFileManager.LayoutDescriptionFooter,
                Path = LayoutFileManager.GridTemplatePathFoundationContainer
            };

            var twoColumns = new LayoutControlDescription()
            {
                Caption = LayoutFileManager.LayoutCaption2Columns,
                Description = LayoutFileManager.LayoutDescriptionContentColumns,
                Path = LayoutFileManager.GridTemplatePathFoundation2Columns
            };

            var threeColumns = new LayoutControlDescription()
            {
                Caption = LayoutFileManager.LayoutCaption3Columns,
                Description = LayoutFileManager.LayoutDescriptionContentColumns,
                Path = LayoutFileManager.GridTemplatePathFoundation3Columns
            };

            var fourColumns = new LayoutControlDescription()
            {
                Caption = LayoutFileManager.LayoutCaption4Columns,
                Description = LayoutFileManager.LayoutDescriptionContentColumns,
                Path = LayoutFileManager.GridTemplatePathFoundation4Columns
            };

            var leftSidebar = new LayoutControlDescription()
            {
                Caption = LayoutFileManager.LayoutCaptionContentSidebar,
                Description = LayoutFileManager.LayoutDescriptionContentSidebar,
                Path = LayoutFileManager.GridTemplatePathFoundationLeftSidebar
            };

            var rightSidebar = new LayoutControlDescription()
            {
                Caption = LayoutFileManager.LayoutCaptionContentSidebar,
                Description = LayoutFileManager.LayoutDescriptionContentSidebar,
                Path = LayoutFileManager.GridTemplatePathFoundationRightSidebar
            };

            this.CreateFoundationTemplate(LayoutFileManager.TemplateCaption1Column, LayoutFileManager.TemplateImage1Column, new LayoutControlDescription[] { header, content, footer });
            this.CreateFoundationTemplate(LayoutFileManager.TemplateCaption2Columns, LayoutFileManager.TemplateImage2Columns, new LayoutControlDescription[] { header, twoColumns, footer });
            this.CreateFoundationTemplate(LayoutFileManager.TemplateCaption3Columns, LayoutFileManager.TemplateImage3Columns, new LayoutControlDescription[] { header, threeColumns, footer });
            this.CreateFoundationTemplate(LayoutFileManager.TemplateCaption4Columns, LayoutFileManager.TemplateImage4Columns, new LayoutControlDescription[] { header, fourColumns, footer });
            this.CreateFoundationTemplate(LayoutFileManager.TemplateCaptionLeftSidebar, LayoutFileManager.TemplateImageLeftSidebar, new LayoutControlDescription[] { header, leftSidebar, footer });
            this.CreateFoundationTemplate(LayoutFileManager.TemplateCaptionRightSidebar, LayoutFileManager.TemplateImageRightSidebar, new LayoutControlDescription[] { header, rightSidebar, footer });
        }

        internal void CreateDefaultBootstrapTemplates()
        {
            var header = new LayoutControlDescription() 
                    {
                        Caption = LayoutFileManager.LayoutCaptionHeader,
                        Description = LayoutFileManager.LayoutDescriptionHeader,
                        Path = LayoutFileManager.GridTemplatePathBootstrapContainer
                    };

            var content = new LayoutControlDescription()
                    {
                        Caption = LayoutFileManager.LayoutCaptionContent,
                        Description = LayoutFileManager.LayoutDescriptionContent,
                        Path = LayoutFileManager.GridTemplatePathBootstrapContainer
                    };

            var footer = new LayoutControlDescription()
                    {
                        Caption = LayoutFileManager.LayoutCaptionFooter,
                        Description = LayoutFileManager.LayoutDescriptionFooter,
                        Path = LayoutFileManager.GridTemplatePathBootstrapContainer
                    };

            var twoColumns = new LayoutControlDescription()
                    {
                        Caption = LayoutFileManager.LayoutCaption2Columns,
                        Description = LayoutFileManager.LayoutDescriptionContentColumns,
                        Path = LayoutFileManager.GridTemplatePathBootstrap2Columns
                    };

            var threeColumns = new LayoutControlDescription()
                    {
                        Caption = LayoutFileManager.LayoutCaption3Columns,
                        Description = LayoutFileManager.LayoutDescriptionContentColumns,
                        Path = LayoutFileManager.GridTemplatePathBootstrap3Columns
                    };

            var fourColumns = new LayoutControlDescription()
                    {
                        Caption = LayoutFileManager.LayoutCaption4Columns,
                        Description = LayoutFileManager.LayoutDescriptionContentColumns,
                        Path = LayoutFileManager.GridTemplatePathBootstrap4Columns
                    };

            var leftSidebar = new LayoutControlDescription()
                    {
                        Caption = LayoutFileManager.LayoutCaptionContentSidebar,
                        Description = LayoutFileManager.LayoutDescriptionContentSidebar,
                        Path = LayoutFileManager.GridTemplatePathBootstrapLeftSidebar
                    };

            var rightSidebar = new LayoutControlDescription()
                    {
                        Caption = LayoutFileManager.LayoutCaptionContentSidebar,
                        Description = LayoutFileManager.LayoutDescriptionContentSidebar,
                        Path = LayoutFileManager.GridTemplatePathBootstrapRightSidebar
                    };

            this.CreateBootstrapTemplate(LayoutFileManager.TemplateCaption1Column, LayoutFileManager.TemplateImage1Column, new LayoutControlDescription[] { header, content, footer });
            this.CreateBootstrapTemplate(LayoutFileManager.TemplateCaption2Columns, LayoutFileManager.TemplateImage2Columns, new LayoutControlDescription[] { header, twoColumns, footer });
            this.CreateBootstrapTemplate(LayoutFileManager.TemplateCaption3Columns, LayoutFileManager.TemplateImage3Columns, new LayoutControlDescription[] { header, threeColumns, footer });
            this.CreateBootstrapTemplate(LayoutFileManager.TemplateCaption4Columns, LayoutFileManager.TemplateImage4Columns, new LayoutControlDescription[] { header, fourColumns, footer });
            this.CreateBootstrapTemplate(LayoutFileManager.TemplateCaptionLeftSidebar, LayoutFileManager.TemplateImageLeftSidebar, new LayoutControlDescription[] { header, leftSidebar, footer });
            this.CreateBootstrapTemplate(LayoutFileManager.TemplateCaptionRightSidebar, LayoutFileManager.TemplateImageRightSidebar, new LayoutControlDescription[] { header, rightSidebar, footer });
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Determines whether a file exists on the specified location and whether it is applicable for the current application.
        /// </summary>
        /// <remarks>
        /// Valid locations depends on the values inside the <see cref="FolderPathStructure"/>.
        /// </remarks>
        /// <param name="filePath">The file path.</param>
        /// <param name="packageName">Name of the package.</param>
        /// <returns>true if the file exists and is placed under the application root in a folder [package]/Mvc/Views/Layouts.</returns>
        private bool IsFileInValidFolder(string filePath, string packageName = "")
        {
            bool isFileInValidFolder = false;

            FileInfo info = new FileInfo(filePath);

            if (!info.Exists)
                return false;

            var directory = info.Directory;

            var expectedBaseFolderStructure = string.Join(Path.DirectorySeparatorChar.ToString(), this.FolderPathStructure);
            var expectedLayoutFolderStructure = expectedBaseFolderStructure + Path.DirectorySeparatorChar + ResourceType.Layouts.ToString();

            if (!string.IsNullOrEmpty(packageName))
                expectedLayoutFolderStructure = packageName + Path.DirectorySeparatorChar + expectedLayoutFolderStructure;

            if (directory.FullName.EndsWith(expectedLayoutFolderStructure, StringComparison.OrdinalIgnoreCase) && directory.FullName.StartsWith(HostingEnvironment.ApplicationPhysicalPath, StringComparison.OrdinalIgnoreCase))
                isFileInValidFolder = true;

            return isFileInValidFolder;
        }

        /// <summary>
        /// Creates the template and file data.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="packageName">Name of the package.</param>
        /// <param name="fileMonitorDataManager">The file monitor data manager.</param>
        /// <param name="fileData">The file data.</param>
        private void CreateTemplateAndFileData(string fileName, string filePath, string packageName, FileMonitorDataManager fileMonitorDataManager, FileData fileData)
        {
            var absolutePath = FrontendManager.VirtualPathBuilder.MapPath(filePath);

            if (!this.IsFileInValidFolder(absolutePath, packageName))
                return;

            var extension = fileName.Split('.').LastOrDefault();
            var fileNameWithoutExtension = fileName.Substring(0, fileName.Length - (extension.Length + 1));

            var viewFileExtensions = this.GetViewExtensions();

            if (viewFileExtensions.Contains(extension, StringComparer.Ordinal))
            {
                if (fileData == null)
                    fileData = fileMonitorDataManager.CreateFileData();

                fileData.FilePath = filePath;
                fileData.FileName = fileName;
                fileData.PackageName = packageName;

                fileMonitorDataManager.SaveChanges();

                this.CreateTemplate(packageName, fileNameWithoutExtension);
            }
        }

        /// <summary>
        /// Gets the allowed views extensions.
        /// </summary>
        /// <returns></returns>
        private string[] GetViewExtensions()
        {
            return ViewEngines.Engines.OfType<VirtualPathProviderViewEngine>()
                .SelectMany(p => p.FileExtensions).ToArray();
        }

        private void CreateTemplate(string packageName, string fileNameWithoutExtension)
        {
            this.CreateTemplate(packageName, fileNameWithoutExtension, fileNameWithoutExtension, null, null);
        }

        private void CreateBootstrapTemplate(string title, string image, LayoutControlDescription[] layoutControls)
        {
            this.CreateTemplate("Bootstrap", "default", title, image, layoutControls);
        }

        private void CreateFoundationTemplate(string title, string image, LayoutControlDescription[] layoutControls)
        {
            this.CreateTemplate("Foundation", "default", title, image, layoutControls);
        }

        private void CreateSemanticUiTemplate(string title, string image, LayoutControlDescription[] layoutControls)
        {
            this.CreateTemplate("SemanticUI", "default", title, image, layoutControls);
        }

        private void CreateTemplate(string packageName, string fileNameWithoutExtension, string title, string image, LayoutControlDescription[] layoutControls)
        {
            var multisiteContext = SystemManager.CurrentContext as MultisiteContext;
            var prevSite = SystemManager.CurrentContext.CurrentSite;
            if (multisiteContext != null)
            {
                var defaultSite = multisiteContext.GetSites().Single(s => s.IsDefault);
                multisiteContext.ChangeCurrentSite(defaultSite);
            }

            try
            {
                PageManager pageManager = PageManager.GetManager();
                using (new ElevatedModeRegion(pageManager))
                {
                    var fullTemplateName = string.IsNullOrEmpty(packageName) ? fileNameWithoutExtension : string.Format("{0}.{1}", packageName, fileNameWithoutExtension);

                    if (!pageManager.GetTemplates().Any(pt => (string.Compare(pt.Name, fullTemplateName, true) == 0 && string.Compare(pt.Title, title, true) == 0) || string.Compare(pt.Title, fullTemplateName, true) == 0))
                    {
                        var template = pageManager.CreateTemplate();

                        template.Category = this.GetOrCreateTemplateCategoryId(packageName);
                        template.Name = fullTemplateName;
                        template.Title = title;
                        template.Framework = Pages.Model.PageTemplateFramework.Mvc;
                        template.Theme = ThemeController.NoThemeName;
                        var languageData = pageManager.CreatePublishedInvarianLanguageData();
                        template.LanguageData.Add(languageData);

                        this.AttachImageToTemplate(template, pageManager, image ?? "default");

                        this.AddLayoutControls(pageManager, template, layoutControls);

                        pageManager.SaveChanges();

                        var master = pageManager.TemplatesLifecycle.Edit(template);
                        pageManager.TemplatesLifecycle.Publish(master);
                        pageManager.SaveChanges();

                        if (string.Equals(LayoutFileManager.BootstrapDefaultTemplateName, fullTemplateName, StringComparison.OrdinalIgnoreCase))
                            this.CreateDefaultBootstrapTemplates();
                        else if (string.Equals(LayoutFileManager.FoundationDefaultTemplateName, fullTemplateName, StringComparison.OrdinalIgnoreCase))
                            this.CreateDefaultFoundationTemplates();
                        else if (string.Equals(LayoutFileManager.SemanticUIDefaultTemplateName, fullTemplateName, StringComparison.OrdinalIgnoreCase))
                            this.CreateDefaultSemanticUiTemplates();
                    }
                }
            }
            finally
            {
                if (multisiteContext != null)
                {
                    multisiteContext.ChangeCurrentSite(prevSite);
                }
            }
        }

        private void AddLayoutControls(PageManager pageManager, PageTemplate template, LayoutControlDescription[] layoutControls)
        {
            if (layoutControls == null)
                return;

            var siblingId = Guid.Empty;
            for (int i = 0; i < layoutControls.Length; i++)
            {
                var description = layoutControls[i];
                var layout = new GridControl();
                layout.Layout = description.Path;
                var ctrlData = pageManager.CreateControl<TemplateControl>(isBackendObject: false);
                ctrlData.ObjectType = layout.GetType().FullName;
                ctrlData.PlaceHolder = "Contentplaceholder1";
                ctrlData.SiblingId = siblingId;
                siblingId = ctrlData.Id;
                ctrlData.Caption = description.Caption;
                ctrlData.Description = description.Description;

                pageManager.ReadProperties(layout, ctrlData);
                pageManager.SetControlId(template, ctrlData);

                ctrlData.SetDefaultPermissions(pageManager);
                template.Controls.Add(ctrlData);
            }
        }

        private Image GetTemplateImage(PageTemplate template, string imageName)
        {
            Image image = null;

            if (template != null)
            {
                var libraryManager = LibrariesManager.GetManager("SystemLibrariesProvider");
                if (libraryManager != null)
                {
                    var templateThumbsImageLibrary = libraryManager.GetAlbums().FirstOrDefault(lib => lib.Id == LibrariesModule.DefaultTemplateThumbnailsLibraryId);

                    if (templateThumbsImageLibrary != null)
                    {
                        // Try get image from library
                        image = templateThumbsImageLibrary.Images().FirstOrDefault(i => i.Title.Equals("MVC_" + imageName, StringComparison.OrdinalIgnoreCase));
                        if (image == null)
                        {
                            // Check if image is in the resources and upload it
                            var iconResource = string.Format(LayoutFileManager.PageTemplateIconPathFormat, imageName);
                            if (Assembly.GetExecutingAssembly().GetManifestResourceNames().Any(mrn => mrn.Equals(iconResource, StringComparison.OrdinalIgnoreCase)))
                            {
                                image = this.UploadTemplateImage(libraryManager, templateThumbsImageLibrary, "MVC_" + imageName, iconResource);
                            }
                        }
                    }
                }
            }

            return image;
        }

        private Image UploadTemplateImage(LibrariesManager libraryManager, Album templateThumbsImageLibrary, string templateName, string iconResource)
        {
            Image image = null;

            var suppressSecurityChecks = libraryManager.Provider.SuppressSecurityChecks;
            try
            {
                libraryManager.Provider.SuppressSecurityChecks = true;

                image = libraryManager.CreateImage();
                image.Parent = templateThumbsImageLibrary;
                image.Title = templateName;
                image.UrlName = templateName.ToLower().Replace(' ', '-');
                image.Description = "Description_" + templateName;
                image.AlternativeText = "AltText_" + templateName;
                image.ApprovalWorkflowState = "Published";
                libraryManager.RecompileItemUrls<Image>(image);

                using (var imageStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(iconResource))
                {
                    using (var resourceImage = System.Drawing.Image.FromStream(imageStream))
                    {
                        var resourceImageStream = new MemoryStream();
                        resourceImage.Save(resourceImageStream, System.Drawing.Imaging.ImageFormat.Png);

                        libraryManager.Upload(image, resourceImageStream, ".png");
                        libraryManager.Lifecycle.Publish(image);
                        libraryManager.SaveChanges();
                    }
                }
            }
            finally
            {
                libraryManager.Provider.SuppressSecurityChecks = suppressSecurityChecks;
            }

            return image;
        }

        private void AddTemplatePresentation(PageTemplate template, PageManager pageManager, string imageName)
        {
            var present = pageManager.CreatePresentationItem<TemplatePresentation>();
            present.DataType = Presentation.ImageUrl;
            present.Name = "icon";
            present.Theme = ThemeController.NoThemeName;
            present.Data = string.Format(LayoutFileManager.PageTemplateIconPathFormat, imageName);
            template.Presentation.Add(present);
        }

        private void AddTemplateRelatedData(PageTemplate template, Image image, PageManager pageManager)
        {
            var changedRelations = new ContentLinkChange[] 
            { 
                new ContentLinkChange()
                {
                    ChildItemId = image.Id,
                    ChildItemProviderName = image.GetProviderName(),
                    ChildItemType = image.GetType().FullName,
                    ComponentPropertyName = PageTemplate.ThumbnailFieldName,
                    Ordinal = -2,
                    State = Telerik.Sitefinity.Web.UI.Fields.Enums.ContentLinkChangeState.Added
                }
            };

            var type = Type.GetType("Telerik.Sitefinity.RelatedData.RelatedDataHelper, Telerik.Sitefinity");
            var method = type.GetMethod("SaveRelatedDataChanges", BindingFlags.NonPublic | BindingFlags.Static);
            method.Invoke(null, new object[] { pageManager, template, changedRelations, false });
        }

        #endregion

        #region Private fileds

        private List<string> folderPathStructure;

        #endregion

        #region Private classes

        private class LayoutControlDescription
        {
            [Localizable(false)]
            public string Caption { get; set; }

            [Localizable(false)]
            public string Description { get; set; }

            [Localizable(false)]
            public string Path { get; set; }
        }

        #endregion

        #region Constants

        /// <summary>
        /// The page template icon path format
        /// </summary>
        public const string PageTemplateIconPathFormat = "Telerik.Sitefinity.Frontend.Resources.PageTemplateImages.{0}.gif";

        public const string BootstrapDefaultTemplateName = "Bootstrap.default";
        public const string SemanticUIDefaultTemplateName = "SemanticUI.default";
        public const string FoundationDefaultTemplateName = "Foundation.default";

        public static readonly string[] DefaultTemplateNames = new string[] 
                { 
                    LayoutFileManager.BootstrapDefaultTemplateName, 
                    LayoutFileManager.SemanticUIDefaultTemplateName, 
                    LayoutFileManager.FoundationDefaultTemplateName 
                };

        private const string TemplateCaption1Column = "1 Column, Header, Footer";
        private const string TemplateCaption2Columns = "2 Equal Columns, Header, Footer";
        private const string TemplateCaption3Columns = "3 Equal Columns, Header, Footer";
        private const string TemplateCaption4Columns = "4 Equal Columns, Header, Footer";
        private const string TemplateCaptionLeftSidebar = "Left Sidebar, Header, Footer";
        private const string TemplateCaptionRightSidebar = "Right Sidebar, Header, Footer";

        private const string TemplateImage1Column = "1ColumnHeaderFooter";
        private const string TemplateImage2Columns = "2EqualColumnsHeaderFooter";
        private const string TemplateImage3Columns = "3EqualColumnsHeaderFooter";
        private const string TemplateImage4Columns = "4EqualColumnsHeaderFooter";
        private const string TemplateImageLeftSidebar = "LeftSidebarHeaderFooter";
        private const string TemplateImageRightSidebar = "RightSidebarHeaderFooter";

        private const string LayoutCaptionHeader = "Header";
        private const string LayoutCaptionFooter = "Footer";
        private const string LayoutCaptionContent = "Content";
        private const string LayoutCaptionContentSidebar = "Sidebar + Content";
        private const string LayoutCaption2Columns = "2 Equal Columns";
        private const string LayoutCaption3Columns = "3 Equal Columns";
        private const string LayoutCaption4Columns = "4 Equal Columns";

        private const string LayoutDescriptionHeader = "Represents the header of the template.";
        private const string LayoutDescriptionFooter = "Represents the footer of the template.";
        private const string LayoutDescriptionContent = "Represents the content area of the template.";
        private const string LayoutDescriptionContentSidebar = "Represents the sidebar and content areas of the template.";
        private const string LayoutDescriptionContentColumns = "Represents the content areas of the template.";

        private const string GridTemplatePathBootstrapContainer = "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/container.html";
        private const string GridTemplatePathBootstrap2Columns = "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-6+6.html";
        private const string GridTemplatePathBootstrap3Columns = "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-4+4+4.html";
        private const string GridTemplatePathBootstrap4Columns = "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-3+3+3+3.html";
        private const string GridTemplatePathBootstrapLeftSidebar = "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-3+9.html";
        private const string GridTemplatePathBootstrapRightSidebar = "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-9+3.html";

        private const string GridTemplatePathFoundationContainer = "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-12.html";
        private const string GridTemplatePathFoundation2Columns = "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-6+6.html";
        private const string GridTemplatePathFoundation3Columns = "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-4+4+4.html";
        private const string GridTemplatePathFoundation4Columns = "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-3+3+3+3.html";
        private const string GridTemplatePathFoundationLeftSidebar = "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-3+9.html";
        private const string GridTemplatePathFoundationRightSidebar = "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-9+3.html";

        private const string GridTemplatePathSemanticUiContainer = "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-one-col.html";
        private const string GridTemplatePathSemanticUi2Columns = "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-two-cols.html";
        private const string GridTemplatePathSemanticUi3Columns = "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-three-cols.html";
        private const string GridTemplatePathSemanticUi4Columns = "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-four-cols.html";
        private const string GridTemplatePathSemanticUiLeftSidebar = "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-4+12.html";
        private const string GridTemplatePathSemanticUiRightSidebar = "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-12+4.html";

        #endregion
    }
}
