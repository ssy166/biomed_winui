using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using biomed.Models;

namespace biomed.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:81";

        public ApiClient()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            // 添加必要的请求头
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "BiomedWPFApp/1.0");
            
            // 设置超时
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            
            System.Diagnostics.Debug.WriteLine($"🚀 ApiClient初始化完成，BaseUrl: {BaseUrl}");
        }

        public void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                string.IsNullOrEmpty(token) ? null : new AuthenticationHeaderValue("Bearer", token);
        }

        public void SetCsrfToken(string csrfToken)
        {
            if (string.IsNullOrEmpty(csrfToken))
            {
                _httpClient.DefaultRequestHeaders.Remove("X-CSRF-TOKEN");
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Remove("X-CSRF-TOKEN");
                _httpClient.DefaultRequestHeaders.Add("X-CSRF-TOKEN", csrfToken);
            }
        }

        public async Task<string> GetCsrfTokenAsync()
        {
            // 尝试不同的CSRF token获取端点
            var endpoints = new[] { "/csrf-token", "/api/csrf-token", "/auth/csrf-token" };
            
            foreach (var endpoint in endpoints)
            {
                try
                {
                    var response = await _httpClient.GetAsync(endpoint);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        
                        // 尝试解析JSON响应或纯文本响应
                        if (result.StartsWith("{"))
                        {
                            // JSON响应格式，假设为 {"csrfToken": "xxx"} 或 {"token": "xxx"}
                            var json = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(result);
                            if (json.ContainsKey("csrfToken"))
                                return json["csrfToken"].ToString();
                            if (json.ContainsKey("token"))
                                return json["token"].ToString();
                        }
                        else
                        {
                            // 纯文本响应
                            return result.Trim().Trim('"');
                        }
                    }
                }
                catch (Exception)
                {
                    // 继续尝试下一个端点
                    continue;
                }
            }
            
            // 如果所有端点都失败，返回空字符串（某些API可能不需要CSRF token）
            return string.Empty;
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
                
                var isSuccess = (apiResponse.Code >= 20000 && apiResponse.Code < 30000) || (apiResponse.Msg != null && apiResponse.Msg.Contains("成功"));

                if (!isSuccess)
                {
                    throw new Exception($"API Error: {apiResponse.Msg} (Code: {apiResponse.Code})");
                }
                return apiResponse.Data;
            }
            catch (HttpRequestException ex)
            {
                // 将HTTP请求本身的错误包装后重新抛出，以便上层捕获
                throw new Exception($"网络请求失败: {ex.Message} (Endpoint: {endpoint})", ex);
            }
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(endpoint, data);
                response.EnsureSuccessStatusCode();

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<TResponse>>();
                
                var isSuccess = (apiResponse.Code >= 20000 && apiResponse.Code < 30000) || (apiResponse.Msg != null && apiResponse.Msg.Contains("成功"));
                
                if (!isSuccess)
                {
                    throw new Exception($"API Error: {apiResponse.Msg} (Code: {apiResponse.Code})");
                }
                return apiResponse.Data;
            }
            catch (HttpRequestException ex)
            {
                // 将HTTP请求本身的错误包装后重新抛出
                throw new Exception($"网络请求失败: {ex.Message} (Endpoint: {endpoint})", ex);
            }
        }
        
        public async Task PostAsync<TRequest>(string endpoint, TRequest data)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(endpoint, data);
            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            
            // 检查是否为成功状态码
            var isSuccess = apiResponse.Code == 20000 || apiResponse.Code == 20051 || 
                           (apiResponse.Code >= 20000 && apiResponse.Code < 21000 && apiResponse.Msg?.Contains("成功") == true);
            
            if (!isSuccess)
            {
                throw new Exception($"API Error: {apiResponse.Msg} (Code: {apiResponse.Code})");
            }
        }

        // 网络连接测试方法
        public async Task<string> TestConnectionAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/");
                return $"连接成功 - 状态码: {response.StatusCode}";
            }
            catch (Exception ex)
            {
                return $"连接失败: {ex.Message}";
            }
        }

        // 测试多个常见的API路径配置
        public async Task<string> DiagnoseConnectionAsync()
        {
            var baseUrl = "http://localhost:81";
            var testPaths = new[]
            {
                "/api/users/login",           // 当前配置
                "/users/login",               // 没有api前缀
                "/api/user/login",            // 单数形式
                "/user/login",                // 单数且没有api前缀
                "/login",                     // 直接登录路径
                "/auth/login",                // 认证相关路径
                "/api/auth/login",            // API + auth
                "/api/v1/users/login",        // 版本化API
                "/v1/users/login",            // 版本化但没有api前缀
                ""                            // 根路径测试
            };

            var results = new System.Text.StringBuilder();
            results.AppendLine("🔍 API路径诊断:");
            results.AppendLine($"基础服务器: {baseUrl}");
            results.AppendLine("");

            using var testClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(3) };
            testClient.BaseAddress = new Uri(baseUrl);

            foreach (var path in testPaths)
            {
                try
                {
                    var testPath = string.IsNullOrEmpty(path) ? "/" : path;
                    var response = await testClient.GetAsync(testPath);
                    var content = await response.Content.ReadAsStringAsync();
                    
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        results.AppendLine($"✅ {baseUrl}{testPath} - 状态码: {response.StatusCode}");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.MethodNotAllowed)
                    {
                        results.AppendLine($"🔶 {baseUrl}{testPath} - 状态码: {response.StatusCode} (端点存在但不支持GET)");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        results.AppendLine($"❌ {baseUrl}{testPath} - 状态码: {response.StatusCode}");
                    }
                    else
                    {
                        results.AppendLine($"⚠️ {baseUrl}{testPath} - 状态码: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    var testPath = string.IsNullOrEmpty(path) ? "/" : path;
                    results.AppendLine($"❌ {baseUrl}{testPath} - 错误: {ex.Message.Split('\n')[0]}");
                }
            }

            // 额外测试：尝试POST到登录端点
            results.AppendLine("");
            results.AppendLine("🔍 POST登录端点测试:");
            
            var loginPaths = new[] { "/api/users/login", "/users/login", "/login", "/auth/login" };
            foreach (var loginPath in loginPaths)
            {
                try
                {
                    var testData = new { username = "test", passwordHash = "test" };
                    var response = await testClient.PostAsJsonAsync(loginPath, testData);
                    
                    if (response.StatusCode == System.Net.HttpStatusCode.OK || 
                        response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                        response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        results.AppendLine($"✅ {baseUrl}{loginPath} - 登录端点存在 (状态码: {response.StatusCode})");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        results.AppendLine($"❌ {baseUrl}{loginPath} - 登录端点不存在");
                    }
                    else
                    {
                        results.AppendLine($"⚠️ {baseUrl}{loginPath} - 状态码: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    results.AppendLine($"❌ {baseUrl}{loginPath} - 错误: {ex.Message.Split('\n')[0]}");
                }
            }

            return results.ToString();
        }

                // 直接使用API文档中的正确路径进行登录
        public async Task<User> LoginWithoutCsrfAsync(LoginRequestDto loginRequest)
        {
            try
            {
                // 确保请求头设置正确
                _httpClient.DefaultRequestHeaders.Remove("Accept");
                _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                
                // 根据API文档，使用正确的登录路径
                var loginPath = "/api/users/login";
                
                System.Diagnostics.Debug.WriteLine($"🔍 尝试登录路径: {_httpClient.BaseAddress}{loginPath}");
                System.Diagnostics.Debug.WriteLine($"📝 请求数据: username={loginRequest.Username}, passwordHash={loginRequest.Password}");
                
                // 创建JSON内容
                var json = System.Text.Json.JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                System.Diagnostics.Debug.WriteLine($"📤 发送JSON: {json}");
                
                var response = await _httpClient.PostAsync(loginPath, content);
                
                System.Diagnostics.Debug.WriteLine($"📥 响应状态码: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"📄 响应内容: {responseContent}");
                    
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<LoginToken>>();
                    System.Diagnostics.Debug.WriteLine($"📊 API响应解析: Code={apiResponse?.Code}, Msg={apiResponse?.Msg}, HasData={apiResponse?.Data != null}");
                    
                    // 检查多种可能的成功状态码
                    var isSuccess = apiResponse != null && 
                                  (apiResponse.Code == 20000 || apiResponse.Code == 20051 || 
                                   (apiResponse.Code >= 20000 && apiResponse.Code < 21000 && apiResponse.Msg?.Contains("成功") == true));
                    
                    if (isSuccess && apiResponse.Data != null)
                    {
                        var tokenData = apiResponse.Data;
                        if (!string.IsNullOrEmpty(tokenData.Token))
                        {
                            SetAuthToken(tokenData.Token);
                            System.Diagnostics.Debug.WriteLine($"✅ 登录成功，获得Token: {tokenData.Token.Substring(0, 10)}...");

                            // 设置CSRF token（如果登录接口返回了）
                            if (!string.IsNullOrEmpty(tokenData.CsrfToken))
                            {
                                SetCsrfToken(tokenData.CsrfToken);
                                System.Diagnostics.Debug.WriteLine($"✅ 设置CSRF Token: {tokenData.CsrfToken.Substring(0, 10)}...");
                            }

                            // 尝试获取用户信息
                            try
                            {
                                var userInfo = await GetAsync<User>("/api/users/userInfo");
                                if (userInfo != null)
                                {
                                    userInfo.Token = tokenData.Token;
                                    userInfo.CsrfToken = tokenData.CsrfToken;
                                    System.Diagnostics.Debug.WriteLine($"✅ 获取用户信息成功: {userInfo.Username}");
                                    return userInfo;
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"⚠️ 获取用户信息失败: {ex.Message}");
                            }
                            
                            // 如果获取用户信息失败，返回基本用户对象
                            var user = new User 
                            { 
                                Token = tokenData.Token,
                                CsrfToken = tokenData.CsrfToken,
                                Username = loginRequest.Username
                            };
                            return user;
                        }
                    }
                    else if (apiResponse != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ 登录API调用失败 - Code: {apiResponse.Code}, Msg: {apiResponse.Msg}");
                        
                        // 检查是否有其他类型的成功标志
                        if (apiResponse.Msg?.Contains("成功") == true)
                        {
                            throw new Exception($"登录可能成功但数据格式异常: {apiResponse.Msg} (Code: {apiResponse.Code})");
                        }
                        else
                        {
                            throw new Exception($"API返回错误: {apiResponse.Msg} (Code: {apiResponse.Code})");
                        }
                    }
                    else
                    {
                        throw new Exception("API返回了空响应或无法解析的响应");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"❌ 登录失败 - 状态码: {response.StatusCode}, 内容: {errorContent}");
                    
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        throw new Exception("登录失败: 请求参数错误，请检查用户名和密码格式。");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        throw new Exception("登录失败: 用户名或密码错误。");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        throw new Exception($"登录失败: API端点不存在 ({response.StatusCode})。请确认后端API路径配置正确。");
                    }
                    else
                    {
                        throw new Exception($"登录失败: HTTP {response.StatusCode} - {errorContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                SetAuthToken(null);
                System.Diagnostics.Debug.WriteLine($"💥 登录异常: {ex.Message}");
                throw;
            }
            
            throw new Exception("登录失败: 未知错误");
        }

        public async Task<User> LoginAsync(LoginRequestDto loginRequest)
        {
            string csrfToken = string.Empty;
            
            try
            {
                // Step 1: 尝试获取CSRF token（静默失败，不影响登录流程）
                try
                {
                    csrfToken = await GetCsrfTokenAsync();
                    if (!string.IsNullOrEmpty(csrfToken))
                    {
                        SetCsrfToken(csrfToken);
                    }
                }
                catch (Exception)
                {
                    // CSRF token获取失败，继续登录流程
                    // 某些后端可能不需要CSRF token
                }

                // Step 2: 登录获取认证token
                var tokenData = await PostAsync<LoginRequestDto, LoginToken>("/users/login", loginRequest);
                if (tokenData == null || string.IsNullOrEmpty(tokenData.Token))
                {
                    throw new Exception("登录失败: 未返回认证token。");
                }
                
                // Step 3: 设置认证token用于后续请求
                SetAuthToken(tokenData.Token);

                // Step 4: 如果登录接口返回了新的CSRF token，则更新
                if (!string.IsNullOrEmpty(tokenData.CsrfToken))
                {
                    SetCsrfToken(tokenData.CsrfToken);
                    csrfToken = tokenData.CsrfToken;
                }

                // Step 5: 获取用户信息
                var userInfo = await GetAsync<User>("/users/userInfo");
                if (userInfo == null)
                {
                    throw new Exception("登录失败: 无法获取用户信息。");
                }

                // Step 6: 合并token和用户信息
                userInfo.Token = tokenData.Token;
                userInfo.CsrfToken = csrfToken;
                return userInfo;
            }
            catch (Exception ex)
            {
                // 登录失败时清除token
                SetAuthToken(null);
                SetCsrfToken(null);
                
                // 检查是否为认证相关错误，提供更友好的错误信息
                if (ex.Message.Contains("401") || ex.Message.Contains("Unauthorized"))
                {
                    throw new Exception("登录失败: 用户名或密码错误。");
                }
                else if (ex.Message.Contains("403") || ex.Message.Contains("Forbidden"))
                {
                    throw new Exception("登录失败: 账户被禁用或权限不足。");
                }
                else if (ex.Message.Contains("404") || ex.Message.Contains("Not Found"))
                {
                    throw new Exception("登录失败: 服务器接口不存在，请检查服务器配置。");
                }
                else if (ex.Message.Contains("500") || ex.Message.Contains("Internal Server Error"))
                {
                    throw new Exception("登录失败: 服务器内部错误，请稍后重试。");
                }
                else if (ex.Message.Contains("网络") || ex.Message.Contains("连接") || ex.Message.Contains("timeout"))
                {
                    throw new Exception("登录失败: 网络连接错误，请检查网络设置。");
                }
                else
                {
                    throw new Exception($"登录失败: {ex.Message}");
                }
            }
        }

        public async Task RegisterAsync(RegisterRequestDto registerRequest)
        {
            try
            {
                // Step 1: 尝试获取CSRF token（静默失败，不影响注册流程）
                if (!_httpClient.DefaultRequestHeaders.Contains("X-CSRF-TOKEN"))
                {
                    try
                    {
                        var csrfToken = await GetCsrfTokenAsync();
                        if (!string.IsNullOrEmpty(csrfToken))
                        {
                            SetCsrfToken(csrfToken);
                        }
                    }
                    catch (Exception)
                    {
                        // CSRF token获取失败，继续注册流程
                        // 某些后端可能不需要CSRF token
                    }
                }

                // Step 2: 注册用户
                await PostAsync("/api/users/register", registerRequest);
            }
            catch (Exception ex)
            {
                // 提供更友好的错误信息
                if (ex.Message.Contains("409") || ex.Message.Contains("Conflict"))
                {
                    throw new Exception("注册失败: 用户名已存在。");
                }
                else if (ex.Message.Contains("400") || ex.Message.Contains("Bad Request"))
                {
                    throw new Exception("注册失败: 请求参数有误，请检查输入信息。");
                }
                else if (ex.Message.Contains("404") || ex.Message.Contains("Not Found"))
                {
                    throw new Exception("注册失败: 服务器接口不存在，请检查服务器配置。");
                }
                else if (ex.Message.Contains("500") || ex.Message.Contains("Internal Server Error"))
                {
                    throw new Exception("注册失败: 服务器内部错误，请稍后重试。");
                }
                else
                {
                    throw new Exception($"注册失败: {ex.Message}");
                }
            }
        }

        public void Logout()
        {
            SetAuthToken(null);
            SetCsrfToken(null);
        }

        public async Task UpdatePasswordAsync(object passwordData)
        {
            // The backend expects fields: old_pwd, new_pwd, re_pwd
            // Using an anonymous object for simplicity. A DTO would be better.
            await _httpClient.PatchAsJsonAsync("/api/users/updatePwd", passwordData);
        }

        // Research Platform APIs

        // Teacher APIs
        public async Task<PagedResult<ResearchProject>> GetTeacherProjectsAsync(int page = 1, int size = 10, string keyword = null, string projectType = null, string status = null)
        {
            var queryParams = new List<string> { $"page={page}", $"size={size}" };
            if (!string.IsNullOrEmpty(keyword)) queryParams.Add($"keyword={keyword}");
            if (!string.IsNullOrEmpty(projectType)) queryParams.Add($"projectType={projectType}");
            if (!string.IsNullOrEmpty(status)) queryParams.Add($"status={status}");
            
            var query = string.Join("&", queryParams);
            return await GetAsync<PagedResult<ResearchProject>>($"/api/teacher/research/projects?{query}");
        }

        public async Task<ResearchProject> GetTeacherProjectDetailsAsync(int projectId)
        {
            return await GetAsync<ResearchProject>($"/api/teacher/research/projects/{projectId}");
        }

        public async Task<PagedResult<ResearchApplication>> GetPendingApplicationsAsync(int page = 1, int size = 10, int? projectId = null)
        {
            var query = $"page={page}&size={size}";
            if (projectId.HasValue) query += $"&projectId={projectId.Value}";
            return await GetAsync<PagedResult<ResearchApplication>>($"/api/teacher/research/applications?{query}");
        }

        public async Task<PagedResult<ResearchTask>> GetTeacherTasksAsync(int page = 1, int size = 10, int? projectId = null)
        {
            var query = $"page={page}&size={size}";
            if (projectId.HasValue) query += $"&projectId={projectId.Value}";
            return await GetAsync<PagedResult<ResearchTask>>($"/api/teacher/research/tasks?{query}");
        }

        public async Task<PagedResult<ResearchSubmission>> GetPendingSubmissionsAsync(int page = 1, int size = 10)
        {
            return await GetAsync<PagedResult<ResearchSubmission>>($"/api/teacher/research/submissions/pending?page={page}&size={size}");
        }

        // Student APIs
        public async Task<PagedResult<ResearchProject>> GetAvailableProjectsAsync(int page = 1, int size = 10, string keyword = null, string projectType = null, string researchField = null)
        {
            var queryParams = new List<string> { $"page={page}", $"size={size}" };
            if (!string.IsNullOrEmpty(keyword)) queryParams.Add($"keyword={keyword}");
            if (!string.IsNullOrEmpty(projectType)) queryParams.Add($"projectType={projectType}");
            if (!string.IsNullOrEmpty(researchField)) queryParams.Add($"researchField={researchField}");
            
            var query = string.Join("&", queryParams);
            return await GetAsync<PagedResult<ResearchProject>>($"/api/student/research/projects/available?{query}");
        }

        public async Task<ResearchProject> GetStudentProjectDetailsAsync(int projectId)
        {
            return await GetAsync<ResearchProject>($"/api/student/research/projects/{projectId}");
        }

        public async Task<List<ResearchApplication>> GetMyApplicationsAsync()
        {
            return await GetAsync<List<ResearchApplication>>("/api/student/research/applications");
        }

        public async Task<PagedResult<ResearchTask>> GetMyTasksAsync(int page = 1, int size = 10)
        {
            return await GetAsync<PagedResult<ResearchTask>>($"/api/student/research/tasks?page={page}&size={size}");
        }

        public async Task<List<ResearchSubmission>> GetMySubmissionsAsync()
        {
            return await GetAsync<List<ResearchSubmission>>("/api/student/research/submissions");
        }

        public async Task<int> SubmitApplicationAsync(int projectId, string applicationReason)
        {
            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("projectId", projectId.ToString()),
                new KeyValuePair<string, string>("applicationReason", applicationReason)
            };
            
            var formContent = new FormUrlEncodedContent(formData);
            var response = await _httpClient.PostAsync("/api/student/research/applications", formContent);
            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<int>>();
            
            // 检查是否为成功状态码
            var isSuccess = apiResponse.Code == 20000 || apiResponse.Code == 20051 || 
                           (apiResponse.Code >= 20000 && apiResponse.Code < 21000 && apiResponse.Msg?.Contains("成功") == true);
            
            if (!isSuccess)
            {
                throw new Exception($"API Error: {apiResponse.Msg} (Code: {apiResponse.Code})");
            }
            return apiResponse.Data;
        }

        public async Task<int> SubmitPaperAsync(ResearchSubmission submission)
        {
            return await PostAsync<ResearchSubmission, int>("/api/student/research/submissions", submission);
        }

        public async Task ReviewApplicationAsync(ReviewRequestDto reviewRequest)
        {
            await PostAsync<ReviewRequestDto, object>("/api/teacher/research/applications/review", reviewRequest);
        }

        #region Education Platform APIs

        public async Task<List<EduCategory>> GetEduCategoriesAsync()
        {
            // Corrected endpoint from API documentation
            return await GetAsync<List<EduCategory>>("/api/categories");
        }

        public async Task<EduPagedResult<EduResource>> GetEduResourcesAsync(int page = 0, int size = 10, string title = null, string categoryId = null)
        {
            var queryParams = new List<string> { $"page={page}", $"size={size}", "sort=createdAt,DESC" };
            if (!string.IsNullOrEmpty(title))
            {
                queryParams.Add($"title={Uri.EscapeDataString(title)}");
            }
            if (!string.IsNullOrEmpty(categoryId))
            {
                queryParams.Add($"categoryId={categoryId}");
            }
            var endpoint = $"/api/resources?{string.Join("&", queryParams)}";
            return await GetAsync<EduPagedResult<EduResource>>(endpoint);
        }

        public async Task<EduResourceDetail> GetResourceDetailAsync(long resourceId)
        {
            var endpoint = $"/api/resources/{resourceId}";
            return await GetAsync<EduResourceDetail>(endpoint);
        }

        // 视频相关方法
        public async Task<VideoPagedResult> GetVideosAsync(int pageNum = 1, int pageSize = 10, string status = null)
        {
            var queryParams = new List<string> { $"pageNum={pageNum}", $"pageSize={pageSize}" };
            if (!string.IsNullOrEmpty(status))
            {
                queryParams.Add($"status={status}");
            }
            var endpoint = $"/api/videos/page?{string.Join("&", queryParams)}";
            return await GetAsync<VideoPagedResult>(endpoint);
        }

        public async Task<VideoDto> GetVideoDetailAsync(long videoId)
        {
            var endpoint = $"/api/videos/{videoId}";
            return await GetAsync<VideoDto>(endpoint);
        }

        #endregion

        // 获取所有任务 - 合并教师和学生的任务
        public async Task<PagedResult<ResearchTask>> GetAllTasksAsync(int page = 1, int size = 10)
        {
            try
            {
                // 优先尝试获取学生任务，如果失败则获取教师任务
                return await GetAsync<PagedResult<ResearchTask>>($"/api/student/research/tasks?page={page}&size={size}");
            }
            catch
            {
                try
                {
                    return await GetAsync<PagedResult<ResearchTask>>($"/api/teacher/research/tasks?page={page}&size={size}");
                }
                catch
                {
                    // 如果都失败，返回空结果
                    return new PagedResult<ResearchTask>
                    {
                        Records = new List<ResearchTask>(),
                        Total = 0,
                        Size = size,
                        Current = page,
                        Pages = 0
                    };
                }
            }
        }

        // 获取所有论文提交 - 合并教师和学生的论文
        public async Task<PagedResult<ResearchSubmission>> GetAllSubmissionsAsync(int page = 1, int size = 10)
        {
            try
            {
                // 优先尝试获取学生论文，如果失败则获取教师论文
                var studentSubmissions = await GetMySubmissionsAsync();
                var result = new PagedResult<ResearchSubmission>
                {
                    Records = studentSubmissions,
                    Total = studentSubmissions.Count,
                    Size = size,
                    Current = page,
                    Pages = (int)Math.Ceiling((double)studentSubmissions.Count / size)
                };
                return result;
            }
            catch
            {
                try
                {
                    return await GetAsync<PagedResult<ResearchSubmission>>($"/api/teacher/research/submissions/pending?page={page}&size={size}");
                }
                catch
                {
                    // 如果都失败，返回空结果
                    return new PagedResult<ResearchSubmission>
                    {
                        Records = new List<ResearchSubmission>(),
                        Total = 0,
                        Size = size,
                        Current = page,
                        Pages = 0
                    };
                }
            }
        }

        // === 方剂管理相关API ===

        /// <summary>
        /// 确保方剂API有必要的认证信息
        /// </summary>
        private async Task EnsureFormulaApiTokenAsync()
        {
            // 检查是否有必要的认证信息
            if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                throw new Exception("请先登录后再使用方剂管理功能");
            }

            // CSRF token 已在登录时设置，这里只需要确认即可
            await Task.CompletedTask;
        }

        /// <summary>
        /// 分页查询方剂列表
        /// </summary>
        public async Task<FormulaPagedResult> GetFormulasAsync(int page = 1, int size = 12, string keyword = null, string source = null, int? categoryId = null)
        {
            try
            {
                // 确保方剂API有CSRF token
                await EnsureFormulaApiTokenAsync();
                
                var queryParams = new List<string>();
                queryParams.Add($"page={page}");
                queryParams.Add($"size={size}");
                
                if (!string.IsNullOrWhiteSpace(keyword))
                    queryParams.Add($"keyword={Uri.EscapeDataString(keyword)}");
                
                if (!string.IsNullOrWhiteSpace(source))
                    queryParams.Add($"source={Uri.EscapeDataString(source)}");
                
                if (categoryId.HasValue)
                    queryParams.Add($"categoryId={categoryId.Value}");

                var endpoint = $"/api/formula/page?{string.Join("&", queryParams)}";
                return await GetAsync<FormulaPagedResult>(endpoint);
            }
            catch (Exception ex)
            {
                throw new Exception($"获取方剂列表失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 获取方剂详情
        /// </summary>
        public async Task<Formula> GetFormulaDetailAsync(long formulaId)
        {
            try
            {
                await EnsureFormulaApiTokenAsync();
                var endpoint = $"/api/formula/{formulaId}";
                return await GetAsync<Formula>(endpoint);
            }
            catch (Exception ex)
            {
                throw new Exception($"获取方剂详情失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 基于症状推荐方剂
        /// </summary>
        public async Task<List<FormulaRecommendation>> GetFormulaRecommendationsAsync(List<string> symptoms)
        {
            try
            {
                await EnsureFormulaApiTokenAsync();
                var request = new SymptomRequest { Symptoms = symptoms };
                var endpoint = "/api/formula/recommend";
                return await PostAsync<SymptomRequest, List<FormulaRecommendation>>(endpoint, request);
            }
            catch (Exception ex)
            {
                throw new Exception($"获取方剂推荐失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 分析中药组合配伍规律
        /// </summary>
        public async Task<List<HerbCombination>> GetHerbCombinationsAsync(string herbName)
        {
            try
            {
                await EnsureFormulaApiTokenAsync();
                var endpoint = $"/api/formula/analysis/herb-combinations?herbName={Uri.EscapeDataString(herbName)}";
                return await GetAsync<List<HerbCombination>>(endpoint);
            }
            catch (Exception ex)
            {
                throw new Exception($"获取中药配伍分析失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 比较多个方剂
        /// </summary>
        public async Task<FormulaComparison> CompareFormulasAsync(List<long> formulaIds)
        {
            try
            {
                await EnsureFormulaApiTokenAsync();
                var endpoint = "/api/formula/compare";
                return await PostAsync<List<long>, FormulaComparison>(endpoint, formulaIds);
            }
            catch (Exception ex)
            {
                throw new Exception($"方剂比较失败: {ex.Message}", ex);
            }
        }
    }
} 