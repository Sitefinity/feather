using System;
using ArtOfTest.WebAii.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.TestUI.Framework;
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
                string error = String.Empty;
                Manager.Current.ConfigureBrowser(BrowserType.InternetExplorer, out error);
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

        /// <summary>
        /// Gets a value indicating whether the site is in multisite.
        /// </summary>
        /// <value>True if in multisite, false if in single site.</value>
        protected bool IsMultisite
        {
            get
            {
                if (this.isMultisite == null)
                {
                    this.isMultisite = BAT.Utilities().CheckIsMultisiteMode();
                }

                return (bool)this.isMultisite;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current site is in multilingual.
        /// </summary>
        /// <value>True if in multilingual, false if in monolingual.</value>
        protected bool IsMultilingual
        {
            get
            {
                if (this.isMultilingual == null)
                {
                    this.isMultilingual = BAT.Utilities().IsCurrentSiteInMultilingual();
                }

                return (bool)this.isMultilingual;
            }
        }

        /// <summary>
        /// Gets the culture.
        /// </summary>
        /// <value>The culture.</value>
        protected virtual string Culture
        {
            get
            {
                if (this.culture == "undefined")
                {
                    if (this.IsMultilingual)
                        this.culture = BAT.Utilities().GetDefaultArrangementCulture();
                    else
                        this.culture = null;
                }

                return this.culture;
            }
        }

        private bool? isMultisite = null;
        private bool? isMultilingual = null;
        private string culture = "undefined";
        protected const string AdminEmail = "admin@test.test";
        protected const string AdminPassword = "admin@2";
        protected const string AdminNickname = "admin admin";
    }
}