using System;
using System.Net.Http;
using System.Text;
using ITValetFrontEnd.HelperClasses;
using ITValetFrontEnd.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace ITValetFrontEnd.ClientSideServices
{
    public interface IAdminService
    {
        Task<ResponseDto> PostAddUser(PostAddUserDto user, string jwtToken);
        Task<ResponseDto> GetUserById(int? id, string jwtToken);
        Task<ResponseDto> GetUserByIdEncId(string? id, string jwtToken);
        Task<ResponseDto> UpdateUserStatus(int? Role, string EncUserId, string jwtToken, int isDeleted = 0);
        Task<ResponseDto> GetActiveUsersName(string jwtToken);
        Task<ResponseDto> GetValetRatingRecords(string? id, string jwtToken);
        Task<ResponseDto> GetUserSlotByUserId(string? id, string jwtToken);
    }

    public class AdminService : IAdminService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        public AdminService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = ProjectVariables.BaseUrl;
        }

        public async Task<ResponseDto> PostAddUser(PostAddUserDto user, string jwtToken)
        {
            string apiEndpoint = "Admin/PostAddUser";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.PostAsJsonAsync(_baseUrl + apiEndpoint, user);
            var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
            return responseDto;
        }

        public async Task<ResponseDto> GetActiveUsersName(string jwtToken)
        {
            string apiEndpoint = "Admin/GetActiveUsersNameForSearching";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResponseDto>(responseString);
        }

        public async Task<ResponseDto> GetUserById(int? id, string jwtToken)
        {
            string apiEndpoint = $"Admin/GetUserById?id={id}";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResponseDto>(responseString);
        }

        public async Task<ResponseDto> GetUserByIdEncId(string? id, string jwtToken)
        {
            string apiEndpoint = $"Admin/GetUserByIdEncryptedId?userId={id}";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResponseDto>(responseString);
        }        
        
        public async Task<ResponseDto> GetValetRatingRecords(string? id, string jwtToken)
        {
            string apiEndpoint = $"User/GetValetRatingRecordByDecreaptedId?ValetEncId={int.Parse(id)}";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResponseDto>(responseString);
        }
        
        public async Task<ResponseDto> GetUserSlotByUserId(string? id, string jwtToken)
        {
            string apiEndpoint = $"User/GetUserAvailableSlotByUserId?userId={id}";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResponseDto>(responseString);
        }

        public async Task<ResponseDto> UpdateUserStatus(int? Role, string EncUserId, string jwtToken, int isDeleted = 0)
        {
            var apiEndPoint = $"Admin/UpdateUserAccountStatus?UserId={EncUserId}&statuses={isDeleted}";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.DeleteAsync(_baseUrl + apiEndPoint);
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResponseDto>(responseString);
        }
    }
}
