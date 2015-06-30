using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Frontend.FilesMonitoring.Data;
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
    internal class LayoutFileManager : IFileManager
    {
        #region Properties

        /// <summary>
        /// Gets the default template names.
        /// </summary>
        /// <value>
        /// The default template names.
        /// </value>
        public virtual ICollection<string> DefaultTemplateNames
        {
            get
            {
                return new List<string>() 
                { 
                    LayoutFileManager.BootstrapDefaultTemplateName, 
                    LayoutFileManager.SemanticUIDefaultTemplateName, 
                    LayoutFileManager.FoundationDefaultTemplateName 
                };
            }
        }

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
        public virtual void AttachImageToTemplate(PageTemplate template, PageManager pageManager)
        {
            var templateImage = this.GetTemplateImage(template);
            if (templateImage != null)
            {
                this.AddTemplatePresentation(template, pageManager);
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

        /// <summary>
        /// Creates the template.
        /// </summary>
        /// <param name="packageName">Name of the package.</param>
        /// <param name="fileNameWithoutExtension">The file name without extension.</param>
        private void CreateTemplate(string packageName, string fileNameWithoutExtension)
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

                    if (!pageManager.GetTemplates().Any(pt => string.Compare(pt.Name, fullTemplateName, true) == 0))
                    {
                        var template = pageManager.CreateTemplate();

                        template.Category = this.GetOrCreateTemplateCategoryId(packageName);
                        template.Name = fullTemplateName;
                        template.Title = fileNameWithoutExtension;
                        template.Framework = Pages.Model.PageTemplateFramework.Mvc;
                        template.Theme = ThemeController.NoThemeName;
                        var languageData = pageManager.CreatePublishedInvarianLanguageData();
                        template.LanguageData.Add(languageData);

                        this.AttachImageToTemplate(template, pageManager);

                        pageManager.SaveChanges();

                        var master = pageManager.TemplatesLifecycle.Edit(template);
                        pageManager.TemplatesLifecycle.Publish(master);
                        pageManager.SaveChanges();
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

        private Image GetTemplateImage(PageTemplate template)
        {
            Image image = null;

            if (template != null && !string.IsNullOrEmpty(template.Name))
            {
                // Try get image from library
                var libraryManager = LibrariesManager.GetManager("SystemLibrariesProvider");
                var templateThumbsImageLibrary = libraryManager.GetAlbums().FirstOrDefault(lib => lib.Id == LibrariesModule.DefaultTemplateThumbnailsLibraryId);

                image = templateThumbsImageLibrary.Images().FirstOrDefault(i => i.Title.Equals(template.Name, StringComparison.OrdinalIgnoreCase));
                if (image == null)
                {
                    // Check if image is in the resources and upload it
                    var iconResource = string.Format(LayoutFileManager.PageTemplateIconPathFormat, template.Name);
                    if (Assembly.GetExecutingAssembly().GetManifestResourceNames().Any(mrn => mrn.Equals(iconResource, StringComparison.OrdinalIgnoreCase)))
                    {
                        image = this.UploadTemplateImage(libraryManager, templateThumbsImageLibrary, template.Name, iconResource);
                        libraryManager.SaveChanges();
                    }
                }
            }

            return image;
        }

        private Image UploadTemplateImage(LibrariesManager libraryManager, Album templateThumbsImageLibrary, string templateName, string iconResource)
        {
            var image = libraryManager.CreateImage();
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

                    libraryManager.Upload(image, resourceImageStream, Path.GetExtension(iconResource));
                    libraryManager.Lifecycle.Publish(image);
                }
            }

            return image;
        }

        private void AddTemplatePresentation(PageTemplate template, PageManager pageManager)
        {
            var present = pageManager.CreatePresentationItem<TemplatePresentation>();
            present.DataType = Presentation.ImageUrl;
            present.Name = "icon";
            present.Theme = ThemeController.NoThemeName;
            present.Data = string.Format(LayoutFileManager.PageTemplateIconPathFormat, template.Name);
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

        #region Constants

        /// <summary>
        /// The page template icon path format
        /// </summary>
        public const string PageTemplateIconPathFormat = "Telerik.Sitefinity.Frontend.Resources.PageTemplateImages.{0}.gif";

        private const string BootstrapDefaultTemplateName = "Bootstrap.default";
        private const string SemanticUIDefaultTemplateName = "SemanticUI.default";
        private const string FoundationDefaultTemplateName = "Foundation.default";

        #endregion
    }
}
