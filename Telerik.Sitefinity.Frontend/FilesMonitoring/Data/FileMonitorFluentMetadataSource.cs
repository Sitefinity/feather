using System.Collections.Generic;
using Telerik.Sitefinity.Data.OA;
using Telerik.Sitefinity.Model;

namespace Telerik.Sitefinity.Frontend.FilesMonitoring.Data
{
    /// <summary>
    /// A concrete implementation of the <see cref="SitefinityMetadataSourceBase"/> for the file monitoring.
    /// </summary>
    internal class FileMonitorFluentMetadataSource : SitefinityMetadataSourceBase
    {
        #region Contructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileMonitorFluentMetadataSource"/> class.
        /// </summary>
        public FileMonitorFluentMetadataSource()
            : base(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileMonitorFluentMetadataSource"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
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
