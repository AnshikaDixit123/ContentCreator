using Microsoft.AspNetCore.Mvc;

namespace ContentCreator.Web.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult SecuredLogin()
        {
            return View();
        }
        public IActionResult EndUserLogin()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }
    }
}
