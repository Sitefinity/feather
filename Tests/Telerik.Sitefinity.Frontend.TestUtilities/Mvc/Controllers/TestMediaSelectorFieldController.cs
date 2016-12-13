using System.Web.Mvc;
using SitefinityWebApp.Mvc.Models;
using Telerik.Sitefinity.Mvc;

namespace SitefinityWebApp.Mvc.Controllers
{
    [ControllerToolboxItem(SectionName = "CustomMvcWidgets", Title = "Test media selector field for videos", Name = "TestMediaSelectorField")]
    public class TestMediaSelectorFieldController : Controller
    {
        public string SelectedMediaId { get; set; }

        public string SelectedMedia { get; set; }

        public string MediaProvider
        {
            get
            {
                return string.Empty;
            }
        }

        public ActionResult Index()
        {
            new TestMediaSelectorFieldModel
            {
                MediaProvider = this.MediaProvider,
                SelectedMedia = this.SelectedMedia,
                SelectedMediaId = this.SelectedMediaId
            };

            return this.Content("This is the media field widget.");
        }
    }
}