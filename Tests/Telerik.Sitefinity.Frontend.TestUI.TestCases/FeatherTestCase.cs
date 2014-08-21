using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.MS.TestUI.Framework.MSTest;
using Telerik.TestUI.Core.Configuration;

namespace Telerik.Sitefinity.Frontend.TestUI.TestCases
{
    /// <summary>
    /// This test case is responsible for initializing the ArtOfTest Manager.
    /// </summary>
    public class FeatherTestCase : SitefinityBaseTestCase
    {
        /// <summary>
        /// The method will be executed before methods marked with the ClassInitializeAttribute, TestInitializeAttribute, and TestMethodAttribute attributes. 
        /// </summary>
        /// <param name="context">The test context.</param>
        [AssemblyInitialize]
        public static void AssemblyInitalize(TestContext context)
        {
            AppConfig.TestContext = context;
           
            SitefinityBaseTestCase.InitializeManager();
            try
            {
                ArtOfTest.WebAii.Core.Manager.Current.ConfigureBrowser(ArtOfTest.WebAii.Core.BrowserType.InternetExplorer, true);
            }
            catch (Exception e)
            {
                context.WriteLine("There was an error while trying to configure IE browser. Message: {0}\r\nStackTrace: {1}", e.Message, e.StackTrace);
            }
        }

        /// <summary>
        /// This method will be executed after methods marked with the TestCleanupAttribute and the ClassCleanupAttribute attributes. 
        /// This will not execute if an unhandled exception is thrown.
        /// </summary>
        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            SitefinityBaseTestCase.DisposeManager();          
        }

        /// <summary>
        /// Forces calling initialize methods that will prepare test with data and resources. This method must be overridden if you want
        /// in your test case.
        /// </summary>
        protected override void ServerSetup()
        {
            base.ServerSetup();
        }

        /// <summary>
        /// Forces cleanup of the test data. This method is thrown if test setup fails. This method must be overridden in your test case.
        /// </summary>
        protected override void ServerCleanup()
        {
            base.ServerCleanup();
        }
    }
}