using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Frontend.Resources;

namespace Telerik.Sitefinity.Frontend.Test.Resources
{
    /// <summary>
    /// Ensures that PackagesManager class works correctly.
    /// </summary>
    [TestClass]
    public class PackagesManagerTests
    {
        #region AppendPackageParam

        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether the PackagesManager properly appends the package parameter to a given URL.")]
        public void AppendPackageParam_GivenUrl_VerifyThePackageNameIsAppendedCorrectly()
        {
            //Arrange: Create variables holding the package name and fake URL to which the package will be appended
            string urlWithParamters = @"http://fakedomain.org/homePage?fakeParam=0";
            string urlWithNoParamters = @"http://fakedomain.org/homePage";
            string packageName = "fakePackageName";
            string appendedUrhWithParams;
            string appendedUrWithNoParams;
            string appendedUrlWithEmptyPackagename;


            //Act: append the package name
            appendedUrlWithEmptyPackagename = UrlTransformations.AppendParam(urlWithNoParamters, PackagesManager.PackageUrlParamterName, null);
            appendedUrhWithParams = UrlTransformations.AppendParam(urlWithParamters, PackagesManager.PackageUrlParamterName, packageName);
            appendedUrWithNoParams = UrlTransformations.AppendParam(urlWithNoParamters, PackagesManager.PackageUrlParamterName, packageName);


            //Assert: Verify the package name is properly appended
            Assert.AreEqual<string>(urlWithNoParamters, appendedUrlWithEmptyPackagename, "The URL must not be changed due to empty package name passed as parameter");
            Assert.AreEqual<string>(string.Format("http://fakedomain.org/homePage?fakeParam=0&package={0}", packageName), appendedUrhWithParams, "The package name was not appended correctly as a second parameter");
            Assert.AreEqual<string>(string.Format("http://fakedomain.org/homePage?package={0}", packageName), appendedUrWithNoParams, "The package name was not appended correctly as a parameter");
        }

        #endregion

        #region GetPackageVirtualPath

        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether the PackagesManager properly return the virtual path of a given package")]
        public void GetPackageVirtualPath_GivenPackage_VerifyTheVirtualPathIsCorrect()
        {
            //Arrange: Initialize the PackagesManager and a fake package name
            var packageManager = new PackagesManager();
            string packageName = "fakePackageName";
            string packageVirtualpath;

            //Act: gets the package virtual path
            packageVirtualpath = packageManager.GetPackageVirtualPath(packageName);

            //Assert: Verify if the manager throws an error if the parameter is null and if the package virtual path is correct
            try
            {
                packageManager.GetPackageVirtualPath(null);
                Assert.Fail("Expected exception was not thrown");
            }
            catch
            {
            }

            Assert.AreEqual<string>(string.Format("~/{0}/{1}", PackagesManager.PackagesFolder, packageName), packageVirtualpath, "Package virtual path is not correct");
        }

        #endregion

        #region StripInvalidCharacters

        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether the PackagesManager properly strips all invalid chars and replace them with a proper substitute")]
        public void StripInvalidCharacters_TitleWithInvalidCharacters_VerifyStringIsProperlyInvalidated()
        {
            //Arrange: Initialize the PackagesManager and a fake package name
            var packageManager = new PackagesManager();
            string title = "fake\\/Title<Name>With:Invalid?Chars\"And*Symbols|Included";
            string cleanedTitle;

            //Act: clean the title
            cleanedTitle = packageManager.StripInvalidCharacters(title);

            //Assert: Verify if the manager properly strips all invalid characters
            Assert.AreEqual<string>("fake_Title_Name_With_Invalid_Chars_And_Symbols_Included", cleanedTitle, "Title is not striped correctly");
        }

        #endregion

        #region GetCurrentPackage

        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether the GetCurrentPackage extracts properly the currernt package name form the HttpContext.")]
        public void GetCurrentPackage_FakeContext_VerifyThePackageNameIsCorrect()
        {
            //Arrange: Initialize the PackagesManager and create fake HttpContextWrapper which has fake package name set as parameter in its parameters collection
            var packageManager = new PackagesManager();
            string packageName = string.Empty;

            var context = new HttpContextWrapper(new HttpContext(
                new HttpRequest(null, "http://tempuri.org", null),
                new HttpResponse(null)));
            context.Items[PackagesManager.CurrentPackageKey] = "testPackageName";

            //Act:  Get the package name from the request parameters collection 
            SystemManager.RunWithHttpContext(context, () =>
            {
                packageName = packageManager.GetCurrentPackage();
            });

            //Assert: Verify if the manager properly strips all invalid characters
            Assert.AreEqual<string>("testPackageName", packageName, "The package name was not resolved correctly");
        }

        #endregion

        #region GetPackageFromUrl

        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether the PackagesManager properly gets a package name from the request URL query string")]
        public void GetPackageFromUrl_FakeCurrentUrlInHttpContext_VerifyThePackageNameIsCorrect()
        {
            //Arrange: Initialize the PackagesManager and create fake HttpContextWrapper which has fake request URL with the package name set as query parameter
            var packageManager = new PackagesManager();
            string packageName = string.Empty;

            var context = new HttpContextWrapper(new HttpContext(
                new HttpRequest(null, "http://tempuri.org", "package=testPackageName"),
                new HttpResponse(null)));

            //Act: Get the package name from the request URL query string 
            SystemManager.RunWithHttpContext(context, () =>
            {
                packageName = packageManager.GetPackageFromUrl();
            });

            //Assert: Verify if the manager properly strips all invalid characters
            Assert.AreEqual<string>("testPackageName", packageName, "The package name was not resolved correctly");
        }

        #endregion
    }
}
