using Microsoft.AspNetCore.Mvc;

namespace ContentCreator.Web.Controllers
{
    public class ContentController : Controller
    {
        public IActionResult UploadContent()
        {
            return View();
        }
    }
}
