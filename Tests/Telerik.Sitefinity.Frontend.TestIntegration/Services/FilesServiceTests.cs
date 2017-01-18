using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using MbUnit.Framework;
using Telerik.Sitefinity.Frontend.Services.FilesService;
using Telerik.Sitefinity.Frontend.Services.FilesService.DTO;
using Telerik.Sitefinity.TestUtilities;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.TestIntegration.Services
{
    [TestFixture]
    [Category(TestCategories.Services)]
    [Author(TestAuthor.Team2)]
    [Description("Integration tests that ensure that the FilesService is operating properly.")]
    public class FilesServiceTests
    {
        #region Errors

        #region File Extension Errors

        [Test]
        [Description("Verifies that when called with no file extension proper error is returned")]
        public void PassingNoFileExtension_FilesService_ProperErrorReturned()
        {
            // Arrange
            var url = new Uri(UrlPath.ResolveAbsoluteUrl(FilesServiceTests.ServiceRootUrl));
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.CookieContainer = new CookieContainer();
            webRequest.Headers["Authorization"] = HttpContext.Current.Request.Headers["Authorization"];

            // Act
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            var responseString = (new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8)).ReadToEnd();
            var filesViewModel = Json.Decode<FilesViewModel>(responseString);

            // Assert
            Assert.IsTrue(filesViewModel != null, "The response is not a FilesViewModel");
            Assert.IsTrue(filesViewModel.Items == null, "There are items in the response");
            Assert.AreEqual(FilesWebServiceConstants.FileExtensionNullOrEmptyExceptionMessage, filesViewModel.Error, "When called with no file extension proper error is NOT returned");
        }

        [Test]
        [Description("Verifies that when called with unsupported file extension proper error is returned")]
        public void PassingUnsupportedFileExtension_FilesService_ProperErrorReturned()
        {
            // Arrange
            var fileFormat = "unsupported";
            var url = new Uri(UrlPath.ResolveAbsoluteUrl(FilesServiceTests.ServiceRootUrl + string.Format("?extension={0}", fileFormat)));
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.CookieContainer = new CookieContainer();
            webRequest.Headers["Authorization"] = HttpContext.Current.Request.Headers["Authorization"];

            // Act
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            var responseString = (new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8)).ReadToEnd();
            var filesViewModel = Json.Decode<FilesViewModel>(responseString);

            // Assert
            Assert.IsTrue(filesViewModel != null, "The response is not a FilesViewModel");
            Assert.IsTrue(filesViewModel.Items == null, "There are items in the response");
            Assert.AreEqual(string.Format(FilesWebServiceConstants.FileExtensionNotSupportedExceptionMessageFormat, fileFormat), filesViewModel.Error, "When called with unsupported file extension proper error is NOT returned");
        }

        #endregion

        #region Parent Path Errors

        [Test]
        [Description("Verifies that when called with parent path with forbidden symbol '..' proper error is returned")]
        public void PassingForbiddenSymbolDotsInParentPath_FilesService_ProperErrorReturned()
        {
            // Arrange
            var forbiddenSymbol = "..";
            var parentPath = "fakePath/" + forbiddenSymbol;
            var url = new Uri(UrlPath.ResolveAbsoluteUrl(FilesServiceTests.ServiceRootUrl + string.Format("?extension=css&path={0}", parentPath)));
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.CookieContainer = new CookieContainer();
            webRequest.Headers["Authorization"] = HttpContext.Current.Request.Headers["Authorization"];

            // Act
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            var responseString = (new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8)).ReadToEnd();
            var filesViewModel = Json.Decode<FilesViewModel>(responseString);

            // Assert
            Assert.IsTrue(filesViewModel != null, "The response is not a FilesViewModel");
            Assert.IsTrue(filesViewModel.Items == null, "There are items in the response");
            Assert.AreEqual(string.Format(FilesWebServiceConstants.ParentPathForbiddenSymbolInPathExceptionMessageFormat, parentPath, forbiddenSymbol), filesViewModel.Error, "When called with parent path with forbidden symbol '..' proper error is NOT returned");
        }

        [Test]
        [Description("Verifies that when called with parent path with forbidden symbol '~' proper error is returned")]
        public void PassingForbiddenSymbolWaveDashInParentPath_FilesService_ProperErrorReturned()
        {
            // Arrange
            var forbiddenSymbol = "~";
            var parentPath = "fakePath/" + forbiddenSymbol;
            var url = new Uri(UrlPath.ResolveAbsoluteUrl(FilesServiceTests.ServiceRootUrl + string.Format("?extension=css&path={0}", parentPath)));
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.CookieContainer = new CookieContainer();
            webRequest.Headers["Authorization"] = HttpContext.Current.Request.Headers["Authorization"];

            // Act
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            var responseString = (new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8)).ReadToEnd();
            var filesViewModel = Json.Decode<FilesViewModel>(responseString);

            // Assert
            Assert.IsTrue(filesViewModel != null, "The response is not a FilesViewModel");
            Assert.IsTrue(filesViewModel.Items == null, "There are items in the response");
            Assert.AreEqual(string.Format(FilesWebServiceConstants.ParentPathForbiddenSymbolInPathExceptionMessageFormat, parentPath, forbiddenSymbol), filesViewModel.Error, "When called with parent path with forbidden symbol '~' proper error is NOT returned");
        }

        [Test]
        [Description("Verifies that when called with parent path that does not exist proper error is returned")]
        public void PassingNotExistingParentPath_FilesService_ProperErrorReturned()
        {
            // Arrange
            var parentPath = "unexistingParentPath/";
            var url = new Uri(UrlPath.ResolveAbsoluteUrl(FilesServiceTests.ServiceRootUrl + string.Format("?extension=css&path={0}", parentPath)));
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.CookieContainer = new CookieContainer();
            webRequest.Headers["Authorization"] = HttpContext.Current.Request.Headers["Authorization"];

            // Act
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            var responseString = (new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8)).ReadToEnd();
            var filesViewModel = Json.Decode<FilesViewModel>(responseString);

            // Assert
            Assert.IsTrue(filesViewModel != null, "The response is not a FilesViewModel");
            Assert.IsTrue(filesViewModel.Items == null, "There are items in the response");
            Assert.AreEqual(string.Format(FilesWebServiceConstants.ParentPathNotExistingExceptionMessageFormat, parentPath), filesViewModel.Error, "When called with parent path that does not exist proper error is NOT returned");
        }

        #endregion

        #region Skip Take Errors

        [Test]
        [Description("Verifies that when called with negative skip value proper error is returned")]
        public void PassingNegativeSkipValue_FilesService_ProperErrorReturned()
        {
            // Arrange
            var skip = -1;
            var url = new Uri(UrlPath.ResolveAbsoluteUrl(FilesServiceTests.ServiceRootUrl + string.Format("?extension=css&skip={0}", skip)));
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.CookieContainer = new CookieContainer();
            webRequest.Headers["Authorization"] = HttpContext.Current.Request.Headers["Authorization"];

            // Act
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            var responseString = (new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8)).ReadToEnd();
            var filesViewModel = Json.Decode<FilesViewModel>(responseString);

            // Assert
            Assert.IsTrue(filesViewModel != null, "The response is not a FilesViewModel");
            Assert.IsTrue(filesViewModel.Items == null, "There are items in the response");
            Assert.AreEqual(FilesWebServiceConstants.FilesSkipNegativeValueExceptionMessage, filesViewModel.Error, "When called with negative skip value proper error is NOT returned");
        }

        [Test]
        [Description("Verifies that when called with more than maximum take value proper error is returned")]
        public void PassingMoreThanMaximumTakeValue_FilesService_ProperErrorReturned()
        {
            // Arrange
            var take = FilesWebServiceConstants.MaxItemsPerRequest + 1;
            var url = new Uri(UrlPath.ResolveAbsoluteUrl(FilesServiceTests.ServiceRootUrl + string.Format("?extension=css&take={0}", take)));
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.CookieContainer = new CookieContainer();
            webRequest.Headers["Authorization"] = HttpContext.Current.Request.Headers["Authorization"];

            // Act
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            var responseString = (new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8)).ReadToEnd();
            var filesViewModel = Json.Decode<FilesViewModel>(responseString);

            // Assert
            Assert.IsTrue(filesViewModel != null, "The response is not a FilesViewModel");
            Assert.IsTrue(filesViewModel.Items == null, "There are items in the response");
            Assert.AreEqual(string.Format(FilesWebServiceConstants.FilesTakeMaxLimitExceptionMessageFormat, FilesWebServiceConstants.MaxItemsPerRequest), filesViewModel.Error, "When called with more than maximum take value proper error is NOT returned");
        }

        [Test]
        [Description("Verifies that when called with negative take value proper error is returned")]
        public void PassingNegativeTakeValue_FilesService_ProperErrorReturned()
        {
            // Arrange
            var take = -1;
            var url = new Uri(UrlPath.ResolveAbsoluteUrl(FilesServiceTests.ServiceRootUrl + string.Format("?extension=css&take={0}", take)));
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.CookieContainer = new CookieContainer();
            webRequest.Headers["Authorization"] = HttpContext.Current.Request.Headers["Authorization"];

            // Act
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            var responseString = (new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8)).ReadToEnd();
            var filesViewModel = Json.Decode<FilesViewModel>(responseString);

            // Assert
            Assert.IsTrue(filesViewModel != null, "The response is not a FilesViewModel");
            Assert.IsTrue(filesViewModel.Items == null, "There are items in the response");
            Assert.AreEqual(FilesWebServiceConstants.FilesTakeNegativeValueExceptionMessage, filesViewModel.Error, "When called with negative take value proper error is NOT returned");
        }

        #endregion

        #endregion

        #region Files

        [Test]
        [Description("Verifies that when called with css file extension and no parent path proper items are returned")]
        public void PassingCssFileExtensionWithNoParentPath_FilesService_ProperItemsAreReturned()
        {
            // Arrange
            var fileExtension = "css";
            var url = new Uri(UrlPath.ResolveAbsoluteUrl(FilesServiceTests.ServiceRootUrl + string.Format("?extension={0}", fileExtension)));
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.CookieContainer = new CookieContainer();
            webRequest.Headers["Authorization"] = HttpContext.Current.Request.Headers["Authorization"];

            var tempDirName = string.Format("__tempDirectoryNameUsedForTest{0}", Guid.NewGuid());
            var tempDirPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, tempDirName);
            
            var tempFileName = string.Format("__tempFileNameUsedForTest{0}.{1}", Guid.NewGuid(), fileExtension);
            var tempFilePath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, tempFileName);

            // Act
            try
            {
                Directory.CreateDirectory(tempDirPath);
                File.Create(tempFilePath).Close();

                var webResponse = (HttpWebResponse)webRequest.GetResponse();
                var responseString = (new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8)).ReadToEnd();
                var filesViewModel = Json.Decode<FilesViewModel>(responseString);

                // Assert
                Assert.IsTrue(filesViewModel != null, "The response is not a FilesViewModel");
                Assert.IsTrue(string.IsNullOrEmpty(filesViewModel.Error), "There is an error in the response");
                Assert.IsTrue(filesViewModel.Items != null, "The items are null");
                Assert.IsTrue(filesViewModel.Items.Count() > 0, "There are no items");
                Assert.IsTrue(filesViewModel.Items.Any(i => i.Name == tempDirName && i.IsFolder), "When called with css file extension and no parent path fake directory is NOT returned");
                Assert.IsTrue(filesViewModel.Items.Any(i => i.Name == tempFileName && !i.IsFolder), "When called with css file extension and no parent path fake css file is NOT returned");
            }
            finally
            {
                File.Delete(tempFilePath);
                Directory.Delete(tempDirPath);
            }
        }

        [Test]
        [Description("Verifies that when called with js file extension and no parent path proper items are returned")]
        public void PassingJsFileExtensionWithNoParentPath_FilesService_ProperItemsAreReturned()
        {
            // Arrange
            var fileExtension = "js";
            var url = new Uri(UrlPath.ResolveAbsoluteUrl(FilesServiceTests.ServiceRootUrl + string.Format("?extension={0}", fileExtension)));
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.CookieContainer = new CookieContainer();
            webRequest.Headers["Authorization"] = HttpContext.Current.Request.Headers["Authorization"];

            var tempDirName = string.Format("__tempDirectoryNameUsedForTest{0}", Guid.NewGuid());
            var tempDirPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, tempDirName);

            var tempFileName = string.Format("__tempFileNameUsedForTest{0}.{1}", Guid.NewGuid(), fileExtension);
            var tempFilePath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, tempFileName);

            // Act
            try
            {
                Directory.CreateDirectory(tempDirPath);
                File.Create(tempFilePath).Close();

                var webResponse = (HttpWebResponse)webRequest.GetResponse();
                var responseString = (new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8)).ReadToEnd();
                var filesViewModel = Json.Decode<FilesViewModel>(responseString);

                // Assert
                Assert.IsTrue(filesViewModel != null, "The response is not a FilesViewModel");
                Assert.IsTrue(string.IsNullOrEmpty(filesViewModel.Error), "There is an error in the response");
                Assert.IsTrue(filesViewModel.Items != null, "The items are null");
                Assert.IsTrue(filesViewModel.Items.Count() > 0, "There are no items");
                Assert.IsTrue(filesViewModel.Items.Any(i => i.Name == tempDirName && i.IsFolder), "When called with js file extension and no parent path fake directory is NOT returned");
                Assert.IsTrue(filesViewModel.Items.Any(i => i.Name == tempFileName && !i.IsFolder), "When called with js file extension and no parent path fake css file is NOT returned");
            }
            finally
            {
                File.Delete(tempFilePath);
                Directory.Delete(tempDirPath);
            }
        }

        [Test]
        [Description("Verifies that when called with html file extension and no parent path proper items are returned")]
        public void PassingHtmlFileExtensionWithNoParentPath_FilesService_ProperItemsAreReturned()
        {
            // Arrange
            var fileExtension = "html";
            var url = new Uri(UrlPath.ResolveAbsoluteUrl(FilesServiceTests.ServiceRootUrl + string.Format("?extension={0}", fileExtension)));
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.CookieContainer = new CookieContainer();
            webRequest.Headers["Authorization"] = HttpContext.Current.Request.Headers["Authorization"];

            var tempDirName = string.Format("__tempDirectoryNameUsedForTest{0}", Guid.NewGuid());
            var tempDirPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, tempDirName);

            var tempFileName = string.Format("__tempFileNameUsedForTest{0}.{1}", Guid.NewGuid(), fileExtension);
            var tempFilePath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, tempFileName);

            // Act
            try
            {
                Directory.CreateDirectory(tempDirPath);
                File.Create(tempFilePath).Close();

                var webResponse = (HttpWebResponse)webRequest.GetResponse();
                var responseString = (new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8)).ReadToEnd();
                var filesViewModel = Json.Decode<FilesViewModel>(responseString);

                // Assert
                Assert.IsTrue(filesViewModel != null, "The response is not a FilesViewModel");
                Assert.IsTrue(string.IsNullOrEmpty(filesViewModel.Error), "There is an error in the response");
                Assert.IsTrue(filesViewModel.Items != null, "The items are null");
                Assert.IsTrue(filesViewModel.Items.Count() > 0, "There are no items");
                Assert.IsTrue(filesViewModel.Items.Any(i => i.Name == tempDirName && i.IsFolder), "When called with html file extension and no parent path fake directory is NOT returned");
                Assert.IsTrue(filesViewModel.Items.Any(i => i.Name == tempFileName && !i.IsFolder), "When called with html file extension and no parent path fake css file is NOT returned");
            }
            finally
            {
                File.Delete(tempFilePath);
                Directory.Delete(tempDirPath);
            }
        }

        [Test]
        [Description("Verifies that when called with css file extension and take 1 proper items are returned")]
        public void PassingCssFileExtensionWithNoParentPathAndOnlyTake1_FilesService_ProperItemsAreReturned()
        {
            // Arrange
            var fileExtension = "css";
            var url = new Uri(UrlPath.ResolveAbsoluteUrl(FilesServiceTests.ServiceRootUrl + string.Format("?extension={0}&take=1", fileExtension)));
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.CookieContainer = new CookieContainer();
            webRequest.Headers["Authorization"] = HttpContext.Current.Request.Headers["Authorization"];

            var tempDirName = string.Format("__tempDirectoryNameUsedForTest{0}", Guid.NewGuid());
            var tempDirPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, tempDirName);

            var tempFileName = string.Format("__tempFileNameUsedForTest{0}.{1}", Guid.NewGuid(), fileExtension);
            var tempFilePath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, tempFileName);

            // Act
            try
            {
                Directory.CreateDirectory(tempDirPath);
                File.Create(tempFilePath).Close();

                var webResponse = (HttpWebResponse)webRequest.GetResponse();
                var responseString = (new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8)).ReadToEnd();
                var filesViewModel = Json.Decode<FilesViewModel>(responseString);

                // Assert
                Assert.IsTrue(filesViewModel != null, "The response is not a FilesViewModel");
                Assert.IsTrue(string.IsNullOrEmpty(filesViewModel.Error), "There is an error in the response");
                Assert.IsTrue(filesViewModel.Items != null, "The items are null");
                Assert.AreEqual(1, filesViewModel.Items.Count(), "The items returned are not 1");
            }
            finally
            {
                File.Delete(tempFilePath);
                Directory.Delete(tempDirPath);
            }
        }

        [Test]
        [Description("Verifies that when called with css file extension and parent path proper items are returned")]
        public void PassingCssFileExtensionWithParentPath_FilesService_ProperItemsAreReturned()
        {
            // Arrange
            var fileExtension = "css";

            var tempDirName = string.Format("__tempDirectoryNameUsedForTest{0}", Guid.NewGuid());
            var tempDirPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, tempDirName);

            var tempFileName = string.Format("__tempFileNameUsedForTest{0}.{1}", Guid.NewGuid(), fileExtension);
            var tempFilePath = Path.Combine(tempDirPath, tempFileName);

            var url = new Uri(UrlPath.ResolveAbsoluteUrl(FilesServiceTests.ServiceRootUrl + string.Format("?extension={0}&path={1}", fileExtension, tempDirName)));
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.CookieContainer = new CookieContainer();
            webRequest.Headers["Authorization"] = HttpContext.Current.Request.Headers["Authorization"];

            // Act
            try
            {
                Directory.CreateDirectory(tempDirPath);
                File.Create(tempFilePath).Close();

                var webResponse = (HttpWebResponse)webRequest.GetResponse();
                var responseString = (new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8)).ReadToEnd();
                var filesViewModel = Json.Decode<FilesViewModel>(responseString);

                // Assert
                Assert.IsTrue(filesViewModel != null, "The response is not a FilesViewModel");
                Assert.IsTrue(string.IsNullOrEmpty(filesViewModel.Error), "There is an error in the response");
                Assert.IsTrue(filesViewModel.Items != null, "The items are null");
                Assert.AreEqual(1, filesViewModel.Items.Count(), "The items returned are not 1");
                Assert.AreEqual(tempFileName, filesViewModel.Items.FirstOrDefault().Name, "The item returned is not the real one");
            }
            finally
            {
                File.Delete(tempFilePath);
                Directory.Delete(tempDirPath);
            }
        }

        #endregion

        private const string ServiceRootUrl = "~/RestApi/files-api";
    }
}