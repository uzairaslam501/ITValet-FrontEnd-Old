using Microsoft.AspNetCore.Mvc;

namespace ITValetFrontEnd.Controllers
{
    public class ArticleController : Controller
    {
        [Route("why-is-my-computer-so-slow")]
        public IActionResult Index()
        {
            return View();
        }
        [Route("why-is-my-mobile-so-slow")]
        public IActionResult whyismymobilesoslow()
        {
            return View();
        }
        [Route("turbo-charge-your-android-tablet-with-these-10-simple-fixes")]
        public IActionResult turbochargeyourandroidtablet()
        {
            return View();
        }
        [Route("guides")]
        public IActionResult Guides()
        {
            return View();
        }
        [Route("master-your-printer-troubles-with-these-solutions")]
        public IActionResult MasterYourPrinter()
        {
            return View();
        }
    }
}
