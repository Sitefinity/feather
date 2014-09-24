using System;
using System.Linq;
using Telerik.Sitefinity.Data;

namespace Telerik.Sitefinity.Frontend.FilesMonitoring.Data
{
    /// <summary>
    /// Represents the data provider for the file monitoring functionality.
    /// </summary>
    [Telerik.Microsoft.Practices.Unity.InterceptionExtension.ApplyNoPolicies]
    internal abstract class FileMonitorDataProvider : DataProviderBase
    {
        #region Public methods

        /// <summary>
        /// Gets the known types.
        /// </summary>
        /// <returns></returns>
        public override Type[] GetKnownTypes()
        {
            if (knownTypes == null)
            {
                knownTypes = new Type[]
                {
                    typeof(FileData),
                };
            }

            return knownTypes;
        }

        /// <summary>
        /// Gets a unique key for each data provider base.
        /// </summary>
        /// <value></value>
        public override string RootKey
        {
            get { return "FileMonitorDataProvider"; }
        }

        #region Package Methods

        /// <summary>
        /// Creates the file data.
        /// </summary>
        /// <returns></returns>
        public abstract FileData CreateFileData();

        /// <summary>
        /// Creates the file data.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public abstract FileData CreateFileData(Guid id);

        /// <summary>
        /// Gets the file data.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public abstract FileData GetFileData(Guid id);

        /// <summary>
        /// Gets the files data.
        /// </summary>
        /// <returns></returns>
        public abstract IQueryable<FileData> GetFilesData();

        /// <summary>
        /// Deletes the specified file data.
        /// </summary>
        /// <param name="fileDataToDelete">The file data to delete.</param>
        public abstract void Delete(FileData fileDataToDelete);

        #endregion

        #region Item Methods

        /// <inheritdoc />
        public override object CreateItem(Type itemType, Guid id)
        {
            if (itemType == null)
                throw new ArgumentNullException("itemType");

            if (itemType == typeof(FileData))
                return this.CreateFileData(id);

            throw Telerik.Sitefinity.Data.DataProviderBase.GetInvalidItemTypeException(itemType, this.GetKnownTypes());
        }

        /// <inheritdoc />
        public override object GetItem(Type itemType, Guid id)
        {
            if (itemType == null)
                throw new ArgumentNullException("itemType");

            if (itemType == typeof(FileData))
                return this.GetFileData(id);

            return base.GetItem(itemType, id);
        }

        /// <inheritdoc />
        public override object GetItemOrDefault(Type itemType, Guid id)
        {
            return base.GetItemOrDefault(itemType, id);
        }

        /// <inheritdoc />
        public override System.Collections.IEnumerable GetItems(Type itemType, string filterExpression, string orderExpression, int skip, int take, ref int? totalCount)
        {
            if (itemType == null)
                throw new ArgumentNullException("itemType");

            throw Telerik.Sitefinity.Data.DataProviderBase.GetInvalidItemTypeException(itemType, this.GetKnownTypes());
        }

        /// <inheritdoc />
        public override void DeleteItem(object item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            var itemType = item.GetType();

            if (itemType == typeof(FileData))
                this.Delete((FileData)item);
            else
                throw Telerik.Sitefinity.Data.DataProviderBase.GetInvalidItemTypeException(itemType, this.GetKnownTypes());
        }

        #endregion

        #endregion

        #region Private fields

        private static Type[] knownTypes;

        #endregion
    }
}
