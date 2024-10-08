using ITValetFrontEnd.HelperClasses;
using ITValetFrontEnd.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ITValetFrontEnd.ClientSideServices
{
    public interface IUserService
    {
        Task<ResponseDto> UpdateProfile(PostUpdateUserDto user, string jwtToken);
        Task<ResponseDto> AddRequestService(PostAddRequestServices postAddRequestServices, string jwtToken);
        Task<ResponseDto> GetRequestServiceById(string RequestServiceId, string jwtToken);
        Task<ResponseDto> GetUsersList(string jwtToken);
        Task<ResponseDto> GetOrderById(string orderId, string jwtToken);
        Task<ResponseDto> GetUserSkillsByUserId(int UserId, string jwtToken);
        Task<ResponseDto> ChargeStripePayment(CheckOutDTO Data, string jwtToken);
        Task<ResponseDto> AddPayPalAccount(PayPalAccountViewModel account, string jwtToken);
        Task<ResponseDto> GetPayPalAccountByUserId(int? Id, string jwtToken);
        Task<FilePathResponseDto> UploadOrderWork(IFormFile File, OrderDeliverDto deliverOrder, string jwtToken);
        Task<ResponseDto> AcceptOrder(OrderDeliverDto orderDeliverDto, string jwtToken);
        Task<ResponseDto> OrderAccepeted(AcceptOrder order, string jwtToken); 
        Task<ResponseDto> GetValetsBySearchKey(string keyword, string jwtToken);
        Task<ResponseDto> GetRankedKeyWords(string jwtToken);
        Task<ResponseDto> CreateOrderBySession(CheckOutDTO Data, string jwtToken);
        Task<ResponseDto> GetUserPackageByUserId(int? UserId, string jwtToken);
        Task<ResponseDto> UpdateUser(int? userId, string jwtToken);
        Task<ResponseDto> GetValetRating(string ValetId, string jwtToken);
    }

    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;


        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = ProjectVariables.BaseUrl;
        }

        public async Task<ResponseDto> UpdateProfile(PostUpdateUserDto user, string jwtToken)
        {
            string apiEndpoint = "User/PostUpdateUser";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.PutAsJsonAsync(_baseUrl + apiEndpoint, user);
            var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
            return responseDto;
        }

        public async Task<ResponseDto> GetUsersList(string jwtToken)
        {
            string apiEndpoint = "User/GetUserListAsync";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);
            var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
            return responseDto;
        }

        #region RequestedService
        public async Task<ResponseDto> AddRequestService(PostAddRequestServices postAddRequestServices, string jwtToken)
        {
            string apiEndpoint = "Customer/PostAddRequestService";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.PostAsJsonAsync(_baseUrl + apiEndpoint, postAddRequestServices);
            var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
            return responseDto;
        }

        public async Task<ResponseDto> GetRequestServiceById(string RequestServiceId, string jwtToken)
        {
            string apiEndpoint = "User/GetRequestServiceById?requestServiceId=" + RequestServiceId;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);
            var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
            return responseDto;
        }

        public async Task<ResponseDto> GetOrderById(string orderId, string jwtToken)
        {
            string apiEndpoint = "User/GetOrderById?orderId=" + orderId;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);
            var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
            return responseDto;
        }

        public async Task<ResponseDto> GetValetRating(string ValetId, string jwtToken)
        {
            string apiEndpoint = "User/GetValetRatingRecord?ValetEncId=" + ValetId;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);
            var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
            return responseDto;
        }

        public async Task<ResponseDto> GetUserSkillsByUserId(int UserId,string jwtToken)
        {
            string apiEndpoint = "User/GetUserSkillByUserId?userId=" + UserId;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);
            var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
            return responseDto;
        }
        #endregion

        #region OrderAndWork
        public async Task<FilePathResponseDto> UploadOrderWork(IFormFile file, OrderDeliverDto deliverOrder, string jwtToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            _httpClient.Timeout = TimeSpan.FromMinutes(10);
            string endpoint = ProjectVariables.BaseUrl + "Message/UploadOrderWork";
            
            string para = "ValetId=" + deliverOrder.ValetId +
                "&CustomerId=" + deliverOrder.CustomerId +
                "&OrderId=" + deliverOrder.OrderId +
                "&Description=" + deliverOrder.Description;

            var requestUri = $"{endpoint}?{para}";
            var request = new HttpRequestMessage();
            if (file != null)
            {
                request = new HttpRequestMessage(HttpMethod.Put, requestUri)
                {
                    Content = CreateMultipartContent(file)
                };
            }
            using var apiResponse = await _httpClient.SendAsync(request);
            var response = await apiResponse.Content.ReadFromJsonAsync<FilePathResponseDto>();
            return response;

        }

        private MultipartFormDataContent CreateMultipartContent(IFormFile file)
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

        public async Task<ResponseDto> AcceptOrder(OrderDeliverDto deliverDto, string jwtToken)
        { 
            string apiEndpoint = "Message/AcceptOrder";

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.PostAsJsonAsync(_baseUrl + apiEndpoint, deliverDto);
            var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
            return responseDto;
        }

        #endregion

        #region PayPalRegionForOrderAccepted
        public async Task<ResponseDto> OrderAccepeted(AcceptOrder order, string jwtToken)
        {
            string apiEndpoint = "PayPalGateWay/OrderAccepted";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.PostAsJsonAsync(_baseUrl + apiEndpoint, order);
            var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
            return responseDto;
        }

        public async Task<ResponseDto> GetPayPalAccountByUserId(int? Id, string jwtToken)
        {
            string apiEndpoint = "PayPalGateWay/GetPayPalAccount?id=" + Id;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);
            var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
            return responseDto;
        }

        public async Task<ResponseDto> GetPayPalTotalEarnedAmount(int? Id, string jwtToken)
        {
            string apiEndpoint = "PayPalGateWay/GetPayPalAccount?id=" + Id;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);
            var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
            return responseDto;
        }

        public async Task<ResponseDto> AddPayPalAccount(PayPalAccountViewModel account, string jwtToken)
        {
            string apiEndpoint = "PayPalGateWay/addPayPalAccount";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.PostAsJsonAsync(_baseUrl + apiEndpoint, account);
            var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
            return responseDto;
        }

        public async Task<ResponseDto> CreateOrderBySession(CheckOutDTO Data, string jwtToken)
        {
            string apiEndpoint = "PayPalGateWay/CreateOrderBySession";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.PostAsJsonAsync(_baseUrl + apiEndpoint, Data);
            var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
            return responseDto;
        }
        #endregion

        #region StripePayment
        public async Task<ResponseDto> ChargeStripePayment(CheckOutDTO Data, string jwtToken)
        {
            string apiEndpoint = "StripePayment/CreateStripeCharge";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.PostAsJsonAsync(_baseUrl + apiEndpoint, Data);
            var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
            return responseDto;
        }

        #endregion

        #region SearchValet 
        public async Task<ResponseDto> GetValetsBySearchKey(string keyword, string jwtToken)
        {
            string apiEndpoint = "User/SearchValet?keyword=" + keyword;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);
            var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
            return responseDto;
        }

        public async Task<ResponseDto> GetRankedKeyWords(string jwtToken)
        {
            string apiEndpoint = "User/GetHighSearchedKeys";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);
            var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
            return responseDto;
        }
        #endregion

        public async Task<ResponseDto> GetUserPackageByUserId(int? UserId, string jwtToken)
        {
            string apiEndpoint = $"Customer/GetUserPackageByUserId?userId={UserId}";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResponseDto>(responseString);
        }
        public async Task<ResponseDto> UpdateUser(int? userId, string jwtToken)
        {
            string apiEndpoint = "User/postUpdateUserRecord?userId=" + userId;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);
            var response = await _httpClient.GetAsync(_baseUrl + apiEndpoint);
            var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
            return responseDto;
        }
    }
}
