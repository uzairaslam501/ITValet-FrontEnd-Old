using ITValetFrontEnd.ClientSideServices;
using ITValetFrontEnd.Filters;
using ITValetFrontEnd.HelperClasses;
using ITValetFrontEnd.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace ITValetFrontEnd.Controllers
{
    public class UserController : Controller
    {
        private readonly GeneralHelper _helper;
        private readonly IAdminService _adminService;
        private readonly IAuthService _authService;
        private readonly string _baseUrl;
        private readonly IUserService _userService;
        HttpClient _httpClient = new HttpClient();
        
        public UserController( GeneralHelper generalHelper, IAdminService adminService, IAuthService authService, IUserService userService)
        {
            _helper = generalHelper;
            _adminService = adminService;
            _authService = authService;
            _userService = userService;
            _baseUrl = ProjectVariables.BaseUrl;

        }
        
        public async Task<IActionResult> Account(string? message = "", string color = "")
        {
            var user = await _helper.ValidateUserAsync();
            if (user == null)
            {
                return RedirectToAction("UserLogin", "Auth");
            }
            if (user != null)
            {
                var response = await _adminService.GetUserById(user.Id, user.Token);
                if (response.StatusCode == "200" && response.Status == true)
                {
                    var updateUserRecord = JsonConvert.DeserializeObject<UserListDto>(response.Data.ToString());
                    var updateUserClaims = JsonConvert.DeserializeObject<UserClaims>(response.Data.ToString());
                    if (updateUserClaims != null)
                    {
                        updateUserClaims.Token = user.Token;
                        updateUserClaims.UserEncId = user.UserEncId;
                        await PostLogins(updateUserClaims);
                    }
                    if (message == "Stripe Account Verification Success")
                    {
                        if (updateUserRecord.IsVerify_StripeAccount != 1)
                        {
                            await _userService.UpdateUser(user.Id, user.Token);
                            updateUserRecord.IsVerify_StripeAccount = 1;
                        }
                    }

                    ViewBag.UserRecord = updateUserRecord;
                }
                var paypalResponse = await _userService.GetPayPalAccountByUserId(user.Id, user.Token);
                if (paypalResponse.Status == true)
                {
                    ViewBag.PayPalEmail = paypalResponse.Data.ToString();
                }
            }
            ViewBag.Message = message;
            ViewBag.Color = color;
            return View();
        }

        public async Task<IActionResult> PostUpdateProfile(PostUpdateUserDto updateUser)
        {
            var user = await _helper.ValidateUserAsync();
            if (user == null)
            {
                return RedirectToAction("UserLogin", "Auth");
            }
            if (user != null)
            {
                updateUser.Id = user.Id;
                
                var response = await _userService.UpdateProfile(updateUser, user.Token);
                if (response.StatusCode == "200" && response.Status == true)
                {
                    var userObj = JsonConvert.DeserializeObject<UserClaims>(response.Data.ToString());
                    if (userObj != null)
                    {
                        userObj.Token = user.Token;
                        userObj.UserEncId = user.UserEncId;
                        await PostLogins(userObj);
                    }
                    return RedirectToAction("Account", new { message = response.Message, color = ProjectVariables.SuccessColor });
                }
                else
                {
                    return RedirectToAction("Account", new { message = response.Message, color = ProjectVariables.DangerColor });
                }
            }
            return RedirectToAction("Login", "Auth");
        }

        public async Task<IActionResult> UploadProfileImage(IFormFile ImagePath)
        {
            var user = await _helper.ValidateUserAsync();
            if (user != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(user.Token);
                _httpClient.Timeout = TimeSpan.FromMinutes(10);
                string endpoint = ProjectVariables.BaseUrl + "Admin/UploadPicture";
                string para = "UserId=" + user.Id;
                var requestUri = $"{endpoint}?{para}";
                var request = new HttpRequestMessage();
                if (ImagePath != null)
                {
                    request = new HttpRequestMessage(HttpMethod.Put, requestUri)
                    {
                        Content = BuildMultipartContent(ImagePath)
                    };
                }
                using var apiResponse = await _httpClient.SendAsync(request);
                var response = await apiResponse.Content.ReadFromJsonAsync<ResponseDto>();

                if (apiResponse.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return RedirectToAction("Account", new { message = response.Message, color = ProjectVariables.DangerColor });
                }
                else
                {
                    var userObj = JsonConvert.DeserializeObject<UserClaims>(response.Data.ToString());
                    if (userObj != null)
                    {
                        userObj.Token = user.Token;
                        userObj.UserEncId = user.UserEncId;
                        await PostLogins(userObj);
                    }
                    return RedirectToAction("Account", new { message = response.Message, color = ProjectVariables.SuccessColor });

                }
            }
            else
            {
                return RedirectToAction("Auth", "UserLogin", new { message = "Login Again", color = ProjectVariables.DangerColor });
            }
            
        }

        [RoleBasedValidation(EnumRoles.Valet, EnumRoles.Admin)]
        public async Task<IActionResult> Earnings()
        {
            try
            {
                var user = await _helper.ValidateUserAsync();
                string apiEndpoint = $"User/GetEarnings";

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(user.Token);
                var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);


                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var earningsApiResponse = JsonConvert.DeserializeObject<ResponseDto>(responseData);
                    if(earningsApiResponse.Status == true)
                    {
                        var earningObj = JsonConvert.DeserializeObject<EarningsApiResponse>(earningsApiResponse.Data.ToString());
                        ViewBag.Earning = earningObj;
                    }
                    return View();
                }
                else
                {
                    return RedirectToAction("Error", "Home", new { message = "There is an error while getting Seller Earnings", color = ProjectVariables.DangerColor });
                }
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }
        
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("UserLogin", "Auth");
        }

        #region CustomerRequestHelp
        public async Task<IActionResult> RequestService(string message = "", string color = "")
        {
            var user = await _helper.ValidateUserAsync();
            if (user == null)
            {
                return RedirectToAction("UserLogin", "Auth");
            }
            if (user.Role != "Valet")
            {
                ViewBag.RequestedServiceUserId = user.Id;
                return View();
            }
            else
            {
                return RedirectToAction("Error", "Home", new {message = "You are not authorized to access this page", color = ProjectVariables.DangerColor});
            }
        }

        public async Task<IActionResult> PostRequestService(PostAddRequestServices postAddRequestServices)
        {
            var user = await _helper.ValidateUserAsync();
            if (postAddRequestServices.RequestServiceType == "LiveNow")
            {
                postAddRequestServices.RequestServiceType = "1";
                postAddRequestServices.FromDateTime = DateTime.Now.ToString();
                postAddRequestServices.ToDateTime = DateTime.Now.ToString();
            }
            else
            {
                postAddRequestServices.RequestServiceType = "2";
            }
            var response = await _userService.AddRequestService(postAddRequestServices, user.Token);
            if (response.StatusCode == "200" && response.Status == true)
            {
                return RedirectToAction("UsersForRequestedService", new { requestServiceId = response.Data, message = response.Message, color = ProjectVariables.SuccessColor });
            }
            else
            {
                return RedirectToAction("RequestService", new { message = response.Message, color = ProjectVariables.SuccessColor });
            }
        }

        public async Task<IActionResult> UsersForRequestedService(string requestServiceId = "", string message = "", string color = "")
        {
            var user = await _helper.ValidateUserAsync();
            if (user == null)
            {
                return RedirectToAction("UserLogin", "Auth");
            }
            if (user.Role != "Valet")
            {
                var response = await _userService.GetRequestServiceById(requestServiceId, user.Token);
                if (response.StatusCode == "200" && response.Status == true)
                {
                    var requestedService = JsonConvert.DeserializeObject<RequestServicesDto>(response.Data.ToString());
                    ViewBag.RequestedService = requestedService;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Error", "Home", new { message = "You are not authorized to access this page", color = ProjectVariables.DangerColor });
            }
        }

        public async Task<IActionResult> ManageAppointments()
        {
            var loggendInUser = await _helper.ValidateUserAsync();
            if (loggendInUser == null)
            {
                return RedirectToAction("UserLogin", "Auth");
            }
            ViewBag.User = loggendInUser;
            return View();
        }

        #endregion

        #region Orders
        [RoleBasedValidation(EnumRoles.Customer, EnumRoles.Valet, EnumRoles.Admin)]
        public async Task<IActionResult> ManageOrders()
        {
            var loggendInUser = await _helper.ValidateUserAsync();
            if (loggendInUser == null)
            {
                return RedirectToAction("UserLogin", "Auth");
            }
            ViewBag.User = loggendInUser;
            return View();
        }

        [RoleBasedValidation(EnumRoles.Customer, EnumRoles.Valet, EnumRoles.Admin)]
        public async Task<IActionResult> OrderDetail(string? orderId = "", string msg = "", string color = "")
        {
            var loggendInUser = await _helper.ValidateUserAsync();
            if (loggendInUser == null)
            {
                return RedirectToAction("Index", "Home", new { message = "Session Expired, Logged In Again", color = ProjectVariables.DangerColor });
            }
            var response = await _userService.GetOrderById(orderId, loggendInUser.Token);
            if (response.StatusCode == "200" && response.Status == true)
            {
                var updateUserRecord = JsonConvert.DeserializeObject<OrderDtoList>(response.Data.ToString());
                ViewBag.UserRecord = updateUserRecord;
            }
            ViewBag.User = loggendInUser;
            ViewBag.Message = msg;
            ViewBag.Color = color;
            return View();
        }

        [RoleBasedValidation(EnumRoles.Customer, EnumRoles.Valet)]
        public async Task<IActionResult> UploadOrderWork(IFormFile File, OrderDeliverDto deliverOrder = null)
        {
            var loggendInUser = await _helper.ValidateUserAsync();
            if (loggendInUser == null)
            {
                return RedirectToAction("Index", "Home", new { message = "Session Expired, Logged In Again", color = ProjectVariables.DangerColor });
            }
            var response = await _userService.UploadOrderWork(File, deliverOrder, loggendInUser.Token);
            if (response.StatusCode == "200" && response.Status == true)
            {
                return RedirectToAction("OrderDetail", new { OrderId = deliverOrder.OrderId });
            }
            ViewBag.User = loggendInUser;
            return View();
        }

        [RoleBasedValidation(EnumRoles.Customer, EnumRoles.Valet)]
        public async Task<IActionResult> AcceptOrder(OrderDeliverDto deliverOrder = null)
        {
            var loggendInUser = await _helper.ValidateUserAsync();
            if (loggendInUser == null)
            {
                return RedirectToAction("Index", "Home", new { message = "Session Expired, Logged In Again", color = ProjectVariables.DangerColor });
            }
            var response = await _userService.AcceptOrder(deliverOrder, loggendInUser.Token);
            if(response.Status == true)
            {
                return RedirectToAction("OrderDetail", new { orderId = deliverOrder.OrderId, msg = response.Message, color = ProjectVariables.SuccessColor });
            }
            else
            {
                return RedirectToAction("OrderDetail", new { orderId = deliverOrder.OrderId, msg = response.Message, color = ProjectVariables.DangerColor });
            }
        }
        #endregion

        #region UserProfile

        public async Task<IActionResult>ViewUserProfile(string Id = "", int preview = -1)
        {
            var loggedInUser = await _helper.ValidateUserAsync();
            if (loggedInUser == null)
            {
                return RedirectToAction("UserLogin", "Auth");
            }
            if (!string.IsNullOrEmpty(Id))
            {
                ViewBag.loggedinUser = loggedInUser;
                ViewBag.Id = Id;
                var userResponse = await _adminService.GetUserByIdEncId(Id, loggedInUser.Token);

                if (userResponse.StatusCode == "200" && userResponse.Status==true)
                {
                    var updateUserRecord = JsonConvert.DeserializeObject<UserListDto>(userResponse.Data.ToString());
                    ViewBag.UserRecord = updateUserRecord;

                    var userSkillsResponse = await _userService.GetUserSkillsByUserId(updateUserRecord.Id, loggedInUser.Token);
                    if (userSkillsResponse.StatusCode == "200" && userSkillsResponse.Status == true)
                    {
                        var userSkillsList = JsonConvert.DeserializeObject<List<UserSkillDto>>(userSkillsResponse.Data.ToString());
                        ViewBag.UserSkillsList = userSkillsList;
                    }

                    var valetRatingRecord = await _userService.GetValetRating(Id, loggedInUser.Token);
                    if (valetRatingRecord.Status == true)
                    {
                        var ratingRecordData = JsonConvert.DeserializeObject<ValetRatingReviewRecord>(valetRatingRecord.Data.ToString());
                        ViewBag.ValetRatingRecord = ratingRecordData;
                        ViewBag.TotalReviewsCount = ratingRecordData.Rating.Count;
                    }
                }
            }

            ViewBag.preview = preview;
            return View();
        }

        private async Task<UserPackageDto> GetUserPackage()
        {
            var loggedInUser = await _helper.ValidateUserAsync();
            if (loggedInUser == null)
            {
                return null;
            }

            ViewBag.loggedinUser = loggedInUser;
            var userResponse = await _userService.GetUserPackageByUserId(loggedInUser.Id, loggedInUser.Token);

            if (userResponse.StatusCode == "200" && userResponse.Status == true && userResponse.Data != null)
            {
                var updateUserPackageRecord = JsonConvert.DeserializeObject<UserPackageDto>(userResponse.Data.ToString());
                return updateUserPackageRecord;
            }

            return null;
        }

        public async Task<IActionResult> CheckoutPayment(CheckOutDTO Data)
        {
            var loggedInUser = await _helper.ValidateUserAsync();
         
            var userResponse = await _adminService.GetUserById(loggedInUser.Id, loggedInUser.Token);
            
            if (userResponse.StatusCode == "200" && userResponse.Status == true)
            {
                var updateUserRecord = JsonConvert.DeserializeObject<UserListDto>(userResponse.Data.ToString());
                ViewBag.UserRecord = updateUserRecord;
            }

            var getValetResponse = await _adminService.GetUserById(Convert.ToInt32(Data.ValetId), loggedInUser.Token);
            var valetResponse = new UserListDto();
            if (userResponse.StatusCode == "200" && userResponse.Status == true)
            {
                valetResponse = JsonConvert.DeserializeObject<UserListDto>(getValetResponse.Data.ToString());
            }
            Double userRatingRecord =0;
            Double commission =0;

            // Calculate working hours
            var workingHours = Math.Ceiling((float)TimeSpan.FromTicks(Convert.ToDateTime(Data.ToDateTime).Subtract(Convert.ToDateTime(Data.FromDateTime)).Ticks).TotalHours);
            Data.WorkingHours = workingHours.ToString();
            // Calculate work charges
            decimal workCharges = Convert.ToDecimal(valetResponse?.PricePerHour) * (decimal)workingHours;
            ViewBag.PayableOrderCharges = workCharges.ToString("0.00");
            // Calculate stripe fee (4% of work charges)
            decimal stripeFee = workCharges * 0.04m;
            Data.StripeFee = stripeFee.ToString("0.00");
            // Calculate total payable amount (work charges + stripe fee)
            decimal totalPayableAmount = workCharges + stripeFee;
            ViewBag.PayableAmount = (totalPayableAmount * 100).ToString("0.00"); // Convert to cents for Stripe
            Data.TotalWorkCharges = totalPayableAmount.ToString("0.00");
            // Calculate platform fee (13% of total payable amount)
            decimal platformFee = totalPayableAmount * 0.13m;
            Data.platformFee = platformFee.ToString("0.00");
            Data.ActualOrderPrice = workCharges.ToString();

            //Get User Sessions
            var getUserSessionsDetail = await GetUserPackage();
            var InsuficientSessions = "You have insufficient Sessions";
            var RemainingSessionsAfterOrderConfirm = 0;
            if (getUserSessionsDetail != null)
            {
                if (workingHours <= getUserSessionsDetail.RemainingSessions)
                {
                    InsuficientSessions = "";
                    RemainingSessionsAfterOrderConfirm = getUserSessionsDetail.RemainingSessions.Value - Convert.ToInt32(Data.WorkingHours);
                }
            }
            ViewBag.RemainingSessionsAfterOrderConfirm = RemainingSessionsAfterOrderConfirm;
            ViewBag.InsufficientSession = InsuficientSessions;
            ViewBag.sessionDetails = getUserSessionsDetail;
            ViewBag.hourlyRate = valetResponse?.PricePerHour;
            return View(Data);
        }

        public async Task<IActionResult> ChargePayment(CheckOutDTO Data)
        {
            var loggedInUser = await _helper.ValidateUserAsync();

            if (Data.PackagePaidBy == "PAYPAL")
            {
                var response = await _userService.CreateOrderBySession(Data, loggedInUser.Token);
                var OrderId = response.Id;
                if (response.Status == true)
                {
                    return RedirectToAction("OrderDetail", "User", new { orderId = OrderId, message = "Order Created Successfully" });
                }
            }
            else
            {
                var chargeResponse = await _userService.ChargeStripePayment(Data, loggedInUser.Token);
                var OrderId = chargeResponse.Id;

                if (chargeResponse.StatusCode != "200" || chargeResponse.Status != true)
                {
                    return RedirectToAction("OrderDetail", "User", new { message = "Something Went Wrong! Please Try Again!" });
                }

                return RedirectToAction("OrderDetail", "User", new { orderId = OrderId, message = "Payment Successful!" });
            }

            return RedirectToAction("OrderDetail", "User", new { message = "Invalid Payment Method!" });
        }
        #endregion

        #region Package
        [HttpPost]
        public async Task<IActionResult> StripeCheckoutForPackage(PackageCOutRequest packageCheckoutObj) 
        {
            var user = await _helper.ValidateUserAsync();
            if (user != null)
            {
                string apiEndpoint = "StripePayment/StripeCheckOutForPackages";
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(user.Token);
                var response = await _httpClient.PostAsJsonAsync(ProjectVariables.BaseUrl + apiEndpoint, packageCheckoutObj);
                var responseObj = await response.Content.ReadFromJsonAsync<ResponseDto>();
                if (responseObj.Status == true)
                {
                    return RedirectToAction("SystemPackages", "Home");
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Auth", "Login");
        }

        public async Task<ActionResult> PackageDetails(string msg = "")
        {

            var loggendInUser = await _helper.ValidateUserAsync();
            
            if (loggendInUser == null)
            {
                return RedirectToAction("UserLogin", "Auth");
            }
            ViewBag.loggendInUserId = loggendInUser.Id;
            return View();

        }

        #endregion

        private async Task<bool> PostLogins(UserClaims data, int way = -1)
        {
            var claims = AuthService.GenerateClaimsForUser(data);
            var claimsIdentity = new ClaimsIdentity(claims, "UserClaims");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties()
            {
                IsPersistent = true
            });
            return true;
        }

        private MultipartFormDataContent BuildMultipartContent(IFormFile file)
        {
            byte[] data;
            using (var br = new BinaryReader(file.OpenReadStream()))
            {
                data = br.ReadBytes((int)file.OpenReadStream().Length);
            }
            MultipartFormDataContent multipartFormContent = new();
            multipartFormContent.Headers.ContentType.MediaType = "multipart/form-data";
            var byteArrayContent = new ByteArrayContent(data);
            multipartFormContent.Add(byteArrayContent, "file", file.FileName);
            return multipartFormContent;
        }


    }
}
