using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using biomed.Models;

namespace biomed.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:81/api";

        public ApiClient()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                string.IsNullOrEmpty(token) ? null : new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode(); // Throws on HTTP error status codes

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
            if (apiResponse.Code != 20000)
            {
                throw new Exception($"API Error: {apiResponse.Msg} (Code: {apiResponse.Code})");
            }
            return apiResponse.Data;
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(endpoint, data);
            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<TResponse>>();
            if (apiResponse.Code != 20000)
            {
                throw new Exception($"API Error: {apiResponse.Msg} (Code: {apiResponse.Code})");
            }
            return apiResponse.Data;
        }
        
        public async Task PostAsync<TRequest>(string endpoint, TRequest data)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(endpoint, data);
            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            if (apiResponse.Code != 20000)
            {
                throw new Exception($"API Error: {apiResponse.Msg} (Code: {apiResponse.Code})");
            }
        }

        public async Task<User> LoginAsync(LoginRequestDto loginRequest)
        {
            // Step 1: Login to get the token
            var tokenData = await PostAsync<LoginRequestDto, LoginToken>("/users/login", loginRequest);
            if (tokenData == null || string.IsNullOrEmpty(tokenData.Token))
            {
                throw new Exception("Login failed: Token was not returned.");
            }
            
            // Step 2: Set the auth token for subsequent requests
            SetAuthToken(tokenData.Token);

            // Step 3: Get user info
            var userInfo = await GetAsync<User>("/users/userInfo");
            if (userInfo == null)
            {
                throw new Exception("Login failed: Could not retrieve user info.");
            }

            // Step 4: Combine token and user info
            userInfo.Token = tokenData.Token;
            return userInfo;
        }

        public async Task RegisterAsync(RegisterRequestDto registerRequest)
        {
            await PostAsync("/users/register", registerRequest);
        }

        public async Task UpdatePasswordAsync(object passwordData)
        {
            // The backend expects fields: old_pwd, new_pwd, re_pwd
            // Using an anonymous object for simplicity. A DTO would be better.
            await _httpClient.PatchAsJsonAsync("/api/users/updatePwd", passwordData);
        }
    }
} 