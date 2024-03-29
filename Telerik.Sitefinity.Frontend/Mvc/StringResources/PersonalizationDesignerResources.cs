﻿using Telerik.Sitefinity.Localization;

namespace Telerik.Sitefinity.Frontend.Mvc.StringResources
{
    /// <summary>
    /// Localizable strings for the Personalization's designer.
    /// </summary>
    [ObjectInfo(typeof(PersonalizationDesignerResources), Title = "PersonalizationResourcesTitle", Description = "PersonalizationDesignerResourcesDescription")]
    public class PersonalizationDesignerResources : Resource
    {
        /// <summary>
        /// Title for the personalization designer resources class.
        /// </summary>
        /// <value>Personalization designer resources</value>
        [ResourceEntry("PersonalizationResourcesTitle",
            Value = "Personalization designer resources",
            Description = "Title for the personalization designer resources class.",
            LastModified = "2015/09/09")]
        public string PersonalizationResourcesTitle
        {
            get
            {
                return this["PersonalizationResourcesTitle"];
            }
        }

        /// <summary>
        /// Localizable strings for the Personalization designer
        /// </summary>
        [ResourceEntry("PersonalizationDesignerResourcesDescription",
            Value = "Localizable strings for the Personalizartion designer.",
            Description = "Localizable strings for the Personalization designer",
            LastModified = "2015/09/09")]
        public string PersonalizationDesignerResourcesDescription
        {
            get
            {
                return this["PersonalizationDesignerResourcesDescription"];
            }
        }

        /// <summary>
        /// Error!
        /// </summary>
        [ResourceEntry("Error",
            Value = "Error!",
            Description = "Error!",
            LastModified = "2015/09/09")]
        public string Error
        {
            get
            {
                return this["Error"];
            }
        }

        /// <summary>
        /// Cancel
        /// </summary>
        [ResourceEntry("Cancel",
            Value = "Cancel",
            Description = "Cancel",
            LastModified = "2015/09/09")]
        public string Cancel
        {
            get
            {
                return this["Cancel"];
            }
        }

        /// <summary>
        /// Add personalized version
        /// </summary>
        /// <value>Add personalized version</value>
        [ResourceEntry("AddPersonalizedVersion",
            Value = "Add personalized version",
            Description = "Add personalized version",
            LastModified = "2021/10/15")]
        public string AddPersonalizedVersion
        {
            get
            {
                return this["AddPersonalizedVersion"];
            }
        }

        /// <summary>
        /// Caption for personalization dialog window
        /// </summary>
        /// <value>Add personalized version of this widget</value>
        [ResourceEntry("PersonalizationDialogCaption",
            Value = "Add personalized version of this widget",
            Description = "Caption for personalization dialog window",
            LastModified = "2021/10/15")]
        public string PersonalizationDialogCaption
        {
            get
            {
                return this["PersonalizationDialogCaption"];
            }
        }

        /// <summary>
        /// Segments label for personalization dialog window
        /// </summary>
        /// <value>Which user segment is this personalized version for?</value>
        [ResourceEntry("PersonalizationSegmentsLabel",
            Value = "Which user segment is this personalized version for?",
            Description = "Segments label for personalization dialog window",
            LastModified = "2015/09/09")]
        public string PersonalizationSegmentsLabel
        {
            get
            {
                return this["PersonalizationSegmentsLabel"];
            }
        }

        /// <summary>
        /// Label displayed when you have created versions of the widget for all segments
        /// </summary>
        /// <value>You cannot create other personalization</value>
        [ResourceEntry("AllSegmentsCreatedLabel",
            Value = "You cannot create other personalization.",
            Description = "Label displayed when you have created versions of the widget for all segments.",
            LastModified = "2021/10/15")]
        public string AllSegmentsCreatedLabel
        {
            get
            {
                return this["AllSegmentsCreatedLabel"];
            }
        }

        /// <summary>
        /// Message displayed when you have created versions of the widget for all segments
        /// </summary>
        /// <value>This widget has been personalized for all available user segments.</value>
        [ResourceEntry("AllSegmentsCreatedMessage",
            Value = "This widget has been personalized for all available user segments.",
            Description = "Message displayed when you have created versions of the widget for all segments",
            LastModified = "2015/09/10")]
        public string AllSegmentsCreatedMessage
        {
            get
            {
                return this["AllSegmentsCreatedMessage"];
            }
        }

        /// <summary>
        /// Label displayed when you have not created any segments
        /// </summary>
        /// <value>No user segments have been created yet</value>
        [ResourceEntry("NoSegmentsCreatedLabel",
            Value = "No user segments have been created yet.",
            Description = "Label displayed when you have not created any segments",
            LastModified = "2021/10/15")]
        public string NoSegmentsCreatedLabel
        {
            get
            {
                return this["NoSegmentsCreatedLabel"];
            }
        }

        /// <summary>
        /// Message displayed when you have not created any segments
        /// </summary>
        /// <value>To personalize this widget you should have at least one user segment</value>
        [ResourceEntry("NoSegmentsCreatedMessage",
            Value = "To personalize this widget you should have at least one user segment.",
            Description = "Message displayed when you have not created any segments",
            LastModified = "2021/10/15")]
        public string NoSegmentsCreatedMessage
        {
            get
            {
                return this["NoSegmentsCreatedMessage"];
            }
        }

        /// <summary>
        /// Create a user segment
        /// </summary>
        /// <value>Create a user segment</value>
        [ResourceEntry("CreateUserSegment",
            Value = "Create a user segment",
            Description = "Create a user segment",
            LastModified = "2021/10/15")]
        public string CreateUserSegment
        {
            get
            {
                return this["CreateUserSegment"];
            }
        }
    }
}
