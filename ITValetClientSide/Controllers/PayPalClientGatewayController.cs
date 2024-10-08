using ITValetFrontEnd.HelperClasses;
using ITValetFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;
using ITValetFrontEnd.ClientSideServices;
using System.Security.Principal;

namespace ITValetFrontEnd.Controllers
{
    public class PayPalClientGatewayController : Controller
    {
        private readonly GeneralHelper _helper;
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;
        private readonly string _baseUrl;
        public PayPalClientGatewayController(GeneralHelper helper, HttpClient httpClient, IUserService userService)
        {
            _helper = helper;
            _httpClient = httpClient;
            _baseUrl = ProjectVariables.BaseUrl;
            _userService = userService; 
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PostCheckoutForPackage(PackageCOutRequest packageCheckoutObj)
        {
            var user = await _helper.ValidateUserAsync();
            if (user != null)
            {
                string apiEndpoint = "PayPalGateWay/payPalCheckoutForPackage";
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(user.Token);
                var response = await _httpClient.PostAsJsonAsync(_baseUrl + apiEndpoint, packageCheckoutObj);
                var responseObj = await response.Content.ReadFromJsonAsync<ResponseDto>();
                if (responseObj.Status == true)
                {
                    var packageCheckoutUrl = JsonConvert.DeserializeObject<PayPalCheckOutURL>(responseObj.Data.ToString());
                    if (packageCheckoutUrl != null)
                    {
                        return RedirectToAction("RedirectToPayPal", new { redirectUrl = packageCheckoutUrl.Url });
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Auth", "Login");
        }

        [HttpPost]
        public async Task<IActionResult> PostCheckoutForOrder(PayPalOrderCheckOutViewModel orderDto)
        {
            var user = await _helper.ValidateUserAsync();
            if (user != null)
            {
                string apiEndpoint = "PayPalGateWay/paypalCheckoutForOrder";
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(user.Token);
                var response = await _httpClient.PostAsJsonAsync(_baseUrl + apiEndpoint, orderDto);
                var responseObj = await response.Content.ReadFromJsonAsync<ResponseDto>();
                if (responseObj.Status == true)
                {
                    var packageCheckoutUrl = JsonConvert.DeserializeObject<PayPalCheckOutURL>(responseObj.Data.ToString());
                    if (packageCheckoutUrl != null)
                    {
                        return RedirectToAction("RedirectToPayPal", new { redirectUrl = packageCheckoutUrl.Url });
                    }
                }
                else if (responseObj.Message == "Price can't be negative")
                {
                    return RedirectToAction("It will be defined soon");
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Auth", "Login");
        }

        public async Task<IActionResult> PaymentStatusForPackage(string paymentId, string token, string PayerID)
        {
            var user = await _helper.ValidateUserAsync();
            if (user != null)
            {
                string apiEndpoint = $"PayPalGateWay/packagestatus?paymentId={paymentId}&token={token}&PayerID={PayerID}";
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(user.Token);
                var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);
                var responseObj = await response.Content.ReadAsStringAsync();
                var paymentresponse = JsonConvert.DeserializeObject<ResponseDto>(responseObj);
                if (paymentresponse.Status == true)
                {
                    var paymentStatus = JsonConvert.DeserializeObject<CheckoutPaymentStatusPackage>(paymentresponse.Data.ToString());
                    return RedirectToAction("SystemPackages", "Home");
                }
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Auth", "Login");
        }

        public async Task<IActionResult> PaymentStatusForOrder(string paymentId, string token, string PayerID)
        {
            var user = await _helper.ValidateUserAsync();
            if (user != null)
            {
                string apiEndpoint = $"PayPalGateWay/orderstatus?paymentId={paymentId}&token={token}&PayerID={PayerID}";
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(user.Token);
                var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);
                var responseObj = await response.Content.ReadAsStringAsync();
                var paymentresponse = JsonConvert.DeserializeObject<ResponseDto>(responseObj);
                if (paymentresponse.Status == true)
                {
                    var paymentStatus = JsonConvert.DeserializeObject<CheckoutPaymentStatusOrder>(paymentresponse.Data.ToString());
                   // return RedirectToAction("ManageOrders", "User");
                    return RedirectToAction("OrderDetail", "User", new { OrderId = paymentStatus.EncOrderId });
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home", new { msg = "Session Expired, Logged In Again", color = ProjectVariables.DangerColor });
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrderAndPaymentRefund(CancelOrder cancelationObj)
        {
            var user = await _helper.ValidateUserAsync();
            if (user != null)
            {
                string apiEndpoint = "PayPalGateWay/paypalRefund";
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(user.Token);
                var response = await _httpClient.PostAsJsonAsync(_baseUrl + apiEndpoint, cancelationObj);
                var responseObj = await response.Content.ReadFromJsonAsync<ResponseDto>();
                if (responseObj.Status == true)
                {
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Auth", "Login");
        }

        [HttpPost]
        public async Task<IActionResult> AddPayPalAccountInformation(PayPalAccountViewModel account)
        {
            var user = await _helper.ValidateUserAsync();
            if (user != null)
            {
                account.IsActive = 1;
                var response = await _userService.AddPayPalAccount(account, user.Token);
                if (response.Status == true)
                {
                    return RedirectToAction("Account", "User", new { msg = response.Message, color = "green" });
                }
                else if (response.StatusCode == "204" )
                {
                    return RedirectToAction("Account", "User", new { msg = response.Message, color = "red" });
                }
                else
                {
                    return RedirectToAction("Account", "User", new { msg = "Something went wrong", color = "red" });
                }
            }
            return RedirectToAction("Login", "Auth");
        }

        [HttpPost]
        public async Task<IActionResult> AcceptedOrderOfPayPal(AcceptOrder order)
        {
            var user = await _helper.ValidateUserAsync();
            if (user != null)
            {
                var response = await _userService.OrderAccepeted(order, user.Token);
                if (response.Status == true)
                {
                    return RedirectToAction("OrderDetail", "User", new { OrderId = order.OrderId });
                }
                return View();
            }
            return RedirectToAction("Login", "Auth");
        }

        public async Task<IActionResult> CancelOrder(string token)
        {
            var user = await _helper.ValidateUserAsync();
            if (user != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Login", "Auth");
        }

        public async Task<IActionResult> CancelPackage(string token)
        {
            var user = await _helper.ValidateUserAsync();
            if (user != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Login", "Auth");

        }

        public IActionResult RedirectToPayPal(string redirectUrl)
        {
            return Redirect(redirectUrl);
        }

        //TestingPurposeView
        public IActionResult TestPayPal()
        {
           return View();
        }

        public async Task<IActionResult> TestFund()
        {
            var user = await _helper.ValidateUserAsync();
            if (user != null)
            {
                string apiEndpoint = "PayPalGateWay/TestFundTransfer";
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(user.Token);
                var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);
                var responseObj = await response.Content.ReadAsStringAsync();
                var paymentresponse = JsonConvert.DeserializeObject<ResponseDto>(responseObj);
                if (paymentresponse.Status == true)
                { 
                    return RedirectToAction("SystemPackages", "Home");
                }
                return RedirectToAction("Index");
            }

            return RedirectToAction("Auth", "Login");
        }

    }
}
