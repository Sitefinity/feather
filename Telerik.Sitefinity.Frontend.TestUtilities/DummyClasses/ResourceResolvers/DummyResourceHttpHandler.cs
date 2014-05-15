using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Frontend.Resources;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.ResourceResolvers
{
    /// <summary>
    /// This class mocks the functionality of <see cref="Telerik.Sitefinity.Frontend.Resources.ResourceHttpHandler"/> for test purposes.
    /// </summary>
    public class DummyResourceHttpHandler : ResourceHttpHandler
    {
        /// <summary>
        /// A function that will be called through <see cref="FileExists"/> method.
        /// </summary>
        public Func<string, bool> FileExistsMock;

        /// <summary>
        /// A function that will be called through <see cref="OpenFile"/> method.
        /// </summary>
        public Func<string, Stream> OpenFileMock;

        /// <inheritdoc />
        protected override bool FileExists(string path)
        {
            if (this.FileExistsMock != null)
            {
                return this.FileExistsMock(path);
            }
            else
            {
                return base.FileExists(path);
            }
        }

        /// <inheritdoc />
        protected override Stream OpenFile(string path)
        {
            if (this.OpenFileMock != null)
            {
                return this.OpenFileMock(path);
            }
            else
            {
                return base.OpenFile(path);
            }
        }
    }

}
