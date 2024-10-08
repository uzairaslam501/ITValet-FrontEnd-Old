using ITValetFrontEnd.HelperClasses;
using ITValetFrontEnd.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using System.Drawing;
using ITValetFrontEnd.ClientSideServices;

namespace ITValetFrontEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GeneralHelper _helper;
        HttpClient _httpClient = new HttpClient();
        private readonly string _baseUrl;
        private readonly IUserService _userService;

        public HomeController(ILogger<HomeController> logger, GeneralHelper helper, IUserService userService)
        {
            _logger = logger;
            _helper = helper;
            _baseUrl = ProjectVariables.BaseUrl;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Welcome()
        {
            return View();
        }
        
        public async Task<IActionResult> Index(string message = "", string color = "")
        {
            var getLoggedInUser = await _helper.ValidateUserAsync();
            var loggedInUserRole = "";
            if (getLoggedInUser != null)
            {
                var response = await _userService.GetRankedKeyWords(getLoggedInUser.Token);
                if (response.Status == true)
                {
                    var popularSearched = JsonConvert.DeserializeObject<List<string?>>(response.Data.ToString());
                    ViewBag.PopularSearches = popularSearched;
                }

                loggedInUserRole = getLoggedInUser.Role;
                var getUsersResponse = await _userService.GetUsersList(getLoggedInUser.Token);
                if (getUsersResponse.StatusCode == "200" && getUsersResponse.Status == true)
                {
                    var recentUsers = JsonConvert.DeserializeObject<List<UserListDto>>(getUsersResponse.Data.ToString());
                    ViewBag.recentUsers = recentUsers;
                }
                else
                {
                    ViewBag.recentUsers = null;
                }
            }
            else
            {
                ViewBag.recentUsers = null;
            }

            ViewBag.LoggedInUseRole = loggedInUserRole;
            ViewBag.Message = message;
            ViewBag.Color = color;
            return View();
        }

        public async Task<IActionResult> ScheduledTask()
        {
            var loggedInUser = await _helper.ValidateUserAsync();
            if (loggedInUser != null)
            {
                ViewBag.UserId = loggedInUser.Id.ToString();
                return View();
            }
            return RedirectToAction("UserLogin", "Auth");
        }

        public async Task<IActionResult> Messages(string? Way = "", string? UserId = "", string? color = "", string message = "")
        {
            var getLoggedInUserId = await _helper.ValidateUserAsync();
            if (getLoggedInUserId == null)
            {
                return RedirectToAction("UserLogin", "Auth");
            }
            ViewBag.LoggedInUserId = getLoggedInUserId;
            ViewBag.Way = string.IsNullOrEmpty(Way) ? "" : Way;
            ViewBag.UserId = string.IsNullOrEmpty(UserId) ? "" : UserId;
            ViewBag.Message = message;
            ViewBag.Color = color;
            return View();
        }

        public async Task<ActionResult> SystemPackages(string msg = "")
        {
            var user = await _helper.ValidateUserAsync();
            if (user == null)
            {
                return RedirectToAction("UserLogin", "Auth", new { msg = "Session Expired, Logged In Again", color = ProjectVariables.DangerColor });
            }
            if (user != null)
            {
                string idAsString = user.Id.ToString();
                string apiEndpoint = $"User/GetUserSessionStatus?customerId={idAsString}";

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(user.Token);
                var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);
                var responseObj = await response.Content.ReadFromJsonAsync<ResponseDto>();
                if (responseObj.Status == true)
                {
                    ViewBag.IsSession = true;
                    ViewBag.SessionCount = responseObj.Data;
                }
                else
                {
                    ViewBag.IsSession = false;
                    ViewBag.SessionCount = 0;
                }
                ViewBag.UserName = user.UserName;
                ViewBag.Id = user.Id;
            }
            ViewBag.message = msg;
            return View();

        }

        #region Search
        public async Task<IActionResult> Search(string searchKeyword)
        {
            var user = await _helper.ValidateUserAsync();
            if (user != null)
            {
                if (string.IsNullOrEmpty(searchKeyword))
                {
                    return RedirectToAction("Index");
                }
                searchKeyword = searchKeyword.Trim();
                var searchResponse = await _userService.GetValetsBySearchKey(searchKeyword, user.Token);
                if (searchResponse.Status == true)
                {
                    var userRecord = JsonConvert.DeserializeObject<List<SearchedUserList>>(searchResponse.Data.ToString());
                    ViewBag.SearchedUserList = userRecord;
                    ViewBag.Search = searchKeyword;
                    return View();
                }
                else
                {
                    return RedirectToAction("SearchNotFound", new { searchKey = searchKeyword });
                }
            }
            return RedirectToAction("UserLogin", "Auth", new { msg = "Session Expired, Logged In Again", color = ProjectVariables.DangerColor });
        }

        public IActionResult SearchNotFound(string searchKey)
        {
            ViewBag.Search = searchKey;
            return View();
        }
        #endregion

        #region ContactUs
        public IActionResult Contact(string? message = "", string? color = "")
        {
            ViewBag.Message = message;
            ViewBag.Color = color;
            return View();
        }

        public async Task<IActionResult> PostFeedback(PostAddContact contact)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ProjectVariables.BaseUrl);
            string jsonContact = JsonConvert.SerializeObject(contact);
            StringContent content = new StringContent(jsonContact, Encoding.UTF8, "application/json");
            string endpoint = "Contact/PostAddContact";
            using (var Response = await httpClient.PostAsync(endpoint, content))
            {
                var response = await Response.Content.ReadFromJsonAsync<ResponseDto>();
                if (response != null && response.Status != false)
                {
                    return RedirectToAction("Contact", new { message = response.Message, color = ProjectVariables.SuccessColor });
                }
                else
                {
                    return RedirectToAction("Contact", new { message = response.Message, color = ProjectVariables.DangerColor });
                }
            }
        }

        #endregion

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}