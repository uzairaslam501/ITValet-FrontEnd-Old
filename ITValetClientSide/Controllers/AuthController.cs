using ITValetFrontEnd.ClientSideServices;
using ITValetFrontEnd.Filters;
using ITValetFrontEnd.HelperClasses;
using ITValetFrontEnd.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Common;
using System.Data;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Security.Claims;
using System.Net;

namespace ITValetFrontEnd.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly GeneralHelper _helper;
        private readonly IAdminService _adminService;
        HttpClient _httpClient = new HttpClient();
        private readonly string _baseUrl;
        public AuthController(IAuthService authService, GeneralHelper generalHelper, IAdminService adminService)
        {
            _authService = authService;
            _helper = generalHelper;
            _adminService = adminService;
            _baseUrl = ProjectVariables.BaseUrl;

        }
        public IActionResult Index(string message = "", string color = "")
        {
            ViewBag.Message = message;
            ViewBag.Color = color;
            return View();
        }

        #region Login
        public IActionResult Login(string returnUrl = "", string message = "", string color = "")
        {
            ViewBag.ReturnUrl = returnUrl; 
            ViewBag.Message = message;
            ViewBag.Color = color;
            return View();
        }

        public IActionResult UserLogin(string message = "", string color = "")
        {
            ViewBag.Message = message;
            ViewBag.Color = color;
            return View();
        }

        public async Task<ActionResult> PostLogin(LoginDto loginDto, string returnUrl = "", string way = "")
        {
            var responseDto = await _authService.Login(loginDto);

            if (responseDto.StatusCode == "200" && responseDto.Status == true)
            {
                var userObj = JsonConvert.DeserializeObject<UserClaims>(responseDto.Data.ToString());
                // Successful login
                var claims = AuthService.GenerateClaimsForUser(userObj);
                var claimsIdentity = new ClaimsIdentity(claims, "UserClaims");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties()
                {
                    IsPersistent = true
                });
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else if (way == "Admin")
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    return RedirectToAction("Account", "User");
                }
            }
            else
            {
                // Handle error cases based on the responseDto properties
                if (way == "Admin")
                {
                    return RedirectToAction("Login", "Auth", new { message = responseDto.Message, color = ProjectVariables.DangerColor });
                }
                else
                {
                    return RedirectToAction("UserLogin", "Auth", new { message = responseDto.Message, color = ProjectVariables.DangerColor });
                }
            }
        }
        #endregion

        #region ForgotPassword
        public async Task<IActionResult> ForgotPassword(string? message = "", string? color = "")
        {
            ViewBag.Message = message;
            ViewBag.Color = color;
            return View();
        }

        public async Task<IActionResult> UserForgotPassword(string? message = "", string? color = "")
        {
            ViewBag.Message = message;
            ViewBag.Color = color;
            return View();
        }

        public async Task<IActionResult> PostForgotPassword(string Email = "", string msg = "", string color = "")
        {
            string apiEndpoint = $"Auth/PostForgotPassword?Email={Email}";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJ1bmlxdWVfbmFtZSI6IjEiLCJ1c2VyRk5hbWUiOiJTeXN0ZW0iLCJ1c2VyTE5hbWUiOiJBZG1pbiIsInVzZXJOYW1lIjoiU3lzdGVtQWRtaW4iLCJ1c2VyRW1haWwiOiJ1c21hbkBnbWFpbC5jb20iLCJ1c2VyUm9sZSI6IkFkbWluIiwidGltZVpvbmUiOiItNzowMCIsInVzZXJTdGF0dXMiOiJBY3RpdmUiLCJuYmYiOjE2OTMzOTY4NTIsImV4cCI6MTY5NDAwMTY1MiwiaWF0IjoxNjkzMzk2ODUyfQ.t9u_HhVh55i_QCZHtxc4rxOAmt3Iwoij-gFDeC7T3p0");
            var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);
            var responseObj = await response.Content.ReadAsStringAsync();
            var accountConfirmResponse = JsonConvert.DeserializeObject<ResponseDto>(responseObj);
            if (accountConfirmResponse.Status == true)
            {
                msg = accountConfirmResponse.Message;
                return RedirectToAction("UserForgotPassword", "Auth", new { message = msg, color = color != "" ? color : ProjectVariables.SuccessColor });
            }
            else
            {
                msg = accountConfirmResponse.Message;
                return RedirectToAction("UserForgotPassword", new { message = msg, color = color != "" ? color : ProjectVariables.DangerColor });
            }
        }

        public IActionResult RenewPassword(string Id = "", long t = 1, string? message = "", string? color = "")
        {
            ViewBag.Message = message;
            ViewBag.Color = color;
            ViewBag.Id = Id;
            ViewBag.t = t;
            return View();
        }

        public async Task<IActionResult> PostRenewPassword(string Id = "", string Password = "", long t = 1, string msg = "", string color = "")
        {
            string apiEndpoint = $"Auth/PostRenewPassword?Id={Id}&Password={Password}&t={t}&t={t}";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJ1bmlxdWVfbmFtZSI6IjEiLCJ1c2VyRk5hbWUiOiJTeXN0ZW0iLCJ1c2VyTE5hbWUiOiJBZG1pbiIsInVzZXJOYW1lIjoiU3lzdGVtQWRtaW4iLCJ1c2VyRW1haWwiOiJ1c21hbkBnbWFpbC5jb20iLCJ1c2VyUm9sZSI6IkFkbWluIiwidGltZVpvbmUiOiItNzowMCIsInVzZXJTdGF0dXMiOiJBY3RpdmUiLCJuYmYiOjE2OTMzOTY4NTIsImV4cCI6MTY5NDAwMTY1MiwiaWF0IjoxNjkzMzk2ODUyfQ.t9u_HhVh55i_QCZHtxc4rxOAmt3Iwoij-gFDeC7T3p0");
            var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);
            var responseObj = await response.Content.ReadAsStringAsync();
            var accountConfirmResponse = JsonConvert.DeserializeObject<ResponseDto>(responseObj);
            if (accountConfirmResponse.Status == true)
            {
                msg = accountConfirmResponse.Message;
                return RedirectToAction("Index", "Home", new { message = msg, color = color != "" ? color : ProjectVariables.SuccessColor });
            }
            else
            {
                msg = accountConfirmResponse.Message;
                return RedirectToAction("Error", "Home", new { message = msg, color = color != "" ? color : ProjectVariables.DangerColor });
            }
        }
        #endregion

        #region Registeration
        public async Task<IActionResult> Register(string value = "", string message = "", string color = "")
        {
            var user = await _helper.ValidateUserAsync();
            if(user!=null)
            {
                ViewBag.LoginUserRole = user.Role;
            }

            ViewBag.UserType = value;
            ViewBag.Message = message;
            ViewBag.Color = color;
            return View();
        }

        public async Task<IActionResult> PostRegister(PostAddUserDto user)
        {
            var responseDto = await _authService.Register(user);
            if(responseDto.StatusCode == "200" && responseDto.Status == true)
            {
                return RedirectToAction("UserLogin", "Auth", new { message = "Your account has been created, please login to access your account" });
            }
            return View();
        }
        #endregion

        public IActionResult ResetPassword(string? message = "", string? color = "")
        {
            ViewBag.Message = message;
            ViewBag.Color = color;
            return View();
        }

        #region UpdateProfile
        public async Task<IActionResult> UpdateProfile(string message = "", string color = "")
        {
            var user = await _helper.ValidateUserAsync();
            if (user != null)
            {
                var response = await _adminService.GetUserById(user.Id, user.Token);
                if (response.StatusCode == "200" && response.Status == true)
                {
                    var updateUserRecord = JsonConvert.DeserializeObject<UserClaims>(response.Data.ToString());
                    ViewBag.UserRecord = updateUserRecord;
                }
            }
            ViewBag.Message = message;
            ViewBag.Color = color;
            return View();
        }

        [HttpPost]
        [RoleBasedValidation(EnumRoles.Admin)]
        public async Task<IActionResult> PostUpdateProfile(PostUpdateUserDto updateUser)
        {
            var user = await _helper.ValidateUserAsync();
            if (user != null)
            {
                var response = await _authService.UpdateProfile(updateUser, user.Token);

                if (response.StatusCode == "200" && response.Status == true)
                {
                    return RedirectToAction("UpdateProfile", "Auth", new { message = response.Message, color = "green" });
                }
                else if (response.StatusCode == "400" && response.Status == false)
                {
                    return RedirectToAction("UpdateProfile", "Auth", new { message = response.Message, color = ProjectVariables.DangerColor });
                }
                else
                {
                    return RedirectToAction("UpdateProfile", "Auth", new { message = "Something went wrong", color = ProjectVariables.DangerColor });
                }
            }
            return RedirectToAction("Login", "Auth");
        }

        public IActionResult UpdateUserPassword(string message = "", string color = "")
        {
            ViewBag.Message = message;
            ViewBag.Color = color;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PostUpdateUserPassword(UpdatePasswordDto updatePassword)
        {
            var user = await _helper.ValidateUserAsync();
            if (user != null)
            {
                var response = await _authService.UpdatePassword(updatePassword, user.Token);

                if (response.StatusCode == "200" && response.Status == true)
                {
                    return RedirectToAction("UpdateUserPassword", new { message = response.Message, color = "green" });
                }
                else if (response.StatusCode == "404" && response.Status == false)
                {
                    return RedirectToAction("UpdateUserPassword", new { message = response.Message, color = ProjectVariables.DangerColor });
                }
                else
                {
                    return RedirectToAction("UpdateUserPassword", new { message = "Something went wrong", color = ProjectVariables.DangerColor });
                }
            }
            return RedirectToAction("Login", "Auth");
        }

        #endregion

        public IActionResult UpdatePassword(string message = "", string color = "")
        {
            ViewBag.Message = message;
            ViewBag.Color = color;
            return View();
        }

        [HttpPost]
        [RoleBasedValidation(EnumRoles.Admin)]
        public async Task<IActionResult> PostUpdatePassword(UpdatePasswordDto updatePassword)
        {
            var user = await _helper.ValidateUserAsync();
            if (user != null)
            {
                var response = await _authService.UpdatePassword(updatePassword, user.Token);

                if (response.StatusCode == "200" && response.Status == true)
                {
                    return RedirectToAction("UpdatePassword", "Auth", new { message = response.Message, color = "green" });
                }
                else if (response.StatusCode == "404" && response.Status == false)
                {
                    return RedirectToAction("UpdatePassword", "Auth", new { message = response.Message, color = ProjectVariables.DangerColor });
                }
                else
                {
                    return RedirectToAction("UpdatePassword", "Auth", new { message = "Something went wrong", color = ProjectVariables.DangerColor });
                }
            }
            return RedirectToAction("Login", "Auth");
        }

        public async Task<IActionResult> ConfirmAccount(string Id = "", long t = 1, string msg = "", string color = "")
        {
            string apiEndpoint = $"Auth/ConfirmAccount?Id={Id}&t={t}";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJ1bmlxdWVfbmFtZSI6IjEiLCJ1c2VyRk5hbWUiOiJTeXN0ZW0iLCJ1c2VyTE5hbWUiOiJBZG1pbiIsInVzZXJOYW1lIjoiU3lzdGVtQWRtaW4iLCJ1c2VyRW1haWwiOiJ1c21hbkBnbWFpbC5jb20iLCJ1c2VyUm9sZSI6IkFkbWluIiwidGltZVpvbmUiOiItNzowMCIsInVzZXJTdGF0dXMiOiJBY3RpdmUiLCJuYmYiOjE2OTMzOTY4NTIsImV4cCI6MTY5NDAwMTY1MiwiaWF0IjoxNjkzMzk2ODUyfQ.t9u_HhVh55i_QCZHtxc4rxOAmt3Iwoij-gFDeC7T3p0");
            var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);
            var responseObj = await response.Content.ReadAsStringAsync();
            var accountConfirmResponse = JsonConvert.DeserializeObject<ResponseDto>(responseObj);
            if (accountConfirmResponse.Status == true) 
            {
                msg = accountConfirmResponse.Message;
                return RedirectToAction("Index", "Home", new { message = msg, color = color != "" ? color : ProjectVariables.SuccessColor });
            }
            else
            {
                msg = accountConfirmResponse.Message;
                return RedirectToAction("Error", "Home", new { message = msg, color = color != "" ? color : ProjectVariables.DangerColor });
            }
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }
    }
}
