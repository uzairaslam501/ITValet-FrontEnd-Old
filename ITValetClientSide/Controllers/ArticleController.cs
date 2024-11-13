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
    }
}
