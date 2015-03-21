using System.Collections.Generic;
using System.Linq;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Frontend.GridSystem;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Configs;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.GridSystem;
using Telerik.Sitefinity.Modules.Pages.Configuration;

namespace Telerik.Sitefinity.Frontend.TestUnit.GridSystem
{
    /// <summary>
    /// Tests related to the GridWidgetRegistrator class
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Registrator"), TestClass]
    public class GridWidgetRegistratorTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The add layout control_ existing toolbox section_ verify control is properly added to the toolbox.
        /// </summary>
        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether the GridWidgetRegistrator properly adds a new set of layout controls to the toolbox")]
        public void AddLayoutControl_ExistingToolboxSection_VerifyControlIsProperlyAddedToTheToolbox()
        {
            // Arrange: Initialize the GridWidgetRegistrator, ToolboxesConfig and add a PageLayouts section, create a layouts section, create a dummy grid controls 
            var initializer = new DummyGridWidgetRegistrator();
            var fakeTemplatePath = "~/GridSystem/Templates/grid1.html";
            var dummyData = initializer.PublicCreateGridControlsData(fakeTemplatePath);
            var toolboxesConfig = new DummyToolboxesConfig();
            var pageControlsMock = new Toolbox(toolboxesConfig.Toolboxes);
            pageControlsMock.Name = "PageLayouts";
            toolboxesConfig.Toolboxes.Add("PageLayouts", pageControlsMock);
            var htmlLayoutsSection = initializer.PublicCreateToolBoxSection(toolboxesConfig, "BootstrapGrids", "BootstrapGridWidgets");
            ConfigElementList<ToolboxItem> parentToolboxItem = htmlLayoutsSection.Tools;

            // Act: add the grid controls to the toolbox
            initializer.PublicAddGridControl(parentToolboxItem, dummyData);

            // Assert: Verify the newly created controls are properly created
            Assert.AreEqual(1, parentToolboxItem.Count, "The grid controls were added to the toolbox.");

            var oneColumnGridToolboxItem = parentToolboxItem.Where<ToolboxItem>(toolboxItem => toolboxItem.Name == dummyData.Name).FirstOrDefault();

            Assert.IsNotNull(oneColumnGridToolboxItem, "The grid control was not added to the toolbox.");
            Assert.AreEqual(dummyData.Title, oneColumnGridToolboxItem.Title, "The grid control toolbox item has wrong title.");
            Assert.AreEqual(dummyData.LayoutTemplatePath, oneColumnGridToolboxItem.LayoutTemplate, "The grid control toolbox item has layout template path.");
        }

        /// <summary>
        /// The get or create tool box section_ create fake toolbox section_ verify section is added and has proper data.
        /// </summary>
        [TestMethod]
        [Owner("Bonchev")]
        [Description("Checks whether the GridWidgetRegistrator properly creates a new section for the layout controls")]
        public void GetOrCreateToolBoxSection_CreateFakeToolboxSection_VerifySectionIsAddedAndHasProperData()
        {
            // Arrange: Initialize the GridWidgetRegistrator, ToolboxesConfig and add a PageLayouts section
            var registrator = new DummyGridWidgetRegistrator();
            var toolboxesConfig = new DummyToolboxesConfig();
            var pageControlsMock = new Toolbox(toolboxesConfig.Toolboxes);
            var sectionName = "BootstrapGrids";
            var sectionTitle = "Bootstrap grid widgets";

            pageControlsMock.Name = "PageLayouts";
            toolboxesConfig.Toolboxes.Add("PageLayouts", pageControlsMock);

            // Act: create a new toolbox section for the layout controls
            var htmlLayoutsSection = registrator.PublicCreateToolBoxSection(toolboxesConfig, sectionName, sectionTitle);

            // Assert: Verify the newly created section exists and has proper name and title
            Assert.IsNotNull(htmlLayoutsSection, "The toolbox section was not properly created.");
            Assert.AreEqual(sectionName, htmlLayoutsSection.Name, "The toolbox section has unexpected name.");
            Assert.AreEqual(sectionTitle, htmlLayoutsSection.Title, "The toolbox section has unexpected title.");
        }

        #endregion
    }
}