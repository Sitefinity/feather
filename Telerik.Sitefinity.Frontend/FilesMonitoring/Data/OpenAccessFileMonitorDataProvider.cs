using System;
using System.Linq;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Data.OA;

namespace Telerik.Sitefinity.Frontend.FilesMonitoring.Data
{
    /// <summary>
    /// OpenAccess implementation of the <see cref="FileMonitorDataProvider"/>.
    /// </summary>
    internal class OpenAccessFileMonitorDataProvider : FileMonitorDataProvider, IOpenAccessDataProvider, IOpenAccessUpgradableProvider
    {
        #region Upgrade

        /// <inheritdoc />
        public virtual int CurrentSchemaVersionNumber
        {
            get
            {
                return this.GetAssemblyBuildNumber();
            }
        }

        /// <inheritdoc />
        public virtual void OnUpgrading(UpgradingContext context, int upgradingFromSchemaVersionNumber)
        {
        }

        /// <inheritdoc />
        public virtual void OnUpgraded(UpgradingContext context, int upgradedFromSchemaVersionNumber)
        {
        }

        #endregion

        #region IOpenAccessDataProvider Members

        /// <inheritdoc />
        public Telerik.OpenAccess.Metadata.MetadataSource GetMetaDataSource(Telerik.Sitefinity.Model.IDatabaseMappingContext context)
        {
            return new FileMonitorFluentMetadataSource(context);
        }

        /// <inheritdoc />
        public OpenAccessProviderContext Context
        {
            get;
            set;
        }

        #endregion

        #region File data

        /// <inheritdoc />
        public override FileData CreateFileData()
        {
            return this.CreateFileData(this.GetNewGuid());
        }

        /// <inheritdoc />
        public override FileData CreateFileData(Guid id)
        {
            var fileData = new FileData(id);
            this.GetContext().Add(fileData);
            return fileData;
        }

        /// <inheritdoc />
        public override FileData GetFileData(Guid id)
        {
            return this.GetContext().GetItemById<FileData>(id.ToString());
        }

        /// <inheritdoc />
        public override IQueryable<FileData> GetFilesData()
        {
            return this.GetContext().GetAll<FileData>();
        }

        /// <inheritdoc />
        public override void Delete(FileData fileDataToDelete)
        {
            this.GetContext().Delete(fileDataToDelete);
        }

        #endregion
    }
}
