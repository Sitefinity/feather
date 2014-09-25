using System.Collections.Generic;
using Telerik.OpenAccess;
using Telerik.OpenAccess.Metadata.Fluent;
using Telerik.Sitefinity.Model;

namespace Telerik.Sitefinity.Frontend.FilesMonitoring.Data
{
    /// <summary>
    /// A concrete implementation of the <see cref="OpenAccessFluentMappingBase"/> for the file monitoring.
    /// </summary>
    internal class FileMonitorFluentMapping : OpenAccessFluentMappingBase
    {
        #region Contructors

        public FileMonitorFluentMapping(IDatabaseMappingContext context)
            : base(context)
        {
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public override IList<MappingConfiguration> GetMapping()
        {
            var mappings = new List<MappingConfiguration>();

            this.MapFileData(mappings);

            return mappings;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Maps the file data.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        private void MapFileData(List<MappingConfiguration> mappings)
        {
            var fileDataMapping = new MappingConfiguration<FileData>();

            fileDataMapping.MapType(p => new { }).WithConcurencyControl(OptimisticConcurrencyControlStrategy.None).ToTable("sf_files_monitor_data");
            fileDataMapping.HasProperty(p => p.Id).IsIdentity();
            fileDataMapping.HasProperty(p => p.FileName).ToColumn("file_name");
            fileDataMapping.HasProperty(p => p.FilePath).ToColumn("file_path");
            fileDataMapping.HasProperty(p => p.PackageName).ToColumn("package_name");

            mappings.Add(fileDataMapping);
        }

        #endregion
    }
}
