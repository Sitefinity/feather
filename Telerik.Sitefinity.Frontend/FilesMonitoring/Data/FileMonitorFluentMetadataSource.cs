using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Data.OA;
using Telerik.Sitefinity.Model;

namespace Telerik.Sitefinity.Frontend.FilesMonitoring.Data
{
    public class FileMonitorFluentMetadataSource : SitefinityMetadataSourceBase
    {
        #region Contructors

        public FileMonitorFluentMetadataSource()
            : base(null)
        { }


         public FileMonitorFluentMetadataSource(IDatabaseMappingContext context)
            : base(context)
        {

        }

        #endregion

        #region Protected methods

         /// <summary>
        /// Builds the custom mappings.
        /// </summary>
        /// <returns></returns>
        protected override IList<IOpenAccessFluentMapping> BuildCustomMappings()
        {
            var sitefinityMappings = new List<IOpenAccessFluentMapping>();
            sitefinityMappings.Add(new FileMonitorFluentMapping(this.Context) { });
            return sitefinityMappings;
        }

         #endregion

    }
}
