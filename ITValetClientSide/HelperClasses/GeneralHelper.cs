using ITValetFrontEnd.Models;
using System.Security.Claims;

namespace ITValetFrontEnd.HelperClasses
{
    public class GeneralHelper
    {
        private readonly HttpContext _httpContext;
        public GeneralHelper(IHttpContextAccessor accessor)
        {
            _httpContext = accessor.HttpContext;
        }

        public async Task<UserValidation> ValidateUserAsync()
        {
            var userIdClaim = _httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            string userId = userIdClaim?.Value;

            var tokenClaim = _httpContext.User.FindFirst("Token");
            var userEncIdClaim = _httpContext.User.FindFirst("UserEncId");
            var userProfile = _httpContext.User.FindFirst("ProfileImage")?.Value;
            userProfile = string.IsNullOrEmpty(userProfile) ? "../frontAssets/images/user/s1.png" : userProfile;


            string userName = _httpContext.User.FindFirstValue(ClaimTypes.Name);
            string email = _httpContext.User.FindFirstValue(ClaimTypes.Email);
            string role = _httpContext.User.FindFirstValue(ClaimTypes.Role);
            string IsCompleteValetAccount = _httpContext.User.FindFirstValue("IsCompleteValetAccount");
            string token = tokenClaim?.Value;
            string userEncId = userEncIdClaim?.Value;

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(userName) && 
                !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(role) && 
                !string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(userEncId))
            {
                // User claims exist, create a new UserValidation object
                var validUser = new UserValidation
                {
                    Id = int.Parse(userId),
                    UserName = userName,
                    Email = email,
                    UserProfile = userProfile,
                    UserEncId = userEncId,
                    Role = role,
                    Token = token,
                    IsCompleteValetAccount = IsCompleteValetAccount
                };

                return validUser;
            }

            return null; // User claims not found in the cookies
        }


    }
}
