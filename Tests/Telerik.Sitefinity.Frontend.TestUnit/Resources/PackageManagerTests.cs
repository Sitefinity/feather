using System;
using System.Web;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.TestUnit.Resources
{
    /// <summary>
    /// Ensures that PackageManager class works correctly.
    /// </summary>
    [TestClass]
    public class PackageManagerTests
    {
        #region Public Methods and Operators

        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether the PackageManager properly appends the package parameter to a given URL.")]
        public void AppendPackageParam_GivenUrl_VerifyThePackageNameIsAppendedCorrectly()
        {
            // Arrange: Create variables holding the package name and fake URL to which the package will be appended
            var urlWithParamters = @"http://fakedomain.org/homePage?fakeParam=0";
            var urlWithNoParamters = @"http://fakedomain.org/homePage";
            var packageName = "fakePackageName";

            string appendedUrhWithParams;
            string appendedUrWithNoParams;
            string appendedUrlWithEmptyPackagename;

            // Act: append the package name
            appendedUrlWithEmptyPackagename = UrlTransformations.AppendParam(urlWithNoParamters, PackageManager.PackageUrlParameterName, null);
            appendedUrhWithParams = UrlTransformations.AppendParam(urlWithParamters, PackageManager.PackageUrlParameterName, packageName);
            appendedUrWithNoParams = UrlTransformations.AppendParam(urlWithNoParamters, PackageManager.PackageUrlParameterName, packageName);

            // Assert: Verify the package name is properly appended
            Assert.AreEqual(urlWithNoParamters, appendedUrlWithEmptyPackagename, "The URL must not be changed due to empty package name passed as parameter");
            Assert.AreEqual(string.Format(System.Globalization.CultureInfo.InvariantCulture, "http://fakedomain.org/homePage?fakeParam=0&package={0}", packageName), appendedUrhWithParams, "The package name was not appended correctly as a second parameter");
            Assert.AreEqual(string.Format(System.Globalization.CultureInfo.InvariantCulture, "http://fakedomain.org/homePage?package={0}", packageName), appendedUrWithNoParams, "The package name was not appended correctly as a parameter");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether EnhanceUrl method will append the package url param to a given url.")]
        public void EnhanceUrl_HasPackage_AppendsPackageUrlParam()
        {
            // Arrange: Initialize the PackageManager and create fake HttpContextWrapper which has fake request URL with the package name set as query parameter.
            string url = "http://mysite/";
            string expectedEnhancedUrl = url + "?package=testPackageName";

            var context =
                new HttpContextWrapper(
                    new HttpContext(
                        new HttpRequest(null, "http://tempuri.org", "package=testPackageName"), 
                        new HttpResponse(null)));

            // Act: Get the enhanced URL from the package manager.
            SystemManager.RunWithHttpContext(context, () => { url = new PackageManager().EnhanceUrl(url); });

            // Assert: Verify if the manager appends package param.
            Assert.AreEqual(expectedEnhancedUrl, url, "The URL does not contain the package name.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether EnhanceUrl will not modify a given URL when there is no package.")]
        public void EnhanceUrl_NoPackage_AppendsPackageUrlParam()
        {
            // Arrange
            var url = "http://mysite/";
            var expectedEnhancedUrl = url;

            // Act
            url = new PackageManager().EnhanceUrl(url);

            // Assert
            Assert.AreEqual(expectedEnhancedUrl, url, "The URL was modified.");
        }

        [TestMethod]
        [Owner("Bonchev")]
        [Description(
            "Checks whether the GetCurrentPackage extracts properly the currernt package name form the HttpContext.")]
        public void GetCurrentPackage_FakeContext_VerifyThePackageNameIsCorrect()
        {
            // Arrange: Initialize the PackageManager and create fake HttpContextWrapper which has fake package name set as parameter in its parameters collection
            var packageManager = new PackageManager();
            var packageName = string.Empty;

            var context = new HttpContextWrapper(new HttpContext(new HttpRequest(null, "http://tempuri.org", null), new HttpResponse(null)));
            context.Items[PackageManager.CurrentPackageKey] = "testPackageName";

            // Act:  Get the package name from the request parameters collection 
            SystemManager.RunWithHttpContext(context, () => { packageName = packageManager.GetCurrentPackage(); });

            // Assert: Verify if the manager properly strips all invalid characters
            Assert.AreEqual("testPackageName", packageName, "The package name was not resolved correctly");
        }

        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether the PackageManager properly gets a package name from the request URL query string.")]
        public void GetPackageFromUrl_FakeCurrentUrlInHttpContext_VerifyThePackageNameIsCorrect()
        {
            // Arrange: Initialize the PackageManager and create fake HttpContextWrapper which has fake request URL with the package name set as query parameter
            var packageManager = new PackageManager();
            var packageName = string.Empty;

            var context =
                new HttpContextWrapper(
                    new HttpContext(
                        new HttpRequest(null, "http://tempuri.org", "package=testPackageName"), 
                        new HttpResponse(null)));

            // Act: Get the package name from the request URL query string 
            SystemManager.RunWithHttpContext(context, () => { packageName = packageManager.GetCurrentPackage(); });

            // Assert: Verify if the manager properly strips all invalid characters
            Assert.AreEqual("testPackageName", packageName, "The package name was not resolved correctly");
        }

        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether the PackageManager properly return the virtual path of a given package")]
        public void GetPackageVirtualPath_GivenPackage_VerifyTheVirtualPathIsCorrect()
        {
            // Arrange: Initialize the PackageManager and a fake package name
            var packageManager = new PackageManager();
            var packageName = "fakePackageName";
            string packageVirtualpath;

            // Act: gets the package virtual path
            packageVirtualpath = packageManager.GetPackageVirtualPath(packageName);

            // Assert: Verify if the manager throws an error if the parameter is null and if the package virtual path is correct
            try
            {
                packageManager.GetPackageVirtualPath(null);
                Assert.Fail("Expected exception was not thrown");
            }
            catch (ArgumentNullException)
            {
                Assert.AreEqual(string.Format(System.Globalization.CultureInfo.InvariantCulture, "~/{0}/{1}", PackageManager.PackagesFolder, packageName), packageVirtualpath, "Package virtual path is not correct");
            }
        }

        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether the PackageManager properly strips all invalid chars and replace them with a proper substitute")]
        public void StripInvalidCharacters_TitleWithInvalidCharacters_VerifyStringIsProperlyInvalidated()
        {
            // Arrange: Initialize the PackageManager and a fake package name
            var packageManager = new PackageManager();
            string title = "fake\\/Title<Name>With:Invalid?Chars\"And*Symbols|Included";
            string cleanedTitle;

            // Act: clean the title
            cleanedTitle = packageManager.StripInvalidCharacters(title);

            // Assert: Verify if the manager properly strips all invalid characters
            Assert.AreEqual("fake_Title_Name_With_Invalid_Chars_And_Symbols_Included", cleanedTitle, "Title is not striped correctly");
        }

        #endregion
    }
}