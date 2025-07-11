using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using biomed.Models;
using biomed.Services;
using System;
using System.Linq;

namespace biomed.Views
{
    public sealed partial class ArticleDetailPage : Page
    {
        public EduResourceDetail ResourceDetail { get; private set; }
        public bool HasVideos => ResourceDetail?.Videos?.Any() == true;

        private readonly ApiClient _apiClient;

        public ArticleDetailPage()
        {
            this.InitializeComponent();
            _apiClient = App.GetService<ApiClient>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            try
            {
                if (e.Parameter is long resourceId)
                {
                    await LoadResourceDetailAsync(resourceId);
                }
                else if (e.Parameter is EduResource resource)
                {
                    await LoadResourceDetailAsync(resource.Id);
                }
                else
                {
                    ShowError("无法加载资源信息：参数无效");
                }
            }
            catch (Exception ex)
            {
                ShowError($"初始化资源详情页面失败: {ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task LoadResourceDetailAsync(long resourceId)
        {
            try
            {
                // 显示加载状态
                // 如果需要加载指示器，可以在这里添加

                // 获取资源详情
                ResourceDetail = await _apiClient.GetResourceDetailAsync(resourceId);
                
                // 触发属性变更通知
                Bindings.Update();

                // 加载HTML内容到WebView
                await LoadHtmlContentAsync();
            }
            catch (Exception ex)
            {
                ShowError($"加载资源详情失败: {ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task LoadHtmlContentAsync()
        {
            try
            {
                // 确保WebView2已经初始化
                await ContentWebView.EnsureCoreWebView2Async();

                if (ResourceDetail != null && !string.IsNullOrWhiteSpace(ResourceDetail.Content))
                {
                    // 创建完整的HTML页面，包含样式
                    var htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            margin: 0;
            padding: 20px;
            background-color: transparent;
            color: #333;
        }}
        h1, h2, h3, h4, h5, h6 {{
            color: #2c3e50;
            margin-top: 1.5em;
            margin-bottom: 0.5em;
        }}
        p {{
            margin-bottom: 1em;
        }}
        img {{
            max-width: 100%;
            height: auto;
            border-radius: 4px;
        }}
        blockquote {{
            border-left: 4px solid #3498db;
            margin: 1em 0;
            padding-left: 1em;
            background-color: #f8f9fa;
            font-style: italic;
        }}
        code {{
            background-color: #f4f4f4;
            padding: 2px 4px;
            border-radius: 3px;
            font-family: 'Consolas', 'Monaco', monospace;
        }}
        pre {{
            background-color: #f4f4f4;
            padding: 1em;
            border-radius: 4px;
            overflow-x: auto;
        }}
        table {{
            border-collapse: collapse;
            width: 100%;
            margin: 1em 0;
        }}
        th, td {{
            border: 1px solid #ddd;
            padding: 8px;
            text-align: left;
        }}
        th {{
            background-color: #f2f2f2;
        }}
    </style>
</head>
<body>
    {ResourceDetail.Content}
</body>
</html>";

                    ContentWebView.NavigateToString(htmlContent);
                }
                else
                {
                    ContentWebView.NavigateToString("<html><body><p>暂无内容</p></body></html>");
                }
            }
            catch (Exception ex)
            {
                ShowError($"加载文章内容失败: {ex.Message}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var navigationService = App.GetService<INavigationService>();
                navigationService.NavigateTo(typeof(EducationPage));
            }
            catch (Exception ex)
            {
                ShowError($"返回教学平台失败: {ex.Message}");
            }
        }

        private async void PlayVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is EduVideo eduVideo)
                {
                    // 通过VideoId获取完整的视频信息
                    var videoDetail = await _apiClient.GetVideoDetailAsync(eduVideo.VideoId);
                    
                    var navigationService = App.GetService<INavigationService>();
                    navigationService.Navigate(typeof(VideoPlayerPage), videoDetail);
                }
            }
            catch (Exception ex)
            {
                ShowError($"播放视频失败: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            try
            {
                if (ErrorInfoBar != null)
                {
                    ErrorInfoBar.Message = message;
                    ErrorInfoBar.IsOpen = true;
                }
            }
            catch
            {
                // 忽略错误信息显示失败的情况
            }
        }

        public string FormatDate(DateTime dateTime)
        {
            try
            {
                return dateTime.ToString("yyyy-MM-dd");
            }
            catch
            {
                return "未知日期";
            }
        }
    }
} 