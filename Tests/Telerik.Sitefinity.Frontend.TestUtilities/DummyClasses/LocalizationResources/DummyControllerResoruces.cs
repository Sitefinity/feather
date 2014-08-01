using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Localization;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.LocalizationResources
{
    /// <summary>
    /// This class is used to test localization for the controllers.
    /// </summary>
    [ObjectInfo(typeof(DummyControllerResoruces), Title = "DummyControllerResoruces", Description = "DummyControllerResorucesDescription")]
    public class DummyControllerResoruces : Resource
    {
         /// <summary>
        /// Resources for Comments
        /// </summary>
        [ResourceEntry("DummyResource",
            Value = "Dummy Resource",
            Description = "Some dummy resource.",
            LastModified = "2014/05/20")]
        public string DummyResource
        {
            get
            {
                return this["DummyResource"];
            }
        }
    }
}
