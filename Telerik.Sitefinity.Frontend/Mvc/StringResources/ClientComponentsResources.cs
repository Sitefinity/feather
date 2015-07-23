using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Localization.Data;

namespace Telerik.Sitefinity.Frontend.Mvc.StringResources
{
    /// <summary>
    /// Localizable strings for the client components.
    /// </summary>
    [ObjectInfo(typeof(ClientComponentsResources), ResourceClassId = "ClientComponentsResources", Title = "ClientComponentsResourcesTitle", Description = "ClientComponentsResourcesDescription")]
    public class ClientComponentsResources : Resource
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientComponentsResources"/> class. 
        /// Initializes new instance of <see cref="ClientComponentsResources"/> class with the default <see cref="ResourceDataProvider"/>.
        /// </summary>
        public ClientComponentsResources()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientComponentsResources"/> class.
        /// </summary>
        /// <param name="dataProvider">The data provider.</param>
        public ClientComponentsResources(ResourceDataProvider dataProvider)
            : base(dataProvider)
        {
        }
        #endregion

        #region Class Description
        /// <summary>
        /// Gets Title for the Client Components resources class.
        /// </summary>
        [ResourceEntry("ClientComponentsResourcesTitle",
            Value = "Client Components resources",
            Description = "Title for the Client Components resources class.",
            LastModified = "2015/03/30")]
        public string ClientComponentsResourcesTitle
        {
            get
            {
                return this["ClientComponentsResourcesTitle"];
            }
        }

        /// <summary>
        /// Gets Description for the Client Components resources class.
        /// </summary>
        [ResourceEntry("ClientComponentsResourcesDescription",
            Value = "Localizable strings for the Client Components.",
            Description = "Description for the Client Components resources class.",
            LastModified = "2015/03/30")]
        public string ClientComponentsResourcesDescription
        {
            get
            {
                return this["ClientComponentsResourcesDescription"];
            }
        }
        #endregion

        /// <summary>
        /// Insert hyperlink
        /// </summary>
        /// <value>Insert hyperlink</value>
        [ResourceEntry("InsertHyperlink",
            Value = "Insert hyperlink",
            Description = "phrase: Insert hyperlink",
            LastModified = "2015/03/21")]
        public string InsertHyperlink
        {
            get
            {
                return this["InsertHyperlink"];
            }
        }

        /// <summary>
        /// Insert an image
        /// </summary>
        /// <value>Insert an image</value>
        [ResourceEntry("InsertImage",
            Value = "Insert an image",
            Description = "phrase: Insert an image",
            LastModified = "2015/03/21")]
        public string InsertImage
        {
            get
            {
                return this["InsertImage"];
            }
        }

        /// <summary>
        /// Insert this image
        /// </summary>
        /// <value>Insert this image</value>
        [ResourceEntry("InsertThisImage",
            Value = "Insert this image",
            Description = "phrase: Insert this image",
            LastModified = "2015/05/05")]
        public string InsertThisImage
        {
            get
            {
                return this["InsertThisImage"];
            }
        }

        /// <summary>
        /// Insert file
        /// </summary>
        /// <value>Insert file</value>
        [ResourceEntry("InsertFile",
            Value = "Insert file",
            Description = "phrase: Insert file",
            LastModified = "2015/03/21")]
        public string InsertFile
        {
            get
            {
                return this["InsertFile"];
            }
        }

        /// <summary>
        /// Insert this file
        /// </summary>
        /// <value>Insert this file</value>
        [ResourceEntry("InsertThisFile",
            Value = "Insert this file",
            Description = "phrase: Insert this file",
            LastModified = "2015/06/26")]
        public string InsertThisFile
        {
            get
            {
                return this["InsertThisFile"];
            }
        }

        /// <summary>
        /// Insert video
        /// </summary>
        /// <value>Insert video</value>
        [ResourceEntry("InsertVideo",
            Value = "Insert video",
            Description = "phrase: Insert video",
            LastModified = "2015/04/06")]
        public string InsertVideo
        {
            get
            {
                return this["InsertVideo"];
            }
        }

        /// <summary>
        /// Insert this video
        /// </summary>
        /// <value>Insert this video</value>
        [ResourceEntry("InsertThisVideo",
            Value = "Insert this video",
            Description = "phrase: Insert this video",
            LastModified = "2015/05/05")]
        public string InsertThisVideo
        {
            get
            {
                return this["InsertThisVideo"];
            }
        }

        /// <summary>
        /// phrase: sfAction attribute is required!
        /// </summary>
        [ResourceEntry("SfActionAttrRequired",
            Value = "sfAction attribute is required!",
            Description = "phrase: sfAction attribute is required!",
            LastModified = "2015/03/23")]
        public string SfActionAttrRequired
        {
            get
            {
                return this["SfActionAttrRequired"];
            }
        }

        /// <summary>
        /// phrase: Insert a document link
        /// </summary>
        /// <value>Insert a document link</value>
        [ResourceEntry("InsertDocumentLink",
            Value = "Insert a document link",
            Description = "phrase: Insert a document link",
            LastModified = "2015/03/23")]
        public string InsertDocumentLink
        {
            get
            {
                return this["InsertDocumentLink"];
            }
        }

        /// <summary>
        /// phrase: Insert this document link
        /// </summary>
        /// <value>Insert this document link</value>
        [ResourceEntry("InsertThisDocumentLink",
            Value = "Insert this document link",
            Description = "phrase: Insert this document link",
            LastModified = "2015/03/23")]
        public string InsertThisDocumentLink
        {
            get
            {
                return this["InsertThisDocumentLink"];
            }
        }

        /// <summary>
        /// word: Title
        /// </summary>
        /// <value>Title</value>
        [ResourceEntry("Title",
            Value = "Title",
            Description = "word: Title",
            LastModified = "2015/03/23")]
        public string Title
        {
            get
            {
                return this["Title"];
            }
        }

        /// <summary>
        /// phrase: Less than 35 characters are recommended
        /// </summary>
        /// <value>Less than 35 characters are recommended</value>
        [ResourceEntry("RecommendedCharacters",
            Value = "Less than 35 characters are recommended",
            Description = "phrase: Less than 35 characters are recommended",
            LastModified = "2015/03/23")]
        public string RecommendedCharacters
        {
            get
            {
                return this["RecommendedCharacters"];
            }
        }

        /// <summary>
        /// phrase: Title is required
        /// </summary>
        /// <value>Title is required</value>
        [ResourceEntry("TitleRequired",
            Value = "Title is required",
            Description = "phrase: Title is required",
            LastModified = "2015/03/23")]
        public string TitleRequired
        {
            get
            {
                return this["TitleRequired"];
            }
        }

        /// <summary>
        /// phrase: CSS classes
        /// </summary>
        /// <value>CSS classes</value>
        [ResourceEntry("CssClasses",
            Value = "CSS classes",
            Description = "phrase: CSS classes",
            LastModified = "2015/03/23")]
        public string CssClasses
        {
            get
            {
                return this["CssClasses"];
            }
        }

        /// <summary>
        /// word: Done
        /// </summary>
        /// <value>Done</value>
        [ResourceEntry("Done",
            Value = "Done",
            Description = "word: Done",
            LastModified = "2015/03/23")]
        public string Done
        {
            get
            {
                return this["Done"];
            }
        }

        /// <summary>
        /// word: Image
        /// </summary>
        /// <value>Image</value>
        [ResourceEntry("Image",
            Value = "Image",
            Description = "word: Image",
            LastModified = "2015/03/23")]
        public string Image
        {
            get
            {
                return this["Image"];
            }
        }

        /// <summary>
        /// phrase: Alternative text
        /// </summary>
        /// <value>Alternative text</value>
        [ResourceEntry("AlternativeText",
            Value = "Alternative text",
            Description = "phrase: Alternative text",
            LastModified = "2015/03/23")]
        public string AlternativeText
        {
            get
            {
                return this["AlternativeText"];
            }
        }

        /// <summary>
        /// word: Alignment
        /// </summary>
        /// <value>Alignment</value>
        [ResourceEntry("Alignment",
            Value = "Alignment",
            Description = "word: Alignment",
            LastModified = "2015/03/23")]
        public string Alignment
        {
            get
            {
                return this["Alignment"];
            }
        }

        /// <summary>
        /// word: None
        /// </summary>
        /// <value>None</value>
        [ResourceEntry("None",
            Value = "None",
            Description = "word: None",
            LastModified = "2015/03/23")]
        public string None
        {
            get
            {
                return this["None"];
            }
        }

        /// <summary>
        /// word: Left
        /// </summary>
        /// <value>Left</value>
        [ResourceEntry("Left",
            Value = "Left",
            Description = "word: Left",
            LastModified = "2015/03/23")]
        public string Left
        {
            get
            {
                return this["Left"];
            }
        }

        /// <summary>
        /// word: Center
        /// </summary>
        /// <value>Center</value>
        [ResourceEntry("Center",
            Value = "Center",
            Description = "word: Center",
            LastModified = "2015/03/23")]
        public string Center
        {
            get
            {
                return this["Center"];
            }
        }

        /// <summary>
        /// word: Right
        /// </summary>
        /// <value>Right</value>
        [ResourceEntry("Right",
            Value = "Right",
            Description = "word: Right",
            LastModified = "2015/03/23")]
        public string Right
        {
            get
            {
                return this["Right"];
            }
        }

        /// <summary>
        /// phrase: Image thumbnail
        /// </summary>
        /// <value>Image thumbnail</value>
        [ResourceEntry("ImageThumbnail",
            Value = "Image thumbnail",
            Description = "phrase: Image thumbnail",
            LastModified = "2015/03/23")]
        public string ImageThumbnail
        {
            get
            {
                return this["ImageThumbnail"];
            }
        }

        /// <summary>
        /// word: Margins
        /// </summary>
        /// <value>Margins</value>
        [ResourceEntry("Margins",
            Value = "Margins",
            Description = "word: Margins",
            LastModified = "2015/03/23")]
        public string Margins
        {
            get
            {
                return this["Margins"];
            }
        }

        /// <summary>
        /// word: Top
        /// </summary>
        /// <value>Top</value>
        [ResourceEntry("Top",
            Value = "Top",
            Description = "word: Top",
            LastModified = "2015/03/23")]
        public string Top
        {
            get
            {
                return this["Top"];
            }
        }

        /// <summary>
        /// word: Bottom
        /// </summary>
        /// <value>Bottom</value>
        [ResourceEntry("Bottom",
            Value = "Bottom",
            Description = "word: Bottom",
            LastModified = "2015/03/23")]
        public string Bottom
        {
            get
            {
                return this["Bottom"];
            }
        }

        /// <summary>
        /// word: Type
        /// </summary>
        [ResourceEntry("Type",
            Value = "Type:",
            Description = "word: Type",
            LastModified = "2015/03/23")]
        public string Type
        {
            get
            {
                return this["Type"];
            }
        }

        /// <summary>
        /// phrase: File size:
        /// </summary>
        /// <value>File size:</value>
        [ResourceEntry("FileSize",
            Value = "File size:",
            Description = "phrase: File size",
            LastModified = "2015/03/23")]
        public string FileSize
        {
            get
            {
                return this["FileSize"];
            }
        }

        /// <summary>
        /// word: Uploaded:
        /// </summary>
        /// <value>Uploaded:</value>
        [ResourceEntry("Uploaded",
            Value = "Uploaded:",
            Description = "word: Uploaded",
            LastModified = "2015/03/23")]
        public string Uploaded
        {
            get
            {
                return this["Uploaded"];
            }
        }

        /// <summary>
        /// phrase: Change image
        /// </summary>
        /// <value>Change image</value>
        [ResourceEntry("ChangeImage",
            Value = "Change image",
            Description = "phrase: Change image",
            LastModified = "2015/03/23")]
        public string ChangeImage
        {
            get
            {
                return this["ChangeImage"];
            }
        }

        /// <summary>
        /// phrase: Edit all properties
        /// </summary>
        /// <value>Edit all properties</value>
        [ResourceEntry("EditAllProperties",
            Value = "Edit all properties",
            Description = "phrase: Edit all properties",
            LastModified = "2015/03/23")]
        public string EditAllProperties
        {
            get
            {
                return this["EditAllProperties"];
            }
        }

        /// <summary>
        /// phrase: Select image
        /// </summary>
        /// <value>Select image</value>
        [ResourceEntry("SelectImage",
            Value = "Select image",
            Description = "phrase: Select image",
            LastModified = "2015/03/23")]
        public string SelectImage
        {
            get
            {
                return this["SelectImage"];
            }
        }

        /// <summary>
        /// phrase: Change document
        /// </summary>
        /// <value>Change document</value>
        [ResourceEntry("ChangeDocument",
            Value = "Change document",
            Description = "phrase: Change document",
            LastModified = "2015/03/23")]
        public string ChangeDocument
        {
            get
            {
                return this["ChangeDocument"];
            }
        }

        /// <summary>
        /// phrase: Select document
        /// </summary>
        /// <value>Select document</value>
        [ResourceEntry("SelectDocument",
            Value = "Select document",
            Description = "phrase: Select document",
            LastModified = "2015/03/23")]
        public string SelectDocument
        {
            get
            {
                return this["SelectDocument"];
            }
        }

        /// <summary>
        /// phrase: The document has been deleted or unpublished
        /// </summary>
        /// <value>The document has been deleted or unpublished</value>
        [ResourceEntry("DocumentHasBeenDeleted",
            Value = "The document has been deleted or unpublished",
            Description = "phrase: The document has been deleted or unpublished",
            LastModified = "2015/03/23")]
        public string DocumentHasBeenDeleted
        {
            get
            {
                return this["DocumentHasBeenDeleted"];
            }
        }

        /// <summary>
        /// phrase: Created on:
        /// </summary>
        /// <value>Created on:</value>
        [ResourceEntry("CreatedOn",
            Value = "Created on:",
            Description = "phrase: Created on:",
            LastModified = "2015/03/23")]
        public string CreatedOn
        {
            get
            {
                return this["CreatedOn"];
            }
        }

        /// <summary>
        /// word: Library:
        /// </summary>
        /// <value>Library:</value>
        [ResourceEntry("Library",
            Value = "Library:",
            Description = "word: Library:",
            LastModified = "2015/03/23")]
        public string Library
        {
            get
            {
                return this["Library"];
            }
        }

        /// <summary>
        /// word: Size:
        /// </summary>
        /// <value>Size:</value>
        [ResourceEntry("Size",
            Value = "Size:",
            Description = "word: Size:",
            LastModified = "2015/03/23")]
        public string Size
        {
            get
            {
                return this["Size"];
            }
        }

        /// <summary>
        /// word: by
        /// </summary>
        /// <value>by</value>
        [ResourceEntry("By",
            Value = "by",
            Description = "word: by",
            LastModified = "2015/03/23")]
        public string By
        {
            get
            {
                return this["By"];
            }
        }

        /// <summary>
        /// phrase: Select document from your computer
        /// </summary>
        /// <value>Select document from your computer</value>
        [ResourceEntry("SelectDocumentFromComputer",
            Value = "Select document from your computer",
            Description = "phrase: Select document from your computer",
            LastModified = "2015/03/23")]
        public string SelectDocumentFromComputer
        {
            get
            {
                return this["SelectDocumentFromComputer"];
            }
        }

        /// <summary>
        /// phrase: or simply drag & drop it here
        /// </summary>
        /// <value>or simply drag & drop it here</value>
        [ResourceEntry("SimplyDragAndDrop",
            Value = "or simply drag & drop it here",
            Description = "phrase: or simply drag & drop it here",
            LastModified = "2015/03/23")]
        public string SimplyDragAndDrop
        {
            get
            {
                return this["SimplyDragAndDrop"];
            }
        }

        /// <summary>
        /// phrase: Search by title...
        /// </summary>
        /// <value>Search by title...</value>
        [ResourceEntry("SearchByTitle",
            Value = "Search by title...",
            Description = "phrase: Search by title...",
            LastModified = "2015/03/23")]
        public string SearchByTitle
        {
            get
            {
                return this["SearchByTitle"];
            }
        }

        /// <summary>
        /// phrase: All libraries
        /// </summary>
        /// <value>All libraries</value>
        [ResourceEntry("AllLibraries",
            Value = "All libraries",
            Description = "phrase: All libraries",
            LastModified = "2015/03/23")]
        public string AllLibraries
        {
            get
            {
                return this["AllLibraries"];
            }
        }

        /// <summary>
        /// phrase: No documents
        /// </summary>
        /// <value>No documents</value>
        [ResourceEntry("NoDocuments",
            Value = "No documents",
            Description = "phrase: No documents",
            LastModified = "2015/03/23")]
        public string NoDocuments
        {
            get
            {
                return this["NoDocuments"];
            }
        }

        /// <summary>
        /// phrase: Sorting and view
        /// </summary>
        /// <value>Sorting and view</value>
        [ResourceEntry("SortingAndView",
            Value = "Sorting and view",
            Description = "phrase: Sorting and view",
            LastModified = "2015/03/23")]
        public string SortingAndView
        {
            get
            {
                return this["SortingAndView"];
            }
        }

        /// <summary>
        /// phrase: Upload document
        /// </summary>
        /// <value>Upload document</value>
        [ResourceEntry("UploadDocument",
            Value = "Upload document",
            Description = "phrase: Upload document",
            LastModified = "2015/03/23")]
        public string UploadDocument
        {
            get
            {
                return this["UploadDocument"];
            }
        }

        /// <summary>
        /// phrase: Already uploaded documents
        /// </summary>
        /// <value>Already uploaded documents</value>
        [ResourceEntry("AlreadyUploadedDocuments",
            Value = "Already uploaded documents",
            Description = "phrase: Already uploaded documents",
            LastModified = "2015/03/23")]
        public string AlreadyUploadedDocuments
        {
            get
            {
                return this["AlreadyUploadedDocuments"];
            }
        }

        /// <summary>
        /// word: Libraries
        /// </summary>
        /// <value>Libraries</value>
        [ResourceEntry("Libraries",
            Value = "Libraries",
            Description = "word: Libraries",
            LastModified = "2015/03/23")]
        public string Libraries
        {
            get
            {
                return this["Libraries"];
            }
        }

        /// <summary>
        /// word: Tags
        /// </summary>
        /// <value>Tags</value>
        [ResourceEntry("Tags",
            Value = "Tags",
            Description = "word: Tags",
            LastModified = "2015/03/23")]
        public string Tags
        {
            get
            {
                return this["Tags"];
            }
        }

        /// <summary>
        /// word: Categories
        /// </summary>
        /// <value>Categories</value>
        [ResourceEntry("Categories",
            Value = "Categories",
            Description = "word: Categories",
            LastModified = "2015/03/23")]
        public string Categories
        {
            get
            {
                return this["Categories"];
            }
        }

        /// <summary>
        /// word: Dates
        /// </summary>
        /// <value>Dates</value>
        [ResourceEntry("Dates",
            Value = "Dates",
            Description = "word: Dates",
            LastModified = "2015/03/23")]
        public string Dates
        {
            get
            {
                return this["Dates"];
            }
        }

        /// <summary>
        /// phrase: Other filter options
        /// </summary>
        /// <value>Other filter options</value>
        [ResourceEntry("OtherFilterOptions",
            Value = "Other filter options",
            Description = "phrase: Other filter options",
            LastModified = "2015/03/23")]
        public string OtherFilterOptions
        {
            get
            {
                return this["OtherFilterOptions"];
            }
        }

        /// <summary>
        /// phrase: Narrow by typing...
        /// </summary>
        /// <value>Narrow by typing...</value>
        [ResourceEntry("NarrowByTyping",
            Value = "Narrow by typing...",
            Description = "phrase: Narrow by typing...",
            LastModified = "2015/03/23")]
        public string NarrowByTyping
        {
            get
            {
                return this["NarrowByTyping"];
            }
        }

        /// <summary>
        /// phrase: Document to upload
        /// </summary>
        /// <value>Document to upload</value>
        [ResourceEntry("DocumentToUpload",
            Value = "Document to upload",
            Description = "phrase: Document to upload",
            LastModified = "2015/03/23")]
        public string DocumentToUpload
        {
            get
            {
                return this["DocumentToUpload"];
            }
        }

        /// <summary>
        /// phrase: Where to store the uploaded document?
        /// </summary>
        /// <value>Where to store the uploaded document?</value>
        [ResourceEntry("WhereToStoreDocument",
            Value = "Where to store the uploaded document?",
            Description = "phrase: Where to store the uploaded document?",
            LastModified = "2015/03/23")]
        public string WhereToStoreDocument
        {
            get
            {
                return this["WhereToStoreDocument"];
            }
        }

        /// <summary>
        /// No items have been created yet.
        /// phrase: No items have been created yet.
        /// </summary>
        /// <value>No items have been created yet.</value>
        [ResourceEntry("NoItemsCreated",
            Value = "No items have been created yet.",
            Description = "phrase: No items have been created yet.",
            LastModified = "2015/03/21")]
        public string NoItemsCreated
        {
            get
            {
                return this["NoItemsCreated"];
            }
        }

        /// <summary>
        /// phrase: No items found
        /// </summary>
        /// <value>No items found</value>
        [ResourceEntry("NoItemsFound",
            Value = "No items found",
            Description = "phrase: No items found",
            LastModified = "2015/03/21")]
        public string NoItemsFound
        {
            get
            {
                return this["NoItemsFound"];
            }
        }

        /// <summary>
        /// word: All
        /// </summary>
        /// <value>All</value>
        [ResourceEntry("All",
            Value = " All",
            Description = "word: All",
            LastModified = "2015/03/21")]
        public string All
        {
            get
            {
                return this["All"];
            }
        }

        /// <summary>
        /// word: Selected
        /// </summary>
        /// <value>Selected</value>
        [ResourceEntry("Selected",
            Value = "Selected",
            Description = "word: Selected",
            LastModified = "2015/03/21")]
        public string Selected
        {
            get
            {
                return this["Selected"];
            }
        }

        /// <summary>
        /// phrase: Done selecting
        /// </summary>
        /// <value>Done selecting</value>
        [ResourceEntry("DoneSelecting",
            Value = "Done selecting",
            Description = "phrase: Done selecting",
            LastModified = "2015/03/21")]
        public string DoneSelecting
        {
            get
            {
                return this["DoneSelecting"];
            }
        }

        /// <summary>
        /// word: Cancel
        /// </summary>
        /// <value>Cancel</value>
        [ResourceEntry("Cancel",
            Value = "Cancel",
            Description = "word: Cancel",
            LastModified = "2015/03/22")]
        public string Cancel
        {
            get
            {
                return this["Cancel"];
            }
        }

        /// <summary>
        /// word: Close
        /// </summary>
        /// <value>Close</value>
        [ResourceEntry("Close",
            Value = "Close",
            Description = "word: Close",
            LastModified = "2015/03/21")]
        public string Close
        {
            get
            {
                return this["Close"];
            }
        }

        /// <summary>
        /// phrase: No items have been selected yet.
        /// </summary>
        /// <value>No items have been selected yet.</value>
        [ResourceEntry("NoItemsSelected",
            Value = "No items have been selected yet.",
            Description = "phrase: No items have been selected yet.",
            LastModified = "2015/03/21")]
        public string NoItemsSelected
        {
            get
            {
                return this["NoItemsSelected"];
            }
        }

        /// <summary>
        /// phrase: not translated
        /// </summary>
        /// <value>not translated</value>
        [ResourceEntry("NotTranslated",
            Value = "not translated",
            Description = "phrase: not translated",
            LastModified = "2015/03/23")]
        public string NotTranslated
        {
            get
            {
                return this["NotTranslated"];
            }
        }

        /// <summary>
        /// phrase: Date...
        /// </summary>
        /// <value>Date... 1</value>
        [ResourceEntry("Date",
            Value = "Date...",
            Description = "phrase: Date...",
            LastModified = "2015/03/23")]
        public string Date
        {
            get
            {
                return this["Date"];
            }
        }

        /// <summary>
        /// phrase: Add hour
        /// </summary>
        /// <value>Add hour</value>
        [ResourceEntry("AddHour",
            Value = "Add hour",
            Description = "phrase: Add hour",
            LastModified = "2015/03/21")]
        public string AddHour
        {
            get
            {
                return this["AddHour"];
            }
        }

        /// <summary>
        /// phrase: Add minutes
        /// </summary>
        /// <value>Add minutes</value>
        [ResourceEntry("AddMinutes",
            Value = "Add minutes",
            Description = "phrase: Add minutes",
            LastModified = "2015/03/21")]
        public string AddMinutes
        {
            get
            {
                return this["AddMinutes"];
            }
        }

        /// <summary>
        /// word: Select
        /// </summary>
        /// <value>Select</value>
        [ResourceEntry("Select",
            Value = "Select",
            Description = "word: Select",
            LastModified = "2015/03/24")]
        public string Select
        {
            get
            {
                return this["Select"];
            }
        }

        /// <summary>
        /// word: Change
        /// </summary>
        /// <value>Change</value>
        [ResourceEntry("Change",
            Value = "Change",
            Description = "word: Change",
            LastModified = "2015/03/21")]
        public string Change
        {
            get
            {
                return this["Change"];
            }
        }

        /// <summary>
        /// phrase: Select dates
        /// </summary>
        /// <value>Select dates</value>
        [ResourceEntry("SelectDates",
            Value = "Select dates",
            Description = "phrase: Select dates",
            LastModified = "2015/03/21")]
        public string SelectDates
        {
            get
            {
                return this["SelectDates"];
            }
        }

        /// <summary>
        /// phrase: Display items published in...
        /// </summary>
        /// <value>Display items published in...</value>
        [ResourceEntry("DisplayPublishedItems",
            Value = "Display items published in...",
            Description = "phrase: Display items published in...",
            LastModified = "2015/03/21")]
        public string DisplayPublishedItems
        {
            get
            {
                return this["DisplayPublishedItems"];
            }
        }

        /// <summary>
        /// phrase: Any time
        /// </summary>
        /// <value>Any time</value>
        [ResourceEntry("AnyTime",
            Value = "Any time",
            Description = "phrase: Any time",
            LastModified = "2015/03/21")]
        public string AnyTime
        {
            get
            {
                return this["AnyTime"];
            }
        }

        /// <summary>
        /// word: Last
        /// </summary>
        /// <value>Last</value>
        [ResourceEntry("Last",
            Value = "Last",
            Description = "word: Last",
            LastModified = "2015/03/21")]
        public string Last
        {
            get
            {
                return this["Last"];
            }
        }

        /// <summary>
        /// word: day
        /// </summary>
        /// <value>day</value>
        [ResourceEntry("Day",
            Value = "day",
            Description = "word: day",
            LastModified = "2015/03/22")]
        public string Day
        {
            get
            {
                return this["Day"];
            }
        }

        /// <summary>
        /// word: week
        /// </summary>
        /// <value>week</value>
        [ResourceEntry("Week",
            Value = "week",
            Description = "word: week",
            LastModified = "2015/03/22")]
        public string Week
        {
            get
            {
                return this["Week"];
            }
        }

        /// <summary>
        /// word: month
        /// </summary>
        /// <value>month</value>
        [ResourceEntry("Month",
            Value = "month",
            Description = "word: month",
            LastModified = "2015/03/22")]
        public string Month
        {
            get
            {
                return this["Month"];
            }
        }

        /// <summary>
        /// word: year
        /// </summary>
        /// <value>year</value>
        [ResourceEntry("Year",
            Value = "year",
            Description = "word: year",
            LastModified = "2015/03/22")]
        public string Year
        {
            get
            {
                return this["Year"];
            }
        }

        /// <summary>
        /// (s)
        /// </summary>
        /// <value>(s)</value>
        [ResourceEntry("Plural",
            Value = "(s)",
            Description = "(s)",
            LastModified = "2015/03/22")]
        public string Plural
        {
            get
            {
                return this["Plural"];
            }
        }

        /// <summary>
        /// phrase: Custom range...
        /// </summary>
        /// <value>Custom range...</value>
        [ResourceEntry("CustomRange",
            Value = "Custom range...",
            Description = "phrase: Custom range...",
            LastModified = "2015/03/22")]
        public string CustomRange
        {
            get
            {
                return this["CustomRange"];
            }
        }

        /// <summary>
        /// word: From
        /// </summary>
        /// <value>From</value>
        [ResourceEntry("From",
            Value = "From",
            Description = "word: From",
            LastModified = "2015/03/22")]
        public string From
        {
            get
            {
                return this["From"];
            }
        }

        /// <summary>
        /// word: To
        /// </summary>
        /// <value>To</value>
        [ResourceEntry("To",
            Value = "To",
            Description = "word: To",
            LastModified = "2015/03/23")]
        public string To
        {
            get
            {
                return this["To"];
            }
        }

        /// <summary>
        /// word: Language
        /// </summary>
        /// <value>Language</value>
        [ResourceEntry("Language",
            Value = "Language",
            Description = "word: Language",
            LastModified = "2015/03/23")]
        public string Language
        {
            get
            {
                return this["Language"];
            }
        }

        /// <summary>
        /// word: Site
        /// </summary>
        /// <value>Site</value>
        [ResourceEntry("Site",
            Value = "Site",
            Description = "word: Site",
            LastModified = "2015/03/23")]
        public string Site
        {
            get
            {
                return this["Site"];
            }
        }

        /// <summary>
        /// phrase: External URLs
        /// </summary>
        /// <value>External URLs</value>
        [ResourceEntry("ExternalUrls",
            Value = "External URLs",
            Description = "phrase: External URLs",
            LastModified = "2015/03/23")]
        public string ExternalUrls
        {
            get
            {
                return this["ExternalUrls"];
            }
        }

        /// <summary>
        /// phrase: Insert link
        /// </summary>
        /// <value>Insert link</value>
        [ResourceEntry("InsertLinkButton",
            Value = "Insert this link",
            Description = "phrase: Insert this link",
            LastModified = "2015/06/12")]
        public string InsertLinkButton
        {
            get
            {
                return this["InsertLinkButton"];
            }
        }

        /// <summary>
        /// phrase: Insert a link
        /// </summary>
        /// <value>Insert a link</value>
        [ResourceEntry("InsertLinkHeader",
            Value = "Insert a link",
            Description = "phrase: Insert a link",
            LastModified = "2015/03/23")]
        public string InsertLinkHeader
        {
            get
            {
                return this["InsertLinkHeader"];
            }
        }

        /// <summary>
        /// phrase: Text to display
        /// </summary>
        /// <value>Text to display</value>
        [ResourceEntry("TextToDisplay",
            Value = "Text to display",
            Description = "phrase: Text to display",
            LastModified = "2015/03/23")]
        public string TextToDisplay
        {
            get
            {
                return this["TextToDisplay"];
            }
        }

        /// <summary>
        /// phrase: Link to
        /// </summary>
        /// <value>Link to</value>
        [ResourceEntry("LinkTo",
            Value = "Link to",
            Description = "phrase: Link to",
            LastModified = "2015/03/23")]
        public string LinkTo
        {
            get
            {
                return this["LinkTo"];
            }
        }

        /// <summary>
        /// phrase: Web address
        /// </summary>
        /// <value>Web address</value>
        [ResourceEntry("WebAddress",
            Value = "Web address",
            Description = "phrase: Web address",
            LastModified = "2015/03/23")]
        public string WebAddress
        {
            get
            {
                return this["WebAddress"];
            }
        }

        /// <summary>
        /// phrase: Page from this site
        /// </summary>
        /// <value>Page from this site</value>
        [ResourceEntry("PageFromThisSite",
            Value = "Page from this site",
            Description = "phrase: Page from this site",
            LastModified = "2015/03/23")]
        public string PageFromThisSite
        {
            get
            {
                return this["PageFromThisSite"];
            }
        }

        /// <summary>
        /// word: Anchor
        /// </summary>
        /// <value>Anchor</value>
        [ResourceEntry("Anchor",
            Value = "Anchor",
            Description = "word: Anchor",
            LastModified = "2015/03/23")]
        public string Anchor
        {
            get
            {
                return this["Anchor"];
            }
        }

        /// <summary>
        /// word: Email
        /// </summary>
        /// <value>Email</value>
        [ResourceEntry("Email",
            Value = "Email",
            Description = "word: Email",
            LastModified = "2015/03/23")]
        public string Email
        {
            get
            {
                return this["Email"];
            }
        }

        /// <summary>
        /// phrase: Weather forecast
        /// </summary>
        /// <value>Weather forecast</value>
        [ResourceEntry("WebAddressTextToDisplayExample",
            Value = "Weather forecast",
            Description = "phrase: Weather forecast",
            LastModified = "2015/03/24")]
        public string WebAddressTextToDisplayExample
        {
            get
            {
                return this["WebAddressTextToDisplayExample"];
            }
        }

        /// <summary>
        /// phrase: Open this link in a new window
        /// </summary>
        /// <value>Open this link in a new window</value>
        [ResourceEntry("OpenInNewWindow",
            Value = "Open this link in a new window",
            Description = "phrase: Open this link in a new window",
            LastModified = "2015/03/23")]
        public string OpenInNewWindow
        {
            get
            {
                return this["OpenInNewWindow"];
            }
        }

        /// <summary>
        /// phrase: Test this link:
        /// </summary>
        /// <value>Test this link:</value>
        [ResourceEntry("TestThisLink",
            Value = "Test this link:",
            Description = "phrase: Test this link:",
            LastModified = "2015/03/23")]
        public string TestThisLink
        {
            get
            {
                return this["TestThisLink"];
            }
        }

        /// <summary>
        /// phrase: - Select -
        /// </summary>
        /// <value>- Select -</value>
        [ResourceEntry("SelectOption",
            Value = "- Select -",
            Description = "phrase: - Select -",
            LastModified = "2015/03/23")]
        public string SelectOption
        {
            get
            {
                return this["SelectOption"];
            }
        }

        /// <summary>
        /// phrase: Example:
        /// </summary>
        /// <value>Example:</value>
        [ResourceEntry("Example",
            Value = "Example:",
            Description = "phrase: Example:",
            LastModified = "2015/03/24")]
        public string Example
        {
            get
            {
                return this["Example"];
            }
        }

        /// <summary>
        /// phrase: A list of anchors already inserted in the text.
        /// </summary>
        /// <value>A list of anchors already inserted in the text.</value>
        [ResourceEntry("AnchorDescription",
            Value = "A list of anchors already inserted in the text.",
            Description = "phrase: A list of anchors already inserted in the text.",
            LastModified = "2015/03/24")]
        public string AnchorDescription
        {
            get
            {
                return this["AnchorDescription"];
            }
        }

        /// <summary>
        /// phrase: How to insert an anchor?
        /// </summary>
        /// <value>How to insert an anchor?</value>
        [ResourceEntry("HowToInsertAnchor",
            Value = "How to insert an anchor?",
            Description = "phrase: How to insert an anchor?",
            LastModified = "2015/03/24")]
        public string HowToInsertAnchor
        {
            get
            {
                return this["HowToInsertAnchor"];
            }
        }

        /// <summary>
        /// phrase: FAQ list
        /// </summary>
        /// <value>FAQ list</value>
        [ResourceEntry("AnchorTextToDisplayExample",
            Value = "FAQ list",
            Description = "phrase: FAQ list",
            LastModified = "2015/03/24")]
        public string AnchorTextToDisplayExample
        {
            get
            {
                return this["AnchorTextToDisplayExample"];
            }
        }

        /// <summary>
        /// phrase: No anchors have been inserted in the text.
        /// </summary>
        /// <value>No anchors have been inserted in the text.</value>
        [ResourceEntry("NoAnchorsSelected",
            Value = "No anchors have been inserted in the text.",
            Description = "phrase: No anchors have been inserted in the text.",
            LastModified = "2015/03/24")]
        public string NoAnchorsSelected
        {
            get
            {
                return this["NoAnchorsSelected"];
            }
        }

        /// <summary>
        /// phrase: Email address
        /// </summary>
        /// <value>Email address</value>
        [ResourceEntry("EmailAddress",
            Value = "Email address",
            Description = "phrase: Email address",
            LastModified = "2015/03/24")]
        public string EmailAddress
        {
            get
            {
                return this["EmailAddress"];
            }
        }

        /// <summary>
        /// phrase: You have entered an invalid email address
        /// </summary>
        /// <value>You have entered an invalid email address</value>
        [ResourceEntry("InvalidEmailMessage",
            Value = "You have entered an invalid email address",
            Description = "phrase: You have entered an invalid email address",
            LastModified = "2015/03/24")]
        public string InvalidEmailMessage
        {
            get
            {
                return this["InvalidEmailMessage"];
            }
        }

        /// <summary>
        /// phrase: Send email to John
        /// </summary>
        /// <value>Send email to John</value>
        [ResourceEntry("EmailTextToDisplayExample",
            Value = "Send email to John",
            Description = "phrase: Send email to John",
            LastModified = "2015/03/24")]
        public string EmailTextToDisplayExample
        {
            get
            {
                return this["EmailTextToDisplayExample"];
            }
        }

        /// <summary>
        /// phrase: Select news
        /// </summary>
        /// <value>Select news</value>
        [ResourceEntry("NewsSelectorHeader",
            Value = "Select news",
            Description = "phrase: Select news",
            LastModified = "2015/03/24")]
        public string NewsSelectorHeader
        {
            get
            {
                return this["NewsSelectorHeader"];
            }
        }

        /// <summary>
        /// phrase: Select content
        /// </summary>
        /// <value>Select content</value>
        [ResourceEntry("DynamicItemsSelectorHeader",
            Value = "Select content",
            Description = "phrase: Select content",
            LastModified = "2015/03/24")]
        public string DynamicItemsSelectorHeader
        {
            get
            {
                return this["DynamicItemsSelectorHeader"];
            }
        }

        /// <summary>
        /// phrase: Select a role
        /// </summary>
        /// <value>Select a role</value>
        [ResourceEntry("RoleSelectorHeader",
            Value = "Select a role",
            Description = "phrase: Select a role",
            LastModified = "2015/03/24")]
        public string RoleSelectorHeader
        {
            get
            {
                return this["RoleSelectorHeader"];
            }
        }

        /// <summary>
        /// phrase: Select a library
        /// </summary>
        /// <value>Select a library</value>
        [ResourceEntry("LibrarySelectorHeader",
            Value = "Select a library",
            Description = "phrase: Select a library",
            LastModified = "2015/03/24")]
        public string LibrarySelectorHeader
        {
            get
            {
                return this["LibrarySelectorHeader"];
            }
        }

        /// <summary>
        /// phrase: Select a page
        /// </summary>
        /// <value>Select a page</value>
        [ResourceEntry("PageSelectorHeader",
            Value = "Select a page",
            Description = "phrase: Select a page",
            LastModified = "2015/03/24")]
        public string PageSelectorHeader
        {
            get
            {
                return this["PageSelectorHeader"];
            }
        }

        /// <summary>
        /// phrase: Select categories
        /// </summary>
        /// <value>Select categories</value>
        [ResourceEntry("CategorySelectorHeader",
            Value = "Select categories",
            Description = "phrase: Select categories",
            LastModified = "2015/03/24")]
        public string CategorySelectorHeader
        {
            get
            {
                return this["CategorySelectorHeader"];
            }
        }

        /// <summary>
        /// phrase: No external URLs have been added.
        /// </summary>
        /// <value>No external URLs have been added.</value>
        [ResourceEntry("NoExternalUrlsAdded",
            Value = "No external URLs have been added.",
            Description = "phrase: No external URLs have been added.",
            LastModified = "2015/03/24")]
        public string NoExternalUrlsAdded
        {
            get
            {
                return this["NoExternalUrlsAdded"];
            }
        }

        /// <summary>
        /// phrase: Enter Title
        /// </summary>
        /// <value>Enter Title</value>
        [ResourceEntry("EnterTitle",
            Value = "Enter Title",
            Description = "phrase: Enter Title",
            LastModified = "2015/03/24")]
        public string EnterTitle
        {
            get
            {
                return this["EnterTitle"];
            }
        }

        /// <summary>
        /// phrase: Enter URL
        /// </summary>
        [ResourceEntry("EnterUrl",
            Value = "Enter URL",
            Description = "phrase: Enter URL",
            LastModified = "2015/03/24")]
        public string EnterUrl
        {
            get
            {
                return this["EnterUrl"];
            }
        }

        /// <summary>
        /// phrase: Open external URLs in new tab/window
        /// </summary>
        [ResourceEntry("OpenExternalUrlInNewWindow",
            Value = "Open external URLs in new tab/window",
            Description = "phrase: Open external URLs in new tab/window",
            LastModified = "2015/03/24")]
        public string OpenExternalUrlInNewWindow
        {
            get
            {
                return this["OpenExternalUrlInNewWindow"];
            }
        }

        /// <summary>
        /// phrase: Add external URL
        /// </summary>
        [ResourceEntry("AddExternalUrl",
            Value = "Add external URL",
            Description = "phrase: Add external URL",
            LastModified = "2015/03/24")]
        public string AddExternalUrl
        {
            get
            {
                return this["AddExternalUrl"];
            }
        }

        /// <summary>
        /// phrase: - Hour -
        /// </summary>
        /// <value>- Hour -</value>
        [ResourceEntry("HourOption",
            Value = "- Hour -",
            Description = "phrase: - Hour -",
            LastModified = "2015/03/25")]
        public string HourOption
        {
            get
            {
                return this["HourOption"];
            }
        }

        /// <summary>
        /// phrase: - Minute -
        /// </summary>
        /// <value>- Minute -</value>
        [ResourceEntry("MinuteOption",
            Value = "- Minute -",
            Description = "phrase: - Minute -",
            LastModified = "2015/03/25")]
        public string MinuteOption
        {
            get
            {
                return this["MinuteOption"];
            }
        }

        /// <summary>
        /// phrase: Upload image
        /// </summary>
        /// <value>Upload image</value>
        [ResourceEntry("UploadImage",
            Value = "Upload image",
            Description = "phrase: Upload image",
            LastModified = "2015/03/25")]
        public string UploadImage
        {
            get
            {
                return this["UploadImage"];
            }
        }

        /// <summary>
        /// phrase: Image to upload
        /// </summary>
        /// <value>Image to upload</value>
        [ResourceEntry("ImageToUpload",
            Value = "Image to upload",
            Description = "phrase: Image to upload",
            LastModified = "2015/03/25")]
        public string ImageToUpload
        {
            get
            {
                return this["ImageToUpload"];
            }
        }

        /// <summary>
        /// phrase: Where to store the uploaded image?
        /// </summary>
        /// <value>Where to store the uploaded image?</value>
        [ResourceEntry("WhereToStoreImage",
            Value = "Where to store the uploaded image?",
            Description = "phrase: Where to store the uploaded image?",
            LastModified = "2015/03/25")]
        public string WhereToStoreImage
        {
            get
            {
                return this["WhereToStoreImage"];
            }
        }

        /// <summary>
        /// phrase: You must select the library in which the files ought to be uploaded.
        /// </summary>
        /// <value>You must select the library in which the files ought to be uploaded.</value>
        [ResourceEntry("SelectLibraryErrorMessage",
            Value = "You must select the library in which the files ought to be uploaded.",
            Description = "phrase: You must select the library in which the files ought to be uploaded.",
            LastModified = "2015/03/25")]
        public string SelectLibraryErrorMessage
        {
            get
            {
                return this["SelectLibraryErrorMessage"];
            }
        }

        /// <summary>
        /// word: Upload
        /// </summary>
        /// <value>Upload</value>
        [ResourceEntry("Upload",
            Value = "Upload",
            Description = "word: Upload",
            LastModified = "2015/03/25")]
        public string Upload
        {
            get
            {
                return this["Upload"];
            }
        }

        /// <summary>
        /// phrase: Categories and tags
        /// </summary>
        /// <value>Categories and tags</value>
        [ResourceEntry("CategoriesAndTags",
            Value = "Categories and tags",
            Description = "phrase: Categories and tags",
            LastModified = "2015/03/25")]
        public string CategoriesAndTags
        {
            get
            {
                return this["CategoriesAndTags"];
            }
        }

        /// <summary>
        /// phrase: Custom thumbnail size
        /// </summary>
        /// <value>Custom thumbnail size</value>
        [ResourceEntry("CustomThumbnailSize",
            Value = "Custom thumbnail size",
            Description = "phrase: Custom thumbnail size",
            LastModified = "2015/03/25")]
        public string CustomThumbnailSize
        {
            get
            {
                return this["CustomThumbnailSize"];
            }
        }

        /// <summary>
        /// phrase: Resize image
        /// </summary>
        /// <value>Resize image</value>
        [ResourceEntry("ResizeImage",
            Value = "Resize image",
            Description = "phrase: Resize image",
            LastModified = "2015/03/25")]
        public string ResizeImage
        {
            get
            {
                return this["ResizeImage"];
            }
        }

        /// <summary>
        /// phrase: Generated image will be resized to desired area
        /// </summary>
        /// <value>Generated image will be resized to desired area</value>
        [ResourceEntry("ResizeToAreaDescription",
            Value = "Generated image will be resized to desired area",
            Description = "phrase: Generated image will be resized to desired area",
            LastModified = "2015/03/25")]
        public string ResizeToAreaDescription
        {
            get
            {
                return this["ResizeToAreaDescription"];
            }
        }

        /// <summary>
        /// phrase: resize to area example
        /// </summary>
        /// <value>resize to area example</value>
        [ResourceEntry("ResizeToAreaExample",
            Value = "resize to area example",
            Description = "phrase: resize to area example",
            LastModified = "2015/03/25")]
        public string ResizeToAreaExample
        {
            get
            {
                return this["ResizeToAreaExample"];
            }
        }

        /// <summary>
        /// phrase: Generated image will be resized and cropped to desired area
        /// </summary>
        /// <value>Generated image will be resized and cropped to desired area</value>
        [ResourceEntry("CropToAreaDescription",
            Value = "Generated image will be resized and cropped to desired area",
            Description = "phrase: Generated image will be resized and cropped to desired area",
            LastModified = "2015/03/25")]
        public string CropToAreaDescription
        {
            get
            {
                return this["CropToAreaDescription"];
            }
        }

        /// <summary>
        /// phrase: crop to area example
        /// </summary>
        /// <value>crop to area example</value>
        [ResourceEntry("CropToAreaExample",
            Value = "crop to area example",
            Description = "phrase: crop to area example",
            LastModified = "2015/03/25")]
        public string CropToAreaExample
        {
            get
            {
                return this["CropToAreaExample"];
            }
        }

        /// <summary>
        /// phrase: Resize to area
        /// </summary>
        /// <value>Resize to area</value>
        [ResourceEntry("ResizeToArea",
            Value = "Resize to area",
            Description = "phrase: Resize to area",
            LastModified = "2015/03/25")]
        public string ResizeToArea
        {
            get
            {
                return this["ResizeToArea"];
            }
        }

        /// <summary>
        /// phrase: Crop to area
        /// </summary>
        /// <value>Crop to area</value>
        [ResourceEntry("CropToArea",
            Value = "Crop to area",
            Description = "phrase: Crop to area",
            LastModified = "2015/03/25")]
        public string CropToArea
        {
            get
            {
                return this["CropToArea"];
            }
        }

        /// <summary>
        /// phrase: What's this?
        /// </summary>
        /// <value>What's this?</value>
        [ResourceEntry("WhatIsThisLink",
            Value = "What's this?",
            Description = "phrase: What's this?",
            LastModified = "2015/03/25")]
        public string WhatIsThisLink
        {
            get
            {
                return this["WhatIsThisLink"];
            }
        }

        /// <summary>
        /// phrase: Max width
        /// </summary>
        /// <value>Max width</value>
        [ResourceEntry("MaxWidth",
            Value = "Max width",
            Description = "phrase: Max width",
            LastModified = "2015/03/25")]
        public string MaxWidth
        {
            get
            {
                return this["MaxWidth"];
            }
        }

        /// <summary>
        /// phrase: Value must be an integer between 1 and 9999 inclusive.
        /// </summary>
        /// <value>Value must be an integer between 1 and 9999 inclusive.</value>
        [ResourceEntry("WidthHeightErrorMessage",
            Value = "Value must be an integer between 1 and 9999 inclusive.",
            Description = "phrase: Value must be an integer between 1 and 9999 inclusive.",
            LastModified = "2015/03/25")]
        public string WidthHeightErrorMessage
        {
            get
            {
                return this["WidthHeightErrorMessage"];
            }
        }

        /// <summary>
        /// phrase: Max height
        /// </summary>
        /// <value>Max height</value>
        [ResourceEntry("MaxHeight",
            Value = "Max height",
            Description = "phrase: Max height",
            LastModified = "2015/03/25")]
        public string MaxHeight
        {
            get
            {
                return this["MaxHeight"];
            }
        }

        /// <summary>
        /// phrase: Max width is required
        /// </summary>
        /// <value>Max width is required</value>
        [ResourceEntry("MaxWidthRequired",
            Value = "Max width is required",
            Description = "phrase: Max width is required",
            LastModified = "2015/03/25")]
        public string MaxWidthRequired
        {
            get
            {
                return this["MaxWidthRequired"];
            }
        }

        /// <summary>
        /// phrase: Max height is required
        /// </summary>
        /// <value>Max height is required</value>
        [ResourceEntry("MaxHeightRequired",
            Value = "Max height is required",
            Description = "phrase: Max height is required",
            LastModified = "2015/03/25")]
        public string MaxHeightRequired
        {
            get
            {
                return this["MaxHeightRequired"];
            }
        }

        /// <summary>
        /// word: Width
        /// </summary>
        /// <value>Width</value>
        [ResourceEntry("Width",
            Value = "Width",
            Description = "word: Width",
            LastModified = "2015/03/25")]
        public string Width
        {
            get
            {
                return this["Width"];
            }
        }

        /// <summary>
        /// phrase: Width is required
        /// </summary>
        /// <value>Width is required</value>
        [ResourceEntry("WidthRequired",
            Value = "Width is required",
            Description = "phrase: Width is required",
            LastModified = "2015/03/25")]
        public string WidthRequired
        {
            get
            {
                return this["WidthRequired"];
            }
        }

        /// <summary>
        /// word: Height
        /// </summary>
        /// <value>Height</value>
        [ResourceEntry("Height",
            Value = "Height",
            Description = "word: Height",
            LastModified = "2015/03/25")]
        public string Height
        {
            get
            {
                return this["Height"];
            }
        }

        /// <summary>
        /// phrase: Height is required
        /// </summary>
        /// <value>Height is required</value>
        [ResourceEntry("HeightRequired",
            Value = "Height is required",
            Description = "phrase: Height is required",
            LastModified = "2015/03/25")]
        public string HeightRequired
        {
            get
            {
                return this["HeightRequired"];
            }
        }

        /// <summary>
        /// phrase: Resize smaller images to bigger dimensions
        /// </summary>
        /// <value>Resize smaller images to bigger dimensions</value>
        [ResourceEntry("ResizeSmallerToBigger",
            Value = "Resize smaller images to bigger dimensions",
            Description = "phrase: Resize smaller images to bigger dimensions",
            LastModified = "2015/03/25")]
        public string ResizeSmallerToBigger
        {
            get
            {
                return this["ResizeSmallerToBigger"];
            }
        }

        /// <summary>
        /// word: Quality
        /// </summary>
        /// <value>Quality</value>
        [ResourceEntry("Quality",
            Value = "Quality",
            Description = "word: Quality",
            LastModified = "2015/03/25")]
        public string Quality
        {
            get
            {
                return this["Quality"];
            }
        }

        /// <summary>
        /// phrase: Dimensions:
        /// </summary>
        /// <value>Dimensions:</value>
        [ResourceEntry("Dimensions",
            Value = "Dimensions:",
            Description = "phrase: Dimensions:",
            LastModified = "2015/03/25")]
        public string Dimensions
        {
            get
            {
                return this["Dimensions"];
            }
        }

        /// <summary>
        /// phrase: Date modified:
        /// </summary>
        /// <value>Date modified:</value>
        [ResourceEntry("ModifiedOn",
            Value = "Date modified:",
            Description = "phrase: Date modified:",
            LastModified = "2015/03/25")]
        public string ModifiedOn
        {
            get
            {
                return this["ModifiedOn"];
            }
        }

        /// <summary>
        /// word: Style
        /// </summary>
        /// <value>Style</value>
        [ResourceEntry("Style",
            Value = "Style",
            Description = "word: Style",
            LastModified = "2015/03/25")]
        public string Style
        {
            get
            {
                return this["Style"];
            }
        }

        /// <summary>
        /// phrase: - Select -
        /// </summary>
        /// <value>- Select -</value>
        [ResourceEntry("SelectStyle",
            Value = "- Select -",
            Description = "phrase: - Select -",
            LastModified = "2015/03/25")]
        public string SelectStyle
        {
            get
            {
                return this["SelectStyle"];
            }
        }

        /// <summary>
        /// phrase: Select image from your computer
        /// </summary>
        /// <value>Select image from your computer</value>
        [ResourceEntry("SelectImageFromComputer",
            Value = "Select image from your computer",
            Description = "phrase: Select image from your computer",
            LastModified = "2015/03/25")]
        public string SelectImageFromComputer
        {
            get
            {
                return this["SelectImageFromComputer"];
            }
        }

        /// <summary>
        /// phrase: No images
        /// </summary>
        /// <value>No images</value>
        [ResourceEntry("NoImages",
            Value = "No images",
            Description = "phrase: No images",
            LastModified = "2015/03/25")]
        public string NoImages
        {
            get
            {
                return this["NoImages"];
            }
        }

        /// <summary>
        /// phrase: Already uploaded images
        /// </summary>
        /// <value>Already uploaded images</value>
        [ResourceEntry("AlreadyUploadedImages",
            Value = "Already uploaded images",
            Description = "phrase: Already uploaded images",
            LastModified = "2015/03/25")]
        public string AlreadyUploadedImages
        {
            get
            {
                return this["AlreadyUploadedImages"];
            }
        }

        /// <summary>
        /// phrase: Select lists
        /// </summary>
        /// <value>Select lists</value>
        [ResourceEntry("ListSelectorHeader",
            Value = "Select lists",
            Description = "phrase: Select lists",
            LastModified = "2015/03/25")]
        public string ListSelectorHeader
        {
            get
            {
                return this["ListSelectorHeader"];
            }
        }

        #region Videos
        /// <summary>
        /// phrase: No videos
        /// </summary>
        /// <value>No videos</value>
        [ResourceEntry("NoVideos",
            Value = "No videos",
            Description = "phrase: No videos",
            LastModified = "2015/03/30")]
        public string NoVideos
        {
            get
            {
                return this["NoVideos"];
            }
        }

        /// <summary>
        /// phrase: Upload video
        /// </summary>
        /// <value>Upload video</value>
        [ResourceEntry("UploadVideo",
            Value = "Upload video",
            Description = "phrase: Upload video",
            LastModified = "2015/03/30")]
        public string UploadVideo
        {
            get
            {
                return this["UploadVideo"];
            }
        }

        /// <summary>
        /// phrase: Already uploaded videos
        /// </summary>
        /// <value>Already uploaded videos</value>
        [ResourceEntry("AlreadyUploadedVideos",
            Value = "Already uploaded videos",
            Description = "phrase: Already uploaded videos",
            LastModified = "2015/03/30")]
        public string AlreadyUploadedVideos
        {
            get
            {
                return this["AlreadyUploadedVideos"];
            }
        }

        /// <summary>
        /// word: Video
        /// </summary>
        /// <value>Video</value>
        [ResourceEntry("VideoSingular",
            Value = "Video",
            Description = "word: Video",
            LastModified = "2015/03/30")]
        public string VideoSingular
        {
            get
            {
                return this["VideoSingular"];
            }
        }

        /// <summary>
        /// word: Videos
        /// </summary>
        /// <value>Videos</value>
        [ResourceEntry("VideoPlural",
            Value = "Videos",
            Description = "word: Videos",
            LastModified = "2015/03/30")]
        public string VideoPlural
        {
            get
            {
                return this["VideoPlural"];
            }
        }

        /// <summary>
        /// word: Select video from your computer
        /// </summary>
        /// <value>Select video from your computer</value>
        [ResourceEntry("SelectVideoFromComputer",
            Value = "Select video from your computer",
            Description = "word: Select video from your computer",
            LastModified = "2015/03/30")]
        public string SelectVideoFromComputer
        {
            get
            {
                return this["SelectVideoFromComputer"];
            }
        }

        /// <summary>
        /// phrase: Insert a video
        /// </summary>
        /// <value>Insert a video</value>
        [ResourceEntry("InsertVideoLink",
            Value = "Insert a video",
            Description = "phrase: Insert a video",
            LastModified = "2015/03/31")]
        public string InsertVideoLink
        {
            get
            {
                return this["InsertVideoLink"];
            }
        }

        /// <summary>
        /// phrase: Aspect ratio
        /// </summary>
        /// <value>Aspect ratio</value>
        [ResourceEntry("AspectRatio",
            Value = "Aspect ratio",
            Description = "phrase: Aspect ratio",
            LastModified = "2015/03/31")]
        public string AspectRatio
        {
            get
            {
                return this["AspectRatio"];
            }
        }

        /// <summary>
        /// phrase: The video has been deleted or unpublished
        /// </summary>
        [ResourceEntry("VideoHasBeenDeleted",
            Value = "The video has been deleted or unpublished",
            Description = "phrase: The video has been deleted or unpublished",
            LastModified = "2015/03/23")]
        public string VideoHasBeenDeleted
        {
            get
            {
                return this["VideoHasBeenDeleted"];
            }
        }

        /// <summary>
        /// word: Auto
        /// </summary>
        /// <value>Auto</value>
        [ResourceEntry("Auto",
            Value = "Auto",
            Description = "word: Auto",
            LastModified = "2015/03/31")]
        public string Auto
        {
            get
            {
                return this["Auto"];
            }
        }

        /// <summary>
        /// phrase: Change video
        /// </summary>
        [ResourceEntry("ChangeVideo",
            Value = "Change video",
            Description = "phrase: Change video",
            LastModified = "2015/03/23")]
        public string ChangeVideo
        {
            get
            {
                return this["ChangeVideo"];
            }
        }

        /// <summary>
        /// word: 4x3
        /// </summary>
        /// <value>4x3</value>
        [ResourceEntry("Ratio4X3",
            Value = "4x3",
            Description = "word: 4x3",
            LastModified = "2015/04/01")]
        public string Ratio4X3
        {
            get
            {
                return this["Ratio4X3"];
            }
        }

        /// <summary>
        /// phrase: Select video
        /// </summary>
        [ResourceEntry("SelectVideo",
            Value = "Select video",
            Description = "phrase: Select video",
            LastModified = "2015/03/23")]
        public string SelectVideo
        {
            get
            {
                return this["SelectVideo"];
            }
        }

        /// <summary>
        /// word: 16x9
        /// </summary>
        /// <value>16x9</value>
        [ResourceEntry("Ratio16X9",
            Value = "16x9",
            Description = "word: 16x9",
            LastModified = "2015/04/01")]
        public string Ratio16X9
        {
            get
            {
                return this["Ratio16X9"];
            }
        }

        /// <summary>
        /// phrase: Video to upload
        /// </summary>
        /// <value>Video to upload</value>
        [ResourceEntry("VideoToUpload",
            Value = "Video to upload",
            Description = "phrase: Video to upload",
            LastModified = "2015/04/01")]
        public string VideoToUpload
        {
            get
            {
                return this["VideoToUpload"];
            }
        }

        /// <summary>
        /// phrase: Where to store video
        /// </summary>
        /// <value>Where to store video</value>
        [ResourceEntry("WhereToStoreVideo",
            Value = "Where to store video",
            Description = "phrase: Where to store video",
            LastModified = "2015/04/01")]
        public string WhereToStoreVideo
        {
            get
            {
                return this["WhereToStoreVideo"];
            }
        }

        /// <summary>
        /// phrase: Recent videos
        /// </summary>
        /// <value>Recent videos</value>
        [ResourceEntry("RecentVideos",
            Value = "Recent videos",
            Description = "phrase: Recent videos",
            LastModified = "2015/04/01")]
        public string RecentVideos
        {
            get
            {
                return this["RecentVideos"];
            }
        }

        /// <summary>
        /// phrase: My videos
        /// </summary>
        /// <value>My videos</value>
        [ResourceEntry("MyVideos",
            Value = "My videos",
            Description = "phrase: My videos",
            LastModified = "2015/04/01")]
        public string MyVideos
        {
            get
            {
                return this["MyVideos"];
            }
        }
        #endregion

        /// <summary>
        /// word: Folder
        /// </summary>
        /// <value>Folder</value>
        [ResourceEntry("FolderSingular",
            Value = "Folder",
            Description = "word: Folder",
            LastModified = "2015/03/30")]
        public string FolderSingular
        {
            get
            {
                return this["FolderSingular"];
            }
        }

        /// <summary>
        /// word: Folders
        /// </summary>
        /// <value>Folders</value>
        [ResourceEntry("FolderPlural",
            Value = "Folders",
            Description = "word: Folders",
            LastModified = "2015/03/30")]
        public string FolderPlural
        {
            get
            {
                return this["FolderPlural"];
            }
        }

        /// <summary>
        /// phrase: More options
        /// </summary>
        /// <value>More options</value>
        [ResourceEntry("MoreOptions",
            Value = "More options",
            Description = "phrase: More options",
            LastModified = "2015/03/31")]
        public string MoreOptions
        {
            get
            {
                return this["MoreOptions"];
            }
        }

        /// <summary>
        /// word: Custom
        /// </summary>
        /// <value>Custom</value>
        [ResourceEntry("Custom",
            Value = "Custom",
            Description = "word: Custom",
            LastModified = "2015/03/31")]
        public string Custom
        {
            get
            {
                return this["Custom"];
            }
        }

        /// <summary>
        /// word: px
        /// </summary>
        /// <value>px</value>
        [ResourceEntry("PixelShort",
            Value = "px",
            Description = "word: px",
            LastModified = "2015/03/31")]
        public string PixelShort
        {
            get
            {
                return this["PixelShort"];
            }
        }

        /// <summary>
        /// phrase: Last 1 day
        /// </summary>
        /// <value>Last 1 day</value>
        [ResourceEntry("Last1Day",
            Value = "Last 1 day",
            Description = "phrase: Last 1 day",
            LastModified = "2015/04/01")]
        public string Last1Day
        {
            get
            {
                return this["Last1Day"];
            }
        }

        /// <summary>
        /// phrase: Last 3 days
        /// </summary>
        /// <value>Last 3 days</value>
        [ResourceEntry("Last3Days",
            Value = "Last 3 days",
            Description = "phrase: Last 3 days",
            LastModified = "2015/04/01")]
        public string Last3Days
        {
            get
            {
                return this["Last3Days"];
            }
        }

        /// <summary>
        /// phrase: Last 1 week
        /// </summary>
        /// <value>Last 1 week</value>
        [ResourceEntry("Last1Week",
            Value = "Last 1 week",
            Description = "phrase: Last 1 week",
            LastModified = "2015/04/01")]
        public string Last1Week
        {
            get
            {
                return this["Last1Week"];
            }
        }

        /// <summary>
        /// phrase: Last 1 month
        /// </summary>
        /// <value>Last 1 month</value>
        [ResourceEntry("Last1Month",
            Value = "Last 1 month",
            Description = "phrase: Last 1 month",
            LastModified = "2015/04/01")]
        public string Last1Month
        {
            get
            {
                return this["Last1Month"];
            }
        }

        /// <summary>
        /// phrase: Last 6 months
        /// </summary>
        /// <value>Last 6 months</value>
        [ResourceEntry("Last6Months",
            Value = "Last 6 months",
            Description = "phrase: Last 6 months",
            LastModified = "2015/04/01")]
        public string Last6Months
        {
            get
            {
                return this["Last6Months"];
            }
        }

        /// <summary>
        /// phrase: Last 1 year
        /// </summary>
        /// <value>Last 1 year</value>
        [ResourceEntry("Last1Year",
            Value = "Last 1 year",
            Description = "phrase: Last 1 year",
            LastModified = "2015/04/01")]
        public string Last1Year
        {
            get
            {
                return this["Last1Year"];
            }
        }

        /// <summary>
        /// phrase: Last 2 years
        /// </summary>
        /// <value>Last 2 years</value>
        [ResourceEntry("Last2Years",
            Value = "Last 2 years",
            Description = "phrase: Last 2 years",
            LastModified = "2015/04/01")]
        public string Last2Years
        {
            get
            {
                return this["Last2Years"];
            }
        }

        /// <summary>
        /// phrase: Last 5 years
        /// </summary>
        /// <value>Last 5 years</value>
        [ResourceEntry("Last5Years",
            Value = "Last 5 years",
            Description = "phrase: Last 5 years",
            LastModified = "2015/04/01")]
        public string Last5Years
        {
            get
            {
                return this["Last5Years"];
            }
        }

        /// <summary>
        /// phrase: Select blogs
        /// </summary>
        /// <value>Select blogs</value>
        [ResourceEntry("SelectBlogs",
            Value = "Select blogs",
            Description = "phrase: Select blogs",
            LastModified = "2015/04/24")]
        public string SelectBlogs
        {
            get
            {
                return this["SelectBlogs"];
            }
        }

        /// <summary>
        /// phrase: Select blog posts
        /// </summary>
        /// <value>Select blog posts</value>
        [ResourceEntry("SelectBlogPosts",
            Value = "Select blog posts",
            Description = "phrase: Select blog posts",
            LastModified = "2015/04/24")]
        public string SelectBlogPosts
        {
            get
            {
                return this["SelectBlogPosts"];
            }
        }

        /// <summary>
        /// phrase: Select a user
        /// </summary>
        /// <value>Select a user</value>
        [ResourceEntry("UserSelectorHeader",
            Value = "Select a user",
            Description = "phrase: Select a user",
            LastModified = "2015/04/09")]
        public string UserSelectorHeader
        {
            get
            {
                return this["UserSelectorHeader"];
            }
        }

        /// <summary>
        /// phrase: Select feed
        /// </summary>
        /// <value>Select feed</value>
        [ResourceEntry("FeedSelectorHeader",
            Value = "Select feed",
            Description = "phrase: Select feed",
            LastModified = "2015/06/10")]
        public string FeedSelectorHeader
        {
            get
            {
                return this["FeedSelectorHeader"];
            }
        }

        /// <summary>
        /// phrase: All tools
        /// </summary>
        /// <value>All tools</value>
        [ResourceEntry("AllTools",
            Value = "All tools",
            Description = "phrase: All tools",
            LastModified = "2015/06/10")]
        public string AllTools
        {
            get
            {
                return this["AllTools"];
            }
        }

        /// <summary>
        /// phrase: Select a mailing list
        /// </summary>
        /// <value>Select a mailing list</value>
        [ResourceEntry("MailingListSelectorHeader",
            Value = "Select a mailing list",
            Description = "phrase: Select a mailing list",
            LastModified = "2015/06/17")]
        public string MailingListSelectorHeader
        {
            get
            {
                return this["MailingListSelectorHeader"];
            }
        }
    }
}
