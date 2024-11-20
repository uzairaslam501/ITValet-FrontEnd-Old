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
        [Route("troubleshoot-your-wifi-with-these-simple-fixes")]
        public IActionResult TroubleshootYourWifi()
        {
            return View();
        }

        [Route("why-your-internet-keep-dropping-and-how-to-fix-it")]
        public IActionResult WhyYourInternetKeepDropping()
        {
            return View();
        }
        [Route("computer-support")]
        public IActionResult ComputerSupport()
        {
            return View();
        }
        [Route("mobile-support")]
        public IActionResult MobileSupport()
        {
            return View();
        }
        [Route("tablet-assistance")]
        public IActionResult TabletAssistance()
        {
            return View();
        }
        [Route("internet-troubleshooting")]
        public IActionResult InternetTroubleshooting()
        {
            return View();
        } 
        [Route("printer-solutions")]
        public IActionResult PrinterSolutions()
        {
            return View();
        }
        [Route("website-support")]
        public IActionResult WebsiteSupport()
        {
            return View();
        }
        [Route("email-troubleshooting")]
        public IActionResult EmailSupport()
        {
            return View();
        }
        [Route("device-troubleshooting")]
        public IActionResult DeviceTroubleshooting()
        {
            return View();
        }
    }
}
