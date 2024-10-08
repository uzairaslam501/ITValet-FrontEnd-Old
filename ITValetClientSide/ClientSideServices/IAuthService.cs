using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using ITValetFrontEnd.HelperClasses;
using ITValetFrontEnd.Models;

namespace ITValetFrontEnd.ClientSideServices
{
    public interface IAuthService
    {
        Task<ResponseDto> Login(LoginDto loginDto);
        Task<ResponseDto> Register(PostAddUserDto user);
        Task<ResponseDto> UpdateProfile(PostUpdateUserDto user, string jwtToken);
        Task<ResponseDto> UpdatePassword(UpdatePasswordDto user, string jwtToken);
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;


        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = ProjectVariables.BaseUrl;
        }

        public async Task<ResponseDto> Login(LoginDto loginDto)
        {
            string apiEndpoint = "Auth/Login";
            var response = await _httpClient.PostAsJsonAsync(_baseUrl + apiEndpoint, loginDto);
            var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
            return responseDto;
        }

        public async Task<ResponseDto> Register(PostAddUserDto user)
        {
            string apiEndpoint = "Auth/UserRegisteration";
            var response = await _httpClient.PostAsJsonAsync(_baseUrl + apiEndpoint, user);
            var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
            return responseDto;
        }

        public static List<Claim> GenerateClaimsForUser(UserClaims user)
        {

            user.IsActive = user.IsActive == "1" ? "Active" : user.IsActive;
            user.IsActive = user.IsActive == "2" ? "EmailVerificationPending" : user.IsActive;
            user.IsActive = user.IsActive == "3" ? "EmailVerificationPending" : user.IsActive;
            user.IsActive = user.IsActive == "4" ? "AccountOnHold" : user.IsActive;
            user.Role = user.Role == "1" ? "Admin" : user.Role;
            user.Role = user.Role == "2" ? "Employee" : user.Role;
            user.Role = user.Role == "3" ? "Customer" : user.Role;
            user.Role = user.Role == "4" ? "Valet" : user.Role;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("UserEncId", user.UserEncId),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("IsActive", user.IsActive),
                new Claim("IsCompleteValetAccount", user.IsCompleteValetAccount),
                new Claim("UserName", user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("Token", user.Token),
                new Claim("ProfileImage", user.ProfilePicture != null ? user.ProfilePicture : ""),
            };
            return claims;
        }

        public async Task<ResponseDto> UpdateProfile(PostUpdateUserDto user, string jwtToken)
        {
            string apiEndpoint = "Auth/UpdateProfile";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.PutAsJsonAsync(_baseUrl + apiEndpoint, user);
            var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
            return responseDto;
        }

        public async Task<ResponseDto> UpdatePassword(UpdatePasswordDto user, string jwtToken)
        {
            string apiEndpoint = "Auth/UpdatePassword";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.PutAsJsonAsync(_baseUrl + apiEndpoint, user);
            var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
            return responseDto;
        }

    }
}
