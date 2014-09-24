using System;
using System.Collections.Specialized;
using System.Linq;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.FilesMonitoring.Data
{
    /// <summary>
    /// Represents the data manager for the file monitoring functionality.
    /// </summary>
    internal class FileMonitorDataManager : ManagerBase<FileMonitorDataProvider>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FileMonitorDataManager"/> class.
        /// </summary>
        public FileMonitorDataManager() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileMonitorDataManager"/> class.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        public FileMonitorDataManager(string providerName)
        : base(providerName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileMonitorDataManager"/> class.
        /// </summary>
        /// <param name="providerName">
        /// The name of the provider. If empty string or null the default provider is set
        /// </param>
        /// <param name="transactionName">
        /// The name of a distributed transaction. If empty string or null this manager will use separate transaction.
        /// </param>
        public FileMonitorDataManager(string providerName, string transactionName)
        : base(providerName, transactionName)
        {
        }

        #endregion

        #region Base Overrides

        /// <inheritdoc />
        public override string ModuleName
        {
            get { return "Telerik.Sitefinity.Frontend"; }
        }

        /// <inheritdoc />
        public static FileMonitorDataManager GetManager()
        {
            return ManagerBase<FileMonitorDataProvider>.GetManager<FileMonitorDataManager>();
        }

        /// <inheritdoc />
        public static FileMonitorDataManager GetManager(string providerName, string transactionName)
        {
            return ManagerBase<FileMonitorDataProvider>.GetManager<FileMonitorDataManager>(providerName, transactionName);
        }

        /// <inheritdoc />
        public static FileMonitorDataManager GetManager(string providerName)
        {
            return ManagerBase<FileMonitorDataProvider>.GetManager<FileMonitorDataManager>(providerName);
        }

        /// <inheritdoc />
        protected override GetDefaultProvider DefaultProviderDelegate
        {
            get { return () => "OpenAccessDataProvider"; }
        }

        /// <inheritdoc />
        protected override ConfigElementDictionary<string, DataProviderSettings> ProvidersSettings
        {
            get { return this.providerSettings; }
        }

        #endregion

        #region Package

        /// <summary>
        /// Creates the file data.
        /// </summary>
        /// <returns></returns>
        public FileData CreateFileData()
        {
            return this.Provider.CreateFileData();
        }

        /// <summary>
        /// Creates the file data.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public FileData CreateFileData(Guid id)
        {
            return this.Provider.CreateFileData(id);
        }

        /// <summary>
        /// Gets the file data.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public FileData GetFileData(Guid id)
        {
            return this.Provider.GetFileData(id);
        }

        public IQueryable<FileData> GetFilesData()
        {
            return this.Provider.GetFilesData();
        }

        /// <summary>
        /// Deletes the specified file data to delete.
        /// </summary>
        /// <param name="fileDataToDelete">The file data to delete.</param>
        public void Delete(FileData fileDataToDelete)
        {
            this.Provider.Delete(fileDataToDelete);
        }

        #endregion

        #region Protected methods

        protected override void Initialize()
        {
            this.providerSettings = new ConfigElementDictionary<string, DataProviderSettings>(Config.Get<SystemConfig>());
            this.providerSettings.Add(new DataProviderSettings(this.providerSettings)
            {
                Name = "OpenAccessDataProvider",
                Description = "A provider that stores File monitor data in database using OpenAccess ORM.",
                ProviderType = typeof(OpenAccessFileMonitorDataProvider),
                Parameters = new NameValueCollection() { { "applicationName", "/Telerik.Sitefinity.Frontend" } }
            });

            base.Initialize();
        }

        #endregion

        #region Private fields

        private ConfigElementDictionary<string, DataProviderSettings> providerSettings;

        #endregion
    }
}
