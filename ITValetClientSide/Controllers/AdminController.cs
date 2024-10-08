using ITValetFrontEnd.ClientSideServices;
using ITValetFrontEnd.Filters;
using ITValetFrontEnd.HelperClasses;
using ITValetFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Drawing;

namespace ITValetFrontEnd.Controllers
{
    public class AdminController : Controller
    {
        private readonly GeneralHelper _helper;
        private readonly IAdminService _adminService;
        HttpClient _httpClient = new HttpClient();
        private readonly string _baseUrl;


        public AdminController(GeneralHelper generalHelper, IAdminService adminService)
        {
            _helper = generalHelper;
            _adminService = adminService;
            _baseUrl = ProjectVariables.BaseUrl;

        }

        [RoleBasedValidation(EnumRoles.Admin)]
        public async Task<IActionResult> Index(string? message = "", string? color = "")
        {
            try
            {
                ViewBag.Message = message;
                ViewBag.Color = color;

                string apiEndpoint = $"Admin/PostIndex";
                var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var earningsApiResponse = JsonConvert.DeserializeObject<ResponseDto>(responseData);
                    ViewBag.Customer = earningsApiResponse.Data.customer;
                    ViewBag.CustomersUnderReview = earningsApiResponse.Data.customersUnderReview;
                    ViewBag.CustomersVerificationPending = earningsApiResponse.Data.customersVerificationPending;
                    ViewBag.Valet = earningsApiResponse.Data.valet;
                    ViewBag.ValetUnderReview = earningsApiResponse.Data.valetUnderReview;
                    ViewBag.ValetVerificationPending = earningsApiResponse.Data.valetVerificationPending;

                    return View();
                }
                else
                {
                    return RedirectToAction("Error", "Home", new { message = "There is an error while getting Seller Earnings", color = ProjectVariables.DangerColor });
                }
            }
            catch (Exception ex)
            {           
                ViewBag.Message = message;
                ViewBag.Color = color;
                return View("Error");
            }

        }

        #region Manage_Customers_And_ITValets 
        //Valet(4) and Customer(3) values for database insertion:
        public IActionResult AddNewUser(string message = "", string color = "black", int valetOrCustomer = 0)
        {
            ViewBag.Message = message;
            ViewBag.Color = color;
            ViewBag.ValetOrCustomer = valetOrCustomer;
            return View();
        }

        [HttpPost]
        [RoleBasedValidation(EnumRoles.Admin)]
        public async Task<IActionResult> PostNewUser(PostAddUserDto userObj)
        {
            var user = await _helper.ValidateUserAsync();
            if (user != null)
            {
                var response = await _adminService.PostAddUser(userObj, user.Token);
                if (response.StatusCode == "200" && response.Status == true)
                {
                    return RedirectToAction("AddNewUser", "Admin", new { message = response.Message, color = ProjectVariables.SuccessColor, valetOrCustomer = userObj.Role });
                }
                else if (response.StatusCode == "400" && response.Status == false)
                {
                    return RedirectToAction("AddNewUser", "Admin", new { message = response.Message, color = ProjectVariables.DangerColor, valetOrCustomer = userObj.Role });
                }
                else
                {
                    return RedirectToAction("AddNewUser", "Admin", new { message = "Something went wrong", color = ProjectVariables.DangerColor, valetOrCustomer = userObj.Role });
                }
            }
            return RedirectToAction("Login", "Auth");
        }

        [RoleBasedValidation(EnumRoles.Admin)]
        public IActionResult ViewUser(string message = "", string color = "black", int valetOrCustomer = 0)
        {
            ViewBag.Message = message;
            ViewBag.Color = color;
            ViewBag.ValetOrCustomer = valetOrCustomer;
            return View();
        }

        public IActionResult UserUnderReviews(string message = "", string color = "black", int valetOrCustomer = 0)
        {
            ViewBag.Message = message;
            ViewBag.Color = color;
            ViewBag.ValetOrCustomer = valetOrCustomer;
            return View();
        }
        #endregion

        #region ContactUs_FeedBackRecord

        [RoleBasedValidation(EnumRoles.Admin)]
        public IActionResult FeedbackRecord(string message = "", string color = "black")
        {
            ViewBag.Message = message;
            ViewBag.Color = color;
            return View();
        }
        [RoleBasedValidation(EnumRoles.Admin)]
        public IActionResult ViewSubscriptions()
        {
            return View();
        }
        #endregion

        #region UserSubscription_Record
        public IActionResult UserSubscriptionRecord(string message = "", string color = "black")
        {
            ViewBag.Message = message;
            ViewBag.Color = color;
            return View();
        }
        #endregion

        #region UpdateUserStatus
        [HttpPost]
        public async Task<IActionResult> UpdateUserStatus(int role, string EncId, string status = "")
        {
            var user = await _helper.ValidateUserAsync();
            if (user != null)
            {
                if(status == "Deleted")
                {
                    var response = await _adminService.UpdateUserStatus(role, EncId, user.Token, 0);
                    if (response.Status == true)
                    {
                        return RedirectToAction("ViewUser", "Admin", new { message = response.Message, color = ProjectVariables.SuccessColor, valetOrCustomer = role });
                    }
                    else
                    {
                        return RedirectToAction("ViewUser", "Admin", new { message = response.Message, color = ProjectVariables.DangerColor, valetOrCustomer = role });
                    }
                }

            }
            return RedirectToAction("Login", "Auth");

        }
        #endregion


        #region UserOrderRecordStatus
        public async Task<IActionResult> ViewOrderEvents()
        {
            var user = await _helper.ValidateUserAsync();
            if (user != null)
            {
                var response = await _adminService.GetActiveUsersName(user.Token);
                if (response.Status == true)
                {
                    var fectedUserNames = JsonConvert.DeserializeObject<List<ActiveUsersNameDto>>(response.Data.ToString());
                    ViewBag.ActiveUsers = fectedUserNames;
                }
                return View();
            }
            return RedirectToAction("Auth", "Login");
        }

        #endregion

        #region PayPalTransactionsRecord

        [RoleBasedValidation(EnumRoles.Admin)]
        public async Task<IActionResult> PayPalOrdersRecord(string message = "", string color = "black")
        {
            ViewBag.Message = message;
            ViewBag.Color = color;
            return View();
        }

        [RoleBasedValidation(EnumRoles.Admin)]
        public async Task<IActionResult> PayPalTransactionRecord(string message = "", string color = "black")
        {
            ViewBag.Message = message;
            ViewBag.Color = color;
            return View("TransactionRecord");
        }

        [RoleBasedValidation(EnumRoles.Admin)]
        public async Task<IActionResult> PayPalUnclaimedPaymentRecord(string message = "", string color = "black")
        {
            ViewBag.Message = message;
            ViewBag.Color = color;
            return View("UnclaimedPaymentRecord");
        }


        [RoleBasedValidation(EnumRoles.Admin)]
        public async Task<IActionResult> StripeOrdersRecord(string message = "", string color = "black")
        {
            ViewBag.Message = message;
            ViewBag.Color = color;
            return View();
        }
        #endregion


    }
}
