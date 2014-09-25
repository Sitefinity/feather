using System;
using Telerik.OpenAccess;

namespace Telerik.Sitefinity.Frontend.FilesMonitoring.Data
{
    /// <summary>
    /// This class represent file data persisted by the file monitoring functionality. 
    /// </summary>
    [Persistent]
    [ManagerType(typeof(FileMonitorDataManager))]
    internal class FileData
    {
        #region Contructors

        public FileData()
        {
        }

        public FileData(Guid id)
        {
            this.Id = id;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id 
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName
        {
            get
            {
                return this.fileName;
            }

            set
            {
                this.fileName = value;
            }
        }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public string FilePath 
        {
            get
            {
                return this.filePath;
            }

            set
            {
                this.filePath = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the package.
        /// </summary>
        /// <value>
        /// The name of the package.
        /// </value>
        public string PackageName
        {
            get
            {
                return this.packageName;
            }

            set
            {
                this.packageName = value;
            }
        }

        #endregion

        #region Private fields

        private Guid id;
        private string filePath;
        private string fileName;
        private string packageName;

        #endregion
    }
}