namespace Telerik.Sitefinity.Frontend.Services.FilesService
{
    internal static class FilesWebServiceConstants
    {
        public const string FileExtensionNullOrEmptyExceptionMessage = "File extension can not be null or empty.";
        public const string FileExtensionNotSupportedExceptionMessageFormat = "File extension {0} is not supported.";

        public const string ParentPathForbiddenSymbolInPathExceptionMessageFormat = "Parent path {0} contains forbidden symbols '{1}' .";
        public const string ParentPathNotExistingExceptionMessageFormat = "Parent path {0} does not exist.";

        public const string FilesTakeMaxLimitExceptionMessageFormat = "Can not request more than {0} items.";
        public const string FilesTakeNegativeValueExceptionMessage = "Can not request to take less than 0 items.";
        public const string FilesSkipNegativeValueExceptionMessage = "Can not request to skip less than 0 items.";

        public const int MaxItemsPerRequest = 50;
    }
}
