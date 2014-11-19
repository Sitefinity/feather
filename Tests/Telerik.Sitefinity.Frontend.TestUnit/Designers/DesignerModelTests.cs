using System;
using System.Collections.Generic;
using System.Linq;

using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Abstractions.VirtualPath.Configuration;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Configuration.Data;
using Telerik.Sitefinity.Frontend.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.Mvc.Models;
using Telerik.Sitefinity.Frontend.Test.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Configs;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Models;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.ResourceResolvers;
using Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers;
using Telerik.Sitefinity.Localization.Configuration;
using Telerik.Sitefinity.Web.Configuration;

namespace Telerik.Sitefinity.Frontend.TestUnit.Designers
{
    /// <summary>
    /// These tests are meant to ensure that the DesignerModel class is working correctly.
    /// </summary>
    [TestClass]
    public class DesignerModelTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The extract view name_ correct file name_ extracts view name.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether ExtractViewName method returns the correct view name.")]
        public void ExtractViewName_CorrectFileName_ExtractsViewName()
        {
            // Arrange
            var dummyModel = new DummyDesignerModel();
            string fileName = "DesignerView.someViewName.cshtml";

            // Act
            string viewName = dummyModel.ExtractViewNamePublic(fileName);

            // Assert
            Assert.AreEqual("someViewName", viewName, "The viewName is not extracted correctly.");
        }

        /// <summary>
        /// The extract view name_ full file name_ extracts view name.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description(
            "Checks whether ExtractViewName method returns the correct view name from file name which contains'.'.")]
        public void ExtractViewName_FullFileName_ExtractsViewName()
        {
            // Arrange
            var dummyModel = new DummyDesignerModel();
            string fileName = "DesignerView.Property.Grid.cshtml";

            // Act
            string viewName = dummyModel.ExtractViewNamePublic(fileName);

            // Assert
            Assert.AreEqual("Property.Grid", viewName, "The viewName is not extracted correctly.");
        }

        /// <summary>
        /// The get view script file name_ view name with dot_ construct script name.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether GetViewScriptFileName method constructs correct script name from view name which contains'.'.")]
        public void GetViewScriptFileName_ViewNameWithDot_ConstructScriptName()
        {
            // Arrange
            var dummyModel = new DummyDesignerModel();
            var viewName = "Property.Grid";

            // Act
            string scriptName = dummyModel.GetViewScriptFileNamePublic(viewName);

            // Assert
            Assert.AreEqual("designerview-property-grid.js", scriptName, "The script name is not constructed correctly.");
        }

        /// <summary>
        /// The is designer view_ designer prefix_ returns true.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether IsDesignerView method returns true when starting with the designer prefix.")]
        public void IsDesignerView_DesignerPrefix_ReturnsTrue()
        {
            // Arrange
            var dummyModel = new DummyDesignerModel();
            var fileName = "DesignerView.someViewName";

            // Act
            var isDesignerView = dummyModel.IsDesignerViewPublic(fileName);

            // Assert
            Assert.IsTrue(isDesignerView, "The fileName is not recognized as valid designer view");
        }

        /// <summary>
        /// The populate script references_ empty views collection_ construct script refences collection.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether PopulateScriptReferences method populates script collection correctly when Views collection is empty.")]
        public void PopulateScriptReferences_EmptyViewsCollection_ConstructScriptReferencesCollection()
        {
            // Arrange
            var dummyModel = new DummyDesignerModel();
            var widgetName = "Dummy";
            var designerViewConfigs = this.CreateDummyDesignerViewConfigModel();

            // Act
            dummyModel.PopulateScriptReferencesPublic(widgetName, designerViewConfigs);

            // Assert
            Assert.AreEqual(3, dummyModel.ScriptReferences.Count(), "The script count is not as expected.");
            Assert.IsTrue(dummyModel.ScriptReferences.Contains(Script1), "ScriptReferences doesn't contain expected scripts.");
            Assert.IsTrue(dummyModel.ScriptReferences.Contains(Script2), "ScriptReferences doesn't contain expected scripts.");
            Assert.IsTrue(dummyModel.ScriptReferences.Contains(Script3), "ScriptReferences doesn't contain expected scripts.");
        }

        /// <summary>
        /// The populate script references_ views collection_ construct script refences collection.
        /// </summary>
        [TestMethod]
        [Owner("EGaneva")]
        [Description("Checks whether PopulateScriptReferences method populates script collection correctly when Views collection is empty.")]
        public void PopulateScriptReferences_ViewsCollection_ConstructScriptReferencesCollection()
        {
            // TODO: Reduce class coupling
            // Arrange
            string widgetName = "Dummy";
            var views = new List<string>();
            views.Add("DesignerView.someViewName");
            var designerViewConfigs = this.CreateDummyDesignerViewConfigModel();

            using (var objFactory = new ObjectFactoryContainerRegion())
            {
                ObjectFactory.Container.RegisterType<ConfigManager, ConfigManager>(typeof(XmlConfigProvider).Name.ToUpperInvariant(), new InjectionConstructor(typeof(XmlConfigProvider).Name));
                ObjectFactory.Container.RegisterType<XmlConfigProvider, DummyConfigProvider>();
                Config.RegisterSection<VirtualPathSettingsConfig>();
                Config.RegisterSection<ControlsConfig>();
                Config.RegisterSection<ResourcesConfig>();

                using (var factoryReg = new ControllerFactoryRegion<DummyControllerFactory>())
                {
                    factoryReg.Factory.ControllerRegistry["Dummy"] = typeof(DummyController);
                    factoryReg.Factory.ControllerRegistry["Designer"] = typeof(DesignerController);

                    var fileResolverPrefix = "~/Frontend-Assembly";
                    VirtualPathManager.AddVirtualFileResolver<DummyVirtualFileResolver>(fileResolverPrefix + "*", "DummyVirtualFileResolver");

                    var dummyModel = new DummyDesignerModel(views, new List<string>(), widgetName, Guid.Empty, "someViewName");

                    try
                    {
                        // Act
                        dummyModel.PopulateScriptReferencesPublic(widgetName, designerViewConfigs);
                    }
                    finally
                    {
                        VirtualPathManager.RemoveVirtualFileResolver(fileResolverPrefix);
                        VirtualPathManager.Reset();
                    }

                    // Assert
                    Assert.AreEqual(4, dummyModel.ScriptReferences.Count(), "The script count is not as expected.");
                    Assert.IsTrue(dummyModel.ScriptReferences.Contains("Mvc/Scripts/Dummy/designerview-someviewname.js"), "ScriptReferences doesn't contain scripts for the view.");
                    Assert.IsTrue(dummyModel.ScriptReferences.Contains(Script1), "ScriptReferences doesn't contain expected scripts.");
                    Assert.IsTrue(dummyModel.ScriptReferences.Contains(Script2), "ScriptReferences doesn't contain expected scripts.");
                    Assert.IsTrue(dummyModel.ScriptReferences.Contains(Script3), "ScriptReferences doesn't contain expected scripts.");
                }
            }
        }

        #endregion

        #region Methods

        private List<KeyValuePair<string, DesignerViewConfigModel>> CreateDummyDesignerViewConfigModel()
        {
            var designerViewConfigs = new List<KeyValuePair<string, DesignerViewConfigModel>>();

            var designerConfig1 = new DesignerViewConfigModel();
            designerConfig1.Hidden = true;
            designerConfig1.Priority = 5;
            designerConfig1.Scripts = new List<string> { Script1 };
            designerViewConfigs.Add(new KeyValuePair<string, DesignerViewConfigModel>("View1", designerConfig1));

            var designerConfig2 = new DesignerViewConfigModel();
            designerConfig2.Hidden = true;
            designerConfig2.Priority = 1;
            designerConfig2.Scripts = new List<string> { Script2, Script3, };
            designerViewConfigs.Add(new KeyValuePair<string, DesignerViewConfigModel>("View2", designerConfig2));

            return designerViewConfigs;
        }

        #endregion

        #region Constants

        private const string Script1 = "Mvc/Scripts/Dummy/config-script1.js";
        private const string Script2 = "Mvc/Scripts/Dummy/config-script2-1.js";
        private const string Script3 = "Mvc/Scripts/Dummy/config-script2-2.js";

        #endregion
    }
}