using System;
using System.Runtime.Serialization;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// This type of exception is thrown when sections with duplicating names are found on the current page.
    /// </summary>
    [Serializable]
    public class DuplicateSectionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateSectionException"/> class.
        /// </summary>
        public DuplicateSectionException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateSectionException"/> class.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        public DuplicateSectionException(string sectionName)
            : base(DuplicateSectionException.ExceptionMessage(sectionName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateSectionException"/> class.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="innerException">The inner exception.</param>
        public DuplicateSectionException(string sectionName, Exception innerException)
            : base(DuplicateSectionException.ExceptionMessage(sectionName), innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateSectionException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected DuplicateSectionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        private static string ExceptionMessage(string sectionName)
        {
            return "Duplicate section name found: \"{0}\".".Arrange(sectionName);
        }
    }
}
